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
                    //Inspector: [udp adress] / udp://192.168.0.1:10755
                    return "-f mpegts";
                case StreamPreset.Rtp:
                    //Inspector: [rtp adress] / rtp://192.168.0.1:10755
                    return "-f rtp_mpegts";
                case StreamPreset.Rtsp:
                    //Inspector: [rtsp adress] / rtsp://192.168.0.1:10755
                    return "-f rtsp";
                case StreamPreset.Hls:
                    //Inspector: [http server base url] [htdocs directory path + m3u8 file name extension] / http://192.168.0.1:8000/ D:\Repo\UnityCameraStream\miniweb\htdocs\stream.m3u8
                    return "-f hls -hls_flags delete_segments -hls_init_time 0.5 -hls_time 0.5 -hls_list_size 10 -hls_allow_cache 1 -hls_base_url";
                case StreamPreset.Hls_ssegment:
                    //Inspector: [http server base url] -segment_list [htdocs directory path + m3u8 file name extension] [htdocs directory path + ts file name extension] / http://192.168.0.185:8000/ -segment_list D:\Repo\UnityCameraStream\miniweb\htdocs\stream.m3u8 D:\Repo\UnityCameraStream\miniweb\htdocs\out%03d.ts
                    return "-f segment -segment_list_type m3u8 -segment_list_size 10 -segment_list_flags +live -segment_time 1 -segment_wrap 10 -segment_list_entry_prefix";
                case StreamPreset.Rtmp:
                    //Inspector: [rtmp adress] / rtmp://192.168.0.1:1935/rtmp_stream/mystream
                    return "-f flv";
            }

            return null;
        }
    }
}
