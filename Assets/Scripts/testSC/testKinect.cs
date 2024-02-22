using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;
using System;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;

public class testKinect : MonoBehaviour
{
    Device kinect;
    Texture2D kinectColorTexture;
    [SerializeField]
    UnityEngine.UI.RawImage rawColorImg;
    int a = 0;
    int b = 0;
    Tracker tracker;
    [SerializeField] bool isThrow = false;
    [SerializeField] bool risThrow = false;
    [SerializeField] bool fisThrow = false;
    [SerializeField] bool lisThrow = false;
    //List<float> preVelocity;
    Vector3 preVelocity;
    Vector3 prerVelocity;
    Vector3 prerfVelocity;
    Vector3 prelfVelocity;
    [SerializeField]
    GameObject leftHand;
    [SerializeField]
    GameObject rightHand;
    [SerializeField]
    GameObject leftAnkle;
    [SerializeField]
    GameObject rightAnkle;
    [SerializeField]
    GameObject left;
    [SerializeField]
    GameObject right;
    [SerializeField]
    GameObject cube;
    [SerializeField]
    GameObject right_f;
    [SerializeField]
    GameObject left_f;

    // List<Joint> preJoints = new List<Joint>();
    //List<Joint> nowJoints = new List<Joint>();

    private Vector3 _prevPosition;
    private Vector3 _prevrPosition;
    private Vector3 _prevrfPosition;
    private Vector3 _prevlfPosition;

    private Vector3 prevPosition;
    private Vector3 prevrPosition;
    private Vector3 prevrfPosition;
    private Vector3 prevlfPosition;
    private Vector3 _nowvPosition;
    private Vector3 _nowvrPosition;
    private Vector3 _nowvrfPosition;
    private Vector3 _nowvlfPosition;

    private Vector3 nowvPosition;
    private Vector3 nowvrPosition;
    private Vector3 nowvrfPosition;
    private Vector3 nowvlfPosition;

    private Joint preJoints;
    private Joint prerJoints;
    private Joint nowJoints;
    private Joint nowrJoints;

    private Joint prerfJoints;
    private Joint prelfJoints;
    private Joint nowrfJoints;
    private Joint nowlfJoints;
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

                    _nowvPosition = GetVector3(JointId.HandLeft, frame);
                    _nowvrPosition = GetVector3(JointId.HandRight, frame);

                    _nowvrfPosition = GetVector3(JointId.AnkleRight, frame);
                    _nowvlfPosition = GetVector3(JointId.AnkleLeft, frame);

                    nowvPosition = new Vector3(_nowvPosition.x, _nowvPosition.y, _nowvPosition.z);
                    nowvrPosition = new Vector3(_nowvrPosition.x, _nowvrPosition.y, _nowvrPosition.z);
                    nowvrfPosition = new Vector3(_nowvrfPosition.x, _nowvrfPosition.y, _nowvrfPosition.z);
                    nowvlfPosition = new Vector3(_nowvlfPosition.x, _nowvlfPosition.y, _nowvlfPosition.z);

                    //for (int i = 0; i < 32; i++)
                    //{
                    //    nowJoints[i] = skeleton.GetJoint(i);
                    //}
                    //Debug.Log(GetrAccerareta());

                    //現在の位置
                    nowJoints = skeleton.GetJoint(JointId.HandLeft);
                    nowrJoints = skeleton.GetJoint(JointId.HandRight);
                    nowrfJoints = skeleton.GetJoint(JointId.AnkleRight);
                    nowlfJoints = skeleton.GetJoint(JointId.AnkleLeft);

                    //角度算出
                    float R_Fot = this.get_angle(R_Hip, R_Knee, R_Ankle);
                    float L_hand_right = this.get_dis(nowJoints, preJoints);
                    //this.SetMarkPos(this.rightHand, JointId.HandRight, frame);
                    //this.SetrMarkPos(this.rightHand, JointId.HandRight, frame);
                    //this.SetrMarkPos(this.leftHand, JointId.KneeLeft, frame);
                    // Debug.Log(GetAccerareta().magnitude);
                    if (fisThrow == false) { }


                    Debug.Log(GetrfVelocity());

                    //加速度判定
                    if (0.10 < GetrfAccerareta().magnitude && currentframe > 60 && GetrfVelocity().y > 0 && R_Fot > 70 && risThrow == false && isThrow == false && lisThrow == false)
                    {
                        fisThrow = true;

                    }
                    else { }
                    //true
                    if (fisThrow)
                    {
                        right_f.SetActive(true);
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
                        right_f.SetActive(false);
                    }

                    this.SetMarkPos(this.leftHand, JointId.HandLeft, frame);
                    this.SetMarkPos(this.rightAnkle, JointId.AnkleRight, frame);
                    //this.SetMarkPos(this.leftAnkle, JointId.AnkleLeft, frame);
                    this.SetMarkPos(this.rightHand, JointId.HandRight, frame);


                    //左手
                    if (isThrow == false)
                    {

                    }

                    Debug.Log(GetVelocity());
                    ////加速度判定
                    if (0.15 < GetAccerareta().magnitude && currentframe > 60 && GetVelocity().z < -5 && fisThrow == false && lisThrow == false)
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
                    //足



                    if (lisThrow == false)
                    {

                    }

                    if (0.2 < GetlfAccerareta().magnitude && currentframe > 60 && risThrow == false && fisThrow == false && isThrow == false)
                    {

                        lisThrow = true;
                    }
                    else
                    {

                    }


                    //true
                    if (lisThrow)
                    {
                        left_f.SetActive(true);

                        // this.leftHand.transform.localPosition = -Vector3.Slerp(nowvPosition / 50, prevPosition / 50, 1);
                    }

                    else
                    {
                        left_f.SetActive(false);
                        this.leftAnkle.GetComponent<Rigidbody>().velocity = new Vector3(GetlfVelocity().x * -10, GetlfVelocity().y * -5, GetlfVelocity().z * 5);
                        this.SetMarkPos(this.leftAnkle, JointId.AnkleLeft, frame);


                        //isThrow = true;
                        // this.leftHand.transform.localPosition = -Vector3.Slerp(nowvPosition / 50, prevPosition / 50, 1);
                        //this.SetMarkPos(this.rightHand, JointId.ElbowRight, frame);
                    }

                    if (this.leftAnkle.transform.position.z < -30 || this.leftAnkle.transform.position.z > 50 || -20 > this.leftAnkle.transform.position.x || 20 < this.leftAnkle.transform.position.x || -30 > this.leftAnkle.transform.position.y || 15 < this.leftAnkle.transform.position.y)
                    {

                        lisThrow = false;
                    }

                    //Debug.Log(GetVelocity().z);

                    //if (this.leftHand.transform.position.z < -1 || this.leftHand.transform.position.z > 30 || -15 > this.leftHand.transform.position.x || 15 < this.leftHand.transform.position.x || -6 > this.leftHand.transform.position.y || 8 < this.leftHand.transform.position.y)
                    //{

                    //    isThrow = false;
                    //}


                    if (risThrow == false)
                    {

                    }

                    ////加速度判定
                    if (0.2 < GetrAccerareta().magnitude && currentframe > 60 && fisThrow == false && lisThrow == false)
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


                        if (b > 20)
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
                        this.SetMarkPos(this.rightHand, JointId.HandRight, frame);

                        // this.leftHand.transform.localPosition = -Vector3.Slerp(nowvPosition / 50, prevPosition / 50, 1);
                        //this.SetMarkPos(this.rightHand, JointId.ElbowRight, frame);
                    }

                    //if (this.rightHand.transform.position.z < -1 || this.rightHand.transform.position.z > 30 || -15 > this.rightHand.transform.position.x || 15 < this.leftHand.transform.position.x || -6 > this.rightHand.transform.position.y || 8 < this.rightHand.transform.position.y)
                    //{

                    //    risThrow = false;
                    //}
                    //cube.SetActive(false);

                    //if (get_dis(HTRjoint, HTLjoint) < 100)
                    //{
                    //    cube.SetActive(true);
                    //    this.SetrMarkPos(this.cube, JointId.HandTipRight, frame);
                    //}
                    //Debug.Log(get_dis(HTRjoint, HTLjoint));

                    _prevPosition = GetVector3(JointId.HandLeft, frame);
                    _prevrPosition = GetrVector3(JointId.HandRight, frame);

                    prevPosition = new Vector3(_prevPosition.x, _prevPosition.y, _prevPosition.z);
                    prevrPosition = new Vector3(_prevrPosition.x, _prevrPosition.y, _prevrPosition.z);

                    preJoints = skeleton.GetJoint(JointId.HandLeft);
                    prerJoints = skeleton.GetJoint(JointId.HandRight);

                    preVelocity = GetVelocity();
                    prerVelocity = GetrVelocity();

                    //足
                    //prerfVelocity = GetrfVelocity();

                    _prevrfPosition = GetrfVector3(JointId.AnkleRight, frame);

                    prevrfPosition = new Vector3(_prevrfPosition.x, _prevrfPosition.y, _prevrfPosition.z);

                    prerfJoints = skeleton.GetJoint(JointId.AnkleRight);


                    prerfVelocity = GetrfVelocity();


                    //_prevrfPosition = GetVector3(JointId.AnkleLeft, frame);
                    _prevlfPosition = GetlfVector3(JointId.AnkleRight, frame);

                    //prevrfPosition = new Vector3(_prevrfPosition.x, _prevrfPosition.y, _prevrfPosition.z);
                    prevlfPosition = new Vector3(_prevlfPosition.x, _prevlfPosition.y, _prevlfPosition.z);

                    //prerfJoints = skeleton.GetJoint(JointId.AnkleRight);
                    prelfJoints = skeleton.GetJoint(JointId.AnkleLeft);

                    //prerfVelocity = GetVelocity();
                    prelfVelocity = GetlfVelocity();

                    //}
                    currentframe++;
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
    //float GetAccerareta(int jointId)
    //{
    //    return (GetVelocity(jointId) - preVelocity[jointId])/Time.deltaTime;
    //}
    //float GetVelocity(int jointId)
    //{
    //    var position = nowJoints[jointId].Position - preJoints[jointId].Position;
    //    var velocity = position.LengthSquared()/ Time.deltaTime;
    //    return velocity;
    //}
    //加速度


    Vector3 GetAccerareta()
    {
        return ((GetVelocity() - preVelocity) / 30);

    }
    Vector3 GetrAccerareta()
    {
        return ((GetrVelocity() - prerVelocity) / 30);

    }
    Vector3 GetrfAccerareta()
    {
        return ((GetrfVelocity() - prerfVelocity) / 30);

    }
    Vector3 GetlfAccerareta()
    {
        return ((GetlfVelocity() - prelfVelocity) / 30);

    }
    //速度
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
    Vector3 GetrfVelocity()
    {
        var position = nowrfJoints.Position - prerfJoints.Position;
        var velocity = GetrfVector3(position) / 30;
        return velocity;
    }
    Vector3 GetlfVelocity()
    {
        var position = nowlfJoints.Position - prelfJoints.Position;
        var velocity = GetlfVector3(position) / 30;
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
    Vector3 GetrfVector3(System.Numerics.Vector3 vector3)
    {
        return new Vector3(vector3.X, vector3.Y, vector3.Z);
    }
    Vector3 GetlfVector3(System.Numerics.Vector3 vector3)
    {
        return new Vector3(vector3.X, vector3.Y, vector3.Z);
    }


    Vector3 GetVector3(JointId jointId, Frame frame)
    {
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }
    Vector3 GetrVector3(JointId jointId, Frame frame)
    {
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }
    Vector3 GetrfVector3(JointId jointId, Frame frame)
    {
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }
    Vector3 GetlfVector3(JointId jointId, Frame frame)
    {
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }
    private void SetMarkPos(GameObject effectPrefab, JointId jointId, Frame frame)
    {


        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        //var offset = 50;
        //var pos = new Vector3(joint.Position.X / -offset, -(joint.Position.Y/offset) ,joint.Position.Z /offset);
        //effectPrefab.transform.position = new Vector3 (nowJoints[(int)jointId].Position.X, nowJoints[(int)jointId].Position.Y, nowJoints[(int)jointId].Position.Z) - Vector3.one * 50;
        effectPrefab.transform.localPosition = new Vector3(-joint.Position.X, -joint.Position.Y, joint.Position.Z) / 50;

    }
    private void SetrMarkPos(GameObject effectPrefab, JointId jointId, Frame frame)
    {
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        effectPrefab.transform.localPosition = new Vector3(-joint.Position.X, -joint.Position.Y, joint.Position.Z) / 50;

    }
    private void OnDestroy()
    {
        kinect.StopCameras();
    }

}
