using UnityEngine;
using UnityEditor;

namespace FFmpegOut
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CameraCapture))]
    public class CameraCaptureEditor : Editor
    {
        SerializedProperty _setResolution;
        SerializedProperty _width;
        SerializedProperty _height;
        SerializedProperty _frameRate;
        SerializedProperty _allowSlowDown;
        SerializedProperty _preset;
        SerializedProperty _startTime;
        SerializedProperty _recordLength;

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
            _setResolution = serializedObject.FindProperty("_setResolution");
            _width = serializedObject.FindProperty("_width");
            _height = serializedObject.FindProperty("_height");
            _frameRate = serializedObject.FindProperty("_frameRate");
            _allowSlowDown = serializedObject.FindProperty("_allowSlowDown");
            _preset = serializedObject.FindProperty("_preset");
            _startTime = serializedObject.FindProperty("_startTime");
            _recordLength = serializedObject.FindProperty("_recordLength");
        }

        public override void OnInspectorGUI()
        {
            // Show the editor controls.
            serializedObject.Update();

            EditorGUILayout.PropertyField(_setResolution);

            if (_setResolution.hasMultipleDifferentValues || _setResolution.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_width);
                EditorGUILayout.PropertyField(_height);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(_frameRate);
            EditorGUILayout.PropertyField(_allowSlowDown);
            EditorGUILayout.IntPopup(_preset, _presetLabels, _presetOptions);
            EditorGUILayout.PropertyField(_startTime);
            EditorGUILayout.PropertyField(_recordLength);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
