// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "LightingBox/Terrain/Terrain 6-Layers"
{
	Properties
	{
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 10
		_TessMin( "Tess Min Distance", Float ) = 30
		_TessMax( "Tess Max Distance", Float ) = 100
		_SplatMap1("SplatMap1", 2D) = "white" {}
		_SplatMap2("SplatMap2", 2D) = "white" {}
		_AlbedoMaps("AlbedoMaps", 2DArray) = "white" {}
		_Smoothness_1("Smoothness_1", Range( 0 , 1)) = 0.5
		_Smoothness_2("Smoothness_2", Range( 0 , 1)) = 0.5
		_Smoothness_3("Smoothness_3", Range( 0 , 1)) = 0.5
		_Smoothness_4("Smoothness_4", Range( 0 , 1)) = 0.5
		_Smoothness_5("Smoothness_5", Range( 0 , 1)) = 0.5
		_Smoothness_6("Smoothness_6", Range( 0 , 1)) = 0.5
		_Specular_Color_1("Specular_Color_1", Color) = (0,0,0,0)
		_Specular_Color_2("Specular_Color_2", Color) = (0,0,0,0)
		_Specular_Color_3("Specular_Color_3", Color) = (0,0,0,0)
		_Specular_Color_4("Specular_Color_4", Color) = (0,0,0,0)
		_Specular_Color_5("Specular_Color_5", Color) = (0,0,0,0)
		_Specular_Color_6("Specular_Color_6", Color) = (0,0,0,0)
		_UV_4("UV_4", Vector) = (30,30,0,0)
		_UV_0("UV_0", Vector) = (30,30,0,0)
		_Normal_Scale_1("Normal_Scale_1", Range( 0 , 1)) = 1
		_UV_5("UV_5", Vector) = (30,30,0,0)
		_UV_1("UV_1", Vector) = (30,30,0,0)
		_Normal_Scale_2("Normal_Scale_2", Range( 0 , 1)) = 1
		_UV_2("UV_2", Vector) = (30,30,0,0)
		_Normal_Scale_3("Normal_Scale_3", Range( 0 , 1)) = 1
		_UV_3("UV_3", Vector) = (30,30,0,0)
		_Normal_Scale_4("Normal_Scale_4", Range( 0 , 1)) = 1
		_Normal_Scale_5("Normal_Scale_5", Range( 0 , 1)) = 1
		_Normal_Scale_6("Normal_Scale_6", Range( 0 , 1)) = 0
		_Displacement_1("Displacement_1", Range( 0 , 3)) = 0
		_Displacement_2("Displacement_2", Range( 0 , 3)) = 0
		_Displacement_3("Displacement_3", Range( 0 , 3)) = 0
		_Displacement_4("Displacement_4", Range( 0 , 3)) = 0
		_Displacement_5("Displacement_5", Range( 0 , 3)) = 0
		_Displacement_6("Displacement_6", Range( 0 , 3)) = 0
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
		uniform sampler2D _SplatMap2;
		uniform float4 _SplatMap2_ST;
		uniform float2 _UV_4;
		uniform float _Normal_Scale_5;
		uniform float2 _UV_5;
		uniform float _Normal_Scale_6;
		uniform float4 _Specular_Color_1;
		uniform float4 _Specular_Color_2;
		uniform float4 _Specular_Color_3;
		uniform float4 _Specular_Color_4;
		uniform float4 _Specular_Color_5;
		uniform float4 _Specular_Color_6;
		uniform float _Smoothness_1;
		uniform float _Smoothness_2;
		uniform float _Smoothness_3;
		uniform float _Smoothness_4;
		uniform float _Smoothness_5;
		uniform float _Smoothness_6;
		uniform float _Displacement_1;
		uniform float _Displacement_2;
		uniform float _Displacement_3;
		uniform float _Displacement_4;
		uniform float _Displacement_5;
		uniform float _Displacement_6;
		uniform float _TessValue;
		uniform float _TessMin;
		uniform float _TessMax;

		float4 tessFunction( appdata v0, appdata v1, appdata v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _TessValue );
		}

		void vertexDataFunc( inout appdata v )
		{
			float2 uv_SplatMap2 = v.texcoord * _SplatMap2_ST.xy + _SplatMap2_ST.zw;
			float4 tex2DNode138 = tex2Dlod( _SplatMap2, float4( uv_SplatMap2, 0, 0.0) );
			float2 uv_AlbedoMaps20 = v.texcoord;
			uv_AlbedoMaps20.xy = v.texcoord.xy * _AlbedoMaps_ST.xy + _AlbedoMaps_ST.zw;
			float4 texArray171 = UNITY_SAMPLE_TEX2DARRAY_LOD(_AlbedoMaps, float3(( uv_AlbedoMaps20 * _UV_4 ), 4.0), 0 );
			float4 texArray172 = UNITY_SAMPLE_TEX2DARRAY_LOD(_AlbedoMaps, float3(( uv_AlbedoMaps20 * _UV_5 ), 5.0), 0 );
			float4 weightedBlendVar197 = tex2DNode138;
			float weightedBlend197 = ( weightedBlendVar197.x*texArray171.a + weightedBlendVar197.y*texArray172.a + weightedBlendVar197.z*0.0 + weightedBlendVar197.w*0.0 );
			float2 uv_SplatMap1 = v.texcoord * _SplatMap1_ST.xy + _SplatMap1_ST.zw;
			float4 tex2DNode9 = tex2Dlod( _SplatMap1, float4( uv_SplatMap1, 0, 0.0) );
			float4 texArray10 = UNITY_SAMPLE_TEX2DARRAY_LOD(_AlbedoMaps, float3(( uv_AlbedoMaps20 * _UV_0 ), 0.0), 0 );
			float4 texArray11 = UNITY_SAMPLE_TEX2DARRAY_LOD(_AlbedoMaps, float3(( uv_AlbedoMaps20 * _UV_1 ), 1.0), 0 );
			float4 texArray14 = UNITY_SAMPLE_TEX2DARRAY_LOD(_AlbedoMaps, float3(( uv_AlbedoMaps20 * _UV_2 ), 2.0), 0 );
			float4 texArray17 = UNITY_SAMPLE_TEX2DARRAY_LOD(_AlbedoMaps, float3(( uv_AlbedoMaps20 * _UV_3 ), 3.0), 0 );
			float4 weightedBlendVar123 = tex2DNode9;
			float weightedBlend123 = ( weightedBlendVar123.x*texArray10.a + weightedBlendVar123.y*texArray11.a + weightedBlendVar123.z*texArray14.a + weightedBlendVar123.w*texArray17.a );
			float4 weightedBlendVar129 = tex2DNode9;
			float weightedBlend129 = ( weightedBlendVar129.x*_Displacement_1 + weightedBlendVar129.y*_Displacement_2 + weightedBlendVar129.z*_Displacement_3 + weightedBlendVar129.w*_Displacement_4 );
			float4 weightedBlendVar195 = tex2DNode138;
			float weightedBlend195 = ( weightedBlendVar195.x*_Displacement_5 + weightedBlendVar195.y*_Displacement_6 + weightedBlendVar195.z*0.0 + weightedBlendVar195.w*0.0 );
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( ( ( weightedBlend197 + weightedBlend123 ) * ( weightedBlend129 + weightedBlend195 ) ) * ase_vertexNormal );
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_SplatMap1 = i.uv_texcoord * _SplatMap1_ST.xy + _SplatMap1_ST.zw;
			float4 tex2DNode9 = tex2D( _SplatMap1, uv_SplatMap1 );
			float2 uv_AlbedoMaps = i.uv_texcoord * _AlbedoMaps_ST.xy + _AlbedoMaps_ST.zw;
			float3 texArray200 = UnpackScaleNormal( UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_0 ), 8.0)  ) ,_Normal_Scale_1 );
			float3 texArray94 = UnpackScaleNormal( UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_1 ), 9.0)  ) ,_Normal_Scale_2 );
			float3 texArray95 = UnpackScaleNormal( UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_2 ), 10.0)  ) ,_Normal_Scale_3 );
			float3 texArray96 = UnpackScaleNormal( UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_3 ), 11.0)  ) ,_Normal_Scale_4 );
			float4 weightedBlendVar97 = tex2DNode9;
			float3 weightedBlend97 = ( weightedBlendVar97.x*texArray200 + weightedBlendVar97.y*texArray94 + weightedBlendVar97.z*texArray95 + weightedBlendVar97.w*texArray96 );
			float2 uv_SplatMap2 = i.uv_texcoord * _SplatMap2_ST.xy + _SplatMap2_ST.zw;
			float4 tex2DNode138 = tex2D( _SplatMap2, uv_SplatMap2 );
			float3 texArray141 = UnpackScaleNormal( UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_4 ), 12.0)  ) ,_Normal_Scale_5 );
			float3 texArray144 = UnpackScaleNormal( UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_5 ), 13.0)  ) ,_Normal_Scale_6 );
			float4 weightedBlendVar159 = tex2DNode138;
			float3 weightedBlend159 = ( weightedBlendVar159.x*texArray141 + weightedBlendVar159.y*texArray144 + weightedBlendVar159.z*float3( 0.0,0,0 ) + weightedBlendVar159.w*float3( 0.0,0,0 ) );
			o.Normal = ( weightedBlend97 + weightedBlend159 );
			float4 texArray10 = UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_0 ), 0.0)  );
			float4 texArray11 = UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_1 ), 1.0)  );
			float4 texArray14 = UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_2 ), 2.0)  );
			float4 texArray17 = UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_3 ), 3.0)  );
			float4 weightedBlendVar66 = tex2DNode9;
			float4 weightedBlend66 = ( weightedBlendVar66.x*texArray10 + weightedBlendVar66.y*texArray11 + weightedBlendVar66.z*texArray14 + weightedBlendVar66.w*texArray17 );
			float4 texArray171 = UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_4 ), 4.0)  );
			float4 texArray172 = UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(( uv_AlbedoMaps * _UV_5 ), 5.0)  );
			float4 weightedBlendVar179 = tex2DNode138;
			float4 weightedBlend179 = ( weightedBlendVar179.x*texArray171 + weightedBlendVar179.y*texArray172 + weightedBlendVar179.z*float4( 0,0,0,0 ) + weightedBlendVar179.w*float4( 0,0,0,0 ) );
			o.Albedo = ( weightedBlend66 + weightedBlend179 ).rgb;
			float4 weightedBlendVar90 = tex2DNode9;
			float4 weightedBlend90 = ( weightedBlendVar90.x*( _Specular_Color_1 * texArray10.g ) + weightedBlendVar90.y*( _Specular_Color_2 * texArray11.g ) + weightedBlendVar90.z*( _Specular_Color_3 * texArray14.g ) + weightedBlendVar90.w*( _Specular_Color_4 * texArray17.g ) );
			float4 weightedBlendVar189 = tex2DNode138;
			float4 weightedBlend189 = ( weightedBlendVar189.x*( _Specular_Color_5 * texArray171.g ) + weightedBlendVar189.y*( _Specular_Color_6 * texArray172.g ) + weightedBlendVar189.z*float4( 0,0,0,0 ) + weightedBlendVar189.w*float4( 0,0,0,0 ) );
			o.Specular = ( weightedBlend90 + weightedBlend189 ).rgb;
			float4 weightedBlendVar75 = tex2DNode9;
			float weightedBlend75 = ( weightedBlendVar75.x*( _Smoothness_1 * texArray10.r ) + weightedBlendVar75.y*( _Smoothness_2 * texArray11.r ) + weightedBlendVar75.z*( _Smoothness_3 * texArray14.r ) + weightedBlendVar75.w*( _Smoothness_4 * texArray17.r ) );
			float4 weightedBlendVar161 = tex2DNode138;
			float weightedBlend161 = ( weightedBlendVar161.x*( _Smoothness_5 * texArray171.r ) + weightedBlendVar161.y*( _Smoothness_6 * texArray172.r ) + weightedBlendVar161.z*0.0 + weightedBlendVar161.w*0.0 );
			o.Smoothness = ( weightedBlend75 + weightedBlend161 );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13501
7;29;1010;692;6236.835;1580.396;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;125;-2025.839,-1783.219;Float;False;1500.728;1727.471;Albedo;14;176;175;172;171;47;46;32;33;17;14;11;10;3;6;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;3;-2005.124,-1726.299;Float;True;Property;_AlbedoMaps;AlbedoMaps;7;0;Assets/AmplifyShaderEditor/Examples/Official/TextureArray/WoodTextureArrayAlbedo.asset;False;white;LockedToTexture2DArray;0;1;SAMPLER2D
Node;AmplifyShaderEditor.Vector2Node;207;-5842.818,-1524.824;Float;False;Property;_UV_4;UV_4;21;0;30,30;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;201;-5847.84,-1675.636;Float;False;Property;_UV_3;UV_3;29;0;30,30;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1836.68,-1016.791;Float;False;0;3;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;208;-5840.255,-1374.986;Float;False;Property;_UV_5;UV_5;24;0;30,30;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;202;-5847.841,-1823.597;Float;False;Property;_UV_2;UV_2;27;0;30,30;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;204;-5847.842,-1979.483;Float;False;Property;_UV_1;UV_1;25;0;30,30;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;203;-5856.208,-2123.517;Float;False;Property;_UV_0;UV_0;22;0;30,30;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-1275.999,-1518.258;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;175;-1246.81,-712.723;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.CommentaryNode;130;478.0417,656.0351;Float;False;1271.142;776.9246;Displacement;11;195;194;192;129;126;128;114;127;112;115;113;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;176;-1294.196,-915.4965;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-1290.239,-1666.571;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-1279.017,-1123.781;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-1272.265,-1317.37;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;194;540.1483,1144.367;Float;False;Property;_Displacement_6;Displacement_6;38;0;0;0;3;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;124;-3728.566,326.5414;Float;False;889.2822;1491.483;Specular;14;189;187;186;182;181;90;88;86;87;89;82;83;81;80;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;126;538.4833,794.0829;Float;False;Property;_Displacement_2;Displacement_2;34;0;0;0;3;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;136;-1867.461,-2979.216;Float;False;1068.033;924.5947;Comment;14;169;168;165;163;161;76;77;78;71;74;72;73;75;79;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;127;535.4833,881.0829;Float;False;Property;_Displacement_3;Displacement_3;35;0;0;0;3;0;1;FLOAT
Node;AmplifyShaderEditor.TextureArrayNode;11;-971.4817,-1543.736;Float;True;Property;_TextureArray1;Texture Array 1;3;0;None;0;Instance;10;Auto;False;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;1.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureArrayNode;171;-955.6483,-895.368;Float;True;Property;_TextureArray10;Texture Array 10;8;0;None;0;Instance;10;Auto;False;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;4.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;138;-5893.803,-477.8911;Float;True;Property;_SplatMap2;SplatMap2;6;0;Assets/Plugins/LandscapeAutoMaterial/Prefabs/Prefab Forest Terrain 2.asset;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureArrayNode;14;-960.2587,-1326.269;Float;True;Property;_TextureArray2;Texture Array 2;4;0;None;0;Instance;10;Auto;False;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;2.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureArrayNode;10;-981.0203,-1759.673;Float;True;Property;_AlbedoArray;AlbedoArray;8;0;None;0;Object;-1;Auto;False;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;114;541.3897,707.4896;Float;False;Property;_Displacement_1;Displacement_1;33;0;0;0;3;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;135;-3864.35,-4702.2;Float;False;1820.084;1642.141;Comment;18;144;143;142;141;140;139;96;133;109;95;94;107;131;108;132;110;106;200;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureArrayNode;17;-961.4342,-1120.175;Float;True;Property;_TextureArray3;Texture Array 3;5;0;None;0;Instance;10;Auto;False;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;3.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;192;543.0546,1057.773;Float;False;Property;_Displacement_5;Displacement_5;37;0;0;0;3;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;9;-5900.493,-692.1379;Float;True;Property;_SplatMap1;SplatMap1;5;0;Assets/Plugins/LandscapeAutoMaterial/Prefabs/Prefab Forest Terrain 2.asset;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;128;535.4833,969.0829;Float;False;Property;_Displacement_4;Displacement_4;36;0;0;0;3;0;1;FLOAT
Node;AmplifyShaderEditor.TextureArrayNode;172;-945.9139,-711.8343;Float;True;Property;_TextureArray11;Texture Array 11;3;0;None;0;Instance;10;Auto;False;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;5.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;182;-3665.243,1283.632;Float;False;Property;_Specular_Color_6;Specular_Color_6;20;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;143;-2745.229,-3574.727;Float;False;Property;_Normal_Scale_6;Normal_Scale_6;32;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;139;-3009.528,-3847.268;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.ColorNode;83;-3675.567,879.4498;Float;False;Property;_Specular_Color_4;Specular_Color_4;18;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SummedBlendNode;129;911.4785,852.512;Float;False;5;0;COLOR;0,0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;108;-3011.54,-4236.562;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-3005.901,-4621.729;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SummedBlendNode;195;906.2344,1081.32;Float;False;5;0;COLOR;0,0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;131;-2747.749,-4347.972;Float;False;Property;_Normal_Scale_2;Normal_Scale_2;26;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;110;-2750.775,-4537.538;Float;False;Property;_Normal_Scale_1;Normal_Scale_1;23;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;133;-2751.162,-3969.981;Float;False;Property;_Normal_Scale_4;Normal_Scale_4;30;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;132;-2743.087,-4156.866;Float;False;Property;_Normal_Scale_3;Normal_Scale_3;28;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;74;-1810.119,-2626.099;Float;False;Property;_Smoothness_4;Smoothness_4;12;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;71;-1817.461,-2919.253;Float;False;Property;_Smoothness_1;Smoothness_1;9;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;165;-1804.511,-2473.179;Float;False;Property;_Smoothness_5;Smoothness_5;13;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SummedBlendNode;197;-76.87102,-381.3903;Float;False;5;0;COLOR;0,0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;181;-3662.67,1113.327;Float;False;Property;_Specular_Color_5;Specular_Color_5;19;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;72;-1810.119,-2825.099;Float;False;Property;_Smoothness_2;Smoothness_2;10;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SummedBlendNode;123;-88.0489,-668.4572;Float;False;5;0;COLOR;0,0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;80;-3676.735,384.4427;Float;False;Property;_Specular_Color_1;Specular_Color_1;15;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;163;-1797.169,-2379.025;Float;False;Property;_Smoothness_6;Smoothness_6;14;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-3007.839,-4429.345;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;-3013.984,-4045.315;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;140;-2744.24,-3769.399;Float;False;Property;_Normal_Scale_5;Normal_Scale_5;31;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;142;-3012.744,-3660.013;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;73;-1810.119,-2721.098;Float;False;Property;_Smoothness_3;Smoothness_3;11;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;82;-3678.567,718.4485;Float;False;Property;_Specular_Color_3;Specular_Color_3;17;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;81;-3677.567,558.4481;Float;False;Property;_Specular_Color_2;Specular_Color_2;16;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureArrayNode;96;-2389.242,-4075.758;Float;True;Property;_TextureArray5;Texture Array 5;11;0;None;0;Instance;10;Auto;True;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;11.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-1419.313,-2829.074;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-1412.183,-2603.296;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;169;-1405.044,-2487.096;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;196;1111.285,969.7888;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;168;-1406.362,-2383.001;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-1412.312,-2733.695;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-1419.313,-2929.216;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-3316.429,727.5101;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-3312.796,1293.454;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.TextureArrayNode;94;-2387.316,-4458.061;Float;True;Property;_TextureArray0;Texture Array 0;11;0;None;0;Instance;10;Auto;True;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;9.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureArrayNode;200;-2387.033,-4657.596;Float;True;Property;_NormallArray;NormallArray;42;0;None;0;Instance;10;Auto;True;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;8.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-3319.095,883.6687;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.TextureArrayNode;95;-2385.346,-4263.377;Float;True;Property;_TextureArray4;Texture Array 4;11;0;None;0;Instance;10;Auto;True;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;10.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureArrayNode;144;-2401.606,-3687.552;Float;True;Property;_TextureArray7;Texture Array 7;11;0;None;0;Instance;10;Auto;True;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;13.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureArrayNode;141;-2396.909,-3877.723;Float;True;Property;_TextureArray6;Texture Array 6;11;0;None;0;Instance;10;Auto;True;7;6;SAMPLER2D;;False;0;FLOAT2;0,0;False;1;FLOAT;12.0;False;2;FLOAT;0.0;False;3;FLOAT;1.0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-3327.296,564.8068;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;198;242.1236,-544.5306;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;-3327.871,418.7313;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;187;-3319.373,1107.393;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SummedBlendNode;159;-1263.048,-3775.889;Float;False;5;0;COLOR;0,0,0,0;False;1;FLOAT3;0;False;2;FLOAT3;0.0,0,0;False;3;FLOAT3;0.0,0,0;False;4;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SummedBlendNode;179;-128.6516,-944.6517;Float;False;5;0;COLOR;0,0,0,0;False;1;COLOR;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SummedBlendNode;75;-1007.892,-2826.422;Float;False;5;0;COLOR;0,0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SummedBlendNode;66;-138.0398,-1255.402;Float;False;5;0;COLOR;0,0,0,0;False;1;COLOR;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SummedBlendNode;90;-3053.426,643.9554;Float;False;5;0;COLOR;0,0,0,0;False;1;COLOR;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SummedBlendNode;161;-1008.632,-2399.23;Float;False;5;0;COLOR;0,0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SummedBlendNode;97;-1217.887,-4309.968;Float;False;5;0;COLOR;0,0,0,0;False;1;FLOAT3;0;False;2;FLOAT3;0.0,0,0;False;3;FLOAT3;0.0,0,0;False;4;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.NormalVertexDataNode;115;1344.534,977.7064;Float;False;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SummedBlendNode;189;-3057.549,1308.791;Float;False;5;0;COLOR;0,0,0,0;False;1;COLOR;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;113;1329.35,708.1938;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;160;-189.9032,-3942.589;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;170;-668.9998,-2542.664;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;190;-387.7433,731.7554;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;112;1580.184,766.9326;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;180;142.0839,-1034.604;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1995.239,-749.6547;Float;False;True;6;Float;ASEMaterialInspector;0;0;StandardSpecular;LightingBox/Terrain/Terrain 6-Layers;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;True;0;10;30;100;False;1;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;0;0;0;0;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;2;3;0
WireConnection;33;0;6;0
WireConnection;33;1;204;0
WireConnection;175;0;6;0
WireConnection;175;1;208;0
WireConnection;176;0;6;0
WireConnection;176;1;207;0
WireConnection;32;0;6;0
WireConnection;32;1;203;0
WireConnection;47;0;6;0
WireConnection;47;1;201;0
WireConnection;46;0;6;0
WireConnection;46;1;202;0
WireConnection;11;0;33;0
WireConnection;171;0;176;0
WireConnection;14;0;46;0
WireConnection;10;6;3;0
WireConnection;10;0;32;0
WireConnection;17;0;47;0
WireConnection;172;0;175;0
WireConnection;139;0;6;0
WireConnection;139;1;207;0
WireConnection;129;0;9;0
WireConnection;129;1;114;0
WireConnection;129;2;126;0
WireConnection;129;3;127;0
WireConnection;129;4;128;0
WireConnection;108;0;6;0
WireConnection;108;1;202;0
WireConnection;106;0;6;0
WireConnection;106;1;203;0
WireConnection;195;0;138;0
WireConnection;195;1;192;0
WireConnection;195;2;194;0
WireConnection;197;0;138;0
WireConnection;197;1;171;4
WireConnection;197;2;172;4
WireConnection;123;0;9;0
WireConnection;123;1;10;4
WireConnection;123;2;11;4
WireConnection;123;3;14;4
WireConnection;123;4;17;4
WireConnection;107;0;6;0
WireConnection;107;1;204;0
WireConnection;109;0;6;0
WireConnection;109;1;201;0
WireConnection;142;0;6;0
WireConnection;142;1;208;0
WireConnection;96;0;109;0
WireConnection;96;3;133;0
WireConnection;77;0;72;0
WireConnection;77;1;11;1
WireConnection;79;0;74;0
WireConnection;79;1;17;1
WireConnection;169;0;165;0
WireConnection;169;1;171;1
WireConnection;196;0;129;0
WireConnection;196;1;195;0
WireConnection;168;0;163;0
WireConnection;168;1;172;1
WireConnection;78;0;73;0
WireConnection;78;1;14;1
WireConnection;76;0;71;0
WireConnection;76;1;10;1
WireConnection;88;0;82;0
WireConnection;88;1;14;2
WireConnection;186;0;182;0
WireConnection;186;1;172;2
WireConnection;94;0;107;0
WireConnection;94;3;131;0
WireConnection;200;0;106;0
WireConnection;200;3;110;0
WireConnection;89;0;83;0
WireConnection;89;1;17;2
WireConnection;95;0;108;0
WireConnection;95;3;132;0
WireConnection;144;0;142;0
WireConnection;144;3;143;0
WireConnection;141;0;139;0
WireConnection;141;3;140;0
WireConnection;87;0;81;0
WireConnection;87;1;11;2
WireConnection;198;0;197;0
WireConnection;198;1;123;0
WireConnection;86;0;80;0
WireConnection;86;1;10;2
WireConnection;187;0;181;0
WireConnection;187;1;171;2
WireConnection;159;0;138;0
WireConnection;159;1;141;0
WireConnection;159;2;144;0
WireConnection;179;0;138;0
WireConnection;179;1;171;0
WireConnection;179;2;172;0
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
WireConnection;90;0;9;0
WireConnection;90;1;86;0
WireConnection;90;2;87;0
WireConnection;90;3;88;0
WireConnection;90;4;89;0
WireConnection;161;0;138;0
WireConnection;161;1;169;0
WireConnection;161;2;168;0
WireConnection;97;0;9;0
WireConnection;97;1;200;0
WireConnection;97;2;94;0
WireConnection;97;3;95;0
WireConnection;97;4;96;0
WireConnection;189;0;138;0
WireConnection;189;1;187;0
WireConnection;189;2;186;0
WireConnection;113;0;198;0
WireConnection;113;1;196;0
WireConnection;160;0;97;0
WireConnection;160;1;159;0
WireConnection;170;0;75;0
WireConnection;170;1;161;0
WireConnection;190;0;90;0
WireConnection;190;1;189;0
WireConnection;112;0;113;0
WireConnection;112;1;115;0
WireConnection;180;0;66;0
WireConnection;180;1;179;0
WireConnection;0;0;180;0
WireConnection;0;1;160;0
WireConnection;0;3;190;0
WireConnection;0;4;170;0
WireConnection;0;11;112;0
ASEEND*/
//CHKSM=145F12D661C85374A8045ED9AEECDA1720E09B72