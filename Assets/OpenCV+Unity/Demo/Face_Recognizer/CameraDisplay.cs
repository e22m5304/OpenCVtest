using UnityEngine;
using UnityEngine.UI;

public class CameraDisplay : MonoBehaviour
{
    public RawImage rawImage;
    private WebCamTexture webCamTexture;

    void Start()
    {
        // WebƒJƒƒ‰‚Ì‰f‘œ‚ğæ“¾
        webCamTexture = new WebCamTexture();
        webCamTexture.Play();

        // RawImage‚ÉWebƒJƒƒ‰‚Ì‰f‘œ‚ğİ’è
        rawImage.texture = webCamTexture;
    }
}
