Shader "Custom/BdGridShader"
{
    Properties
    {
        _CellSize ("Cell Size", Float) = 1
        _LineWidth ("Line Width", Float) = 0.02
        _GridColor ("Grid Color", Color) = (1,1,1,0.5)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float _CellSize;
                float _LineWidth;
                half4 _GridColor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.worldPos = worldPos;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 coord = IN.worldPos.xz / _CellSize;
                float2 grid = abs(frac(coord) - 0.5);
                float gridlLine = step(0.5 - _LineWidth, max(grid.x, grid.y));
                return _GridColor * gridlLine;
            }

            ENDHLSL
        }
    }
}
