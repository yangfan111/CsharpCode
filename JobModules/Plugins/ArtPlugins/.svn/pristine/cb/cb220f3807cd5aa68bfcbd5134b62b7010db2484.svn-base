// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "DepthOfFiled"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 0.1
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;  
    sampler2D _MaskTex;  
    sampler2D _BlurTex;  
    sampler2D _CameraDepthTexture;  
    uniform half4 _MainTex_TexelSize;  
    uniform float _BlurSize;
    uniform float _FocusDistance;
    uniform float _FocusRange;
 uniform float _FocusFar;

    static const half weight[4] = {0.0205, 0.0855, 0.232, 0.324};
    static const half4 coordOffset = half4(1.0h,1.0h,-1.0h,-1.0h);

    struct v2f_blurSGX
    {
        float4 pos:SV_POSITION;
        half2 uv:TEXCOORD0;
        half4 uvoff[3]:TEXCOORD1;
    };

    struct v2f_dof
    {
        float4 pos:SV_POSITION;
        half2 uv:TEXCOORD0;
    };

    v2f_blurSGX vert_BlurHorizontal(appdata_img v)
    {
        v2f_blurSGX o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.texcoord.xy;
        half2 offs = _MainTex_TexelSize.xy*half2(1,0)*_BlurSize;
        o.uvoff[0] = v.texcoord.xyxy+offs.xyxy*coordOffset*3;
        o.uvoff[1] = v.texcoord.xyxy+offs.xyxy*coordOffset*2;
        o.uvoff[2] = v.texcoord.xyxy+offs.xyxy*coordOffset;

        return o;
    }

    v2f_blurSGX vert_BlurVertical(appdata_img v)
    {
        v2f_blurSGX o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.texcoord.xy;

        half2 offs = _MainTex_TexelSize.xy*half2(0,1)*_BlurSize;
        o.uvoff[0] = v.texcoord.xyxy+offs.xyxy*coordOffset*3;
        o.uvoff[1] = v.texcoord.xyxy+offs.xyxy*coordOffset*2;
        o.uvoff[2] = v.texcoord.xyxy+offs.xyxy*coordOffset;

        return o;
    }

    fixed4 frag_Blur(v2f_blurSGX i):SV_Target
    {
        
        fixed4 c = tex2D(_MainTex,i.uv)*weight[3];
        for(int idx=0; idx<3; idx++)
        {
            c+=tex2D(_MainTex,i.uvoff[idx].xy)*weight[idx];
            c+=tex2D(_MainTex,i.uvoff[idx].zw)*weight[idx];
        }

        return c;
    }

    v2f_dof vert_Dof(appdata_img v)
    {
        v2f_dof o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.texcoord.xy;

        return o;
    }

    fixed4 frag_Dof(v2f_dof i):SV_Target
    {
        fixed4 c = tex2D(_MainTex,i.uv);
        fixed4 b = tex2D(_BlurTex,i.uv);
 fixed4 m = tex2D(_MaskTex,i.uv);
        float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);  
        //将深度值转化到01线性空间  
        depth = Linear01Depth(depth); 

         b= lerp(c,b,m.r*clamp((abs(depth-_FocusDistance)-_FocusRange)/_FocusFar,0,1));
		 return  b*(1 - 0.35*(m.r));
    }
    ENDCG

    SubShader
    {
        // No culling or depth
        //Cull Off ZWrite Off 

        //Pass 0
        Pass
        {
            ZTest Always
            CGPROGRAM
            #pragma vertex vert_BlurHorizontal
            #pragma fragment frag_Blur
            

            ENDCG
        }

        //Pass 1 
        Pass
        {
            ZTest Always
            CGPROGRAM
            #pragma vertex vert_BlurVertical
            #pragma fragment frag_Blur
            

            ENDCG
        }

        //Pass 2
        Pass
        {
            ZTest Off  
            Cull Off  
            ZWrite Off  
            Fog{ Mode Off }  
            ColorMask RGBA

            CGPROGRAM
            #pragma vertex vert_Dof
            #pragma fragment frag_Dof
            

            ENDCG
        }


    }
}