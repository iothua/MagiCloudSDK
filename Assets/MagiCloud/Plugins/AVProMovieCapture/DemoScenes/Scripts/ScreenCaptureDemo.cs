using UnityEngine;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture.Demos
{
	public class ScreenCaptureDemo : MonoBehaviour
	{
		[SerializeField]
		private AudioClip _audioBG;

		[SerializeField]
		private AudioClip _audioHit;

		[SerializeField]
		private float _speed = 1.0f;

		[SerializeField]
		private CaptureBase _capture;

		[SerializeField]
		private GUISkin _guiSkin;

		[SerializeField]
		private bool _spinCamera = true;

		// State
		private float _timer;

		private void Start()
		{
			// Play music track
			if (_audioBG != null)
			{
				AudioSource.PlayClipAtPoint(_audioBG, Vector3.zero);
			}
		}

		private void Update()
		{
			// Press the S key to trigger audio and background color change - useful for testing A/V sync
			if (Input.GetKeyDown(KeyCode.S))
			{
				if (_audioHit != null && _capture.IsCapturing())
				{
					AudioSource.PlayClipAtPoint(_audioHit, Vector3.zero);
					Camera.main.backgroundColor = new Color(Random.value, Random.value, Random.value, 0);
				}
			}

			// ESC to stop capture and quit
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (_capture != null && _capture.IsCapturing())
				{
					_capture.StopCapture();
				}
				else
				{
					Application.Quit();
				}
			}

			// Spin the camera around
			if (_spinCamera && Camera.main != null)
			{
				Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, 20f * Time.deltaTime * _speed);
			}
		}

		private void OnGUI()
		{
			GUI.skin = _guiSkin;
			Rect r = new Rect(Screen.width - 108, 64, 128, 28);
			GUI.Label(r, "Frame " + Time.frameCount);
		}
	}
}