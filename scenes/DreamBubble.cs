using Godot;
using System;
using System.Collections.Generic;

public class DreamBubble : Node2D
{
    [Export]
    public bool PreviewMode = false;

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private static Random Rng = new Random();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var children = new List<Node2D>();
        foreach (var c in this.GetChildren())
            if (c is Node2D n)
                children.Add(n);

        var ci = Rng.Next() % children.Count;
        // GD.Print(ci);
        var keepC = children[ci];
        foreach (var c in children)
            if (c != keepC)
                c.QueueFree();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
