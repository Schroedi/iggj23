using Godot;
using System;
using System.Collections.Generic;

public class AnimalDataSetup
{
    public class BodyPart
    {
        public float RotSpeed = 0f;
        public float RotLimit = 0f;
        public int ParentIndex = -1; // -1 is root
        public Vector2 Origin;
        public Texture Texture;
        public Vector2 TexOffset; 
        public Vector2 TexScale;
        public float TexRot;
        public List<Vector2> Poly = new List<Vector2>();

        // from idx to idx + 1 (mod Poly Count)
        public List<int> BloodySegments = new List<int>();

        public BodyPartConfig Definition;

        public void DebugPrint()
        {
            GD.Print($"    Origin: {Origin}");
            GD.Print($"    RotSpeed: {RotSpeed}");
            GD.Print($"    RotLimit: {RotLimit}");
            GD.Print($"    ParentIndex: {ParentIndex}");
            GD.Print($"    Poly: [{string.Join(", ", Poly)}]");
            GD.Print($"    BloodySegments: [{string.Join(", ", BloodySegments)}]");
        }
    }

    public List<BodyPart> Parts = new List<BodyPart>();

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
