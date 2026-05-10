using System.Collections.Generic;
using UnityEngine;

public class SpearheadSpawner : MonoBehaviour
{
    [SerializeField] private Spearhead[] _spearheads;

    private List<Spearhead> _spawnedSpears = new();

    public void SpawnRandom()
    {
        foreach (var spear in _spawnedSpears)
            GameObject.Destroy(spear);
        _spawnedSpears.Clear();
    }
}
