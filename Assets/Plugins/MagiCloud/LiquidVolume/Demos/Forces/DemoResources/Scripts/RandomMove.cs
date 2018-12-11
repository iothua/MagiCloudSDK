using UnityEngine;
using System.Collections;

namespace LiquidVolumeFX
{
	public class RandomMove : MonoBehaviour
	{

		[Range(0,10f)]
		public float right = 2f;

		[Range(0,10f)]
		public float left = -2f;
		
		[Range(0,10f)]
		public float back = 2f;
		
		[Range(0,10f)]
		public float front = -1f;

		[Range(0,0.2f)]
		public float speed = 0.5f;

		[Range(0.1f, 2f)]
		public float randomSpeed;

		public bool automatic;


		Vector3 velocity = Vector3.zero;
		Vector3 accel = Vector3.zero;
		int flaskType = 0;

		void Update ()
		{
			if (Input.GetKeyDown(KeyCode.F)) {
				flaskType++;
				if (flaskType>=3) flaskType = 0;
				transform.Find("SphereFlask").gameObject.SetActive(flaskType==0);
				transform.Find("CylinderFlask").gameObject.SetActive(flaskType==1);
				transform.Find("CubeFlask").gameObject.SetActive(flaskType==2);
			}

			if (automatic) {
				if (Random.value > 0.99f) {
					accel = Vector3.right * (speed + (Random.value - 0.5f) * randomSpeed);
				}
			} else {
				if (Input.GetKey(KeyCode.RightArrow)) {
					accel = Vector3.right * speed;
				} else if (Input.GetKey(KeyCode.LeftArrow)) {
					accel = Vector3.left * speed;
				} else if (Input.GetKey(KeyCode.UpArrow)) {
					accel = Vector3.forward * speed;
				} else if (Input.GetKey(KeyCode.DownArrow)) {
					accel = Vector3.back * speed;
				} else {
					accel = Vector3.zero;
				}
			}
			velocity += accel;
			velocity *= 0.95f;
			transform.localPosition += velocity;
			if (transform.localPosition.x > right) {
				transform.localPosition = new Vector3(right, transform.localPosition.y, transform.localPosition.z);
				accel *= -1;
			}
			if (transform.localPosition.x < left) {
				transform.localPosition = new Vector3(left, transform.localPosition.y, transform.localPosition.z);
				accel *= -1;
			}
			if (transform.localPosition.z > back) {
				transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, back);
				accel *= -1;
			}
			if (transform.localPosition.z < front) {
				transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, front);
				accel *= -1;
			}

		}
	}

}