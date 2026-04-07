using System.Collections.Generic;
using UnityEngine;

public class R2MotorMovement : MonoBehaviour
{
    [SerializeField] HingeJoint[] _rightWheels;
    [SerializeField] HingeJoint[] _leftWheels;
    [SerializeField] Rigidbody _base;
    [SerializeField] float _forwardSpeed = 10, _turnSpeed = 10, _motorForce = 1000;
    [SerializeField] float _minTargetDistance = 0.1f;
    [SerializeField] float _minAllowedDot = 0.5f;
    [SerializeField] float _minAllowedDegree = 1;
    [SerializeField] List<Vector2> _targets = new();

    public void AddTarget(Vector2 target) => _targets.Add(target);

    void SetMovement(float forward, float turn)
    {
        Debug.Log($"Forward: {forward}, Turn: {turn}");
        foreach (var wheel in _rightWheels)
        {
            var offset = wheel.transform.position - _base.transform.position;
            var velocity = Vector3.Dot(offset * turn, wheel.transform.rotation * wheel.axis);
            wheel.motor = new JointMotor()
            {
                force = _motorForce,
                targetVelocity = velocity + forward
            };
        }
        foreach (var wheel in _leftWheels)
        {
            var offset = wheel.transform.position - _base.transform.position;
            var velocity = Vector3.Dot(offset, wheel.transform.rotation * wheel.axis) * turn + forward;
            wheel.motor = new JointMotor()
            {
                force = _motorForce,
                targetVelocity = velocity
            };
        }
    }

    void FixedUpdate()
    {
        if (_targets.Count == 0) {
            SetMovement(0, 0);
            return;
        }

        var target = _targets[0];
        var position = _base.position;
        var xzPosition = new Vector2(position.x, position.z);
        var targetOffset = target - xzPosition;
        if (targetOffset.sqrMagnitude < _minTargetDistance * _minTargetDistance) { // Equal
            _targets.RemoveAt(0);
            return;
        }

        var forward = _base.transform.forward;
        var xzForward = new Vector2(forward.x, forward.z).normalized;
        var angle = Vector2.SignedAngle(xzForward, targetOffset);
        var dot = Vector2.Dot(xzForward, targetOffset.normalized);
        SetMovement(dot >= _minAllowedDot ? dot * _forwardSpeed : 0, (angle >= _minAllowedDegree ? 1 : angle <= -_minAllowedDegree ? -1 : 0) * _turnSpeed);
    }
}
