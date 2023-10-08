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

    public override void _Process(float delta)
    {
        if (IsThrowing)
        {
            TimeInThrowing += delta;
            TimeToLive -= delta;

            if (TimeToLive < 0 && LargestAnimal != null)
                FinishGame();
        }
    }

    void FinishGame()
    {
        // explode all others
        var bloodExplScene = GD.Load<PackedScene>("res://BloodParticlesEx.tscn");

        foreach (var ap in LargestAnimal.AllAnimals)
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

        LargestAnimal = null;
    }

    public float TimeInThrowing = 0f;

    public float TimeToLive = 20;

    public int Score = 0;

    public void StartPhase2()
    {
        CurrentState = "Stitching";
    }

    public void StartPhase3()
    {
        CurrentState = "Throwing";
        TimeInThrowing = 0;
        Score = 0;

        var allAnimals = ConnectedAnimal.ComputeAnimals(GameRoot);

        LargestAnimal = null;
        foreach (var ap in allAnimals)
            if (LargestAnimal == null || LargestAnimal.TotalArea < ap.TotalArea || (LargestAnimal.AllAnimals.Count == 1 && ap.AllAnimals.Count > 1))
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
