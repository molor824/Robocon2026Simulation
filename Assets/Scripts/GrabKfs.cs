using System.Collections;
using UnityEngine;

public class GrabKfs : MonoBehaviour
{
    [SerializeField] private Transform _pivot0, _pivot1, _pivot2;
    [SerializeField] private float _extendDuration = 1;
    [SerializeField] private float _collapseDuration = 1;
    [SerializeField] private Transform _raySource;
    [SerializeField] private float _rayDistance = 2;
    [SerializeField] private GameStateManager _manager;
    [SerializeField] private bool _grabbing;

    void Start()
    {
        _pivot0.localEulerAngles = -45 * Vector3.right;
        _pivot1.localEulerAngles = 45 * Vector3.right;
    }

    public void StartGrab()
    {
        if (_grabbing) return;
        StartCoroutine(GrabCoroutine());
    }

    IEnumerator GrabCoroutine()
    {
        _grabbing = true;
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

        var hits = Physics.RaycastAll(_raySource.position, _raySource.forward, _rayDistance);
        Transform targetKfs = null;
        foreach (var hit in hits)
        {
            if (hit.transform.TryGetComponent(out Kfs kfs))
            {
                if (kfs.KfsType != Kfs.Type.Real || kfs.KfsTeam != _manager.Team) break;
                targetKfs = kfs.transform;
            }
        }

        if (targetKfs == null)
        {
            _manager.RequestRetry();
            _grabbing = false;
            yield break;
        }

        var kfsRb = targetKfs.GetComponent<Rigidbody>();
        kfsRb.Sleep();

        targetKfs.SetParent(_pivot2);
        targetKfs.position = _pivot2.position;

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
