using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(LabelGenerator))]
public class LabelUI : MonoBehaviour
{
    bool _generating;
    InputAction _generateAction;
    LabelGenerator _labelGenerator;

    [SerializeField] KfsSpawner _kfsSpawner;
    [SerializeField] SpearheadSpawner _spearheadSpawner;
    [SerializeField] LightRandomizer _lightRandomizer;
    [SerializeField] DatasetGenerator _datasetGenerator;
    [SerializeField] Vector3 _rotMin = new(-20, 0, -30), _rotMax = new(20, 360, 30);
    [SerializeField] Vector2 _offsetMin = new(-0.2f, -0.2f), _offsetMax = new(0.2f, 0.2f);
    [SerializeField] float _distMin = 2, _distMax = 10;
    [SerializeField] int _datasetCount = 100;
    [SerializeField] float _spawnDuration = 3.0f / 60.0f;

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
    }

    public IEnumerator GenerateDatasets(DatasetGenerator generator)
    {
        for (int i = 0; i < _datasetCount;)
        {
            _kfsSpawner.SpawnRandomKfss();
            _spearheadSpawner.SpawnRandom();
            _lightRandomizer.Randomize();
            yield return new WaitForSeconds(_spawnDuration);

            var rot = RandomExt.RangeVec3(_rotMin, _rotMax);
            var offset = RandomExt.RangeVec2(_offsetMin, _offsetMax);
            var dist = Random.Range(_distMin, _distMax);

            var qrot = Quaternion.Euler(rot);
            var cameraOffset = qrot * (Vector3.forward + (Vector3)offset) * dist;
            generator.transform.SetPositionAndRotation(transform.position - cameraOffset, qrot);

            var task = generator.GenerateDataset(Enumerable.Concat<ClassIndex>(_kfsSpawner.ActiveKfss, _spearheadSpawner.SpawnedSpears));
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.Result) i++;
        }
        _generating = false;
    }

    void OnGUI()
    {
        var enumerator = Enumerable.Concat<ClassIndex>(_kfsSpawner.ActiveKfss, _spearheadSpawner.SpawnedSpears);
        foreach (var obj in enumerator)
        {
            var rect = _labelGenerator.GenerateLabel(obj.GetComponent<MeshFilter>());
            if (rect.HasValue)
            {
                var uiRect = LabelGenerator.ToGuiRect(rect.Value);

                GuiExt.DrawRectOutline(uiRect, 2, Color.green);
                GUI.Label(new Rect(uiRect.x, uiRect.y + uiRect.height, 100, 20), obj.name, new GUIStyle()
                {
                    fontSize = 15,
                    alignment = TextAnchor.UpperRight,
                    normal = new GUIStyleState { textColor = Color.green }
                });
            }
        }
    }
}