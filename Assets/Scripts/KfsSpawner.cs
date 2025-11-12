using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class KfsSpawner : MonoBehaviour
{
    const int TotalSpot = 12;

    const int MaxRealKfsCount = 4,
        MaxFakeKfsCount = 1,
        MaxR1KfsCount = 3;

    const float PositionVariation = 0.1f;

    [SerializeField] Transform _realKfs;
    [SerializeField] Transform _fakeKfs;
    [SerializeField] Transform _r1Kfs;
    [SerializeField] LabelGenerator _labelGenerator;

    List<Kfs> _placedRealKfss = new();
    List<Kfs> _placedFakeKfss = new();
    List<Kfs> _placedR1Kfss = new();

    void Start()
    {
        var realIndices = Enumerable.Range(0, _realKfs.childCount).ToArray();
        var fakeIndices = Enumerable.Range(0, _fakeKfs.childCount).ToArray();
        RandomExt.Shuffle(realIndices);
        RandomExt.Shuffle(fakeIndices);

        var placeOrder = Enumerable.Range(0, TotalSpot).ToArray();
        RandomExt.Shuffle(placeOrder, 3, TotalSpot); // First 3 row must always be placed

        for (var order = 0; order < TotalSpot; order++)
        {
            int r1Count = _placedR1Kfss.Count;
            int realCount = _placedRealKfss.Count;
            int fakeCount = _placedFakeKfss.Count;

            if (r1Count >= MaxR1KfsCount
                && realCount >= MaxRealKfsCount
                && _placedFakeKfss.Count >= MaxFakeKfsCount)
                break;

            int i = placeOrder[order];
            int x = i % 3;
            int y = i / 3;

            bool r1 = (x == 0 || x == 2) && r1Count < MaxR1KfsCount;
            bool real = realCount < MaxRealKfsCount;
            bool fake = y != 0 && fakeCount < MaxFakeKfsCount;

            if (!r1 && !real && !fake)
                continue;

            bool[] mask = { r1, real, fake };
            int index = Random.Range(0, 3);
            while (!mask[index])
                index = (index + 1) % 3;

            Transform[] kfss = { _r1Kfs, _realKfs.GetChild(realCount), _fakeKfs.GetChild(fakeCount) };
            var spawner = transform.GetChild(i);
            var cloned = Instantiate(kfss[index]);
            cloned.localPosition = new Vector3(
                Random.Range(-PositionVariation, PositionVariation),
                Random.Range(-PositionVariation, PositionVariation),
                Random.Range(-PositionVariation, PositionVariation)
            ) + spawner.position;
            cloned.Rotate(Vector3.up, Random.Range(0f, 360f), Space.World);
            cloned.gameObject.SetActive(true);

            var kfs = cloned.GetComponent<Kfs>();
            _labelGenerator.Kfss.Add(kfs);

            switch (index)
            {
                case 0:
                    _placedR1Kfss.Add(kfs);
                    break;
                case 1:
                    _placedRealKfss.Add(kfs);
                    break;
                case 2:
                    _placedFakeKfss.Add(kfs);
                    break;
                default:
                    throw new Exception("Should not reach");
            }
        }
    }
}