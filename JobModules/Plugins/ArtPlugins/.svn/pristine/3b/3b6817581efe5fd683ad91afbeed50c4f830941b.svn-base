Shader "Standard(MSAO)ShadowFade" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
 		 _MetallicGlossMap ("Metallic (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Tranparent" "Queue"="Transparent" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert  alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		 sampler2D _MetallicGlossMap;
				sampler2D buffer0;
				sampler2D buffer4;
 
		struct Input {
			float2 uv_MainTex;
 float4 grabUV;

		};

  		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END
   void vert (inout appdata_full v, out Input o) {
                   UNITY_INITIALIZE_OUTPUT(Input,o);

            float4 hpos = UnityObjectToClipPos (v.vertex);
            o.grabUV = ComputeGrabScreenPos(hpos);
        }
		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			clip(c.a - 0.01);
						fixed4 colBg0 = tex2Dproj( buffer0, UNITY_PROJ_COORD(IN.grabUV));
fixed4 colBg4 = tex2Dproj( buffer4, UNITY_PROJ_COORD(IN.grabUV));
			//c.r=c.g=c.b=c.a=1;
			//c.r=1;c.b=0;c.g=0;
			o.Albedo = c.rgb/(colBg0.rgb/colBg4.rgb);
	 	half4 cm = tex2D(_MetallicGlossMap, IN.uv_MainTex) ;
			// Metallic and smoothness come from slider variables
			o.Metallic = cm.r;
			o.Smoothness =  cm.g;
			o.Occlusion = cm.b;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
