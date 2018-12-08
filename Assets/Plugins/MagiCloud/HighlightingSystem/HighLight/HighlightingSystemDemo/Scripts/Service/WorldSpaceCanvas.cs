using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WorldSpaceCanvas : MonoBehaviour
{
	public float scale = 1f;

	private Transform tr;

	private Material _material;
	private Material material
	{
		get
		{
			if (_material == null)
			{
				_material = new Material(Graphic.defaultGraphicMaterial);
				_material.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always);
			}
			return _material;
		}
	}

	#region MonoBehaviour
	// 
	void Awake()
	{
		tr = GetComponent<Transform>();
	}

	// 
	void Start()
	{
		SetMaterial();
	}

	// 
	void LateUpdate()
	{
		UpdateTransform(Camera.main);
	}
	#endregion

	#region Private Methods
	// 
	void SetMaterial()
	{
		Graphic[] graphic = GetComponentsInChildren<Graphic>();
		for (int i = 0; i < graphic.Length; i++)
		{
			Graphic g = graphic[i];
			g.material = material;
		}
	}

	// 
	void UpdateTransform(Camera cam)
	{
		if (cam == null) { return; }

		Transform camTr = cam.GetComponent<Transform>();

		tr.rotation = camTr.rotation;
		float s = scale * Vector3.Dot((tr.position - camTr.position), camTr.forward);
		tr.localScale = new Vector3(s, s, s);
	}
	#endregion
}