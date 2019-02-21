using UnityEngine;
using System.Collections;
using HighlightingSystem;

public class HighlighterToggle : HighlighterInteractive
{
	public float delayMin = 1f;
	public float delayMax = 1f;

	private bool state = false;

	#region MonoBehaviour
	// 
	protected override void Start()
	{
		Toggle();
		StartCoroutine(ToggleRoutine());
	}

	// 
	protected override void OnValidate()
	{
		base.OnValidate();

		if (delayMin < 0f) { delayMin = 0f; }
		if (delayMax < 0f) { delayMax = 0f; }
		if (delayMin > delayMax) { delayMin = delayMax; }
	}
	#endregion

	// 
	IEnumerator ToggleRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
			Toggle();
		}
	}

	// 
	void Toggle()
	{
		if (state)
		{
			h.ConstantOffImmediate();
			state = false;
		}
		else
		{
			Color color = ColorTool.GetColor(Random.value);
			h.ConstantOnImmediate(color);
			state = true;
		}
	}
}