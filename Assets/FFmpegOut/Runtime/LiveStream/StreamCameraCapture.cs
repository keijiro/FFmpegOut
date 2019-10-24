using UnityEngine;

namespace FFmpegOut.LiveStream
{
    public class StreamCameraCapture : CameraCapture
    {
        [SerializeField] protected StreamPreset _streamPreset;
        [SerializeField] protected string _streamAddress;

        protected override FFmpegSession GetSession(int texWidth, int texHeight)
        {
            return StreamFFmpegSession.Create(
                texWidth,
                texHeight,
                frameRate,
                preset,
                _streamPreset,
                _streamAddress);
        }
    }
}
