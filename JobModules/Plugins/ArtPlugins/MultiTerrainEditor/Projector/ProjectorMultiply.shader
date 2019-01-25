// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'

Shader "Projector/Projector MultiplyMine"
{
    Properties
    {
        _Color ("Main Colour", Color) = (1,1,1,0)
        _ShadowTex ("Cookie", 2D) = "gray" { TexGen ObjectLinear }
    }
 
    Subshader
    {
        Tags { "RenderType"="Transparent"  "Queue"="Transparent+100"}
        Pass
        {
            ZWrite Off
            Offset -1, -1
 
            Fog { Mode Off }
           
            AlphaTest Less 1
            ColorMask RGB
            Blend One SrcAlpha
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_fog_exp2
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"
 
struct v2f
{
    float4 pos : SV_POSITION;
    float4 uv_Main     : TEXCOORD0;
};
 
sampler2D _ShadowTex;
float4 _Color;
float4x4 unity_Projector;
 
v2f vert(appdata_tan v)
{
    v2f o;
    o.pos = UnityObjectToClipPos (v.vertex);
    o.uv_Main = mul (unity_Projector, v.vertex);
      
    return o;
}
 
half4 frag (v2f i) : COLOR
{
    half4 tex = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uv_Main)) ;
  
    tex.a = (1.3-tex.a)*0.8f;
    
  	if (i.uv_Main.x >0 && i.uv_Main.x < 1 && i.uv_Main.y>0 && i.uv_Main.y < 1)
  	{
  		tex.rgb = 1-tex.a;
  		tex.rgb*=_Color;
  		
    	return tex;
    	
    }
    else
    	clip(-1);
    	
    return tex;
}
ENDCG
        }
    }
}
 