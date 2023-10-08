using Godot;
using System;
using System.Collections.Generic;
using System.Security.Policy;

public class ConnectedAnimal
{
    public List<AnimalPhysics> AllAnimals = new List<AnimalPhysics>();
    public float TotalArea = 0f;

    public Dictionary<AnimalType, int> AnimalParts = new Dictionary<AnimalType, int>();


    public static List<ConnectedAnimal> ComputeAnimals(Node2D root)
    {
        // TODO for now
        // TODO: go over rigidbodies in connected animals
        //       go into joints
        //       compute connected comps

        var animals = new List<ConnectedAnimal>();

        var connectedTo = new Dictionary<AnimalPhysics, List<AnimalPhysics>>();

        void Connect(RigidBody2D rb0, RigidBody2D rb1)
        {
            var ap0 = rb0.GetParent() as AnimalPhysics;
            var ap1 = rb1.GetParent() as AnimalPhysics;

            if (!connectedTo.ContainsKey(ap0))
                connectedTo.Add(ap0, new List<AnimalPhysics>());
            if (!connectedTo.ContainsKey(ap1))
                connectedTo.Add(ap1, new List<AnimalPhysics>());

            connectedTo[ap0].Add(ap1);
            connectedTo[ap1].Add(ap0);
        }

        // collect all AP -> AP connections
        foreach (var n in root.GetChildren())
            if (n is AnimalPhysics ap)
            {
                if (!connectedTo.ContainsKey(ap))
                    connectedTo.Add(ap, new List<AnimalPhysics>());
                foreach (var nn in ap.GetChildren())
                    if (nn is RigidBody2D rb)
                        foreach (var nnn in rb.GetChildren())
                            if (nnn is PinJoint2D pj)
                                Connect(root.GetNode<RigidBody2D>(pj.NodeA), root.GetNode<RigidBody2D>(pj.NodeB));
            }

        var visited = new HashSet<AnimalPhysics>();

        foreach (var n in root.GetChildren())
            if (n is AnimalPhysics apRoot)
            {
                if (visited.Contains(apRoot))
                    continue;

                var ca = new ConnectedAnimal { };

                void Visit(AnimalPhysics ap)
                {
                    if (visited.Contains(ap))
                        return;

                    ca.AllAnimals.Add(ap);
                    ca.TotalArea += ap.TotalArea;
                    visited.Add(ap);

                    foreach (var app in connectedTo[ap])
                        Visit(app);
                }
                Visit(apRoot);

                // compute percentages
                foreach (var ap in ca.AllAnimals)
                {
                    var scorePart = Mathf.RoundToInt(100f * ap.TotalArea / ca.TotalArea);
                    if (!ca.AnimalParts.ContainsKey(ap.AnimalType))
                        ca.AnimalParts[ap.AnimalType] = scorePart;
                    else ca.AnimalParts[ap.AnimalType] += scorePart;
                }

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