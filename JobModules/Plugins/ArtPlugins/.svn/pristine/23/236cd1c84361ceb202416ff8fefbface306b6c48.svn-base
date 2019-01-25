// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/TerrainFormer/BrushPlaneTop" {
	Properties {
		_Color("Main Color", Color) = (0.2, 0.7, 1.0, 0.7)
		_MainTex("Main Texture", 2D) = "white" {}
		_OutlineTex("Outline", 2D) = "" {}
	}

	SubShader {
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Pass {
			Cull Off Lighting Off ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			float4 _MainTex_ST;
			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _OutlineTex;

			v2f vert (appdata_t v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target {
				fixed4 colour = tex2D(_MainTex, i.texcoord);
				colour.a += tex2D(_OutlineTex, i.texcoord).a;
				colour.a *= _Color.a;
				colour.rgb = _Color.rgb;
				return colour;
			}
			ENDCG 
		}
	}
}