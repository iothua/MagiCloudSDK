Shader "LiquidVolume/Demo Skybox Grid"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1, 1, 1, 0)
        _Color2 ("Color 2", Color) = (1, 1, 1, 0)
        _UpVector ("Up Vector", Vector) = (0, 1, 0, 0)
        _Intensity ("Intensity", Float) = 1.0
        _Exponent ("Exponent", Float) = 1.0
        // The properties below are used in the custom inspector.
        _UpVectorPitch ("Up Vector Pitch", float) = 0
        _UpVectorYaw ("Up Vector Yaw", float) = 0
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    struct appdata
    {
        float4 position : POSITION;
        float3 texcoord : TEXCOORD0;
    };
    
    struct v2f
    {
        float4 position : SV_POSITION;
        float3 texcoord : TEXCOORD0;
    };
    
    half4 _Color1;
    half4 _Color2;

    v2f vert (appdata v)
    {
        v2f o;
        o.position = UnityObjectToClipPos (v.position);
        o.texcoord = v.texcoord;
        return o;
    }
    
    fixed4 frag (v2f i) : COLOR
    {
    	float2 gridCoords = i.texcoord * 10.0;
		float2 grd = abs(frac(gridCoords + 0.5) - 0.5);
		grd /= fwidth(gridCoords);
		float  lin = min(grd.x, grd.y);
		float4 col = float4(min(lin.xxx * 0.75 , 1.0), 1.0);
		col *= lerp (_Color1, _Color2, i.texcoord.y);
		return col;
    }

    ENDCG

    SubShader
    {
        Tags { "RenderType"="Background" "Queue"="Background" }
        Pass
        {
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            CGPROGRAM
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}