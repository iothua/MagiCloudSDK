Shader "LiquidVolume/Smoke" {
	Properties {
		[HideInInspector] _FlaskColor ("Flask Color", Color) = (0,0,0)
		[HideInInspector] _FlaskThickness ("Flask Thickness", Vector) = (0.05,0.05,0.05)
		[HideInInspector] _Glossiness ("Smoothness", Range(0,1)) = 0.5
		[HideInInspector] _GlossinessInt ("Internal Smoothness", Range(0,1)) = 0.5
		[HideInInspector] _EmissionColor ("Emission Color", Color) = (0,0,0)
		[HideInInspector] _SmokeColor ("Smoke Color", Color) = (0.7,0.7,0.7,0.1)
		[HideInInspector] _SmokeAtten("Smoke Atten", Range(0,10)) = 2.0
		[HideInInspector] _SmokeRaySteps ("Smoke Ray Steps", Int) = 10
		[HideInInspector] _SmokeSpeed ("Smoke Speed", Range(0,20)) = 5.0
		_NoiseTex2D ("Noise Tex 2D", 2D) = "white"
		[HideInInspector] _Scale ("Scale", Vector) = (0.25, 0.2, 1, 5.0)
		
		[HideInInspector] _CullMode ("Cull Mode", Int) = 2
		[HideInInspector] _ZTestMode ("ZTest Mode", Int) = 4

		[HideInInspector] _AlphaCombined ("Alpha Combined", Float) = 1.0
		[HideInInspector] _LevelPos ("Level Pos", Float) = 0
		[HideInInspector] _UpperLimit ("Upper Limit", Float) = 1
		[HideInInspector] _LowerLimit ("Lower Limit", Float) = -1
		[HideInInspector] _NoiseTex ("Noise Tex", 3D) = "white"
		[HideInInspector] _Center ("Center", Vector) = (1,1,1)
		[HideInInspector] _Size ("Size", Vector) = (1,1,1,0.5)
		[HideInInspector] _DoubleSidedBias ("Double Sided Bias", Float) = 0
	}
	SubShader {
	Tags { "Queue" = "Transparent+1" "IgnoreProjector"="True" "RenderType"="Transparent"  }

	// PBS Liquid ====================================================================================================================================
		ZWrite Off 
		Cull [_CullMode]
		ZTest [_ZTestMode]

		CGPROGRAM
		#pragma vertex vert
		#pragma surface surf WrappedSpecular alpha nolightmap nofog noinstancing noforwardadd
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma target 3.0
		#pragma multi_compile __ LIQUID_VOLUME_DEPTH_AWARE
		#pragma multi_compile __ LIQUID_VOLUME_DEPTH_AWARE_PASS
		#pragma multi_compile __ LIQUID_VOLUME_NON_AABB LIQUID_VOLUME_IGNORE_GRAVITY
		#pragma multi_compile LIQUID_VOLUME_SPHERE LIQUID_VOLUME_CUBE LIQUID_VOLUME_CYLINDER LIQUID_VOLUME_IRREGULAR
		#pragma multi_compile __ LIQUID_VOLUME_SCATTERING
		#include "LVLiquidPass3DSmoke.cginc"
		ENDCG
		
		// Flask ====================================================================================================================================
		ZWrite Off Cull Back
		
		CGPROGRAM
		#pragma surface surf Standard alpha nofog noinstancing noforwardadd
		#pragma target 3.0
				
		struct Input {
			float3 worldPos;
		};

		half _Glossiness;
		fixed4 _FlaskTint;
		
		void surf (Input IN, inout SurfaceOutputStandard o) {
			o.Alpha = 0;
			o.Metallic = 0;
			o.Emission = _FlaskTint.rgb;
			o.Smoothness = _Glossiness;
		}
		ENDCG
		
	}
	
	Fallback "Transparent/VertexLit"
}
