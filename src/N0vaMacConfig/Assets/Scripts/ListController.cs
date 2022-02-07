using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
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

    [SerializeField] private RectTransform content;

    [SerializeField] private GameObject iconPrefab;

    void Awake()
    {
        Application.targetFrameRate = 24;
    }
    
    async void Start()
    {
        var pairs = await DownloadList($"{baseUrl}mapping_v2.txt");
        await PopulateTexture(pairs);
    }

    async UniTask<IDictionary<string, Metadata>> DownloadList(string mappingPath)
    {
        var pairs = new Dictionary<string, Metadata>();
        var mappingInfo = (await GetAsync(mappingPath)).text;
        foreach (var s in mappingInfo.Split('\n'))
        {
            var keyValue = s.Split('=');
            if (keyValue.Length < 2)
            {
                continue;
            }
            
            // big data has been split.
            var key = keyValue[0];
            var metadata = new Metadata
            {
                dataName = keyValue[1],
                chunkCount =  keyValue.Length > 2 ? Int32.Parse(keyValue[2]) : 0
            };
            
            pairs.Add(key, metadata);
        }
        return pairs;
    }

    private async UniTask PopulateTexture(IDictionary<string, Metadata> metadatas)
    {
        var client = new UdpClient();
        foreach (var metadata in metadatas)
        {
            var icon = Instantiate(iconPrefab);
            var iconTransform = icon.GetComponent<RectTransform>();
            iconTransform.SetParent(content);
            iconTransform.localPosition = Vector3.zero;
            iconTransform.localScale = Vector3.one;
            var button = icon.GetComponentInChildren<Button>();
            var video = $"{dataDir}/{metadata.Value.dataName}.mp4";
       
            var text = button.GetComponentInChildren<Text>();
            text.text = File.Exists(video) ? "Select" : "Download";
            try
            {
                icon.GetComponent<RawImage>().texture = await LoadTexture(metadata.Key);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                text.text = "Texture Error";
            }

            AddOnClickHandler(button,text, client, video,metadata); 
        }
    }

    private async void AddOnClickHandler(Button button, Text text, UdpClient client, string video, KeyValuePair<string,Metadata> metadata)
    {
        var chunkCount = metadata.Value.chunkCount;
        while (!this.GetCancellationTokenOnDestroy().IsCancellationRequested)
        {
            try
            {
                await button.OnClickAsync(this.GetCancellationTokenOnDestroy());
                if (File.Exists(video))
                {
                    var data = Encoding.UTF8.GetBytes($"dataPath:{video}");
                    await client.SendAsync(data, data.Length, "localhost", 9205);
                }
                else
                {
                    button.enabled = false;
                    try
                    {
                        await DownloadVideo($"{baseUrl}game/{metadata.Value.dataName}.ndf", video,
                            chunkCount, text);
                        text.text = "Select";
                    }
                    catch (UnityWebRequestException e)
                    {
                        Debug.LogException(e);
                        text.text = "Error";
                    }

                    button.enabled = true;
                }
            }
            catch (OperationCanceledException)
            {
                //ignore console error
            }
        }
    }

    private async UniTask DownloadVideo(string url, string path, int chunkCount, Text text)
    {
        if (chunkCount > 0)
        {
            using (var writer = File.OpenWrite($"{path}_tmp"))
            {
                for (var i = 1; i <= chunkCount; i++)
                {
                    text.text = $"Downloading... {i}/{chunkCount}";
                    var chunkURL = $"{url}_chunk{i}";
                    Debug.Log($"download from {chunkURL}");
                    var data = (await GetAsync(chunkURL)).data;
                    var offset = i == 1 ? 2 :0 ;
                    await writer.WriteAsync(data, offset, data.Length - offset);
                }
            }
            File.Move($"{path}_tmp", path);
            Debug.Log($"saved into {path}");
        }
        else
        {
            text.text = "Downloading...";
            Debug.Log($"download from {url}");
            var data = (await GetAsync(url)).data;
            using (var writer = File.OpenWrite($"{path}_tmp"))
            {
                // I discovered that ndf is mp4 but first 2 bytes cause collapse. 
                var offset = 2;
                await writer.WriteAsync(data, offset, data.Length - offset) ;
            }

            File.Move($"{path}_tmp", path);
            Debug.Log($"saved into {path}");
        }
    }

    private async UniTask<Texture> LoadTexture(string key)
    {
        var url = $"{baseUrl}game/{key}.ndf";
        var texturePath = $"{dataDir}/{key}.ndf";
        if (File.Exists(texturePath))
        {
            url = $"file://{texturePath}";
        }
        Debug.Log(url);
        return (await DoGetAsync(UnityWebRequestTexture.GetTexture(url)) as DownloadHandlerTexture).texture;
    }

    private async UniTask<DownloadHandler> GetAsync(string url)
    {
        return await DoGetAsync(UnityWebRequest.Get(url));
    }
    
    private async UniTask<DownloadHandler> DoGetAsync(UnityWebRequest request)
    {
        return (await request.SendWebRequest()
            .WithCancellation(this.GetCancellationTokenOnDestroy())).downloadHandler;
    }

}
