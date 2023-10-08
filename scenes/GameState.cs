using Godot;
using System;

public class GameState : Node2D
{
    // string for easy GD interop   
    public string[] StateNames = new[] { "Cutting", "Stitching", "Throwing", "Fighting" };
    public string CurrentState = "Cutting";

    public static GameState Current(Node someNode)
    {
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

    public void StartPhase2()
    {
        CurrentState = "Stitching";
    }

    public void StartPhase3()
    {
        CurrentState = "Throwing";

        LargestAnimal = null;
        foreach (var n in GameRoot.GetChildren())
            if (n is AnimalPhysics ap)
                if (LargestAnimal == null || LargestAnimal.TotalArea < ap.TotalArea)
                    LargestAnimal = ap;
    }

    public bool IsCutting => CurrentState == "Cutting";
    public bool IsStitching => CurrentState == "Stitching";
    public bool IsThrowing => CurrentState == "Throwing";

    public AnimalPhysics LargestAnimal = null;
    public Node2D GameRoot = null;
}
