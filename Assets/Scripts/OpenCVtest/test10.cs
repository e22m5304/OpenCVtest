using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;
using System;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;

public class test10 : MonoBehaviour
{
    private Vector3[] preVelocity;
    private Vector3[] prerVelocity;
    // Kinect device and related variables
    Device kinect;
    Texture2D kinectColorTexture;

    private Dictionary<uint, int> bodyIndices = new Dictionary<uint, int>();

    Tracker tracker;
    string koji = FaceRecognizer.koji2;
    int a = 0;
    int b = 0;
    int c = 0;
    int d = 0;
    // Various flags and variable
    // Game objects to represent body parts
    [SerializeField]
    GameObject[] right;
    [SerializeField]
    GameObject[] rightEffects;
    [SerializeField]
    GameObject[] cube;
    [SerializeField]
    GameObject[] left;
    [SerializeField]
    GameObject[] leftEffects;
    
    [SerializeField] private bool risThrow = false;
    [SerializeField] private bool risThrow2 = false;
    [SerializeField] private bool isThrow = false;
    [SerializeField] private bool isThrow2 = false;
    private Vector3[] _prevPosition;
    private Vector3[] _prevrPosition;

    private Vector3[] prevPosition;
    private Vector3[] prevrPosition;

    private Vector3[] _nowvPosition;
    private Vector3[] _nowvrPosition;

    private Vector3[] nowvPosition;
    private Vector3[] nowvrPosition;

    private Joint[] preJoints;
    private Joint[] prerJoints;

    private Joint[] nowJoints;
    private Joint[] nowrJoints;

    float timer;
    int currentframe;
    private int currentBodyIndex = -1;

    private void Start()
    {
        currentframe = 0;
        int MaxBodies = 2;
        _nowvPosition = new Vector3[MaxBodies];
        _nowvrPosition = new Vector3[MaxBodies];
        nowvPosition = new Vector3[MaxBodies];
        nowvrPosition = new Vector3[MaxBodies];

        _prevPosition = new Vector3[MaxBodies];
        _prevrPosition = new Vector3[MaxBodies];
        prevPosition = new Vector3[MaxBodies];
        prevrPosition = new Vector3[MaxBodies];

        preJoints = new Joint[MaxBodies];
        prerJoints = new Joint[MaxBodies];
        nowJoints = new Joint[MaxBodies];
        nowrJoints = new Joint[MaxBodies];
        preVelocity = new Vector3[MaxBodies];
        prerVelocity = new Vector3[MaxBodies];
        InitKinect();
        Task t = KinectLoop();
    }

    private void Update()
    {
        //SetColor();
    }

    // Initialize the Kinect device and related settings
    private void InitKinect()
    {
        kinect = Device.Open(0);

        kinect.StartCameras(new DeviceConfiguration
        {
            CameraFPS = FPS.FPS30,
            ColorResolution = ColorResolution.Off,
            DepthMode = DepthMode.NFOV_Unbinned,
            WiredSyncMode = WiredSyncMode.Standalone,
        });
        tracker = Tracker.Create(kinect.GetCalibration(), TrackerConfiguration.Default);
    }
    private void ProcessBody(Frame frame, int bodyIndex)
    {
        var body = frame.GetBody((uint)bodyIndex);
        var skeleton = frame.GetBodySkeleton((uint)bodyIndex);

        if (!bodyIndices.ContainsKey(body.Id))
        {
            // 新しいボディの場合に新しい currentBodyIndex を設定
            int newBodyIndex = bodyIndices.Count;
            bodyIndices.Add(body.Id, newBodyIndex);
        }

        int objectIndex = bodyIndices[body.Id];
        if (objectIndex >= 0 && objectIndex < right.Length)
        {
            SetrMarkPos(right[objectIndex], JointId.HandRight, frame, bodyIndex);
        }
        if (objectIndex >= 0 && objectIndex < left.Length)
        {
            SetrMarkPos(left[objectIndex], JointId.HandLeft, frame, bodyIndex);
        }
        //Debug.Log(frame.NumberOfBodies);
        _nowvrPosition[bodyIndex] = GetrVector3(JointId.HandRight, frame, bodyIndex);
        nowvrPosition[bodyIndex] = new Vector3(_nowvrPosition[bodyIndex].x, _nowvrPosition[bodyIndex].y, _nowvrPosition[bodyIndex].z);
        nowrJoints[bodyIndex] = skeleton.GetJoint(JointId.HandRight);

        _nowvPosition[bodyIndex] = GetVector3(JointId.HandLeft, frame, bodyIndex);
        nowvPosition[bodyIndex] = new Vector3(_nowvPosition[bodyIndex].x, _nowvPosition[bodyIndex].y, _nowvPosition[bodyIndex].z);
        nowJoints[bodyIndex] = skeleton.GetJoint(JointId.HandLeft);

        if (risThrow == false)
        {

        }
        Debug.Log(GetrAccerareta(0).magnitude);
        ////加速度判定
        if (0.12 < GetrAccerareta(0).magnitude && currentframe > 60 && GetrVelocity(0).z < -5)
        {

            risThrow = true;
        }
        else
        {

        }

        if (risThrow)
        {
            rightEffects[0].SetActive(true);
            b++;
            if (b > 10)
            {
                risThrow = false;
                b = 0;
            }
            ;

        }
        else
        {
            rightEffects[0].SetActive(false);

        }

        if (risThrow2 == false)
        {

        }
        Debug.Log(bodyIndex);
        ////加速度判定
        if (0.10 < GetrAccerareta(1).magnitude && currentframe > 60 && GetrVelocity(1).z < -5)
        {

            risThrow2 = true;
        }
        else
        {

        }

        if (risThrow2)
        {
            rightEffects[1].SetActive(true);
            c++;
            if (c > 10)
            {
                risThrow2 = false;
                c = 0;
            }
            ;

        }
        else
        {
            rightEffects[1].SetActive(false);

        }
        if (isThrow == false)
        {

        }
        // 加速度判定
        if (0.2 < GetAccerareta(0).magnitude && currentframe > 60)
        {

            isThrow = true;
        }
        else
        {

        }

        if (isThrow)
        {
            leftEffects[0].SetActive(true);

            a++;

            if (a > 10)
            {
                isThrow = false;
                a = 0;
            }


        }

        else
        {

            leftEffects[0].SetActive(false);

        }
        if (isThrow2 == false)
        {

        }
        // 加速度判定
        if (0.2 < GetAccerareta(1).magnitude && currentframe > 60)
        {

            isThrow2 = true;
        }
        else
        {

        }

        if (isThrow2)
        {
            leftEffects[1].SetActive(true);

            d++;

            if (a > 10)
            {
                isThrow2 = false;
                d = 0;
            }


        }

        else
        {

            leftEffects[1].SetActive(false);

        }
        _prevrPosition[bodyIndex] = GetrVector3(JointId.HandRight, frame, bodyIndex);
        prevrPosition[bodyIndex] = new Vector3(_prevrPosition[bodyIndex].x, _prevrPosition[bodyIndex].y, _prevrPosition[bodyIndex].z);
        prerJoints[bodyIndex] = skeleton.GetJoint(JointId.HandRight);
        prerVelocity[bodyIndex] = GetrVelocity(bodyIndex);

        _prevPosition[bodyIndex] = GetVector3(JointId.HandLeft, frame, bodyIndex);
        prevPosition[bodyIndex] = new Vector3(_prevPosition[bodyIndex].x, _prevPosition[bodyIndex].y, _prevPosition[bodyIndex].z);
        preJoints[bodyIndex] = skeleton.GetJoint(JointId.HandLeft);
        preVelocity[bodyIndex] = GetVelocity(bodyIndex);
        currentframe++;
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
                int numberOfBodies = (int)frame.NumberOfBodies;
                // && koji == "koji"
                if (numberOfBodies > 0)
                {
                    for (int i = 0; i < numberOfBodies; i++)
                    {
                        //Debug.Log("a");
                        ProcessBody(frame, i);
                    }
                }
            }
        }
    }
    Vector3 GetAccerareta(int index)
    {
        return (GetVelocity(index) - preVelocity[index]) / 30;
    }

    Vector3 GetrAccerareta(int index)
    {
        return ((GetrVelocity(index) - prerVelocity[index]) / 30);
    }

    Vector3 GetVelocity(int index)
    {
        var position = nowJoints[index].Position - preJoints[index].Position;
        var velocity = GetVector3(position) / 30;
        return velocity;
    }

    Vector3 GetrVelocity(int index)
    {
        var position = nowrJoints[index].Position - prerJoints[index].Position;
        var velocity = GetrVector3(position) / 30;
        return velocity;
    }

    Vector3 GetVector3(System.Numerics.Vector3 vector3)
    {
        return new Vector3(vector3.X, vector3.Y, vector3.Z);
    }


    Vector3 GetrVector3(System.Numerics.Vector3 vector3)
    {
        return new Vector3(vector3.X, vector3.Y, vector3.Z);
    }

    Vector3 GetVector3(JointId jointId, Frame frame, int index)
    {
        var joint = frame.GetBodySkeleton((uint)index).GetJoint(jointId);
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }
    Vector3 GetrVector3(JointId jointId, Frame frame, int index)
    {
        var joint = frame.GetBodySkeleton((uint)index).GetJoint(jointId);
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }
    private void SetMarkPos(GameObject effectPrefab, JointId jointId, Frame frame)
    {
        var joint = frame.GetBodySkeleton(1).GetJoint(jointId);
        effectPrefab.transform.localPosition = new Vector3(joint.Position.X, -joint.Position.Y, joint.Position.Z);
    }

    private void SetrMarkPos(GameObject right, JointId jointId, Frame frame, int index)
    {
        var joint = frame.GetBodySkeleton((uint)index).GetJoint(jointId);
        right.transform.localPosition = new Vector3(joint.Position.X, -joint.Position.Y, joint.Position.Z) / 40;
    }

    private void OnDestroy()
    {
        kinect.StopCameras();
    }
}
