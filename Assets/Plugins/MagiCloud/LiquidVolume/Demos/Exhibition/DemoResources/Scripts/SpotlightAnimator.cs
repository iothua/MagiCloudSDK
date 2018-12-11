using UnityEngine;
using System.Collections;

namespace LiquidVolumeFX
{
	public class SpotlightAnimator : MonoBehaviour
	{

		public float lightOnDelay = 2f;
		public float targetIntensity = 3.5f;
		public float initialIntensity = 0f;
		public float duration = 3f;

		public float nextColorInterval = 2f;
		public float colorChangeDuration = 2f;

		Light spotLight;
		float elapsedNextColor;
		float lastColorChange, colorChangeStarted;
		Color currentColor, nextColor;
		bool changingColor;

		void Awake ()
		{
			spotLight = GetComponent<Light> ();
			spotLight.intensity = 0;
		}
	
		// Update is called once per frame
		void Update ()
		{
			if (Time.time < lightOnDelay)
				return;

			float t = (Time.time - lightOnDelay) / duration;
			spotLight.intensity = Mathf.Lerp (initialIntensity, targetIntensity, t);

			if (Time.time - lastColorChange > nextColorInterval) {
				if (changingColor) {
					t = (Time.time - colorChangeStarted) / colorChangeDuration;
					if (t>=1f) {
						changingColor = false;
						lastColorChange = Time.time;
					}
					spotLight.color = Color.Lerp(currentColor, nextColor, t);
				} else {
					currentColor = spotLight.color;
					const float b = 0.25f;
					nextColor = new Color( Mathf.Clamp01 (Random.value + b), Mathf.Clamp01 (Random.value + b), Mathf.Clamp01( Random.value + b), 1f);
					changingColor = true;
					colorChangeStarted = Time.time;
				}
			}

	
		}
	}
}
