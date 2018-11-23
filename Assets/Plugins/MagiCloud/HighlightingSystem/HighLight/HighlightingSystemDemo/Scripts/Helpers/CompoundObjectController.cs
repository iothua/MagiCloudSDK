using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HighlightingSystem;

public class CompoundObjectController : MonoBehaviour
{
	// Reference to meshes and shaders
	public Mesh[] meshes;
	public Shader[] shaders;

	// Cached transform component
	private Transform tr;
	
	// Cached list of child objects
	private List<GameObject> objects;
	
	private int shaderIndex = 0;

	private Highlighter h;

	#region MonoBehaviour
	// 
	void Start()
	{
		tr = GetComponent<Transform>();
		objects = new List<GameObject>();
		h = GetComponent<Highlighter>();
	}
	#endregion

	// 
	public void AddObject()
	{
		Mesh m = meshes[Random.Range(0, meshes.Length)];
		GameObject o = new GameObject("SubObject");
		MeshFilter mf = o.AddComponent<MeshFilter>();
		mf.mesh = m;
		MeshCollider mc = o.AddComponent<MeshCollider>();
		mc.sharedMesh = m;
		MeshRenderer mr = o.AddComponent<MeshRenderer>();
		mr.material = new Material(shaders[0]);
		Transform t = o.GetComponent<Transform>();
		t.parent = tr;
		t.localPosition = Random.insideUnitSphere * 2f;
		objects.Add(o);
		
		// Reinitialize highlighting materials, because child objects have changed
		h.ReinitMaterials();
	}

	// 
	public void ChangeMaterial()
	{
		if (objects.Count < 1) { AddObject(); }

		shaderIndex++;
		if (shaderIndex >= shaders.Length) { shaderIndex = 0; }
		Shader shader = shaders[shaderIndex];

		foreach (GameObject obj in objects)
		{
			Renderer renderer = obj.GetComponent<Renderer>();
			renderer.material = new Material(shader);
		}
		
		// Reinitialize highlightable materials, because material(s) have changed
		h.ReinitMaterials();
	}

	// 
	public void ChangeShader()
	{
		if (objects.Count < 1) { AddObject(); }

		shaderIndex++;
		if (shaderIndex >= shaders.Length) { shaderIndex = 0; }
		Shader shader = shaders[shaderIndex];

		foreach (GameObject obj in objects)
		{
			Renderer renderer = obj.GetComponent<Renderer>();
			renderer.material.shader = shader;
		}
		
		// Reinitialize highlightable materials, because shader(s) have changed
		h.ReinitMaterials();
	}

	// 
	public void RemoveObject()
	{
		if (objects.Count < 1) { return; }
		
		GameObject toRemove = objects[objects.Count - 1];
		objects.Remove(toRemove);
		Destroy(toRemove);
		
		// Reinitialize highlighting materials, because child objects have changed
		h.ReinitMaterials();
	}
}
