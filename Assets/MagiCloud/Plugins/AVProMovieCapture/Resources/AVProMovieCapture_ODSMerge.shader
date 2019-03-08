// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/AVProMovieCapture/ODSMerge"
{
	Properties
	{
	}
	SubShader
	{
		Lighting Off
		ZTest Always
		Cull Off
		ZWrite Off
		Fog { Mode off }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile LAYOUT_EQUIRECT360 LAYOUT_EQUIRECT180
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			#define QUARTERPI 0.7853981633974483

			uniform sampler2D _leftTopTex;
			uniform sampler2D _leftBotTex;
			uniform sampler2D _rightTopTex;
			uniform sampler2D _rightBotTex;

			uniform float4 _leftTopTex_TexelSize;
			uniform float4 _leftBotTex_TexelSize;
			uniform float4 _rightTopTex_TexelSize;
			uniform float4 _rightBotTex_TexelSize;

			uniform float _sliceCenter;
			uniform float _pixelSliceSize;
			uniform int _paddingSize;

			uniform float _targetXTexelSize;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 computePixelColour(float2 uv) 
			{
				fixed4 col;

				// uv.y > 0.5 is top of texture (left eye)
				// uv.y < 0.5 is bottom of texture (right eye)
				if (uv.y < 0.25f) {
					uv.y = uv.y * 4.0;
					float phi = ((uv.y - 0.5) * 2.0) * QUARTERPI;
					uv.y = (tan(phi) * 0.5) + 0.5;
					col = tex2D(_rightBotTex, uv);
				}
				else if (uv.y < 0.5) {
					uv.y = (uv.y - 0.25) * 4.0;
					float phi = ((uv.y - 0.5) * 2.0) * QUARTERPI;
					uv.y = (tan(phi) * 0.5) + 0.5;
					col = tex2D(_rightTopTex, uv);
				}
				else if (uv.y < 0.75) {
					uv.y = (uv.y - 0.5) * 4.0;
					float phi = ((uv.y - 0.5) * 2.0) * QUARTERPI;
					uv.y = (tan(phi) * 0.5) + 0.5;
					col = tex2D(_leftBotTex, uv);
				}
				else {
					uv.y = (uv.y - 0.75) * 4.0;
					float phi = ((uv.y - 0.5) * 2.0) * QUARTERPI;
					uv.y = (tan(phi) * 0.5) + 0.5;
					col = tex2D(_leftTopTex, uv);
				}

				return col;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = 0.0;
				
				float column = float(_sliceCenter) * _targetXTexelSize;
				float range = _pixelSliceSize * _targetXTexelSize / 2.0 + 0.000001;
				#if defined(LAYOUT_EQUIRECT180)
				float dif = ((i.uv.x + 0.5) * 0.5) - column;
				#else
				float dif = i.uv.x - column;
				#endif
				float adif = abs(dif);

				if (adif < range)
				{ 
					float r = _pixelSliceSize * _targetXTexelSize / 2.0;
					float rp = (_pixelSliceSize + 2.0 * _paddingSize) * _targetXTexelSize / 2.0;
					float xRescaled = (dif < 0 ? r - adif : r + adif) / (rp  * 2.0) + _paddingSize * _targetXTexelSize;
					col = computePixelColour(float2(xRescaled, i.uv.y));
				}
				else
				{
					discard;
				}	

				return col;
			}
			ENDCG
		}
	}
}
