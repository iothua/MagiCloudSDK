using UnityEngine;
using System.Collections;
using HighlightingSystem;

// Use with HighlighterRevealer component
[DisallowMultipleComponent]
public class HighlighterItem : MonoBehaviour, IHighlightingTarget
{
	public bool seeThrough = true;
	public Color revealColor = new Color(0f, 1f, 1f, 1f);
	
	private Highlighter h;
	private int revealCount = 0;

	#region MonoBehaviour
	// 
	void Awake()
	{
		h = GetComponent<Highlighter>();
		if (h == null) { h = gameObject.AddComponent<Highlighter>(); }
	}

	// 
	void OnEnable()
	{
		h.seeThrough = seeThrough;
	}

	// 
	void OnValidate()
	{
		if (h != null)
		{
			h.seeThrough = seeThrough;
		}
	}
	#endregion

	#region Public Methods
	// 
	public void Reveal()
	{
		revealCount++;
		UpdateInternal();
	}

	// 
	public void Hide()
	{
		revealCount = Mathf.Max(0, revealCount-1);
		UpdateInternal();
	}
	#endregion

	#region Private Methods
	// 
	private void UpdateInternal()
	{
		if (revealCount > 0)
		{
			h.ConstantOn(revealColor);
		}
		else
		{
			h.ConstantOff();
		}
	}
	#endregion

	#region IHighlightingTarget implementation
	public void OnHighlightingFire1Down() { }
	public void OnHighlightingFire1Held() { }
	public void OnHighlightingFire1Up() { }
	public void OnHighlightingFire2Down() { }
	public void OnHighlightingFire2Held() { }
	public void OnHighlightingFire2Up() { }

	public void OnHighlightingMouseOver()
	{
		h.On(Color.red);
	}
	#endregion
}