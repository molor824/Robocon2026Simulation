using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Spearhead : MonoBehaviour
{
    [SerializeField] private SpearType Type;

    private MeshFilter _meshFilter;

    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }
    Rect? GetLabel(Camera camera)
    {
        var vertices = _meshFilter.sharedMesh.vertices;
        var projectedVertices = vertices.Select(camera.WorldToViewportPoint);
        if (projectedVertices.Any(v => v.z < 0)) return null;

        var minBounding = new Vector2(
            projectedVertices.Min(v => v.x),
            projectedVertices.Min(v => v.y)
        );
        var maxBounding = new Vector2(
            projectedVertices.Max(v => v.x),
            projectedVertices.Max(v => v.y)
        );
        var center = (minBounding + maxBounding) * 0.5f;

        if (center.x > 1 || center.x < 0 || center.y > 1 || center.y < 0) return null;

        return new Rect(minBounding, maxBounding - minBounding);
    }
}
public enum SpearType
{
    Spear,
    Hand,
    Fist,
}