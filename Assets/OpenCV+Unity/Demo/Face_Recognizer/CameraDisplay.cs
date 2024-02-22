using UnityEngine;
using UnityEngine.UI;

public class CameraDisplay : MonoBehaviour
{
    public RawImage rawImage;
    private WebCamTexture webCamTexture;

    void Start()
    {
        // Webカメラの映像を取得
        webCamTexture = new WebCamTexture();
        webCamTexture.Play();

        // RawImageにWebカメラの映像を設定
        rawImage.texture = webCamTexture;
    }
}
