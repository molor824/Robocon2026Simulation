using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(LabelGenerator))]
public class LabelUI : MonoBehaviour
{
    bool _generating;
    InputAction _generateAction;
    LabelGenerator _labelGenerator;
    List<Kfs> _kfss = new();

    [SerializeField] DatasetGenerator _datasetGenerator;
    [SerializeField] Transform _activeKfss;
    [SerializeField] Vector3 _rotMin = new(-20, 0, -30), _rotMax = new(20, 360, 30);
    [SerializeField] Vector2 _offsetMin = new(-0.2f, -0.2f), _offsetMax = new(0.2f, 0.2f);
    [SerializeField] float _distMin = 2, _distMax = 10;
    [SerializeField] int _datasetCount = 100;

    void Start()
    {
        _labelGenerator = GetComponent<LabelGenerator>();
        _generateAction = InputSystem.actions.FindAction("DatasetGen");
        _generateAction.performed += _ => 
        {
            if (!_generating)
            {
                _generating = true;
                StartCoroutine(GenerateDatasets(_datasetGenerator));
            }
        };
        
        for (var i = 0; i < _activeKfss.childCount; i++)
        {
            var child = _activeKfss.GetChild(i);
            if (child.TryGetComponent(out Kfs kfs))
                _kfss.Add(kfs);
        }
    }

    void ShuffleKfsPositions()
    {
        for (int i = 0; i < _kfss.Count; i++)
        {
            var j = Random.Range(0, _kfss.Count);
            var kfs1 = _kfss[i].transform;
            var kfs2 = _kfss[j].transform;
            // Swap positions
            var temp = kfs1.position;
            kfs1.position = kfs2.position;
            kfs2.position = temp;
            // Random Y rotation
            kfs1.Rotate(Vector3.up, Random.Range(0.0f, 360.0f), Space.World);
            kfs2.Rotate(Vector3.up, Random.Range(0.0f, 360.0f), Space.World);
        }
    }

    public IEnumerator GenerateDatasets(DatasetGenerator generator)
    {
        for (int i = 0; i < _datasetCount;)
        {
            ShuffleKfsPositions();

            var rot = RandomExt.RangeVec3(_rotMin, _rotMax);
            var offset = RandomExt.RangeVec2(_offsetMin, _offsetMax);
            var dist = Random.Range(_distMin, _distMax);

            var qrot = Quaternion.Euler(rot);
            var cameraOffset = qrot * (Vector3.forward + (Vector3)offset) * dist;
            generator.transform.SetPositionAndRotation(transform.position - cameraOffset, qrot);

            var task = generator.GenerateDataset(i, _kfss);
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
                color.a = 0.3f;

                EditorGUI.DrawRect(ToGuiRect(rect.Value), color);
            }
        }
    }
}