
Shader "VRM10/Universal Render Pipeline/MToon10 Refactored"
{
    Properties
    {
        // Rendering properties adjusted for URP
        _Color ("pbrMetallicRoughness.baseColorFactor", Color) = (1, 1, 1, 1) // Unity specified name
        _MainTex ("pbrMetallicRoughness.baseColorTexture", 2D) = "white" {} // Unity specified name
        _Cutoff ("alphaCutoff", Range(0, 1)) = 0.5 // Unity specified name
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

    struct Attributes
    {
        float4 positionOS : POSITION;
        float3 normalOS : NORMAL;
        float2 uv : TEXCOORD0;
    };

    struct Varyings
    {
        float4 positionHCS : SV_POSITION;
        float3 normalWS : NORMAL;
        float2 uv : TEXCOORD0;
    };

    Varyings vert(Attributes IN)
    {
        Varyings OUT;
        OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
        OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
        OUT.uv = IN.uv;
        return OUT;
    }

    half4 frag(Varyings IN) : SV_Target
    {
        half3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb;
        clip(color.a - _Cutoff); // Use _Cutoff for alpha clip
        return half4(albedo, 1.0);
    }
    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            HLSLPROGRAM
            // URP compatible vertex and fragment shaders
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
