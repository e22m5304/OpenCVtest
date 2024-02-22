namespace IMALAB.OC.LE3D.Tutorial
{
    public class CaptureDataProvider
    {
        public bool IsRunning { get; set; } = false;

        private readonly object _lockObject = new();
        private CaptureData _frameCaptureData = new();
        private bool _latest = false;

        public void SetCurrentFrameData(ref CaptureData currentFrameData)
        {
            lock (_lockObject)
            {
                (currentFrameData, _frameCaptureData) = (_frameCaptureData, currentFrameData);
                _latest = true;
            }
        }

        public bool GetCurrentFrameData(ref CaptureData dataBuffer)
        {
            lock (_lockObject)
            {
                (dataBuffer, _frameCaptureData) = (_frameCaptureData, dataBuffer);
                bool result = _latest;
                _latest = false;
                return result;
            }
        }
    }
}
