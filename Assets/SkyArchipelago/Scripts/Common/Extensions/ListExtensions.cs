using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ListExtensions
{
    public static T GetRandom<T>(this IEnumerable<T> list)
    {
        return list.ElementAt(Random.Range(0, list.Count()));
    }

    public static T GetRandomWeighted<T>(this IList<T> list) where T : IWeighted
    {
        if (list == null || list.Count == 0)
            return default;

        if (list.Count == 1)
            return list[0];

        int totalWeight = 0;
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            totalWeight += list[i].Weight;
        }

        if (totalWeight <= 0)
        {
            Debug.LogWarning("Total weight is <= 0 in weighted random selection.");
            return list[0];
        }

        int randomValue = Random.Range(0, totalWeight);

        int current = 0;
        for (int i = 0; i < count; i++)
        {
            var item = list[i];
            current += item.Weight;
            if (randomValue < current)
            {
                return item;
            }
        }

        return list[list.Count - 1];
    }
}