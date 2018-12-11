using UnityEngine;
using System.Collections;

namespace LiquidVolumeFX
{
	public class FishAnimator : MonoBehaviour
	{

		// Update is called once per frame
		void Update ()
		{
			Vector3 t = Camera.main.transform.position;
			transform.LookAt (new Vector3 (-t.x, transform.position.y, -t.z));
		}
	}

}