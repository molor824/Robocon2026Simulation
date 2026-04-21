using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TicTacCell : MonoBehaviour
{
    private List<Collider> _colliders = new();
    private List<Kfs> _kfss = new();

    public Kfs GetKfs()
    {
        if (_kfss.Count == 0) return null;
        return _kfss[_kfss.Count - 1];
    }

    void OnTriggerEnter(Collider other)
    {
        _colliders.Add(other);
        if (other.TryGetComponent(out Kfs kfs))
            _kfss.Add(kfs);
    }
    void OnTriggerExit(Collider other)
    {
        _colliders.Remove(other);
        if (other.TryGetComponent(out Kfs kfs))
            _kfss.Remove(kfs);
    }
}
