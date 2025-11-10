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
    IEnumerable<Rect> RealLabels(KfsSpawner spawner)
    {
        foreach (var label in spawner.PlacedRealKfss.Select(kfs => CreateLabel(kfs)).Where(label => label.HasValue))
        {
            yield return label.Value;
        }
    }
    IEnumerable<Rect> FakeLabels(KfsSpawner spawner)
    {
        foreach (var label in spawner.PlacedFakeKfss.Select(kfs => CreateLabel(kfs)).Where(label => label.HasValue))
        {
            yield return label.Value;
        }
    }
    IEnumerable<Rect> R1Labels(KfsSpawner spawner)
    {
        foreach (var label in spawner.PlacedR1Kfss.Select(kfs => CreateLabel(kfs)).Where(label => label.HasValue))
        {
            yield return label.Value;
        }
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
        List<Rect> realLabels = new();
        List<Rect> fakeLabels = new();
        List<Rect> r1Labels = new();
        if (_redSpawner != null)
        {
            realLabels.AddRange(RealLabels(_redSpawner));
            fakeLabels.AddRange(FakeLabels(_redSpawner));
            r1Labels.AddRange(R1Labels(_redSpawner));
        }
        if (_blueSpawner != null)
        {
            realLabels.AddRange(RealLabels(_blueSpawner));
            fakeLabels.AddRange(FakeLabels(_blueSpawner));
            r1Labels.AddRange(R1Labels(_blueSpawner));
        }

        foreach (var label in realLabels)
        {
            EditorGUI.DrawRect(ToGuiCoord(label), new(0, 1, 0, 0.4f));
        }
        foreach (var label in fakeLabels)
        {
            EditorGUI.DrawRect(ToGuiCoord(label), new(1, 0, 0, 0.4f));
        }
        foreach (var label in r1Labels)
        {
            EditorGUI.DrawRect(ToGuiCoord(label), new(0, 0, 1, 0.4f));
        }
    }
}
