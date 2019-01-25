// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "LightingBox/Water/Simple Water"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Color("Color", Color) = (0.7843137,0.8901961,0.7333333,1)
		_Smoothness("Smoothness", Range( 0 , 1)) = 1
		_NormalPower("Normal Power", Range( 0 , 1)) = 0.1
		_Metallic("Metallic", Range( 0 , 1)) = 0.554
		_MetallicColor("MetallicColor", Color) = (0.6235294,0.7058824,1,1)
		[Normal]_NormalMap("NormalMap", 2D) = "bump" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float4 _Color;
		uniform float _Metallic;
		uniform float4 _MetallicColor;
		uniform float _Smoothness;
		uniform float _NormalPower;
		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 tex2DNode4 = UnpackScaleNormal( tex2D( _NormalMap, (abs( uv_NormalMap+_Time.x * float2(1,1 ))) ) ,_NormalPower );
			o.Normal = tex2DNode4;
			o.Albedo = _Color.rgb;
			o.Metallic = ( _Metallic + _MetallicColor ).r;
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
7;29;1010;692;232.1828;-286.82;1;True;False
Node;AmplifyShaderEditor.TimeNode;8;-1153,151;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-1184,-47;Float;False;0;4;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;7;-891,102;Float;False;1;1;2;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;4;-686.7966,182.4769;Float;True;Property;_NormalMap;NormalMap;4;1;[Normal];None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.1;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;16;-452.0726,627.6822;Float;False;Constant;_Float2;Float 2;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;10;301.7835,699.4836;Float;False;Property;_Metallic;Metallic;2;0;0.554;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;5;-27.83771,655.7706;Float;False;Property;_Smoothness;Smoothness;1;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-263.6984,465.4626;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;6;339.4512,815.9659;Float;False;Property;_MetallicColor;MetallicColor;3;0;0.6235294,0.7058824,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;3;-388,-65;Float;False;Property;_Color;Color;0;0;0.7843137,0.8901961,0.7333333,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;14;943.8577,704.1173;Float;False;2;2;0;FLOAT;0.0,0,0,0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;421.0958,360.0903;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1252.512,188.3273;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;LightingBox/Water/Simple Water;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;9;0
WireConnection;7;1;8;1
WireConnection;4;1;7;0
WireConnection;18;0;4;1
WireConnection;18;1;16;0
WireConnection;14;0;10;0
WireConnection;14;1;6;0
WireConnection;17;0;18;0
WireConnection;17;1;5;0
WireConnection;0;0;3;0
WireConnection;0;1;4;0
WireConnection;0;3;14;0
WireConnection;0;4;17;0
ASEEND*/
//CHKSM=3DFA8A0BF02C5D788A77DFB95183B8D5DCB27E98