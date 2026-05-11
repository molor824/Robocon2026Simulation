using System.Collections.Generic;
using UnityEngine;

public class SpearheadSpawner : MonoBehaviour
{
    [SerializeField] private Spearhead[] _spearheads;

    private List<Spearhead> _spawnedSpears = new();

    public IReadOnlyList<Spearhead> SpawnedSpears => _spawnedSpears;

    public void SpawnRandom()
    {
        foreach (var spear in _spawnedSpears)
            Destroy(spear.gameObject);
        _spawnedSpears.Clear();

        var childCount = transform.childCount;
        var spawnOrder = new int[childCount];

        for (int i = 0; i < childCount; i++)
            spawnOrder[i] = i;
        
        RandomExt.Shuffle(spawnOrder);

        for (int i = 0; i < childCount; i++)
        {
            var spearId = Random.Range(0, _spearheads.Length);
            var spearhead = Instantiate(_spearheads[spearId]);
            spearhead.gameObject.SetActive(true);
            spearhead.transform.position = transform.GetChild(i).position;
            spearhead.transform.Rotate(Vector3.up * Random.Range(0f, 360f));
            var meshRenderer = spearhead.GetComponent<MeshRenderer>();
            meshRenderer.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

            _spawnedSpears.Add(spearhead);
        }
    }

    void Start()
    {
        SpawnRandom();
    }
}
