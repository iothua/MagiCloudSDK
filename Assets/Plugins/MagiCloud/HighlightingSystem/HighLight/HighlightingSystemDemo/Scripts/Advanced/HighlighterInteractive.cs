using UnityEngine;
using System.Collections;
using HighlightingSystem;

public class HighlighterInteractive : HighlighterBase, IHighlightingTarget
{
	#region MonoBehaviour
	// 
	protected override void Update()
	{
		base.Update();

		// Fade in/out constant highlighting with button '1'
		if (Input.GetKeyDown(KeyCode.Alpha1)) { h.ConstantSwitch(); }

		// Turn on/off constant highlighting with button '2'
		else if (Input.GetKeyDown(KeyCode.Alpha2)) { h.ConstantSwitchImmediate(); }
		
		// Turn off all highlighting modes with button '3'
		if (Input.GetKeyDown(KeyCode.Alpha3)) { h.Off(); }
	}
	#endregion

	#region IHighlightingTarget implementation
	// 
	public virtual void OnHighlightingFire1Down()
	{
		// Switch flashing
		h.FlashingSwitch();
	}
	public virtual void OnHighlightingFire1Held() { }
	public virtual void OnHighlightingFire1Up() { }

	// 
	public virtual void OnHighlightingFire2Down() { }
	public virtual void OnHighlightingFire2Held() { }
	public virtual void OnHighlightingFire2Up()
	{
		// Switch seeThrough mode
		h.seeThrough = !h.seeThrough;
	}

	// 
	public virtual void OnHighlightingMouseOver()
	{
		// Highlight object for one frame
		h.On(Color.red);
	}
	#endregion
}