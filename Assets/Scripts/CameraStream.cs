using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraStream : MonoBehaviour
{
    Camera _camera;
    Task<byte[]> _streamTask;
    Texture2D _texture;
    RenderTexture _renderTexture;
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
        if (_renderTexture == null || _renderTexture.width != Screen.width || _renderTexture.height != Screen.height)
        {
            if (_renderTexture != null) Destroy(_renderTexture);
            _renderTexture = new(Screen.width, Screen.height, 16);
            _camera.targetTexture = _renderTexture;
        }
        _camera.Render();

        if (_texture == null || _texture.width != Screen.width || _texture.height != Screen.height)
        {
            if (_texture != null) Destroy(_texture);
            _texture = new(Screen.width, Screen.height, TextureFormat.RGB24, false);
        }
        RenderTexture.active = _renderTexture;
        _texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
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
