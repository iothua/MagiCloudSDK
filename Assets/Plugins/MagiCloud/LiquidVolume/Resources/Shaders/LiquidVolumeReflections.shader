Shader "LiquidVolume/Reflections" {
	Properties {
		[HideInInspector] _Color1 ("Color 1", Color) = (1,0,0,0.1)
		[HideInInspector] _FoamColor ("Foam Color", Color) = (1,1,1,0.9)
		[HideInInspector] _Color2 ("Color 2", Color) = (1,0,0,0.3)
		[HideInInspector] _FlaskColor ("Flask Color", Color) = (0,0,0)
		[HideInInspector] _FlaskThickness ("Flask Thickness", Vector) = (0.05,0.05,0.05)
		[HideInInspector] _FlaskTexAlpha ("Flask Alpha", Float) = 1.0
		[HideInInspector] _Glossiness ("Smoothness", Range(0,1)) = 0.5
		[HideInInspector] _GlossinessInt ("Internal Smoothness", Range(0,1)) = 0.5
		[HideInInspector] _Muddy ("Muddy", Range(0,1)) = 1.0
		[HideInInspector] _Turbulence ("Turbulence", Vector) = (1.0,1.0,1.0,0)
		[HideInInspector] _TurbulenceSpeed("Turbulence Speed", Float) = 1
		[HideInInspector] _SparklingIntensity ("Sparkling Intensity", Range(0,1)) = 1.0
		[HideInInspector] _SparklingThreshold ("Sparkling Threshold", Range(0,1)) = 0.85
		[HideInInspector] _EmissionColor ("Emission Color", Color) = (0,0,0)

		[HideInInspector] _DeepAtten("Deep Atten", Range(0,10)) = 2.0
		[HideInInspector] _LiquidRaySteps ("Liquid Ray Steps", Int) = 10
		[HideInInspector] _SmokeColor ("Smoke Color", Color) = (0.7,0.7,0.7,0.1)
		[HideInInspector] _SmokeAtten("Smoke Atten", Range(0,10)) = 2.0
		[HideInInspector] _SmokeRaySteps ("Smoke Ray Steps", Int) = 10
		[HideInInspector] _SmokeSpeed ("Smoke Speed", Range(0,20)) = 5.0
		_NoiseTex2D ("Noise Tex 2D", 2D) = "white"
		[HideInInspector] _FlaskBlurIntensity ("Flask Blur Intensity", Float) = 1.0
		[HideInInspector] _FoamRaySteps ("Foam Ray Steps", Int) = 15
		[HideInInspector] _FoamWeight ("Foam Weight", Float) = 10.0
		[HideInInspector] _FoamBottom ("Foam Visible From Bottom", Float) = 1.0
		[HideInInspector] _FoamTurbulence ("Foam Turbulence", Float) = 1.0
		[HideInInspector] _Scale ("Scale", Vector) = (0.25, 0.2, 1, 5.0)

		[HideInInspector] _CullMode ("Cull Mode", Int) = 2
		[HideInInspector] _ZTestMode ("ZTest Mode", Int) = 4

		[HideInInspector] _FoamDensity ("Foam Density", Float) = 1
		[HideInInspector] _AlphaCombined ("Alpha Combined", Float) = 1.0
		[HideInInspector] _FoamMaxPos("Foam Max Pos", Float) = 0
		[HideInInspector] _LevelPos ("Level Pos", Float) = 0
		[HideInInspector] _UpperLimit ("Upper Limit", Float) = 1
		[HideInInspector] _LowerLimit ("Lower Limit", Float) = -1
		[HideInInspector] _NoiseTex ("Noise Tex", 3D) = "white"
		[HideInInspector] _Center ("Center", Vector) = (1,1,1)
		[HideInInspector] _Size ("Size", Vector) = (1,1,1,0.5)
		[HideInInspector] _DoubleSidedBias ("Double Sided Bias", Float) = 0
		[NoScaleOffset] _RefractTex ("Refraction Texture", Cube) = "" {}
	}
	SubShader {
	Tags { "Queue" = "Transparent+1" "IgnoreProjector"="True" "RenderType"="Transparent"  }

	// Shadow ==========================================================================================================================================================
	Pass {	
		Cull Front
		Tags { "LightMode" = "ShadowCaster"  }
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
        #pragma multi_compile_shadowcaster
        #pragma fragmentoption ARB_precision_hint_fastest
		#include "LVShadowPass.cginc"
		ENDCG
	} 

	Pass { // Blur background ======================
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull [_CullMode]

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 3.0
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma multi_compile __ LIQUID_VOLUME_DEPTH_AWARE
		#include "LVBlurPass.cginc"
		ENDCG
	}

	Pass { // Reflections on back faces ===============

		Cull Front
		ZWrite Off
		Blend One One

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 3.0
		#include "UnityCG.cginc"
        
		struct v2f {
			float4 pos : SV_POSITION;
			float3 uv : TEXCOORD0;
		};

		v2f vert (float4 v : POSITION, float3 n : NORMAL)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v);
			// reflect view direction along the normal, in view space.
			float3 viewDir = normalize(ObjSpaceViewDir(v));
			o.uv = -reflect(viewDir, n);
			o.uv = mul(unity_ObjectToWorld, float4(o.uv,0));
			return o;
		}

		samplerCUBE _RefractTex;
		half _FlaskTexAlpha;
		fixed4 _FlaskTint;

		half4 frag (v2f i) : SV_Target {
			half3 refraction = texCUBE(_RefractTex, i.uv).rgb * _FlaskTint.rgb;
			half4 reflection = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, i.uv);
			reflection.rgb = DecodeHDR (reflection, unity_SpecCube0_HDR);
			half3 multiplier = reflection.rgb * _FlaskTexAlpha;
			return half4(refraction.rgb * multiplier.rgb, _FlaskTexAlpha);
		}
		ENDCG 
	}

	
	// PBS Liquid ====================================================================================================================================
		ZWrite Off
		Cull  [_CullMode]
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
		#pragma multi_compile __ LIQUID_VOLUME_SMOKE
		#include "LVLiquidPass3D.cginc"
		ENDCG
		
		
	// Flask ====================================================================================================================================
		Pass { 
			ZWrite Off
			Cull Back
			Blend One One
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
        
			struct v2f {
				float4 pos : SV_POSITION;
				float3 uv : TEXCOORD0;
				half fresnel : TEXCOORD1;
			};

			v2f vert (float4 v : POSITION, float3 n : NORMAL)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v);
				float3 viewDir = normalize(ObjSpaceViewDir(v));
				o.uv = -reflect(viewDir, n);
				o.uv = mul(unity_ObjectToWorld, float4(o.uv,0));
				o.fresnel = 1.0 - saturate(dot(n,viewDir));
				return o;
			}

			samplerCUBE _RefractTex;
			half _FlaskTexAlpha;
			fixed4 _FlaskTint;
			half _Glossiness;

			half4 frag (v2f i) : SV_Target
			{
				half3 refraction = texCUBE(_RefractTex, i.uv).rgb * _FlaskTint.rgb;
				half4 reflection = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, i.uv);
				reflection.rgb = DecodeHDR (reflection, unity_SpecCube0_HDR);
				half3 reflection2 = reflection * _Glossiness * i.fresnel;
				half3 multiplier = reflection.rgb * _FlaskTexAlpha;
				return half4(reflection2 + refraction.rgb * multiplier, _FlaskTexAlpha);
			}
			ENDCG
		}
		
	}
	
	Fallback "Transparent/VertexLit"
}
