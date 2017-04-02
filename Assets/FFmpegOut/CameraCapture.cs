using UnityEngine;

namespace FFmpegOut
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("FFmpegOut/Camera Capture")]
    public class CameraCapture : MonoBehaviour
    {
        #region Editable properties

        [SerializeField] float _recordLength = 5;
        [SerializeField] int _frameRate = 30;

        #endregion

        #region Private members

        [SerializeField, HideInInspector] Shader _shader;
        Material _material;

        FFmpegPipe _pipe;
        float _elapsed;

        #endregion

        #region MonoBehavior functions

        void OnValidate()
        {
            _recordLength = Mathf.Max(_recordLength, 0.01f);
        }

        void Start()
        {
            _material = new Material(_shader);
            Time.captureFramerate = _frameRate;
        }

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
            if (_pipe != null) ClosePipe();
        }

        void OnDestroy()
        {
            if (_pipe != null) ClosePipe();
        }

        void Update()
        {
            _elapsed += Time.deltaTime;

            if (_elapsed < _recordLength)
            {
                if (_pipe == null) OpenPipe();
            }
            else
            {
                if (_pipe != null) ClosePipe();
            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_pipe != null)
            {
                var tempRT = RenderTexture.GetTemporary(source.width, source.height);
                Graphics.Blit(source, tempRT, _material, 0);

                var tempTex = new Texture2D(source.width, source.height, TextureFormat.RGB24, false);
                tempTex.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0, false);
                tempTex.Apply();

                _pipe.Write(tempTex.GetRawTextureData());

                Destroy(tempTex);
                RenderTexture.ReleaseTemporary(tempRT);
            }

            Graphics.Blit(source, destination);
        }

        #endregion

        #region Private methods

        void OpenPipe()
        {
            var camera = GetComponent<Camera>();
            var width = camera.pixelWidth;
            var height = camera.pixelHeight;

            _pipe = new FFmpegPipe(name, width, height, _frameRate);

            Debug.Log("Capture started (" + _pipe.Filename + ")");
        }

        void ClosePipe()
        {
            if (_pipe != null)
            {
                Debug.Log("Capture ended (" + _pipe.Filename + ")");

                _pipe.Close();

                if (!string.IsNullOrEmpty(_pipe.Error))
                {
                    Debug.LogWarning(
                        "ffmpeg returned with a warning or an error message. " +
                        "See the following lines for details:\n" + _pipe.Error
                    );
                }

                _pipe = null;
            }
        }

        #endregion
    }
}
