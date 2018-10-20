// FFmpegOut - FFmpeg video encoding plugin for Unity
// https://github.com/keijiro/KlakNDI

namespace FFmpegOut
{
    public enum FFmpegPreset
    {
        H264Default,
        H264Lossless420,
        H264Lossless444,
        ProRes422,
        ProRes4444,
        VP8Default
    }

    static public class FFmpegPresetExtensions
    {
        public static string GetSuffix(this FFmpegPreset preset)
        {
            switch (preset)
            {
                case FFmpegPreset.H264Default:
                case FFmpegPreset.H264Lossless420:
                case FFmpegPreset.H264Lossless444: return ".mp4";
                case FFmpegPreset.ProRes422:
                case FFmpegPreset.ProRes4444:      return ".mov";
                case FFmpegPreset.VP8Default:      return ".webm";
            }
            return null;
        }

        public static string GetOptions(this FFmpegPreset preset)
        {
            switch (preset)
            {
                case FFmpegPreset.H264Default:     return "-pix_fmt yuv420p";
                case FFmpegPreset.H264Lossless420: return "-pix_fmt yuv420p -preset ultrafast -crf 0";
                case FFmpegPreset.H264Lossless444: return "-pix_fmt yuv444p -preset ultrafast -crf 0";
                case FFmpegPreset.ProRes422:       return "-c:v prores_ks -pix_fmt yuv422p10le";
                case FFmpegPreset.ProRes4444:      return "-c:v prores_ks -pix_fmt yuva444p10le";
                case FFmpegPreset.VP8Default:      return "-c:v libvpx -pix_fmt yuv420p";
            }
            return null;
        }

    }
}
