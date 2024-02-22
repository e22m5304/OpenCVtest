using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;
using System;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;

public class test5 : MonoBehaviour
{
    // Kinect device and related variables
    Device kinect;
    Texture2D kinectColorTexture;

    Tracker tracker;
    string koji = FaceRecognizer.koji2;

    // Various flags and variable
    // Game objects to represent body parts
    [SerializeField]
    GameObject[] right;
    [SerializeField]
    GameObject[] cube;

    private int currentBodyIndex = -1;

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
    private void ProcessBody(Frame frame, int bodyIndex)
    {
        var body = frame.GetBody((uint)bodyIndex);

        if (bodyIndex == 0)
        {
            // Å‰‚Ìƒ{ƒfƒB‚Ìê‡‚É currentBodyIndex ‚ðÝ’è
            currentBodyIndex = (int)body.Id;
        }

        int objectIndex = (int)body.Id - currentBodyIndex;
        if (objectIndex >= 0 && objectIndex < right.Length)
        {
            SetrMarkPos(right[objectIndex], JointId.HandRight, frame, bodyIndex);
        }
        Debug.Log(frame.NumberOfBodies);
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

                if (numberOfBodies > 0 )
                {
                    for (int i = 0; i < numberOfBodies; i++)
                    {
                        ProcessBody(frame, i);
                    }
                }
            }
        }
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
