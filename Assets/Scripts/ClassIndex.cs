using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public abstract class ClassIndex : MonoBehaviour
{
    public abstract int Index { get; }
}
