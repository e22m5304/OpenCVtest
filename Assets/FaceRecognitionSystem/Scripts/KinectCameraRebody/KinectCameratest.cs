using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;
using System;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;

public class KinectCameratest : MonoBehaviour
{
    // Kinect device and related variables
    Device kinect;
    Texture2D kinectColorTexture;
    [SerializeField]
    UnityEngine.UI.RawImage rawColorImg;
    Tracker tracker;

    // Various flags and variables

    //[SerializeField] bool risThrow = false;
    Vector3 preVelocity;
    Vector3 prerVelocity;
    Vector3 prerfVelocity;
    Vector3 prelfVelocity;
    Vector3 _prevrfPosition;
    Vector3 _prevlfPosition;
    Vector3 prevrfPosition;
    Vector3 prevlfPosition;
    Vector3 _nowvrfPosition;
    Vector3 _nowvlfPosition;
    Vector3 nowvrfPosition;
    Vector3 nowvlfPosition;
    Joint prerfJoints;
    Joint prelfJoints;
    Joint nowrfJoints;
    Joint nowlfJoints;
    float timer;
    int currentframe;

    // Game objects to represent body parts
  
    [SerializeField]
    GameObject right;
    

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
        int heigth = kinect.GetCalibration().ColorCameraCalibration.ResolutionHeight;
        kinectColorTexture = new Texture2D(width, heigth);
        tracker = Tracker.Create(kinect.GetCalibration(), TrackerConfiguration.Default);
    }



    // Continuously process Kinect data
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

                    this.SetMarkPos(this.right, JointId.HandRight, frame);

                    currentframe++;
                }
            }
        }
    }

    // Calculate and return the angle between three joints
   
    // Calculate and return the distance between two joints


    // Set color for the Kinect image
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

    // Calculate and return velocity difference for right foot

    // Set the position of a game object to represent a body part
    private void SetMarkPos(GameObject effectPrefab, JointId jointId, Frame frame)
    {
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        effectPrefab.transform.localPosition = new Vector3(-joint.Position.X, -joint.Position.Y, joint.Position.Z) / 50;
    }

    

    private void OnDestroy()
    {
        kinect.StopCameras();
    }
}
