#if UNITY_5_4_OR_NEWER || UNITY_5
	#define AVPRO_MOVIECAPTURE_DEFERREDSHADING
#endif
using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture
{
	/// <summary>
	/// Capture from a specific Unity camera.  This component re-renders the camera manually, so it does add extra draw calls.
	/// </summary>
	//[RequireComponent(typeof(Camera))]
	[AddComponentMenu("AVPro Movie Capture/Capture From Camera", 1)]
	public class CaptureFromCamera : CaptureBase
	{
		[SerializeField]
		private Camera _lastCamera;

		[SerializeField]
		private Camera[] _contribCameras;

		[SerializeField]
		private bool _useContributingCameras = true;

		private RenderTexture _target;
		private System.IntPtr _targetNativePointer = System.IntPtr.Zero;

		public bool UseContributingCameras
		{
			get { return _useContributingCameras; }
			set { _useContributingCameras = value; }
		}

		public void SetCamera(Camera mainCamera, bool useContributingCameras = true)
		{
			_lastCamera = mainCamera;
			_contribCameras = null;
			_useContributingCameras = useContributingCameras;

			if (_useContributingCameras && _lastCamera != null)
			{
				if (Utils.HasContributingCameras(_lastCamera))
				{
					_contribCameras = Utils.FindContributingCameras(mainCamera);
				}
			}
		}

		public void SetCamera(Camera mainCamera, Camera[] contributingCameras)
		{
			_lastCamera = mainCamera;
			_contribCameras = contributingCameras;
		}

		private bool HasCamera()
		{
			return (_lastCamera != null);
		}

		private bool HasContributingCameras()
		{
			return (_useContributingCameras && _contribCameras != null && _contribCameras.Length > 0);
		}

		public override void UpdateFrame()
		{
			if (_useWaitForEndOfFrame)
			{
				if (_capturing && !_paused && HasCamera())
				{
					StartCoroutine(FinalRenderCapture());
				}

				base.UpdateFrame();
			}
			else
			{
				base.UpdateFrame();
				Capture();
			}
		}

		private IEnumerator FinalRenderCapture()
		{
			yield return _waitForEndOfFrame;

			Capture();
		}

		// If we're forcing a resolution or AA change then we have to render the camera again to the new target
		// If we try to just set the targetTexture of the camera and grab it in OnRenderImage we can't render it to the screen as before :(
		private void Capture()
		{
			TickFrameTimer();

			if (_capturing && !_paused && HasCamera())
			{
				bool canGrab = true;

				if (IsUsingMotionBlur())
				{
					// If the motion blur is still accumulating, don't grab this frame
					canGrab = _motionBlur.IsFrameAccumulated;
				}

				if (canGrab)
				{
					/*while (_handle >= 0 && !AVProMovieCapturePlugin.IsNewFrameDue(_handle))
					{
						System.Threading.Thread.Sleep(1);
					}*/
					if (_handle >= 0 && CanOutputFrame())
					{
						// Render the camera(s)
						if (!IsUsingMotionBlur())
						{
							UpdateTexture();
						}
						else
						{
							// Just grab the last result of the motion blur
							_target.DiscardContents();
							Graphics.Blit(_motionBlur.FinalTexture, _target);
						}

						if (_supportTextureRecreate)
						{
							// NOTE: If support for captures to survive through alt-tab events, or window resizes where the GPU resources are recreated
							// is required, then this line is needed.  It is very expensive though as it does a sync with the rendering thread.
							_targetNativePointer = _target.GetNativeTexturePtr();
						}

						NativePlugin.SetTexturePointer(_handle, _targetNativePointer);

						RenderThreadEvent(NativePlugin.PluginEvent.CaptureFrameBuffer);

						if (IsRecordingUnityAudio())
						{
							int audioDataLength = 0;
							System.IntPtr audioDataPtr = _audioCapture.ReadData(out audioDataLength);
							if (audioDataLength > 0)
							{
								NativePlugin.EncodeAudio(_handle, audioDataPtr, (uint)audioDataLength);
							}
						}

						UpdateFPS();
					}
				}
			}
			base.UpdateFrame();

			RenormTimer();
		}

	private void UpdateTexture()
	{
		// Render a single camera
		if (!HasContributingCameras())
		{
			RenderTexture prev = _lastCamera.targetTexture;
			// Reset the viewport rect as we're rendering to a texture captures the full viewport
			Rect prevRect = _lastCamera.rect;
			CameraClearFlags prevClear = _lastCamera.clearFlags;
			Color prevColor = _lastCamera.backgroundColor;
			bool clearChanged = false;
			if (_lastCamera.clearFlags == CameraClearFlags.Nothing || _lastCamera.clearFlags == CameraClearFlags.Depth)
			{
				clearChanged = true;
				_lastCamera.clearFlags = CameraClearFlags.SolidColor;
				if (!_supportAlpha)
				{
					_lastCamera.backgroundColor = Color.black;
				}
				else
				{
					_lastCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
				}
			}

			// Render
			_lastCamera.rect = new Rect(0f, 0f, 1f, 1f);
			_lastCamera.targetTexture = _target;
			_lastCamera.Render();

			// Restore camera
			{
				_lastCamera.rect = prevRect;
				if (clearChanged)
				{
					_lastCamera.clearFlags = prevClear;
					_lastCamera.backgroundColor = prevColor;
				}
				_lastCamera.targetTexture = prev;
			}
		}
		// Render the camera chain
		else
		{
			// First render contributing cameras
			for (int cameraIndex = 0; cameraIndex < _contribCameras.Length; cameraIndex++)
			{
				Camera camera = _contribCameras[cameraIndex];
				if (camera != null && camera.isActiveAndEnabled)
				{
					RenderTexture prev = camera.targetTexture;
					camera.targetTexture = _target;
					camera.Render();
					camera.targetTexture = prev;
				}
			}
			// Finally render the last camera
			if (_lastCamera != null)
			{
				RenderTexture prev = _lastCamera.targetTexture;
				_lastCamera.targetTexture = _target;
				_lastCamera.Render();
				_lastCamera.targetTexture = prev;
			}
		}
	}

#if false
	// NOTE: This is old code based on OnRenderImage...may be revived at some point
	private void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		if (_capturing && !_paused)
		{
#if true
			while (_handle >= 0 && !NativePlugin.IsNewFrameDue(_handle))
			{
				System.Threading.Thread.Sleep(1);
			}
			if (_handle >= 0)
			{
                if (_audioCapture && _audioDeviceIndex < 0 && !_noAudio && _isRealTime)
                {
					int audioDataLength = 0;
					System.IntPtr audioDataPtr = _audioCapture.ReadData(out audioDataLength);
					if (audioDataLength > 0)
					{
						NativePlugin.EncodeAudio(_handle, audioDataPtr, (uint)audioDataLength);
					}
                }

                // In Direct3D the RT can be flipped vertically
                /*if (source.texelSize.y < 0)
                {

                }*/

				Graphics.Blit(source, dest);

				_lastSource = source;
				_lastDest = dest;

				if (dest != _originalTarget)
				{
					Graphics.Blit(dest, _originalTarget);
				}

#if AVPRO_MOVIECAPTURE_GLISSUEEVENT_52
				GL.IssuePluginEvent(NativePlugin.GetRenderEventFunc(), NativePlugin.PluginID | (int)NativePlugin.PluginEvent.CaptureFrameBuffer | _handle);
#else
				GL.IssuePluginEvent(NativePlugin.PluginID | (int)NativePlugin.PluginEvent.CaptureFrameBuffer | _handle);
#endif
				GL.InvalidateState();
				
				UpdateFPS();

				return;
			}
#endif
		}

		// Pass-through
		Graphics.Blit(source, dest);

		_lastSource = source;
		_lastDest = dest;
	}
#endif

		public override void UnprepareCapture()
		{
			NativePlugin.SetTexturePointer(_handle, System.IntPtr.Zero);

			if (_target != null)
			{
				_target.DiscardContents();
			}

			base.UnprepareCapture();
		}

		public override Texture GetPreviewTexture()
		{
			return _target;
		}

		public override bool PrepareCapture()
		{
			if (_capturing)
			{
				return false;
			}

			if (SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL") && !SystemInfo.graphicsDeviceVersion.Contains("emulated"))
			{
				Debug.LogError("[AVProMovieCapture] OpenGL not yet supported for CaptureFromCamera component, please use Direct3D11 instead. You may need to switch your build platform to Windows.");
				return false;
			}

			// Setup material
			_pixelFormat = NativePlugin.PixelFormat.RGBA32;
			_isTopDown = true;

			if (!HasCamera())
			{
				_lastCamera = this.GetComponent<Camera>();
				if (!HasCamera())
				{
					_lastCamera = Camera.main;
				}
				if (!HasCamera())
				{
					Debug.LogError("[AVProMovieCapture] No camera assigned to CaptureFromCamera");
					return false;
				}
			}

			if (!HasContributingCameras() && (_lastCamera.clearFlags == CameraClearFlags.Depth || _lastCamera.clearFlags == CameraClearFlags.Nothing))
			{
				Debug.LogWarning("[AVProMovieCapture] This camera doesn't clear, consider setting contributing cameras");
			}

			int width = Mathf.FloorToInt(_lastCamera.pixelRect.width);
			int height = Mathf.FloorToInt(_lastCamera.pixelRect.height);

			// Setup rendering a different render target if we're overriding resolution or anti-aliasing
			//if (_renderResolution != Resolution.Original || (_renderAntiAliasing > 0 && _renderAntiAliasing != QualitySettings.antiAliasing))
			{
				if (_renderResolution == Resolution.Custom)
				{
					width = (int)_renderSize.x;
					height = (int)_renderSize.y;
				}
				else if (_renderResolution != Resolution.Original)
				{
					GetResolution(_renderResolution, ref width, ref height);
				}

				int aaLevel = GetCameraAntiAliasingLevel(_lastCamera);

				// Create the render target
				if (_target != null)
				{
					_target.DiscardContents();
					if (_target.width != width || _target.height != height || _target.antiAliasing != aaLevel)
					{
						_targetNativePointer = System.IntPtr.Zero;
						RenderTexture.ReleaseTemporary(_target);
						_target = null;
					}
				}
				if (_target == null)
				{
					_target = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default, aaLevel);
					_target.name = "[AVProMovieCapture] Camera Target";

					_target.Create();
					_targetNativePointer = _target.GetNativeTexturePtr();
				}

				//camera.targetTexture = _target;

				// Adjust size for camera rectangle
				/*if (camera.rect.width < 1f || camera.rect.height < 1f)
				{
					float rectWidth = Mathf.Clamp01(camera.rect.width + camera.rect.x) - Mathf.Clamp01(camera.rect.x);
					float rectHeight = Mathf.Clamp01(camera.rect.height + camera.rect.y) - Mathf.Clamp01(camera.rect.y);
					width = Mathf.FloorToInt(width * rectWidth);
					height = Mathf.FloorToInt(height * rectHeight);
				}*/

				if (_useMotionBlur)
				{
					_motionBlurCameras = new Camera[1];
					_motionBlurCameras[0] = _lastCamera;
				}
			}

			SelectRecordingResolution(width, height);

			GenerateFilename();

			return base.PrepareCapture();
		}

		public override void OnDestroy()
		{
			if (_target != null)
			{
				_targetNativePointer = System.IntPtr.Zero;
				RenderTexture.ReleaseTemporary(_target);
				_target = null;
			}

			base.OnDestroy();
		}
	}
}