#if UNITY_5_5_OR_NEWER 
	#define AVPRO_MOVIECAPTURE_UNITYPROFILER_55
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture
{
	/// <summary>
	/// Capture to a stereo 360 view from camera in equi-rectangular format.
	/// Based on Google's Omni-direction Stereo paper: https://developers.google.com/vr/jump/rendering-ods-content.pdf
	/// </summary>
	[AddComponentMenu("AVPro Movie Capture/Capture From Camera 360 Stereo ODS (VR)", 101)]
	public class CaptureFromCamera360ODS : CaptureBase
	{
		[System.Serializable]
		public class Settings
		{
			[SerializeField]
			public Camera camera;

			[SerializeField]
			[Tooltip("Render 180 degree equirectangular instead of 360 degrees.  Also faster rendering")]
			public bool render180Degrees = false;	

			[SerializeField]
			[Tooltip("Makes assumption that 1 Unity unit is 1m")]
			public float ipd = 0.064f;

			[SerializeField]
			[Tooltip("Higher value meant less slices to render, but can affect quality.")]
			public int pixelSliceSize = 1;

			[SerializeField]
			[Range(1, 31)]
			[Tooltip("May need to be increased to work with some post image effects. Value is in pixels.")]
			public int paddingSize = 1;

			[SerializeField]
			public CameraClearFlags cameraClearMode = CameraClearFlags.Color;

			[SerializeField]
			public Color cameraClearColor = Color.black;

			[SerializeField]
			public Behaviour[] cameraImageEffects;
		}

		[SerializeField]
		private Settings _settings = new Settings();

		public Settings Setup
		{
			get { return _settings; }
		}

		// State

		private int _eyeWidth = 1920;
		private int _eyeHeight = 1080;

		private Transform _cameraGroup;
		private Camera _leftCameraTop;
		private Camera _leftCameraBot;
		private Camera _rightCameraTop;
		private Camera _rightCameraBot;

		private RenderTexture _final;
		private System.IntPtr _targetNativePointer = System.IntPtr.Zero;
		private Material _finalMaterial;
		private int _propSliceCenter;

		public CaptureFromCamera360ODS()
		{
			// Override the default values to match more common use cases for this capture component
			_isRealTime = false;
			_renderResolution = Resolution.POW2_4096x4096;

			/*_settings.camera = this.GetComponent<Camera>();
			if (_settings.camera != null)
			{
				_settings.cameraClearMode = _settings.camera.clearFlags;
				_settings.cameraClearColor = _settings.camera.backgroundColor;
			}*/
		}

		public void SetCamera(Camera camera)
		{
			_settings.camera = camera;
			// TODO: add support for camera chains
		}

		public override void Start()
		{
			Shader mergeShader = Shader.Find("Hidden/AVProMovieCapture/ODSMerge");
			if (mergeShader != null)
			{
				_finalMaterial = new Material(mergeShader);
			}
			else
			{
				Debug.LogError("[AVProMovieCapture] Can't find Hidden/AVProMovieCapture/ODSMerge shader");
			}

			_propSliceCenter = Shader.PropertyToID("_sliceCenter");

			base.Start();
		}

		private Camera CreateEye(Camera camera, string gameObjectName, float yRot, float xOffset, int cameraTargetHeight, int cullingMask, float fov, float aspect, int aalevel)
		{
			bool isCreated = false;
			if (camera == null)
			{
				GameObject eye = new GameObject(gameObjectName);
				eye.transform.parent = _cameraGroup;
				eye.transform.rotation = Quaternion.AngleAxis(-yRot, Vector3.right);
				eye.transform.localPosition = new Vector3(xOffset, 0f, 0f);
				// NOTE: We copy the hideFlags otherwise when instantiated by the Movie Capture window which has the DontSave flag
				// Unity throws an error about destroying transforms when coming out of play mode.
				eye.hideFlags = this.gameObject.hideFlags;
				camera = eye.AddComponent<Camera>();
				isCreated = true;
			}

			camera.fieldOfView = fov;
			camera.aspect = aspect;
			camera.clearFlags = _settings.cameraClearMode;
			camera.backgroundColor = _settings.cameraClearColor;
			camera.cullingMask = cullingMask;
			camera.useOcclusionCulling = _settings.camera.useOcclusionCulling;
			camera.renderingPath = _settings.camera.renderingPath;
#if UNITY_5_6_OR_NEWER
			camera.allowHDR = _settings.camera.allowHDR;
			camera.allowMSAA = _settings.camera.allowMSAA;
			if (camera.renderingPath == RenderingPath.DeferredShading 
#if AVPRO_MOVIECAPTURE_DEFERREDSHADING
				|| camera.renderingPath == RenderingPath.DeferredLighting
#endif
				)
			{
				camera.allowMSAA = false;
			}
#endif

			{
				int textureWidth = _settings.pixelSliceSize + 2 * _settings.paddingSize;
				int textureHeight = cameraTargetHeight;

				if (camera.targetTexture != null)
				{
					camera.targetTexture.DiscardContents();
					if (camera.targetTexture.width != textureWidth || camera.targetTexture.height != textureHeight || camera.targetTexture.antiAliasing != aalevel)
					{
						RenderTexture.ReleaseTemporary(camera.targetTexture);
						camera.targetTexture = null;
					}
				}
				if (camera.targetTexture == null)
				{
					camera.targetTexture = RenderTexture.GetTemporary(textureWidth, textureHeight, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default, aalevel);
				}
			}

			// Disable the camera as we render it manually
			camera.enabled = false;

			// Copy any image effects to the new cameras
			if (isCreated)
			{
				// TODO: make it so the components can be added/updated each time if they have changed
				if (_settings.cameraImageEffects != null)
				{
					for (int i = 0; i < _settings.cameraImageEffects.Length; i++)
					{
						Behaviour origComponent = _settings.cameraImageEffects[i];
						if (origComponent != null)
						{
							if (origComponent.enabled)
							{
#if UNITY_EDITOR
								Behaviour newComponent = (Behaviour)camera.gameObject.AddComponent(origComponent.GetType());
								// TODO: we need to copy any post image effect component fields here via reflection? or perhaps use prefabs?
								UnityEditor.EditorUtility.CopySerialized(origComponent, newComponent);

								// Some effects (such as MICHAËL JIMENEZ HBAO) need to be toggled to apply the settings
								newComponent.enabled = false;
								newComponent.enabled = true;
#endif
							}
						}
						else
						{
							Debug.LogWarning("[AVProMovieCapture] Image effect is null");
						}
					}
				}
			}

			return camera;
		}

		public override void UpdateFrame()
		{
			if (_useWaitForEndOfFrame)
			{
				if (_capturing && !_paused)
				{
					StartCoroutine(FinalRenderCapture());
				}
			}
			else
			{
				Capture();
			}
			base.UpdateFrame();
		}

		private IEnumerator FinalRenderCapture()
		{
			yield return _waitForEndOfFrame;

			Capture();
		}

		private void Capture()
		{
			TickFrameTimer();

			AccumulateMotionBlur();

			if (_capturing && !_paused)
			{
				if (_settings.camera != null && _handle >= 0)
				{
					bool canGrab = true;

					if (IsUsingMotionBlur())
					{
						// If the motion blur is still accumulating, don't grab this frame
						canGrab = _motionBlur.IsFrameAccumulated;
					}

					if (canGrab && CanOutputFrame())
					{
						// Frame to encode either comes from rendering, or motion blur accumulation
						RenderTexture finalTexture = null;
						if (!IsUsingMotionBlur())
						{
							RenderFrame();
							finalTexture = _final;
						}
						else
						{
							finalTexture = _motionBlur.FinalTexture;
						}

						if (_targetNativePointer == System.IntPtr.Zero || _supportTextureRecreate)
						{
							// NOTE: If support for captures to survive through alt-tab events, or window resizes where the GPU resources are recreated
							// is required, then this line is needed.  It is very expensive though as it does a sync with the rendering thread.
							_targetNativePointer = finalTexture.GetNativeTexturePtr();
						}

						NativePlugin.SetTexturePointer(_handle, _targetNativePointer);

						RenderThreadEvent(NativePlugin.PluginEvent.CaptureFrameBuffer);
						GL.InvalidateState();

						UpdateFPS();
					}
				}
			}

			RenormTimer();
		}

		private void AccumulateMotionBlur()
		{
			if (_motionBlur != null)
			{
				if (_capturing && !_paused)
				{
					if (_settings.camera != null && _handle >= 0)
					{
						RenderFrame();
						_motionBlur.Accumulate(_final);
					}
				}
			}
		}

		private void RenderFrame()
		{
			_cameraGroup.position = _settings.camera.transform.position;
			Quaternion originalRot = _settings.camera.transform.rotation * Quaternion.AngleAxis(180f, Vector3.up);	// Apply 180 degree correction align the front camera vector to the center of the output image

			float rotationStep = 360f / _eyeWidth;
			int startPixelX = 0;
			int maxPixelX = _eyeWidth / _settings.pixelSliceSize;
			int endPixelX = maxPixelX;

			if (_settings.render180Degrees)
			{
				// Remove the first and last quarter of the rendering
				startPixelX = endPixelX / 4;
				endPixelX -= startPixelX;
				// Add a few pixels back in case of bilinear filtering issues
				startPixelX = Mathf.Max(0, startPixelX - 2);
				endPixelX = Mathf.Min(maxPixelX, endPixelX + 2);
			}

			_final.DiscardContents();
			for (int i = startPixelX; i < endPixelX; ++i)
			{
				int step = i * _settings.pixelSliceSize;
				float v = step * rotationStep;

				_cameraGroup.rotation = originalRot * Quaternion.AngleAxis(v, Vector3.up);

				_leftCameraTop.targetTexture.DiscardContents();
				_leftCameraBot.targetTexture.DiscardContents();
				_rightCameraTop.targetTexture.DiscardContents();
				_rightCameraBot.targetTexture.DiscardContents();

				_leftCameraTop.Render();
				_leftCameraBot.Render();
				_rightCameraTop.Render();
				_rightCameraBot.Render();

				_finalMaterial.SetFloat(_propSliceCenter, step + _settings.pixelSliceSize / 2f);

				Graphics.Blit(null, _final, _finalMaterial);
				//System.GC.Collect();
			}
		}

		public override Texture GetPreviewTexture()
		{
			if (IsUsingMotionBlur())
			{
				return _motionBlur.FinalTexture;
			}
			return _final;
		}

		public override bool PrepareCapture()
		{
			if (_capturing)
			{
				return false;
			}

			if (SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL") && !SystemInfo.graphicsDeviceVersion.Contains("emulated"))
			{
				Debug.LogError("[AVProMovieCapture] OpenGL not yet supported for CaptureFromCamera360ODS component, please use Direct3D11 instead. You may need to switch your build platform to Windows.");
				return false;
			}

#if AVPRO_MOVIECAPTURE_UNITYPROFILER_55
			// This component makes the profiler use a TON of memory, so warn the user to disable it
			if (UnityEngine.Profiling.Profiler.enabled)
			{
				Debug.LogWarning("[AVProMovieCapture] Having the Unity profiler enabled while using the CaptureFromCamera360ODS component is not recommended. Too many samples are generated which can make the system run out of memory. Disable the profiler, close the window and remove the tab. A Unity restart may be required after disabling the profiler recording");
			}
#endif

			// Setup material
			_pixelFormat = NativePlugin.PixelFormat.RGBA32;
			_isTopDown = true;
			if (_settings.camera == null)
			{
				_settings.camera = this.GetComponent<Camera>();
			}
			if (_settings.camera == null)
			{
				Debug.LogError("[AVProMovieCapture] No camera assigned to CaptureFromCamera360ODS");
				return false;
			}

			// Resolution
			int finalWidth = Mathf.FloorToInt(_settings.camera.pixelRect.width);
			int finalHeight = Mathf.FloorToInt(_settings.camera.pixelRect.height);
			if (_renderResolution == Resolution.Custom)
			{
				finalWidth = (int)_renderSize.x;
				finalHeight = (int)_renderSize.y;
			}
			else if (_renderResolution != Resolution.Original)
			{
				GetResolution(_renderResolution, ref finalWidth, ref finalHeight);
			}

			_eyeWidth = Mathf.Clamp(finalWidth, 1, 8192);
			// NOTE: Height must be even
			_eyeHeight = Mathf.Clamp(finalHeight / 2, 1, 4096);
			_eyeHeight -= _eyeHeight & 1;

			finalWidth = _eyeWidth;
			finalHeight = _eyeHeight * 2;

			int aaLevel = GetCameraAntiAliasingLevel(_settings.camera);

			// NOTE: Pixel slice size must be divisible by the total width
			_settings.pixelSliceSize = Mathf.Clamp(_settings.pixelSliceSize, 1, _eyeWidth);
			_settings.pixelSliceSize = _settings.pixelSliceSize - (_eyeWidth % _settings.pixelSliceSize);

			_settings.paddingSize = Mathf.Clamp(_settings.paddingSize, 0, 31);

			float offset = _settings.ipd / 2f;

			float aspect = (_settings.pixelSliceSize * 2f) / _eyeHeight;

			if (_cameraGroup == null)
			{
				GameObject go = new GameObject("OdsCameraGroup");
				go.transform.parent = this.gameObject.transform;
				// NOTE: We copy the hideFlags otherwise when instantiated by the Movie Capture window which has the DontSave flag
				// Unity throws an error about destroying transforms when coming out of play mode.
				go.hideFlags = this.gameObject.hideFlags;
				_cameraGroup = go.transform;
			}

			// TODO: only recreate textures if they don't already exist or size has changed
			_leftCameraTop = CreateEye(_leftCameraTop, "LeftEyeTop", 45f, -offset, _eyeHeight / 2, _settings.camera.cullingMask, 90f, aspect, aaLevel);
			_leftCameraBot = CreateEye(_leftCameraBot, "LeftEyeBot", -45f, -offset, _eyeHeight / 2, _settings.camera.cullingMask, 90f, aspect, aaLevel);
			_rightCameraTop = CreateEye(_rightCameraTop, "RightEyeTop", 45f, offset, _eyeHeight / 2, _settings.camera.cullingMask, 90f, aspect, aaLevel);
			_rightCameraBot = CreateEye(_rightCameraBot, "RightEyeBot", -45f, offset, _eyeHeight / 2, _settings.camera.cullingMask, 90f, aspect, aaLevel);

			// Create final texture (if not already created)
			_targetNativePointer = System.IntPtr.Zero;
			if (_final != null)
			{
				_final.DiscardContents();
				if (_final.width != finalWidth || _final.height != finalHeight || _final.antiAliasing != aaLevel)
				{
					RenderTexture.ReleaseTemporary(_final);
					_final = null;
				}
				_final = null;
			}
			if (_final == null)
			{
				_final = RenderTexture.GetTemporary(finalWidth, finalHeight, 0);
			}

			// Setup material
			_finalMaterial.SetTexture("_leftTopTex", _leftCameraTop.targetTexture);
			_finalMaterial.SetTexture("_leftBotTex", _leftCameraBot.targetTexture);
			_finalMaterial.SetTexture("_rightTopTex", _rightCameraTop.targetTexture);
			_finalMaterial.SetTexture("_rightBotTex", _rightCameraBot.targetTexture);
			_finalMaterial.SetFloat("_pixelSliceSize", _settings.pixelSliceSize);
			_finalMaterial.SetInt("_paddingSize", _settings.paddingSize);
			_finalMaterial.SetFloat("_targetXTexelSize", 1.0f / finalWidth);

			if (_settings.render180Degrees)
			{
				_finalMaterial.DisableKeyword("LAYOUT_EQUIRECT360");
				_finalMaterial.EnableKeyword("LAYOUT_EQUIRECT180");
			}
			else
			{
				_finalMaterial.DisableKeyword("LAYOUT_EQUIRECT180");
				_finalMaterial.EnableKeyword("LAYOUT_EQUIRECT360");
			}			

			// Setup capture
			SelectRecordingResolution(finalWidth, finalHeight);
			GenerateFilename();
			return base.PrepareCapture();
		}

		private static void DestroyEye(Camera camera)
		{			
			if (camera != null)
			{
				RenderTexture.ReleaseTemporary(camera.targetTexture);
				if (Application.isPlaying)
				{
					GameObject.Destroy(camera.gameObject);
				}
				#if UNITY_EDITOR
				else
				{
					GameObject.DestroyImmediate(camera.gameObject);
				}
				#endif	
			}
		}

		public override void OnDestroy()
		{
			_targetNativePointer = System.IntPtr.Zero;
			if (_final != null)
			{
				RenderTexture.ReleaseTemporary(_final);
				_final = null;
			}

			DestroyEye(_leftCameraTop);
			DestroyEye(_leftCameraBot);
			DestroyEye(_rightCameraTop);
			DestroyEye(_rightCameraBot);
			_leftCameraTop = null;
			_leftCameraBot = null;
			_rightCameraTop = null;
			_rightCameraBot = null;

			if (_cameraGroup != null)
			{
				if (Application.isPlaying)
				{
					GameObject.Destroy(_cameraGroup.gameObject);
				}
				#if UNITY_EDITOR
				else
				{
					GameObject.DestroyImmediate(_cameraGroup.gameObject);
				}
				#endif	
				_cameraGroup = null;
			}

			if (_finalMaterial)
			{
				Destroy(_finalMaterial);
				_finalMaterial = null;
			}

			base.OnDestroy();
		}
	}
}