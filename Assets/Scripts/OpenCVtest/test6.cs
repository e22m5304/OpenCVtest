using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;
using System;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;

public class test6 : MonoBehaviour
{
    Device kinect;
    Texture2D kinectColorTexture;
    [SerializeField] UnityEngine.UI.RawImage rawColorImg;
    Tracker tracker;

    [SerializeField] GameObject rightPrefab;  // �E���\���I�u�W�F�N�g�̃v���n�u

    private List<Dictionary<JointId, GameObject>> bodyObjectsList = new List<Dictionary<JointId, GameObject>>();

    private async void Start()
    {
        InitKinect();
        await KinectLoop();
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
        int height = kinect.GetCalibration().ColorCameraCalibration.ResolutionHeight;
        kinectColorTexture = new Texture2D(width, height);
        tracker = Tracker.Create(kinect.GetCalibration(), TrackerConfiguration.Default);
    }

    private async Task KinectLoop()
    {
        while (true)
        {
            using (Capture capture = await Task.Run(() => kinect.GetCapture()).ConfigureAwait(true))
            {
                tracker.EnqueueCapture(capture);
                var frame = tracker.PopResult();
                if (frame.NumberOfBodies > 0)
                {
                    // ��l���̍��i��������
                    for (int i = 0; i < frame.NumberOfBodies; i++)
                    {
                        var skeleton = frame.GetBodySkeleton((uint)i);
                        //var joints = skeleton.GetJoint();

                        // �e�֐߂̈ʒu�ɃI�u�W�F�N�g��z�u
                        //SetObjectPositions(joints, i);
                    }
                }
            }
        }
    }

    private void SetObjectPositions(Dictionary<JointId, Joint> joints, int index)
    {
        // ���������݂��Ȃ���ΐV�K�쐬
        if (bodyObjectsList.Count <= index)
        {
            bodyObjectsList.Add(new Dictionary<JointId, GameObject>());
        }

        foreach (var jointId in joints.Keys)
        {
            // �e�֐߂̈ʒu�ɃI�u�W�F�N�g��z�u
            SetObjectPosition(jointId, joints[jointId], index);
        }
    }

    private void SetObjectPosition(JointId jointId, Joint joint, int index)
    {
        // �v���n�u����V�����I�u�W�F�N�g�𐶐�
        GameObject obj = GetOrCreateObject(index, jointId, rightPrefab);

        // ���i�̈ʒu�ɃI�u�W�F�N�g��z�u
        obj.transform.localPosition = new Vector3(-joint.Position.X, -joint.Position.Y, joint.Position.Z) / 50;
    }

    private GameObject GetOrCreateObject(int index, JointId jointId, GameObject prefab)
    {
        // �C���f�b�N�X�ɑΉ����鎫�������݂��Ȃ���ΐV�K�쐬
        if (!bodyObjectsList[index].ContainsKey(jointId))
        {
            GameObject newObj = Instantiate(prefab, transform);
            bodyObjectsList[index].Add(jointId, newObj);
        }

        // �C���f�b�N�X�Ɗ֐߂ɑΉ�����I�u�W�F�N�g��Ԃ�
        return bodyObjectsList[index][jointId];
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

    private void OnDestroy()
    {
        if (kinect != null)
        {
            kinect.StopCameras();
            kinect.Dispose(); // �����ǉ�
        }
    }
}
