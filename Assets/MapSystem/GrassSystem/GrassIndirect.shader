Shader "Custom/GrassIndirect"
{
    Properties
    {
        _WindStrength("Wind Strength", Range(0,1)) = 0.3
        _WindSpeed("Wind Speed", Range(0,10)) = 2
        _MainTex("Main Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _ScaleCorrection("Scale Correction", Vector) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            Cull Off

            HLSLPROGRAM
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            StructuredBuffer<float4> _PositionBuffer;
            float4 _BaseColor;
            float _WindStrength;
            float _WindSpeed;
            float4 _ScaleCorrection;

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
                uint instanceID   : SV_InstanceID;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : NORMAL;
                float2 uv          : TEXCOORD0;
                float3 positionWS  : TEXCOORD1;
                float4 shadowCoord : TEXCOORD2;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float4 data = _PositionBuffer[IN.instanceID];
                float3 instancePos = data.xyz;
                float scale = data.w;

                float3 objPos = IN.positionOS * scale * _ScaleCorrection.xyz;
                float3 worldPos = TransformObjectToWorld(objPos);
                worldPos += instancePos;

                float wind = sin(worldPos.x * 0.2 + _Time.y * _WindSpeed)
                           * cos(worldPos.z * 0.2 + _Time.y * _WindSpeed)
                           * _WindStrength;
                worldPos.xz += float2(wind * 0.2, wind * 0.1);

                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS));
                OUT.uv = IN.uv;
                OUT.positionWS = worldPos;
                OUT.shadowCoord = TransformWorldToShadowCoord(worldPos);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _BaseColor;
                float3 normal = normalize(IN.normalWS);
            
                // 메인 라이트
                Light mainLight = GetMainLight(IN.shadowCoord);
                float NdotL = saturate(dot(normal, -mainLight.direction));
                NdotL = lerp(0.25, 1.0, NdotL);
                float3 diffuse = mainLight.color * NdotL * mainLight.shadowAttenuation;
            
                float3 ambient = 0.1 * _BaseColor.rgb;
                float3 finalColor = texColor.rgb * (diffuse + ambient);
            
                return float4(finalColor, 1);
            }

            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ColorMask 0
            Cull Off

            HLSLPROGRAM
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            StructuredBuffer<float4> _PositionBuffer;
            float4 _ScaleCorrection;

            struct Attributes
            {
                float3 positionOS : POSITION;
                uint instanceID   : SV_InstanceID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float4 data = _PositionBuffer[IN.instanceID];
                float3 instancePos = data.xyz;
                float scale = data.w;

                float3 objPos = IN.positionOS * scale * _ScaleCorrection.xyz;
                float3 worldPos = TransformObjectToWorld(objPos) + instancePos;

                OUT.positionCS = TransformWorldToHClip(worldPos);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }
}
