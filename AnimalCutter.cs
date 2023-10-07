using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class AnimalCutResult
{
    // TODO: invalid if too close to any joint
    public bool IsValid = false;

    // non-empty only if valid
    public List<AnimalDataSetup> NewAnimals = new List<AnimalDataSetup>();
}

public static class AnimalCutter
{
    public static AnimalCutResult Cut(AnimalDataSetup animal, Vector2 pos, Vector2 dir)
    {
        // array of array of parts
        // outer index corresponds to original index (and thus ParentIndex)
        var newParts = animal.Parts.Select(p => CutPart(p, pos, dir)).ToArray();

        // TODO: preserve connections

        // compute parents
        var parentOf = new Dictionary<AnimalDataSetup.BodyPart, AnimalDataSetup.BodyPart>();
        var childrenOf = new Dictionary<AnimalDataSetup.BodyPart, List<AnimalDataSetup.BodyPart>>();
        foreach (var parts in newParts)
            foreach (var p in parts)
                childrenOf.Add(p, new List<AnimalDataSetup.BodyPart>());
        foreach (var parts in newParts)
        {
            foreach (var p in parts)
            {
                if (p.ParentIndex == -1)
                    continue; // not connected

                AnimalDataSetup.BodyPart parentPart = null;
                foreach (var pp in newParts[p.ParentIndex])
                {
                    if (IsPointInside(p.Origin, pp.Poly, pp.Origin))
                        parentPart = pp;
                }

                parentOf.Add(p, parentPart);
                if (parentPart != null)
                    childrenOf[parentPart].Add(p);
            }
        }

        // compute connected components
        var visited = new HashSet<AnimalDataSetup.BodyPart>();

        var res = new AnimalCutResult();
        res.IsValid = true;
        foreach (var parts in newParts)
            foreach (var p in parts)
            {
                if (visited.Contains(p))
                    continue;

                var newAnimal = new AnimalDataSetup();
                var newPartIndex = new Dictionary<AnimalDataSetup.BodyPart, int>();

                // collect connected parts
                void Visit(AnimalDataSetup.BodyPart part)
                {
                    if (part == null)
                        return;
                    if (visited.Contains(part))
                        return;

                    // GD.Print(part);
                    // GD.Print(newPartIndex.ContainsKey(part));
                    newPartIndex.Add(part, newAnimal.Parts.Count);
                    newAnimal.Parts.Add(part);
                    visited.Add(part);

                    foreach (var cp in childrenOf[part])
                        Visit(cp);
                    if (parentOf.ContainsKey(part))
                        Visit(parentOf[part]);
                }
                Visit(p);

                // rewrite parent indices
                foreach (var pp in newAnimal.Parts)
                    pp.ParentIndex = parentOf.ContainsKey(pp) && parentOf[pp] != null ? newPartIndex[parentOf[pp]] : -1;

                res.NewAnimals.Add(newAnimal);
            }
        return res;
    }

    private static bool IsPointInside(Vector2 p, List<Vector2> pts, Vector2 ptsOffset)
    {
        p -= ptsOffset; // to local

        var polyArea = 0.0;
        for (var i = 2; i < pts.Count; ++i)
        {
            var p0 = pts[0];
            var p1 = pts[i - 1];
            var p2 = pts[i];

            polyArea += Mathf.Abs((p2 - p0).Cross(p1 - p0));
        }

        var pArea = 0.0;
        for (var i = 0; i < pts.Count; ++i)
        {
            var i0 = i;
            var i1 = i == pts.Count - 1 ? 0 : i + 1;
            var p0 = pts[i0];
            var p1 = pts[i1];

            pArea += Mathf.Abs((p1 - p).Cross(p0 - p));
        }

        // GD.Print(Math.Abs(polyArea - pArea));
        return Math.Abs(polyArea - pArea) < 0.1;
    }

    private static List<AnimalDataSetup.BodyPart> CutPart(AnimalDataSetup.BodyPart part, Vector2 pos, Vector2 dir)
    {
        double PlaneDis(Vector2 p)
        {
            var x = part.Origin.x + p.x - pos.x;
            var y = part.Origin.y + p.y - pos.y;
            var d = x * (double)(dir.y) - y * (double)(dir.x);
            // if (Mathf.Abs(d) < 0.1)
            //     d = 0.1f;
            return d;
        }

        var distances = part.Poly.Select(PlaneDis).ToArray();
        // FIXME: distances close to 0?

        var minDis = distances.Min();
        var maxDis = distances.Max();

        // no cut
        if (minDis >= 0 || maxDis <= 0)
            return new List<AnimalDataSetup.BodyPart> { part };

        AnimalDataSetup.BodyPart ExtractPositivePart()
        {
            var newPart = new AnimalDataSetup.BodyPart();

            newPart.Origin = part.Origin;
            newPart.Definition = part.Definition;
            newPart.RotLimit = part.RotLimit;
            newPart.RotSpeed = part.RotSpeed;
            newPart.Texture = part.Texture;
            newPart.TexOffset = part.TexOffset;
            newPart.TexScale = part.TexScale;
            newPart.TexRot = part.TexRot;

            for (var i = 0; i < part.Poly.Count; ++i)
            {
                var i0 = i;
                var i1 = i == part.Poly.Count - 1 ? 0 : i + 1;
                var d0 = distances[i0];
                var d1 = distances[i1];
                var p0 = part.Poly[i0];
                var p1 = part.Poly[i1];

                Vector2 IntersectionPoint()
                {
                    if (d1 == d0)
                        return (p0 + p1) / 2;

                    var a = (float)((0 - d0) / (d1 - d0));
                    return p0 * (1 - a) + p1 * a;
                }

                if (d0 > 0)
                {
                    if (part.BloodySegments.Contains(i))
                        newPart.BloodySegments.Add(newPart.Poly.Count);
                    newPart.Poly.Add(p0);
                }

                if (d0 > 0 && d1 <= 0)
                {
                    newPart.BloodySegments.Add(newPart.Poly.Count);
                    newPart.Poly.Add(IntersectionPoint());
                }

                if (d0 <= 0 && d1 > 0)
                {
                    if (part.BloodySegments.Contains(i))
                        newPart.BloodySegments.Add(newPart.Poly.Count);
                    newPart.Poly.Add(IntersectionPoint());
                }
            }

            // parent index
            if (IsPointInside(newPart.Origin, newPart.Poly, newPart.Origin))
                newPart.ParentIndex = part.ParentIndex;

            return newPart;
        }

        var part0 = ExtractPositivePart();
        for (var i = 0; i < distances.Length; ++i)
            distances[i] = -distances[i];
        var part1 = ExtractPositivePart();

        return new List<AnimalDataSetup.BodyPart> { part0, part1 };
    }
}