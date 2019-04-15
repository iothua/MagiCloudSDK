using UnityEngine;
using System.Collections.Generic;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture.Demos
{
	/// <summary>
	/// Spawns cube prefabs from a transform and removes them once they reach a maximum number
	/// </summary>
	public class SurroundCaptureDemo : MonoBehaviour
	{
		[SerializeField]
		private Transform _spawnPoint;

		[SerializeField]
		private GameObject _cubePrefab;

		[SerializeField]
		private bool _spawn = true;

		private const int MaxCubes = 48;
		private const float SpawnTime = 0.25f;

		// State
		private float _timer = SpawnTime;
		private List<GameObject> _cubes = new List<GameObject>(32);

		private void Update()
		{
			// Spawn cubes at a certain rate
			_timer -= Time.deltaTime;
			if (_timer <= 0f)
			{
				if (_spawn)
				{
					_timer = SpawnTime;
					SpawnCube();
				}

				// Remove cubes when there are too many
				if (_cubes.Count > MaxCubes || !_spawn)
				{
					RemoveCube();
				}
			}
		}

		private void SpawnCube()
		{
			Quaternion rotation = Quaternion.Euler(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
			float scale = Random.Range(0.1f, 0.6f);

			GameObject go = (GameObject)GameObject.Instantiate(_cubePrefab, _spawnPoint.position, rotation);
			Transform t = go.GetComponent<Transform>();
			go.GetComponent<Rigidbody>().AddExplosionForce(10f, _spawnPoint.position, 5f, 0f, ForceMode.Impulse);

			//AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier = 0.0F, ForceMode mode = ForceMode.Force);
			t.localScale = new Vector3(scale * 2f, scale, scale * 2f);
			t.SetParent(_spawnPoint);
			_cubes.Add(go);
		}

		private void RemoveCube()
		{
			if (_cubes.Count > 0)
			{
				// Remove the oldest cube
				GameObject go = _cubes[0];

				// Disabling the collider makes it fall through the floor - which is a neat way to hide its removal
				go.GetComponent<Collider>().enabled = false;
				_cubes.RemoveAt(0);
				StartCoroutine("KillCube", go);
			}
		}

		private System.Collections.IEnumerator KillCube(GameObject go)
		{
			yield return new WaitForSeconds(1.5f);
			Destroy(go);
		}
	}
}