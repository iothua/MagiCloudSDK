using UnityEngine;
using System.Collections;
using HighlightingSystem;

public class HighlighterBase : MonoBehaviour
{
	public bool seeThrough = true;
	
	protected Highlighter h;
	
	#region MonoBehaviour
	// 
	protected virtual void Awake()
	{
		h = GetComponent<Highlighter>();
		if (h == null) { h = gameObject.AddComponent<Highlighter>(); }
	}
	
	// 
	protected virtual void OnEnable()
	{
		h.seeThrough = seeThrough;
	}
	
	// 
	protected virtual void Start()
	{
		
	}
	
	// 
	protected virtual void Update()
	{

	}
	
	// 
	protected virtual void OnValidate()
	{
		if (h != null)
		{
			h.seeThrough = seeThrough;
		}
	}
	#endregion
}