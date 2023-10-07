using Godot;
using System;

public class CutTester : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private bool first = true;

    [Export]
    public NodePath TestAnimal;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (first)
        {
            first = false;

            var animal = GetNode<Animal>(TestAnimal);
            var cut = AnimalCutter.Cut(animal.Setup, new Vector2(0, 360), new Vector2(1, 0));
            foreach (var a in cut.NewAnimals)
            {
                var ap = new AnimalPhysics() { AnimalSetup = a };
                AddChild(ap);
            }
        }
    }
}
