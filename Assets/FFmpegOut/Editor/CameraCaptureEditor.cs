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
        SerializedProperty _codec;
        SerializedProperty _recordLength;

        static GUIContent [] _codecLabels = {
            new GUIContent("ProRes (QuickTime)"),
            new GUIContent("H.264 (MP4)"),
            new GUIContent("VP8 (WebM)")
        };

        static int [] _codecOptions = { 0, 1, 2 };

        void OnEnable()
        {
            _setResolution = serializedObject.FindProperty("_setResolution");
            _width = serializedObject.FindProperty("_width");
            _height = serializedObject.FindProperty("_height");
            _frameRate = serializedObject.FindProperty("_frameRate");
            _codec = serializedObject.FindProperty("_codec");
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
            EditorGUILayout.IntPopup(_codec, _codecLabels, _codecOptions);
            EditorGUILayout.PropertyField(_recordLength);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
