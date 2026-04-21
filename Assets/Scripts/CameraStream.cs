using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraStream : MonoBehaviour
{
    Camera _camera;
    Task<byte[]> _streamTask;
    Texture2D _texture;
    byte[] _data;

    [SerializeField] string _url = "http://127.0.0.1:3445";

    public IReadOnlyList<byte> ResponseData => _data;
    public event Action Responded; 

    void Start()
    {
        _camera = GetComponent<Camera>();
    }
    void Update()
    {
        if (_streamTask != null)
        {
            if (!_streamTask.IsCompleted) return;
            _data = _streamTask.Result;
            Responded?.Invoke();
            _streamTask.Dispose();
            _streamTask = null;
        }
        var rt = _camera.targetTexture;
        _camera.Render();

        if (_texture == null || _texture.width != rt.width || _texture.height != rt.height)
        {
            if (_texture != null) Destroy(_texture);
            _texture = new(rt.width, rt.height, TextureFormat.RGB24, false);
        }
        RenderTexture.active = rt;
        _texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, false);
        var bytes = _texture.EncodeToPNG();

        _streamTask = StreamBytesAsync(bytes);
    }
    async Task<byte[]> StreamBytesAsync(byte[] bytes)
    {
        using var client = new HttpClient();
        var response = await client.PostAsync(_url, new ByteArrayContent(bytes));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }
}
