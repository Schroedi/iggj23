using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

        var allAnimals = ConnectedAnimal.ComputeAnimals(GameRoot);

        LargestAnimal = null;
        foreach (var ap in allAnimals)
            if (LargestAnimal == null || LargestAnimal.TotalArea < ap.TotalArea)
                LargestAnimal = ap;

        // explode all others
        var bloodExplScene = GD.Load<PackedScene>("res://BloodParticlesEx.tscn");
        foreach (var ca in allAnimals)
        {
            if (ca == LargestAnimal)
                continue;

            foreach (var ap in ca.AllAnimals)
            {
                foreach (var bp in ap.Parts)
                {
                    var blood = bloodExplScene.Instance<CPUParticles2D>();
                    blood.GlobalPosition = bp.GlobalPosition;
                    blood.Emitting = true;
                    GameRoot.AddChild(blood);
                }

                // delete animal
                ap.QueueFree();
            }
        }
    }

    public bool IsCutting => CurrentState == "Cutting";
    public bool IsStitching => CurrentState == "Stitching";
    public bool IsThrowing => CurrentState == "Throwing";

    public ConnectedAnimal LargestAnimal = null;
    public Node2D GameRoot = null;
}
