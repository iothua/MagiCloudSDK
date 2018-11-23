using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Revealer for GameObjects with HighlighterItem components
[DisallowMultipleComponent]
public class HighlighterRevealer : MonoBehaviour
{
	public float radius = 2f;
	public LayerMask layerMask = -1;

	private HashSet<HighlighterItem> items = new HashSet<HighlighterItem>();
	private Transform tr;

	#region Radius Visualization
	public Mesh sphereMesh;
	public Material sphereMaterial;

	// 
	void Update()
	{
		if (sphereMesh != null && sphereMaterial != null)
		{
			float s = radius * 2f;
			Matrix4x4 m = Matrix4x4.TRS(tr.position, Quaternion.identity, new Vector3(s, s, s));
			Graphics.DrawMesh(sphereMesh, m, sphereMaterial, 0);
		}
	}
	#endregion

	#region MonoBehaviour
	// 
	void Awake()
	{
		tr = GetComponent<Transform>();
	}

	// After all movement finishes
	void LateUpdate()
	{
		Clear();

		// Collect HighlightableItem components in radius and reveal them
		Collider[] colliders = Physics.OverlapSphere(tr.position, radius, layerMask);
		for (int i = 0, l = colliders.Length; i < l; i++)
		{
			HighlighterItem hi = colliders[i].GetComponentInParent<HighlighterItem>();
			if (hi != null && !items.Contains(hi))
			{
				hi.Reveal();
				items.Add(hi);
			}
		}
	}

	// 
	void OnDisable()
	{
		Clear();
	}

	// 
	void OnValidate()
	{
		if (radius < 0.0001f) { radius = 0.0001f; }
	}

	// 
	void OnDrawGizmos()
	{
		if (enabled)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, radius);
		}
	}
	#endregion

	#region Private Methods
	// 
	private void Clear()
	{
		var e = items.GetEnumerator();
		while (e.MoveNext())
		{
			HighlighterItem hi = e.Current;
			hi.Hide();
		}
		items.Clear();
	}
	#endregion
}
