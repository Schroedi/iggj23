using Godot;
using System;
using System.Collections.Generic;

public class ConnectedAnimal
{
    public List<AnimalPhysics> AllAnimals = new List<AnimalPhysics>();
    public float TotalArea = 0f;



    public static List<ConnectedAnimal> ComputeAnimals(Node2D root)
    {
        // TODO for now
        // TODO: go over rigidbodies in connected animals
        //       go into joints
        //       compute connected comps

        var animals = new List<ConnectedAnimal>();

        foreach (var n in root.GetChildren())
            if (n is AnimalPhysics ap)
            {
                var ca = new ConnectedAnimal { };
                ca.AllAnimals.Add(ap);
                ca.TotalArea += ap.TotalArea;
                animals.Add(ca);
            }

        return animals;
    }

    public Vector2 ComputeAveragePosition()
    {
        Vector2 pos = new Vector2();
        float wsum = 0f;
        foreach (var a in AllAnimals)
        {
            float w = a.TotalArea;
            pos += a.AveragePosition * w;
            wsum += w;
        }
        return pos / wsum;
    }
}