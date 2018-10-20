// FFmpegOut - FFmpeg video encoding plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

namespace FFmpegOut
{
    [AddComponentMenu("FFmpegOut/Video Capture")]
    public sealed class VideoCapture : MonoBehaviour
    {
        #region Public properties

        [SerializeField] RenderTexture _sourceTexture;

        public RenderTexture sourceTexture {
            get { return _sourceTexture; }
            set { _sourceTexture = value; }
        }

        [SerializeField] FFmpegPreset _preset;

        public FFmpegPreset preset {
            get { return _preset; }
            set { _preset = value; }
        }

        #endregion

        #region Private objects

        FFmpegPipe _pipe;
        Material _blitMaterial;

        #endregion

        #region Frame readback queue

        struct Frame
        {
            public AsyncGPUReadbackRequest request;
            public RenderTexture texture;
        }

        Queue<Frame> _readbackQueue = new Queue<Frame>(4);

        void QueueFrame(RenderTexture source)
        {
            if (_readbackQueue.Count > 4)
            {
                Debug.LogWarning("Too many GPU readback requests.");
                return;
            }

            // Lazy initialization of the blit shader
            if (_blitMaterial == null)
            {
                var shader = Shader.Find("Hidden/FFmpegOut/CameraCapture");
                _blitMaterial = new Material(shader);
            }

            // Blit to a tenporary RT.
            var rt = RenderTexture.GetTemporary
                (source.width, source.height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(source, rt, _blitMaterial, 0);

            // Request read-back and push it to the queue.
            _readbackQueue.Enqueue(new Frame {
                request = AsyncGPUReadback.Request(rt),
                texture = rt
            });
        }

        void ProcessQueue()
        {
            while (_readbackQueue.Count > 0)
            {
                var req = _readbackQueue.Peek().request;

                // Break on an uncompleted readback.
                if (!req.done) break;

                // Skip error frames.
                if (req.hasError)
                {
                    Debug.LogWarning("GPU readback error was detected.");
                    RenderTexture.ReleaseTemporary(_readbackQueue.Dequeue().texture);
                    continue;
                }

                // Lazy initialization of ffmpeg pipe
                if (_pipe == null)
                {
                    _pipe = new FFmpegPipe(name, req.width, req.height, 60, _preset);
                    Debug.Log("Capture started (" + _pipe.Filename + ")");
                }

                // Feed the frame to the ffmpeg pipe.
                _pipe.PushFrameAsync(req.GetData<byte>());

                // Done. Remove the frame from the queue.
                RenderTexture.ReleaseTemporary(_readbackQueue.Dequeue().texture);
            }
        }

        #endregion

        #region MonoBehaviour implementation

        void OnEnable()
        {
            // Check if ffmpeg is available. Disable itself when unavailable.
            if (!FFmpegConfig.CheckAvailable)
            {
                Debug.LogError(
                    "ffmpeg.exe is missing. " +
                    "Please refer to the installation instruction. " +
                    "https://github.com/keijiro/FFmpegOut"
                );
                enabled = false;
            }
        }

        void OnDisable()
        {
            // Stop and close an active pipe.
            if (_pipe != null)
            {
                Debug.Log("Capture stopped (" + _pipe.Filename + ")");

                _pipe.Close();

                if (!string.IsNullOrEmpty(_pipe.Error))
                    Debug.LogWarning(
                        "ffmpeg returned with a warning or an error message. " +
                        "See the following lines for details:\n" + _pipe.Error
                    );

                _pipe = null;
            }

            // Dispose remains in the readback queue.
            while (_readbackQueue.Count > 0)
                RenderTexture.ReleaseTemporary(_readbackQueue.Dequeue().texture);
        }

        void OnDestroy()
        {
            if (_blitMaterial != null) Destroy(_blitMaterial);
        }

        IEnumerator Start()
        {
            var eof = new WaitForEndOfFrame();

            while (true)
            {
                yield return eof;

                // The GPU readback buffer will be disposed soon, so wait for
                // completion on frame pushes.
                _pipe?.CompletePushFrames();
            }
        }

        void Update()
        {
            ProcessQueue();

            // Render texture mode
            if (GetComponent<Camera>() == null && _sourceTexture != null)
                QueueFrame(_sourceTexture);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // Camera capture mode
            QueueFrame(source);

            Graphics.Blit(source, destination);
        }

        #endregion
    }
}
