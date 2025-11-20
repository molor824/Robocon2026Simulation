using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LabelGenerator))]
public class LabelUI : MonoBehaviour
{
    bool _clicked, _generating;
    Movement _movement;
    InputAction _clickAction, _pointAction;
    LabelGenerator _labelGenerator;

    void Start()
    {
        _clickAction = InputSystem.actions.FindAction("Click");
        _pointAction = InputSystem.actions.FindAction("Point");

        _clickAction.performed += _ => _clicked = true;

        _movement = GetComponentInParent<Movement>();
        _labelGenerator = GetComponent<LabelGenerator>();
    }
    
    static Rect ToGuiRect(Rect rect)
    {
        var position = rect.position;
        var size = rect.size;

        position.y = 1 - position.y - size.y;

        var screenSize = new Vector2(Screen.width, Screen.height);

        return new(position * screenSize, size * screenSize);
    }
    void OnGUI()
    {
        if (_generating) return;

        foreach (var kfs in _labelGenerator.Kfss)
        {
            var rect = _labelGenerator.CreateLabel(kfs.transform);
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
                    StartCoroutine(kfs.CreateDataset(_labelGenerator, () =>
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