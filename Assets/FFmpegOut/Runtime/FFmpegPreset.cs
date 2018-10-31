// FFmpegOut - FFmpeg video encoding plugin for Unity
// https://github.com/keijiro/KlakNDI

namespace FFmpegOut
{
    public enum FFmpegPreset
    {
        H264Default,
        H264Nvidia,
        H264Lossless420,
        H264Lossless444,
        HevcDefault,
        HevcNvidia,
        ProRes422,
        ProRes4444,
        VP8Default,
        VP9Default,
        Hap,
        HapAlpha,
        HapQ
    }

    static public class FFmpegPresetExtensions
    {
        public static string GetDisplayName(this FFmpegPreset preset)
        {
            switch (preset)
            {
                case FFmpegPreset.H264Default:     return "H.264 Default (MP4)";
                case FFmpegPreset.H264Nvidia:      return "H.264 NVIDIA (MP4)";
                case FFmpegPreset.H264Lossless420: return "H.264 Lossless 420 (MP4)";
                case FFmpegPreset.H264Lossless444: return "H.264 Lossless 444 (MP4)";
                case FFmpegPreset.HevcDefault:     return "HEVC Default (MP4)";
                case FFmpegPreset.HevcNvidia:      return "HEVC NVIDIA (MP4)";
                case FFmpegPreset.ProRes422:       return "ProRes 422 (QuickTime)";
                case FFmpegPreset.ProRes4444:      return "ProRes 4444 (QuickTime)";
                case FFmpegPreset.VP8Default:      return "VP8 (WebM)";
                case FFmpegPreset.VP9Default:      return "VP9 (WebM)";
                case FFmpegPreset.Hap:             return "HAP (QuickTime)";
                case FFmpegPreset.HapAlpha:        return "HAP Alpha (QuickTime)";
                case FFmpegPreset.HapQ:            return "HAP Q (QuickTime)";
            }
            return null;
        }

        public static string GetSuffix(this FFmpegPreset preset)
        {
            switch (preset)
            {
                case FFmpegPreset.H264Default:
                case FFmpegPreset.H264Nvidia:
                case FFmpegPreset.H264Lossless420:
                case FFmpegPreset.H264Lossless444:
                case FFmpegPreset.HevcDefault:
                case FFmpegPreset.HevcNvidia:      return ".mp4";
                case FFmpegPreset.ProRes422:
                case FFmpegPreset.ProRes4444:      return ".mov";
                case FFmpegPreset.VP9Default:
                case FFmpegPreset.VP8Default:      return ".webm";
                case FFmpegPreset.Hap:
                case FFmpegPreset.HapQ:
                case FFmpegPreset.HapAlpha:        return ".mov";
            }
            return null;
        }

        public static string GetOptions(this FFmpegPreset preset)
        {
            switch (preset)
            {
                case FFmpegPreset.H264Default:     return "-pix_fmt yuv420p";
                case FFmpegPreset.H264Nvidia:      return "-c:v h264_nvenc -pix_fmt yuv420p";
                case FFmpegPreset.H264Lossless420: return "-pix_fmt yuv420p -preset ultrafast -crf 0";
                case FFmpegPreset.H264Lossless444: return "-pix_fmt yuv444p -preset ultrafast -crf 0";
                case FFmpegPreset.HevcDefault:     return "-c:v libx265 -pix_fmt yuv420p";
                case FFmpegPreset.HevcNvidia:      return "-c:v hevc_nvenc -pix_fmt yuv420p";
                case FFmpegPreset.ProRes422:       return "-c:v prores_ks -pix_fmt yuv422p10le";
                case FFmpegPreset.ProRes4444:      return "-c:v prores_ks -pix_fmt yuva444p10le";
                case FFmpegPreset.VP8Default:      return "-c:v libvpx -pix_fmt yuv420p";
                case FFmpegPreset.VP9Default:      return "-c:v libvpx-vp9";
                case FFmpegPreset.Hap:             return "-c:v hap";
                case FFmpegPreset.HapAlpha:        return "-c:v hap -format hap_alpha";
                case FFmpegPreset.HapQ:            return "-c:v hap -format hap_q";
            }
            return null;
        }
    }
}
