// FFmpegOut - FFmpeg video encoding plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using System.Collections;

namespace FFmpegOut
{
    [AddComponentMenu("FFmpegOut/Video Capture")]
    public sealed class VideoCapture : MonoBehaviour
    {
        #region Public properties

        [SerializeField] int _width = 1920;

        public int width {
            get { return _width; }
            set { _width = value; }
        }

        [SerializeField] int _height = 1080;

        public int height {
            get { return _height; }
            set { _height = value; }
        }

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

        #region Private members

        FFmpegSession _session;
        RenderTexture _frame;
        GameObject _blitter;

        #endregion

        #region MonoBehaviour implementation

        void OnValidate()
        {
            _width = Mathf.Max(8, _width);
            _height = Mathf.Max(8, _height);
        }

        void OnDisable()
        {
            if (_session != null)
            {
                // Close and dispose the FFmpeg session.
                _session.Close();
                _session.Dispose();
                _session = null;
            }

            if (_frame != null)
            {
                // Dispose the frame texture.
                GetComponent<Camera>().targetTexture = null;
                Destroy(_frame);
                _frame = null;
            }

            if (_blitter != null)
            {
                // Destroy the blitter game object.
                Destroy(_blitter);
                _blitter = null;
            }
        }

        IEnumerator Start()
        {
            // Sync with FFmpeg pipe thread at the end of every frame.
            for (var eof = new WaitForEndOfFrame();;)
            {
                yield return eof;
                _session?.CompletePushFrames();
            }
        }

        void Update()
        {
            // Lazy initialization
            if (_session == null)
            {
                // Create and activate a frame texture.
                _frame = new RenderTexture(_width, _height, 0);
                GetComponent<Camera>().targetTexture = _frame;

                // Create a blitter object to keep presenting frames.
                _blitter = Blitter.CreateInstance(GetComponent<Camera>());

                // Start an FFmpeg session.
                _session = FFmpegSession.Create
                    (gameObject.name, _width, _height, 60, preset);
            }

            // Push the current frame to FFmpeg;
            _session.PushFrame(_frame);
        }

        #endregion
    }
}
