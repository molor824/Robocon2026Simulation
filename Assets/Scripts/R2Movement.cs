using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class R2Movement : MonoBehaviour
{
    private CharacterController _cc;
    private float _verticalVelocity;

    [SerializeField] private List<Ray2D> _targetRays = new();
    [SerializeField] private float _maxSpeed = 10.0f;
    [SerializeField] private float _acceptableRadius = 0.1f;
    [SerializeField] private float _acceptableAngle = 1.0f;
    [SerializeField] private float _maxAngularSpeed = 10.0f;
    [SerializeField] private float _gravity = 1000.0f;

    public void AddTarget(Ray2D target) => _targetRays.Add(target);

    public IReadOnlyList<Ray2D> Targets => _targetRays;

    void Start()
    {
        _cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        var dt = Time.deltaTime;
        var linearVelocity = Vector3.zero;
        var angularVelocity = 0f;

        if (_targetRays.Count != 0)
        {
            var targetRay = _targetRays[0];
            var position = transform.position;
            var flatPosition = new Vector2(position.x, position.z);
            var offset = targetRay.origin - flatPosition;
            var forward = transform.forward;
            var flatForward = new Vector2(forward.x, forward.z).normalized;

            if (offset.sqrMagnitude < _acceptableRadius * _acceptableRadius)
            {
                // Within radius: turn to face the ray direction
                // NOTE: Negative because our rotation is clockwise in the positive direction
                // But in math, its usually counter clockwise
                var angle = -Vector2.SignedAngle(flatForward, targetRay.direction);
                if (Mathf.Abs(angle) < _acceptableAngle)
                {
                    _targetRays.RemoveAt(0);
                }
                else
                {
                    angularVelocity = Mathf.Sign(angle) * _maxAngularSpeed;
                }
            }
            else
            {
                // Outside radius: first turn to face target, then move
                var angle = -Vector2.SignedAngle(flatForward, offset);
                angularVelocity = Mathf.Abs(angle) < _acceptableAngle ? 0 : Mathf.Sign(angle) * _maxAngularSpeed;
                if (Mathf.Abs(angle) < _acceptableAngle)
                {
                    linearVelocity = _maxSpeed * transform.forward;
                }
            }
        }

        if (_cc.isGrounded)
            _verticalVelocity = 0f;
        else
            _verticalVelocity -= _gravity * dt;

        linearVelocity.y = _verticalVelocity;

        transform.Rotate(Vector3.up, angularVelocity * dt);
        _cc.Move(linearVelocity * dt);
    }
}
