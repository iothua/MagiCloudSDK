Shader "Particles/Additive Displacement" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
	ColorMask RGB
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off Lighting Off ZWrite Off
	
	SubShader {

		GrabPass { "_BackgroundTexture" }

		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float4 projPos : TEXCOORD2;
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			sampler2D_float _CameraDepthTexture;
			sampler2D _BackgroundTexture;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = i.color * _TintColor * tex2D(_MainTex, i.texcoord);
				float2 disp = (i.texcoord-0.5) * 0.04 * col.a;
				fixed4 bgColor = tex2D(_BackgroundTexture, i.projPos.xy / i.projPos.w + disp);
				col.rgb = lerp(bgColor.rgb, col.rgb, col.a);
				col.rgb += 0.1;
				return col;
			}
			ENDCG 
		}
	}	
}
}
