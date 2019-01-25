Shader "Hidden/TerrainEngine/Details/WavingDoublePass"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[Header(Translucency)]
		_Grass_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
	//	_Grass_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		//_Grass_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		_MainTex("MainTex", 2D) = "white" {}
		_Grass_Translucency_10_x_multiplier("Translucency_10_x_multiplier", Float) = 10
				[Header(Wind Settings)]
		// Wave mode leave
        _MinY("Minimum Y Value", Range(-5,0)) = -1

        _xScale ("X Amount", Range(-0.2,0.2)) = 0.5
        _yScale ("Z Amount", Range(-0.2,0.2)) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Off
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecularCustom keepalpha addshadow fullforwardshadows exclude_path:deferred  vertex:vert
		struct Input
		{
			float2 uv_texcoord;
		};

		struct SurfaceOutputStandardSpecularCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			half3 Emission;
			fixed3 Specular;
			half Smoothness;
			half Occlusion;
			fixed Alpha;
			fixed3 Translucency;
		};

		uniform float4 _Grass_Color;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 Grass_Specular_Color;
		uniform float _Grass_Smoothness;
		uniform half _Grass_Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _Grass_TransAmbient;
		uniform half _Grass_TransShadow;
		uniform float _Grass_Translucency_Intensity;
		uniform float _Grass_Translucency_10_x_multiplier;
		uniform float4 _Grass_Translucency_Color;
		uniform float _Cutoff = 0.5;
		float _MinY;
        float _xScale;
        float _yScale;
        float _Grass_Wind_Scale;
        float _Grass_Wind_Speed;
        float _Grass_World_Scale;
        float _Amount;

		void vert (inout appdata_full v)
        {
            float num = v.vertex.z;

            if ((num-_MinY) > 0.0) {
                float3 worldPos = mul (unity_ObjectToWorld, v.vertex).xyz;
                float x = sin(worldPos.x/_Grass_World_Scale + (_Time.y*_Grass_Wind_Speed)) * (num-_MinY) * _Grass_Wind_Scale * 0.01;
                float y = sin(worldPos.y/_Grass_World_Scale + (_Time.y*_Grass_Wind_Speed)) * (num-_MinY) * _Grass_Wind_Scale * 0.01;

                v.vertex.x += x * _xScale;
                v.vertex.y += y * _yScale;
            }
        }


		inline half4 LightingStandardSpecularCustom(SurfaceOutputStandardSpecularCustom s, half3 viewDir, UnityGI gi )
		{
			#if !DIRECTIONAL
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _Grass_TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _Grass_TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Grass_Translucency, 0 );

			SurfaceOutputStandardSpecular r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Specular = s.Specular;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandardSpecular (r, viewDir, gi) + c;
		}

		inline void LightingStandardSpecularCustom_GI(SurfaceOutputStandardSpecularCustom s, UnityGIInput data, inout UnityGI gi )
		{
			UNITY_GI(gi, s, data);
		}

		void surf( Input i , inout SurfaceOutputStandardSpecularCustom o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = ( _Grass_Color * tex2DNode1 ).rgb;
			o.Specular = Grass_Specular_Color.rgb;
			o.Smoothness = _Grass_Smoothness;
			o.Translucency = ( tex2DNode1 * ( ( _Grass_Translucency_Intensity * _Grass_Translucency_10_x_multiplier ) * _Grass_Translucency_Color ) ).rgb;
			o.Alpha = 1;
			clip( tex2DNode1.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
}