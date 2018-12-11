using UnityEngine;
using System.Collections;

namespace LiquidVolumeFX
{
	public class CubeSpawn : MonoBehaviour
	{

		public int instances = 150;
		public float radius = 2f;
		public float jitter = 0.5f;
		public float expansion = 0.04f;
		public float laps = 2f;

		void Start ()
		{
			for (int k=1;k<=instances;k++) {

				GameObject c = Instantiate<GameObject>(gameObject);
				c.GetComponent<CubeSpawn>().enabled = false; // prevent infinite recursion
				c.name = "Cube" + k;
				float rad = ((float)k/instances) * Mathf.PI * 2f * laps;

				float e = k * expansion;
				float x = Mathf.Cos (rad) * (radius + e);
				float z = Mathf.Sin (rad) * (radius + e);
				Vector3 jitter3 = Random.insideUnitSphere * jitter;
				c.transform.position = transform.position + new Vector3(x, 0, z) + jitter3;
				c.transform.localScale *= 1.0f - Random.value * jitter;
			}
		}
	
	}
}