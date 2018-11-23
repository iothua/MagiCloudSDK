using UnityEngine;
using System.Collections;
using HighlightingSystem;

public class HighlighterOccluder : HighlighterBase
{
	#region MonoBehaviour
	// 
	protected override void OnEnable()
	{
		base.OnEnable();
		h.occluder = true;
	}
	#endregion
}
