using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(KfsSpawner))]
public class KfsTracker : MonoBehaviour
{
    private KfsSpawner _spawner;
    private List<Kfs>[] _kfss;

    void Start()
    {
        _spawner = GetComponent<KfsSpawner>();
    }

    void Update()
    {
        foreach (var kfs in _spawner.ActiveKfss)
        {
            
        }
    }
}
