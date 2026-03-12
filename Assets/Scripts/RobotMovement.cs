using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RobotMovement : MonoBehaviour
{
    Rigidbody _rb;
    
    public Vector2 Target;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;
    }
}
