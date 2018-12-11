using UnityEngine;
using System.Collections;

namespace LiquidVolumeFX {

				public class RotateLight : MonoBehaviour {

								// Use this for initialization
								void Start () {
	
								}
	
								// Update is called once per frame
								void Update () {
												float t = Mathf.PingPong (Time.time * 8f, 30) - 5;
												transform.localRotation = Quaternion.Euler (new Vector3 (2f, 180 + t, 0));
								}
				}

}