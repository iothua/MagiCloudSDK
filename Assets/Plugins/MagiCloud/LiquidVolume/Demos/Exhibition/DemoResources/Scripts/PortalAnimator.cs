using UnityEngine;
using System.Collections;

namespace LiquidVolumeFX
{
	public class PortalAnimator : MonoBehaviour
	{

		public float delay = 2f;
		public float duration = 1f;
		public float delayFadeOut = 4;

		Vector3 scale;

		void Start() {
			scale = transform.localScale;
			transform.localScale = Vector3.zero;
		}

		void Update ()
		{
			if (Time.time<delay) return;

			float t;
			if (Time.time > delayFadeOut) {
				t = 1.0f - (Time.time - delayFadeOut) / duration;
			} else {
				t = (Time.time - delay) / duration;
			}
			transform.localScale = Mathf.Clamp01 (t) * scale;
		}
	}
}