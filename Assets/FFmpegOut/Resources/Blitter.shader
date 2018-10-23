// FFmpegOut - FFmpeg video encoding plugin for Unity
// https://github.com/keijiro/KlakNDI

Shader "Hidden/FFmpegOut/Blitter"
{
    Properties
    {
        _MainTex("", 2D) = "gray" {}
    }

    HLSLINCLUDE

    sampler2D _MainTex;

    void Vertex(
        uint vid : SV_VertexID,
        out float4 position : SV_Position,
        out float2 texcoord : TEXCOORD
    )
    {
        float x = (vid == 1) ? 1 : 0;
        float y = (vid == 2) ? 1 : 0;
        position = float4(x * 4 - 1, y * 4 - 1, 1, 1);
        texcoord = float2(x * 2, 1 - y * 2);
    }

    half4 Fragment(
        float4 position : SV_Position,
        float2 texcoord : TEXCOORD
    ) : SV_Target
    {
        return tex2D(_MainTex, texcoord);
    }

    ENDHLSL

    SubShader
    {
        Tags { "Queue" = "Transparent+100" }
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDHLSL
        }
    }
}
