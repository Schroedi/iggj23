using Godot;
using System;

public class BodyPartConfig : Polygon2D
{
    [Export]
    public float RotSpeed = 1.5f;

    [Export]
    public float RotLimit = 1.5f;
}
