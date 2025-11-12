using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LabelGenerator : MonoBehaviour
{
    public List<Kfs> Kfss = new();

    Rect? CreateLabel(Transform kfs)
    {
        var offset = transform.position - kfs.position;
        var hit = Physics.Raycast(kfs.position, offset.normalized, offset.magnitude, 1);

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
    static Rect ToGuiCoord(Rect rect)
    {
        var position = rect.position;
        var size = rect.size;
        return new(
            position.x, Screen.height - size.y - position.y, size.x, size.y
        );
    }
    void OnGUI()
    {
        foreach (var kfs in Kfss)
        {
            var rect = CreateLabel(kfs.transform);
            if (rect.HasValue)
            {
                Color color;
                if (kfs.KfsTeam == Kfs.Team.Red)
                {
                    color = Color.red;
                }
                else
                {
                    color = Color.blue;
                }
                if (kfs.KfsType == Kfs.Type.Fake)
                {
                    color = Color.black;
                }
                else if (kfs.KfsType == Kfs.Type.R1)
                {
                    color = Color.purple;
                }
                color.a = 0.5f;
                EditorGUI.DrawRect(ToGuiCoord(rect.Value), color);
            }
        }
    }
}
