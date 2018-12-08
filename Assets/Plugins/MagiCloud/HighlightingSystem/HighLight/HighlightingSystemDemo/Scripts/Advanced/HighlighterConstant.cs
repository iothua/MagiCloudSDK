using UnityEngine;
using System.Collections;

public class HighlighterConstant : HighlighterInteractive
{
	public Color color = Color.cyan;

	#region MonoBehaviour
	// 
	protected override void OnEnable()
	{
		base.OnEnable();
		h.ConstantOnImmediate(color);
	}

	// 
	protected override void OnValidate()
	{
		base.OnValidate();
		if (h != null)
		{
			h.ConstantOnImmediate(color);
		}
	}
	#endregion
}