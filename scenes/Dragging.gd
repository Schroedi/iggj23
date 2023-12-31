extends Node2D

var dragging = false
var mouseBody: KinematicBody2D
var joint: PinJoint2D

func _ready() -> void:
	# mouse body should be outside of this body
	mouseBody = KinematicBody2D.new()
	get_parent().get_parent().add_child(mouseBody)
	get_parent().connect("input_event", self, "_on_RigidBody2D_input_event")
	get_parent().connect("mouse_entered", self, "_on_RigidBody2D_mouse_entered")
	get_parent().connect("mouse_exited", self, "_on_RigidBody2D_mouse_exited")


func _on_RigidBody2D_input_event(viewport: Viewport, event: InputEvent, shape_idx: int) -> void:
	if event is InputEventScreenTouch and event.is_pressed():
		# pick only once
		if GlobalHack.Picked:
			return
		if GlobalHack.CurrentGamestate.IsThrowing:
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
			stopDrag()
	if event is InputEventScreenDrag:
		mouseBody.global_position = event.position


func _on_RigidBody2D_mouse_entered() -> void:
	GlobalHack.PartsHovering += 1


func _on_RigidBody2D_mouse_exited() -> void:
	GlobalHack.PartsHovering -= 1


func stopDrag():
	dragging = false
	GlobalHack.Picked = false
	joint.queue_free()
	
	if !GlobalHack.CurrentGamestate.IsStitching:
		return
		
	doConnectMagic()


func doConnectMagic():
	# try to re-attach
	# get the overlapping areas of this body's connection areas
	for c in $"../Connectors".get_children():
		if c is Area2D:
			var overlaps = c.get_overlapping_areas()
			var p = get_parent()
			var first = null
			for o in overlaps:
				# don't connect to self
				if o.get_parent() == p:
					continue
				if o.is_queued_for_deletion():
					continue
				first = o
			if first == null:
				continue
			
			var otherBody: RigidBody2D = first.get_parent().get_parent()
			
			# TODO: snapping
			
			# add joints
			var j = PinJoint2D.new()
			
			var j2 = PinJoint2D.new()
			
			get_parent().add_child(j)
			get_parent().add_child(j2)
				   
			j.global_position = first.global_position+ c.Normal*20;    
			j2.global_position = first.global_position+ c.Normal*-20;
			j.disable_collision = true
			j2.disable_collision = true
			yield(get_tree(), "idle_frame")
			j.node_a = otherBody.get_path()
			j.node_b = get_parent().get_path()
			j2.node_a = otherBody.get_path()
			j2.node_b = get_parent().get_path()
			
			# delete connectors
			c.queue_free()
			first.queue_free()
