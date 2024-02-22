using UnityEngine;
using UnityEngine.Android;
using FaceRecognitionSystem;
using UnityEngine.UI;
using System.Threading.Tasks;

public class WebcamController : MonoBehaviour, IImageProvider
{
    public int CameraIndex;
    public Vector2 Resolution = new Vector2(640, 480);

    public ImageProviderReadyEvent Ready = new ImageProviderReadyEvent();

    public Color32[] ImgData
    {
        get
        {
            return _webcam?.GetPixels32();
        }
        set
        {

        }
    }

    public int Width
    {
        get
        {
            return _webcam.width;
        }
        set
        {
        }
    }

    public int Height
    {
        get
        {
            return _webcam.height;
        }
        set
        {
        }
    }

    private WebCamTexture _webcam = null;
    private bool _inited = false;

    private void Start()
    {
        //Task t = InitializeWebcam();
    }

    private void Update()
    {
#if PLATFORM_ANDROID && !UNITY_EDITOR
        if (Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            //tryInit();
        }
#else
        tryInit();
#endif
    }

    private void tryInit()
    {
        if (!_inited)
        {
            var availableCameras = WebCamTexture.devices;
            if ((CameraIndex >= 0) && (CameraIndex < availableCameras.Length))
            {
                var device = availableCameras[CameraIndex];
                _webcam = new WebCamTexture(device.name, (int)Resolution.x, (int)Resolution.y, 30);
                _webcam.Play();

                Ready.Invoke(this);
                _inited = true;
            }
        }
    }

    private Task InitializeWebcam()
    {
#if PLATFORM_ANDROID && !UNITY_EDITOR
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
#endif
        tryInit();
        return Task.CompletedTask;
    }

    private void OnDestroy()
    {
        _webcam?.Stop();
    }
}
