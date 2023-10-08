using Godot;
using System;

public class GameState : Node2D
{
    // string for easy GD interop   
    public string[] StateNames = new[] { "Cutting", "Stitching", "Throwing", "Fighting" };
    public string CurrentState = "Cutting";
    
    public static GameState Current(Node someNode) {
        var global = someNode.GetNode<Node>("/root/GlobalHack");
        var gs = global.Get("CurrentGamestate") as GameState;
        if (gs == null)
            throw new Exception("GameState not found!");
        return gs;
    }
    
    public override void _Ready()
    {
        GetNode<Node>("/root/GlobalHack").Set("CurrentGamestate", this);
    }
}
