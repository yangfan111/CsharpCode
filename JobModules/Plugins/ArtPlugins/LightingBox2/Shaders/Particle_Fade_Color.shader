Shader "LightingBox/Particle/Distance Based Fade" 
{
Properties 
{
	_Color("Color",Color) = (1,1,1,1)
    _MainTex ("Particle Texture", 2D) = "white" {}
    _MaxDistance("Max Distance",Float) = 7
    _MaxIntensity("Max Intensity",Float) = 30
}
SubShader
 {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
    Blend OneMinusDstColor One
    Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
   
    CGPROGRAM
    #pragma exclude_renderers flash
    #pragma surface surf Lambert vertex:vert noforwardadd
   
    sampler2D _MainTex;
    float _Dist;
    float _MaxIntensity;
    float _MaxDistance;
    half3 _Color;


    struct Input {
        float2 uv_MainTex;
        float4 color : Color;
       
        float3 worldPos;
    };


   void vert (inout appdata_full v, out Input o) 
   {
        UNITY_INITIALIZE_OUTPUT(Input,o);
        o.color = v.color;
   }

    void surf (Input IN, inout SurfaceOutput o) 
    {
        fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

        _Dist = distance(_WorldSpaceCameraPos, IN.worldPos);
        	if(_Dist>_MaxDistance)
				_Dist = _MaxDistance;

        o.Albedo = (c.rgb*_Dist/_MaxIntensity) * IN.color.rgb * _Color.rgb;
        o.Alpha = c.a * IN.color.a*_Dist/_MaxIntensity * _Color.rgb                 ;
    }
   
    ENDCG
 }
}
 