// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "LightingBox/Water/Simple Water Tessellation"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 32
		_Color("Color", Color) = (0.7843137,0.8901961,0.7333333,1)
		_Smoothness("Smoothness", Range( 0 , 1)) = 1
		_NormalPower("Normal Power", Range( 0 , 1)) = 0.1
		_Specularity("Specularity", Range( 0 , 1)) = 0.554
		_SpecularColor("SpecularColor", Color) = (0.6235294,0.7058824,1,1)
		[Normal]_NormalMap("NormalMap", 2D) = "bump" {}
		_Wave("Wave", 2D) = "white" {}
		_Float0("Float 0", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 4.6
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float2 uv_texcoord;
		};

		struct appdata
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			float4 texcoord3 : TEXCOORD3;
			fixed4 color : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float4 _Color;
		uniform float _Specularity;
		uniform float4 _SpecularColor;
		uniform float _Smoothness;
		uniform sampler2D _Wave;
		uniform float4 _Wave_ST;
		uniform float _Float0;
		uniform float _TessValue;
		uniform float _NormalPower;
		float4 tessFunction( )
		{
			return _TessValue;
		}

		void vertexDataFunc( inout appdata v )
		{
			float3 ase_vertexNormal = v.normal.xyz;
			float2 uv_Wave20 = v.texcoord;
			uv_Wave20.xy = v.texcoord.xy * _Wave_ST.xy + _Wave_ST.zw;
			v.vertex.xyz += ( float4( ase_vertexNormal , 0.0 ) * ( tex2Dlod( _Wave, float4( (abs( uv_Wave20+_Time.x * float2(1,0 ))), 0.0 , 0.0 ) ) * _Float0 ) ).rgb;
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 tex2DNode4 = UnpackScaleNormal( tex2D( _NormalMap, (abs( uv_NormalMap+_Time.x * float2(1,1 ))) ) ,_NormalPower );
			o.Normal = tex2DNode4;
			o.Albedo = _Color.rgb;
			o.Specular = ( _Specularity + _SpecularColor ).rgb;
			o.Smoothness = ( ( tex2DNode4.r + 1.0 ) * _Smoothness );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13101
7;29;1010;692;938.1149;71.64065;2.633082;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-1184,-47;Float;False;0;4;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TimeNode;8;-1153,151;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;25;-431.2374,1055.814;Float;False;0;19;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TimeNode;24;-400.2373,1253.813;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;7;-891,102;Float;False;1;1;2;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.PannerNode;26;-138.2375,1204.813;Float;False;1;0;2;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;22;420.9991,1450.87;Float;False;Property;_Float0;Float 0;12;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;19;241.8785,1112.409;Float;True;Property;_Wave;Wave;11;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;16;-452.0726,627.6822;Float;False;Constant;_Float2;Float 2;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;4;-686.7966,182.4769;Float;True;Property;_NormalMap;NormalMap;10;1;[Normal];None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.1;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;20;742.9994,1098.106;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-263.6984,465.4626;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;587.1267,1321.66;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;5;-27.83771,655.7706;Float;False;Property;_Smoothness;Smoothness;7;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;6;339.4512,815.9659;Float;False;Property;_SpecularColor;SpecularColor;9;0;0.6235294,0.7058824,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;10;301.7835,699.4836;Float;False;Property;_Specularity;Specularity;8;0;0.554;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;915.2799,1436.514;Float;False;2;2;0;FLOAT3;0.0,0,0,0;False;1;COLOR;0.0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;14;943.8577,704.1173;Float;False;2;2;0;FLOAT;0.0,0,0,0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;3;-388,-65;Float;False;Property;_Color;Color;6;0;0.7843137,0.8901961,0.7333333,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;421.0958,360.0903;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1252.512,188.3273;Float;False;True;6;Float;ASEMaterialInspector;0;0;StandardSpecular;LightingBox/Simple Water Tesss;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;True;1;32;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;9;0
WireConnection;7;1;8;1
WireConnection;26;0;25;0
WireConnection;26;1;24;1
WireConnection;19;1;26;0
WireConnection;4;1;7;0
WireConnection;18;0;4;1
WireConnection;18;1;16;0
WireConnection;23;0;19;0
WireConnection;23;1;22;0
WireConnection;21;0;20;0
WireConnection;21;1;23;0
WireConnection;14;0;10;0
WireConnection;14;1;6;0
WireConnection;17;0;18;0
WireConnection;17;1;5;0
WireConnection;0;0;3;0
WireConnection;0;1;4;0
WireConnection;0;3;14;0
WireConnection;0;4;17;0
WireConnection;0;11;21;0
ASEEND*/
//CHKSM=0DD0438E5E4FA01771AFD896B9EE3F46F719A781