class_name RangedWeaponShootingBehavior
extends WeaponShootingBehavior

signal projectile_shot(projectile)


func shoot(_distance:float)->void :
	SoundManager.play(Utils.get_rand_element(_parent.current_stats.shooting_sounds), _parent.current_stats.sound_db_mod, 0.2)
	
	var initial_position:Vector2 = _parent.sprite.position
	
	_parent.set_shooting(true)
	
	for i in _parent.current_stats.nb_projectiles:
		var proj_rotation = rand_range(_parent.rotation - _parent.current_stats.projectile_spread, _parent.rotation + _parent.current_stats.projectile_spread)
		var knockback_direction: = - Vector2(cos(proj_rotation), sin(proj_rotation))
		shoot_projectile(proj_rotation, knockback_direction)
	
	_parent.tween.interpolate_property(
		_parent.sprite, 
		"position", 
		initial_position, 
		Vector2(initial_position.x - _parent.current_stats.recoil, initial_position.y), 
		_parent.current_stats.recoil_duration, 
		Tween.TRANS_EXPO, 
		Tween.EASE_OUT
	)
	
	_parent.tween.start()
	yield (_parent.tween, "tween_all_completed")
	
	_parent.tween.interpolate_property(
		_parent.sprite, 
		"position", 
		_parent.sprite.position, 
		initial_position, 
		_parent.current_stats.recoil_duration, 
		Tween.TRANS_EXPO, 
		Tween.EASE_OUT
	)
	
	_parent.tween.start()
	yield (_parent.tween, "tween_all_completed")
	
	_parent.set_shooting(false)


func shoot_projectile(rotation:float = _parent.rotation, knockback:Vector2 = Vector2.ZERO)->void :
	var projectile = WeaponService.spawn_projectile(rotation, 
		_parent.current_stats, 
		_parent.muzzle.global_position, 
		knockback, 
		false, 
		_parent.effects, 
		_parent
	)
	
	emit_signal("projectile_shot", projectile)
