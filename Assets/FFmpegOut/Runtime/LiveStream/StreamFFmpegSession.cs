namespace FFmpegOut.LiveStream
{
    /// <summary>
    ///     This FFmpegSession pipes whatever is on stdin to FFmpeg.
    /// </summary>
    public sealed class StreamFFmpegSession : FFmpegSession
    {
        private const string UNITY_CAM_TEX_BYTE_FORMAT =
            "-pixel_format rgba -colorspace bt709 -f rawvideo -vcodec rawvideo";

        private const string FFMPEG_LOGLEVEL = "-loglevel warning";

        private StreamFFmpegSession(string arguments) : base(arguments) { }

        public static StreamFFmpegSession Create(
            int width, int height, float frameRate,
            FFmpegPreset encodingPreset, StreamPreset streamPreset,
            string address)
        {
            // pipe:0 corresponds to stdin
            string ffmpegArguments =
                $"{UNITY_CAM_TEX_BYTE_FORMAT} {FFMPEG_LOGLEVEL} -framerate {frameRate} -video_size {width}x{height} "
                + $"-re -i pipe:0 {encodingPreset.GetOptions()} "
                + $"{streamPreset.GetOptions()} {address}";

            return new StreamFFmpegSession(ffmpegArguments);
        }
    }
}
