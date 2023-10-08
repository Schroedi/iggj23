using Godot;
using System;

public class lScore : Label
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
        this.Visible = state.IsThrowing;

        this.Text = $"Score: {state.Score}";
    }
}
