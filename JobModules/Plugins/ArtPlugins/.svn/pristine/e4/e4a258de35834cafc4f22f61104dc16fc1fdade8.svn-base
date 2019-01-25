// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/TerrainFormer/Grid" {
	Properties {
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	}

	SubShader {
		Tags {
			"Queue" = "Transparent" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent"
		}
	
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 
		Lighting Off

		Pass {  
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
				half2 texcoord : TEXCOORD0;
				float2 localPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata_t v) {
				v2f o;
				o.localPos = v.vertex.xy;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target {
				fixed4 colour = tex2D(_MainTex, i.texcoord);

				if(colour.a != 0) {
					colour.a = 1.0 - sqrt(pow(i.localPos.x, 2.0) + pow(i.localPos.y, 2.0)) * 2.0;
				}

				return colour;
			}
			ENDCG
		}
	}
}