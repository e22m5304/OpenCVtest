using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Kinect.Sensor;
using FaceRecognitionSystem;
using UnityEngine;
using UnityEngine.UI;

namespace IMALAB.OC.LE3D.Tutorial
{
    public class RetrieveImages : MonoBehaviour, IImageProvider
    {
        [SerializeField] private RawImage colorRawImage;
        
        private Device _device;

        private CaptureData _lastFrameData = new();
        private readonly CaptureDataProvider _captureDataProvider = new();
        
        private Color32[] _colors;
        private int colorWidth;
        private int colorHeight;
        private Texture2D _colorTexture;
        
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private CancellationToken Token => _cancellationTokenSource.Token;

        public ImageProviderReadyEvent Ready = new ImageProviderReadyEvent( );
        private bool _inited = false;

        public Color32 [ ] ImgData {
            get {
                
                return _colors;
            }
            set {

            }
        }

        public int Width {
            get {
                return colorWidth;
            }
            set {
            }
        }

        public int Height {
            get {
                return colorHeight;
            }
            set {
            }
        }

        private void Start()
        {
            if (Device.GetInstalledCount() == 0)
            {
                Debug.LogWarning("Device not connected.\n" +
                          "デバイスが接続されていません．");
                return;
            }

            try
            {
                _device = Device.Open();
                
                print($"Successfully opened the device. [S/N:{_device.SerialNum}]\n" +
                      $"デバイスを開くのに成功しました．［S/N:{_device.SerialNum}］");

                // Create device configuration.
                // デバイス設定を作成する．
                var deviceConfiguration = new DeviceConfiguration
                {
                    CameraFPS = FPS.FPS30,
                    ColorFormat = ImageFormat.ColorBGRA32,
                    ColorResolution = ColorResolution.R1080p,
                    DepthMode = DepthMode.NFOV_Unbinned
                };

                // Start the device with the created configuration.
                // 作成した設定でカメラを起動する．
                _device.StartCameras(deviceConfiguration);
                
                print($"Successfully Start the device with the created configuration. [S/N:{_device.SerialNum}]\n" +
                      $"作成した設定でデバイスを起動するのに成功しました．［S/N:{_device.SerialNum}］");
            }
            catch (AzureKinectOpenDeviceException e)
            {
                Debug.LogError($"Failed to open the device. [{e.Message}]\n" +
                               $"デバイスを開くのに失敗しました．[{e.Message}]");
            }
            catch (AzureKinectStartCamerasException e)
            {
                Debug.LogError($"Failed to start cameras. [{e.Message}]\n" +
                               $"カメラを起動するのに失敗しました．[{e.Message}]");
            }

            // 別スレッドで非同期にキャプチャを保存する．
            Task.Run(() => UpdateCaptureAsync(Token));
        }

        // Get stored data and update texture
        // 保管されたデータを取得し，テクスチャの更新をする．
        private void Update()
        {


            if (_captureDataProvider.IsRunning && _captureDataProvider.GetCurrentFrameData(ref _lastFrameData))
            {
                if ( !_inited ) {
                    Ready.Invoke( this );
                    _inited = true;
                }
                
                if (!_colorTexture)
                {
                    _colorTexture = new Texture2D(_lastFrameData.ColorImageWidth, _lastFrameData.ColorImageHeight, TextureFormat.RGBA32, false);
                    colorRawImage.texture = _colorTexture;
                }

                if (colorRawImage.rectTransform.sizeDelta.x < _lastFrameData.ColorImageWidth ||
                    colorRawImage.rectTransform.sizeDelta.y < _lastFrameData.ColorImageHeight)
                {
                    colorRawImage.rectTransform.sizeDelta = new Vector2(_lastFrameData.ColorImageWidth,
                        _lastFrameData.ColorImageHeight);
                }
                
                _colors ??= new Color32[_lastFrameData.Color32Image.Length];

                _colors = _lastFrameData.Color32Image;
                colorWidth = _lastFrameData.ColorImageWidth;
                colorHeight = _lastFrameData.ColorImageHeight;

                // Update texture
                // テクスチャを更新
                _colorTexture.SetPixels32(_colors);
                _colorTexture.Apply();
            }
        }

        // Get a capture and store the data.
        // キャプチャを取得し，その時点のデータを保管する．
        private void UpdateCaptureAsync(CancellationToken token)
        {
            // Since errors in a separate thread are not displayed in the Unity log,
            // try-catch is used to output an error log when an exception occurs.
            // 別スレッドのエラーはUnityのログに表示されないため，try-catch を使用して例外発生時にエラーログを出力する．
            try
            {
                // Variables for temporary storage
                // 一時保管用の変数
                var currentFrameData = new CaptureData();

                while (!token.IsCancellationRequested)
                {
                    // Get a capture from the device.
                    // デバイスからキャプチャを取得する．
                    using Capture capture = _device.GetCapture();
                    Debug.Log("Get a capture from the device.\n" +
                              "デバイスからキャプチャを取得しました．");

                    if (capture.Color == null)
                    {
                        print("Color is null");
                    }
                    else
                    {
                        currentFrameData.ColorImageWidth = capture.Color.WidthPixels;
                        currentFrameData.ColorImageHeight = capture.Color.HeightPixels;

                        currentFrameData.RawColorImage ??=
                            new byte[currentFrameData.ColorImageWidth * currentFrameData.ColorImageHeight * 4];
                        currentFrameData.ColorImage ??=
                            new BGRA[currentFrameData.ColorImageWidth * currentFrameData.ColorImageHeight];
                        currentFrameData.Color32Image ??=
                            new Color32[currentFrameData.ColorImageWidth * currentFrameData.ColorImageHeight];
                        var colorFrame = capture.Color.Memory.Span;
                        for (var i = 0;
                             i < currentFrameData.ColorImageWidth * currentFrameData.ColorImageHeight;
                             i++)
                        {
                            currentFrameData.RawColorImage[i*4] = colorFrame[i*4];
                            currentFrameData.RawColorImage[i*4+1] = colorFrame[i*4+1];
                            currentFrameData.RawColorImage[i*4+2] = colorFrame[i*4+2];
                            currentFrameData.RawColorImage[i*4+3] = colorFrame[i*4+3];

                            currentFrameData.ColorImage[i] = new BGRA(
                                colorFrame[i * 4],
                                colorFrame[i * 4 + 1],
                                colorFrame[i * 4 + 2],
                                255);

                            currentFrameData.Color32Image[i] = new Color32(
                                colorFrame[i * 4 + 2],
                                colorFrame[i * 4 + 1],
                                colorFrame[i * 4],
                                255);
                        }
                    }
                    
                    if (capture.Depth == null)
                    {
                        print("Depth is null");
                    }
                    else
                    {
                        currentFrameData.DepthImageWidth = capture.Depth.WidthPixels;
                        currentFrameData.DepthImageHeight = capture.Depth.HeightPixels;

                        currentFrameData.DepthImage ??=
                            new ushort[currentFrameData.DepthImageWidth * currentFrameData.DepthImageHeight];
                        var depthFrame = MemoryMarshal.Cast<byte, ushort>(capture.Depth.Memory.Span);
                        for (var i = 0;
                             i < currentFrameData.DepthImageWidth * currentFrameData.DepthImageHeight;
                             i++)
                        {
                            currentFrameData.DepthImage[i] = depthFrame[i];
                        }
                    }

                    // まとめたデータを
                    _captureDataProvider.SetCurrentFrameData(ref currentFrameData);
                    
                    _captureDataProvider.IsRunning = true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                token.ThrowIfCancellationRequested();
                _device.StopCameras();
            }
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _device?.Dispose();
        }
    }
}
