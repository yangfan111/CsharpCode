// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Nature/Terrain/Standard_Modify" {
	Properties{
		// set by terrain engine
		[HideInInspector] _Control("Control (RGBA)", 2D) = "red" {}
	[HideInInspector] _Splat3("Layer 3 (A)", 2D) = "white" {}
	[HideInInspector] _Splat2("Layer 2 (B)", 2D) = "white" {}
	[HideInInspector] _Splat1("Layer 1 (G)", 2D) = "white" {}
	[HideInInspector] _Splat0("Layer 0 (R)", 2D) = "white" {}
	[HideInInspector] _Normal3("Normal 3 (A)", 2D) = "bump" {}
	[HideInInspector] _Normal2("Normal 2 (B)", 2D) = "bump" {}
	[HideInInspector] _Normal1("Normal 1 (G)", 2D) = "bump" {}
	[HideInInspector] _Normal0("Normal 0 (R)", 2D) = "bump" {}
	[HideInInspector][Gamma] _Metallic0("Metallic 0", Range(0.0, 1.0)) = 0.0
		[HideInInspector][Gamma] _Metallic1("Metallic 1", Range(0.0, 1.0)) = 0.0
		[HideInInspector][Gamma] _Metallic2("Metallic 2", Range(0.0, 1.0)) = 0.0
		[HideInInspector][Gamma] _Metallic3("Metallic 3", Range(0.0, 1.0)) = 0.0
		[HideInInspector] _Smoothness0("Smoothness 0", Range(0.0, 1.0)) = 1.0
		[HideInInspector] _Smoothness1("Smoothness 1", Range(0.0, 1.0)) = 1.0
		[HideInInspector] _Smoothness2("Smoothness 2", Range(0.0, 1.0)) = 1.0
		[HideInInspector] _Smoothness3("Smoothness 3", Range(0.0, 1.0)) = 1.0
		[HideInInspector] FarUV("colIndex,rowIndex,colMax,rolMax",Vector) =(0,0,8,8)

		// used in fallback on old cards & base map
		[HideInInspector] _MainTex("BaseMap (RGB)", 2D) = "white" {}
	[HideInInspector] _Color("Main Color", Color) = (1,1,1,1)

		_FarMainTex("FarTexture Base (RGB) Smoothness (A)", 2D) = "white" {}
	//_FarMetallicTex("FarMetallic (R)", 2D) = "white" {}
	}

	 

		SubShader{
		Tags{
		"Queue" = "Geometry-100"
		"RenderType" = "Opaque"
	}
		 
		CGPROGRAM
#pragma surface surf Standard vertex:SplatmapVert finalcolor:SplatmapFinalColor finalgbuffer:SplatmapFinalGBuffer fullforwardshadows noinstancing
#pragma multi_compile_fog
#pragma target 3.5
		// needs more than 8 texcoords
#pragma exclude_renderers gles psp2
#include "UnityPBSLighting.cginc"

#pragma multi_compile __ _TERRAIN_NORMAL_MAP
#if defined(SHADER_API_D3D9) && defined(SHADOWS_SCREEN) && defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && defined(DYNAMICLIGHTMAP_ON) && defined(SHADOWS_SHADOWMASK) && defined(_TERRAIN_NORMAL_MAP) && defined(UNITY_SPECCUBE_BLENDING)
		// TODO : On d3d9 17 samplers would be used when : defined(SHADOWS_SCREEN) && defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && defined(DYNAMICLIGHTMAP_ON) && defined(SHADOWS_SHADOWMASK) && defined(_TERRAIN_NORMAL_MAP) && defined(UNITY_SPECCUBE_BLENDING)
		// In that case it would be probably acceptable to undef UNITY_SPECCUBE_BLENDING however at the moment (10/2/2016) we can't undef UNITY_SPECCUBE_BLENDING or other platform defines. CGINCLUDE being added after "Lighting.cginc".
		// For now, remove _TERRAIN_NORMAL_MAP in this case.
#undef _TERRAIN_NORMAL_MAP
#define DONT_USE_TERRAIN_NORMAL_MAP // use it to initialize o.Normal to (0,0,1) because the surface shader analysis still see this shader writes to per-pixel normal.
#endif
#define FAR_FOD_ON 1
#define TERRAIN_STANDARD_SHADER
#define TERRAIN_SURFACE_OUTPUT SurfaceOutputStandard
#include "TerrainSplatmapCommon_Modify.cginc"

		half _Metallic0;
	half _Metallic1;
	half _Metallic2;
	half _Metallic3;

	half _Smoothness0;
	half _Smoothness1;
	half _Smoothness2;
	half _Smoothness3;
	sampler2D _FarMainTex;
	//sampler2D _FarMetallicTex;
	half4 FarUV;
	void surf(Input IN, inout SurfaceOutputStandard o) {
		half4 splat_control;
		half weight;
		fixed4 mixedDiffuse;
		half4 defaultSmoothness = half4(_Smoothness0, _Smoothness1, _Smoothness2, _Smoothness3);
#ifdef DONT_USE_TERRAIN_NORMAL_MAP
		o.Normal = fixed3(0, 0, 1);
#endif
		SplatmapMix(IN, defaultSmoothness, splat_control, weight, mixedDiffuse, o.Normal);
#if FAR_FOD_ON
		  half4 c = tex2D(_FarMainTex, IN.tc_Control / FarUV.z + FarUV.xy / FarUV.z);
		  half dis =clamp((IN.dis-200 )/ 800, 0, 1);
	 	o.Albedo = lerp(mixedDiffuse.rgb,c,dis);
	//	o.Albedo = mixedDiffuse.rgb;
		o.Alpha = lerp( weight,1,dis);
		o.Smoothness = lerp(mixedDiffuse.a, 0.1, dis);
		o.Metallic = lerp( dot(splat_control, half4(_Metallic0, _Metallic1, _Metallic2, _Metallic3)),0,dis);
#else
o.Albedo = mixedDiffuse.rgb ;
o.Smoothness = mixedDiffuse.a ;
o.Alpha = weight;
o.Metallic = dot(splat_control, half4(_Metallic0, _Metallic1, _Metallic2, _Metallic3));
#endif


	}
	ENDCG
	}

		Dependency "AddPassShader" = "Hidden/TerrainEngine/Splatmap/Standard-AddPass"
		Dependency "BaseMapShader" = "Hidden/TerrainEngine/Splatmap/Standard-Base_Modify"

		Fallback "Nature/Terrain/Diffuse"
}
