#include "LVLiquidPassBase.cginc"

		sampler3D _NoiseTex;

		fixed4 raymarch(float4 vertex, float3 rd, float t0, float t1) {

			// ray-march smoke
			fixed4 sumSmoke = fixed4(0,0,0,0);
			float stepSize = (t1 - t0) / (float)_SmokeRaySteps;
			float4 dir  = float4(rd * stepSize, 0);
			float4 rpos = float4(wsCameraPos + rd * t0, 0);
			float4 disp = float4(0, _Time.x * _Turbulence.x * _Size.y * _SmokeSpeed, 0, 0);
			for (int k=_SmokeRaySteps;k>0;k--, rpos += dir) {
				fixed deep = exp(((_LevelPos - rpos.y)/_Size.y) * _SmokeAtten);
				half n = tex3Dlod(_NoiseTex, (rpos - disp) * _Scale.x).r;
				fixed4 lc  = fixed4(_SmokeColor.rgb, n * _SmokeColor.a);
				lc.rgb *= lc.aaa;
				lc *= deep;
				sumSmoke += lc * (1.0-sumSmoke.a);
			}
			return sumSmoke;
		}
