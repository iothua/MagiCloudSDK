#include "LVLiquidPassBase.cginc"

	    fixed4 raymarch(float4 vertex, float3 rd, float t0, float t1) {

	        float3 wpos = wsCameraPos + rd * t0;
	        
			float turbulence = (tex2D(_NoiseTex2D, vertex.xz).g - 0.5) * _Turbulence.x;
			turbulence += sin(vertex.w) * _Turbulence.y;
			turbulence *= 0.05 * _Size.y;
			_LevelPos += turbulence;
			
			// compute level of liquid (t2)
			float2 delta = float2(1.0, length(rd.xz) / rd.y);
			float h = wpos.y - _LevelPos;
			float t2 = t0 + length(delta * h.xx);

			// ray-march smoke
			float tmin, tmax;
			#if LIQUID_VOLUME_SMOKE
			fixed4 sumSmoke = fixed4(0,0,0,0);
			if (wpos.y > _LevelPos) {
				tmin = t0;
				tmax = rd.y<0 ? min(t2,t1) : t1;
				float stepSize = (tmax - tmin) / (float)_SmokeRaySteps;
				float4 dir  = float4(rd * stepSize, 0);
				float4 rpos = float4(wsCameraPos + rd * tmin, 0);		
				float4 disp = float4(0, _Time.x * _Turbulence.x * _Size.y * _SmokeSpeed, 0, 0);
				#if SHADER_API_GLES
				for (int k=0;k<5;k++) {
				if (k<_SmokeRaySteps) {
				#else
				for (int k=0;k<_SmokeRaySteps;k++) {
				#endif
					fixed deep = exp(((_LevelPos - rpos.y)/_Size.y) * _SmokeAtten);
					fixed4 lc  = _SmokeColor;
					lc.rgb *= lc.aaa;
					lc *= deep;
					sumSmoke += lc * (1.0-sumSmoke.a);
					rpos += dir;
				}
				#if SHADER_API_GLES
				}
				#endif
			}
			#endif
			
			// ray-march liquid
			fixed4 sum = fixed4(0,0,0,0);
			tmax = t1, tmin = t0;
			if (wpos.y > _LevelPos) {
				if (rd.y<0) {
					tmin = t2;
					tmax = t1;
					if (t2<t1) sum += 0.1;
				} else {
					tmax = -99999.0;
				}
			} else if (rd.y>0 && t2<t1) sum += 0.1 * _FoamBottom;
			if (tmax>tmin) {
				float stepSize = (tmax-tmin) / (float)_LiquidRaySteps;
				float4 dir   = float4(rd * stepSize, 0);
				float4 rpos  = float4(_WorldSpaceCameraPos + rd * tmin, 0);	// does not matter to move to level pos
				rpos.y -= _LevelPos;
				#if SHADER_API_GLES
				for (int k=0;k<10;k++) {
				if (k<_LiquidRaySteps) {
				#else
				for (int k=0;k<_LiquidRaySteps;k++) {
				#endif
					fixed deep = exp((rpos.y/_Size.y) * _DeepAtten);
					fixed4 lc  = _Color1;
					lc.a *= _Color1.a;
					lc.rgb *= lc.aaa;
					lc.rgb *= deep;
					sum += lc * (1.0-sum.a);
					rpos += dir;
				}
				#if SHADER_API_GLES
				}
				#endif
			}
			
			// Final blend
			#if LIQUID_VOLUME_SMOKE
			if (wpos.y>_LevelPos) {
				sum = sum * saturate(1.0 - sumSmoke.a) + sumSmoke;
			} else {
				sum = sumSmoke * saturate(1.0 - sum.a) + sum;
			}
			#endif

			return sum;
		}

