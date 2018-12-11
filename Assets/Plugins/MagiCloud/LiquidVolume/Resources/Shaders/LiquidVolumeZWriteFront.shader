Shader "LiquidVolume/ZWriteFront" {
	Properties {
		_Color ("Color", Color) = (0,0,1,1)
	}
	SubShader {
	
	Pass {	
		//ZTest Always
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
        #pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"

		struct v2f {
			float4 pos: SV_POSITION;
		    float dist : TEXCOORD0;
		};
		
		v2f vert(appdata_base v) {
    		v2f o;
    		o.pos = UnityObjectToClipPos(v.vertex);
    		float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
    		o.dist = 1.0 / (distance(_WorldSpaceCameraPos, worldPos) + 1.0);
    		return o;
		}

		float4 frag (v2f i) : SV_Target {
			return EncodeFloatRGBA(i.dist);
		}
		ENDCG
	} 	
}
}

    