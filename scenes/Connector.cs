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
        Connect("area_entered", this, nameof(_on_Area2D_area_entered));
        Connect("area_exited", this, nameof(_on_Area2D_area_exited));
    }

    public void _on_Area2D_area_entered(Area2D area)
    {
        if (_globals.Get("Picked") as bool? == true)
            _snapIndicator.Visible = true; 
    }
    
    public void _on_Area2D_area_exited(Area2D area)
    {
        _snapIndicator.Visible = false;
    }
}

