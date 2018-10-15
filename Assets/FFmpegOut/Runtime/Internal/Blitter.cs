using UnityEngine;

namespace FFmpegOut
{
    // Simply blit a given texture to the screen.
    [RequireComponent(typeof(Camera))]
    internal class Blitter : MonoBehaviour
    {
        #region Static functions

        // A utility function for creating a blitter object.
        public static GameObject CreateGameObject(Camera originalCamera)
        {
            var go = new GameObject("Blitter", _blitterComponents);
            go.hideFlags = HideFlags.HideInHierarchy;

            var camera = go.GetComponent<Camera>();
            camera.cullingMask = 0;
            camera.targetDisplay = originalCamera.targetDisplay;

            var blitter = go.GetComponent<Blitter>();
            blitter._sourceTexture = originalCamera.targetTexture;

            return go;
        }

        #endregion

        #region Private members

        static System.Type[] _blitterComponents = {
                typeof(Camera), typeof(Blitter)
        };

        RenderTexture _sourceTexture;

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(_sourceTexture, destination);
        }

        #endregion
    }
}
