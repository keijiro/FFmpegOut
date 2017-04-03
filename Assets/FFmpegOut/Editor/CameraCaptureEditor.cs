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
        SerializedProperty _recordLength;

        void OnEnable()
        {
            _setResolution = serializedObject.FindProperty("_setResolution");
            _width = serializedObject.FindProperty("_width");
            _height = serializedObject.FindProperty("_height");
            _frameRate = serializedObject.FindProperty("_frameRate");
            _recordLength = serializedObject.FindProperty("_recordLength");
        }

        public override bool RequiresConstantRepaint()
        {
            return ((CameraCapture)target).isCapturing;
        }

        public override void OnInspectorGUI()
        {
            var capture = (CameraCapture)target;

            if (capture.isCapturing)
            {
                // Display the preview texture.
                var preview = capture.previewTexture;
                if (preview != null)
                {
                    var width = EditorGUIUtility.currentViewWidth;
                    var height = width * preview.height / preview.width;
                    var rect = EditorGUILayout.GetControlRect(false, height);
                    EditorGUI.DrawPreviewTexture(rect, preview);
                }
            }
            else
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
                EditorGUILayout.PropertyField(_recordLength);

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
