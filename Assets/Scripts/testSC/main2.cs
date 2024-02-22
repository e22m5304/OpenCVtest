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
                // BodyTraking�����擾���ĉ΂̋ʃI�u�W�F�N�g��Position�Ɏw�肵�܂�
                tracker.EnqueueCapture(capture);
                var frame = tracker.PopResult();
                if (frame.NumberOfBodies > 0) // ���̔��肪�Ȃ��ƎB�e�͈͂���l�����Ȃ��Ȃ����ꍇ�Ɍ�̏����Ŏ~�܂�
                {
                    this.SetMarkPos(this.right, JointId.HandRight, frame); // �E��
                    // ���w�肷��JointId��ς���΍��⓪�ɉ΂̋ʂ��o�����Ƃ��\
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
        // �w��̊֐ߏ�񂩂�ʒu���擾����GameObject�̈ʒu�w���
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        var offset = 50; // �擾�����ʂ�̒l���Ɠ���������̂ŏ����������Ă��܂�
        var pos = new Vector3(joint.Position.X / -offset, joint.Position.Y / -offset, joint.Position.Z / offset);
        right.transform.localPosition = pos;
    }

    private void OnDestroy()
    {
        kinect.StopCameras();
    }
}