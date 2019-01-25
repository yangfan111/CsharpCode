Shader "Custom/Eye Shading" {
	Properties{
		//Cornea角膜
		//Iris虹膜
		//Sclera巩膜

		//diffuse的光照亮度，影响基础纹理亮度
		_Lum("Luminance", Range(0, 10)) = 1
		//角膜覆盖部分的颜色，可设置成虹膜颜色，与MASK中间圆圈区域大小有关
		_IrisColor("Iris Color", Color) = (1, 1, 1, 1)
		//瞳孔大小
		_PupilSize("PupilSize", Range(0.01, 1)) = 0.01

		//基础纹理
		_MainTex("Base (RGB)", 2D) = "white" {}
		//计算角膜高光和虹膜/巩膜高光时，Cook-Torrance模型的f项菲涅尔函数的变量，高光颜色,实际好像不会有影响
		[HideInInspector]_SCCornea("Specular Color", Color) = (1, 1, 1, 1)
		//纹理，给整体蒙上粗糙的高光，对眼球整体都会有影响，虹膜的高光和巩膜的高光会乘上它
		_SpecularTex("SpecularTex (RGB)", 2D) = "white" {}
		//*****角膜所在部分的normal map，改变每个frag上的原本的normal，使其具有凸出或其他凹凸的感觉来计算高光
		[HideInInspector]_NormalCorneaTex("Normal Cornea Tex (RGB)", 2D) = "white" {}
		//整个眼球的细节normal map，比如血管等细节，影响整个眼球的diffuse，同时会用他计算细节处的高光
		_NormalIrisDetialTex("Normal Detial Tex (RGB)", 2D) = "white" {}
		//角膜遮罩，避免角膜的高光和虹膜的颜色等影响巩膜
		_MaskTex("MaskTex (RGB)", 2D) = "white" {}
		//瞳孔遮罩，瞳孔不透光
		_PupilMaskTex("PupilTex(A)",2D) = "white"{}

		//角膜高光计算的的gloss，影响角膜的高光是否小而锐利
		_GLCornea("Cornea Gloss", Range(0, 2)) = 0.8
		//角膜高光的亮度
		_SPCornea("Cornea Specular",Range(0.01,10))=2
		//角膜高光颜色
		_SPCorneaDetial("Cornea Specular Color ", Color) = (1, 1, 1, 1)
		//****虹膜巩膜高光计算的gloss
		_GLIris("Iris&Sclera Gloss", Range(0, 2)) = 0.1
		//虹膜巩膜细节的高光*粗糙高光纹理*细节高光颜色的最终亮度调节
		_SPIris("Iris&Sclera Specular Power", Range(0.1, 100)) = 1
		//细节巩膜高光颜色
		_SPScleraDetial("Detial Sclera Specular Color ", Color) = (1, 1, 1, 0.1)
		//细节虹膜高光颜色
		_SPIrisDetial("Detial Iris Specular Color ", Color) = (1, 1, 1, 0.1)


		//cubeMap的反射强度
		_ReflAmount("ReflAmount", Range(0, 4)) = 0.4
		//反射周围环境的cubeMap
		_Cubemap("CubeMap", CUBE) = ""{}

		////虹膜的视差贴图
		//_ParallaxMap("Heightmap (A)", 2D) = "black" {}
		////视差高度
		//_Parallax("Height",Range(0.005,0.2))=0.02

	}
		SubShader{
		pass {
		Tags{ "LightMode" = "ForwardBase" }
			Cull Back
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
			float4 _SPScleraDetial;
		float _SPIris;
		float _Lum;
		float  _ReflAmount;
		samplerCUBE _Cubemap;
		float4 _LightColor0;
		float4 _SCCornea;
		float4 _IrisColor;
		float _GLCornea;
		float _SPCornea;
		float4 _SPCorneaDetial;
		float4 _SPIrisDetial;
		float _GLIris;
		sampler2D _MaskTex;
		sampler2D _PupilMaskTex;
		sampler2D _MainTex;
		sampler2D _SpecularTex;
		sampler2D _NormalCorneaTex;
		sampler2D _NormalIrisDetialTex;
		float4 _MainTex_ST;
		float4 _MaskTex_ST;
		float4 _PupilMaskTex_ST;

		float _PupilSize;
		sampler2D _ParallaxMap;
		float _Parallax;

		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv_MainTex : TEXCOORD0;
			float3 lightDir : TEXCOORD1;
			float3 viewDir : TEXCOORD2;
			float3 normal : TEXCOORD3;
			float2 uv_MaskTex : TEXCOORD4;
			float2 uv_PupilMaskTex:TEXCOORD5;
		};

		v2f vert(appdata_full v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.normal = v.normal;
			o.lightDir = ObjSpaceLightDir(v.vertex);
			o.viewDir = ObjSpaceViewDir(v.vertex);
			o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.uv_MaskTex = TRANSFORM_TEX(v.texcoord, _MaskTex);
			o.uv_PupilMaskTex= TRANSFORM_TEX(v.texcoord, _PupilMaskTex);
			return o;
		}
#define PIE 3.14159265


		float4 frag(v2f i) :COLOR
		{
			float3 viewDir = normalize(i.viewDir);
			float3 lightDir = normalize(i.lightDir);
			float3 H = normalize(lightDir + viewDir);
			float3 N = normalize(i.normal);
			float3 c;
			//瞳孔大小
			float2 UVs = i.uv_MainTex;
			float2 delta = float2(0.5, 0.5) - UVs;
			// Calculate pow(distance,2) to center (pythagoras...)
			float factor = (delta.x*delta.x + delta.y*delta.y);
			// Clamp it in order to mask our pixels, then bring it back into 0 - 1 range
			// Max distance = 0.15 --> pow(max,2) = 0.0225
			factor = saturate(0.0225 - factor) * 44.444;
			UVs += delta * factor * _PupilSize;

			c = tex2D(_MainTex, UVs);

			//虹膜的颜色			
			//if (tex2D(_MaskTex, i.uv_MaskTex).a > 0)
				c = lerp(c, c*_IrisColor, tex2D(_MaskTex, i.uv_MaskTex).a -0.02);
			
			//角膜的PBR高光
			float3 n1;
			n1 = UnpackNormal(tex2D(_NormalCorneaTex, i.uv_MainTex));

			float _SP;
			_SP = lerp(pow(8192, lerp(0, 0.5, _GLCornea - 0.4)), pow(8192, _GLCornea), tex2D(_MaskTex, i.uv_MaskTex).a + 0.005);
			float d = (_SP + 2) / (8 * PIE) * pow(dot(N, H), _SP);
			float f = _SCCornea + (1 - _SCCornea)*pow(2, -10 * dot(H, lightDir));
			float k = min(1, _GLCornea + 0.545);
			float v = 1 / (k* dot(viewDir, H)*dot(viewDir, H) + (1 - k));

			float SpecIns = d*f*v;
			//if (tex2D(_MaskTex, i.uv_MaskTex).a < 0.005)
			//	SpecIns *= tex2D(_SpecularTex, i.uv_MainTex);

			//在这个地方控制了角膜的大小,可以调整uv_MaskTex改变
			SpecIns = lerp(0, SpecIns, clamp(tex2D(_MaskTex, i.uv_MaskTex).a, 0, 1));
			if (SpecIns < 0.01f)
				SpecIns = 0.0;

			//巩膜和虹膜的细节高光
			float3 n2;
			//if (tex2D(_MaskTex, i.uv_MaskTex).a > 0.005)
			//	n2 = UnpackNormal(tex2D(_NormalCorneaTex, i.uv_MainTex));
			//else
			n2 = UnpackNormal(tex2D(_NormalIrisDetialTex, UVs));

			if (tex2D(_MaskTex, i.uv_MaskTex).a > 0.02)
				_SP = pow(8192, _GLIris);
			else
				_SP = pow(8192, lerp(0.8, 1.5, _GLIris + 0.1));			
			//!!!!调整过渡
			//_SP =lerp(pow(8192, lerp(1.05, 1.8, _GLIris + 0.1)),pow(8192, _GLIris),clamp(tex2D(_MaskTex, i.uv_MaskTex).a,0,1));
			d = (_SP + 2) / (8 * PIE) * pow(dot(n2, H), _SP);
			f = _SCCornea + (1 - _SCCornea)*pow(2, -10 * dot(H, lightDir));
			k = min(1, _GLIris + 0.545);
			v = 1 / (k* dot(viewDir, H)*dot(viewDir, H) + (1 - k));


			float roughSpecIns = d*f*v;
			if (tex2D(_MaskTex, i.uv_MaskTex).a > 0.02)
				roughSpecIns *= _SPIrisDetial.a;
			else
				roughSpecIns *= _SPScleraDetial.a;
			roughSpecIns = sqrt(roughSpecIns*roughSpecIns);
			//float3 roughSpec = roughSpecIns;
			float3 roughSpec = tex2D(_SpecularTex, i.uv_MainTex)*roughSpecIns;
			if (tex2D(_MaskTex, i.uv_MaskTex).a < 0.02)
			{
				//roughSpec *= _SPScleraDetial.rgb;
				roughSpec = 0;
			}
			else
				roughSpec *= _SPIrisDetial.rgb;

			//避免边缘高光突兀，把遮罩边缘的高光过渡一下
			if(tex2D(_MaskTex, i.uv_MaskTex).a>0&& tex2D(_MaskTex, i.uv_MaskTex).a<1)
				roughSpec = roughSpec*(tex2D(_MaskTex, i.uv_MaskTex).a);
			roughSpec = abs(roughSpec);

			//漫反射
			float3 diff1;
			diff1 =dot(lightDir, UnpackNormal(tex2D(_NormalIrisDetialTex, i.uv_MainTex)))*0.5+0.5;
			float3 diff2;
			diff2 = dot(lightDir, N)*0.5+0.5;
			//环境光
			fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
			float3 diff;
			//normalmap直接计算有点问题，把两者相加
			diff =0.5*diff1+0.5*diff2+ ambient;

			//球面环境反射
			float3 refDir = reflect(-viewDir, N);
			float3 ref = texCUBElod(_Cubemap, float4(refDir, 0.5 - _GLCornea*0.5)).rgb;
			
			//瞳孔部分无环境反射，通过基础纹理是否为黑判断瞳孔
			float sub = min(min(c.x, c.y), c.z);
			if (tex2D(_MaskTex, i.uv_MaskTex).a > 0.02&&sub > 0.1) sub *= 1.5+ 2*tex2D(_MaskTex, i.uv_MaskTex).a;
			else if (sub > 0.1) sub *= 1.5;
			//角膜的高光也要考虑瞳孔会吸收
			float2 UVs2 = i.uv_PupilMaskTex;
			delta = float2(0.5, 0.5) - UVs2;
			factor = (delta.x*delta.x + delta.y*delta.y);
			factor = saturate(0.0225 - factor) * 44.444;
			UVs2 += delta * factor * _PupilSize;
			float sub2 = 1;
			if (tex2D(_PupilMaskTex, UVs2).a < 0.0)
				sub2 = tex2D(_PupilMaskTex, UVs2).a*tex2D(_PupilMaskTex, UVs2).a;
			else
				sub2 = tex2D(_PupilMaskTex, UVs2).a*tex2D(_PupilMaskTex, UVs2).a;

			//最终结果
			//注意瞳孔部分无反光
			//return float4(c *(diff)* _Lum + roughSpec * _SPIris+ sub2*SpecIns*_SPCornea*_SPCorneaDetial + sub*Luminance(ref)* _ReflAmount, 1) * _LightColor0;
			return float4(c *(diff)* _Lum + 0*roughSpec * _SPIris + sub2*SpecIns*_SPCornea*_SPCorneaDetial + sub*Luminance(ref)* _ReflAmount, 1) * _LightColor0;



		}
			ENDCG
	}
	}
}