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
            Joint.Position = Position;

            GetParent().CallDeferred("add_child", Joint);
        }

        if (poly.BloodySegments.Count > 0)
        {
            foreach (var bs in poly.BloodySegments)
            {
                var blood = bloodPScence.Instance<CPUParticles2D>();
                
                Vector2 dir = poly.Poly[(bs + 1) % poly.Poly.Count] - poly.Poly[bs];
                blood.GlobalPosition = poly.Poly[bs]+dir*0.5f;
                blood.EmissionRectExtents= new Vector2(dir.Length()/2,0);
                blood.Rotation = dir.Angle();
                this.AddChild(blood);
            }
        }
    }

    public override void _EnterTree()
    {
        base._EnterTree();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        runtime += delta;

        if (Joint != null)
        {
            var targetAngle = Math.Cos(runtime * RotSpeed);
            AngularVelocity = (float)targetAngle * RotLimit;
        }
        
    }
}