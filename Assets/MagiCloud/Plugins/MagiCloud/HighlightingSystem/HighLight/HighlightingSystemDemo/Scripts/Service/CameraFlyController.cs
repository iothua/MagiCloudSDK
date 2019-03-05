using UnityEngine;
using System.Collections;

public class CameraFlyController : MonoBehaviour
{
	private float speed = 4f;
	
	private Transform tr;

	private bool rmbDownInRect;
	private Vector3 mpStart;
	private Vector3 originalRotation;

	private float t;

	private Vector3 mousePosition
	{
		get
		{
			Camera cam = GetComponent<Camera>();
			return cam == null ? Vector3.Scale(Input.mousePosition, new Vector3(1f/Screen.width, 1f/Screen.height, 1f)) : cam.ScreenToViewportPoint(Input.mousePosition);
		}
	}

	// 
	void Awake()
	{
		tr = GetComponent<Transform>();
	}

	// 
	void OnEnable()
	{
		t = Time.realtimeSinceStartup;
	}

	// 
	void Update()
	{
		Vector3 mp = mousePosition;
		bool rmbDown = Input.GetMouseButtonDown(1);
		bool rmbHeld = Input.GetMouseButton(1);
		bool mouseInCameraRect = mp.x >= 0f && mp.x < 1f && mp.y >= 0f && mp.y < 1f;
		rmbDownInRect = (rmbDownInRect && rmbHeld) || (mouseInCameraRect && rmbDown);

		float timeNow = Time.realtimeSinceStartup;
		float dT = timeNow - t;
		t = timeNow;

		// Movement
		if (rmbDownInRect || (!rmbHeld && mouseInCameraRect))
		{
			float forward = 0f;
			float right = 0f;
			float up = 0f;
			
			if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) { forward += 1f; }
			if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) { forward -= 1f; }
			
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) { right += 1f; }
			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) { right -= 1f; }
			
			if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Space)) { up += 1f; }
			if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.C)) { up -= 1f; }
			
			float speedMultiplier = (Input.GetKey(KeyCode.LeftShift) ? 2f : 1f);
			tr.position += tr.TransformDirection(new Vector3(right, up, forward) * speed * speedMultiplier * dT);
		}

		// Rotation
		if (rmbDownInRect)
		{
			// Right Mouse Button Down
			if (rmbDown)
			{
				originalRotation = tr.localEulerAngles;
				mpStart = mp;
			}
			
			// Right Mouse Button Hold
			if (rmbHeld)
			{
				Vector2 offs = new Vector2((mp.x - mpStart.x), (mpStart.y - mp.y));
				tr.localEulerAngles = originalRotation + new Vector3(offs.y * 360f, offs.x * 360f, 0f);
			}
		}
	}
}
