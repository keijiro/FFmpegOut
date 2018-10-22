// FFmpegOut - FFmpeg video encoding plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using UnityEngine.Rendering;

namespace FFmpegOut
{
    sealed class Blitter : MonoBehaviour
    {
        #region Factory method

        static System.Type[] _initialComponents =
            { typeof(Camera), typeof(Blitter) };

        public static GameObject CreateInstance(Camera source)
        {
            var go = new GameObject("Blitter", _initialComponents);
            go.hideFlags = HideFlags.HideInHierarchy;

            var camera = go.GetComponent<Camera>();
            camera.cullingMask = 0;
            camera.targetDisplay = source.targetDisplay;

            var blitter = go.GetComponent<Blitter>();
            blitter._sourceTexture = source.targetTexture;

            return go;
        }

        #endregion

        #region Private members

        RenderTexture _sourceTexture;
        CommandBuffer _commandBuffer;

        #endregion

        #region MonoBehaviour implementation

        void Update()
        {
            if (_commandBuffer == null)
            {
                _commandBuffer = new CommandBuffer();
                _commandBuffer.name = "FFmpegOut Blitter";

                _commandBuffer.Blit
                    (_sourceTexture, BuiltinRenderTextureType.CameraTarget);

                GetComponent<Camera>().AddCommandBuffer
                    (CameraEvent.AfterEverything, _commandBuffer);
            }
        }

        void OnDisable()
        {
            if (_commandBuffer != null)
            {
                GetComponent<Camera>().RemoveCommandBuffer
                    (CameraEvent.AfterEverything, _commandBuffer);

                _commandBuffer.Dispose();
                _commandBuffer = null;
            }
        }

        #endregion
    }
}
