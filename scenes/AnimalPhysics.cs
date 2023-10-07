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

    List<bodypart> Parts = new List<bodypart>();

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
        if (AnimalPath != null && Animal == null)
            Animal = GetNode<Animal>(AnimalPath);
        if (Animal != null)
        {
            foreach (var part in Animal.Setup.Parts)
            {
               var node = bpScenes.Instance<bodypart>();
                node.Init(part, part.ParentIndex == -1 ? null : Parts[part.ParentIndex]);
                Parts.Add(node);
                this.CallDeferred("add_child", node);
            }
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}