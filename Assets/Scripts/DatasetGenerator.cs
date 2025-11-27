using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

[RequireComponent(typeof(LabelGenerator), typeof(Camera))]
public class DatasetGenerator : MonoBehaviour
{
    [SerializeField] string _imageDirectory = "Assets/.datasets/Images";
    [SerializeField] string _labelDirectory = "Assets/.datasets/Labels";

    LabelGenerator _labelGenerator;
    Camera _camera;

    static Rect InvertY(Rect rect)
    {
        return new Rect(rect.x, 1 - rect.y - rect.height, rect.width, rect.height);
    }

    public async Task<bool> GenerateDataset(int index, IEnumerable<Kfs> kfss)
    {
        var labelContent = new StringBuilder();
        var filename = new StringBuilder();
        foreach (var kfs in kfss)
        {
            var label = _labelGenerator.GenerateLabel(kfs.transform);
            if (!label.HasValue) continue;

            var rect = InvertY(label.Value);
            var center = rect.center;
            var size = rect.size;
            var labelIndex = kfs.GetIndex();
            labelContent.AppendLine($"{labelIndex} {center.x} {center.y} {size.x} {size.y}");
            filename.Append(filename.Length == 0 ? $"{labelIndex}" : $"-{labelIndex}");
        }

        if (labelContent.Length == 0)
            return false;

        _camera.Render();
        var rt = _camera.activeTexture;
        RenderTexture.active = rt;

        Texture2D tex = new(rt.width, rt.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, false);

        var bytes = tex.EncodeToPNG();
        Destroy(tex);
        await Task.WhenAll(
            File.WriteAllTextAsync($"{_labelDirectory}/{filename}_{index}.txt", labelContent.ToString()),
            File.WriteAllBytesAsync($"{_imageDirectory}/{filename}_{index}.png", bytes)
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