class_name RangedWeaponStats
extends WeaponStats

export (int) var nb_projectiles: = 1
export (float) var projectile_spread: = 0.0
export (int) var piercing: = 0
export (float, 0, 1, 0.05) var piercing_dmg_reduction: = 0.5
export (int) var bounce: = 0
export (float, 0, 1, 0.05) var bounce_dmg_reduction: = 0.5
export (int) var projectile_speed: = 3000
export (bool) var increase_projectile_speed_with_range: = false
export (PackedScene) var projectile_scene = null


func get_bouncing_text(base_stats:Resource)->String:
	if bounce <= 0:return ""
	
	var a = get_col_a(bounce, base_stats.bounce)
	var aa = get_col_a( - bounce_dmg_reduction, - base_stats.bounce_dmg_reduction)
	
	return "\n" + Text.text("PIERCING_FORMATTED", [col_a + tr("BOUNCE") + col_b, a + str(bounce) + col_b, aa + str(round(bounce_dmg_reduction * 100.0)) + col_b])


func get_piercing_text(base_stats:Resource)->String:
	if piercing <= 0:return ""
	
	var a = get_col_a(piercing, base_stats.piercing)
	var aa = get_col_a( - piercing_dmg_reduction, - base_stats.piercing_dmg_reduction)
	
	return "\n" + Text.text("PIERCING_FORMATTED", [col_a + tr("PIERCING") + col_b, a + str(piercing) + col_b, aa + str(round(piercing_dmg_reduction * 100.0)) + col_b])


func get_damage_text(base_stats:Resource)->String:
	return get_dmg_text_with_scaling_stats(base_stats, scaling_stats, nb_projectiles)


func get_base_cooldown_value(base_stats:Resource)->float:
	var base_shooting_data = RangedShootingData.new(base_stats, false)
	return base_shooting_data.get_shooting_total_duration() + base_stats.cooldown / 60.0


func get_cooldown_value(_base_stats:Resource)->float:
	var current_shooting_data = RangedShootingData.new(self)
	return current_shooting_data.get_shooting_total_duration() + cooldown / 60.0


func get_type_text()->String:
	return tr("RANGED")


func serialize()->Dictionary:
	var serialized = .serialize()
	
	serialized.type = "ranged"
	serialized.nb_projectiles = nb_projectiles
	serialized.projectile_spread = projectile_spread
	serialized.piercing = piercing
	serialized.piercing_dmg_reduction = piercing_dmg_reduction
	serialized.bounce = bounce
	serialized.bounce_dmg_reduction = bounce_dmg_reduction
	serialized.projectile_speed = projectile_speed
	serialized.increase_projectile_speed_with_range = increase_projectile_speed_with_range
	
	if projectile_scene != null:
		serialized.projectile_scene = projectile_scene.resource_path
	
	return serialized


func deserialize_and_merge(serialized:Dictionary):
	.deserialize_and_merge(serialized)
	
	nb_projectiles = serialized.nb_projectiles as int
	projectile_spread = serialized.projectile_spread
	piercing = serialized.piercing as int
	piercing_dmg_reduction = serialized.piercing_dmg_reduction
	bounce = serialized.bounce as int
	bounce_dmg_reduction = serialized.bounce_dmg_reduction
	projectile_speed = serialized.projectile_speed as int
	increase_projectile_speed_with_range = serialized.increase_projectile_speed_with_range
	
	if serialized.has("projectile_scene"):
		projectile_scene = load(serialized.projectile_scene)
