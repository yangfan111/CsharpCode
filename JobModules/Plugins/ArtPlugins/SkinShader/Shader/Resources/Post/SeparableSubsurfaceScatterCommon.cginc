#include "UnityCG.cginc" 
#define DistanceToProjectionWindow 5.671281819617709             //1.0 / tan(0.5 * radians(20));
#define DPTimes300 1701.384545885313                             //DistanceToProjectionWindow * 300
#define SamplerSteps 25

uniform float _SSSScale;
uniform float2 _RandomNumber;
uniform float4 _Kernel[SamplerSteps], _CameraDepthTexture_TexelSize;
uniform sampler2D _MainTex, _CameraDepthTexture;

struct VertexInput {
    float4 vertex : POSITION;
};
struct VertexOutput {
    float4 pos : SV_POSITION;
    float4 uv : TEXCOORD0;
};
VertexOutput vert (VertexInput v) {
    VertexOutput o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = ComputeScreenPos(o.pos);
    return o;
}
#define RANDOM(seed) (sin(cos(seed * 1354.135748 + 13.546184) * 1354.135716 + 32.6842317) * 0.5 + 0.5)
float4 SSS(float4 SceneColor, float2 UV, float2 SSSIntencity) {
    float SceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UV));                                   
    float BlurLength = DistanceToProjectionWindow / SceneDepth;                                   
    float2 UVOffset = SSSIntencity * BlurLength;             
    float4 BlurSceneColor = SceneColor;
    BlurSceneColor.rgb *=  _Kernel[0].rgb;
    for (int i = 1; i < SamplerSteps; i++) {
        float2 currentUV = RANDOM(float2(_ScreenParams.yx * UV.yx + UV.xy) * _ScreenParams.xy + _RandomNumber) * UVOffset;
        float2 SSSUV = UV +  _Kernel[i].a * currentUV;
        float4 SSSSceneColor = tex2D(_MainTex, SSSUV);
        float SSSDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, SSSUV)).r;         
        float SSSScale = saturate(DPTimes300 * SSSIntencity * abs(SceneDepth - SSSDepth));
        SSSSceneColor.rgb = lerp(SSSSceneColor.rgb, SceneColor.rgb, SSSScale);
        BlurSceneColor.rgb +=  _Kernel[i].rgb * SSSSceneColor.rgb;
    }
    return BlurSceneColor;
    //return float4(UVOffset, 0, 1);
}