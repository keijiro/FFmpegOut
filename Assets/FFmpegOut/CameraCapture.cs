using UnityEngine;
using System.Diagnostics;
using System.IO;

namespace FFmpegOut
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("FFmpegOut/Camera Capture")]
    public class CameraCapture : MonoBehaviour
    {
        #region Editable properties

        [SerializeField] float _recordLength = 5;

        #endregion

        #region Private members

        [SerializeField, HideInInspector] Shader _shader;

        float _elapsed;
        bool _recording;

        Material _material;
        Process _subprocess;
        BinaryWriter _stdin;

        #endregion

        #region MonoBehavior functions

        void OnValidate()
        {
            _recordLength = Mathf.Max(_recordLength, 0.01f);
        }

        void Start()
        {
            _recording = true;
            _material = new Material(_shader);
        }

        void OnDisable()
        {
            if (_recording)
            {
                CloseSubprocess();
                _recording = false;
            }
        }

        void Update()
        {
            if (_recording)
            {
                _elapsed += Time.deltaTime;
                if (_elapsed > _recordLength)
                {
                    CloseSubprocess();
                    _recording = false;
                }
            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_recording)
            {
                var temp = RenderTexture.GetTemporary(source.width, source.height);
                Graphics.Blit(source, temp, _material, 0);
                Dump(temp);
                RenderTexture.ReleaseTemporary(temp);
            }

            Graphics.Blit(source, destination);
        }

        #endregion

        #region Private methods

        void Dump(RenderTexture rt)
        {
            if (_subprocess == null) OpenSubprocess(name, rt);

            var temp = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            temp.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, false);
            temp.Apply();
            _stdin.Write(temp.GetRawTextureData());
            DestroyImmediate(temp);
        }

        void OpenSubprocess(string name, RenderTexture rt)
        {
            var exePath = Application.streamingAssetsPath + "/FFmpegOut/ffmpeg.exe";
            var outPath = name.Replace(" ", "_") + ".mov";

            var opt = "-y -f rawvideo -vcodec rawvideo -pixel_format rgb24";
            opt += " -video_size " + rt.width + "x" + rt.height;
            opt += " -framerate 30";
            opt += " -i - -c:v prores_ks -pix_fmt yuv422p10le ";
            opt += outPath;

            var info = new ProcessStartInfo(exePath, opt);
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;

            _subprocess = Process.Start(info);
            _stdin = new BinaryWriter(_subprocess.StandardInput.BaseStream);

            UnityEngine.Debug.Log("Capture started.\n" + exePath + " " + opt);
        }

        void CloseSubprocess()
        {
            _subprocess.StandardInput.Close();
            _subprocess.WaitForExit();

            var outputReader = _subprocess.StandardError;
            var output = outputReader.ReadToEnd();

            _subprocess.Close();

            outputReader.Close();
            outputReader.Dispose();

            _subprocess = null;
            _stdin = null;

            UnityEngine.Debug.Log("Capture completed.\n" + output);
        }

        #endregion
    }
}
