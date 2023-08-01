class_name Effect
extends Resource

enum Sign{POSITIVE, NEGATIVE, NEUTRAL, FROM_VALUE, FROM_ARG}
enum StorageMethod{SUM, KEY_VALUE, REPLACE}

export (String) var key: = ""
export (String) var text_key: = ""
export (int) var value: = 0
export (String) var custom_key: = ""
export (StorageMethod) var storage_method = StorageMethod.SUM
export (Sign) var effect_sign: = Sign.FROM_VALUE
export (Array, Resource) var custom_args: = []

var base_value = 0


static func get_id()->String:
	return "effect"


func apply()->void :
	if custom_key != "" or storage_method == StorageMethod.KEY_VALUE:
		RunData.effects[custom_key].push_back([key, value])
	elif storage_method == StorageMethod.REPLACE:
		base_value = RunData.effects[key]
		RunData.effects[key] = value
	else :
		RunData.effects[key] += value


func unapply()->void :
	if custom_key != "" or storage_method == StorageMethod.KEY_VALUE:
		RunData.effects[custom_key].erase([key, value])
	elif storage_method == StorageMethod.REPLACE:
		RunData.effects[key] = base_value
	else :
		RunData.effects[key] -= value


func get_text(colored:bool = true)->String:
	var key_text = key.to_upper() if text_key.length() == 0 else text_key.to_upper()
	var args = get_args()
	var signs = []
	
	for i in args:
		signs.push_back(get_sign(effect_sign, value))
	
	for custom_arg in custom_args:
		var i = custom_arg.arg_index
		if i >= args.size():
			for j in (i - args.size()) + 1:
				args.push_back("")
				signs.push_back(Sign.NEUTRAL)
		
		args[i] = get_arg_value(custom_arg.arg_value, args[i])
		signs[i] = get_sign(custom_arg.arg_sign, int(args[i]))
		args[i] = get_formatted(custom_arg.arg_format, args[i], custom_arg.arg_value)
	
	return Text.text(key_text, args, [] if not colored else signs)


func get_arg_value(from_arg_value:int, p_base_value:String)->String:
	var final_value = p_base_value
	
	if from_arg_value != ArgValue.USUAL:
		match from_arg_value:
			ArgValue.VALUE:final_value = str(value)
			ArgValue.KEY:final_value = str(tr(key.to_upper()))
			ArgValue.UNIQUE_WEAPONS:
				var nb = RunData.get_unique_weapon_ids().size()
				final_value = str(value * nb)
			ArgValue.ADDITIONAL_WEAPONS:
				var nb = RunData.weapons.size()
				final_value = str(value * nb)
			ArgValue.TIER:
				var val = "TIER_I"
				if value == 1:val = "TIER_II"
				elif value == 2:val = "TIER_III"
				elif value == 3:val = "TIER_IV"
				final_value = tr(val)
			ArgValue.SCALING_STAT:
				final_value = Utils.get_scaling_stat_text(key, value / 100.0)
			ArgValue.SCALING_STAT_VALUE:
				final_value = str(WeaponService.get_scaling_stats_value([[key, value / 100.0]]))
			ArgValue.MAX_NB_OF_WAVES:
				final_value = str(RunData.nb_of_waves)
			ArgValue.TIER_IV_WEAPONS:
				var nb_tier_iv_weapons = 0
				for weapon in RunData.weapons:
					if weapon.tier >= Tier.LEGENDARY:
						nb_tier_iv_weapons += 1
				final_value = str(value * nb_tier_iv_weapons)
			ArgValue.TIER_I_WEAPONS:
				var nb_tier_i_weapons = 0
				for weapon in RunData.weapons:
					if weapon.tier <= Tier.COMMON:
						nb_tier_i_weapons += 1
				final_value = str(value * nb_tier_i_weapons)
			_:print("wrong value")
	return final_value


func get_sign(from_sign:int, from_value:int)->Sign:
	
	var final_sign = from_sign
	
	if from_sign == Sign.FROM_VALUE:
		final_sign = Sign.POSITIVE if value > 0 else Sign.NEGATIVE if value < 0 else Sign.NEUTRAL
	elif from_sign == Sign.FROM_ARG:
		final_sign = Sign.POSITIVE if from_value > 0 else Sign.NEGATIVE if from_value < 0 else Sign.NEUTRAL
	else :
		final_sign = from_sign
	
	return final_sign


func get_formatted(from_format:int, from_value:String, base_arg_value:int)->String:
	var formatted = from_value
	
	if from_format != Format.USUAL:
		match from_format:
			Format.PERCENT:formatted = str(float(from_value) / 100.0)
			Format.ARG_VALUE_AS_NUMBER:formatted = str(base_arg_value)
			_:print("wrong format")
	
	return formatted


func get_args()->Array:
	
	var displayed_key = key
	
	if custom_key == "starting_weapon":
		displayed_key = key.substr(0, key.length() - 2)
	
	return [str(value), tr(displayed_key.to_upper())]


func serialize()->Dictionary:
	return {
		"effect_id":get_id(), 
		"key":key, 
		"custom_key":custom_key, 
		"text_key":text_key, 
		"storage_method":storage_method, 
		"value":str(value), 
		"effect_sign":effect_sign, 
		"base_value":base_value
	}


func deserialize_and_merge(effect:Dictionary)->void :
	key = effect.key
	custom_key = effect.custom_key
	text_key = effect.text_key
	value = effect.value as int
	effect_sign = effect.effect_sign as int
	storage_method = effect.storage_method as int
	base_value = effect.base_value
