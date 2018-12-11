using UnityEngine;
using System.Collections;

namespace LiquidVolumeFX
{
				
	public class LiquidLevelPouringSync : MonoBehaviour
	{

		public float fillSpeed = 0.01f;
		public float sinkFactor = 0.1f;
		LiquidVolume lv;
		Rigidbody rb;

		void Start ()
		{
			rb = GetComponent<Rigidbody> ();
			lv = transform.parent.GetComponent<LiquidVolume> ();
			UpdateColliderPos ();
		}

		void OnParticleCollision (GameObject other)
		{
			if (lv.level < 1f) {
				lv.level += fillSpeed;
			}
			UpdateColliderPos ();
		}

		void UpdateColliderPos ()
		{
			Vector3 pos = new Vector3 (transform.position.x, lv.liquidSurfaceYPosition - transform.localScale.y * 0.5f - sinkFactor, transform.position.z);
			rb.position = pos;
			if (lv.level >= 1f) {
				transform.localRotation = Quaternion.Euler (Random.value * 30 - 15, Random.value * 30 - 15, Random.value * 30 - 15);
			} else {
				transform.localRotation = Quaternion.Euler (0, 0, 0);
			}
		}

			
	}
}
