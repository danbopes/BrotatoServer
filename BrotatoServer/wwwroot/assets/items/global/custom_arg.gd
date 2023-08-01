class_name CustomArg
extends Resource

enum Sign{POSITIVE, NEGATIVE, NEUTRAL, FROM_VALUE, FROM_ARG}
enum ArgValue{
	USUAL = 0, 
	VALUE = 1, 
	KEY = 2, 
	UNIQUE_WEAPONS = 3, 
	ADDITIONAL_WEAPONS = 4, 
	TIER = 5, 
	SCALING_STAT = 6, 
	SCALING_STAT_VALUE = 7, 
	MAX_NB_OF_WAVES = 8, 
	TIER_IV_WEAPONS = 9, 
	TIER_I_WEAPONS = 10
}
enum Format{USUAL, PERCENT, ARG_VALUE_AS_NUMBER}


export (int) var arg_index = 0
export (Sign) var arg_sign = Sign.FROM_ARG
export (ArgValue) var arg_value = ArgValue.USUAL
export (Format) var arg_format = Format.USUAL
