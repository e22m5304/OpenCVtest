using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;
using UnityEngine;

public class KinectHandler : MonoBehaviour
{
    private Device kinect;
    private Transformation transformation;
    private Capture cameraCapture;

    private Texture2D texture;

    private Tracker tracker;

    [SerializeField]
    GameObject fireBall1; // �E��ɏo���΂̋�

  

    void Start()
    {
        // Azure Kinect�̏����ݒ肨��ыN��
        this.kinect = Device.Open(0);
        this.kinect.StartCameras(new DeviceConfiguration
        {
            ColorFormat = ImageFormat.ColorBGRA32,
            ColorResolution = ColorResolution.R720p,
            DepthMode = DepthMode.NFOV_2x2Binned,
            SynchronizedImagesOnly = true,
            CameraFPS = FPS.FPS30
        });

        this.transformation = kinect.GetCalibration().CreateTransformation();
        this.cameraCapture = kinect.GetCapture();

        // �J�����f����`�悷��Texture2D�̏����ݒ�
        // �J��������̏������ɃT�C�Y�����肷��
        var width = kinect.GetCalibration().ColorCameraCalibration.ResolutionWidth;
        var height = kinect.GetCalibration().ColorCameraCalibration.ResolutionHeight;
        this.transform.localScale = new Vector3(width / 100, height / 100, 0.1f);

        this.texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        this.texture.Apply();
        this.GetComponent<Renderer>().material.mainTexture = texture;

        // BodyTracking���邽�߂̃g���b�J�[��������
        this.tracker = Tracker.Create(kinect.GetCalibration(), TrackerConfiguration.Default);

        // Kinect����̏��擾�̓^�X�N�ŉ񂵂ČJ��Ԃ��擾���܂�
        Task t = KinectLoop();
    }

    private void OnDestroy()
    {
        // �A�v���I�����ɃJ�������I��������
        this.kinect.StopCameras();
    }

    private async Task KinectLoop()
    {
        while (true)
        {
            using (Capture capture = await Task.Run(() => this.kinect.GetCapture()).ConfigureAwait(true))
            {
                // �摜����
                // �擾�����s�N�Z�����̐F����Texture2D�ɏ�������
                var colorImage = capture.Color;
                var colorArray = colorImage.GetPixels<BGRA>().ToArray();
                var colors = new Color32[colorArray.Length];

                for (var i = 0; i < colorArray.Length; i++)
                {
                    colors[i].b = colorArray[i].B;
                    colors[i].g = colorArray[i].G;
                    colors[i].r = colorArray[i].R;
                    colors[i].a = 255;
                }

                this.texture.SetPixels32(colors);
                this.texture.Apply();

                // BodyTraking�����擾���ĉ΂̋ʃI�u�W�F�N�g��Position�Ɏw�肵�܂�
                this.tracker.EnqueueCapture(capture);
                var frame = tracker.PopResult();
                if (frame.NumberOfBodies > 0) // ���̔��肪�Ȃ��ƎB�e�͈͂���l�����Ȃ��Ȃ����ꍇ�Ɍ�̏����Ŏ~�܂�
                {
                    this.SetMarkPos(this.fireBall1, JointId.HandRight, frame); // �E��
                    // ���w�肷��JointId��ς���΍��⓪�ɉ΂̋ʂ��o�����Ƃ��\
                }
            }
        }
    }

    private void SetMarkPos(GameObject fireball, JointId jointId, Frame frame)
    {
        // �w��̊֐ߏ�񂩂�ʒu���擾����GameObject�̈ʒu�w���
        var joint = frame.GetBodySkeleton(0).GetJoint(jointId);
        var offset = 50; // �擾�����ʂ�̒l���Ɠ���������̂ŏ����������Ă��܂�
        var pos = new Vector3(joint.Position.X / -offset, joint.Position.Y / -offset, joint.Position.Z / offset);
        fireball.transform.localPosition = pos;
    }
}
