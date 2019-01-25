// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "LightingBox/Nature/Leave Standard (Wind Support)"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_MaskClipValue( "Mask Clip Value", Float ) = 0.5

		[Header(Translucency)]
		[HideInInspector]_Translucency("Strength", Range( 0 , 50)) = 1
		[HideInInspector]_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		[HideInInspector]_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		[HideInInspector]_TransDirect("Direct", Range( 0 , 1)) = 1
		   
		[Header(Specular  Smoothness)]
		_SpecularColor("Specular Color", Color) = (0,0,0,0)
		_Smoothness("Smoothness", Range( 0 , 3)) = 0
		_SpecularMap("Specular Map", 2D) = "white" {}

		[Header(Albedo)]
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		_BumpMap("BumpMap", 2D) = "bump" {}
		_AO("AO", 2D) = "white" {}

		[Header(Wind Settings)]
		// Wave mode leave
        _MinY("Minimum Y Value", Range(-5,0)) = -1

        _xScale ("X Amount", Range(-0.2,0.2)) = 0.5
        _yScale ("Z Amount", Range(-0.2,0.2)) = 0.5

     ////   _Scale("Scale", Range(0,100)) = 1.0 
      ///  _Speed("Speed", Range(0,10)) = 1.0 

        _WorldScale("World Scale", Range(0.001,1)) = 0.1

		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Off
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecularCustom keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vert
		struct Input
		{
			float2 uv_texcoord;
		};

		struct SurfaceOutputStandardSpecularCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			half3 Emission;
			fixed3 Specular;
			half Smoothness;
			half Occlusion;
			fixed Alpha;
			fixed3 Translucency;
		};

		uniform sampler2D _BumpMap;
		uniform float4 _BumpMap_ST;
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _SpecularColor;
		uniform sampler2D _SpecularMap;
		uniform float4 _SpecularMap_ST;
		uniform float _Smoothness;
		uniform sampler2D _AO;
		uniform float4 _AO_ST;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform float _TranslucencyIntensity;
		uniform float4 _TranslucencyColor;
		uniform float _MaskClipValue = 0.5;

		float _MinY;
        float _xScale;
        float _yScale;
        float _WindScale;
        float _WorldScale;
        float _WindSpeed;
        float _Amount;

		void vert (inout appdata_full v)
        {
            float num = v.vertex.z;

            if ((num-_MinY) > 0.0) {
                float3 worldPos = mul (unity_ObjectToWorld, v.vertex).xyz;
                float x = sin(worldPos.x/_WorldScale + (_Time.y*_WindSpeed)) * (num-_MinY) * _WindScale * 0.01;
                float y = sin(worldPos.y/_WorldScale + (_Time.y*_WindSpeed)) * (num-_MinY) * _WindScale * 0.01;

                v.vertex.x += x * _xScale;
                v.vertex.y += y * _yScale;
            }
        }


		inline half4 LightingStandardSpecularCustom(SurfaceOutputStandardSpecularCustom s, half3 viewDir, UnityGI gi )
		{
			#if !DIRECTIONAL
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandardSpecular r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Specular = s.Specular;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandardSpecular (r, viewDir, gi) + c;
		}

		inline void LightingStandardSpecularCustom_GI(SurfaceOutputStandardSpecularCustom s, UnityGIInput data, inout UnityGI gi )
		{
			UNITY_GI(gi, s, data);
		}

		void surf( Input i , inout SurfaceOutputStandardSpecularCustom o )
		{
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			o.Normal = UnpackNormal( tex2D( _BumpMap, uv_BumpMap ) );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = ( _Color * tex2DNode1 ).rgb;
			float2 uv_SpecularMap = i.uv_texcoord * _SpecularMap_ST.xy + _SpecularMap_ST.zw;
			float4 tex2DNode42 = tex2D( _SpecularMap, uv_SpecularMap );
			o.Specular = ( _SpecularColor * tex2DNode42 ).rgb;
			o.Smoothness = ( tex2DNode42 * _Smoothness ).r;
			float2 uv_AO = i.uv_texcoord * _AO_ST.xy + _AO_ST.zw;
			o.Occlusion = tex2D( _AO, uv_AO ).r;
			o.Translucency = ( tex2DNode1 * ( ( _TranslucencyIntensity * 10.0 ) * _TranslucencyColor ) ).xyz;
			o.Alpha = 1;
			clip( tex2DNode1.a - _MaskClipValue );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13101
7;29;1010;692;515.4685;169.9111;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;36;-900.7419,539.6217;Float;False;Property;_TranslucencyIntensity;Translucency Intensity;9;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;39;-917.5498,638.2397;Float;False;Constant;_Translucency_10_x_multiplier;Translucency_10_x_multiplier;12;0;10;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;31;-677.8762,764.1608;Float;False;Property;_TranslucencyColor;Translucency Color;8;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-489.8774,540.2047;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;25;-896.3705,436.9063;Float;False;Property;_Smoothness;Smoothness;11;0;0;0;3;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-300.8866,747.2303;Float;False;2;2;0;FLOAT;0.0,0,0,0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;1;-882.6503,-471.8886;Float;True;Property;_MainTex;MainTex;13;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;23;-833.1885,-657.0099;Float;False;Property;_Color;Color;12;0;1,1,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;42;-923.9894,204.2724;Float;True;Property;_SpecularMap;Specular Map;15;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;34;-572.1591,56.72324;Float;False;Property;_SpecularColor;Specular Color;10;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-507.0631,-560.0507;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT4;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;4;-885.9719,-264.5165;Float;True;Property;_BumpMap;BumpMap;14;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;40;-886.0046,-57.34707;Float;True;Property;_AO;AO;16;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-62.9407,720.4819;Float;False;2;2;0;FLOAT4;0.0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-180.1663,210.7971;Float;False;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-260.4652,331.3827;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;146.9001,42.89999;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;LightingBox/Nature/Leave Standard (Wind Support);False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;3;False;0;0;Masked;0.5;True;True;0;False;TransparentCutout;AlphaTest;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;1;-1;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;38;0;36;0
WireConnection;38;1;39;0
WireConnection;37;0;38;0
WireConnection;37;1;31;0
WireConnection;24;0;23;0
WireConnection;24;1;1;0
WireConnection;32;0;1;0
WireConnection;32;1;37;0
WireConnection;43;0;34;0
WireConnection;43;1;42;0
WireConnection;35;0;42;0
WireConnection;35;1;25;0
WireConnection;0;0;24;0
WireConnection;0;1;4;0
WireConnection;0;3;43;0
WireConnection;0;4;35;0
WireConnection;0;5;40;0
WireConnection;0;7;32;0
WireConnection;0;10;1;4
ASEEND*/
//CHKSM=6BB031E0D9C625654719CEDBF2E6ED6AA6DCBE39