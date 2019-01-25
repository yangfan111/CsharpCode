// Made with Amplify Shader Editor - Edited by ALIyerEdon   
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "LightingBox/Particle/Distance Based Translucency NoShadow"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Color("Color", Color) = (1,1,1,1)
		_TranslucencyPower("Translucency Power", Range(0,10)) = 0
		_MaxDistance("Max Distance",Float) = 7
   		_MaxIntensity("Max Intensity",Float) = 30
		_MainTex("Texture", 2D) = "white" {}
		[Header(Translucency)]
		[HideInInspector]_Translucency("Strength", Range( 0 , 50)) = 1
		[HideInInspector]_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		[HideInInspector]_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		[HideInInspector]_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		[HideInInspector]_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		Blend One OneMinusSrcAlpha

		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0

		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		struct SurfaceOutputStandardCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			fixed Alpha;
			fixed3 Translucency;
		};

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _Color;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform float _TranslucencyPower;
		float _Dist;
		float _MaxIntensity;
    	float _MaxDistance;

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			#if !DIRECTIONAL
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + c;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			UNITY_GI(gi, s, data);
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{

			_Dist = distance(_WorldSpaceCameraPos, i.worldPos);

        	if(_Dist>_MaxDistance)
				_Dist = _MaxDistance;

			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = ( tex2DNode1 * _Color ).rgb * (_Dist/_MaxIntensity);
			o.Metallic = 0.0;
			o.Smoothness = 0.0;

			float3 temp_cast_2 = (_TranslucencyPower).xxx;
			o.Translucency = temp_cast_2;
			o.Alpha = ( ( tex2DNode1.a * 10.0 ) * ( _Color.a * 0.1 ) )*_Dist/_MaxIntensity;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustom alpha:fade keepalpha fullforwardshadows noshadow exclude_path:deferred nometa noforwardadd

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}