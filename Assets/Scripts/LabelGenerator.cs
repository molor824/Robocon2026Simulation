using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LabelGenerator : MonoBehaviour
{
    Camera _camera;

    public Rect? GenerateLabel(Transform kfs)
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
            return _camera.WorldToViewportPoint(corner);
        });
        if (corners.Any(corner => corner.z <= 0))
            return null;

        var xmin = corners.Min(corner => corner.x);
        var ymin = corners.Min(corner => corner.y);
        var xmax = corners.Max(corner => corner.x);
        var ymax = corners.Max(corner => corner.y);

        if (!float.IsFinite(xmin) || !float.IsFinite(ymin) || !float.IsFinite(xmax) || !float.IsFinite(ymax))
            return null;

        var xcenter = (xmin + xmax) / 2;
        var ycenter = (ymin + ymax) / 2;

        if (xcenter < 0 || ycenter < 0 || xcenter > 1 || ycenter > 1)
            return null;

        return Rect.MinMaxRect(xmin, ymin, xmax, ymax);
    }

    void Start()
    {
        _camera = GetComponent<Camera>();
    }
}
