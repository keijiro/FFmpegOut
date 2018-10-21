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

        void PushFrame(RenderTexture frame)
        {
            // Lazy initialization of capturing session
            if (_session == null)
                _session = FFmpegSession.Create(
                    gameObject.name, frame.width, frame.height, 60, preset
                );

            _session.PushFrame(frame);
        }

        #endregion

        #region MonoBehaviour implementation

        void OnDisable()
        {
            if (_session != null)
            {
                _session.Close();
                _session.Dispose();
                _session = null;
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
            // Render texture mode
            if (GetComponent<Camera>() == null && _sourceTexture != null)
                PushFrame(_sourceTexture);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // Camera capture mode
            PushFrame(source);
            Graphics.Blit(source, destination);
        }

        #endregion
    }
}
