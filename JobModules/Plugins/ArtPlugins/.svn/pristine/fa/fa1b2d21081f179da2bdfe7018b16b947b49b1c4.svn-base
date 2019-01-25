// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "LightingBox/Terrain/Terrain 4-Layers"
{
	Properties
	{
		_SplatMap1("SplatMap1", 2D) = "white" {}
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 1
		_TessMin( "Tess Min Distance", Float ) = 30
		_TessMax( "Tess Max Distance", Float ) = 30
		_AlbedoMaps("AlbedoMaps", 2DArray) = "white" {}
		_Smoothness_1("Smoothness_1", Range( 0 , 1)) = 0.5
		_Smoothness_2("Smoothness_2", Range( 0 , 1)) = 0.5
		_Smoothness_3("Smoothness_3", Range( 0 , 1)) = 0.5
		_Smoothness_4("Smoothness_4", Range( 0 , 1)) = 0.5
		_Specular_Color_1("Specular_Color_1", Color) = (0,0,0,0)
		_Specular_Color_2("Specular_Color_2", Color) = (0,0,0,0)
		_Specular_Color_3("Specular_Color_3", Color) = (0,0,0,0)
		_Specular_Color_4("Specular_Color_4", Color) = (0,0,0,0)
		_Normal_Scale_1("Normal_Scale_1", Range( 0 , 1)) = 1
		_Normal_Scale_2("Normal_Scale_2", Range( 0 , 1)) = 1
		_Normal_Scale_3("Normal_Scale_3", Range( 0 , 1)) = 1
		_Normal_Scale_4("Normal_Scale_4", Range( 0 , 1)) = 1
		_Displacement_1("Displacement_1", Range( 0 , 3)) = 0
		_UV_0("UV_0", Vector) = (30,30,0,0)
		_UV_1("UV_1", Vector) = (30,30,0,0)
		_UV_2("UV_2", Vector) = (30,30,0,0)
		_UV_3("UV_3", Vector) = (30,30,0,0)
		_Displacement_2("Displacement_2", Range( 0 , 3)) = 0
		_Displacement_3("Displacement_3", Range( 0 , 3)) = 0
		_Displacement_4("Displacement_4", Range( 0 , 3)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "Tessellation.cginc"
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

		uniform sampler2D _SplatMap1;
		uniform float4 _SplatMap1_ST;
		uniform UNITY_DECLARE_TEX2DARRAY( _AlbedoMaps );
		uniform float4 _AlbedoMaps_ST;
		uniform float2 _UV_0;
		uniform float _Normal_Scale_1;
		uniform float2 _UV_1;
		uniform float _Normal_Scale_2;
		uniform float2 _UV_2;
		uniform float _Normal_Scale_3;
		uniform float2 _UV_3;
		uniform float _Normal_Scale_4;
		uniform float4 _Specular_Color_1;
		uniform float4 _Specular_Color_2;
		uniform float4 _Specular_Color_3;
		uniform float4 _Specular_Color_4;
		uniform float _Smoothness_1;
		uniform float _Smoothness_2;
		uniform float _Smoothness_3;
		uniform float _Smoothness_4;
		uniform float _Displacement_1;
		uniform float _Displacement_2;
		uniform float _Displacement_3;
		uniform float _Displacement_4;
		uniform float _TessValue;
		uniform float _TessMin;
		uniform float _TessMax;

		float4 tessFunction( appdata v0, appdata v1, appdata v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _TessValue );
		}

		void vertexDataFunc( inout appdata v )
		{
			float2 uv_SplatMap1 = v.texcoord * _SplatMap1_ST.xy + _SplatMap1_ST.zw;
			float4 tex2DNode9 = tex2Dlod( _SplatMap1, float4( uv_SplatMap1, 0, 0.0) );
			float2 uv_AlbedoMaps20 = v.texcoord;
			uv_AlbedoMaps20.xy = v.texcoord.xy * _AlbedoMaps_ST.xy + _AlbedoMaps_ST.zw;
			float4 texArray10 = UNITY_SAMPLE_TEX2DARRAY_LOD(_AlbedoMaps, float3(( uv_AlbedoMaps20 * _UV_0 ), 0.0), 0 );
			float4 texArray11 = UNITY_SAMPLE_TEX2DARRAY_LOD(_AlbedoMaps, float3(( uv_AlbedoMaps20 * _UV_1 ), 1.0), 0 );
			float4 texArray14 = UNITY_SAMPLE_TEX2DARRAY_LOD(_AlbedoMaps, float3(( uv_AlbedoMaps20 * _UV_2 ), 2.0), 0 );
			float4 texArray17 = UNITY_SAMPLE_TEX2DARRAY_LOD(_AlbedoMaps, float3(( uv_AlbedoMaps20 * _UV_3 ), 3.0), 0 );
			float4 weightedBlendVar123 = tex2DNode9;
			float weightedBlend123 = ( weightedBlendVar123.x*texArray10.a + weightedBlendVar123.y*texArray11.a + weightedBlendVar123.z*texArray14.a + weightedBlendVar123.w*texArray17.a );
			float4 weightedBlendVar129 = tex2DNode9;
			float weightedBlend129 = ( weightedBlendVar129.x*_Displacement_1 + weightedBlendVar129.y*_Displacement_2 + weightedBlendVar129.z*_Displacement_3 + weightedBlendVar129.w*_Displacement_4 );
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( ( weightedBlend123 * weightedBlend129 ) * ase_vertexNormal );
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_SplatMap1 = i.uv_texcoord * _SplatMap1_ST.xy + _SplatMap1_ST.zw;
			float4 tex2DNode9 = tex2D( _SplatMap1, uv_SplatMap1 );
			float2 uv_AlbedoMaps = i.uv_texcoord * _AlbedoMaps_ST.xy + _AlbedoMaps_ST.zw;
			float3 texArray200 = UnpackScaleNormal( UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_0 ), 4.0)  ) ,_Normal_Scale_1 );
			float3 texArray94 = UnpackScaleNormal( UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_1 ), 5.0)  ) ,_Normal_Scale_2 );
			float3 texArray95 = UnpackScaleNormal( UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_2 ), 6.0)  ) ,_Normal_Scale_3 );
			float3 texArray96 = UnpackScaleNormal( UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_3 ), 7.0)  ) ,_Normal_Scale_4 );
			float4 weightedBlendVar97 = tex2DNode9;
			float3 weightedBlend97 = ( weightedBlendVar97.x*texArray200 + weightedBlendVar97.y*texArray94 + weightedBlendVar97.z*texArray95 + weightedBlendVar97.w*texArray96 );
			o.Normal = weightedBlend97;
			float4 texArray10 = UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_0 ), 0.0)  );
			float4 texArray11 = UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_1 ), 1.0)  );
			float4 texArray14 = UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_2 ), 2.0)  );
			float4 texArray17 = UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_3 ), 3.0)  );
			float4 weightedBlendVar66 = tex2DNode9;
			float4 weightedBlend66 = ( weightedBlendVar66.x*texArray10 + weightedBlendVar66.y*texArray11 + weightedBlendVar66.z*texArray14 + weightedBlendVar66.w*texArray17 );
			o.Albedo = weightedBlend66.rgb;
			float4 weightedBlendVar90 = tex2DNode9;
			float4 weightedBlend90 = ( weightedBlendVar90.x*( _Specular_Color_1 * texArray10.g ) + weightedBlendVar90.y*( _Specular_Color_2 * texArray11.g ) + weightedBlendVar90.z*( _Specular_Color_3 * texArray14.g ) + weightedBlendVar90.w*( _Specular_Color_4 * texArray17.g ) );
			o.Specular = weightedBlend90.rgb;
			float4 weightedBlendVar75 = tex2DNode9;
			float weightedBlend75 = ( weightedBlendVar75.x*( _Smoothness_1 * texArray10.r ) + weightedBlendVar75.y*( _Smoothness_2 * texArray11.r ) + weightedBlendVar75.z*( _Smoothness_3 * texArray14.r ) + weightedBlendVar75.w*( _Smoothness_4 * texArray17.r ) );
			o.Smoothness = weightedBlend75;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13501
7;29;1010;692;-1321.27;2276.352;2.465547;True;False
Node;AmplifyShaderEditor.CommentaryNode;125;-2025.839,-1783.219;Float;False;1455.618;965.606;Albedo;10;10;14;17;11;33;47;32;46;6;3;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;3;-2005.124,-1726.299;Float;True;Property;_AlbedoMaps;AlbedoMaps;6;0;Assets/AmplifyShaderEditor/Examples/Official/TextureArray/WoodTextureArrayAlbedo.asset;False;white;LockedToTexture2DArray;0;1;SAMPLER2D
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1836.68,-1016.791;Float;False;0;3;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;202;-4043.795,-3007.813;Float;False;Property;_UV_0;UV_0;21;0;30,30;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;203;-4035.429,-2863.778;Float;False;Property;_UV_1;UV_1;22;0;30,30;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;205;-4035.427,-2559.931;Float;False;Property;_UV_3;UV_3;24;0;30,30;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;204;-4035.428,-2707.892;Float;False;Property;_UV_2;UV_2;23;0;30,30;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;130;478.0417,656.0351;Float;False;1271.142;491.2252;Displacement;8;112;113;115;129;114;128;126;127;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-1277.886,-1022.007;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-1291.37,-1667.703;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-1285.046,-1454.932;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-1280.181,-1232.558;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.TextureArrayNode;10;-981.0203,-1759.673;Float;True;Property;_AlbedoArray;AlbedoArray;7;0;None;0;Object;-1;Auto;False;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;128;535.4833,969.0829;Float;False;Property;_Displacement_4;Displacement_4;27;0;0;0;3;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;127;535.4833,881.0829;Float;False;Property;_Displacement_3;Displacement_3;26;0;0;0;3;0;1;FLOAT
Node;AmplifyShaderEditor.TextureArrayNode;11;-971.4817,-1543.736;Float;True;Property;_TextureArray1;Texture Array 1;3;0;None;0;Instance;10;Auto;False;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;1.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureArrayNode;14;-960.2587,-1326.269;Float;True;Property;_TextureArray2;Texture Array 2;4;0;None;0;Instance;10;Auto;False;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;2.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureArrayNode;17;-961.4342,-1120.175;Float;True;Property;_TextureArray3;Texture Array 3;5;0;None;0;Instance;10;Auto;False;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;3.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;135;-2154.518,-4246.142;Float;False;1188.032;1077.329;Comment;12;94;200;95;96;131;132;110;133;108;106;107;109;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;124;-1954.95,-109.6801;Float;False;897.999;741.8158;Specular;9;90;86;88;89;87;81;80;82;83;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;9;-4010.896,-1127.317;Float;True;Property;_SplatMap1;SplatMap1;0;0;Assets/Plugins/LandscapeAutoMaterial/Prefabs/Prefab Forest Terrain 2.asset;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;126;538.4833,794.0829;Float;False;Property;_Displacement_2;Displacement_2;25;0;0;0;3;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;114;541.3897,707.4896;Float;False;Property;_Displacement_1;Displacement_1;20;0;0;0;3;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;136;-1867.461,-2979.216;Float;False;1048.55;476.4971;Comment;9;75;78;77;76;79;74;73;72;71;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;132;-1739.269,-3599.948;Float;False;Property;_Normal_Scale_3;Normal_Scale_3;18;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;80;-1903.12,-51.77887;Float;False;Property;_Specular_Color_1;Specular_Color_1;12;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;131;-1743.931,-3791.053;Float;False;Property;_Normal_Scale_2;Normal_Scale_2;17;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-2006.082,-4061.81;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;72;-1810.119,-2825.099;Float;False;Property;_Smoothness_2;Smoothness_2;9;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SummedBlendNode;129;911.4785,852.512;Float;False;5;0;COLOR;0,0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;82;-1904.951,282.227;Float;False;Property;_Specular_Color_3;Specular_Color_3;14;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;73;-1810.119,-2721.098;Float;False;Property;_Smoothness_3;Smoothness_3;10;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;74;-1810.119,-2626.099;Float;False;Property;_Smoothness_4;Smoothness_4;11;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;81;-1903.951,122.2266;Float;False;Property;_Specular_Color_2;Specular_Color_2;13;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;83;-1901.951,443.2281;Float;False;Property;_Specular_Color_4;Specular_Color_4;15;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-2004.021,-3872.426;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;133;-1747.344,-3413.063;Float;False;Property;_Normal_Scale_4;Normal_Scale_4;19;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;108;-2007.722,-3679.644;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;-2010.165,-3488.397;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SummedBlendNode;123;1021.046,457.6179;Float;False;5;0;COLOR;0,0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;71;-1817.461,-2919.253;Float;False;Property;_Smoothness_1;Smoothness_1;8;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;110;-1746.957,-3980.619;Float;False;Property;_Normal_Scale_1;Normal_Scale_1;16;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.TextureArrayNode;94;-1383.499,-3901.142;Float;True;Property;_TextureArray0;Texture Array 0;11;0;None;0;Instance;10;Auto;True;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;5.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;-1554.256,-17.49036;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-1553.681,128.5852;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.TextureArrayNode;200;-1384.216,-4100.677;Float;True;Property;_NormallArray;NormallArray;42;0;None;0;Instance;10;Auto;True;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;4.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-1412.312,-2733.695;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;113;1329.35,708.1938;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-1412.183,-2603.296;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-1419.313,-2929.216;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-1419.313,-2829.074;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-1545.48,447.4471;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-1542.814,291.2885;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.TextureArrayNode;96;-1385.425,-3518.84;Float;True;Property;_TextureArray5;Texture Array 5;11;0;None;0;Instance;10;Auto;True;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;7.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureArrayNode;95;-1381.529,-3706.459;Float;True;Property;_TextureArray4;Texture Array 4;11;0;None;0;Instance;10;Auto;True;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;6.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.NormalVertexDataNode;115;1344.534,977.7064;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SummedBlendNode;90;-1305.961,216.4508;Float;False;5;0;COLOR;0,0,0,0;False;1;COLOR;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SummedBlendNode;75;-1007.892,-2826.422;Float;False;5;0;COLOR;0,0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SummedBlendNode;66;-202.7146,-1519.491;Float;False;5;0;COLOR;0,0,0,0;False;1;COLOR;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SummedBlendNode;97;-758.7121,-3840.461;Float;False;5;0;COLOR;0,0,0,0;False;1;FLOAT3;0;False;2;FLOAT3;0.0,0,0;False;3;FLOAT3;0.0,0,0;False;4;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;112;1580.184,766.9326;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3282.172,-1878.023;Float;False;True;6;Float;ASEMaterialInspector;0;0;StandardSpecular;LightingBox/Terrain/Terrain 4-Layers;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;True;0;1;30;30;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;1;0;0;0;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;2;3;0
WireConnection;47;0;6;0
WireConnection;47;1;205;0
WireConnection;32;0;6;0
WireConnection;32;1;202;0
WireConnection;33;0;6;0
WireConnection;33;1;203;0
WireConnection;46;0;6;0
WireConnection;46;1;204;0
WireConnection;10;6;3;0
WireConnection;10;0;32;0
WireConnection;11;0;33;0
WireConnection;14;0;46;0
WireConnection;17;0;47;0
WireConnection;106;0;6;0
WireConnection;106;1;202;0
WireConnection;129;0;9;0
WireConnection;129;1;114;0
WireConnection;129;2;126;0
WireConnection;129;3;127;0
WireConnection;129;4;128;0
WireConnection;107;0;6;0
WireConnection;107;1;203;0
WireConnection;108;0;6;0
WireConnection;108;1;204;0
WireConnection;109;0;6;0
WireConnection;109;1;205;0
WireConnection;123;0;9;0
WireConnection;123;1;10;4
WireConnection;123;2;11;4
WireConnection;123;3;14;4
WireConnection;123;4;17;4
WireConnection;94;0;107;0
WireConnection;94;3;131;0
WireConnection;86;0;80;0
WireConnection;86;1;10;2
WireConnection;87;0;81;0
WireConnection;87;1;11;2
WireConnection;200;0;106;0
WireConnection;200;3;110;0
WireConnection;78;0;73;0
WireConnection;78;1;14;1
WireConnection;113;0;123;0
WireConnection;113;1;129;0
WireConnection;79;0;74;0
WireConnection;79;1;17;1
WireConnection;76;0;71;0
WireConnection;76;1;10;1
WireConnection;77;0;72;0
WireConnection;77;1;11;1
WireConnection;89;0;83;0
WireConnection;89;1;17;2
WireConnection;88;0;82;0
WireConnection;88;1;14;2
WireConnection;96;0;109;0
WireConnection;96;3;133;0
WireConnection;95;0;108;0
WireConnection;95;3;132;0
WireConnection;90;0;9;0
WireConnection;90;1;86;0
WireConnection;90;2;87;0
WireConnection;90;3;88;0
WireConnection;90;4;89;0
WireConnection;75;0;9;0
WireConnection;75;1;76;0
WireConnection;75;2;77;0
WireConnection;75;3;78;0
WireConnection;75;4;79;0
WireConnection;66;0;9;0
WireConnection;66;1;10;0
WireConnection;66;2;11;0
WireConnection;66;3;14;0
WireConnection;66;4;17;0
WireConnection;97;0;9;0
WireConnection;97;1;200;0
WireConnection;97;2;94;0
WireConnection;97;3;95;0
WireConnection;97;4;96;0
WireConnection;112;0;113;0
WireConnection;112;1;115;0
WireConnection;0;0;66;0
WireConnection;0;1;97;0
WireConnection;0;3;90;0
WireConnection;0;4;75;0
WireConnection;0;11;112;0
ASEEND*/
//CHKSM=07B8CECC1D1BCC3510813F6215C91F9F391436D8