// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/AdditiveMatrix"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	    _Color("color", Color) =  (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
			blend one one
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
 			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
 				float4 vertex : SV_POSITION;
  
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _Color;
			float4x4 _transMt;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(mul(_transMt, v.vertex));
				 o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex,i.uv)*_Color;
	 
		 
				return col;
			}
			ENDCG
		}
	}
}
