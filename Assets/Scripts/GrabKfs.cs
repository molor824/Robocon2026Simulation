using UnityEngine;
using UnityEngine.InputSystem;

public class GrabKfs : MonoBehaviour
{
    [SerializeField] private GameStateManager _manager;
    [SerializeField] private float _distance = 1;
    [SerializeField] private float _grabDistance = 1;

    private InputAction _equipAction;
    private Rigidbody _target;

    void Start()
    {
        _equipAction = InputSystem.actions.FindAction("Equip");
        _equipAction.started += ctx =>
        {
            if (_target != null)
            {
                _target.isKinematic = false;
                _target = null;
                return;
            }

            var hits = Physics.RaycastAll(transform.position, transform.forward, _distance);
            
            foreach (var hit in hits)
            {
                Debug.Log(hit.transform);
                if (!hit.transform.TryGetComponent(out Kfs kfs)) continue;
                if (kfs.KfsTeam != _manager.Team || kfs.KfsType != Kfs.Type.R1) continue;
                _target = kfs.GetComponent<Rigidbody>();
                break;
            }

            if (_target == null) return;
        };
    }

    void Update()
    {
        if (_target == null) return;

        _target.isKinematic = true;

        var rot = transform.eulerAngles;
        _target.transform.position = transform.position + transform.forward * _grabDistance;
        var targetRot = _target.transform.eulerAngles;
        targetRot.y = rot.y;
        _target.transform.eulerAngles = targetRot;
    }
}