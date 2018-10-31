// FFmpegOut - FFmpeg video encoding plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using System.Collections;

namespace FFmpegOut
{
    [AddComponentMenu("FFmpegOut/Frame Rate Controller")]
    public sealed class FrameRateController : MonoBehaviour
    {
        [SerializeField] float _frameRate = 60;
        [SerializeField] bool _offlineMode = true;

        int _originalFrameRate;
        int _originalVSyncCount;

        internal int CalculateVSyncCount()
        {
            // Determine the display refresh rate.
            // We assume 59=59.95Hz, 23=23.976Hz and so on.
            // Is it the right way to get fractional-number rate? Who knows.
            var i_rate = Screen.currentResolution.refreshRate;
            var f_rate = (float)i_rate;

            switch (i_rate)
            {
                case  23: f_rate = 23.976f; break;
                case  29: f_rate = 29.970f; break;
                case  47: f_rate = 47.952f; break;
                case  59: f_rate = 59.940f; break;
                case  71: f_rate = 71.928f; break;
                case 119: f_rate = 119.88f; break;
            }

            // Return a positive value if it's divisible by the frame rate.
            if (Mathf.Approximately(f_rate % _frameRate, 0))
                return Mathf.RoundToInt(f_rate / _frameRate);
            else
                return 0; // Don't use v-sync.
        }

        void OnEnable()
        {
            var ifps = Mathf.RoundToInt(_frameRate);

            if (_offlineMode)
            {
                _originalFrameRate = Time.captureFramerate;
                Time.captureFramerate = ifps;
            }
            else
            {
                _originalFrameRate = Application.targetFrameRate;
                _originalVSyncCount = QualitySettings.vSyncCount;
                Application.targetFrameRate = ifps;
                QualitySettings.vSyncCount = CalculateVSyncCount();
            }
        }

        void OnDisable()
        {
            if (_offlineMode)
            {
                Time.captureFramerate = _originalFrameRate;
            }
            else
            {
                Application.targetFrameRate = _originalFrameRate;
                QualitySettings.vSyncCount = _originalVSyncCount;
            }
        }
    }
}
