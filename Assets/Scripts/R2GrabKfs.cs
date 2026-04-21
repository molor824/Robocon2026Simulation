using System.Collections;
using UnityEngine;

[RequireComponent(typeof(R2Movement))]
public class R2GrabKfs : MonoBehaviour
{
    [SerializeField] private Transform _pivot0, _pivot1, _pivot2;
    [SerializeField] private float _extendDuration = 1;
    [SerializeField] private float _collapseDuration = 1;
    [SerializeField] private GameStateManager _manager;
    [SerializeField] private bool _grabbing;
    [SerializeField] private float _collapseAngle0 = -30, _collapseAngle1 = 30;
    [SerializeField] private float _extendAngle0 = 30, _extendAngle1 = -30;
    [SerializeField] private float _releaseAngle0 = 0, _releaseAngle1 = -30;
    [SerializeField] private CameraStream _cameraStream;

    private Transform _targetKfs;
    private int _targetIndex;
    private R2Movement _movement;
    private LabelDecoder _labelDecoder;
    private bool _streamResponded;

    public bool Grabbing => _grabbing;

    void Start()
    {
        SetPivot(_collapseAngle0, _collapseAngle1);
        _movement = GetComponent<R2Movement>();
        _labelDecoder = _cameraStream.GetComponent<LabelDecoder>();
        _cameraStream.Responded += () => _streamResponded = true;
    }

    public void StartGrab(Kfs targetKfs)
    {
        if (_targetKfs != null) return;
        if (_grabbing) return;
        _grabbing = true;
        _targetIndex = targetKfs.GetIndex();
        StartCoroutine(GrabCoroutine());
    }
    public void StartRelease()
    {
        if (_targetKfs == null) return;
        if (_grabbing) return;
        _grabbing = true;
        StartCoroutine(ReleaseRoutine());
    }

    void SetPivot(float pivot0, float pivot1)
    {
        _pivot0.localEulerAngles = Vector3.right * pivot0;
        _pivot1.localEulerAngles = Vector3.right * pivot1;
    }

    IEnumerator GrabCoroutine()
    {
        // Wait until new frame is detected
        var camera = _cameraStream.GetComponent<Camera>();

        while (true) {
            _streamResponded = false;
            yield return new WaitUntil(() => _streamResponded);

            var labels = _labelDecoder.Labels;
            Label? label = null;
            foreach (var label1 in labels)
            {
                if (label1.Index == _targetIndex)
                {
                    label = label1;
                    break;
                }
            }
            if (label == null) continue;

            var center = label.Value.Box.center;
            var point = camera.ViewportToWorldPoint(new Vector3(
                center.x,
                1 - center.y,
                1
            ));
            var offset = point - camera.transform.position;

            Debug.DrawRay(camera.transform.position, offset.normalized);

            var hits = Physics.RaycastAll(camera.transform.position, offset.normalized, 10);

            foreach (var hit in hits)
            {
                if (hit.transform.TryGetComponent(out Kfs kfs))
                {
                    _targetKfs = kfs.transform;
                    break;
                }
            }

            if (_targetKfs == null)
            {
                Debug.LogWarning("No KFS Found in the detected KFS path...");
                continue;
            }

            offset = _targetKfs.transform.position - transform.position;
            _movement.AddTarget(new(new(transform.position.x, transform.position.z), new Vector2(offset.x, offset.z)));

            break;
        }

        yield return new WaitUntil(() => _movement.Targets.Count == 0);

        float elapsed = 0;
        while (elapsed < _extendDuration)
        {
            elapsed = Mathf.Min(_extendDuration, elapsed + Time.deltaTime);
            var t = elapsed / _extendDuration;
            SetPivot(Mathf.Lerp(_collapseAngle0, _extendAngle0, t), Mathf.Lerp(_collapseAngle1, _extendAngle1, t));
            yield return null;
        }

        var kfsRb = _targetKfs.GetComponent<Rigidbody>();
        kfsRb.isKinematic = true;

        _targetKfs.SetParent(_pivot2);
        _targetKfs.localPosition = Vector3.zero;

        elapsed = 0;
        while (elapsed < _collapseDuration)
        {
            elapsed = Mathf.Min(_collapseDuration, elapsed + Time.deltaTime);
            var t = elapsed / _collapseDuration;
            SetPivot(Mathf.Lerp(_extendAngle0, _collapseAngle0, t), Mathf.Lerp(_extendAngle1, _collapseAngle1, t));
            yield return null;
        }

        _grabbing = false;
    }

    IEnumerator ReleaseRoutine()
    {
        float elapsed = 0;
        while (elapsed < _extendDuration)
        {
            elapsed = Mathf.Min(_extendDuration, elapsed + Time.deltaTime);
            var t = elapsed / _extendDuration;
            SetPivot(Mathf.Lerp(_collapseAngle0, _releaseAngle0, t), Mathf.Lerp(_collapseAngle1, _releaseAngle1, t));
            yield return null;
        }

        _targetKfs.SetParent(null, true);
        _targetKfs.GetComponent<Rigidbody>().isKinematic = false;
        _targetKfs = null;

        elapsed = 0;
        while (elapsed < _collapseDuration)
        {
            elapsed = Mathf.Min(_collapseDuration, elapsed + Time.deltaTime);
            var t = elapsed / _collapseDuration;
            SetPivot(Mathf.Lerp(_releaseAngle0, _collapseAngle0, t), Mathf.Lerp(_releaseAngle1, _collapseAngle1, t));
            yield return null;
        }
        _grabbing = false;
    }
}
