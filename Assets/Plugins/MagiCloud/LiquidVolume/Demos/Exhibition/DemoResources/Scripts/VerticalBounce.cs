using UnityEngine;
using System.Collections;

namespace LiquidVolumeFX
{
	public class VerticalBounce : MonoBehaviour
	{

		[Range(0f,0.1f)]
		public float acceleration = 0.1f;

		float direction = 1f;
		float y, speed = 0.01f;

		void Update ()
		{
			transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
			y += speed;
			direction = (y<0) ? 1f: -1f;
			speed += Time.deltaTime * direction * acceleration;
		}
	}

}