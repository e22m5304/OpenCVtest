using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;
using System;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;



public class particle : MonoBehaviour
{
    Device kinect;
    Texture2D kinectColorTexture;
    [SerializeField]
    UnityEngine.UI.RawImage rawColorImg;
    
    Tracker tracker;
    public Dictionary<JointId, JointId> parentJointMap;

    void Awake()
    {
        parentJointMap = new Dictionary<JointId, JointId>();

        // pelvis has no parent so set to count
        parentJointMap[JointId.Pelvis] = JointId.Count;
        parentJointMap[JointId.SpineNavel] = JointId.Pelvis;
        parentJointMap[JointId.SpineChest] = JointId.SpineNavel;
        parentJointMap[JointId.Neck] = JointId.SpineChest;
        parentJointMap[JointId.ClavicleLeft] = JointId.SpineChest;
        parentJointMap[JointId.ShoulderLeft] = JointId.ClavicleLeft;
        parentJointMap[JointId.ElbowLeft] = JointId.ShoulderLeft;
        parentJointMap[JointId.WristLeft] = JointId.ElbowLeft;
        parentJointMap[JointId.HandLeft] = JointId.WristLeft;
        parentJointMap[JointId.HandTipLeft] = JointId.HandLeft;
        parentJointMap[JointId.ThumbLeft] = JointId.HandLeft;
        parentJointMap[JointId.ClavicleRight] = JointId.SpineChest;
        parentJointMap[JointId.ShoulderRight] = JointId.ClavicleRight;
        parentJointMap[JointId.ElbowRight] = JointId.ShoulderRight;
        parentJointMap[JointId.WristRight] = JointId.ElbowRight;
        parentJointMap[JointId.HandRight] = JointId.WristRight;
        parentJointMap[JointId.HandTipRight] = JointId.HandRight;
        parentJointMap[JointId.ThumbRight] = JointId.HandRight;
        parentJointMap[JointId.HipLeft] = JointId.SpineNavel;
        parentJointMap[JointId.KneeLeft] = JointId.HipLeft;
        parentJointMap[JointId.AnkleLeft] = JointId.KneeLeft;
        parentJointMap[JointId.FootLeft] = JointId.AnkleLeft;
        parentJointMap[JointId.HipRight] = JointId.SpineNavel;
        parentJointMap[JointId.KneeRight] = JointId.HipRight;
        parentJointMap[JointId.AnkleRight] = JointId.KneeRight;
        parentJointMap[JointId.FootRight] = JointId.AnkleRight;
        parentJointMap[JointId.Head] = JointId.Pelvis;
        parentJointMap[JointId.Nose] = JointId.Head;
        parentJointMap[JointId.EyeLeft] = JointId.Head;
        parentJointMap[JointId.EarLeft] = JointId.Head;
        parentJointMap[JointId.EyeRight] = JointId.Head;
        parentJointMap[JointId.EarRight] = JointId.Head;
        print(JointId.EarLeft);

    }

    [SerializeField]
    GameObject right;  // �E��ɏo��


    private void Start()
    {
        InitKinect();
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
                // BodyTraking�����擾���ĉ΂̋ʃI�u�W�F�N�g��Position�Ɏw�肵�܂�
                tracker.EnqueueCapture(capture);
                var frame = tracker.PopResult();
                if (frame.NumberOfBodies > 0) // ���̔��肪�Ȃ��ƎB�e�͈͂���l�����Ȃ��Ȃ����ꍇ�Ɍ�̏����Ŏ~�܂�
                {
                    this.SetMarkPos(this.right, JointId.HandRight, frame); // �E��
                    // ���w�肷��JointId��ς���΍��⓪�ɉ΂̋ʂ��o�����Ƃ��\
                }

            }
        }
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

    private void SetMarkPos(GameObject right, JointId jointId, Frame frame)
    {
        // �w��̊֐ߏ�񂩂�ʒu���擾����GameObject�̈ʒu�w���
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        var offset = 50; // �擾�����ʂ�̒l���Ɠ���������̂ŏ����������Ă��܂�
        var pos = new Vector3(joint.Position.X / -offset, joint.Position.Y / -offset, joint.Position.Z / offset);
        right.transform.localPosition = pos;
    }
    private void OnDestroy()
    {
        kinect.StopCameras();
    }

}
