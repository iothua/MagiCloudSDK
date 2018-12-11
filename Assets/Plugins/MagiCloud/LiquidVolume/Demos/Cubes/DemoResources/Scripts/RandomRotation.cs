using UnityEngine;
using System.Collections;

namespace LiquidVolumeFX
{
	public class RandomRotation : MonoBehaviour
	{
		[Range(1f, 50)]
		public float speed = 10f;

		[Range(1f,30f)]
		public float randomChangeInterval = 10f;

		float lastTime;
		Vector3 v;
		float randomization;

		void Start() {
			randomization = Random.value;
		}

		void Update ()
		{
			if (Time.time>lastTime) {
				lastTime = Time.time + randomChangeInterval + randomization;
				v = new Vector3 (Random.value, Random.value, Random.value);
			}
			transform.Rotate (v * Time.deltaTime * speed);
	
		}
	}
}
