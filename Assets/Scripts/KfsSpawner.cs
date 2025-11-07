using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class KfsSpawner : MonoBehaviour
{
    private const int TotalSpot = 12;

    private const int MaxRealKfsCount = 4,
        MaxFakeKfsCount = 1,
        MaxR1KfsCount = 3;

    private const float PositionVariation = 0.1f;

    [SerializeField] private Transform _realKfs;
    [SerializeField] private Transform _fakeKfs;
    [SerializeField] private Transform _r1Kfs;

    void Start()
    {
        var realIndices = Enumerable.Range(0, _realKfs.childCount).ToArray();
        var fakeIndices = Enumerable.Range(0, _fakeKfs.childCount).ToArray();
        RandomExt.Shuffle(realIndices);
        RandomExt.Shuffle(fakeIndices);

        var placeOrder = Enumerable.Range(0, TotalSpot).ToArray();
        RandomExt.Shuffle(placeOrder, 3, TotalSpot); // First 3 row must always be placed

        var placedR1Kfs = 0;
        var placedRealKfs = 0;
        var placedFakeKfs = 0;

        for (var order = 0; order < TotalSpot; order++)
        {
            if (placedR1Kfs >= MaxR1KfsCount && placedRealKfs >= MaxRealKfsCount && placedFakeKfs >= MaxFakeKfsCount)
                break;

            int i = placeOrder[order];
            int x = i % 3;
            int y = i / 3;

            bool r1 = (x == 0 || x == 2) && placedR1Kfs < MaxR1KfsCount;
            bool real = placedRealKfs < MaxRealKfsCount;
            bool fake = y != 0 && placedFakeKfs < MaxFakeKfsCount;

            if (!r1 && !real && !fake)
                continue;

            bool[] mask = { r1, real, fake };
            int index = Random.Range(0, 3);
            while (!mask[index])
                index = (index + 1) % 3;

            Transform[] kfss = { _r1Kfs, _realKfs.GetChild(placedRealKfs), _fakeKfs.GetChild(placedFakeKfs) };
            var spawner = transform.GetChild(i);
            var cloned = Instantiate(kfss[index]);
            cloned.localPosition = new Vector3(
                Random.Range(-PositionVariation, PositionVariation),
                Random.Range(-PositionVariation, PositionVariation),
                Random.Range(-PositionVariation, PositionVariation)
            ) + spawner.position;
            cloned.Rotate(Vector3.up, Random.Range(0f, 360f), Space.World);
            cloned.gameObject.SetActive(true);

            switch (index)
            {
                case 0:
                    placedR1Kfs++; break;
                case 1:
                    placedRealKfs++; break;
                case 2:
                    placedFakeKfs++; break;
                default:
                    throw new Exception("Should not reach");
            }
        }

        Debug.Log($"{placedR1Kfs} {placedRealKfs} {placedFakeKfs}");
    }
}