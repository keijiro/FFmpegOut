namespace FFmpegOut.LiveStream
{
    public enum StreamPreset
    {
        Udp,
        Rtp,
        Rtsp,
        Hls,
        Hls_ssegment,
        Rtmp
    }

    public static class StreamPresetExtensions
    {
        public static string GetOptions(this StreamPreset preset)
        {
            switch (preset)
            {
                case StreamPreset.Udp:
                    return "-f mpegts";
                case StreamPreset.Rtp:
                    return "-f rtp_mpegts";
                case StreamPreset.Rtsp:
                    return "-f rtsp";
                case StreamPreset.Hls:
                    return "-f hls -hls_flags delete_segments -hls_init_time 0.5 -hls_time 0.5 -hls_list_size 10 -hls_allow_cache 1 -hls_base_url";
                case StreamPreset.Hls_ssegment:
                    return "-f segment -segment_list_type m3u8 -segment_list_size 10 -segment_list_flags +live -segment_time 1 -segment_wrap 10 -segment_list_entry_prefix";
                case StreamPreset.Rtmp:
                    return "-f flv";
            }

            return null;
        }
    }
}
