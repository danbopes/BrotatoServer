class_name Consumable
extends Item

var consumable_data:Resource = null


func pickup()->void :
	.pickup()
	SoundManager.play(Utils.get_rand_element(consumable_data.pickup_sounds))
