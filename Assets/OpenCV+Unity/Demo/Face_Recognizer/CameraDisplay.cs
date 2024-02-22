using UnityEngine;
using UnityEngine.UI;

public class CameraDisplay : MonoBehaviour
{
    public RawImage rawImage;
    private WebCamTexture webCamTexture;

    void Start()
    {
        // Web�J�����̉f�����擾
        webCamTexture = new WebCamTexture();
        webCamTexture.Play();

        // RawImage��Web�J�����̉f����ݒ�
        rawImage.texture = webCamTexture;
    }
}
