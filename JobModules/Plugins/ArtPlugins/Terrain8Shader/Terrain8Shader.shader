Shader "Nature/Terrain8Shader" {
	Properties {
		//_Color ("Color", Color) = (1,1,1,1)
	 
		 _TextureArray1("TexArray1", 2D) = "black" {}
		 _TextureArray2("TexArray2", 2D) = "black" {}
		 _TextureArray3("TexArray3", 2D) = "black" {}
		 _TextureArray4("TexArray4", 2D) = "black" {}
		 _TextureArray5("TexArray5", 2D) = "black" {}
		 _TextureArray6("TexArray6", 2D) = "black" {}
		 _TextureArray7("TexArray7", 2D) = "black" {}
		 _TextureArray8("TexArray8", 2D) = "black" {}
		 
		 _NormalArray1("NormalArray1", 2D) = "bump" {}
		 _NormalArray2("NormalArray2", 2D) = "bump" {}
		 _NormalArray3("NormalArray3", 2D) = "bump" {}
		 _NormalArray4("NormalArray4", 2D) = "bump" {}
		 _NormalArray5("NormalArray5", 2D) = "bump" {}
		 _NormalArray6("NormalArray6", 2D) = "bump" {}
		 _NormalArray7("NormalArray7", 2D) = "bump" {}
		 _NormalArray8("NormalArray8", 2D) = "bump" {}

 
 		_Split1Tex ("Split1 (RGB)", 2D) = "red" {}
		_Split2Tex("Split2 (RGB)", 2D) = "black" {}
		_Metallic1("_Metallic1 (1234)", Vector) = (0,0,0,0)
		_Metallic2("_Metallic1 (5678)", Vector) = (0,0,0,0)	
		_Smooth1("_Smooth1 (1234)", Vector) = (0,0,0,0)
			_Smooth2("_Smooth2 (5678)", Vector) = (0,0,0,0)
	 
	}
	SubShader {
		Tags { "RenderType"="Opaque" 	"Queue" = "Geometry-100" }
		LOD 200
		
		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
 // Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
//#pragma exclude_renderers d3d11 gles
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 4.0

			//UNITY_DECLARE_TEX2DARRAY(_TextureArray);
			//UNITY_DECLARE_TEX2DARRAY(_NormalArray);
		//uniform	half4 _UVArray[8];
		//uniform	half _Glossiness[8];
		//uniform	half _Metallic[8];
		 	
 
		sampler2D _Split1Tex;
 		 sampler2D _TextureArray1;
		 sampler2D _TextureArray2;
		 sampler2D _TextureArray3;
		 sampler2D _TextureArray4;
 

	 
		 sampler2D _NormalArray1;
		 sampler2D _NormalArray2;
		 sampler2D _NormalArray3;
		 sampler2D _NormalArray4;
		 half4 _Metallic1;
		 half4 _Smooth1;

 		//UNITY_DECLARE_TEX2D_NOSAMPLER(_Split2Tex);
	 //UNITY_DECLARE_TEX2D_NOSAMPLER(_NormalArray);
		struct Input {
 			float2 uv_Split1Tex;
 			float2 uv_TextureArray1;
 			float2 uv_TextureArray2;
 			float2 uv_TextureArray3;
 			float2 uv_TextureArray4;
 	 
		};

	 
		//fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
			UNITY_INSTANCING_CBUFFER_END
	
		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			half3 col = 0;
			 
			fixed4 nrm = 0.0f;
			fixed4 sp1 = tex2D(_Split1Tex,IN.uv_Split1Tex);
	//	float	weight = dot(sp1, half4(1, 1, 1, 1));
		//sp1 /= (weight + 1e-3f);
			sampler2D _TextureArray[4] = { _TextureArray1 ,_TextureArray2,_TextureArray3,_TextureArray4};
			sampler2D _NormalArray[4] = { _NormalArray1 ,_NormalArray2,_NormalArray3,_NormalArray4};
			half2 _UVArray[4] = { IN.uv_TextureArray1,IN.uv_TextureArray2,IN.uv_TextureArray3,IN.uv_TextureArray4 };
			for (int i = 0;i < 4;i++) {
			 	half2 uv = _UVArray[i];
		 
 				col += tex2D(_TextureArray[i], uv) * sp1[i];
 			 	 nrm += tex2D(_NormalArray[i], uv)*sp1[i];
 
	 
			}
			o.Normal = UnpackNormal(nrm);
			o.Albedo = col;
			 
			o.Metallic =   dot(sp1, _Metallic1);
			o.Smoothness = dot(sp1, _Smooth1);

			o.Alpha = 1;
		}
		ENDCG


			Tags{ "RenderType" = "Opaque" 	"Queue" = "Geometry-100" }
			LOD 200
			blend  one one// srcAlpha oneMinusSrcAlpha
			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows 

			// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 4.0

			//UNITY_DECLARE_TEX2DARRAY(_TextureArray);
			//UNITY_DECLARE_TEX2DARRAY(_NormalArray);
			uniform	half4 _UVArray[8];
		uniform	half _Glossiness[8];
		uniform	half _Metallic[8];


	 
		sampler2D _Split2Tex;
		 
		sampler2D _TextureArray5;
		sampler2D _TextureArray6;
		sampler2D _TextureArray7;
		sampler2D _TextureArray8;


		 
		sampler2D _NormalArray5;
		sampler2D _NormalArray6;
		sampler2D _NormalArray7;
		sampler2D _NormalArray8;
		half4 _Metallic2;
		half4 _Smooth2;
		//UNITY_DECLARE_TEX2D_NOSAMPLER(_Split2Tex);
		//UNITY_DECLARE_TEX2D_NOSAMPLER(_NormalArray);
		struct Input {
			float2 uv_Split2Tex;
			float2 uv_TextureArray5;
			float2 uv_TextureArray6;
			float2 uv_TextureArray7;
			float2 uv_TextureArray8;
		};


		//fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
			UNITY_INSTANCING_CBUFFER_END

			void surf(Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			half3 col = 0;
 			fixed4 nrm = 0.0f;
			fixed4 sp2 = tex2D(_Split2Tex, IN.uv_Split2Tex);
		//	float	weight = dot(sp2, half4(1, 1, 1, 1));
		//	sp2 /= (weight + 1e-3f);
			sampler2D _TextureArray[4] = { _TextureArray5 ,_TextureArray6,_TextureArray7,_TextureArray8 };
			sampler2D _NormalArray[4] = { _NormalArray5 ,_NormalArray6,_NormalArray7,_NormalArray8 };
			half2 _UVArray[4] = { IN.uv_TextureArray5,IN.uv_TextureArray6,IN.uv_TextureArray7,IN.uv_TextureArray8 };

			for (int i = 0;i < 4;i++) {
				half2 uv = _UVArray[i];
			 
				col += tex2D(_TextureArray[i], uv) * sp2[i];
				nrm += tex2D(_NormalArray[i], uv)*sp2[i];
			 

			}
			 
			o.Normal = UnpackNormal(nrm);
			o.Albedo = col;
			o.Metallic =   dot(sp2, _Metallic2);
			o.Smoothness =   dot(sp2, _Smooth2);
		 
			 
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
		CustomEditor "Terrain8ShaderGUI"

 }
