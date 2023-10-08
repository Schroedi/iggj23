using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class AnimalDataSetup
{
    public class BodyPart
    {
        public float RotSpeed = 0f;
        public float RotLimit = 0f;
        public int ParentIndex = -1; // -1 is root
        public Vector2 Origin;
        public float Rot;
        public Texture Texture;
        public Vector2 TexOffset;
        public Vector2 TexScale;
        public float TexRot;
        public List<Vector2> Poly = new List<Vector2>();
        public AnimalType AnimalType;

        // from idx to idx + 1 (mod Poly Count)
        public List<int> BloodySegments = new List<int>();

        public BodyPartConfig Definition;
        public Animal Animal { get; set; }

        public void DebugPrint()
        {
            GD.Print($"    Origin: {Origin}");
            GD.Print($"    RotSpeed: {RotSpeed}");
            GD.Print($"    RotLimit: {RotLimit}");
            GD.Print($"    ParentIndex: {ParentIndex}");
            GD.Print($"    Poly: [{string.Join(", ", Poly)}]");
            GD.Print($"    BloodySegments: [{string.Join(", ", BloodySegments)}]");
        }

        public float ComputeArea()
        {
            float a = 0;
            for (var i = 2; i < Poly.Count; ++i)
            {
                var p0 = Poly[0];
                var p1 = Poly[i - 1];
                var p2 = Poly[i];
                a += Mathf.Abs((p1 - p0).Cross(p2 - p0));
            }
            return a;
        }
    }

    public List<BodyPart> Parts = new List<BodyPart>();
    public float ComputeArea() => Parts.Sum(p => p.ComputeArea());

    public void DebugPrint()
    {
        GD.Print("[AnimalDataSetup]");
        for (var i = 0; i < Parts.Count; ++i)
        {
            GD.Print($"  [part {i}]");
            Parts[i].DebugPrint();
        }
    }
}
