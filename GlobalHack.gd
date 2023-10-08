extends Node

var Picked = false

# keep track of how many objects are under the curser so we don't cur while hovering and object
# this sometimes breaks of cause
var PartsHovering = 0

var CurrentGamestate

func reset():
	Picked = false
	PartsHovering = 0
