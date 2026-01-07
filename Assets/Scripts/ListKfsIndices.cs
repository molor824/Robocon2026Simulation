using System;
using System.Text;
using UnityEngine;

public class ListKfsIndices : MonoBehaviour
{
    void Start()
    {
        var kfss = new Kfs[Kfs.MaxIndex + 1];

        for (var i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (!child.TryGetComponent(out Kfs kfs)) continue;
            var index = kfs.GetIndex();
            kfss[index] = kfs;
        }

        var builder = new StringBuilder();

        for (var i = 0; i < kfss.Length; i++)
        {
            builder.AppendLine($"{i}: {kfss[i].name}");
        }

        Debug.Log(builder);
    }
}