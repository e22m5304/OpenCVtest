using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;


public class KinectPos : MonoBehaviour
{
    Device kinect;
    Texture2D kinectColorTexture;
    [SerializeField]
    UnityEngine.UI.RawImage rawColorImg;
    GameObject m_tracker;
    Tracker tracker;
    TrackerHandler trackerHandler;
    public BackgroundData m_lastFrameData = new BackgroundData();
    [SerializeField]
    GameObject leftHand;
    [SerializeField]
    GameObject rightHand;
    
    float timer;
    int currentframe;


    private void Start()
    {
        InitKinect();
        currentframe = 0;
        
        Task t = KinectLoop();
        
    }
    private void Update()
    {
        SetColor();
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
                tracker.EnqueueCapture(capture);
                var frame = tracker.PopResult();

            

                if (frame.NumberOfBodies > 0)
                {

                    var skeleton = frame.GetBodySkeleton(0);
                    var Nosejoint = skeleton.GetJoint(JointId.Nose);
                    var WRjoint = skeleton.GetJoint(JointId.WristRight);
                    var ERjoint = skeleton.GetJoint(JointId.ElbowRight);
                    var SRjoint = skeleton.GetJoint(JointId.ShoulderRight);

                    var HTRjoint = skeleton.GetJoint(JointId.HandTipRight);
                    var HTLjoint = skeleton.GetJoint(JointId.HandTipLeft);
                    var HRjoint = skeleton.GetJoint(JointId.HandRight);
                    var TRjoint = skeleton.GetJoint(JointId.ThumbRight);

                    var R_Hip = skeleton.GetJoint(JointId.HipRight);
                    var R_Knee = skeleton.GetJoint(JointId.KneeRight);
                    var R_Ankle = skeleton.GetJoint(JointId.AnkleRight);

                    m_tracker.GetComponent<TrackerHandler>().updateTracker(m_lastFrameData);

                    //for (int i = 0; i < 32; i++)
                    //{
                    //    nowJoints[i] = skeleton.GetJoint(i);
                    //}



                    this.SetMarkPos(this.leftHand, JointId.HandLeft, frame);

                    currentframe++;
                }

            }
        }
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
   


    private void SetMarkPos(GameObject effectPrefab, JointId jointId, Frame frame)
    {


        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        //var offset = 50;
        //var pos = new Vector3(joint.Position.X / -offset, -(joint.Position.Y/offset) ,joint.Position.Z /offset);
        //effectPrefab.transform.position = new Vector3 (nowJoints[(int)jointId].Position.X, nowJoints[(int)jointId].Position.Y, nowJoints[(int)jointId].Position.Z) - Vector3.one * 50;
        effectPrefab.transform.localPosition = new Vector3(-joint.Position.X, -joint.Position.Y, joint.Position.Z) / 50;

    }
  
    private void OnDestroy()
    {
        kinect.StopCameras();
    }

}
