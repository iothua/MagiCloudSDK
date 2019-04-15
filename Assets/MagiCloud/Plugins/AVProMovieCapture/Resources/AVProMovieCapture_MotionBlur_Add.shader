// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/AVProMovieCapture/MotionBlur-Add" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader 
	{
		Pass 
		{
			ZTest Always Cull Off ZWrite Off
			Lighting Off
			Fog { Mode off }
			Blend One One
			
			CGPROGRAM
			#pragma exclude_renderers flash xbox360 ps3 gles
			#pragma vertex vert
			#pragma fragment frag
			//#pragma target 3.0
			//#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_TexelSize;
			uniform float _Weight;

			struct appdata
			{
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				
				// Note: this flip is not needed because motion blur never target anti-aliased rendertextures
/*#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
				{
					o.uv.y = 1.0 - o.uv.y;
				}
#endif*/
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				float4 col = tex2D(_MainTex, i.uv) * _Weight;
				return col;
			}
			ENDCG	
		}	
	}

Fallback off
}