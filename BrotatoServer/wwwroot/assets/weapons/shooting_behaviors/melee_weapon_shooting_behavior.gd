class_name MeleeWeaponShootingBehavior
extends WeaponShootingBehavior

const MIN_SWEEP_DISTANCE = 250

var shooting_data:ShootingData


func shoot(distance:float)->void :
	var initial_position:Vector2 = _parent.sprite.position
	shooting_data = MeleeShootingData.new(_parent.current_stats)
	
	_parent.set_shooting(true)
	
	if _parent.next_attack_type == MeleeAttackType.THRUST:
		melee_thrust_attack(initial_position)
	else :
		melee_sweep_attack(initial_position, distance)


func interpolate(property:String, val_a, val_b, duration, tween_trans = Tween.TRANS_EXPO, tween_ease = Tween.EASE_OUT, object = _parent.sprite)->void :
	_parent.tween.interpolate_property(
		object, 
		property, 
		val_a, 
		val_b, 
		duration, 
		tween_trans, 
		tween_ease
	)


func melee_sweep_attack(initial_position:Vector2, distance:float)->void :
	
	var atk_distance = min(_parent.current_stats.max_range, max(MIN_SWEEP_DISTANCE, distance))
	
	shooting_data.update_atk_duration(atk_distance)
	
	var side_range = atk_distance / 2
	var recoil = _parent.current_stats.recoil
	var recoil_duration = _parent.current_stats.recoil_duration
	var sweep_angle = 0.9 * PI
	var sweep_half_duration = shooting_data.atk_duration / 4
	
	var side_a = side_range
	var side_b = - side_range
	var angle_a = sweep_angle
	var angle_b = - sweep_angle
	
	if not _parent.sprite.flip_v:
		side_a = - side_range
		side_b = side_range
		angle_a = - sweep_angle
		angle_b = sweep_angle
	
	interpolate("position", initial_position, Vector2(initial_position.x - recoil, initial_position.y + side_a), recoil_duration)
	interpolate("rotation", _parent.sprite.rotation, angle_a, recoil_duration)
	
	_parent.tween.start()
	yield (_parent.tween, "tween_all_completed")
	
	SoundManager.play(Utils.get_rand_element(_parent.current_stats.shooting_sounds), _parent.current_stats.sound_db_mod, 0.2)
	_parent.enable_hitbox()
	
	interpolate("position", _parent.sprite.position, Vector2(initial_position.x + _parent.get_sweep_range(atk_distance) * 0.75, initial_position.y), sweep_half_duration, Tween.TRANS_LINEAR)
	interpolate("rotation", _parent.sprite.rotation, 0, sweep_half_duration, Tween.TRANS_LINEAR)
	
	_parent.tween.start()
	yield (_parent.tween, "tween_all_completed")
	
	interpolate("position", _parent.sprite.position, Vector2(initial_position.x - recoil, initial_position.y + side_b), sweep_half_duration, Tween.TRANS_LINEAR)
	interpolate("rotation", _parent.sprite.rotation, angle_b, sweep_half_duration, Tween.TRANS_LINEAR)
	
	_parent.tween.start()
	yield (_parent.tween, "tween_all_completed")
	
	if not _parent.stats.deal_dmg_on_return:
		_parent.disable_hitbox()
	
	interpolate("position", _parent.sprite.position, initial_position, shooting_data.back_duration)
	interpolate("rotation", _parent.sprite.rotation, 0, shooting_data.back_duration)

	_parent.tween.start()
	yield (_parent.tween, "tween_all_completed")
	
	if _parent.stats.deal_dmg_on_return:
		_parent.disable_hitbox()
	
	_parent.set_shooting(false)


func melee_thrust_attack(initial_position:Vector2)->void :
	
	var recoil = _parent.current_stats.recoil
	var recoil_duration = _parent.current_stats.recoil_duration
	var thrust_half_duration = shooting_data.atk_duration / 2
	
	interpolate("position", initial_position, Vector2(initial_position.x - recoil, initial_position.y), recoil_duration)
	
	_parent.tween.start()
	yield (_parent.tween, "tween_all_completed")
	
	SoundManager.play(Utils.get_rand_element(_parent.current_stats.shooting_sounds), _parent.current_stats.sound_db_mod, 0.2)
	_parent.enable_hitbox()
	
	interpolate("position", _parent.sprite.position, Vector2(initial_position.x + _parent.current_stats.max_range, initial_position.y), thrust_half_duration)
	
	_parent.tween.start()
	yield (_parent.tween, "tween_all_completed")
	
	if not _parent.stats.deal_dmg_on_return:
		_parent.disable_hitbox()
		
	interpolate("position", _parent.sprite.position, initial_position, shooting_data.back_duration)
	
	_parent.tween.start()
	yield (_parent.tween, "tween_all_completed")
	
	if _parent.stats.deal_dmg_on_return:
		_parent.disable_hitbox()
	
	_parent.set_shooting(false)
