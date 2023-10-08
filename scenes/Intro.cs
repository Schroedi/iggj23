using Godot;
using System;

public class Intro : Control
{
    public static bool WasAlreadyShown = false;

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
        this.Visible = !WasAlreadyShown;

        if (WasAlreadyShown)
            this.QueueFree();
    }
}
