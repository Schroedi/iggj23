using Godot;
using System;

public class Cutter : Line2D
{
    private Node globals;
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventScreenTouch touch)
        {
            if (globals.Get("PartsHovering") as int? != 0)
                return;
            if (touch.Pressed)
            {
                var pos = touch.Position;
                GlobalPosition = pos;
                this.SetPointPosition(1, new Vector2());
                this.Visible = true;
            }
            else
            {
                this.Visible = false;
            }
        }

        if (@event is InputEventScreenDrag evnt)
        {
            this.SetPointPosition(1, evnt.Position - this.GlobalPosition);
            //GD.Print(this.Points[1]);
        }

        base._Input(@event);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        globals = GetNode<Node>("/root/GlobalHack");
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}