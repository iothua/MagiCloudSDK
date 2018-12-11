#include "LVLiquidPassBase.cginc"

		sampler3D _NoiseTex;
		float _SparklingIntensity, _SparklingThreshold;
		
		fixed4 raymarch(float4 vertex, float3 rd, float t0, float t1) {

	        float3 wpos = wsCameraPos + rd * t0;
	        
			float turbulence = (tex2D(_NoiseTex2D, vertex.xz).g - 0.5) * _Turbulence.x;
			turbulence += sin(vertex.w) * _Turbulence.y;
			turbulence *= 0.05 * _Size.y * _FoamTurbulence;
			_LevelPos += turbulence;
			_FoamMaxPos += turbulence;

//			// compute levels of liquid (t2) & foam (t3)
//			float2 delta = float2(1.0, length(rd.xz) / rd.y);
//			float h = wpos.y - _LevelPos;
//			float t2 = t0 + length(delta * h.xx);;
//						
//			// compute foam level (t3)
//			float hf = wpos.y - _FoamMaxPos;
//			float t3 = t0 + length(delta * hf.xx);

			// we can get rid of 2 length calls simplifying expressions:
			float2 rdy   = rd.xz / rd.y;
			float delta = sqrt(1.0 + dot (rdy, rdy));
			float h = abs(wpos.y - _LevelPos);
			float t2 = t0 + h * delta; // length(delta * h.xx);;
						
			// compute foam level (t3)
			float hf = abs(wpos.y - _FoamMaxPos);
			float t3 = t0 + hf * delta;

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
				for (int k=_SmokeRaySteps;k>0;k--, rpos += dir) {
					fixed n = tex3Dlod(_NoiseTex, (rpos - disp) * _Scale.x).r;
					fixed4 lc  = fixed4(_SmokeColor.rgb, n * _SmokeColor.a);
					lc.rgb *= lc.aaa;
					fixed deep = exp(((_LevelPos - rpos.y)/_Size.y) * _SmokeAtten);
					lc *= deep;
					sumSmoke += lc * (1.0-sumSmoke.a);
				}
			}
			#endif
			
			// ray-march foam
			tmax = min(t3,t1), tmin = t0;
			float sy = sign(rd.y);
			if (wpos.y > _FoamMaxPos) {
				tmin = tmax;
				tmax = min(t2, t1) * -sy;
			} else if (wpos.y < _LevelPos) {
				tmin  = min(t2,t1);
				tmax *= _FoamBottom * sy;
			} else if (rd.y<0) {
				tmax = min(t2, t1);
			}
			fixed4 sumFoam  = fixed4(0,0,0,0);
			if (tmax>tmin) {
				float stepSize = (tmax - tmin) / (float)_FoamRaySteps;
				float4 dir  = float4(rd * stepSize, 0);
				float4 rpos = float4(wsCameraPos + rd * tmin, 0);
				rpos.y -= _LevelPos;
				float foamThickness = _FoamMaxPos - _LevelPos;
				float4 disp = float4(_Time.x, 0, _Time.x, 0) * _Turbulence.x * _Size.w * _FoamTurbulence;
				for (int k=_FoamRaySteps;k>0;k--, rpos += dir) {
//					float h = saturate( (rpos.y - _LevelPos) / foamThickness );
					float h = saturate( rpos.y / foamThickness );
					float n = saturate(tex3Dlod(_NoiseTex, (rpos - disp) * _Scale.y ).r + _FoamDensity);
					if (n>h) {
						fixed4 lc  = fixed4(_FoamColor.rgb, n-h);
						lc.a   *= _FoamColor.a;
						lc.rgb *= lc.aaa;
//						float deep = saturate((rpos.y-_LevelPos) * _FoamWeight / foamThickness);
						fixed deep = saturate(rpos.y * _FoamWeight / foamThickness);
						lc *= deep;
						sumFoam += lc * (1.0 - sumFoam.a);
					}
				}
				sumFoam *= 1.0 + _FoamDensity;
			}	

			// ray-march liquid
			if (wpos.y > _LevelPos) {
				tmin = t2;
				tmax = t1 * -sy;
			} else {
				tmin = t0;
				tmax = min(t2,t1);
			}
			fixed4 sum = fixed4(0,0,0,0);
			if (tmax>tmin) {
				float stepSize = (tmax-tmin) / (float)_LiquidRaySteps;
				float4 dir   = float4(rd * stepSize, 0);
				float4 rpos  = float4(wsCameraPos + rd * tmin, 0);	// does not matter to move to level pos
				rpos.y -= _LevelPos;
				float4 disp  = float4(_Time.x * _Turbulence.y, _Time.x * 1.5, _Time.x * _Turbulence.y, 0) * (_Turbulence.y + _Turbulence.x) * _Size.y;
				float4 disp2 = float4(0,_Time.x*2.5* (_Turbulence.y + _Turbulence.x) * _Size.y,0,0);
				for (int k=_LiquidRaySteps;k>0;k--, rpos += dir) {
//					fixed deep = exp(((rpos.y - _LevelPos)/_Size.y) * _DeepAtten);
					fixed deep = exp((rpos.y/_Size.y) * _DeepAtten);
					half n = tex3Dlod(_NoiseTex, (rpos - disp) * _Scale.z).r;
					fixed4 lc  = fixed4(_Color1.rgb, (1.0 - _Muddy) + n * _Muddy);
					lc.a *= _Color1.a;
					lc.rgb *= lc.aaa;
					lc.rgb *= deep;
					sum += lc * (1.0-sum.a);
					
					n =  tex3Dlod(_NoiseTex, (rpos - disp2) * _Scale.w ).r;
					lc  = fixed4(_Color2.rgb + max(n-_SparklingThreshold, 0) * _SparklingIntensity, (1.0 - _Muddy) + n * _Muddy);
					lc.a *= _Color2.a;
					lc.rgb *= lc.aaa;
					lc.rgb *= deep;
					sum += lc * (1.0-sum.a);
				}
			}

			// Final blend
			if (wpos.y>_LevelPos) {
				#if LIQUID_VOLUME_SMOKE
				fixed4 lfoam = sumFoam * (1.0 - sumSmoke.a);
				fixed4 liquid = sum * (1.0 - lfoam.a) * (1.0 - sumSmoke.a);
				sum = sumSmoke + lfoam + liquid;
				#else
				fixed4 liquid = sum * (1.0 - sumFoam.a);
				sum = sumFoam + liquid;
				#endif
			} else {
				fixed4 lfoam = sumFoam * (1.0 - sum.a);
				sum = sum + lfoam;
			}
			return sum;
		}
