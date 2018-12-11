Shader "LiquidVolume/Blur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

Subshader {	
	
  Pass { // 0 - Horizontal blur
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vertBlurH
      #pragma fragment fragBlur
      #pragma target 3.0
      #include "LVBlur.cginc"
      ENDCG
  }
 
   Pass { // 1 - Vertical blur
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vertBlurV
      #pragma fragment fragBlur
      #pragma target 3.0
      #include "LVBlur.cginc"
      ENDCG
  }  
}
FallBack Off
}
