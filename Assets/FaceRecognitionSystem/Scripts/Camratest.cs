using UnityEngine;
using FaceRecognitionSystem;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

public class Cameratest : MonoBehaviour, IImageProvider
{
    Device kinect;
    Texture2D _kinectColorTexture;
    Tracker tracker;
    [SerializeField]
    RawImage rawColorImg;
    private bool _inited = false;
    public ImageProviderReadyEvent Ready = new ImageProviderReadyEvent();

    // ADD
    private Color32[] imgData;
    //

    public Color32[] ImgData
    {
        get
        {
            //return new Color32[ImgData.Length];
            //return _kinectColorTexture.GetPixels32();
            return imgData;
        }
        set
        {
            imgData = value;
        }
    }

    public int Width
    {
        get
        {
            return _kinectColorTexture.width;
        }
        set
        {
            // セットする処理があれば追加
        }
    }

    public int Height
    {
        get
        {
            return _kinectColorTexture.height;
        }
        set
        {
            // セットする処理があれば追加
        }
    }

    private void Start()
    {
        InitKinect();

        // Invoke the Ready event to notify that the Kinect is ready
    }

    private void Update()
    {
        // Process Kinect data and update color texture
        SetColor();
    }

    // Initialize the Kinect device and related settings
    private void InitKinect()
    {
        kinect = Device.Open(0);

        kinect.StartCameras(new DeviceConfiguration
        {
            ColorFormat = ImageFormat.ColorBGRA32,
            ColorResolution = ColorResolution.R720p,
            DepthMode = DepthMode.NFOV_Unbinned,
            SynchronizedImagesOnly = true,
            CameraFPS = FPS.FPS30
        });

        int width = kinect.GetCalibration().ColorCameraCalibration.ResolutionWidth;
        int height = kinect.GetCalibration().ColorCameraCalibration.ResolutionHeight;
        _kinectColorTexture = new Texture2D(width, height);
        tracker = Tracker.Create(kinect.GetCalibration(), TrackerConfiguration.Default);
    }

    // Process Kinect data and update color texture
    private void SetColor()
    {
        Capture capture = kinect.GetCapture();
        Microsoft.Azure.Kinect.Sensor.Image colorImg = capture.Color;
        // Color32[] pixels = colorImg.GetPixels<Color32>().ToArray();
        Color32[] pixels = colorImg.GetPixels<Color32>().ToArray();
        //Color32[] pixels = new Color32[1280*720];
        for (int i = 0; i < pixels.Length; i++)
        {
            var d = pixels[i].b;
            var k = pixels[i].r;
            pixels[i].r = d;
            pixels[i].b = k;
        }

        // ADD
        ImgData = pixels;
        //

        _kinectColorTexture.SetPixels32(pixels);
        _kinectColorTexture.Apply();
        rawColorImg.texture = _kinectColorTexture;
        if (!_inited)
        {
            Ready.Invoke(this);
            _inited = true;
        }
           
     }

        private void OnDestroy()
    {
        kinect.StopCameras();
    }
    
}
