using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

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

        var xmin = corners.Select(corner => corner.x).Min();
        var ymin = corners.Select(corner => corner.y).Min();
        var xmax = corners.Select(corner => corner.x).Max();
        var ymax = corners.Select(corner => corner.y).Max();

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
