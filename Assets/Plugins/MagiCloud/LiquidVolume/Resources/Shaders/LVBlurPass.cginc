	#include "UnityCG.cginc"
		
		struct v2f {
            float4 pos: SV_POSITION;
			float4 uvgrab : TEXCOORD0;
			float2 uvdisp : TEXCOORD1;
			#if LIQUID_VOLUME_DEPTH_AWARE
			float z: TEXCOORD2;
			#endif
		};

		sampler2D _VLGrabBlurTexture;
		#if LIQUID_VOLUME_DEPTH_AWARE
		sampler2D _CameraDepthTexture;
		#endif
		float4 _VLGrabBlurTexture_TexelSize;
		float _FlaskBlurIntensity;
		sampler2D _DispMap;
		float4 _DispMap_ST;
		float _DispAmount;
		
		v2f vert(appdata_base v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
			#else
				float scale = 1.0;
			#endif
			o.uvgrab = ComputeGrabScreenPos(o.pos);
#if UNITY_SINGLE_PASS_STEREO
			o.uvgrab.x *= 0.5;
			o.uvdisp = UnityStereoScreenSpaceUVAdjust( v.texcoord, _DispMap_ST );
#else
			o.uvdisp = TRANSFORM_TEX( v.texcoord, _DispMap );
#endif
			
						
			#if LIQUID_VOLUME_DEPTH_AWARE
			o.z = COMPUTE_EYEDEPTH(v.vertex);
			#endif
			return o;
		}
		
		half4 frag(v2f i) : SV_Target {
			half2 disp = UnpackNormal(tex2D( _DispMap, i.uvdisp )).rg;
			float2 offset = disp * _DispAmount * _VLGrabBlurTexture_TexelSize.xy;
#if UNITY_SINGLE_PASS_STEREO
			offset.x *= 2.0;
#endif
			
			#if LIQUID_VOLUME_DEPTH_AWARE
			float4 uv4 = i.uvgrab;
			uv4.xy = offset * 3.0 * uv4.z + uv4.xy;	// x3.0 reduces downsampling unaccuracy
			uv4 = UNITY_PROJ_COORD(uv4);
			float z = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv4.xy/uv4.ww));
			offset *= z>i.z;	// object is in front of our transparent flask - discard distortion offset
			#endif

			i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
			
			half4 co = tex2Dproj(_VLGrabBlurTexture, UNITY_PROJ_COORD(i.uvgrab));
			co.a = _FlaskBlurIntensity;
			return co;
		}
	