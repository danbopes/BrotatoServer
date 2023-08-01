class_name Weapon
extends Node2D

signal tracked_value_updated

const DETECTION_RANGE: = 200

export (Resource) var outline_shader_mat

var weapon_pos: = - 1
var nb_enemies_killed_this_wave: = 0
var stats:Resource
var index: = 0

var effects: = []
var current_stats: = WeaponStats.new()
var weapon_id:String = ""
var weapon_sets:Array = []
var tier:int = 0

var _parent:Node;

var _idle_angle: = 0.0
var _current_idle_angle: = _idle_angle
var _current_cooldown:float = 0
var _is_shooting: = false
var _nb_shots_taken: = 0

var _current_shoot_spread: = 0.0
var _current_target: = []
var _targets_in_range: = []
var _original_sprite = null

onready var muzzle:Position2D = $Sprite / Muzzle
onready var tween:Tween = $Tween
onready var sprite:Sprite = $Sprite
onready var _hitbox:Area2D = $Sprite / Hitbox
onready var _attach:Position2D = $Attach
onready var _range:Area2D = $Range
onready var _range_shape:CollisionShape2D = $Range / CollisionShape2D
onready var _shooting_behavior:WeaponShootingBehavior = $ShootingBehavior


func _ready()->void :
	_original_sprite = sprite.texture
	_parent = get_parent().get_parent()
	disable_hitbox()
	var _behavior = _shooting_behavior.init(self)
	
	init_stats()
	
	update_highlighting(ProgressData.settings.weapon_highlighting)
	
	if effects.size() > 0:
		var _killed_sthing = _hitbox.connect("killed_something", self, "on_killed_something")
		var _added_gold_on_crit_kill = _hitbox.connect("added_gold_on_crit", self, "on_added_gold_on_crit")


func update_highlighting(value:int)->void :
	if tier > 0 and value:
		outline_shader_mat.set_shader_param("outline_color", ItemService.get_color_from_tier(tier))
		sprite.material = outline_shader_mat
	else :
		sprite.material = null


func init_stats(at_wave_begin:bool = true)->void :
	if stats is RangedWeaponStats:
		current_stats = WeaponService.init_ranged_stats(stats, weapon_id, weapon_sets, effects)
	else :
		current_stats = WeaponService.init_melee_stats(stats, weapon_id, weapon_sets, effects)
	
	_hitbox.projectiles_on_hit = []
		
	for effect in effects:
		if effect is ProjectilesOnHitEffect:
			var weapon_stats = WeaponService.init_ranged_stats(effect.weapon_stats)
			set_projectile_on_hit(effect.value, weapon_stats, effect.auto_target_enemy)
	
	current_stats.burning_data = current_stats.burning_data.duplicate()
	current_stats.burning_data.from = self
	
	_hitbox.effect_scale = current_stats.effect_scale
	_hitbox.set_damage(current_stats.damage, current_stats.accuracy, current_stats.crit_chance, current_stats.crit_damage, current_stats.burning_data, current_stats.is_healing)
	_hitbox.effects = effects
	_hitbox.from = self
	
	if at_wave_begin:
		_current_cooldown = current_stats.cooldown
	
	_range_shape.shape.radius = current_stats.max_range + DETECTION_RANGE


func _process(_delta:float)->void :
	update_sprite_flipv()
	update_idle_angle()


func attach(attach_to:Vector2, attach_idle_angle:float)->void :
	position = attach_to - _attach.position
	_idle_angle = attach_idle_angle


func _physics_process(delta:float)->void :

	if ProgressData.is_manual_aim():
		if should_rotate_manual():
			rotation = (get_global_mouse_position() - global_position).angle()
	else :
		if _is_shooting:
			rotation = get_direction()
		else :
			rotation = get_direction_and_calculate_target()
	
	if not _is_shooting:
		_current_cooldown = max(_current_cooldown - Utils.physics_one(delta), 0)
	
	if _current_cooldown <= 10 and sprite.texture == stats.custom_on_cooldown_sprite:
		sprite.texture = _original_sprite
	
	if should_shoot():
		shoot()


func on_killed_something(_thing_killed:Node)->void :
	nb_enemies_killed_this_wave += 1
	
	for effect in effects:
		if effect is GainStatEveryKilledEnemiesEffect and nb_enemies_killed_this_wave % effect.value == 0:
			RunData.add_stat(effect.stat, effect.stat_nb)
			emit_signal("tracked_value_updated")


func on_added_gold_on_crit(_gold_added:int)->void :
	for effect in effects:
		if effect.key == "gold_on_crit_kill":
			emit_signal("tracked_value_updated")


func get_max_range()->int:
	return current_stats.max_range + 50


func get_direction_and_calculate_target()->float:

	if _targets_in_range.size() == 0:
		return rotation if _is_shooting else _current_idle_angle

	_current_target = Utils.get_nearest(_targets_in_range, global_position, current_stats.min_range)

	if _current_target.size() == 0:
		return rotation if _is_shooting else _current_idle_angle

	var direction_to_target = (_current_target[0].global_position - global_position).angle()
	return direction_to_target


func get_direction()->float:
	if _current_target.size() == 0 or not is_instance_valid(_current_target[0]):
		return rotation if _is_shooting else get_direction_and_calculate_target()
	else :
		var direction_to_target = (_current_target[0].global_position - global_position).angle()
		return direction_to_target + _current_shoot_spread


func should_shoot()->bool:
	return (_current_cooldown == 0
		 and (
			RunData.effects["can_attack_while_moving"]
			 or 
			_parent._current_movement == Vector2.ZERO
		)
		 and 
		(
			(
				_current_target.size() > 0
				 and is_instance_valid(_current_target[0])
				 and Utils.is_between(_current_target[1], current_stats.min_range, get_max_range())
			)
			 or (
				ProgressData.is_manual_aim()
				 and not _parent.cleaning_up
			)
		)
	)


func shoot()->void :
	_nb_shots_taken += 1
	update_current_spread()
	_hitbox.set_knockback( - Vector2(cos(rotation), sin(rotation)), current_stats.knockback)
	
	if ProgressData.is_manual_aim():
		_shooting_behavior.shoot(current_stats.max_range)
	else :
		_shooting_behavior.shoot(_current_target[1])
	
	var is_big_reload = current_stats.additional_cooldown_every_x_shots != - 1 and _nb_shots_taken % current_stats.additional_cooldown_every_x_shots == 0
	
	if is_big_reload:
		_current_cooldown = current_stats.cooldown * current_stats.additional_cooldown_multiplier
	else :
		var max_rand = min(_parent.get_nb_weapons() * current_stats.cooldown / 5.0, _parent.get_nb_weapons() * 5)
		_current_cooldown = rand_range(max(1, current_stats.cooldown - max_rand), current_stats.cooldown + max_rand)
	
	if ((current_stats.additional_cooldown_every_x_shots != - 1 and is_big_reload) or current_stats.additional_cooldown_every_x_shots == - 1) and stats.custom_on_cooldown_sprite != null:
		sprite.texture = stats.custom_on_cooldown_sprite


func update_current_spread()->void :
	_current_shoot_spread = rand_range( - (1 - current_stats.accuracy), 1 - current_stats.accuracy)
	rotation += _current_shoot_spread


func set_shooting(value:bool)->void :
	_is_shooting = value


func disable_hitbox()->void :
	_hitbox.ignored_objects.clear()
	_hitbox.disable()


func enable_hitbox()->void :
	_hitbox.enable()


func update_sprite_flipv()->void :
	if Utils.is_facing_right(rotation_degrees):
		sprite.flip_v = false
	else :
		sprite.flip_v = true


func update_idle_angle()->void :
	if _parent.get_direction() == 1:
		_current_idle_angle = _idle_angle
	else :
		_current_idle_angle = PI - _idle_angle


func _on_Range_body_entered(body:Node)->void :
	_targets_in_range.push_back(body)
	var _error = body.connect("died", self, "on_target_died")


func _on_Range_body_exited(body:Node)->void :
	_targets_in_range.erase(body)
	if _current_target.size() > 0 and body == _current_target[0]:
		_current_target.clear()
	body.disconnect("died", self, "on_target_died")


func on_target_died(target:Node)->void :
	_targets_in_range.erase(target)
	if _current_target.size() > 0 and target == _current_target[0]:
		_current_target.clear()


func _on_Hitbox_hit_something(thing_hit:Node, damage_dealt:int)->void :
	RunData.manage_life_steal(current_stats)
	
	_hitbox.ignored_objects.push_back(thing_hit)
	
	for effect in effects:
		if effect is ExplodingEffect and randf() < effect.chance:
			var _inst = WeaponService.explode(effect, thing_hit.global_position, _hitbox.damage, _hitbox.accuracy, _hitbox.crit_chance, _hitbox.crit_damage, _hitbox.burning_data, _hitbox.is_healing, [thing_hit])
	
	on_weapon_hit_something(thing_hit, damage_dealt)


func on_weapon_hit_something(_thing_hit:Node, damage_dealt:int)->void :
	RunData.add_weapon_dmg_dealt(weapon_pos, damage_dealt)


func set_projectile_on_hit(nb:int, weapon_stats:RangedWeaponStats, auto_target_enemy:bool)->void :
	_hitbox.projectiles_on_hit = [nb, weapon_stats, auto_target_enemy]


func should_rotate_manual()->bool:
	return true
