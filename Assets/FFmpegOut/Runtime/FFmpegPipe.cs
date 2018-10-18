using System.Diagnostics;
using System.IO;
using System;

namespace FFmpegOut
{
    // A stream pipe class that invokes ffmpeg and connect to it.
    public sealed class FFmpegPipe
    {
        #region Public properties

        public enum Preset {
            H264Default,
            H264Lossless420,
            H264Lossless444,
            ProRes422,
            ProRes4444,
            VP8Default
        }

        public string Filename { get; private set; }
        public string Error { get; private set; }

        #endregion

        #region Public methods

        public FFmpegPipe(string name, int width, int height, int framerate, Preset preset)
        {
            name += DateTime.Now.ToString(" yyyy MMdd HHmmss");
            Filename = name.Replace(" ", "_") + GetSuffix(preset);

            var opt = "-y -f rawvideo -vcodec rawvideo -pixel_format rgba";
            opt += " -colorspace bt709";
            opt += " -video_size " + width + "x" + height;
            opt += " -framerate " + framerate;
            opt += " -loglevel warning -i - " + GetOptions(preset);
            opt += " " + Filename;

            var info = new ProcessStartInfo(FFmpegConfig.BinaryPath, opt);
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;

            _subprocess = Process.Start(info);
        }

        public System.Threading.Tasks.Task WriteAsync(Unity.Collections.NativeArray<byte> data)
        {
            return System.Threading.Tasks.Task.Run(() => {
                if (_buffer == null || _buffer.Length != data.Length)
                    _buffer = new byte[data.Length];
                data.CopyTo(_buffer);
                _subprocess.StandardInput.BaseStream.Flush();
                _subprocess.StandardInput.BaseStream.Write(_buffer, 0, _buffer.Length);
            });
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
            _buffer = null;
        }

        #endregion

        #region Private members

        Process _subprocess;
        byte[] _buffer;

        static string [] _suffixes = {
            ".mp4",
            ".mp4",
            ".mp4",
            ".mov",
            ".mov",
            ".webm"
        };

        static string [] _options = {
            "-pix_fmt yuv420p",
            "-pix_fmt yuv420p -preset ultrafast -crf 0",
            "-pix_fmt yuv444p -preset ultrafast -crf 0",
            "-c:v prores_ks -pix_fmt yuv422p10le",
            "-c:v prores_ks -pix_fmt yuva444p10le",
            "-c:v libvpx -pix_fmt yuv420p"
        };

        static string GetSuffix(Preset preset)
        {
            return _suffixes[(int)preset];
        }

        static string GetOptions(Preset preset)
        {
            return _options[(int)preset];
        }

        #endregion
    }
}
