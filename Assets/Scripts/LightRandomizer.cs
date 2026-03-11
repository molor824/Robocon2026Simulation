using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightRandomizer : MonoBehaviour
{
    [SerializeField] private Vector3 _minRot = new(20, 0, 0), _maxRot = new(80, 360, 0);
    [SerializeField] private float _minTemp = 5500, _maxTemp = 7500;
    [SerializeField] private float _minIntensity = 60000, _maxIntensity = 120000;

    public void Randomize()
    {
        var light = GetComponent<Light>();

        transform.localEulerAngles = RandomExt.RangeVec3(_minRot, _maxRot);
        light.colorTemperature = Random.Range(_minTemp, _maxTemp);
        light.intensity = Random.Range(_minIntensity, _maxIntensity);
    }
}
