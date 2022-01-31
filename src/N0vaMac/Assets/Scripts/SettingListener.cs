using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SettingListener : MonoBehaviour
{
    private const string messagePrefix = "dataPath:";

    private UdpClient udpClient;

    private Task task;

    private VideoController videoController;

    private SynchronizationContext _mainContext;
    
    private void Awake()
    {
        videoController = GetComponent<VideoController>();
        _mainContext = SynchronizationContext.Current;
    }

    private void Start()
    {
        udpClient = new UdpClient(9205);
        task = Task.Run(async () =>
        {
            while (true)
            {
                var result = await udpClient.ReceiveAsync();
                var path = Encoding.UTF8.GetString(result.Buffer);
                Debug.Log($"received message = {path}");
                if (!path.StartsWith(messagePrefix))
                {
                    return;
                }

                path = path.Substring(messagePrefix.Length);
                if (File.Exists(path))
                {
                    _mainContext.Post(_ =>
                    {
                        File.WriteAllBytes($"{Application.persistentDataPath}/path.txt", Encoding.UTF8.GetBytes(path));
                        Debug.Log($"set video = {path}");
                        videoController.ChangeUrl(path);
                    }, null);                    
                }

            }
        });
    }

    private void OnDestroy()
    { 
        udpClient.Close();
        if (task.Status == TaskStatus.RanToCompletion)
        {
            task.Dispose();
        }
    }
}
