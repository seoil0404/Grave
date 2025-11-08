Shader "Custom/GrassIndirect"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (0.4, 0.8, 0.4, 1)
        _WindStrength("Wind Strength", Range(0,1)) = 0.3
        _WindSpeed("Wind Speed", Range(0,10)) = 2
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            HLSLPROGRAM
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            StructuredBuffer<float4> _PositionBuffer;
            float4 _BaseColor;
            float _WindStrength;
            float _WindSpeed;

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                uint instanceID : SV_InstanceID;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : NORMAL;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float4 data = _PositionBuffer[IN.instanceID];
                float3 instancePos = data.xyz;
                float scale = data.w;

                if (data.y < -100.0)
                    instancePos.y = -10000;

                float3 worldPos = TransformObjectToWorld(IN.positionOS * scale);
                worldPos += instancePos;

                float wind = sin(worldPos.x * 0.2 + _Time.y * _WindSpeed)
                           * cos(worldPos.z * 0.2 + _Time.y * _WindSpeed)
                           * _WindStrength;

                worldPos.xz += float2(wind * 0.2, wind * 0.1);

                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half3 lightDir = normalize(float3(0.4, 1, 0.3));
                half diff = saturate(dot(IN.normalWS, lightDir)) * 0.6 + 0.4;
                return half4(_BaseColor.rgb * diff, 1);
            }
            ENDHLSL
        }
    }
}
