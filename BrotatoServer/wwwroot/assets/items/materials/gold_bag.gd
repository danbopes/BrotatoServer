class_name GoldBag
extends Area2D


func _on_GoldBag_area_entered(area:Area2D)->void :
	area.pickup()
