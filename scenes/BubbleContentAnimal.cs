using Godot;
using System;

public class BubbleContentAnimal : Sprite
{
    [Export]
    public AnimalType AnimalType;

    private bool Checked = false;

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Checked)
            return;
        if (!(GetParent() as BubbleContent).HasAnimalType)
            return;
        Checked = true;

        if (AnimalType != (GetParent() as BubbleContent).AnimalType)
            this.QueueFree();
    }
}
