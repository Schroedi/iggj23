using Godot;
using System;
using System.Linq;

public class Animal : Node2D
{
    // empty

    public AnimalDataSetup Setup;

    public override void _Ready()
    {
        var setup = CreateAnimalDataSetup();
        Setup = setup;

        // var cut = AnimalCutter.Cut(setup, new Vector2(0, 360), new Vector2(1, 0));
        // foreach (var a in cut.NewAnimals)
        //     a.DebugPrint();
    }


    private AnimalDataSetup CreateAnimalDataSetup()
    {
        var setup = new AnimalDataSetup();

        void AddNodeRec(BodyPartConfig poly, int parent)
        {
            var idx = setup.Parts.Count;
            var p = new AnimalDataSetup.BodyPart();
            setup.Parts.Add(p);
            p.Origin = poly.GlobalPosition;
            p.ParentIndex = parent;
            p.Definition = poly;
            p.RotLimit = poly.RotLimit;
            p.RotSpeed = poly.RotSpeed;
            p.Poly = poly.Polygon.ToList();
            p.Texture = poly.Texture;
            p.TexOffset = poly.TextureOffset;
            p.TexScale = poly.TextureScale;
            p.TexRot = poly.TextureRotation;
            for (var i = 0; i < p.Poly.Count; ++i)
                p.Poly[i] += poly.Offset;

            var cc = poly.GetChildCount();
            for (var i = 0; i < cc; ++i)
                AddNodeRec(poly.GetChild<BodyPartConfig>(i), idx);
        }
        AddNodeRec(GetChild<BodyPartConfig>(0), -1);

        return setup;
    }
}
