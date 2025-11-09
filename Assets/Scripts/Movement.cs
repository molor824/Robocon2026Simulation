using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    [SerializeField] float _speed = 1f;
    [SerializeField] float _sensitivity = 1f;
    [SerializeField] float _gravity = 10f;

    InputAction _moveAction, _lookAction, _pauseAction;
    CharacterController _controller;
    Transform _camera;

    Vector2 _axis;
    bool _paused;

    Vector2 Axis
    {
        get => _axis;
        set
        {
            _axis.x = Mathf.Clamp(value.x, -90f, 90f);
            _axis.y = value.y;
            transform.localRotation = Quaternion.AngleAxis(_axis.y, Vector3.up);
            _camera.localRotation = Quaternion.AngleAxis(_axis.x, Vector3.right);
        }
    }

    bool Paused
    {
        get => _paused;
        set
        {
            _paused = value;
            Cursor.lockState = !_paused ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
    
    void Start()
    {
        _camera = GetComponentInChildren<Camera>().transform;
        
        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _pauseAction = InputSystem.actions.FindAction("Pause");
        _controller = GetComponent<CharacterController>();
        
        _lookAction.performed += ctx =>
        {
            if (Paused) return;
            var mouseMotion = ctx.ReadValue<Vector2>();
            Axis += new Vector2(-mouseMotion.y, mouseMotion.x) * _sensitivity;
        };
        _pauseAction.started += _ => Paused = !Paused;
        Paused = false;
    }

    void Update()
    {
        var moveValue = _moveAction.ReadValue<Vector2>();
        var direction = transform.localRotation * new Vector3(moveValue.x, 0, moveValue.y);
        var velocity = _controller.velocity;
        
        velocity.x = direction.x * _speed;
        velocity.z = direction.z * _speed;
        
        velocity.y -= _gravity * Time.deltaTime;
        _controller.Move(velocity * Time.deltaTime);
    }
}
