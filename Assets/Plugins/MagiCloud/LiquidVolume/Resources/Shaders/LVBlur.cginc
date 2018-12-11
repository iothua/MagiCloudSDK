	#include "UnityCG.cginc"

	uniform sampler2D _MainTex;
	uniform half4     _MainTex_TexelSize;
	uniform half4     _MainTex_ST;
	
    struct appdata {
    	float4 vertex : POSITION;
		half2 texcoord : TEXCOORD0;
    };
    
	struct v2fBlur {
	    float4 pos : SV_POSITION;
		half2 uv0: TEXCOORD0;
		half2 uv1: TEXCOORD1;
		half2 uv2: TEXCOORD2;
		half2 uv3: TEXCOORD3;
		half2 uv4: TEXCOORD4;
	};

	v2fBlur vertBlurH(appdata v) {
    	v2fBlur o;
    	o.pos = UnityObjectToClipPos(v.vertex);
    	o.uv0 = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
    	half3 inc = half3(_MainTex_TexelSize.x * 1.3846153846, _MainTex_TexelSize.x * 3.2307692308, 0);
		#if UNITY_SINGLE_PASS_STEREO
		inc.xy *= 2.0.xx;
		#endif
    	o.uv1 = o.uv0 + inc.xz;
    	o.uv2 = o.uv0 - inc.xz;
    	o.uv3 = o.uv0 + inc.yz;
    	o.uv4 = o.uv0 - inc.yz;
		#if UNITY_UV_STARTS_AT_TOP
    	if (_MainTex_TexelSize.y < 0) {
	        // Depth texture is inverted WRT the main texture
    	    o.uv0.y = 1.0 - o.uv0.y;
    	    o.uv1.y = 1.0 - o.uv1.y;
    	    o.uv2.y = 1.0 - o.uv2.y;
    	    o.uv3.y = 1.0 - o.uv3.y;
    	    o.uv4.y = 1.0 - o.uv4.y;
    	}
    	#endif    
    	return o;
	}	
	
	v2fBlur vertBlurV(appdata v) {
    	v2fBlur o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv0 = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);

    	half3 inc = half3(_MainTex_TexelSize.y * 1.3846153846, _MainTex_TexelSize.y * 3.2307692308, 0);
    	o.uv1 = o.uv0 + inc.zx;
    	o.uv2 = o.uv0 - inc.zx;
    	o.uv3 = o.uv0 + inc.zy;
    	o.uv4 = o.uv0 - inc.zy;
    	#if UNITY_UV_STARTS_AT_TOP
    	if (_MainTex_TexelSize.y < 0) {
	        // Depth texture is inverted WRT the main texture
    	    o.uv0.y = 1.0 - o.uv0.y;
    	    o.uv1.y = 1.0 - o.uv1.y;
    	    o.uv2.y = 1.0 - o.uv2.y;
    	    o.uv3.y = 1.0 - o.uv3.y;
    	    o.uv4.y = 1.0 - o.uv4.y;
    	}
    	#endif    
    	return o;
	}
	
	half4 fragBlur (v2fBlur i): SV_Target {
		half4 pixel = tex2D(_MainTex, i.uv0) * 0.2270270270;
		// one pixel offset
		pixel += tex2D(_MainTex, i.uv1) * 0.3162162162;
		pixel += tex2D(_MainTex, i.uv2) * 0.3162162162;
		// two pixels offset
		pixel += tex2D(_MainTex, i.uv3) * 0.0702702703;
		pixel += tex2D(_MainTex, i.uv4) * 0.0702702703;
		
  		return pixel;
	}	
