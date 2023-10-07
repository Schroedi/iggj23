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

    private static Texture[] DropletTextures = new[]{
        (Texture)ResourceLoader.Load("res://Assets/Cuteness_Droplet1.png"),
        (Texture)ResourceLoader.Load("res://Assets/Cuteness_Droplet2.png"),
        (Texture)ResourceLoader.Load("res://Assets/Cuteness_Droplet3.png"),
        (Texture)ResourceLoader.Load("res://Assets/Cuteness_Droplet4.png"),
        (Texture)ResourceLoader.Load("res://Assets/Cuteness_Droplet5.png"),
        (Texture)ResourceLoader.Load("res://Assets/Cuteness_Droplet6.png"),
    };
    private static Texture[] FreshWoundTextures = new[]{
        (Texture)ResourceLoader.Load("res://Assets/Cuteness_CutFresh1.png"),
        (Texture)ResourceLoader.Load("res://Assets/Cuteness_CutFresh2.png"),
        (Texture)ResourceLoader.Load("res://Assets/Cuteness_CutFresh3.png"),
        (Texture)ResourceLoader.Load("res://Assets/Cuteness_CutFresh4.png"),
    };

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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
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

        foreach (var tex in DropletTextures)
            foreach (var bs in poly.BloodySegments)
            {
                var blood = bloodPScence.Instance<CPUParticles2D>();

                var p0 = poly.Poly[bs];
                var p1 = poly.Poly[(bs + 1) % poly.Poly.Count];

                Vector2 dir = p1 - p0;
                blood.GlobalPosition = p0 + dir * 0.5f;
                blood.EmissionRectExtents = new Vector2(dir.Length() / 2, 0);
                blood.Rotation = dir.Angle();
                blood.Texture = tex;
                this.AddChild(blood);
            }

        foreach (var bs in poly.BloodySegments)
        {
            var p0 = poly.Poly[bs];
            var p1 = poly.Poly[(bs + 1) % poly.Poly.Count];

            var tex = FreshWoundTextures[0]; // TODO

            Vector2 dir = p1 - p0;

            var texR = new TextureRect();
            texR.Texture = tex;
            texR.RectGlobalPosition = (p0 + p1) / 2;
            texR.RectRotation = dir.Angle();
            texR.RectPivotOffset = tex.GetSize() / 2;
            // texR.RectScale = 
            // FIXME
            this.AddChild(texR);
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
}
