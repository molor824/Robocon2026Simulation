using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public struct Label
{
    public int Index;
    public float Confidence;
    public Rect Box;

    public override string ToString()
    {
        return $"Label(Index: {Index}, Confidence: {Confidence}, Box: {Box})";
    }
}

[RequireComponent(typeof(CameraStream))]
public class LabelDecoder : MonoBehaviour
{
    CameraStream _cameraStream;
    List<Label> _labels = new();
    [SerializeField] ListKfsIndices _kfsIndices;

    public IReadOnlyList<Label> Labels => _labels;

    const int LabelSize = sizeof(byte) + sizeof(float) * 5;

    static float BytesToFloat(IReadOnlyList<byte> bytes, int start) => BitConverter.Int32BitsToSingle(
        BinaryPrimitives.ReadInt32BigEndian(bytes.Skip(start).Take(4).ToArray())
    );
    void Start()
    {
        _cameraStream = GetComponent<CameraStream>();
        _cameraStream.Responded += () =>
        {
            _labels.Clear();
            var data = _cameraStream.ResponseData;
            for (int i = 0; i < data.Count; i += LabelSize)
            {
                _labels.Add(new Label
                {
                    Index = data[i],
                    Confidence = BytesToFloat(data, i + 1),
                    Box = new Rect(
                        BytesToFloat(data, i + 5),
                        BytesToFloat(data, i + 5 + 4),
                        BytesToFloat(data, i + 5 + 8),
                        BytesToFloat(data, i + 5 + 12)
                    )
                });
            }
        };
    }
    static Rect ToGuiRect(Rect rect)
    {
        var screenSize = new Vector2(Screen.width, Screen.height);
        return new Rect((rect.position - rect.size / 2) * screenSize, rect.size * screenSize);
    }
    
    void OnGUI()
    {
        foreach (var label in _labels)
        {
            var kfs = _kfsIndices.GetKfs(label.Index);
            var box = ToGuiRect(label.Box);

            GuiExt.DrawRectOutline(box, 2, Color.blue);
            EditorGUI.TextField(new Rect(box.x, box.y - 20, 100, 20), $"{kfs.name}: {label.Confidence * 100:0.}%", new GUIStyle()
            {
                fontSize = 15,
                alignment = TextAnchor.LowerLeft,
                normal = new GUIStyleState { textColor = Color.blue }
            });
        }
    }
}