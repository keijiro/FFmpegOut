using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Unity.Collections;

namespace FFmpegOut
{
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
            _copyThread = new Thread(CopyThread);
            _pipeThread = new Thread(PipeThread);
            _copyThread.Start();
            _pipeThread.Start();
        }

        bool _pushed;

        public void PushFrameAsync(NativeArray<byte> data)
        {
            if (!_pushed)
            {
                _copyMutex.ReleaseMutex();
                _pushed = true;
            }

            lock (_copyQueue) _copyQueue.Enqueue(data);

            _copyStart.Set();
        }

        public void CompletePushFrames()
        {
            if (_pushed)
            {
                _copyMutex.WaitOne();
                _pushed = false;
            }
        }

        public void Close()
        {
            _terminate = true;

            _copyMutex.ReleaseMutex();
            _copyStart.Set();
            _pipeStart.Set();

            _copyThread.Join();
            _pipeThread.Join();

            _subprocess.StandardInput.Close();
            _subprocess.WaitForExit();

            var outputReader = _subprocess.StandardError;
            Error = outputReader.ReadToEnd();

            _subprocess.Close();
            _subprocess.Dispose();

            outputReader.Close();
            outputReader.Dispose();

            _subprocess = null;
            _copyThread = null;
            _pipeThread = null;
        }

        #endregion

        #region Private members

        Process _subprocess;
        Thread _copyThread;
        Thread _pipeThread;

        Mutex _copyMutex = new Mutex(true);
        AutoResetEvent _copyStart = new AutoResetEvent(false);
        AutoResetEvent _pipeStart = new AutoResetEvent(false);
        bool _terminate;

        Queue<NativeArray<byte>> _copyQueue = new Queue<NativeArray<byte>>();
        Queue<byte[]> _pipeQueue = new Queue<byte[]>();
        Queue<byte[]> _freeBuffer = new Queue<byte[]>();

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

        #region Thread entry methods

        void CopyThread()
        {
            while (!_terminate)
            {
                _copyStart.WaitOne();
                _copyMutex.WaitOne();

                while (!_terminate && _copyQueue.Count > 0)
                {
                    NativeArray<byte> source;
                    lock (_copyQueue) source = _copyQueue.Peek();

                    byte[] buffer = null;
                    if (_freeBuffer.Count > 0)
                        lock (_freeBuffer) buffer = _freeBuffer.Dequeue();

                    if (buffer == null || buffer.Length != source.Length)
                        buffer = source.ToArray();
                    else
                        source.CopyTo(buffer);

                    lock (_pipeQueue) _pipeQueue.Enqueue(buffer);
                    _pipeStart.Set();

                    lock (_copyQueue) _copyQueue.Dequeue();
                }

                _copyMutex.ReleaseMutex();
            }
        }

        void PipeThread()
        {
            while (!_terminate)
            {
                _pipeStart.WaitOne();

                while (!_terminate && _pipeQueue.Count > 0)
                {
                    byte[] buffer;
                    lock (_pipeQueue) buffer = _pipeQueue.Dequeue();

                    _subprocess.StandardInput.BaseStream.Write(buffer, 0, buffer.Length);
                    _subprocess.StandardInput.BaseStream.Flush();

                    lock (_freeBuffer) _freeBuffer.Enqueue(buffer);
                }
            }
        }

        #endregion
    }
}
