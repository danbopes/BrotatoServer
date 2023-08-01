class_name WeaponData
extends ItemParentData

enum Type{MELEE, RANGED}

export (String) var weapon_id = ""
export (Type) var type: = Type.MELEE
export (Array, Resource) var sets: = []
export (PackedScene) var scene = null
export (Resource) var stats = null
export (Resource) var upgrades_into

var dmg_dealt_last_wave: = 0
var tracked_value: = 0


func get_category()->int:
	return Category.WEAPON


func get_weapon_stats_text()->String:
	var current_stats
	
	if type == Type.MELEE:
		current_stats = WeaponService.init_melee_stats(stats, weapon_id, sets, effects)
	else :
		current_stats = WeaponService.init_ranged_stats(stats, weapon_id, sets, effects)
	
	return current_stats.get_text(stats)


func get_effects_text()->String:
	var text = .get_effects_text()
	
	if tracking_text != "":
		text += "\n[color=#" + Utils.SECONDARY_FONT_COLOR.to_html() + "]" + Text.text(tracking_text.to_upper(), [str(tracked_value)]) + "[/color]"
	
	return text


func on_tracked_value_updated()->void :
	tracked_value += 1
