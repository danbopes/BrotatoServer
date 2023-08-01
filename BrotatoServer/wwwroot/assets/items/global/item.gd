class_name Item
extends Area2D

signal picked_up(item)

var push_back: = true
var push_back_destination: = Vector2(0, 0)
var attracted_by:Node2D
var idle_time_after_pushed_back: = 10.0

var _push_back_speed: = 5
var _current_speed: = 500.0

onready var sprite = $Sprite as Sprite


func _ready()->void :
	monitorable = false


func set_texture(texture:Resource)->void :
	if sprite != null:
		sprite.texture = texture


func _physics_process(delta:float)->void :
	if push_back and global_position.distance_to(push_back_destination) > 20:
		global_position = global_position.linear_interpolate(push_back_destination, delta * _push_back_speed)
	elif idle_time_after_pushed_back > 0:
		if not monitorable:
			monitorable = true
		push_back = false
		idle_time_after_pushed_back -= Utils.physics_one(delta)
	elif attracted_by != null:
		global_position = global_position.move_toward(attracted_by.global_position, delta * _current_speed)
		_current_speed += 20


func pickup()->void :
	emit_signal("picked_up", self)
	queue_free()
