Shader "Test Pattern"
{
    CGINCLUDE

    #include "UnityCG.cginc"

    // Hash function from H. Schechter & R. Bridson, goo.gl/RXiKaH
    uint Hash(uint s)
    {
        s ^= 2747636419u;
        s *= 2654435769u;
        s ^= s >> 16;
        s *= 2654435769u;
        s ^= s >> 16;
        s *= 2654435769u;
        return s;
    }

    float Random(uint seed)
    {
        return float(Hash(seed)) / 4294967295.0; // 2^32-1
    }

    half3 Hue2RGB(half h)
    {
        h = frac(saturate(h)) * 6 - 2;
        half3 rgb = saturate(half3(abs(h - 1) - 1, 2 - abs(h), 2 - abs(h - 2)));
        return rgb;
    }

    struct Varyings
    {
        float4 position : SV_Position;
        float2 uv : TEXCOORD0;
    };

    Varyings Vertex(float4 position : POSITION, float2 uv : TEXCOORD)
    {
        Varyings output;
        output.position = UnityObjectToClipPos(position);
        output.uv = uv;
        return output;
    }

    half4 Fragment(Varyings input) : SV_Target
    {
        uint id = floor((input.uv.x + floor(input.uv.y * 64)) * 64);

        half3 c = Hue2RGB(Random(id * 2));
        half param = Random(id * 2 + 1) * 2;

        c = lerp(lerp(0, c, saturate(param)), 1, saturate(param - 1));

    #ifndef UNITY_COLORSPACE_GAMMA
        c= GammaToLinearSpace(c);
    #endif

        return half4(c , 1);
    }

    ENDCG

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDCG
        }
    }
}
