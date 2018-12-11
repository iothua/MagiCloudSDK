using UnityEngine;
using System.Collections;

namespace LiquidVolumeFX
{
	public class CylinderManager : MonoBehaviour
	{

		public float startingDelay = 1f;

		public int numCylinders = 16;
		public float scale = 0.2f;
		public float heightMultiplier = 2f;
		public float circleRadius = 1.75f;

		void Update ()
		{
			if (Time.time < startingDelay) return;

			for (int k=0;k<numCylinders;k++) {
				GameObject cylinder = Instantiate(Resources.Load<GameObject>("Prefabs/CylinderFlask"));
				cylinder.hideFlags = HideFlags.DontSave;
				cylinder.transform.SetParent(transform, false);
				cylinder.transform.localScale = new Vector3(scale, scale * heightMultiplier, scale);
				float x = Mathf.Cos (  ((float)k/numCylinders) * Mathf.PI * 2.0f ) * circleRadius;
				float z = Mathf.Sin (  ((float)k/numCylinders) * Mathf.PI * 2.0f ) * circleRadius;
				cylinder.transform.position = new Vector3(x, -2f, z);
				FlaskAnimator fa = cylinder.AddComponent<FlaskAnimator>();
				fa.initialPosition = cylinder.transform.position;
				fa.finalPosition = cylinder.transform.position + Vector3.up;
				fa.duration = 5f + k * 0.5f;
				fa.acceleration = 0.001f;
				fa.delay = 4f;

				LiquidVolume lv = cylinder.GetComponent<LiquidVolume>();
				lv.liquidColor1 = new Color(Random.value, Random.value, Random.value, Random.value);
				lv.liquidColor2 = new Color(Random.value, Random.value, Random.value, Random.value);
				lv.turbulence2 = 0;
				lv.refractionBlur = false;
			}

			Destroy (this);
	
		}
	}
}