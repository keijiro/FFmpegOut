using UnityEngine;

namespace FFmpegOut
{
    static class FFmpegConfig
    {
        public static string BinaryPath {
            get {
                return Application.streamingAssetsPath + "/FFmpegOut/ffmpeg.exe";
            }
        }

        public static bool CheckAvailable {
            get { return System.IO.File.Exists(BinaryPath); }
        }
    }
}
