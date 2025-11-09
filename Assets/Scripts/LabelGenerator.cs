using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LabelGenerator : MonoBehaviour
{
    [SerializeField] KfsSpawner _redSpawner, _blueSpawner;

    Rect? CreateLabel(Transform kfs)
    {
        var offset = transform.position - kfs.position;
        var hit = Physics.Raycast(transform.position, offset.normalized, offset.magnitude, 1);

        if (hit) return null;

        var kfsMesh = kfs.GetComponent<MeshFilter>();
        var kfsBounds = kfsMesh.sharedMesh.bounds;
        var kfsMin = kfsBounds.min;
        var kfsMax = kfsBounds.max;
        var corners = Enumerable.Range(0, 8).Select(i =>
        {
            var corner = kfs.TransformPoint(new Vector3(
                (i & 1) == 0 ? kfsMin.x : kfsMax.x,
                (i & 2) == 0 ? kfsMin.y : kfsMax.y,
                (i & 4) == 0 ? kfsMin.z : kfsMax.z
            ));
            return Camera.main.WorldToScreenPoint(corner);
        });
        if (corners.Any(corner => corner.z <= 0))
            return null;

        var xmin = corners.Select(corner => corner.x).Min();
        var ymin = corners.Select(corner => corner.y).Min();
        var xmax = corners.Select(corner => corner.x).Max();
        var ymax = corners.Select(corner => corner.y).Max();

        if (!float.IsFinite(xmin) || !float.IsFinite(ymin) || !float.IsFinite(xmax) || !float.IsFinite(ymax))
            return null;
        if (xmax < 0 || ymax < 0
            || xmin >= Screen.width || ymin >= Screen.height)
            return null;

        return Rect.MinMaxRect(xmin, ymin, xmax, ymax);
    }
    IEnumerable<Rect> RealKfsLabel(KfsSpawner spawner)
    {
        foreach (var kfs in spawner.PlacedRealKfss)
        {
            var label = CreateLabel(kfs);
            if (label.HasValue)
                yield return label.Value;
        }
    }
    void OnGUI()
    {
        List<Rect> labels = new();
        if (_redSpawner != null)
            labels.AddRange(RealKfsLabel(_redSpawner));
        if (_blueSpawner != null)
            labels.AddRange(RealKfsLabel(_blueSpawner));

        foreach (var label in labels)
        {
            EditorGUI.DrawRect(
                new(new Vector2(0, Screen.height - label.size.y)
                    + Vector2.Scale(label.position, new(1, -1)), label.size),
                new(0, 1, 0, 0.5f));
        }
    }
}
