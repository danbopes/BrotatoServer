class_name RangedShootingData
extends ShootingData


func _init(from_stats:Resource = null, use_global_stats:bool = true)->void :
	atk_spd = (Utils.get_stat("stat_attack_speed") + from_stats.attack_speed_mod) / 100.0 if use_global_stats else 0
	atk_duration = get_atk_duration(from_stats)


func get_atk_duration(from_stats:Resource)->float:
	return from_stats.recoil_duration * 2


func get_shooting_total_duration()->float:
	return atk_duration
