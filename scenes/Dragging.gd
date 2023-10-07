extends Node

var dragging = false
var mouseBody: KinematicBody2D
var joint: PinJoint2D

func _ready() -> void:
	# mouse body should be outside of this body
	mouseBody = KinematicBody2D.new()
	get_parent().get_parent().add_child(mouseBody)
	var s = Sprite.new()
	s.texture = preload("res://icon.png")
	mouseBody.add_child(s)
	
	get_parent().connect("input_event", self, "_on_RigidBody2D_input_event")


func _on_RigidBody2D_input_event(viewport: Viewport, event: InputEvent, shape_idx: int) -> void:
	if event is InputEventScreenTouch and event.is_pressed():
		# pick only once
		if GlobalHack.Picked:
			return
		dragging = true
		GlobalHack.Picked = true
		mouseBody.global_position = event.position
		joint = PinJoint2D.new()
		joint.name = "mouseJoint"
		get_parent().add_child(joint)
		joint.global_position = event.position
		
		joint.softness = 2
		#joint.bias = 0.5
		
		yield(get_tree(),"idle_frame")
		joint.node_a = get_parent().get_path()
		joint.node_b = mouseBody.get_path()
	pass

func _input(event: InputEvent) -> void:
	if event is InputEventScreenTouch and !event.is_pressed():
		if (dragging):
			dragging = false
			GlobalHack.Picked = false
			joint.node_a = ""
			joint.node_b = ""
			joint.queue_free()
			joint = null
	if event is InputEventScreenDrag:
		mouseBody.global_position = event.position
