Shader "GpuInstancing/DefaultGrass"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _Cutoff ("Cutoff", Float) = 0.375
        _WavingTint("Fade Color", Color) = (.7,.6,.5, 0)
        _WaveAndDistance("Wave and distance", Vector) = (12, 3.6, 1, 1)
        [HideInInspector]
        _GrassColor ("Grass Color", Color) = (1, 1, 1, 1)
        [HideInInspector]
        _Normal ("Normal", Vector) = (0, 0, 0, 0)
	}

	SubShader
	{
		Tags
        {
            "Queue"="Geometry+200"
            "Queue"="AlphaTest"
            "IgnoreProjector"="True"
            "RenderType"="Grass"
        }
        Cull Off
		LOD 200
        ColorMask RGB

		CGPROGRAM
        //#pragma surface surf Lambert vertex:InstancingWavingGrassVert addshadow
        #pragma surface surf Lambert vertex:InstancingWavingGrassVert
        #pragma multi_compile_istancing
        #pragma instancing_options procedural:GrassSetup
        #pragma enable_d3d11_debug_symbols

		#include "UnityCG.cginc"
        #include "TerrainEngine.cginc"
        #include "Include/GpuInstancingSetup.cginc"

        float4 _GrassColor;
        float4 _Normal;

        void InstancingWavingGrassVert(inout appdata_full v)
        {
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            v.color *= _GrassColor;
            v.vertex = mul(unity_ObjectToWorld, v.vertex);
            v.normal = _Normal.xyz;

            unity_ObjectToWorld = float4x4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            );
            unity_WorldToObject = unity_ObjectToWorld;
#endif
            // MeshGrass v.color.a: 1 on top vertices, 0 on bottom vertices
            // _WaveAndDistance.z == 0 for MeshLit
            float waveAmount = v.color.a * _WaveAndDistance.z;

            v.color = TerrainWaveGrass(v.vertex, waveAmount, v.color);
        }


#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        StructuredBuffer<float3> ColorData;
        StructuredBuffer<float3> NormalData;

        void GrassSetup()
        {
            int index = DrawInstanceData[unity_InstanceID];

            _GrassColor = float4(ColorData[index], 1);
            _Normal = float4(NormalData[index], 1);
            InstancingSetup();
        }
#endif

		sampler2D _MainTex;
        float _Cutoff;

        struct Input
        {
            float2 uv_MainTex;
            float4 color : COLOR;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            float4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            clip(o.Alpha - _Cutoff);
            o.Alpha *= IN.color.a;
        }

		ENDCG
	}

    Fallback Off
}
