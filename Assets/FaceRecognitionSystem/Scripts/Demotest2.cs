using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaceRecognitionSystem;
using UnityEngine.UI;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;
using System;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;
using UnityEngine.Events;
using Image = Microsoft.Azure.Kinect.Sensor.Image;

public class Demotest2 : MonoBehaviour
{
    
    public ImageProviderReadyEvent Ready = new ImageProviderReadyEvent();
    
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
    GameObject leftAnkle;
    [SerializeField]
    GameObject rightAnkle;
    [SerializeField]
    GameObject right;
    [SerializeField]
    GameObject cube;

    private void Start()
    {
#if PLATFORM_ANDROID && !UNITY_EDITOR
        if ( !Permission.HasUserAuthorizedPermission( Permission.Camera ) ) {
            Permission.RequestUserPermission( Permission.Camera );
        }
#endif
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

                    // Calculate angles and other data using joint positions
                    _nowvrfPosition = GetrfVector3(JointId.AnkleRight, frame);
                    nowvrfPosition = new Vector3(_nowvrfPosition.x, _nowvrfPosition.y, _nowvrfPosition.z);
                    float R_Fot = this.get_angle(R_Hip, R_Knee, R_Ankle);

                    // Set positions of game objects to represent body parts
                    this.SetrMarkPos(this.right, JointId.HandRight, frame);
                    this.SetMarkPos(this.cube, JointId.HandLeft, frame);

                    prerfVelocity = GetrfVelocity();
                    _prevrfPosition = GetrfVector3(JointId.KneeRight, frame);
                    prevrfPosition = new Vector3(_prevrfPosition.x, _prevrfPosition.y, _prevrfPosition.z);
                    prerfJoints = skeleton.GetJoint(JointId.KneeRight);
                    prelfJoints = skeleton.GetJoint(JointId.AnkleLeft);
                    prerfVelocity = GetrfVelocity();

                    currentframe++;
                }
            }
        }
    }

    // Calculate and return the angle between three joints
    private float get_angle(Joint A, Joint B, Joint C)
    {
        Vector3 BA = transform.position;
        BA.x = B.Position.X - A.Position.X;
        BA.y = B.Position.Y - A.Position.Y;
        BA.z = B.Position.Z - A.Position.Z;

        Vector3 BC = transform.position;
        BC.x = C.Position.X - B.Position.X;
        BC.y = C.Position.Y - B.Position.Y;
        BC.z = C.Position.Z - B.Position.Z;

        double norm_BA = Math.Sqrt(BA.x * BA.x + BA.y * BA.y + BA.z * BA.z);
        double norm_BC = Math.Sqrt(BC.x * BC.x + BC.y * BC.y + BC.z * BC.z);

        float inner_product = BA.x * BC.x + BA.y * BC.y + BA.z * BC.z;

        double radian = Math.Acos(inner_product / (norm_BA * norm_BC));
        double degree = radian * 180.0f / Math.PI;

        return (float)degree;
    }

    // Calculate and return the distance between two joints
    private float get_dis(Joint A, Joint B)
    {
        Vector3 BA = transform.position;
        BA.x = A.Position.X - B.Position.X;
        BA.y = A.Position.Y - B.Position.Y;
        BA.z = A.Position.Z - B.Position.Z;

        double norm_BA = Math.Sqrt(BA.x * BA.x + BA.y * BA.y + BA.z * BA.z);
        return (float)norm_BA;
    }

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
    Vector3 GetrfAccerareta()
    {
        return ((GetrfVelocity() - prerfVelocity) / 30);
    }

    // Calculate and return velocity for right foot
    Vector3 GetrfVelocity()
    {
        var position = nowrfJoints.Position - prerfJoints.Position;
        var velocity = GetrfVector3(position) / 30;
        return velocity;
    }

    // Convert System.Numerics.Vector3 to UnityEngine.Vector3
    Vector3 GetrfVector3(System.Numerics.Vector3 vector3)
    {
        return new Vector3(vector3.X, vector3.Y, vector3.Z);
    }

    // Get the position of a joint in a frame
    Vector3 GetrfVector3(JointId jointId, Frame frame)
    {
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }

    // Set the position of a game object to represent a body part
    private void SetMarkPos(GameObject effectPrefab, JointId jointId, Frame frame)
    {
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        effectPrefab.transform.localPosition = new Vector3(-joint.Position.X, -joint.Position.Y, joint.Position.Z) / 50;
    }

    // Set the position of the 'right' game object to represent a body part
    private void SetrMarkPos(GameObject right, JointId jointId, Frame frame)
    {
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        right.transform.localPosition = new Vector3(-joint.Position.X, -joint.Position.Y, joint.Position.Z) / 50;
    }

    private void OnDestroy()
    {
        kinect.StopCameras();
    }
    private WebCamTexture _webcam = null;
    private bool _inited = false;
}
