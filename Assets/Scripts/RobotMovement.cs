using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RobotMovement : MonoBehaviour
{
    Rigidbody _rb;
    Queue<Vector2> _positionQueue = new();

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        
    }
}
