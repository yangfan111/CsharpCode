Shader "Hidden/SeparableSubsurfaceScatter" {
    Properties {
    }
    
    CGINCLUDE
    #include "SeparableSubsurfaceScatterCommon.cginc"
    #pragma target 3.0
    ENDCG

    SubShader {
        ZWrite Off
        ZTest Equal
        Cull Back
        Pass {
            Name "XBlur"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 frag(VertexOutput i) : SV_TARGET {
                float2 uv = i.uv.xy / i.uv.w;
                float4 SceneColor = tex2D(_MainTex, uv);
                float SSSIntencity = (_SSSScale * _CameraDepthTexture_TexelSize.x);
                float3 XBlurPlus = SSS(SceneColor, uv, float2(SSSIntencity, 0)).rgb;
                //float3 XBlurNagteiv = SSS(SceneColor, i.uv, float2(-SSSIntencity, 0)).rgb;
                //float3 XBlur = (XBlurPlus + XBlurNagteiv) / 2;
                return float4(XBlurPlus, SceneColor.a);
            }
            ENDCG
        } Pass {
            Name "YBlur"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 frag(VertexOutput i) : SV_TARGET {
                float2 uv = i.uv.xy / i.uv.w;
                float4 SceneColor = tex2D(_MainTex, uv);
                float SSSIntencity = (_SSSScale * _CameraDepthTexture_TexelSize.y);
                float3 YBlurPlus = SSS(SceneColor, uv, float2(0, SSSIntencity)).rgb;
                //float3 YBlurNagteiv = SSS(SceneColor, i.uv, float2(0, -SSSIntencity)).rgb;
                //float3 YBlur = (YBlurPlus + YBlurNagteiv) / 2;
                return float4(YBlurPlus, SceneColor.a);
            }
            ENDCG
        }
    }
}
