// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "Instanced/GpuGrassCS" 
{
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
    	}
		SubShader{
			Tags{
			"Queue" = "Geometry+200"
			"IgnoreProjector" = "True"
			"RenderType" = "Grass"
			"DisableBatching" = "True"
		}
		LOD 300
// cull off
		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
//#pragma exclude_renderers d3d11 gles
		// Physically based Standard lighting model
		#pragma surface surf Lambert  vertex:vert  addshadow
		#pragma multi_compile_instancing
		#pragma instancing_options procedural:setup
#include "UnityCG.cginc"
		sampler2D _MainTex;
		half4 lerpColors[16];
		half4 scales[8];
		 
		struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

		struct Input {
			float2 uv_MainTex;
			float2 offset;
			half3 color;
 
		};

 
		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		struct GrassInfo{
 
 half3 position;
  half4 params ;//  scaleWid,scaleHei,colorLerp,Layer
 
 };
  			 StructuredBuffer<GrassInfo> rectGrassBuffer;
 
  		#endif

	void setup()
	{
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

		/// Positions are calculated in the compute shader.
		/// here we just use them.
	 	GrassInfo info= rectGrassBuffer[unity_InstanceID];
  	 float3 position = info.position;
	 int layer = (int)(info.params.w + 0.5);
		
	// float2 scale = float2( info.params.x , info.params.y);
	 float2 scale = float2((scales[layer].x + scales[layer].y)*0.5, (scales[layer].z + scales[layer].w)*0.5);
	 position.y += scale.y*0.5;
		//  position=float3(-1793.878,unity_InstanceID*0.01+156,1041+5);
		//  scale = 1;
 //position.z=unity_InstanceID*1.3;

	// float3 forward = normalize(_WorldSpaceCameraPos - info,position);
	// o.pos = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_MV, float4(0, 0, 0, 1)) + float4(v.vertex.x, v.vertex.y, 0, 0));


  float3  dpos =  _WorldSpaceCameraPos- info.position;
 //half distance = length(dpos);
 //if (distance > 40) {
	// scale.x*= lerp(2, 9, (distance - 40)*0.00625);
 //}
 float rot =     atan2(-dpos.z, dpos.x) - 0.5*3.14159;
 
		unity_ObjectToWorld._11_21_31_41 = float4(scale.x*cos(rot), 0, -sin(rot), 0);
		unity_ObjectToWorld._12_22_32_42 = float4(0, scale.y, 0, 0);
		unity_ObjectToWorld._13_23_33_43 = float4(sin(rot), 0, cos(rot), 0);
		unity_ObjectToWorld._14_24_34_44 = float4(position.xyz,1);
	
	 
		unity_WorldToObject = unity_ObjectToWorld;
		unity_WorldToObject._14_24_34 *= -1;
		unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
#endif
	}
 

	void vert(inout appdata v,out Input o){
	UNITY_INITIALIZE_OUTPUT(Input,o);
	
		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
	 UNITY_SETUP_INSTANCE_ID(v);
 //	
 GrassInfo info= rectGrassBuffer[unity_InstanceID];
 int layer = (int)(info.params.w+0.5);
 half cl = info.params.z;
 o.offset.x= (layer%2) *0.5;
 o.offset.y=( 3-(layer/2)) *0.25;
 
 o.color = lerp(lerpColors[layer * 2 + 1].rgb / 2, lerpColors[layer * 2].rgb / 2, cl);
 v.normal = fixed3(0, 0.4, 0);
 
 //

	////waveing
	/* float2 center=	float2(-v.vertex.x,0.5);
	v.vertex.xy+=center;
 	float3x3 mat;
 
 	float r=sin(_Time.y)*3.14/8*(v.vertex.y);
 	mat._11_21_31=float3(cos(r),sin(r),0);
 	mat._12_22_32=float3(-sin(r),cos(r),0);
 	mat._13_23_33=float3(0,0,1);
 	v.vertex.xyz=mul(v.vertex.xyz,mat);
 	v.vertex.xy-=center;*/
 float3  dpos = _WorldSpaceCameraPos - info.position;

	/*float rotX =    atan2(-dpos.y, dpos.z) - 0.5*3.14159;
	half3x3 rotXMat;
	rotXMat._11_21_31 = float3(1, 0, 0);
	rotXMat._12_22_32  = float3(0,  cos(rotX), sin(rotX));
	rotXMat._13_23_33  = float3(0, -sin(rotX),  cos(rotX));
 	v.vertex.xyz = mul(v.vertex.xyz, rotXMat);*/

	 #endif
 
	}
 

	void surf(Input IN, inout SurfaceOutput o) 
	{
		 
		 
	 
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex + IN.offset);
	 

		o.Albedo = c.rgb*	   IN.color* c.a;
 
		o.Alpha = c.a;
		clip(c.a-0.5);
	}
	ENDCG
	}
	FallBack "Diffuse"
}