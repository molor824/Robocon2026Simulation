using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

using Random = UnityEngine.Random;

[RequireComponent(typeof(LabelGenerator))]
public class LabelUI : MonoBehaviour
{
    bool _clicked, _generating;
    InputAction _clickAction, _pointAction;
    LabelGenerator _labelGenerator;

    [SerializeField] DatasetGenerator _datasetGenerator;
    [SerializeField] List<Kfs> _kfss = new();

    [SerializeField] Vector3 _rotMin = new(-20, 0, -30), _rotMax = new(20, 360, 30);
    [SerializeField] Vector2 _offsetMin = new(-0.2f, -0.2f), _offsetMax = new(0.2f, 0.2f);
    [SerializeField] float _distMin = 2, _distMax = 10;
    [SerializeField] int _datasetCount = 100;

    [SerializeField] bool _groupDataset = false;

    void Start()
    {
        _clickAction = InputSystem.actions.FindAction("Click");
        _pointAction = InputSystem.actions.FindAction("Point");

        _clickAction.performed += _ => _clicked = true;

        _labelGenerator = GetComponent<LabelGenerator>();
    }

    public IEnumerator GenerateDatasets(Kfs kfs, DatasetGenerator generator)
    {
        for (int i = 0; i < _datasetCount;)
        {
            var rot = RandomExt.RangeVec3(_rotMin, _rotMax);
            var offset = RandomExt.RangeVec2(_offsetMin, _offsetMax);
            var dist = Random.Range(_distMin, _distMax);

            var qrot = Quaternion.Euler(rot);
            var cameraOffset = qrot * (Vector3.forward + (Vector3)offset) * dist;
            generator.transform.SetPositionAndRotation(kfs.transform.position - cameraOffset, qrot);

            var task = generator.GenerateDataset($"{i}", _groupDataset ? _kfss : Enumerable.Repeat(kfs, 1));
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.Result) i++;
        }
        _generating = false;
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
        foreach (var kfs in _kfss)
        {
            var rect = _labelGenerator.GenerateLabel(kfs.transform);
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

                if (_clicked && mouseHover && !_generating)
                {
                    _generating = true;
                    StartCoroutine(GenerateDatasets(kfs, _datasetGenerator));
                    _clicked = false;
                }

                EditorGUI.DrawRect(ToGuiRect(rect.Value), color);
            }
        }

        _clicked = false;
    }
}