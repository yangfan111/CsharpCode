//Shader "Custom/GpuTree" {
//	Properties {
//		_Color ("Color", Color) = (1,1,1,1)
//		_MainTex ("Albedo (RGB)", 2D) = "white" {}
//		_Glossiness ("Smoothness", Range(0,1)) = 0.5
//		_Metallic ("Metallic", Range(0,1)) = 0.0
//	}
//	SubShader {
//		Tags { "RenderType"="Opaque" }
//		LOD 200
//		
//		CGPROGRAM
//		// Physically based Standard lighting model, and enable shadows on all light types
//		#pragma surface surf Standard fullforwardshadows
//				#pragma multi_compile_instancing
//		#pragma instancing_options procedural:setup
//		// Use shader model 3.0 target, to get nicer looking lighting
//		#pragma target 3.0
//			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
//  StructuredBuffer<int> renderIDsBuffer;
//  StructuredBuffer<float4> treeLibBuffer;
//  #endif
//		sampler2D _MainTex;
// int meshIndex;
//		struct Input {
//			float2 uv_MainTex;
//		};
//			void setup()
//	{
//#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
//		/// Positions are calculated in the compute shader.
//		/// here we just use them.
//	  
// 	float3 position =treeLibBuffer[renderIDsBuffer[unity_InstanceID+meshIndex*10000]].xyz;
// 
//		
//		float scale =1;
//		 
// //position.z=unity_InstanceID*1.3;
//		unity_ObjectToWorld._11_21_31_41 = float4(scale, 0, 0, 0);
//		unity_ObjectToWorld._12_22_32_42 = float4(0, scale, 0, 0);
//		unity_ObjectToWorld._13_23_33_43 = float4(0, 0, scale, 0);
//		unity_ObjectToWorld._14_24_34_44 = float4(position.xyz, 1);
//		unity_WorldToObject = unity_ObjectToWorld;
//		unity_WorldToObject._14_24_34 *= -1;
//		unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
//#endif
//	}
//
//		half _Glossiness;
//		half _Metallic;
//		fixed4 _Color;
//
//		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
//		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
//		// #pragma instancing_options assumeuniformscaling
//		UNITY_INSTANCING_CBUFFER_START(Props)
//			// put more per-instance properties here
//		UNITY_INSTANCING_CBUFFER_END
//
//		void surf (Input IN, inout SurfaceOutputStandard o) {
//			// Albedo comes from a texture tinted by color
//			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
//			o.Albedo = c.rgb;
//			// Metallic and smoothness come from slider variables
//			o.Metallic = _Metallic;
//			o.Smoothness = _Glossiness;
//			o.Alpha = c.a;
//		}
//		ENDCG
//	}
//	FallBack "Diffuse"
//}
// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Instanced/GpuTreeCS"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_HueVariation("Hue Variation", Color) = (1.0,0.5,0.0,0.1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	_DetailTex("Detail", 2D) = "black" {}
	_BumpMap("Normal Map", 2D) = "bump" {}
	_Cutoff("Alpha Cutoff", Range(0,1)) = 0.333
		[MaterialEnum(Off,0,Front,1,Back,2)] _Cull("Cull", Int) = 2
		[MaterialEnum(None,0,Fastest,1,Fast,2,Better,3,Best,4,Palm,5)] _WindQuality("Wind Quality", Range(0,5)) = 0
	}

		// targeting SM3.0+
		SubShader
	{
		Tags
	{
		"Queue" = "Geometry"
		"IgnoreProjector" = "True"
		"RenderType" = "Opaque"
		"DisableBatching" = "LODFading"
	}
		LOD 400
		Cull[_Cull]

		CGPROGRAM
#pragma surface surf Lambert vertex:SpeedTreeVert nodirlightmap nodynlightmap noshadowmask
#pragma target 3.0
#pragma multi_compile __ LOD_FADE_PERCENTAGE LOD_FADE_CROSSFADE
#pragma instancing_options assumeuniformscaling lodfade maxcount:50 procedural:setup 
#pragma shader_feature GEOM_TYPE_BRANCH GEOM_TYPE_BRANCH_DETAIL GEOM_TYPE_FROND GEOM_TYPE_LEAF GEOM_TYPE_MESH
#pragma shader_feature EFFECT_BUMP
#pragma shader_feature EFFECT_HUE_VARIATION
#define ENABLE_WIND
#include "SpeedTreeCommon.cginc"
 
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		struct TreeInfo {
		half3 position;
		half3  params;//scale,rot,treeIndex;

	};
	   StructuredBuffer<int> renderIDsBuffer;
		   StructuredBuffer<TreeInfo> treeLibBuffer;
		   #endif
		   int meshIndex;
		void setup()
			{
		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
				/// Positions are calculated in the compute shader.
				/// here we just use them.
			  
			TreeInfo info  =treeLibBuffer[renderIDsBuffer[unity_InstanceID+meshIndex*4000]];
			half3 position = info.position;
			 //position.x = unity_InstanceID * 5;
			 //position.z = meshIndex * 10;
			 //position.y = 0;
			half scale =  info.params.x;
				half2 rot_sc= half2(0,1);
			   sincos(info.params.y, rot_sc.x, rot_sc.y);
 
				unity_ObjectToWorld._11_21_31_41 = float4(scale*rot_sc.y, 0, -rot_sc.x, 0);
				unity_ObjectToWorld._12_22_32_42 = float4(0, scale, 0, 0);
				unity_ObjectToWorld._13_23_33_43 = float4(rot_sc.x, 0, rot_sc.y*scale, 0);
				unity_ObjectToWorld._14_24_34_44 = float4(position.xyz, 1);
				unity_WorldToObject = unity_ObjectToWorld;
				unity_WorldToObject._14_24_34 *= -1;
				unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
		#endif
			}
		void surf(Input IN, inout SurfaceOutput OUT)
	{
		SpeedTreeFragOut o;
		SpeedTreeFrag(IN, o);
		SPEEDTREE_COPY_FRAG(OUT, o)
	}
	ENDCG

		Pass
	{
		Tags{ "LightMode" = "ShadowCaster" }

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
#pragma multi_compile __ LOD_FADE_PERCENTAGE LOD_FADE_CROSSFADE
#pragma multi_compile_instancing
#pragma instancing_options assumeuniformscaling lodfade maxcount:50    
#pragma shader_feature GEOM_TYPE_BRANCH GEOM_TYPE_BRANCH_DETAIL GEOM_TYPE_FROND GEOM_TYPE_LEAF GEOM_TYPE_MESH
#pragma multi_compile_shadowcaster
#define ENABLE_WIND
#include "SpeedTreeCommon.cginc"

		struct v2f
	{
		V2F_SHADOW_CASTER;
#ifdef SPEEDTREE_ALPHATEST
		float2 uv : TEXCOORD1;
#endif
		UNITY_DITHER_CROSSFADE_COORDS_IDX(2)
			UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
	};

	v2f vert(SpeedTreeVB v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
#ifdef SPEEDTREE_ALPHATEST
		o.uv = v.texcoord.xy;
#endif
		OffsetSpeedTreeVertex(v, unity_LODFade.x);
		TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
			UNITY_TRANSFER_DITHER_CROSSFADE_HPOS(o, o.pos)

			return o;
	}

	float4 frag(v2f i) : SV_Target
	{
		UNITY_SETUP_INSTANCE_ID(i);
#ifdef SPEEDTREE_ALPHATEST
	clip(tex2D(_MainTex, i.uv).a * _Color.a - _Cutoff);
#endif
	UNITY_APPLY_DITHER_CROSSFADE(i)
		SHADOW_CASTER_FRAGMENT(i)
	}
		ENDCG
	}

		Pass
	{
		Tags{ "LightMode" = "Vertex" }

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
#pragma multi_compile_fog
#pragma multi_compile __ LOD_FADE_PERCENTAGE LOD_FADE_CROSSFADE
#pragma multi_compile_instancing
#pragma instancing_options assumeuniformscaling lodfade maxcount:50  procedural:setup 
#pragma shader_feature GEOM_TYPE_BRANCH GEOM_TYPE_BRANCH_DETAIL GEOM_TYPE_FROND GEOM_TYPE_LEAF GEOM_TYPE_MESH
#pragma shader_feature EFFECT_HUE_VARIATION
#define ENABLE_WIND
#include "SpeedTreeCommon.cginc"

		struct v2f
	{
		float4 vertex	: SV_POSITION;
		UNITY_FOG_COORDS(0)
			Input data : TEXCOORD1;
		UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
	};

	v2f vert(SpeedTreeVB v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		SpeedTreeVert(v, o.data);
		o.data.color.rgb *= ShadeVertexLightsFull(v.vertex, v.normal, 4, true);
		o.vertex = UnityObjectToClipPos(v.vertex);
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		UNITY_SETUP_INSTANCE_ID(i);
	SpeedTreeFragOut o;
	SpeedTreeFrag(i.data, o);
	fixed4 c = fixed4(o.Albedo, o.Alpha);
	UNITY_APPLY_FOG(i.fogCoord, c);
	return c;
	}
		ENDCG
	}
	}
	 

	  

	//	FallBack "Transparent/Cutout/VertexLit"
		CustomEditor "SpeedTreeMaterialInspector"
}
