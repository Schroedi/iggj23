using Godot;
using System;
using System.Collections.Generic;

public class AnimalPhysics : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";



    // Called when the node enters the scene tree for the first time.
    [Export] public Animal Animal;

    [Export] public NodePath AnimalPath;

    public AnimalDataSetup AnimalSetup;

    public List<bodypart> Parts = new List<bodypart>();

    public List<AnimalPhysics> ConnectedPhysics = new List<AnimalPhysics>();

    public float TotalArea = 0f;

    public Vector2 AveragePosition;

    public AnimalType AnimalType;

    public AnimalPhysics(Animal animal)
    {
        Animal = animal;
    }
    public AnimalPhysics()
    {
    }
    public override void _Ready()
    {
        var bpScenes = GD.Load<PackedScene>("res://scenes/bodypart.tscn");

        if (AnimalSetup == null)
        {
            if (AnimalPath != null && Animal == null)
            {
                Animal = GetNode<Animal>(AnimalPath);
                Animal.Visible = false;
            }
            if (Animal != null)
                AnimalSetup = Animal.Setup;
        }

        foreach (var part in AnimalSetup.Parts)
        {
            var node = bpScenes.Instance<bodypart>();
            node.Init(part, part.ParentIndex == -1 ? null : Parts[part.ParentIndex], AnimalSetup);
            Parts.Add(node);
            AnimalType = part.AnimalType; // should be the same anyways
            this.CallDeferred("add_child", node);
        }

        TotalArea = AnimalSetup.ComputeArea();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        // I'm a hack
        GameState.Current(this).GameRoot = GetParent() as Node2D;

        AveragePosition = new Vector2();
        var cnt = 0;
        foreach (var p in Parts)
        {
            AveragePosition += p.GlobalPosition;
            cnt++;
        }
        AveragePosition /= cnt;
    }
}