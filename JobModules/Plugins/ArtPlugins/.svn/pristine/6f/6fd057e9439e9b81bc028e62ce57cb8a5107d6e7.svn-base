Shader "Unlit/GrabPass"
{
	Properties
	{
		 
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"  "Queue"="Transparent" }
		LOD 100
		GrabPass{}
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
				float4 uv : TEXCOORD0;
			 
				float4 vertex : SV_POSITION;
			};

		 
			sampler2D _GrabTexture;
			 
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
 				o.uv = ComputeGrabScreenPos(o.vertex);
			 
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
 				fixed4 col = tex2Dproj(_GrabTexture, i.uv);
 				return col;
			}
			ENDCG
		}
	}
}
