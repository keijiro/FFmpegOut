// FFmpegOut - FFmpeg video encoding plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using UnityEditor;
using System.Linq;

namespace FFmpegOut
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VideoCapture))]
    public class VideoCaptureEditor : Editor
    {
        SerializedProperty _sourceTexture;
        SerializedProperty _preset;

        GUIContent[] _presetLabels;
        int[] _presetOptions;

        void OnEnable()
        {
            _sourceTexture = serializedObject.FindProperty("_sourceTexture");
            _preset = serializedObject.FindProperty("_preset");

            var presets = FFmpegPreset.GetValues(typeof(FFmpegPreset));
            _presetLabels = presets.Cast<FFmpegPreset>().
                Select(p => new GUIContent(p.GetDisplayName())).ToArray();
            _presetOptions = presets.Cast<int>().ToArray();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_sourceTexture);
            EditorGUILayout.IntPopup(_preset, _presetLabels, _presetOptions);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
