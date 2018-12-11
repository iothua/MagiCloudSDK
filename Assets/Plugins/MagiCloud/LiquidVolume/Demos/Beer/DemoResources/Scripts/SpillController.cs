using UnityEngine;
using System.Collections;

/// <summary>
/// Shows the spill point on the glass when it's rotated.
/// </summary>
namespace LiquidVolumeFX
{
	public class SpillController : MonoBehaviour
	{

		public GameObject spill;

		LiquidVolume lv;

		void Start ()
		{
			lv = GetComponent<LiquidVolume> ();
		}

		void Update ()
		{
			const float rotationSpeed = 10f;
			if (Input.GetKey (KeyCode.LeftArrow)) {
				transform.Rotate (Vector3.forward * Time.deltaTime * rotationSpeed);
			}
			if (Input.GetKey (KeyCode.RightArrow)) {
				transform.Rotate (-Vector3.forward * Time.deltaTime * rotationSpeed);
			}
		}


		void FixedUpdate ()
		{
												
			Vector3 spillPos;
			float spillAmount;
			if (lv.GetSpillPoint (out spillPos, out spillAmount)) {
				const int drops = 15;
				for (int k = 0; k < drops; k++) {
					GameObject oneSpill = Instantiate (spill) as GameObject;
					oneSpill.transform.position = spillPos + Random.insideUnitSphere * 0.01f;
					oneSpill.transform.localScale *= Random.Range (0.45f, 0.65f);
					oneSpill.GetComponent<Renderer> ().material.color = Color.Lerp (lv.liquidColor1, lv.liquidColor2, Random.value);
					Vector3 force = new Vector3 (Random.value - 0.5f, Random.value * 0.1f - 0.2f, Random.value - 0.5f);
					oneSpill.GetComponent<Rigidbody> ().AddForce (force);
					StartCoroutine (DestroySpill (oneSpill));
				}
				lv.level -= spillAmount / 10f + 0.001f;
			}
		}


		IEnumerator DestroySpill (GameObject spill)
		{
			yield return new WaitForSeconds (1f);
			Destroy (spill);
		}
	}
}
