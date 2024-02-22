using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

namespace IMALAB.OC.LE3D.Tutorial
{
    public class CaptureData
    {
        public int ColorImageWidth { get; set; }
        public int ColorImageHeight { get; set; }
        public byte[] RawColorImage { get; set; }
        public BGRA[] ColorImage { get; set; }
        public Color32[] Color32Image { get; set; }
        
        public int DepthImageWidth { get; set; }
        public int DepthImageHeight { get; set; }
        public ushort[] DepthImage { get; set; }
    }
}
