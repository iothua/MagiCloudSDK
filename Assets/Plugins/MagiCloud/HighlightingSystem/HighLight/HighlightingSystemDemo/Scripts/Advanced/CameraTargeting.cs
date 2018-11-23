using UnityEngine;
using System.Collections;

public interface IHighlightingTarget
{
	void OnHighlightingFire1Down();
	void OnHighlightingFire1Held();
	void OnHighlightingFire1Up();
	
	void OnHighlightingFire2Down();
	void OnHighlightingFire2Held();
	void OnHighlightingFire2Up();
	
	void OnHighlightingMouseOver();
}

[RequireComponent(typeof(Camera))]
public class CameraTargeting : MonoBehaviour
{
	// Which layers targeting ray must hit (-1 = everything)
	public LayerMask targetingLayerMask = -1;
	
	// Targeting ray length
	private float targetingRayLength = Mathf.Infinity;
	
	// Camera component reference
	private Camera cam;

	// Button names (for Input Manager)
	static private readonly string buttonFire1 = "Fire1";
	static private readonly string buttonFire2 = "Fire2";

	// 
	void Awake()
	{
		cam = GetComponent<Camera>();
	}

	// 
	void Update()
	{
		TargetingRaycast();
	}

	// 
	public void TargetingRaycast()
	{
		// Current target object transform component
		Transform targetTransform = null;
		
		// If camera component is available
		if (cam != null)
		{
			RaycastHit hitInfo;
			
			// Create a ray from mouse coords
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);

			// Targeting raycast
			if (Physics.Raycast(ray, out hitInfo, targetingRayLength, targetingLayerMask.value))
			{
				// Cache what we've hit
				targetTransform = hitInfo.collider.transform;
			}
		}
		
		// If we've hit an object during raycast
		if (targetTransform != null)
		{
			// And this object has component, which implements IHighlightingTarget interface
			IHighlightingTarget ht = targetTransform.GetComponentInParent<IHighlightingTarget>();
			if (ht != null)
			{
				if (Input.GetButtonDown(buttonFire1)) { ht.OnHighlightingFire1Down(); }
				else if (Input.GetButton(buttonFire1)) { ht.OnHighlightingFire1Held(); }
				else if (Input.GetButtonUp(buttonFire1)) { ht.OnHighlightingFire1Up(); }

				if (Input.GetButtonDown(buttonFire2)) { ht.OnHighlightingFire2Down(); }
				else if (Input.GetButton(buttonFire2)) { ht.OnHighlightingFire2Held(); }
				else if (Input.GetButtonUp(buttonFire2)) { ht.OnHighlightingFire2Up(); }

				ht.OnHighlightingMouseOver();
			}
		}
	}
}
