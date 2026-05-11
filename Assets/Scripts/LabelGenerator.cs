using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LabelGenerator : MonoBehaviour
{
    private Camera _camera;

    public Rect? GenerateLabel(MeshFilter meshFilter)
    {
        var offset = transform.position - meshFilter.transform.position;
        var hit = Physics.Raycast(meshFilter.transform.position, offset.normalized, offset.magnitude, 1);

        if (hit) return null;

        var bounds = meshFilter.sharedMesh.bounds;
        var min = bounds.min;
        var max = bounds.max;
        var corners = Enumerable.Range(0, 8).Select(i =>
        {
            var corner = meshFilter.transform.TransformPoint(new Vector3(
                (i & 1) == 0 ? min.x : max.x,
                (i & 2) == 0 ? min.y : max.y,
                (i & 4) == 0 ? min.z : max.z
            ));
            return _camera.WorldToViewportPoint(corner);
        });
        if (corners.Any(corner => corner.z <= 0))
            return null;

        var xmin = corners.Min(v => v.x);
        var ymin = corners.Min(v => v.y);
        var xmax = corners.Max(v => v.x);
        var ymax = corners.Max(v => v.y);

        if (float.IsInfinity(xmin) || float.IsInfinity(ymin) || float.IsInfinity(xmax) || float.IsInfinity(ymax))
            return null;

        var xcenter = (xmin + xmax) / 2;
        var ycenter = (ymin + ymax) / 2;

        if (xcenter < 0 || ycenter < 0 || xcenter > 1 || ycenter > 1)
            return null;

        return Rect.MinMaxRect(xmin, ymin, xmax, ymax);
    }
    public static Rect ToGuiRect(Rect rect)
    {
        var position = rect.position;
        var size = rect.size;

        position.y = 1 - position.y - size.y;

        var screenSize = new Vector2(Screen.width, Screen.height);

        return new(position * screenSize, size * screenSize);
    }

    void Start()
    {
        _camera = GetComponent<Camera>();
    }
}
