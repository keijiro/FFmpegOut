using UnityEngine;

namespace FFmpegOut
{
    static class FFmpegConfig
    {
        public static string BinaryPath
        {
            get {
                var basePath = Application.streamingAssetsPath + "/FFmpegOut";
                
                if (Application.platform == RuntimePlatform.OSXPlayer ||
                    Application.platform == RuntimePlatform.OSXEditor)
                    return basePath + "/OSX/ffmpeg";

                if (Application.platform == RuntimePlatform.LinuxPlayer ||
                    Application.platform == RuntimePlatform.LinuxEditor)
                    return basePath + "/Linux/ffmpeg";

                return basePath + "/Windows/ffmpeg.exe";
            }
        }

        public static bool CheckAvailable
        {
            get { return System.IO.File.Exists(BinaryPath); }
        }
    }
}
