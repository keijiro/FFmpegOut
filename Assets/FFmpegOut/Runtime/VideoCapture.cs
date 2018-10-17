// FFmpegOut - FFmpeg video encoding plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using UnityEngine.Rendering;
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

        [SerializeField] FFmpegPipe.Preset _preset;

        public FFmpegPipe.Preset preset {
            get { return _preset; }
            set { _preset = value; }
        }

        #endregion

        #region Private objects

        FFmpegPipe _pipe;

        Queue<AsyncGPUReadbackRequest> _readbackQueue = new Queue<AsyncGPUReadbackRequest>(4);

        Material _blitMaterial;

        #endregion

        #region Frame readback queue operations

        void QueueFrame(RenderTexture source)
        {
            if (_readbackQueue.Count > 3)
            {
                Debug.LogWarning("Too many GPU readback requests.");
                return;
            }

            // Lazy initialization of the conversion shader.
            if (_blitMaterial == null)
            {
                var shader = Shader.Find("Hidden/FFmpegOut/CameraCapture");
                _blitMaterial = new Material(shader);
            }

            // Blit and readback
            var rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(source, rt, _blitMaterial);
            _readbackQueue.Enqueue(AsyncGPUReadback.Request(rt));
            RenderTexture.ReleaseTemporary(rt);
        }

        void ProcessQueue()
        {
            while (_readbackQueue.Count > 0)
            {
                var req = _readbackQueue.Peek();

                // Skip error frames.
                if (req.hasError)
                {
                    Debug.LogWarning("GPU readback error was detected.");
                    _readbackQueue.Dequeue();
                    continue;
                }

                // Break when found a frame that hasn't been read back yet.
                if (!req.done) break;

                // Lazy initialization of ffmpeg pipe
                if (_pipe == null)
                {
                    _pipe = new FFmpegPipe(name, req.width, req.height, 60, _preset);
                    Debug.Log("Capture started (" + _pipe.Filename + ")");
                }

                // Feed the frame to the ffmpeg pipe.
                _pipe.Write(req.GetData<byte>().ToArray());

                // Done. Remove the frame from the queue.
                _readbackQueue.Dequeue();
            }
        }

        #endregion

        #region MonoBehaviour implementation

        void OnEnable()
        {
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
        }

        void OnDestroy()
        {
            if (_blitMaterial != null) Destroy(_blitMaterial);
        }

        void Update()
        {
            ProcessQueue();

            if (GetComponent<Camera>() == null && _sourceTexture != null)
                QueueFrame(_sourceTexture);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            QueueFrame(source);

            Graphics.Blit(source, destination);
        }

        #endregion
    }
}
