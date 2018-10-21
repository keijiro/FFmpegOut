// FFmpegOut - FFmpeg video encoding plugin for Unity
// https://github.com/keijiro/KlakNDI

Shader "Hidden/FFmpegOut/Preprocess"
{
    Properties
    {
        _MainTex("", 2D) = "white" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;

    fixed4 frag_flip(v2f_img i) : SV_Target
    {
        float2 uv = i.uv;
        uv.y = 1 - uv.y;
        return tex2D(_MainTex, uv);
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_flip
            ENDCG
        }
    }
}
