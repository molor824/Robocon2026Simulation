using System.Collections.Generic;
using UnityEngine;

public class KfsSpawner : MonoBehaviour
{
    [SerializeField] Transform _kfsContainer;
    [SerializeField] Vector2 _positionVariation = new(0.2f, 0.2f);

    Transform[] _spawners;
    Kfs[] _kfss;
    List<Kfs> _activeKfss = new();

    public IReadOnlyList<Kfs> ActiveKfss => _activeKfss;

    void Start()
    {
        _spawners = new Transform[transform.childCount];
        for (var i = 0; i < transform.childCount; i++)
        {
            _spawners[i] = transform.GetChild(i);
        }
        _kfss = new Kfs[_kfsContainer.childCount];
        for (var i = 0; i < _kfsContainer.childCount; i++)
        {
            _kfss[i] = _kfsContainer.GetChild(i).GetComponent<Kfs>();
            _kfss[i].gameObject.SetActive(false);
        }

        SpawnKfss();
    }

    public void SpawnKfss()
    {
        var kfsOrder = new int[_kfss.Length];
        for (int i = 0; i < _kfss.Length; i++)
            kfsOrder[i] = i;
        RandomExt.Shuffle(kfsOrder);

        foreach (var kfs in _kfss)
        {
            kfs.gameObject.SetActive(false);
        }
        _activeKfss.Clear();
        for (int i = 0; i < _spawners.Length; i++)
        {
            var kfs = _kfss[kfsOrder[i]];
            var offset = RandomExt.RangeVec2(-_positionVariation, _positionVariation);

            kfs.gameObject.SetActive(true);
            kfs.transform.position = _spawners[i].position + new Vector3(offset.x, 0.0f, offset.y);
            kfs.transform.rotation = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up) * _spawners[i].rotation;
            _activeKfss.Add(kfs);
        }
    }
}