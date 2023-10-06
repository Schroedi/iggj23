using Godot;
using System;

public class bodypart : RigidBody2D
{
    [Export]
    public NodePath ParentPart;

    [Export]
    public float RotSpeed = 1.5f;

    [Export]
    public float RotLimit = 1.5f;

    private bodypart parentPart;
    private PinJoint2D Joint;
    private double runtime = 0f;

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (ParentPart != null)
        {
            var pnode = GetNode<bodypart>(ParentPart);
            parentPart = pnode;

            Joint = new PinJoint2D();
            Joint.NodeA = this.GetPath();
            Joint.NodeB = ParentPart;
            Joint.Position = Position;

            GetParent().CallDeferred("add_child", Joint);
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        runtime += delta;

        if (Joint != null)
        {
            var targetAngle = Math.Cos(runtime * RotSpeed);
            AngularVelocity = (float)targetAngle * RotLimit;
        }
    }
}
