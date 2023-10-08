using Godot;
using System;

public class CameraController : Camera2D
{
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
        var state = GameState.Current(this);

        if (state.IsThrowing)
        {
            if (state.LargestAnimal != null)
            {
                var a = 0.9f;
                Offset = a * Offset + (1 - a) * state.LargestAnimal.ComputeAveragePosition();
                // GD.Print(Offset);
                // GD.Print(state.LargestAnimal.AveragePosition);
            }
        }
        else
        {
            Offset = new Vector2(960 / 2, 512 / 2);
        }
    }
}
