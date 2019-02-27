Shader "Hidden/Highlighted/Cut"
{
	SubShader
	{
		Lighting Off
		Fog { Mode off }
		ZWrite Off
		ZTest Always
		Cull Back
		Blend Off
		
		Pass
		{
			Stencil
			{
				Ref 1
				Comp NotEqual
				Pass Keep
				ZFail Keep
			}
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};
			
			uniform sampler2D _HighlightingBlurred;
			uniform float2 _HighlightingBufferTexelSize;
			
			v2f vert(appdata_img v)
			{
				v2f o;
				o.pos = v.vertex;
				
				#if defined(UNITY_HALF_TEXEL_OFFSET)
				o.pos.x -= _HighlightingBufferTexelSize.x;
				o.pos.y += _HighlightingBufferTexelSize.y;
				#endif
				
				o.uv = v.texcoord.xy;
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				return tex2D(_HighlightingBlurred, i.uv);
			}
			ENDCG
		}
		
		Pass
		{
			Stencil
			{
				Ref 1
				Comp Equal
				Pass Keep
				ZFail Keep
			}
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"
			
			struct appdata_vert
			{
				float4 vertex : POSITION;
			};
			
			uniform float2 _HighlightingBufferTexelSize;
			
			float4 vert(appdata_vert v) : SV_POSITION
			{
				float4 pos = v.vertex;
				
				#if defined(UNITY_HALF_TEXEL_OFFSET)
				pos.x -= _HighlightingBufferTexelSize.x;
				pos.y += _HighlightingBufferTexelSize.y;
				#endif
				
				return pos;
			}
			
			fixed4 frag() : SV_Target
			{
				return 0;
			}
			ENDCG
		}
	}
	FallBack Off
}
