using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;
using System;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;

public class test8 : MonoBehaviour
{
    //public int MaxBodies = 2; // 最大のボディ数
    private Vector3[] preVelocity;
    private Vector3[] prerVelocity;
    Device kinect;
    Tracker tracker;

    [SerializeField]
    GameObject[] rightHands;
    [SerializeField]
    GameObject[] rightEffects;
    
    [SerializeField]
    GameObject[] leftHands;
    
    [SerializeField]
    GameObject[] leftEffects;
    int a = 0;
    int b = 0;
    [SerializeField] private bool isThrow = false;
    [SerializeField] private bool risThrow = false;
    int dd = 0;
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
        prevPosition  = new Vector3[MaxBodies];
        prevrPosition  = new Vector3[MaxBodies];

        preJoints = new Joint[MaxBodies];
        prerJoints = new Joint[MaxBodies];
        nowJoints = new Joint[MaxBodies];
        nowrJoints = new Joint[MaxBodies];
        preVelocity =  new Vector3[MaxBodies];
        prerVelocity =  new Vector3[MaxBodies];

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
               
                int numberOfBodies = (int)frame.NumberOfBodies;
                if (frame.NumberOfBodies > 0)
                {
                    
                    for (int i = 0; i < numberOfBodies; i++)
                    {
 
                        var skeleton = frame.GetBodySkeleton(0);
                        //var skeleton2 = frame.GetBodySkeleton(1);
                        //Debug.Log(numberOfBodies);

                        _nowvrPosition[i] = GetrVector3(JointId.HandRight, frame,i);
                        nowvrPosition[i] = new Vector3(_nowvrPosition[i].x, _nowvrPosition[i].y, _nowvrPosition[i].z);
                        nowrJoints[i] = skeleton.GetJoint(JointId.HandRight);

                        _nowvPosition[i] = GetVector3(JointId.HandLeft, frame, i);
                        nowvPosition[i] = new Vector3(_nowvPosition[i].x, _nowvPosition[i].y, _nowvPosition[i].z);
                        nowJoints[i] = skeleton.GetJoint(JointId.HandLeft);

                        this.SetrMarkPos(this.rightHands[i], JointId.HandRight, frame, i);
                        this.SetMarkPos(this.leftHands[i], JointId.HandLeft, frame, i);
                        // 左手
                        Debug.Log(i);

                        if (isThrow == false)
                       {

                       }
                       // 加速度判定
                       if (0.2 < GetAccerareta(i).magnitude && currentframe > 60)
                       {

                           isThrow = true;
                       }
                       else
                       {

                       }
                        
                        if (isThrow)
                        {
                             leftEffects[i].SetActive(true);
                             
                             a++;
                            
                            if (a > 10)
                            {
                                isThrow = false;
                                a = 0;
                            }

                   
                        }

                        else
                        {

                             leftEffects[i].SetActive(false);
                          
                        }                      

                        //右手
                        if (risThrow == false)
                       {

                       }

                       ////加速度判定
                       if (0.12 < GetrAccerareta(i).magnitude && currentframe > 60 && GetrVelocity(i).z < -5)
                       {

                           risThrow = true;
                       }
                       else
                       {

                       }

                       if (risThrow)
                       {
                            rightEffects[i].SetActive(true);
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
                            rightEffects[i].SetActive(false);
                          
                       }
                        
                        _prevrPosition[i] = GetrVector3(JointId.HandRight, frame,i);
                        prevrPosition[i] = new Vector3(_prevrPosition[i].x, _prevrPosition[i].y, _prevrPosition[i].z);
                        prerJoints[i] = skeleton.GetJoint(JointId.HandRight);
                        prerVelocity[i] = GetrVelocity(i);

                        _prevPosition[i] = GetVector3(JointId.HandLeft, frame, i);
                        prevPosition[i] = new Vector3(_prevPosition[i].x, _prevPosition[i].y, _prevPosition[i].z);
                        preJoints[i] = skeleton.GetJoint(JointId.HandLeft);
                        preVelocity[i] = GetVelocity(i);
                        currentframe++;
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

    Vector3 GetVector3(JointId jointId, Frame frame,int index)
    {
        var joint = frame.GetBodySkeleton((uint)index).GetJoint(jointId);
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }
    Vector3 GetrVector3(JointId jointId, Frame frame,int index)
    {
        var joint = frame.GetBodySkeleton((uint)index).GetJoint(jointId);
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }

    private void SetMarkPos(GameObject effectPrefab, JointId jointId, Frame frame, int index)
    {
        var joint = frame.GetBodySkeleton((uint)index).GetJoint(jointId);
        effectPrefab.transform.localPosition = new Vector3(joint.Position.X, -joint.Position.Y, joint.Position.Z) / 35;
        //nowJoints[index] = joint;
    }private void SetrMarkPos(GameObject effectPrefab, JointId jointId, Frame frame, int index)
    {
        var joint = frame.GetBodySkeleton((uint)index).GetJoint(jointId);
        effectPrefab.transform.localPosition = new Vector3(joint.Position.X, -joint.Position.Y, joint.Position.Z) / 35;
        //nowJoints[index] = joint;
    }



    private void OnDestroy()    
    {
        kinect.StopCameras();
    }
}
