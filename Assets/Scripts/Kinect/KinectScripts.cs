using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//(�ǉ�1)AzureKinectSDK�̓ǂݍ���
using Microsoft.Azure.Kinect.Sensor;
public class KinectScript : MonoBehaviour
{
    //(�ǉ�2)Kinect�������ϐ�
    Device kinect;

    void Start()
    {
        //(�ǉ�5)�ŏ��̈�񂾂�Kinect���������\�b�h���Ăяo��
        InitKinect();
    }
    //(�ǉ�3)Kinect�̏�����(Form1�R���X�g���N�^����Ăяo��)
    private void InitKinect()
    {

        //(�ǉ�4)0�Ԗڂ�Kinect�Ɛڑ������̂���Kinect�̊e�탂�[�h��ݒ肵�ē���J�n
        kinect = Device.Open(0);
        kinect.StartCameras(new DeviceConfiguration
        {
            ColorFormat = ImageFormat.ColorBGRA32,
            ColorResolution = ColorResolution.R720p,
            DepthMode = DepthMode.NFOV_2x2Binned,
            SynchronizedImagesOnly = true,
            CameraFPS = FPS.FPS30
        });
    }
    //(�ǉ�6)���̃I�u�W�F�N�g��������(�A�v���I��)�Ɠ�����Kinect���~
    private void OnDestroy()
    {
        kinect.StopCameras();
    }

    void Update()
    {

    }
}