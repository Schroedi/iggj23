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
                var a = 0.97f;
                var tarPos = state.LargestAnimal.ComputeAveragePosition();
                tarPos.y -= 150;
                var newOffset = a * Offset + (1 - a) * tarPos;

                if (newOffset.y < 1080 * 1.5)
                    newOffset.x = 1920 / 2;
                if (newOffset.y < 1080 / 2)
                    newOffset.y = 1080 / 2;

                Offset = newOffset;

                // GD.Print(Offset);
                // GD.Print(state.LargestAnimal.AveragePosition);
            }
        }
        else
        {
            Offset = new Vector2(1920 / 2, 1080 / 2);
        }
    }
}
