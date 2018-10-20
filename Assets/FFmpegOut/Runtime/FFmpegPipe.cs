// FFmpegOut - FFmpeg video encoding plugin for Unity
// https://github.com/keijiro/KlakNDI

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

        public string Filename { get; private set; }
        public string Error { get; private set; }

        #endregion

        #region Public methods

        public FFmpegPipe(
            string name, int width, int height, int framerate,
            FFmpegPreset preset
        )
        {
            // Output file name
            name += DateTime.Now.ToString(" yyyy MMdd HHmmss");
            Filename = name.Replace(" ", "_") + preset.GetSuffix();

            // ffmpeg command line options
            var opt = "-y -f rawvideo -vcodec rawvideo -pixel_format rgba";
            opt += " -colorspace bt709";
            opt += " -video_size " + width + "x" + height;
            opt += " -framerate " + framerate;
            opt += " -loglevel warning -i - " + preset.GetOptions();
            opt += " " + Filename;

            // Subprocess options
            var info = new ProcessStartInfo(FFmpegConfig.BinaryPath, opt);
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;

            // Start ffmpeg subprocess.
            _subprocess = Process.Start(info);

            // Start copy/pipe subthreads.
            _copyThread = new Thread(CopyThread);
            _pipeThread = new Thread(PipeThread);
            _copyThread.Start();
            _pipeThread.Start();
        }

        public void PushFrameAsync(NativeArray<byte> data)
        {
            lock (_copyQueue) _copyQueue.Enqueue(data);
            _copyPing.Set();
        }

        public void CompletePushFrames()
        {
            while (_copyQueue.Count > 0) _copyEnd.WaitOne();
        }

        public void Close()
        {
            // Terminate the subthreads.
            _terminate = true;

            _copyPing.Set();
            _pipePing.Set();

            _copyThread.Join();
            _pipeThread.Join();

            // Close ffmpeg subprocess.
            _subprocess.StandardInput.Close();
            _subprocess.WaitForExit();

            var outputReader = _subprocess.StandardError;
            Error = outputReader.ReadToEnd();

            _subprocess.Close();
            _subprocess.Dispose();

            outputReader.Close();
            outputReader.Dispose();
        }

        #endregion

        #region Private members

        Process _subprocess;
        Thread _copyThread;
        Thread _pipeThread;

        AutoResetEvent _copyPing = new AutoResetEvent(false);
        AutoResetEvent _copyEnd  = new AutoResetEvent(false);
        AutoResetEvent _pipePing = new AutoResetEvent(false);
        bool _terminate;

        Queue<NativeArray<byte>> _copyQueue = new Queue<NativeArray<byte>>();
        Queue<byte[]> _pipeQueue = new Queue<byte[]>();
        Queue<byte[]> _freeBuffer = new Queue<byte[]>();

        #endregion

        #region Subthread entry points

        // CopyThread - Copies frames given from the readback queue to the pipe
        // queue. This is required because readback buffers are not under our
        // control -- they'll be disposed before being processed by us. They
        // have to be buffered by end-of-frame.
        void CopyThread()
        {
            while (!_terminate)
            {
                // Wait for ping from the main thread.
                _copyPing.WaitOne();

                // Process all entries in the copy queue.
                while (!_terminate && _copyQueue.Count > 0)
                {
                    // Retrieve an copy queue entry without dequeuing it.
                    // (We don't want to notify the main thread at this point.)
                    NativeArray<byte> source;
                    lock (_copyQueue) source = _copyQueue.Peek();

                    // Try allocating a buffer from the free buffer list.
                    byte[] buffer = null;
                    if (_freeBuffer.Count > 0)
                        lock (_freeBuffer) buffer = _freeBuffer.Dequeue();

                    // Copy the contents of the copy queue entry.
                    if (buffer == null || buffer.Length != source.Length)
                        buffer = source.ToArray();
                    else
                        source.CopyTo(buffer);

                    // Push the buffer entry to the pipe queue.
                    lock (_pipeQueue) _pipeQueue.Enqueue(buffer);
                    _pipePing.Set(); // Ping the pipe thread.

                    // Dequeue the copy buffer entry and ping the main thread.
                    lock (_copyQueue) _copyQueue.Dequeue();
                    _copyEnd.Set();
                }
            }
        }

        // PipeThread - Receives frame entries from the copy thread and push
        // them into the ffmpeg pipe.
        void PipeThread()
        {
            while (!_terminate)
            {
                // Wait for the ping from the copy thread.
                _pipePing.WaitOne();

                // Process all entries in the pipe queue.
                while (!_terminate && _pipeQueue.Count > 0)
                {
                    // Retrieve a frame entry.
                    byte[] buffer;
                    lock (_pipeQueue) buffer = _pipeQueue.Dequeue();

                    // Write it into the ffmpeg pipe.
                    _subprocess.StandardInput.BaseStream.Write(buffer, 0, buffer.Length);
                    _subprocess.StandardInput.BaseStream.Flush();

                    // Add the buffer to the free buffer list to reuse later.
                    lock (_freeBuffer) _freeBuffer.Enqueue(buffer);
                }
            }
        }

        #endregion
    }
}
