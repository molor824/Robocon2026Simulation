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
}