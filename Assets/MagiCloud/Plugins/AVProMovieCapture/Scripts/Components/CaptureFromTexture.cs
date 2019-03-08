#if UNITY_5_4_OR_NEWER
	#define AVPRO_MOVIECAPTURE_RENDERTEXTUREBGRA32_54
#endif
using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture
{
	/// <summary>
	/// Capture from a Texture object (including RenderTexture, WebcamTexture)
	/// </summary>
	[AddComponentMenu("AVPro Movie Capture/Capture From Texture", 3)]
	public class CaptureFromTexture : CaptureBase
	{
		[Tooltip("If enabled the method the encoder will only process frames each time UpdateSourceTexture() is called. This is useful if the texture is updating at a different rate compared to Unity, eg for webcam capture.")]
		[SerializeField]
		private bool _manualUpdate;

		private Texture _sourceTexture;
		private RenderTexture _renderTexture;
		private System.IntPtr _targetNativePointer = System.IntPtr.Zero;

		public bool _isSourceTextureChanged = false;

		public void SetSourceTexture(Texture texture)
		{
			_sourceTexture = texture;

			if (texture is Texture2D)
			{
				if ((((Texture2D)texture).format != TextureFormat.ARGB32) &&
					(((Texture2D)texture).format != TextureFormat.RGBA32) &&
					(((Texture2D)texture).format != TextureFormat.BGRA32))
				{
					Debug.LogWarning("[AVProMovieCapture] texture format may not be supported: " + ((Texture2D)texture).format);
				}
			}
			else if (texture is RenderTexture)
			{
				if ((((RenderTexture)texture).format != RenderTextureFormat.ARGB32) &&
					(((RenderTexture)texture).format != RenderTextureFormat.Default)
#if AVPRO_MOVIECAPTURE_RENDERTEXTUREBGRA32_54
				&& (((RenderTexture)texture).format != RenderTextureFormat.BGRA32)
#endif
				)
				{
					Debug.LogWarning("[AVProMovieCapture] texture format may not be supported: " + ((RenderTexture)texture).format);
				}
			}
		}

		public void UpdateSourceTexture()
		{
			_isSourceTextureChanged = true;
		}

		private bool ShouldCaptureFrame()
		{
			return (_capturing && !_paused && _sourceTexture != null);
		}

		private bool HasSourceTextureChanged()
		{
			return (!_manualUpdate || (_manualUpdate && _isSourceTextureChanged));
		}

		public override void UpdateFrame()
		{
			if (_useWaitForEndOfFrame)
			{
				StartCoroutine(FinalRenderCapture());
				base.UpdateFrame();
			}
			else
			{
				Capture();
				base.UpdateFrame();
			}
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

			if (ShouldCaptureFrame())
			{
				bool hasSourceTextureChanged = HasSourceTextureChanged();

				// If motion blur is enabled, wait until all frames are accumulated
				if (IsUsingMotionBlur())
				{
					// If the motion blur is still accumulating, don't grab this frame
					hasSourceTextureChanged = _motionBlur.IsFrameAccumulated;
				}

				_isSourceTextureChanged = false;
				if (hasSourceTextureChanged)
				{
					if ((_manualUpdate /*&& NativePlugin.IsNewFrameDue(_handle)*/) || CanOutputFrame())
					{
						// If motion blur is enabled, use the motion blur result
						Texture sourceTexture = _sourceTexture;
						if (IsUsingMotionBlur())
						{
							sourceTexture = _motionBlur.FinalTexture;
						}

						// If the texture isn't a RenderTexture then blit it to the Rendertexture so the native plugin can grab it
						if (!(sourceTexture is RenderTexture))
						{
							_renderTexture.DiscardContents();
							Graphics.Blit(sourceTexture, _renderTexture);
							sourceTexture = _renderTexture;
						}

						if (_targetNativePointer == System.IntPtr.Zero || _supportTextureRecreate)
						{
							// NOTE: If support for captures to survive through alt-tab events, or window resizes where the GPU resources are recreated
							// is required, then this line is needed.  It is very expensive though as it does a sync with the rendering thread.
							_targetNativePointer = sourceTexture.GetNativeTexturePtr();
						}

						NativePlugin.SetTexturePointer(_handle, _targetNativePointer);

						RenderThreadEvent(NativePlugin.PluginEvent.CaptureFrameBuffer);

						if (!IsUsingMotionBlur())
						{
							_isSourceTextureChanged = false;
						}

						// Handle audio from Unity
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

			RenormTimer();
		}

		private void AccumulateMotionBlur()
		{
			if (_motionBlur != null)
			{
				if (ShouldCaptureFrame() && HasSourceTextureChanged())
				{
					_motionBlur.Accumulate(_sourceTexture);
					_isSourceTextureChanged = false;
				}
			}
		}

		public override Texture GetPreviewTexture()
		{
			if (IsUsingMotionBlur())
			{
				return _motionBlur.FinalTexture;
			}
			if (_sourceTexture is RenderTexture)
			{
				return _sourceTexture;
			}
			return _renderTexture;
		}

		public override bool PrepareCapture()
		{
			if (_capturing)
			{
				return false;
			}

			if (_sourceTexture == null)
			{
				Debug.LogError("[AVProMovieCapture] No texture set to capture");
				return false;
			}

			if (SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL") && !SystemInfo.graphicsDeviceVersion.Contains("emulated"))
			{
				Debug.LogError("[AVProMovieCapture] OpenGL not yet supported for CaptureFromTexture component, please use Direct3D11 instead. You may need to switch your build platform to Windows.");
				return false;
			}

			_pixelFormat = NativePlugin.PixelFormat.RGBA32;
			_isSourceTextureChanged = false;

			SelectRecordingResolution(_sourceTexture.width, _sourceTexture.height);

			_renderTexture = RenderTexture.GetTemporary(_targetWidth, _targetHeight, 0, RenderTextureFormat.ARGB32);
			_renderTexture.Create();

			GenerateFilename();

			return base.PrepareCapture();
		}

		public override void UnprepareCapture()
		{
			_targetNativePointer = System.IntPtr.Zero;
			NativePlugin.SetTexturePointer(_handle, System.IntPtr.Zero);

			if (_renderTexture != null)
			{
				RenderTexture.ReleaseTemporary(_renderTexture);
				_renderTexture = null;
			}

			base.UnprepareCapture();
		}
	}
}