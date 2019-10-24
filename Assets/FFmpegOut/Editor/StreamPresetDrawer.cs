using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FFmpegOut.LiveStream
{
    [CustomPropertyDrawer(typeof(StreamPreset))]
    public class StreamPresetDrawer : PropertyDrawer
    {
        const int LINES = 3;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect tmpRect = new Rect(
                position.x, position.y,
                position.width, base.GetPropertyHeight(property, GUIContent.none));

            tmpRect.y += base.GetPropertyHeight(property, GUIContent.none) / 2f;
            EditorGUI.PropertyField(tmpRect, property);

            // starting next line
            tmpRect.y += base.GetPropertyHeight(property, GUIContent.none);
            StreamPreset p = (StreamPreset) property.enumValueIndex;
            string tooltip = string.Empty;
            switch (p)
            {
                case StreamPreset.Udp:
                tooltip = @"[udp adress]; e.g. udp://127.0.0.1:10755";
                break;
                case StreamPreset.Rtp:
                tooltip = @"[rtp adress]; e.g. rtp://127.0.0.1:10755";
                break;
                case StreamPreset.Rtsp:
                tooltip = @"[rtsp adress]; e.g. rtsp://127.0.0.1:10755";
                break;
                case StreamPreset.Hls:
                tooltip = @"[http server base url] [htdocs directory path + m3u8 file name extension]; e.g. http://127.0.0.1:8000/ D:\[ServerLocation]\htdocs\stream.m3u8";
                break;
                case StreamPreset.Hls_ssegment:
                tooltip = @"[http server base url] -segment_list [htdocs directory path + m3u8 file name extension] [htdocs directory path + ts file name extension]; e.g. http://127.0.0.185:8000/ -segment_list D:\[ServerLocation]\htdocs\stream.m3u8 D:\[ServerLocation]\htdocs\out%03d.ts";
                break;
                case StreamPreset.Rtmp:
                tooltip = @"[rtmp adress]; e.g. rtmp://127.0.0.1:1935/rtmp_stream/mystream";
                break;
            }

            Rect indentPosition = new Rect(tmpRect);
            indentPosition = EditorGUI.PrefixLabel(tmpRect, new GUIContent("Address Example"));
            EditorGUI.SelectableLabel(indentPosition, tooltip);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * LINES;
        }
    }
}
