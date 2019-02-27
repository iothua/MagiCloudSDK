using UnityEngine;
using UnityEngine.VR;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Canvas))]
public class ScreenSpaceCanvas : MonoBehaviour
{
	#region Constants
	// The log base doesn't have any influence on the results whatsoever, as long as the same base is used everywhere.
	private const float logBase = 2f;
	#endregion

	#region Inspector Fields
	[Range(0f, 1f)]
	public float matchWidthOrHeight = 0f;

	public float distance = 1f;
	#endregion

	#region Private Fields
	private Canvas canvas;
	private RectTransform rt;

	private Vector2 referenceResolution;
	private float _scaleX = -1f;
	private float _scaleY = -1f;
	private float _width = -1f;
	private float _height = -1f;

	private bool _vr = false;
	private float _scaleFactor = 1f;
	#endregion

	#region MonoBehaviour
	// 
	void Awake()
	{
		canvas = GetComponent<Canvas>();
		rt = GetComponent<RectTransform>();
	}

	// 
	void LateUpdate()
	{
		//bool vr = (UnityEngine.XR.XRSettings.loadedDevice != VRDeviceType.None);
		//if (_vr != vr)
		//{
		//	_vr = vr;
			
		//	if (_vr)
		//	{
		//		canvas.renderMode = RenderMode.WorldSpace;
		//	}
		//	else
		//	{
		//		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		//		_scaleX = -1f;
		//		_scaleY = -1f;
		//		_width = -1f;
		//		_height = -1f;
		//	}
		//}
		
		//if (_vr)
		//{
		//	Rect pixelRect = canvas.pixelRect;
		//	referenceResolution = new Vector2(pixelRect.width, pixelRect.height);
		//	UpdateTransform(Camera.main);
		//}
	}
	#endregion

	#region Private Methods
	// 
	void UpdateTransform(Camera cam)
	{
		if (cam == null) { return; }

		Transform camTr = cam.GetComponent<Transform>();

		float width = Screen.width;
		float height = Screen.height;

		float frustumHeight = distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
		float frustumWidth = frustumHeight * 2f * cam.aspect;

		float scaleX = (frustumWidth / width);
		float scaleY = (frustumHeight / height) * 2f;

		rt.position = camTr.TransformPoint(new Vector3(0f, 0f, distance));
		rt.rotation = camTr.rotation;

		// Same approach as in Unity built-in CanvasScaler component
		float logWidth = Mathf.Log(width / referenceResolution.x, logBase);
		float logHeight = Mathf.Log(height / referenceResolution.y, logBase);
		float logWeightedAverage = Mathf.Lerp(logWidth, logHeight, matchWidthOrHeight);
		float scaleFactor = Mathf.Pow(logBase, logWeightedAverage);
		float scaleFactorInv = 1f / scaleFactor;

		float canvasWidth = width * scaleFactorInv;
		float canvasHeight = height * scaleFactorInv;

		float canvasScaleX = scaleX * scaleFactor;
		float canvasScaleY = scaleY * scaleFactor;

		if (_width != canvasWidth)
		{
			_width = canvasWidth;
			rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _width);
		}

		if (_height != canvasHeight)
		{
			_height = canvasHeight;
			rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _height);
		}

		if (_scaleX != canvasScaleX || _scaleY != canvasScaleY)
		{
			_scaleX = canvasScaleX;
			_scaleY = canvasScaleY;
			rt.localScale = new Vector3(_scaleX, _scaleY, 1f);
		}

		if (scaleFactor != _scaleFactor)
		{
			_scaleFactor = scaleFactor;
			canvas.scaleFactor = scaleFactor;
		}
	}
	#endregion
}
