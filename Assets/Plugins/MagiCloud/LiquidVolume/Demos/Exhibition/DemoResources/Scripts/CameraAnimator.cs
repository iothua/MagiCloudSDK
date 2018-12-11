using UnityEngine;
using System.Collections;

namespace LiquidVolumeFX
{
	public class CameraAnimator : MonoBehaviour
	{

		public float baseHeight = 0.6f;
		public float speedY = 0.005f;
		public float speedX = 5f;
		public float distAcceleration = 0.0002f;
		public float distSpeed = 0.0001f;
		public Vector3 lookAt;

		float y;
		float dy = 0;
		float distDirection = 1f;
		float distSum;

		void Start() {
			y = transform.position.y;
		}

		void Update ()
		{
			transform.RotateAround (lookAt, Vector3.up, Time.deltaTime * speedX);
			y += dy;
			dy -= (transform.position.y - baseHeight) * Time.deltaTime * speedY;
			transform.position = new Vector3(transform.position.x, y, transform.position.z);

			Quaternion prevRot = transform.rotation;
			transform.LookAt(lookAt);
			transform.rotation = Quaternion.Lerp(prevRot, transform.rotation, 0.2f);

			transform.position += transform.forward * distSum;
			distSum += distSpeed;
			distDirection = (distSum<0) ? 1f: -1f;
			distSpeed += Time.deltaTime * distDirection * distAcceleration;


		}
	}
}