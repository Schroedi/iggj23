using Godot;
using System;

public class Cutter : Line2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventScreenTouch touch)
        {
            if (touch.Pressed)
            {
                var pos = touch.Position;
                GlobalPosition = pos;
            }
        }

        if (@event is InputEventScreenDrag evnt)
        {
            this.SetPointPosition(1, evnt.Position - this.GlobalPosition);
            GD.Print(this.Points[1]);
        }

        base._Input(@event);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}