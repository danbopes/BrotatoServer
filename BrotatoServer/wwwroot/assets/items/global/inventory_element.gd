class_name InventoryElement
extends Button

signal element_hovered(element)
signal element_unhovered(element)
signal element_focused(element)
signal element_unfocused(element)
signal element_pressed(element)

export (Resource) var sound_on_focus
export (Resource) var sound_on_pressed

var item:Resource
var is_random: = false
var is_special: = false
var current_number: = 0

onready var _number_label = $Label


func set_element(item_data:Resource)->void :
	item = item_data
	icon = item_data.icon
	update_background_color()
	current_number = 1


func add_to_number(value:int = 1)->void :
	current_number += value
	_number_label.text = "x" + str(current_number)
	
	if current_number > 1:
		_number_label.show()


func set_icon(p_icon:Resource)->void :
	icon = p_icon


func set_number(p_number:int)->void :
	_number_label.text = str(p_number)


func update_background_color(p_color:int = - 1)->void :
	var stylebox_color = get_stylebox("normal").duplicate()
	ItemService.change_inventory_element_stylebox_from_tier(stylebox_color, p_color if p_color != - 1 else item.tier, 0.25)
	add_stylebox_override("normal", stylebox_color)


func _on_InventoryElement_mouse_entered()->void :
	SoundManager.play(sound_on_focus, - 5, 0.2)
	emit_signal("element_hovered", self)


func _on_InventoryElement_mouse_exited()->void :
	emit_signal("element_unhovered", self)


func _on_InventoryElement_focus_entered()->void :
	SoundManager.play(sound_on_focus, - 5, 0.2)
	emit_signal("element_focused", self)


func _on_InventoryElement_focus_exited()->void :
	emit_signal("element_unfocused", self)


func _on_InventoryElement_pressed()->void :
	SoundManager.play(sound_on_pressed, 0, 0.2)
	emit_signal("element_pressed", self)
