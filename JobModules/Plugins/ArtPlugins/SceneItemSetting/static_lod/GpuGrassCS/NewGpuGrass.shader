// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/NewGpuGrass"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader{
		Tags{ "LightMode" = "ForwardBase" }
		cull off
		Pass{
		CGPROGRAM
#include "UnityCG.cginc"
#pragma target 5.0  
#pragma vertex vertex_shader
#pragma fragment fragment_shader
#pragma multi_compile_instancing
#pragma instancing_options procedural:setup

		sampler2D _MainTex;
	float4 _MainTex_ST;
	uniform fixed4 _LightColor0;

	struct Point {
		float3         vertex;
		float3         normal;
		float4         tangent;
		float2 uv;
	};
 
	struct GrassInfo {

		half3 position;
		half params;//scale:(0,10) ,colorLer:(10,100) ,layer:(100,1000) :: layer*100+colorLer*10+scale

	};
	StructuredBuffer<GrassInfo> rectGrassBuffer;
	StructuredBuffer<int> renderIDsBuffer;
	StructuredBuffer<Point> points;

 
	

	struct v2f {
		float4 pos : SV_POSITION;
		float4 col : COLOR;
		float2 uv : TEXCOORD0;
	};
 
	void setup()
	{
 	//	/// Positions are calculated in the compute shader.
		///// here we just use them.
		  //GrassInfo info = rectGrassBuffer[renderIDsBuffer[unity_InstanceID]];
		 //float3 position = info.position;


		//float scale = info.params % 10;


		////  position=float3(-1793.878,unity_InstanceID*0.01+156,1041+5);
		////  scale = 1;
		////position.z=unity_InstanceID*1.3;
		//unity_ObjectToWorld._11_21_31_41 = float4(scale, 0, 0, 0);
		//unity_ObjectToWorld._12_22_32_42 = float4(0, scale, 0, 0);
		//unity_ObjectToWorld._13_23_33_43 = float4(0, 0, scale, 0);
		//unity_ObjectToWorld._14_24_34_44 = float4(position.xyz, 1);
		//unity_WorldToObject = unity_ObjectToWorld;
		//unity_WorldToObject._14_24_34 *= -1;
		//unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
 
	}
	v2f vertex_shader(uint id : SV_VertexID, uint inst : SV_InstanceID)
	{
		v2f o;
		GrassInfo info = rectGrassBuffer[renderIDsBuffer[inst]];
		float3 position = info.position;
		float4 vertex_position = float4(points[id].vertex + position, 1.0f);
		float4 vertex_normal = float4(points[id].normal, 1.0f);
		//vertex_position.x += sin(5.0*_Time.g);
		o.pos = UnityObjectToClipPos(vertex_position);
		//o.pos.xyz+=position;
		o.uv = TRANSFORM_TEX(points[id].uv, _MainTex);
		float3 normalDirection = normalize(vertex_normal.xyz);
		float4 AmbientLight = UNITY_LIGHTMODEL_AMBIENT;
		float4 LightDirection = normalize(_WorldSpaceLightPos0);
		float4 DiffuseLight = saturate(dot(LightDirection, normalDirection))*_LightColor0;
		o.col = float4(AmbientLight + DiffuseLight);
		//o.col.a = 1;
		return o;
	}
	//struct appdata
	//{
	//	float4 vertex : POSITION;
	//	float2 uv : TEXCOORD0;
	//	float3 normal :NORMAL;
	//	UNITY_VERTEX_INPUT_INSTANCE_ID
	//};
	//v2f vertex_shader(appdata v)
	//{
	//	v2f o;
	//	UNITY_SETUP_INSTANCE_ID(v);
	//	 GrassInfo info = rectGrassBuffer[renderIDsBuffer[unity_InstanceID]];
	//	 float3 position = 0;
	//	// position.z = 1;
	//	 position.y =   info.position.x;
	//	 
	//	//vertex_position.x += sin(5.0*_Time.g);
	//	 
	//	o.pos = UnityObjectToClipPos(v.vertex) ;
	//	o.pos.xyz += position;
	//	o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	//	float3 normalDirection = normalize(v.normal.xyz);
	//	float4 AmbientLight = UNITY_LIGHTMODEL_AMBIENT;
	//	float4 LightDirection = normalize(_WorldSpaceLightPos0);
	//	float4 DiffuseLight = saturate(dot(LightDirection, normalDirection))*_LightColor0;
	//	o.col = float4(AmbientLight + DiffuseLight);
	//	//o.col.a = 1;
	//	return o;
	//}

	fixed4 fragment_shader(v2f i) : SV_Target
	{
		fixed4 final = tex2D(_MainTex, i.uv);
	final *= i.col;
	 
	clip(final.a - 0.8);
	return final;
	}

		ENDCG
	}
	}
}