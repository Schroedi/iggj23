using Godot;
using System;

public class Connector : Area2D
{
    public Animal Animal;
    public Vector2 Normal;
    private Sprite _snapIndicator;
    private Node _globals;
    
    public override void _Ready()
    {
        base._Ready();
        _globals = GetNode<Node>("/root/GlobalHack");
        _snapIndicator = GetNode<Sprite>("SnapIndicator");
    }

    public override void _PhysicsProcess(float delta)
    {
        bool picking = _globals.Get("Picked") as bool? == true;
        _snapIndicator.Visible = picking && this.GetOverlappingAreas().Count > 0;
    }

}

