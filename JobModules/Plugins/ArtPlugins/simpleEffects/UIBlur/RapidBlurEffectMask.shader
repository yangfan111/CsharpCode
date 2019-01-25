Shader "SimpleEffects/RapidBlurEffectMask"
{
	Properties
	{
		 
	_MainTex("Base (RGB)", 2D) = "white" {}
	_OriginTex("Base (RGB)", 2D) = "white" {}
	_MaskRect1("apply rect", Vector) = (0,0,1,1)
	_MaskRect2("apply rect", Vector) = (0,0,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
		 
 			
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
			sampler2D _OriginTex;
			float4 _MainTex_ST;
			float4 _MaskRect1;
			float4 _MaskRect2;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
 				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
		 
				if (i.uv.x >= _MaskRect1.x&&i.uv.x <= _MaskRect1.z&&i.uv.y >= _MaskRect1.y&&i.uv.y <= _MaskRect1.w)
				return tex2D(_MainTex, i.uv);
				else {
					if (i.uv.x >= _MaskRect2.x&&i.uv.x <= _MaskRect2.z&&i.uv.y >= _MaskRect2.y&&i.uv.y <= _MaskRect2.w)
						return tex2D(_MainTex, i.uv);
					else
					// apply fog
					return tex2D(_OriginTex, i.uv);
					}
			}
			ENDCG
		}
	}
}
