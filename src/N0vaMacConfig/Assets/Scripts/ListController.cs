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

    struct Metadata
    {
        public string dataName;
        public int chunkCount; 
    }

    private string baseUrl = Encoding.UTF8.GetString(Convert.FromBase64String("aHR0cHM6Ly95b3NoaWRhbi5naXRodWIuaW8vc2F2ZWRhdGEv"));

    private string dataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    private IDictionary<string, Metadata> pairs = new Dictionary<string, Metadata>();

    [SerializeField] private RectTransform content;

    [SerializeField] private GameObject iconPrefab;

    void Awake()
    {
        Application.targetFrameRate = 24;
    }
    
    void Start()
    {
        StartCoroutine(PrepareList($"{baseUrl}mapping_v2.txt"));
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
                var metadata = new Metadata
                {
                    dataName = keyValue[1],
                    chunkCount =  keyValue.Length > 2 ? Int32.Parse(keyValue[2]) : 0
                };
                
                pairs.Add(key, metadata);
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

            var video = $"{dataDir}/{pair.Value.dataName}.mp4";
            var chunkCount = pair.Value.chunkCount;
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
                    StartCoroutine(DownloadVideo($"{baseUrl}game/{pair.Value.dataName}.ndf", video,
                        chunkCount, button, text));
                }
            });
        }
    }

    IEnumerator DownloadVideo(string url, string path, int chunkCount, Button button, Text text)
    {
        button.enabled = false;
        Boolean hasError = false;
        if (chunkCount > 0)
        {
            var data = Array.Empty<byte>().AsEnumerable();
            for (var i = 1; i <= chunkCount; i++)
            {
                text.text = $"Downloading... {i}/{chunkCount}";
                var chunkURL = $"{url}_chunk{i}";
                Debug.Log($"download from {chunkURL}");
                var request = new UnityWebRequest(chunkURL);
                request.downloadHandler = new DownloadHandlerBuffer();
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                   data  = data.Concat(request.downloadHandler.data);
                }
                else
                {
                    Debug.LogError($"download failed {request.error}");
                    hasError = true;
                    break;
                }
            }
            if (!hasError)
            {
                //TODO async write
                File.WriteAllBytes($"{path}_tmp", data.Skip(2).ToArray());
                File.Move($"{path}_tmp", path);
                Debug.Log($"saved into {path}");
            }
        }
        else
        {
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
                Debug.Log($"saved into {path}");
            }
            else
            {
                Debug.LogError($"download failed {request.error}");
                hasError = true;
            } 
        }

        if (hasError)
        {
            text.text = "Error";
        }
        else
        {
            button.enabled = true;
            text.text = "Select";
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
