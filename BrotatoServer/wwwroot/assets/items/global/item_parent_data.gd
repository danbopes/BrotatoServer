class_name ItemParentData
extends Resource

enum Tier{COMMON, UNCOMMON, RARE, LEGENDARY, DANGER_4, DANGER_5}

export (String) var my_id = ""
export (bool) var unlocked_by_default = false
export (Resource) var icon
export (String) var name = ""
export (Tier) var tier = Tier.COMMON
export (int) var value = 1
export (Array, Resource) var effects = []
export (String) var tracking_text = ""

var is_locked: = false


func get_category()->int:
	return - 1


func get_effects_text()->String:
	var text = ""
	
	for i in effects.size():
		var effect_text = effects[i].get_text()
		
		text += effect_text
		
		if effect_text != "" and i < effects.size() - 1:
				text += "\n"
	
	if RunData.tracked_item_effects.has(my_id):
		text += "\n[color=#" + Utils.SECONDARY_FONT_COLOR.to_html() + "]" + Text.text(tracking_text, [str(RunData.tracked_item_effects[my_id])]) + "[/color]"
	
	return text
