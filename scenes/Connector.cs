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
        var doesOverlap = false;
        var p = GetParent();
        foreach (var a in GetOverlappingAreas())
        {
            var area = a as Area2D;
            if (p == area.GetParent()) continue;
            doesOverlap = true;
            break;
        }
        
        _snapIndicator.Visible = picking && doesOverlap;
    }

}

