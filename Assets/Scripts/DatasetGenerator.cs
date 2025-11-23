using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading.Tasks;

[RequireComponent(typeof(LabelGenerator), typeof(Camera))]
public class DatasetGenerator : MonoBehaviour
{
    [SerializeField] string _imageDirectory = "Assets/.datasets/Images";
    [SerializeField] string _labelDirectory = "Assets/.datasets/Labels";

    LabelGenerator _labelGenerator;
    Camera _camera;

    public async Task<bool> GenerateDataset(string fileName, Kfs kfs)
    {
        var label = _labelGenerator.CreateLabel(kfs.transform);
        if (!label.HasValue) return false;

        var labelIndex = kfs.GetIndex();
        var center = label.Value.center;
        var size = label.Value.size;

        _camera.Render();
        var rt = _camera.activeTexture;
        RenderTexture.active = rt;

        Texture2D tex = new(rt.width, rt.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, false);

        var bytes = tex.EncodeToPNG();
        await Task.WhenAll(
            File.WriteAllTextAsync($"{_labelDirectory}/{fileName}.txt", $"{labelIndex} {center.x} {center.y} {size.x} {size.y}\n"),
            File.WriteAllBytesAsync($"{_imageDirectory}/{fileName}.png", bytes)
        );
        return true;
    }

    void Start()
    {
        _labelGenerator = GetComponent<LabelGenerator>();
        _camera = GetComponent<Camera>();

        Directory.CreateDirectory(_imageDirectory);
        Directory.CreateDirectory(_labelDirectory);
    }
}