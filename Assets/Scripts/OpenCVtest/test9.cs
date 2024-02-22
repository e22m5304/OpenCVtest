using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;
using System;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;
using System.Collections.Generic;

public class test9 : MonoBehaviour
{
   
    
    private Vector3[] preVelocity;
    private Vector3[] prerVelocity;
    private Vector3[] prefVelocity;
    private Vector3[] prerfVelocity;
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

    [SerializeField]
    GameObject[] righFots;
    [SerializeField]
    GameObject[] rightFotEffects;

    [SerializeField]
    GameObject[] leftFots;

    [SerializeField]
    GameObject[] leftFotEffects;
    int a = 0;
    int b = 0;
    [SerializeField] private bool isThrow = false;
    [SerializeField] private bool risThrow = false;
    [SerializeField] private bool fisThrow = false;
    [SerializeField] private bool frisThrow = false;
    int dd = 0;
    private Vector3[] _prevPosition;
    private Vector3[] _prevrPosition;
    private Vector3[] _prevfPosition;
    private Vector3[] _prevrfPosition;

    private Vector3[] prevPosition;
    private Vector3[] prevrPosition;
    private Vector3[] prevfPosition;
    private Vector3[] prevrfPosition;

    private Vector3[] _nowvPosition;
    private Vector3[] _nowvrPosition;
    private Vector3[] _nowvfPosition;
    private Vector3[] _nowvrfPosition;

    private Vector3[] nowvPosition;
    private Vector3[] nowvrPosition;
    private Vector3[] nowvfPosition;
    private Vector3[] nowvrfPosition;

    private Joint[] preJoints;
    private Joint[] prerJoints;
    private Joint[] prefJoints;
    private Joint[] prerfJoints;

    private Joint[] nowJoints;
    private Joint[] nowrJoints;
    private Joint[] nowfJoints;
    private Joint[] nowrfJoints;

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

        _nowvfPosition = new Vector3[MaxBodies];
        _nowvrfPosition = new Vector3[MaxBodies];
        nowvfPosition = new Vector3[MaxBodies];
        nowvrfPosition = new Vector3[MaxBodies];

        _prevPosition = new Vector3[MaxBodies];
        _prevrPosition = new Vector3[MaxBodies];
        prevPosition  = new Vector3[MaxBodies];
        prevrPosition  = new Vector3[MaxBodies];

        _prevfPosition = new Vector3[MaxBodies];
        _prevrfPosition = new Vector3[MaxBodies];
        prevfPosition = new Vector3[MaxBodies];
        prevrfPosition = new Vector3[MaxBodies];

        preJoints = new Joint[MaxBodies];
        prerJoints = new Joint[MaxBodies];
        nowJoints = new Joint[MaxBodies];
        nowrJoints = new Joint[MaxBodies];
        preVelocity =  new Vector3[MaxBodies];
        prerVelocity =  new Vector3[MaxBodies];


        prefJoints = new Joint[MaxBodies];
        prerfJoints = new Joint[MaxBodies];
        nowfJoints = new Joint[MaxBodies];
        nowrfJoints = new Joint[MaxBodies];
        prefVelocity = new Vector3[MaxBodies];
        prerfVelocity = new Vector3[MaxBodies];

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

                        var skeleton = frame.GetBodySkeleton((uint)i);
                       
                        
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
                        Debug.Log(numberOfBodies);
                        //Debug.Log(skeleton);

                        _nowvrPosition[i] = GetrVector3(JointId.HandRight, frame,i);
                        nowvrPosition[i] = new Vector3(_nowvrPosition[i].x, _nowvrPosition[i].y, _nowvrPosition[i].z);
                        nowrJoints[i] = skeleton.GetJoint(JointId.HandRight);

                        _nowvPosition[i] = GetVector3(JointId.HandLeft, frame, i);
                        nowvPosition[i] = new Vector3(_nowvPosition[i].x, _nowvPosition[i].y, _nowvPosition[i].z);
                        nowJoints[i] = skeleton.GetJoint(JointId.HandLeft);

                        _nowvrfPosition[i] = GetrfVector3(JointId.AnkleRight, frame, i);
                        nowvrfPosition[i] = new Vector3(_nowvrfPosition[i].x, _nowvrfPosition[i].y, _nowvrfPosition[i].z);
                        nowrfJoints[i] = skeleton.GetJoint(JointId.AnkleRight);

                        _nowvfPosition[i] = GetfVector3(JointId.AnkleLeft, frame, i);
                        nowvfPosition[i] = new Vector3(_nowvfPosition[i].x, _nowvfPosition[i].y, _nowvfPosition[i].z);
                        nowfJoints[i] = skeleton.GetJoint(JointId.AnkleLeft);



                        //角度算出
                        float R_Fot = this.get_angle(R_Hip, R_Knee, R_Ankle);
                        float L_hand_right = this.get_dis(nowJoints[i], preJoints[i]);


                        this.SetrMarkPos(this.rightHands[i], JointId.HandRight, frame, i);
                        this.SetMarkPos(this.leftHands[i], JointId.HandLeft, frame, i); 
                        this.SetMarkPos(this.righFots[i], JointId.AnkleRight, frame, i);
                        this.SetMarkPos(this.leftFots[i], JointId.AnkleLeft, frame, i);
                        // 左手
                        //Debug.Log(i);

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
                             //left2.SetActive(true);
                             a++;
                            
                            if (a > 10)
                            {
                                isThrow = false;
                                a = 0;
                            }


                            // this.leftHand.transform.localPosition = -Vector3.Slerp(nowvPosition / 50, prevPosition / 50, 1);
                        }

                        else
                        {

                             leftEffects[i].SetActive(false);
                            //left2.SetActive(false);

                            //this.leftHand.GetComponent<Rigidbody>().velocity = new Vector3(GetVelocity().x * -10, GetVelocity().y * -5, GetVelocity().z * 5);



                            //isThrow = true;
                            // this.leftHand.transform.localPosition = -Vector3.Slerp(nowvPosition / 50, prevPosition / 50, 1);
                            //this.SetMarkPos(this.rightHand, JointId.ElbowRight, frame);
                        }

                        //Debug.Log("S" + GetrVelocity(i).z);

                        //Debug.Log("A" + GetrAccerareta(i).magnitude);
                        //右手
                        if (risThrow == false)
                       {

                       }


                       ////加速度判定
                       if (0.12 < GetrAccerareta(i).magnitude && currentframe > 60 && GetrVelocity(i).z < -2)
                       {

                           risThrow = true;
                       }
                       else
                       {

                       }

                       ////true
                       if (risThrow)
                       {
                            rightEffects[i].SetActive(true);
                           //right2.SetActive(true);
                           
                           //this.SetrMarkPos(this.rightHands[i], JointId.HandRight, frame, i);

                            b++;
                            if (b > 7)
                           {
                               risThrow = false;
                               b = 0;
                           }
                           ;
                           //    this.leftHand.transform.localPosition = -Vector3.Slerp(nowvPosition / 50, prevPosition / 50, 1);
                       }
                       else
                       {
                            rightEffects[i].SetActive(false);
                          
                       }
                        //Debug.Log(GetfAccerareta(i).magnitude);

                        if (fisThrow == false)
                        {

                        }

                        if (0.2 < GetfAccerareta(i).magnitude && currentframe > 60 && risThrow == false && fisThrow == false && isThrow == false)
                        {

                            fisThrow = true;
                        }
                        else
                        {

                        }


                        //true
                        if (fisThrow)
                        {
                            leftFotEffects[i].SetActive(true);

                            // this.leftHand.transform.localPosition = -Vector3.Slerp(nowvPosition / 50, prevPosition / 50, 1);
                        }

                        else
                        {
                            leftFotEffects[i].SetActive(false);
                            this.leftFots[i].GetComponent<Rigidbody>().velocity = new Vector3(GetfVelocity(i).x * -10, GetfVelocity(i).y * -5, GetfVelocity(i).z * 5);
                            this.SetMarkPos(this.leftFots[i], JointId.AnkleLeft, frame,i);


                            //isThrow = true;
                            // this.leftHand.transform.localPosition = -Vector3.Slerp(nowvPosition / 50, prevPosition / 50, 1);
                            //this.SetMarkPos(this.rightHand, JointId.ElbowRight, frame);
                        }
                        if (this.leftFots[i].transform.position.z < -30 || this.leftFots[i].transform.position.z > 50 || -20 > this.leftFots[i].transform.position.x || 20 < this.leftFots[i].transform.position.x || -30 > this.leftFots[i].transform.position.y || 15 < this.leftFots[i].transform.position.y)
                        {

                            fisThrow = false;
                        }

                        if (fisThrow == false) { }

                        //加速度判定
                        if (0.10 < GetrfAccerareta(i).magnitude && currentframe > 60 && GetrfVelocity(i).y > 0 && R_Fot > 70 && risThrow == false && isThrow == false && frisThrow == false)
                        {
                            fisThrow = true;

                        }
                        else { }
                        //true
                        if (fisThrow)
                        {
                            rightFotEffects[i].SetActive(true);
                            a++;
                            if (a > 20)
                            {
                                fisThrow = false;
                                a = 0;
                            }
                            // this.leftHand.transform.localPosition = -Vector3.Slerp(nowvPosition / 50, prevPosition / 50, 1);
                        }
                        else
                        {
                            rightFotEffects[i].SetActive(false);
                        }
                        _prevrPosition[i] = GetrVector3(JointId.HandRight, frame,i);
                        prevrPosition[i] = new Vector3(_prevrPosition[i].x, _prevrPosition[i].y, _prevrPosition[i].z);
                        prerJoints[i] = skeleton.GetJoint(JointId.HandRight);
                        prerVelocity[i] = GetrVelocity(i);

                        _prevPosition[i] = GetVector3(JointId.HandLeft, frame, i);
                        prevPosition[i] = new Vector3(_prevPosition[i].x, _prevPosition[i].y, _prevPosition[i].z);
                        preJoints[i] = skeleton.GetJoint(JointId.HandLeft);
                        preVelocity[i] = GetVelocity(i);

                        _prevrfPosition[i] = GetrfVector3(JointId.AnkleRight, frame, i);
                        prevrfPosition[i] = new Vector3(_prevrfPosition[i].x, _prevrfPosition[i].y, _prevrfPosition[i].z);
                        prerfJoints[i] = skeleton.GetJoint(JointId.AnkleRight);
                        prerfVelocity[i] = GetrfVelocity(i);

                        _prevfPosition[i] = GetfVector3(JointId.AnkleLeft, frame, i);
                        prevfPosition[i] = new Vector3(_prevfPosition[i].x, _prevfPosition[i].y, _prevfPosition[i].z);
                        prefJoints[i] = skeleton.GetJoint(JointId.AnkleLeft);
                        prefVelocity[i] = GetfVelocity(i);
                        currentframe++;
                    }
                }
            }
        }
    }


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
    private float get_dis(Joint A, Joint B)
    {
        Vector3 BA = transform.position;
        BA.x = A.Position.X - B.Position.X;
        BA.y = A.Position.Y - B.Position.Y;
        BA.z = A.Position.Z - B.Position.Z;

        double norm_BA = Math.Sqrt(BA.x * BA.x + BA.y * BA.y + BA.z * BA.z);
        // Debug.Log(norm_BA/50);
        return (float)norm_BA;
    }
    Vector3 GetAccerareta(int index)
    {
        return (GetVelocity(index) - preVelocity[index]) / 30;
    }

    Vector3 GetrAccerareta(int index)
    {
        return ((GetrVelocity(index) - prerVelocity[index]) / 30);
    }

    Vector3 GetfAccerareta(int index)
    {
        return (GetfVelocity(index) - prefVelocity[index]) / 30;
    }

    Vector3 GetrfAccerareta(int index)
    {
        return ((GetrfVelocity(index) - prerfVelocity[index]) / 30);
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
    Vector3 GetfVelocity(int index)
    {
        var position = nowJoints[index].Position - preJoints[index].Position;
        var velocity = GetfVector3(position) / 30;
        return velocity;
    }

    Vector3 GetrfVelocity(int index)
    {
        var position = nowrJoints[index].Position - prerJoints[index].Position;
        var velocity = GetrfVector3(position) / 30;
        return velocity;
    }
    Vector3 GetfVector3(System.Numerics.Vector3 vector3)
    {
        return new Vector3(vector3.X, vector3.Y, vector3.Z);
    }
    

    Vector3 GetrfVector3(System.Numerics.Vector3 vector3)
    {
        return new Vector3(vector3.X, vector3.Y, vector3.Z);
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
    Vector3 GetfVector3(JointId jointId, Frame frame, int index)
    {
        var joint = frame.GetBodySkeleton((uint)index).GetJoint(jointId);
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }
    Vector3 GetrfVector3(JointId jointId, Frame frame, int index)
    {
        var joint = frame.GetBodySkeleton((uint)index).GetJoint(jointId);
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }
    private void SetMarkPos(GameObject effectPrefab, JointId jointId, Frame frame, int index)
    {
        var joint = frame.GetBodySkeleton((uint)index).GetJoint(jointId);
        effectPrefab.transform.localPosition = new Vector3(joint.Position.X, -joint.Position.Y, joint.Position.Z) / 35;
        //nowJoints[index] = joint;
    }
    private void SetrMarkPos(GameObject effectPrefab, JointId jointId, Frame frame, int index)
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
