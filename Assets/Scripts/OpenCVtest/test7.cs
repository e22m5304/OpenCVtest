using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;
using System;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;

public class test7 : MonoBehaviour
{
    // Kinect device and related variables
    Device kinect;
    Texture2D kinectColorTexture;
    
    Tracker tracker;

    // Various flags and variable
    // Game objects to represent body parts
    [SerializeField]
    GameObject[] right;
    [SerializeField]
    GameObject[] cube;

    private void Start()
    {
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
                if (numberOfBodies > 0) // この判定がないと撮影範囲から人がいなくなった場合に後の処理で止まる
                {
                    for (int i = 0; i < numberOfBodies; i++)
                    {
                        Debug.Log(i);
                        var skeleton = frame.GetBodySkeleton((uint)i);
                        this.SetrMarkPos(this.right[i], JointId.HandRight, frame, i);
                        // 右手
                        // ↑指定するJointIdを変えれば腰や頭に火の玉を出すことも可能
                    }
                }
                /*if (numberOfBodies > 0)
                {
                    // Extract joint information from each tracked skeleton
                    for (int i = 0; i < numberOfBodies; i++)
                    {
                        var skeleton = frame.GetBodySkeleton((uint)i);
                        Debug.Log(i);
                        // Set positions of game objects to represent body parts
                        SetrMarkPos(this.right[i], JointId.HandRight, frame, i);
                        SetMarkPos(this.cube[i], JointId.HandLeft, frame, i);
                    }


                }*/
            }
        }
    }

    // ... (その他のメソッドは変更なし)
    
    // Calculate and return velocity difference for right foot

    // Set the position of a game object to represent a body part
    private void SetMarkPos(GameObject effectPrefab, JointId jointId, Frame frame, int index)
    {
        var joint = frame.GetBodySkeleton((uint)index).GetJoint(jointId);
        effectPrefab.transform.localPosition = new Vector3(-joint.Position.X, -joint.Position.Y, joint.Position.Z);
    }

    // Set the position of the 'right' game object to represent a body part
    private void SetrMarkPos(GameObject right, JointId jointId, Frame frame, int index)
    {
        var joint = frame.GetBodySkeleton((uint)index).GetJoint(jointId);
        right.transform.localPosition = new Vector3(-joint.Position.X, -joint.Position.Y, joint.Position.Z) / 40;
    }

    private void OnDestroy()
    {
        //kinect.StopCameras();
    }
}
