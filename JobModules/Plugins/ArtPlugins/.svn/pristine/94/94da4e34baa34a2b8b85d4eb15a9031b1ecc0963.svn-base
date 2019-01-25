Shader "Custom/RTDetail" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_GlossinessTex ("_Glossiness (RGB)", 2D) = "white" {}
		_NormalTex ("_NormalTex (RGB)", 2D) = "bump" {}
		 _ModifyTex ("ModifyTex (RGB)", 2D) = "black" {}
		[HideInInspector]_ScaleOffset("_UVScaleOffset", Vector) = (1,1,0,0)

	 
		 
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows  vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _GlossinessTex;
		sampler2D _NormalTex;
		sampler2D _ModifyTex;
	 
		struct Input {
			float2 uv_MainTex;
			float3 localPos	;
		};

	 
		fixed4 _Color;
		fixed4 _ScaleOffset;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END
			void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.localPos = v.vertex;
			//o.uv_MainTex = v.texcoord;
		 
			// if (o.modifyUV.x < 0)o.modifyUV.x *= -1;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			half2 modifyUV = half2(-IN.localPos.y / _ScaleOffset.x + _ScaleOffset.z, IN.localPos.z / _ScaleOffset.y + _ScaleOffset.w);
			  modifyUV.y += 0.5*max(0, sign(IN.localPos.x));
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 md = tex2D (_ModifyTex, modifyUV) ;
			fixed a =max( max(md.r , md.g) , md.b);
			o.Albedo = c.rgb*(1 -a) + md.rgb*a;
		//	o.Albedo = IN.modifyUV.x;
			// Metallic and smoothness come from slider variables
			 fixed3 msao = tex2D(_GlossinessTex, IN.uv_MainTex);
			o.Metallic = msao.r	;
			o.Smoothness = msao.g;
			o.Occlusion = msao.b;
			o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_MainTex));
			
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
