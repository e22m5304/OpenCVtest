using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;

public class main2 : MonoBehaviour
{
    // Handler for SkeletalTracking thread.
    public GameObject m_tracker;
    private SkeletalTrackingProvider m_skeletalTrackingProvider;
    public BackgroundData m_lastFrameData = new BackgroundData();

    Device kinect;
    Texture2D kinectColorTexture;
    [SerializeField]
    UnityEngine.UI.RawImage rawColorImg;
    Tracker tracker;
    TrackerHandler trackerHandler;

    [SerializeField]
    GameObject leftHand;
    [SerializeField]
    GameObject right;

    void Start()
    {

        InitKinect();
        //tracker ids needed for when there are two trackers
        const int TRACKER_ID = 0;
        m_skeletalTrackingProvider = new SkeletalTrackingProvider(TRACKER_ID);
        Task t = KinectLoop();
    }
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
        int heigth = kinect.GetCalibration().ColorCameraCalibration.ResolutionHeight;
        kinectColorTexture = new Texture2D(width, heigth);
        tracker = Tracker.Create(kinect.GetCalibration(), TrackerConfiguration.Default);
    }
    private async Task KinectLoop()
    {
        while (true)
        {
            using (Capture capture = await Task.Run(() => this.kinect.GetCapture()).ConfigureAwait(true))
            {
                // BodyTraking情報を取得して火の玉オブジェクトのPositionに指定します
                tracker.EnqueueCapture(capture);
                var frame = tracker.PopResult();
                if (frame.NumberOfBodies > 0) // この判定がないと撮影範囲から人がいなくなった場合に後の処理で止まる
                {
                    this.SetMarkPos(this.right, JointId.HandRight, frame); // 右手
                    // ↑指定するJointIdを変えれば腰や頭に火の玉を出すことも可能
                }

            }
        }
    }

    void Update()
    {
        SetColor();
        if (m_skeletalTrackingProvider.IsRunning)
        {
            if (m_skeletalTrackingProvider.GetCurrentFrameData(ref m_lastFrameData))
            {
                if (m_lastFrameData.NumOfBodies != 0)
                {
                    m_tracker.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData);
                }
            }
        }
        Debug.Log(m_lastFrameData.NumOfBodies);

    }


    private void SetColor()
    {
        Capture capture = kinect.GetCapture();

        Image colorImg = capture.Color;

        Color32[] pixels = colorImg.GetPixels<Color32>().ToArray();
        for (int i = 0; i < pixels.Length; i++)
        {
            var d = pixels[i].b;
            var k = pixels[i].r;
            pixels[i].r = d;
            pixels[i].b = k;
        }

        kinectColorTexture.SetPixels32(pixels);
        kinectColorTexture.Apply();

        rawColorImg.texture = kinectColorTexture;
    }
    private void SetMarkPos(GameObject rightl, JointId jointId, Frame frame)
    {
        // 指定の関節情報から位置を取得してGameObjectの位置指定に
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        var offset = 50; // 取得した通りの値だと動きすぎるので少し調整してやります
        var pos = new Vector3(joint.Position.X / -offset, joint.Position.Y / -offset, joint.Position.Z / offset);
        right.transform.localPosition = pos;
    }

    private void OnDestroy()
    {
        kinect.StopCameras();
    }
}