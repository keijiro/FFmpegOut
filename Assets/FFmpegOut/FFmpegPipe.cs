using System.Diagnostics;
using System.IO;

namespace FFmpegOut
{
    // A stream pipe class that invokes ffmpeg and connect to it.
    class FFmpegPipe
    {
        Process _subprocess;
        BinaryWriter _stdin;

        public string Error { get; private set; }

        public FFmpegPipe(string name, int width, int height, int framerate)
        {
            var outPath = name.Replace(" ", "_") + ".mov";

            var opt = "-y -f rawvideo -vcodec rawvideo -pixel_format rgb24";
            opt += " -video_size " + width + "x" + height;
            opt += " -framerate " + framerate;
            opt += " -i - -c:v prores_ks -pix_fmt yuv422p10le ";
            opt += outPath;

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
    }
}
