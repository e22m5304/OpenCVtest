using UnityEngine;
using UnityEngine.Android;
using FaceRecognitionSystem;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;
using System;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;

public class particletest : MonoBehaviour, IImageProvider
{
    public int CameraIndex;
    Device kinect;
    
    [SerializeField]
    GameObject right;
    Tracker tracker;
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

    private void Start()
    {
        InitKinect();
        Task t = KinectLoop();
#if PLATFORM_ANDROID && !UNITY_EDITOR
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
#endif
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
                _webcam = new WebCamTexture(device.name, (int)Resolution.x, (int)Resolution.y,30);
                _webcam.Play();

                Ready.Invoke(this);
                _inited = true;
            }
        }
    }

    private void InitKinect()
    {
        kinect = Device.Open(0);

        kinect.StartCameras(new DeviceConfiguration
        {
            CameraFPS = FPS.FPS30,
            ColorResolution = ColorResolution.Off,
            DepthMode = DepthMode.NFOV_Unbinned,
            WiredSyncMode = WiredSyncMode.Standalone,
            /*
            ColorFormat = ImageFormat.ColorBGRA32,
            ColorResolution = ColorResolution.R720p,
            DepthMode = DepthMode.NFOV_2x2Binned,
            SynchronizedImagesOnly = true,
            CameraFPS = FPS.FPS30*/
        });

        tracker = Tracker.Create(kinect.GetCalibration(), TrackerConfiguration.Default);
    }

    private async Task KinectLoop()
    {
        while (true)
        {
            using (Capture capture = await Task.Run(() => this.kinect.GetCapture()).ConfigureAwait(true))
            {
                tracker.EnqueueCapture(capture);
                var frame = tracker.PopResult();
                if (frame.NumberOfBodies > 0)
                {
                    // Extract joint information from the tracked skeleton
                    var skeleton = frame.GetBodySkeleton(0);
                    var HRjoint = skeleton.GetJoint(JointId.HandRight);

                    this.SetMarkPos(this.right, JointId.HandRight, frame);
                }
            }
        }
    }

    private void SetMarkPos(GameObject effectPrefab, JointId jointId, Frame frame)
    {
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        effectPrefab.transform.localPosition = new Vector3(joint.Position.X, -joint.Position.Y, joint.Position.Z) / 50;
    }

    private void OnDestroy()
    {
        kinect.StopCameras();
        _webcam?.Stop();
    }

    private WebCamTexture _webcam = null;
    private bool _inited = false;
}
