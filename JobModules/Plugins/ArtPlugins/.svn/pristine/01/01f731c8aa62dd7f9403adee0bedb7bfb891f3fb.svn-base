// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Hidden/TerrainEngine/Splatmap/Standard-Base_Modify" {
	Properties {
		_FarMainTex ("FarTexture Base (RGB) Smoothness (A)", 2D) = "white" {}
	//	_FarMetallicTex ("FarMetallic (R)", 2D) = "white" {}
		  FarUV("colIndex,rowIndex,colMax,rolMax",Vector) = (0,0,8,8)

		// used in fallback on old cards
		_Color ("Main Color", Color) = (1,1,1,1)
	}

	SubShader {
		Tags {
			"RenderType" = "Opaque"
			"Queue" = "Geometry-100"
		}
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0
		// needs more than 8 texcoords
		#pragma exclude_renderers gles
		#include "UnityPBSLighting.cginc"

		sampler2D _FarMainTex;
		//sampler2D _FarMetallicTex;
		half4 FarUV;
		struct Input {
			float2 uv_FarMainTex;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half4 c = tex2D (_FarMainTex, IN.uv_FarMainTex/ FarUV.z + FarUV.xy/FarUV.z);
			o.Albedo = c.rgb;
			o.Alpha = 1;
			o.Smoothness = 0.1;
			o.Metallic = 0; //tex2D (_FarMetallicTex, IN.uv_FarMainTex).r*0;
		}

		ENDCG
	}

	FallBack "Diffuse"
}
