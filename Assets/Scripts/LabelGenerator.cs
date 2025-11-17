using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class LabelGenerator : MonoBehaviour
{
    public List<Kfs> Kfss = new();

    [SerializeField] int _width = 640, _height = 360;

    Camera _camera;
    Movement _movement;
    RenderTexture _renderTexture;
    InputAction _clickAction, _pointAction;
    bool _clicked, _generating;

    public Rect? CreateLabel(Transform kfs)
    {
        var offset = transform.position - kfs.position;
        var hit = Physics.Raycast(kfs.position, offset.normalized, offset.magnitude, 1);

        if (hit) return null;

        var kfsMesh = kfs.GetComponent<MeshFilter>();
        var kfsBounds = kfsMesh.sharedMesh.bounds;
        var kfsMin = kfsBounds.min;
        var kfsMax = kfsBounds.max;
        var corners = Enumerable.Range(0, 8).Select(i =>
        {
            var corner = kfs.TransformPoint(new Vector3(
                (i & 1) == 0 ? kfsMin.x : kfsMax.x,
                (i & 2) == 0 ? kfsMin.y : kfsMax.y,
                (i & 4) == 0 ? kfsMin.z : kfsMax.z
            ));
            return _camera.WorldToViewportPoint(corner);
        });
        if (corners.Any(corner => corner.z <= 0))
            return null;

        var xmin = corners.Select(corner => corner.x).Min();
        var ymin = corners.Select(corner => corner.y).Min();
        var xmax = corners.Select(corner => corner.x).Max();
        var ymax = corners.Select(corner => corner.y).Max();

        if (!float.IsFinite(xmin) || !float.IsFinite(ymin) || !float.IsFinite(xmax) || !float.IsFinite(ymax))
            return null;

        var xcenter = (xmin + xmax) / 2;
        var ycenter = (ymin + ymax) / 2;

        if (xcenter < 0 || ycenter < 0 || xcenter > 1 || ycenter > 1)
            return null;

        return Rect.MinMaxRect(xmin, ymin, xmax, ymax);
    }
    static Rect ToGuiRect(Rect rect)
    {
        var position = rect.position;
        var size = rect.size;

        position.y = 1 - position.y - size.y;

        var screenSize = new Vector2(Screen.width, Screen.height);

        return new(position * screenSize, size * screenSize);
    }
    public void Render()
    {
        _camera.Render();
    }

    void Start()
    {
        _camera = GetComponent<Camera>();
        _movement = GetComponentInParent<Movement>();
        _clickAction = InputSystem.actions.FindAction("Click");
        _pointAction = InputSystem.actions.FindAction("Point");

        _clickAction.performed += _ => _clicked = true;

        _renderTexture = new(_width, _height, 32);
        _camera.targetTexture = _renderTexture;
    }
    void OnGUI()
    {
        if (_camera != Camera.main) return;
        if (_generating) return;

        foreach (var kfs in Kfss)
        {
            var rect = CreateLabel(kfs.transform);
            if (rect.HasValue)
            {
                Color color;
                if (kfs.KfsTeam == Kfs.Team.Red)
                {
                    color = Color.red;
                }
                else
                {
                    color = Color.blue;
                }
                if (kfs.KfsType == Kfs.Type.Fake)
                {
                    color = Color.black;
                }
                else if (kfs.KfsType == Kfs.Type.R1)
                {
                    color = Color.purple;
                }

                var guiRect = ToGuiRect(rect.Value);
                var guiMin = guiRect.min;
                var guiMax = guiRect.max;
                var mousePos = _pointAction.ReadValue<Vector2>();

                var mouseHover = guiMin.x < mousePos.x && guiMin.y < mousePos.y && guiMax.x > mousePos.x && guiMax.y > mousePos.y;
                color.a = mouseHover ? 0.7f : 0.5f;

                if (_clicked && mouseHover)
                {
                    _generating = true;
                    _movement?.Disable();
                    var oldPosition = transform.position;
                    var oldRotation = transform.rotation;
                    StartCoroutine(kfs.CreateDataset(this, () =>
                    {
                        _generating = false;
                        _movement?.Enable();
                        transform.position = oldPosition;
                        transform.rotation = oldRotation;
                    }));
                    _clicked = false;
                }

                EditorGUI.DrawRect(ToGuiRect(rect.Value), color);
            }
        }

        _clicked = false;
    }
}
