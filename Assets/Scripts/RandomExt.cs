using System.Collections.Generic;
using UnityEngine;

public static class RandomExt
{
    public static void Shuffle<T>(IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int swapIndex = Random.Range(0, list.Count);
            (list[i], list[swapIndex]) = (list[swapIndex], list[i]);
        }
    }

    public static void Shuffle<T>(IList<T> list, int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            int swapIndex = Random.Range(start, end);
            (list[i], list[swapIndex]) = (list[swapIndex], list[i]);
        }
    }

    public static Vector2 RangeVec2(Vector2 min, Vector2 max) => new(
        Random.Range(min.x, max.x),
        Random.Range(min.y, max.y)
    );
    public static Vector3 RangeVec3(Vector3 min, Vector3 max) => new(
        Random.Range(min.x, max.x),
        Random.Range(min.y, max.y),
        Random.Range(min.z, max.z)
    );
}