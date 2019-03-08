// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/AVProMovieCapture/Plasma"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			//dave hoskins hash
			float2 hash(float2 p)
			{
				float3 HASHSCALE3 = float3(0.1031, 0.1030, 0.0973);
				float3 p3 = frac(float3(p.xyx) * HASHSCALE3);
				p3 += dot(p3, p3.yzx + 19.19);

				return frac((p3.xx+p3.yz)*p3.zy);
			}

			float voronoi(float2 p, float gap)
			{
				p *= 1.0 / gap;

				float2 n = floor(p);
				float2 f = frac(p);

				float min_dist = 99999.0;
				for (int j = -1; j <= 1; j++)
					for (int i = -1; i <= 1; i++)
					{
						float2 pos = float2(float(i), float(j));
						float2 jitter = (hash(n + pos) - 0.5) * 2.0;
						jitter = 0.5 + sin(_Time.y + 6.2831 * jitter) * 0.5;
						float2 r = pos + jitter - f;
						float d = length(r);

						if (d < min_dist)
						{
							min_dist = d;
						}
					}

				return pow(min_dist, 3.0) * gap * 10;
			}

			float3 tonemap(float3 color)
			{
				color = max(float3(0, 0, 0), color - float3(0.004, 0.004, 0.004));
				color = (color * (6.2 * color + .5)) / (color * (6.2 * color + 1.7) + 0.06);
				return color;
			}

			float plasma(float2 p)
			{
				float gap = 0.5;
				float norm_factor = 0.0;
				float total_val = 0.0;

				[unroll(8)]
				for (int i = 0; i < 8; ++i)
				{
					total_val += voronoi(p, gap) / 16.0;
					norm_factor += gap;
					gap /= 2.0;
				}
				return total_val / norm_factor;
			}


			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				i.uv += float2(0, -_Time.y * 0.1);
				
				float r = abs(frac(i.uv.y) - 0.5) * 2.0;
				float b = 1 - r;
				float g = 1.0 - 2 * abs(i.uv.x - 0.5);

				fixed4 col = fixed4(r, g, b, 1) * plasma(i.uv);

				col.rgb = tonemap(col.rgb);
				col.a = 1;

				return col;
			}
			ENDCG
		}
	}

	Fallback Off
}
