using System;
using System.Collections.Generic;
using System.Linq;

public class AnimalDistribution
{
    public Dictionary<AnimalType, float> Types = new Dictionary<AnimalType, float>();

    public AnimalType Sample()
    {
        while (true)
        {
            var v = (float)Rng.NextDouble();
            foreach (var kvp in Types)
            {
                if (v <= kvp.Value)
                    return kvp.Key;

                v -= kvp.Value;
            }
        }
    }

    private static Random Rng = new Random();

    public static AnimalDistribution Create()
    {
        var dis = new AnimalDistribution();

        var allTypes = new[]{
            AnimalType.Cat,
            AnimalType.Bear,
            AnimalType.Bunny,
            AnimalType.Piranha,
            AnimalType.Squirrel,
            AnimalType.Elefant,
            AnimalType.Owl,
            AnimalType.Crab,
            AnimalType.Dragon
        };
        while (dis.Types.Count < 3)
        {
            var type = (AnimalType)allTypes[Rng.Next() % allTypes.Length];
            if (dis.Types.ContainsKey(type))
                continue;

            dis.Types.Add(type, (float)Rng.NextDouble());
        }

        // normalize
        var wsum = dis.Types.Values.Sum();
        foreach (var t in dis.Types.Keys.ToArray())
            dis.Types[t] /= wsum;

        return dis;
    }
}