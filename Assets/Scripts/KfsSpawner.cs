using System.Collections.Generic;
using UnityEngine;

public class KfsSpawner : MonoBehaviour
{
    private static readonly Vector2 Size = new(1.2f, 1.2f);

    [SerializeField] private Transform _kfsContainer;
    [SerializeField] private Vector2 _positionVariation = new(0.2f, 0.2f);
    [SerializeField] private bool _random = true;

    private Transform[] _spawners;
    private Kfs[] _kfss;
    private List<Kfs> _activeKfss = new();

    public IReadOnlyList<Kfs> ActiveKfss => _activeKfss;
    public IReadOnlyList<Transform> Spawners => _spawners;

    public Kfs AtSpawner(int index)
    {
        var spawner = _spawners[index];
        var position = spawner.position;
        var start = new Vector2(position.x, position.z) - Size * 0.5f;
        var end = start + Size;

        foreach (var kfs in _activeKfss)
        {
            var kfsPos = kfs.transform.position;
            if (start.x <= kfsPos.x && end.x >= kfsPos.x && start.y <= kfsPos.z && end.y >= kfsPos.z)
            {
                return kfs;
            }
        }
        return null;
    }

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

        if (_random)
            SpawnRandomKfss();
    }

    public void SpawnKfsAt(int index, Kfs.Team team, Kfs.Type type)
    {
        var kfsOrder = new int[_kfss.Length];
        for (int i = 0; i < kfsOrder.Length; i++)
            kfsOrder[i] = i;
        RandomExt.Shuffle(kfsOrder);

        var spawner = _spawners[team switch
        {
            Kfs.Team.Red => index,
            _ => 12 + index,
        }];
        Debug.Log(index);

        for (int i = 0; i < kfsOrder.Length; i++)
        {
            var kfs = _kfss[kfsOrder[i]];
            if (kfs.KfsTeam == team && kfs.KfsType == type)
            {
                var kfs1 = Instantiate(kfs);
                var offset = RandomExt.RangeVec2(-_positionVariation, _positionVariation);

                kfs1.gameObject.SetActive(true);
                kfs1.transform.position = spawner.position + new Vector3(offset.x, 0.0f, offset.y);
                kfs1.transform.rotation = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up) * spawner.rotation;
                _activeKfss.Add(kfs1);
                return;
            }
        }
    }

    public void SpawnRandomKfss()
    {
        var kfsOrder = new int[_kfss.Length];
        for (int i = 0; i < _kfss.Length; i++)
            kfsOrder[i] = i;
        RandomExt.Shuffle(kfsOrder, 0, _kfss.Length / 2);
        RandomExt.Shuffle(kfsOrder, _kfss.Length / 2, _kfss.Length);

        foreach (var kfs in _kfss)
        {
            kfs.gameObject.SetActive(false);
        }
        _activeKfss.Clear();
        for (int i = 0; i < _spawners.Length; i++)
        {
            var kfs = _kfss[i >= _spawners.Length / 2 ? kfsOrder[i] : kfsOrder[i + _kfss.Length / 2]];
            var offset = RandomExt.RangeVec2(-_positionVariation, _positionVariation);

            kfs.gameObject.SetActive(true);
            kfs.transform.position = _spawners[i].position + new Vector3(offset.x, 0.0f, offset.y);
            kfs.transform.rotation = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up) * _spawners[i].rotation;
            _activeKfss.Add(kfs);
        }
    }
}