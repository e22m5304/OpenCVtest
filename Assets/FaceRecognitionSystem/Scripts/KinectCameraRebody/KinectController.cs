using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;
using System;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;

public class KinectController : MonoBehaviour
{
    public int CameraIndex;
    Device kinect;
    Tracker tracker;

    [SerializeField]
    GameObject right;
    

    private void Start()
    {
        InitKinect();
        Task t = KinectLoop();
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
                    //right2.SetActive(false);
                    SetMarkPos(this.right, JointId.HandRight, frame);
                }
            }
        }
    }

    private void SetMarkPos(GameObject effectPrefab, JointId jointId, Frame frame)
    {
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        effectPrefab.transform.localPosition = new Vector3(joint.Position.X, -joint.Position.Y, joint.Position.Z) / 1;
    }

    private void OnDestroy()
    {
        if (kinect != null)
        {
            kinect.StopCameras();
            kinect.Dispose(); // ‚±‚ê‚ð’Ç‰Á
        }
    }
}
