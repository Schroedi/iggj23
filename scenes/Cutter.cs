using Godot;

public class Cutter : Line2D
{
    private Node _globals;
    private Vector2 _startPos;
    private RayCast2D _raycast;
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventScreenTouch touch)
        {
            // don't cut while we hover any animal(part) - except if we are already cutting
            if (!Visible && _globals.Get("PartsHovering") as int? != 0)
                return;
            
            if (touch.Pressed)
            {
                _startPos = touch.Position;
                GlobalPosition = _startPos;
                this.SetPointPosition(1, new Vector2());
                this.Visible = true;
            }
            else
            {
                // get animal on cut line
                GD.Print("CUT ME INTO PIECES");
                _raycast.GlobalPosition = touch.Position;
                _raycast.CastTo = _startPos - touch.Position;
                _raycast.ForceRaycastUpdate();
                if (_raycast.IsColliding())
                {
                    if (_raycast.GetCollider() is bodypart part)
                    {
                        CutAnimal(part, _startPos, touch.Position);
                    }
                    else
                    {
                        GD.PrintErr("Tried to cut a non animal: " + _raycast.GetCollider().GetType().Name);
                    }

                }

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

    private void CutAnimal(bodypart bodypart, Vector2 from, Vector2 to)
    {
        var cut = AnimalCutter.Cut(bodypart.CurrentSetup, from, to);
        var parent = bodypart.GetParent().GetParent();
        GD.Print($"got {cut.NewAnimals.Count} new animals");
        foreach (var a in cut.NewAnimals)
        {
            // a.DebugPrint();
            var ap = new AnimalPhysics() { AnimalSetup = a };
            ap.Name = "Cut Up Animal";
            parent.AddChild(ap);
        }
        // delete animal
        bodypart.GetParent().QueueFree();
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _globals = GetNode<Node>("/root/GlobalHack");
        _raycast = GetNode<RayCast2D>("RayCast");
    }
}