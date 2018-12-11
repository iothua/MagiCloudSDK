using UnityEngine;
using System.Collections;

namespace LiquidVolumeFX
{
	public class FlaskAnimator : MonoBehaviour
	{

		public float speed = 0.01f;
		public Vector3 initialPosition = Vector3.down * 4f;
		public Vector3 finalPosition = Vector3.zero;
		public float duration = 5f;

		public float delay = 6f;


		[Range(0f,1f)]
		public float level = 0f;

		[Range(0f,1f)]
		public float minRange = 0.05f;

		[Range(0f,1f)]
		public float maxRange = 0.95f;

		[Range(0f,1f)]
		public float acceleration = 0.04f;

		[Range(0f, 1f)]
		public float rotationSpeed = 0.25f;

		[Range(0f, 2f)]
		public float alphaDuration = 2f;

		[Range(0,1f)]
		public float finalRefractionBlur = 0.75f;


		// -- internal fields
		LiquidVolume liquid;
		float direction = 1.0f;

		// Use this for initialization
		void Awake ()
		{
			liquid = GetComponent<LiquidVolume> ();
			level = liquid.level;
			liquid.alpha = 0;
		}
	
		void Update ()
		{
			float t = duration > 0 ? (Time.time - delay) / duration : 1f;
			if (t>=1f) {
				level += direction * speed;
				if (level<minRange || level>maxRange) direction *= -1.0f;

				direction += Mathf.Sign(0.5f - level) * acceleration;
				level = Mathf.Clamp(level, minRange, maxRange);
				liquid.level = level;

				t = alphaDuration > 0 ? Mathf.Clamp01 ( (Time.time - duration - delay) / alphaDuration) : 1f;
				liquid.alpha = t;
				liquid.blurIntensity = t * finalRefractionBlur;

			} else {
				if (initialPosition!=finalPosition) {
					transform.position = Vector3.Lerp(initialPosition, finalPosition, t);
				}
			}
			transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed * Mathf.Rad2Deg, Space.Self);
		}
	}

}