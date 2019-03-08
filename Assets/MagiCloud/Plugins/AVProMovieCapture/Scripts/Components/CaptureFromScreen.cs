#if UNITY_5_6_OR_NEWER
	#define AVPRO_UNITY_CLASS_DISPLAY
#endif

using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture
{
	/// <summary>
	/// Capture from the screen (backbuffer).  Everything is captured as it appears on the screen, including IMGUI rendering.
	/// This component waits for the frame to be completely rendered and then captures it.
	/// </summary>
	[AddComponentMenu("AVPro Movie Capture/Capture From Screen", 0)]
	public class CaptureFromScreen : CaptureBase
	{
		//private const int NewFrameSleepTimeMs = 6;

		public override bool PrepareCapture()
		{
			if (_capturing)
			{
				return false;
			}

			if (SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL") && !SystemInfo.graphicsDeviceVersion.Contains("emulated"))
			{
				Debug.LogError("[AVProMovieCapture] OpenGL not yet supported for CaptureFromScreen component, please use Direct3D11 instead. You may need to switch your build platform to Windows.");
				return false;
			}

#if AVPRO_UNITY_CLASS_DISPLAY && UNITY_EDITOR
			if (Display.displays.Length > 1)
			{
				bool isSecondDisplayActive = false;
				for (int i = 1; i < Display.displays.Length; i++)
				{
					if (Display.displays[i].active)
					{
						isSecondDisplayActive = true;
						break;
					}
				}
				if (isSecondDisplayActive)
				{
					Debug.LogError("[AVProMovieCapture] CaptureFromScreen doesn't work correctly (can cause stretching or incorrect display capture) when there are multiple displays are active.  Use CaptureFromCamera instead.");
				}				
			}
#endif

			SelectRecordingResolution(Screen.width, Screen.height);

			_pixelFormat = NativePlugin.PixelFormat.RGBA32;
			if (SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL") && !SystemInfo.graphicsDeviceVersion.Contains("emulated"))
			{
				// TODO: add this back in once we have fixed opengl support
				_pixelFormat = NativePlugin.PixelFormat.BGRA32;
				_isTopDown = true;
			}
			else
			{
				_isTopDown = false;

				if (_isDirectX11)
				{
					_isTopDown = false;
				}
			}

			GenerateFilename();

			return base.PrepareCapture();
		}

		private IEnumerator FinalRenderCapture()
		{
			yield return _waitForEndOfFrame;

			TickFrameTimer();

			bool canGrab = true;

			if (IsUsingMotionBlur())
			{
				// If the motion blur is still accumulating, don't grab this frame
				canGrab = _motionBlur.IsFrameAccumulated;
			}

			if (canGrab && CanOutputFrame())
			{
				// Grab final RenderTexture into texture and encode
				if (IsRecordingUnityAudio())
				{
					int audioDataLength = 0;
					System.IntPtr audioDataPtr = _audioCapture.ReadData(out audioDataLength);
					if (audioDataLength > 0)
					{
						NativePlugin.EncodeAudio(_handle, audioDataPtr, (uint)audioDataLength);
					}
				}

				RenderThreadEvent(NativePlugin.PluginEvent.CaptureFrameBuffer);
				GL.InvalidateState();

				UpdateFPS();
			}

			RenormTimer();

			//yield return null;
		}

		public override void UpdateFrame()
		{
			if (_capturing && !_paused)
			{
				StartCoroutine(FinalRenderCapture());
			}
			base.UpdateFrame();
		}
	}
}