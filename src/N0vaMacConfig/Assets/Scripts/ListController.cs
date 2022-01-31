using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ListController : MonoBehaviour
{

    private string baseUrl = Encoding.UTF8.GetString(Convert.FromBase64String("aHR0cHM6Ly95b3NoaWRhbi5naXRodWIuaW8vc2F2ZWRhdGEv"));

    private string dataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    private IDictionary<string, string> pairs = new Dictionary<string, string>();

    [SerializeField] private RectTransform content;

    [SerializeField] private GameObject iconPrefab;

    void Awake()
    {
        Application.targetFrameRate = 24;
    }
    
    void Start()
    {
        StartCoroutine(PrepareList($"{baseUrl}mapping.txt"));
    }

    IEnumerator PrepareList(string mappingPath)
    {
        var request = new UnityWebRequest(mappingPath);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            var textData = request.downloadHandler.text;
            foreach (var s in textData.Split('\n'))
            {
                var keyValue = s.Split('=');
                if (keyValue.Length < 2)
                {
                    continue;
                }
                var key = keyValue[0];
                var value = keyValue[1];
                pairs.Add(key, value);
            }
        }
        else
        {
            Debug.LogError(request.error);
        }

        OnConfigPrepared();
    }

    private void OnConfigPrepared()
    {
        var client = new UdpClient();
        foreach (var pair in pairs)
        {
            var icon = Instantiate(iconPrefab);
            var iconTransform = icon.GetComponent<RectTransform>();
            iconTransform.SetParent(content);
            iconTransform.localPosition = Vector3.zero;
            iconTransform.localScale = Vector3.one;
            var button = icon.GetComponentInChildren<Button>();
            StartCoroutine(LoadTexture(icon.GetComponent<RawImage>(), pair.Key, button));

            var video = $"{dataDir}/{pair.Value}.mp4";
            var text = button.GetComponentInChildren<Text>();
            text.text = File.Exists(video) ? "Select" : "Download";
            button.onClick.AddListener(() =>
            {
                if (File.Exists(video))
                {
                    var data = Encoding.UTF8.GetBytes($"dataPath:{video}");
                    client.SendAsync(data, data.Length, "localhost", 9205);
                }
                else
                {
                    StartCoroutine(DownloadVideo($"{baseUrl}game/{pair.Value}.ndf", video,
                        button, text));
                }
            });
        }
    }

    IEnumerator DownloadVideo(string url, string path, Button button, Text text)
    {
        button.enabled = false;
        text.text = "Downloading...";
        Debug.Log($"download from {url}");
        var request = new UnityWebRequest(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            var original = request.downloadHandler.data;
            // I discovered that ndf is mp4 but first 2 bytes cause collapse. 
            var mp4 = original.Skip(2).ToArray();
            File.WriteAllBytes($"{path}_tmp", mp4);
            File.Move($"{path}_tmp", path);
            text.text = "Select";
            Debug.Log($"saved into {path}");
            button.enabled = true;
        }
        else
        {
            Debug.LogError($"download failed {request.error}");
            text.text = "Error";
        }
    }

    IEnumerator LoadTexture(RawImage image, string key, Button button)
    {
        var url = $"{baseUrl}game/{key}.ndf";
        var texturePath = $"{dataDir}/{key}.ndf";
        if (File.Exists(texturePath))
        {
            url = $"file://{texturePath}";
        }
        Debug.Log(url);
        button.enabled = false;
        var request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        button.enabled = true;
    }

}
