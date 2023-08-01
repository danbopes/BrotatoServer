class_name MeleeShootingData
extends ShootingData

const BASE_ATK_DURATION = 0.2

var range_factor:float
var recoil_duration:float
var back_duration:float


func _init(from_stats:Resource = null, use_global_stats:bool = true)->void :
	atk_spd = (Utils.get_stat("stat_attack_speed") + from_stats.attack_speed_mod) / 100.0 if use_global_stats else 0
	range_factor = get_range_factor(from_stats.max_range)
	atk_duration = get_atk_duration()
	recoil_duration = from_stats.recoil_duration
	
	if atk_spd > 0:
		back_duration = BASE_ATK_DURATION / (1 + (atk_spd * 3))
	else :
		back_duration = BASE_ATK_DURATION


func get_range_factor(distance:float)->float:
	return max(0.0, distance / clamp(70.0 * (1 + (atk_spd / 3)), 70.0, 120.0))


func get_atk_duration()->float:
	return max(0.01, BASE_ATK_DURATION - (atk_spd / 10.0)) + range_factor * 0.15


func get_shooting_total_duration()->float:
	return (atk_duration / 2) + back_duration + recoil_duration


func update_atk_duration(distance_to_target:float)->void :
	range_factor = get_range_factor(distance_to_target)
	atk_duration = get_atk_duration()
