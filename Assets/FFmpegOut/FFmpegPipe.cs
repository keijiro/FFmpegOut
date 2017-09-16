using System.Diagnostics;
using System.IO;
using System;

namespace FFmpegOut
{
    // A stream pipe class that invokes ffmpeg and connect to it.
    class FFmpegPipe
    {
        #region Public properties

        public enum Codec { ProRes, H264, VP8, H264Lossless444, H264Lossless420, H264Draft, H264Trasparent, PNGSequence }

        public string Filename { get; private set; }
        public string Error { get; private set; }

        #endregion

        #region Public methods

        public FFmpegPipe(string name, int width, int height, int framerate, Codec codec)
        {
            name += DateTime.Now.ToString(" yyyy MMdd HHmmss");
            Filename = name.Replace(" ", "_") + GetSuffix(codec);

            var opt = "-y -f rawvideo -vcodec rawvideo -pixel_format rgb24";
            opt += " -video_size " + width + "x" + height;
            opt += " -framerate " + framerate;
            opt += " -loglevel warning -i - " + GetOptions(codec);
            opt += " " + Filename;

            var info = new ProcessStartInfo(FFmpegConfig.BinaryPath, opt);
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;

            _subprocess = Process.Start(info);
            _stdin = new BinaryWriter(_subprocess.StandardInput.BaseStream);
        }

        public void Write(byte[] data)
        {
            if (_subprocess == null) return;

            _stdin.Write(data);
            _stdin.Flush();
        }

        public void Close()
        {
            if (_subprocess == null) return;

            _subprocess.StandardInput.Close();
            _subprocess.WaitForExit();

            var outputReader = _subprocess.StandardError;
            Error = outputReader.ReadToEnd();

            _subprocess.Close();
            _subprocess.Dispose();

            outputReader.Close();
            outputReader.Dispose();

            _subprocess = null;
            _stdin = null;
        }

        #endregion

        #region Private members

        Process _subprocess;
        BinaryWriter _stdin;

        static string [] _suffixes = {
            ".mov",
            ".mp4",
            ".webm",
            ".mp4",
            ".mp4",
            ".mp4",
            ".mp4",
            "_%010d.png"
        };

        static string [] _options = {
            "-c:v prores_ks -pix_fmt yuv422p10le",
            "-pix_fmt yuv420p",
            "-c:v libvpx",
            "-c:v libx264 -preset veryslow -qp 0 -tune film -x264opts ref=9:aq-mode=3:bframes=9 -pix_fmt yuv444p",
            "-c:v libx264 -preset veryslow -qp 0 -tune film -x264opts ref=9:aq-mode=3:bframes=9 -pix_fmt yuv420p",
            "-c:v libx264 -preset fast -crf 21 -tune film -x264opts keyint=60 -pix_fmt yuv420p",
            "-c:v libx264 -preset veryslow -crf 16 -tune film -x264opts ref=9:aq-mode=3:bframes=9 -pix_fmt yuv420p",
            ""
        };

        static string GetSuffix(Codec codec)
        {
            return _suffixes[(int)codec];
        }

        static string GetOptions(Codec codec)
        {
            return _options[(int)codec];
        }

        #endregion
    }
}
