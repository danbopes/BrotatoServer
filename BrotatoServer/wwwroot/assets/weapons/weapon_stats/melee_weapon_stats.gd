class_name MeleeWeaponStats
extends WeaponStats

enum AttackType{THRUST, SWEEP}

export (bool) var deal_dmg_on_return: = false
export (AttackType) var attack_type = AttackType.THRUST
export (bool) var alternate_attack_type = false


func get_alternate_attack_type_text()->String:
	return "" if not alternate_attack_type else "\n" + tr("ALTERNATES_ATTACK_TYPE")


func get_deal_dmg_on_return_text()->String:
	return "" if not deal_dmg_on_return else "\n" + tr("DEALS_DAMAGE_ON_RETURN")


func get_damage_text(base_stats:Resource)->String:
	return get_dmg_text_with_scaling_stats(base_stats, scaling_stats, 1)


func get_base_cooldown_value(base_stats:Resource)->float:
	var base_shooting_data = MeleeShootingData.new(base_stats, false)
	return base_shooting_data.get_shooting_total_duration() + base_stats.cooldown / 60.0


func get_cooldown_value(_base_stats:Resource)->float:
	var current_shooting_data = MeleeShootingData.new(self)
	return current_shooting_data.get_shooting_total_duration() + cooldown / 60.0


func get_type_text()->String:
	return tr("MELEE")


func serialize()->Dictionary:
	var serialized = .serialize()
	
	serialized.type = "melee"
	serialized.deal_dmg_on_return = deal_dmg_on_return
	serialized.attack_type = attack_type
	serialized.alternate_attack_type = alternate_attack_type
	
	return serialized


func deserialize_and_merge(serialized:Dictionary):
	.deserialize_and_merge(serialized)
	
	deal_dmg_on_return = serialized.deal_dmg_on_return
	attack_type = serialized.attack_type as int
	alternate_attack_type = serialized.alternate_attack_type
