using Godot;
using System;

public class bodypart : RigidBody2D
{
    [Export] public NodePath ParentPart;

    [Export] public float RotSpeed = 1.5f;

    [Export] public float RotLimit = 1.5f;

    private bodypart parentPart;
    private PinJoint2D Joint;
    private double runtime = 0f;
    private Polygon2D SpritePolygon;
    private CollisionPolygon2D CollisionPolygon;

    private bool IsDragging = false;
    private PinJoint2D MouseJoint = null;
    private KinematicBody2D MouseBody = new KinematicBody2D();
    private Sprite DebugSprite = new Sprite();

    private AnimalDataSetup.BodyPart poly;


    public void Init(AnimalDataSetup.BodyPart bp, bodypart pp)
    {
        poly = bp;
        parentPart = pp;

        RotSpeed = bp.RotSpeed;
        RotLimit = bp.RotLimit;
    }

    public bodypart()
    {
    }
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("input_event", this, nameof(OnInput));
        GetParent().AddChild(MouseBody);

        DebugSprite.Texture = ResourceLoader.Load<Texture>("res://icon.png");
        //MouseBody.AddChild(DebugSprite);

        // otherwise dragging can fail
        CanSleep = false;

        var bloodPScence = GD.Load<PackedScene>("res://BloodParticles.tscn");

        SpritePolygon = GetNode<Polygon2D>("SpritePolygon");
        CollisionPolygon = GetNode<CollisionPolygon2D>("CollisionPolygon");
        if (poly != null)
        {
            this.GlobalPosition = poly.Origin;
            SpritePolygon.Polygon = poly.Poly.ToArray();
            SpritePolygon.Texture = poly.Texture;
            SpritePolygon.TextureRotation = poly.TexRot;
            SpritePolygon.TextureOffset = poly.TexOffset;
            SpritePolygon.TextureScale = poly.TexScale;
            CollisionPolygon.Polygon = poly.Poly.ToArray();
        }

        if (ParentPart == null && parentPart != null)
        {
            ParentPart = parentPart.GetPath();
        }

        if (ParentPart != null)
        {
            var pnode = GetNode<bodypart>(ParentPart);
            parentPart = pnode;

            Joint = new PinJoint2D();
            Joint.NodeA = this.GetPath();
            Joint.NodeB = ParentPart;
            //Joint.Position = Position;
            AddChild(Joint);
            //GetParent().CallDeferred("add_child", Joint);
        }

        if (poly.BloodySegments.Count > 0)
        {
            foreach (var bs in poly.BloodySegments)
            {
                var blood = bloodPScence.Instance<CPUParticles2D>();

                Vector2 dir = poly.Poly[(bs + 1) % poly.Poly.Count] - poly.Poly[bs];
                blood.GlobalPosition = poly.Poly[bs] + dir * 0.5f;
                blood.EmissionRectExtents = new Vector2(dir.Length() / 2, 0);
                blood.Rotation = dir.Angle();
                this.AddChild(blood);
            }
        }
    }

    void OnInput(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventScreenTouch touch)
        {
            IsDragging = touch.IsPressed();

            // add a pin joint between touch position and mouse position
            var globalPos = touch.Position * GetViewportTransform();
            MouseBody.GlobalPosition = touch.Position;
            var partent = GetParent<Node2D>();

            MouseJoint = new PinJoint2D();

            MouseJoint.NodeA = MouseBody.GetPath();
            MouseJoint.NodeB = this.GetPath();

            MouseJoint.DisableCollision = true;


            //MouseJoint.GlobalPosition = GlobalPosition;
            //MouseJoint.Bias = 0.1f;
            MouseJoint.Softness = 10;

            MouseBody.AddChild(MouseJoint);
        }
    }


    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventScreenTouch touch && !touch.IsPressed())
        {
            if (IsDragging)
            {
                IsDragging = false;
                MouseBody.RemoveChild(MouseJoint);
                MouseJoint.Dispose();
            }
        }

        if (@event is InputEventScreenDrag drag)
        {
           
      
            var globalPos = drag.Position * GetViewportTransform();
            MouseBody.GlobalPosition = globalPos;
            GD.Print(drag.Position);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        runtime += delta;

        if (Joint != null)
        {
            var targetAngle = Math.Cos(runtime * RotSpeed);
            AngularVelocity = (float)targetAngle * RotLimit;
        }
    }

    // public override void _IntegrateForces(Physics2DDirectBodyState state)
    // {
    //     if (IsDragging)
    //     {
    //         const float scale = 0.16f;
    //         var v = (GetViewport().GetMousePosition() - Position) * scale;
    //         //state.AddCentralForce(v);
    //         //state.LinearVelocity = v;
    //     }
    //     base._IntegrateForces(state);
    // }
}