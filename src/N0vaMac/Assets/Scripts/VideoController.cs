using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer), typeof(RawImage))]
public class VideoController : MonoBehaviour
{

    private RawImage image;
    private RectTransform imageSize;
    private VideoPlayer player;
    private float aspectRatio = 1.0f;
    private VideoAspectRatio videoAspectRatio;

  //  [SerializeField] private Text debugInfo;

    [SerializeField] private VideoClip defaultClip;
    
    void Awake()
    {
        Application.targetFrameRate = 30;
        image = GetComponent<RawImage>();
        imageSize = GetComponent<RectTransform>();
        player = GetComponent<VideoPlayer>();
        player.prepareCompleted += (source) =>
        {
            var texture = source.texture;
            aspectRatio = (float) texture.height / texture.width;
        };
        videoAspectRatio = player.aspectRatio;
    }

    private void Start()
    {

        var settingFile = $"{Application.persistentDataPath}/path.txt";
        if (File.Exists(settingFile))
        {
            var path = File.ReadAllText(settingFile);
            if (File.Exists(path))
            {
                ChangeUrl(path);
            }
            else
            {
                Debug.Log($"path doesnt exists {path}");
                ChangeClip(defaultClip);
            }
        }
        else
        {
            ChangeClip(defaultClip);
        }
#if !UNITY_EDITOR
        var resolution = Screen.currentResolution;
        Screen.SetResolution(resolution.width, resolution.height, false);
        WindowApi.Background();
        WindowApi.HideTitleBar();
#endif
        StartCoroutine(AdjustLoop());
    }

    public void ChangeUrl(string url)
    {
        player.Stop();
        player.source = VideoSource.Url;
        player.url = $"file://{url}";
        player.Play();
    }

    public void ChangeClip(VideoClip clip)
    {
        player.Stop();
        player.source = VideoSource.VideoClip;
        player.clip = clip;
        player.Play();
    }

    private IEnumerator AdjustLoop()
    {
        while (true)
        {
            yield return Adjust();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator Adjust()
    {
        var resolution = Screen.currentResolution;
        Screen.SetResolution(resolution.width, resolution.height, false); 
        var currentMonitor = Screen.mainWindowDisplayInfo;
        //always move to origin in current monitor
        yield return Screen.MoveMainWindowTo(currentMonitor, currentMonitor.workArea.min);
    }

    void Update()
    {
        if (player.isPrepared)
        {
            image.texture = player.texture;
            
            switch(videoAspectRatio)
            {
                case VideoAspectRatio.FitHorizontally:
                {
                    imageSize.sizeDelta = new Vector2(Screen.width, Screen.width * aspectRatio);
                    break;
                }
                default:
                {
                    imageSize.sizeDelta = new Vector2(Screen.height / aspectRatio, Screen.height);
                    break;
                }
            }
        }
    }
}
