using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class R2Movement : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField] private List<Vector2> _targetLocations = new();
    [SerializeField] private float _maxSpeed = 10.0f;
    [SerializeField] private float _acceptableRadius = 0.1f;
    [SerializeField] private float _acceptableAngle = 5.0f;
    [SerializeField] private float _maxAngularSpeed = 10.0f;
    [SerializeField] private float _acceleration = 1000.0f;
    [SerializeField] private float _angularAcceleration = 1000.0f;

    public void AddTarget(Vector2 target) => _targetLocations.Add(target);

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;
        var linearVelocity = Vector3.zero;
        var angularVelocity = Vector3.zero;

        if (_targetLocations.Count != 0)
        {
            var target = _targetLocations[0];
            var position = _rb.position;
            var flatPosition = new Vector2(position.x, position.z);
            var offset = target - flatPosition;
            if (offset.sqrMagnitude < _acceptableRadius * _acceptableRadius)
            {
                _targetLocations.RemoveAt(0);
                return;
            }

            var forward = transform.forward;
            var flatForward = new Vector2(forward.x, forward.z).normalized;
            var angle = Vector2.SignedAngle(flatForward, offset);
            var dot = Vector2.Dot(flatForward, offset.normalized);
            var linearSpeed = dot * _maxSpeed;
            var angularSpeed = Mathf.Abs(angle) < _acceptableAngle ? 0 : Mathf.Sign(angle) * _maxAngularSpeed;
            linearVelocity = linearSpeed * transform.forward;
            angularVelocity = angularSpeed * Vector3.up;
        }

        _rb.linearVelocity = Vector3.MoveTowards(_rb.linearVelocity, linearVelocity, _acceleration * dt);
        _rb.angularVelocity = Vector3.MoveTowards(_rb.angularVelocity, angularVelocity, _angularAcceleration * dt);
    }
}
