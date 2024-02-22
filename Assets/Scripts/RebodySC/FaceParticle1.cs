using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;
using System;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;

public class FaceParticle1 : MonoBehaviour
{

    Vector3 preVelocity;
    Vector3 prerVelocity;
    public int CameraIndex;
    Device kinect;
    Tracker tracker;
    [SerializeField]
    GameObject rightHand;
    [SerializeField]
    GameObject right;
    [SerializeField]
    GameObject leftHand;
    [SerializeField]
    GameObject left;
    
    int a = 0;
    int b = 0;

    [SerializeField] bool isThrow = false;
    [SerializeField] bool risThrow = false;

    private Vector3 _prevPosition;
    private Vector3 _prevrPosition;

    private Vector3 prevPosition;
    private Vector3 prevrPosition;

    private Vector3 _nowvPosition;
    private Vector3 _nowvrPosition;

    private Vector3 nowvPosition;
    private Vector3 nowvrPosition;

    private Joint preJoints;
    private Joint prerJoints;

    private Joint nowJoints;
    private Joint nowrJoints;
    float timer;
    int currentframe;
    private void Start()
    {
        InitKinect();
        currentframe = 0;
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

                    _nowvPosition = GetVector3(JointId.HandLeft, frame);
                    _nowvrPosition = GetrVector3(JointId.HandRight, frame);

                    nowvPosition = new Vector3(_nowvPosition.x, _nowvPosition.y, _nowvPosition.z);
                    nowvrPosition = new Vector3(_nowvrPosition.x, _nowvrPosition.y, _nowvrPosition.z);
                    //right2.SetActive(false);
                    //this.SetMarkPos(this.rightHand, JointId.HandRight, frame);
                    nowJoints = skeleton.GetJoint(JointId.HandLeft);
                    nowrJoints = skeleton.GetJoint(JointId.HandRight);
                    this.SetMarkPos(this.leftHand, JointId.HandLeft, frame);
                    this.SetMarkPos(this.rightHand, JointId.HandRight, frame);
                    //左手
                    if (isThrow == false)
                    {

                    }

                    Debug.Log(GetVelocity());
                    ////加速度判定
                    if (0.15 < GetAccerareta().magnitude && currentframe > 60 && GetVelocity().z < -5 )
                    {

                        isThrow = true;
                    }
                    else
                    {

                    }

                    //true
                    if (isThrow)
                    {
                        left.SetActive(true);
                        a++;

                        if (a > 20)
                        {
                            isThrow = false;
                            a = 0;
                        }
                        // this.leftHand.transform.localPosition = -Vector3.Slerp(nowvPosition / 50, prevPosition / 50, 1);
                    }

                    else
                    {
                        left.SetActive(false);
                        //this.leftHand.GetComponent<Rigidbody>().velocity = new Vector3(GetVelocity().x * -10, GetVelocity().y * -5, GetVelocity().z * 5);



                        //isThrow = true;
                        // this.leftHand.transform.localPosition = -Vector3.Slerp(nowvPosition / 50, prevPosition / 50, 1);
                        //this.SetMarkPos(this.rightHand, JointId.ElbowRight, frame);
                    }

                    //右手
                    if (risThrow == false)
                    {

                    }
                    print("accera"+GetrAccerareta().magnitude);
                   
                    ////加速度判定
                    if (0.2 < GetrAccerareta().magnitude && currentframe > 60 )
                    {

                        risThrow = true;
                    }
                    else
                    {

                    }

                    ////true
                    if (risThrow)
                    {
                        right.SetActive(true);
                        b++;
                        this.SetMarkPos(this.rightHand, JointId.HandRight, frame);

                        if (b > 5)
                        {
                            risThrow = false;
                            b = 0;
                        }
                        ;
                        //    this.leftHand.transform.localPosition = -Vector3.Slerp(nowvPosition / 50, prevPosition / 50, 1);
                    }

                    else
                    {
                        right.SetActive(false);
                        //this.rightHand.GetComponent<Rigidbody>().velocity = new Vector3(GetrVelocity().x * -10, GetrVelocity().y * -5, GetrVelocity().z * 5);
                        

                        // this.leftHand.transform.localPosition = -Vector3.Slerp(nowvPosition / 50, prevPosition / 50, 1);
                        //this.SetMarkPos(this.rightHand, JointId.ElbowRight, frame);
                    }
                    //SetMarkPos(this.right, JointId.HandRight, frame);
                    //右
                    _prevrPosition = GetrVector3(JointId.HandRight, frame);
                    prevrPosition = new Vector3(_prevrPosition.x, _prevrPosition.y, _prevrPosition.z);
                    prerJoints = skeleton.GetJoint(JointId.HandRight);
                    prerVelocity = GetrVelocity();

                    //左
                    _prevPosition = GetVector3(JointId.HandLeft, frame);
                    prevPosition = new Vector3(_prevPosition.x, _prevPosition.y, _prevPosition.z);
                    preJoints = skeleton.GetJoint(JointId.HandLeft);
                    preVelocity = GetVelocity();


                    currentframe++;
                }
            }
        }
    }

    
    Vector3 GetAccerareta()
    {
        return ((GetVelocity() - preVelocity) / 30);

    }
    Vector3 GetrAccerareta()
    {
        return ((GetrVelocity() - prerVelocity) / 30);

    }
    Vector3 GetVelocity()
    {
        var position = nowJoints.Position - preJoints.Position;
        var velocity = GetVector3(position) / 30;
        return velocity;
    }
    Vector3 GetrVelocity()
    {
        var position = nowrJoints.Position - prerJoints.Position;
        var velocity = GetrVector3(position) / 30;
        return velocity;
    }
    Vector3 GetVector3(System.Numerics.Vector3 vector3)
    {
        return new Vector3(vector3.X, vector3.Y, vector3.Z);
    }
    Vector3 GetVector3(JointId jointId, Frame frame)
    {
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }
    Vector3 GetrVector3(System.Numerics.Vector3 vector3)
    {
        return new Vector3(vector3.X, vector3.Y, vector3.Z);
    }
    Vector3 GetrVector3(JointId jointId, Frame frame)
    {
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }
    private void SetrMarkPos(GameObject effectPrefab, JointId jointId, Frame frame)
    {
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        effectPrefab.transform.localPosition = new Vector3(-joint.Position.X, -joint.Position.Y, joint.Position.Z) / 35;

    }

    private void SetMarkPos(GameObject effectPrefab, JointId jointId, Frame frame)
    {
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        effectPrefab.transform.localPosition = new Vector3(joint.Position.X, -joint.Position.Y, joint.Position.Z) / 35;
    }

    private void OnDestroy()
    {
        kinect.StopCameras();
    }
}
