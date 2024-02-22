using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp.Demo;
using OpenCvSharp;
using OpenCvSharp.Aruco;

public class Test : MonoBehaviour
{
    public Texture2D texture;

    void Start()
    {
        // ‰æ‘œ“Ç‚İ‚İ
        Mat mat = OpenCvSharp.Unity.TextureToMat(this.texture);

        // ‰æ‘œ‘‚«o‚µ
        Texture2D outTexture = new Texture2D(mat.Width, mat.Height, TextureFormat.ARGB32, false);
        OpenCvSharp.Unity.MatToTexture(mat, outTexture);

        // •\¦
        GetComponent<RawImage>().texture = outTexture;
    }
}