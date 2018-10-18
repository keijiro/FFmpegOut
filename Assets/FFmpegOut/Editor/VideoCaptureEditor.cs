// FFmpegOut - FFmpeg video encoding plugin for Unity
// https://github.com/keijiro/KlakNDI

using UnityEngine;
using UnityEditor;

namespace FFmpegOut
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VideoCapture))]
    public class VideoCaptureEditor : Editor
    {
        SerializedProperty _sourceTexture;
        SerializedProperty _preset;

        static GUIContent [] _presetLabels = {
            new GUIContent("H.264 Default (MP4)"),
            new GUIContent("H.264 Lossless 420 (MP4)"),
            new GUIContent("H.264 Lossless 444 (MP4)"),
            new GUIContent("ProRes 422 (QuickTime)"),
            new GUIContent("ProRes 4444 (QuickTime)"),
            new GUIContent("VP8 (WebM)")
        };

        static int [] _presetOptions = { 0, 1, 2, 3, 4, 5 };

        void OnEnable()
        {
            _sourceTexture = serializedObject.FindProperty("_sourceTexture");
            _preset = serializedObject.FindProperty("_preset");
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
