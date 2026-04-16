using System.Collections;
using UnityEngine;

public class R2GrabKfs : MonoBehaviour
{
    [SerializeField] private Transform _pivot0, _pivot1, _pivot2;
    [SerializeField] private float _extendDuration = 1;
    [SerializeField] private float _collapseDuration = 1;
    [SerializeField] private GameStateManager _manager;
    [SerializeField] private bool _grabbing;

    private Transform _targetKfs;

    public bool Grabbing => _grabbing;

    void Start()
    {
        _pivot0.localEulerAngles = -45 * Vector3.right;
        _pivot1.localEulerAngles = 45 * Vector3.right;
    }

    public void StartGrab(Kfs targetKfs)
    {
        if (_targetKfs != null) return;
        if (_grabbing) return;
        _grabbing = true;
        _targetKfs = targetKfs.transform;
        StartCoroutine(GrabCoroutine());
    }

    IEnumerator GrabCoroutine()
    {
        float elapsed = 0;
        while (elapsed < _extendDuration)
        {
            var t = elapsed / _extendDuration;
            _pivot0.localEulerAngles = Vector3.right * Mathf.Lerp(-45, 45, t);
            _pivot1.localEulerAngles = Vector3.right * Mathf.Lerp(45, -45, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _pivot0.localEulerAngles = Vector3.right * 45;
        _pivot1.localEulerAngles = Vector3.right * -45;

        var kfsRb = _targetKfs.GetComponent<Rigidbody>();
        kfsRb.isKinematic = true;

        _targetKfs.SetParent(_pivot2);
        _targetKfs.localPosition = Vector3.zero;

        elapsed = 0;
        while (elapsed < _collapseDuration)
        {
            var t = elapsed / _collapseDuration;
            _pivot0.localEulerAngles = Vector3.right * Mathf.Lerp(45, -45, t);
            _pivot1.localEulerAngles = Vector3.right * Mathf.Lerp(-45, 45, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _pivot0.localEulerAngles = Vector3.right * -45;
        _pivot1.localEulerAngles = Vector3.right * 45;

        _grabbing = false;
    }
}
