using System;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

public class Kfs : MonoBehaviour
{
    public enum Type
    {
        Real,
        Fake,
        R1,
    }
    public enum Team
    {
        Red,
        Blue,
    }

    public Type KfsType;
    public Team KfsTeam;
    public int KfsIndex;

    [SerializeField] Vector3 _rotMin = new(-20, 0, -30), _rotMax = new(20, 360, 30);
    [SerializeField] Vector2 _offsetMin = new(-0.7f, -0.7f), _offsetMax = new(0.7f, 0.7f);
    [SerializeField] float _distMin = 10, _distMax = 50;
    [SerializeField] int _datasetCount = 100;
    [SerializeField] float _duration = 0.1f;

    public IEnumerator CreateDataset(LabelGenerator generator, Action onFinish = null)
    {
        for (int i = 0; i < _datasetCount;)
        {
            var rot = RandomExt.RangeVec3(_rotMin, _rotMax);
            var offset = RandomExt.RangeVec2(_offsetMin, _offsetMax);
            var dist = Random.Range(_distMin, _distMax);

            var qrot = Quaternion.Euler(rot);
            var cameraOffset = qrot * (Vector3.forward + (Vector3)offset) * dist;
            generator.transform.SetPositionAndRotation(transform.position - cameraOffset, qrot);

            var label = generator.CreateLabel(transform);
            if (!label.HasValue) continue;

            i++;
            yield return new WaitForSeconds(_duration);
        }

        onFinish?.Invoke();
    }
}
