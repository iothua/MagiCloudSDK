using UnityEngine;
using System.Collections;

public class HighlighterFlashing : HighlighterInteractive
{
	public Color flashingStartColor = Color.blue;
	public Color flashingEndColor = Color.cyan;
	public float flashingDelay = 2.5f;
	public float flashingFrequency = 2f;

	private Coroutine coroutine;

	#region MonoBehaviour
	// 
	protected override void Start()
	{
		base.Start();
		coroutine = StartCoroutine(DelayFlashing());
	}

	// 
	protected override void OnValidate()
	{
		base.OnValidate();
		// Update flashing parameters only if highlighter was initialized (in Awake) and coroutine is already fired (after delay)
		if (h != null && coroutine == null)
		{
			h.FlashingOn(flashingStartColor, flashingEndColor, flashingFrequency);
		}
	}
	#endregion

	// 
	protected IEnumerator DelayFlashing()
	{
		yield return new WaitForSeconds(flashingDelay);

		coroutine = null;
		
		// Start object flashing after delay
		h.FlashingOn(flashingStartColor, flashingEndColor, flashingFrequency);
	}
}
