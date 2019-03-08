using UnityEngine;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture
{
	/// <summary>
	/// Accumulates frames to average for generating motion blur in renders
	/// </summary>
	[AddComponentMenu("AVPro Movie Capture/Motion Blur", 301)]
	public class MotionBlur : MonoBehaviour
	{
		[SerializeField]
		public RenderTextureFormat _format = RenderTextureFormat.ARGBFloat;

		[SerializeField]
		private int _numSamples = 16;

		// State
		private RenderTexture _accum;
		private RenderTexture _lastComp;
		private Material _addMaterial;
		private Material _divMaterial;
		private int _frameCount;
		private int _targetWidth;
		private int _targetHeight;
		private bool _isDirty;

		private static int _propNumSamples;
		private static int _propWeight;

		public bool IsFrameAccumulated
		{
			get;
			private set;
		}

		public int NumSamples
		{
			get { return _numSamples; }
			set { _numSamples = value; OnNumSamplesChanged(); }
		}

		public int FrameCount
		{
			get { return _frameCount; }
		}

		public RenderTexture FinalTexture
		{
			get { return _lastComp; }
		}

		private void Awake()
		{
			if (_propNumSamples == 0)
			{
				_propNumSamples = Shader.PropertyToID("_NumSamples");
				_propWeight = Shader.PropertyToID("_Weight");
			}
		}

		public void SetTargetSize(int width, int height)
		{
			if (_targetWidth != width || _targetHeight != height)
			{
				_targetWidth = width;
				_targetHeight = height;
				_isDirty = true;
			}
		}

		private void Start()
		{
			Setup();
		}

		private void OnEnable()
		{
			_frameCount = 0;
			IsFrameAccumulated = false;
			ClearAccumulation();
		}

		private void Setup()
		{
			if (_addMaterial == null)
			{
				Shader addShader = Resources.Load<Shader>("AVProMovieCapture_MotionBlur_Add");
				Shader divShader = Resources.Load<Shader>("AVProMovieCapture_MotionBlur_Div");
				_addMaterial = new Material(addShader);
				_divMaterial = new Material(divShader);
			}

			if (_targetWidth == 0 && _targetHeight == 0)
			{
				// TODO: change the size of these RenderTextures based on the output size of the capture
				_targetWidth = Screen.width;
				_targetHeight = Screen.height;
			}

			if (_accum != null)
			{
				_accum.Release();
				RenderTexture.Destroy(_accum);
				_accum = null;
			}
			if (_lastComp != null)
			{
				_lastComp.Release();
				RenderTexture.Destroy(_lastComp);
				_lastComp = null;
			}

			_accum = new RenderTexture(_targetWidth, _targetHeight, 0, _format, RenderTextureReadWrite.Default);
			_lastComp = new RenderTexture(_targetWidth, _targetHeight, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
			_accum.filterMode = FilterMode.Point;
			_lastComp.filterMode = FilterMode.Bilinear;
			_accum.Create();
			_lastComp.Create();

			OnNumSamplesChanged();

			_isDirty = false;
		}

		private void ClearAccumulation()
		{
			RenderTexture prev = RenderTexture.active;
			RenderTexture.active = _accum;
			GL.Clear(false, true, Color.black);
			RenderTexture.active = prev;
		}

		private void OnDestroy()
		{
			if (_addMaterial != null)
			{
				Material.Destroy(_addMaterial);
				_addMaterial = null;
			}
			if (_divMaterial != null)
			{
				Material.Destroy(_divMaterial);
				_divMaterial = null;
			}

			if (_accum != null)
			{
				_accum.Release();
				RenderTexture.Destroy(_accum);
				_accum = null;
			}
			if (_lastComp != null)
			{
				_lastComp.Release();
				RenderTexture.Destroy(_lastComp);
				_lastComp = null;
			}
		}

		public void OnNumSamplesChanged()
		{
			if (_divMaterial != null)
			{
				_addMaterial.SetFloat(_propWeight, 1f);
				_divMaterial.SetFloat(_propNumSamples, _numSamples);
			}
		}

		private static float LerpUnclamped(float a, float b, float t)
		{
			return a + ((b - a) * t);
		}

		[SerializeField]
		public float _bias = 1f;

		private float _total = 0f;

		private void ApplyWeighting()
		{
			// Apply some frame weighting so the newer frames have the most contribution
			// Not sure this is better than non-weighted averaging.
			float weight = ((float)_frameCount / (float)_numSamples);
			weight = Mathf.Pow(weight, 2f);
			_total += weight;
			float numSamples = ((float)_numSamples / 2f) + 0.5f;
			numSamples = _total;
			weight = LerpUnclamped(weight, 1f, _bias);
			numSamples = LerpUnclamped(numSamples, _numSamples, _bias);
			_addMaterial.SetFloat(_propWeight, weight);
			_divMaterial.SetFloat(_propNumSamples, numSamples);
		}

		public void Accumulate(Texture src)
		{
			if (_isDirty)
			{
				Setup();
			}
			//ApplyWeighting();
			//_divMaterial.SetFloat(_propNumSamples, _numSamples);

			Graphics.Blit(src, _accum, _addMaterial);
			_frameCount++;

			if (_frameCount >= _numSamples)
			{
				// Divide the accumation texture
				Graphics.Blit(_accum, _lastComp, _divMaterial);

				// Clear the accumation texture so it is ready to start again
				ClearAccumulation();

				//Graphics.Blit(src, _accum, _addMaterial);
				//Graphics.Blit(_lastComp, _accum, _divMaterial);
				IsFrameAccumulated = true;
				_frameCount = 0;
				_total = 0f;
			}
			else
			{
				IsFrameAccumulated = false;
			}
		}

		private void OnRenderImage(RenderTexture src, RenderTexture dst)
		{
			// Take the result of the camera render and accumulate it
			// NOTE: Some capture components disable the this MotionBlur component so that OnRenderImage() isn't called as the component is used manually
			Accumulate(src);
			Graphics.Blit(_lastComp, dst);
		}

		/*void OnGUI()
		{
			GUILayout.Label("Real (slow) Motion Blur Demo");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Samples");
			int numSamples = (int)GUILayout.HorizontalSlider(_numSamples, 1, 64, GUILayout.Width(128f));
			if (numSamples != _numSamples)
			{
				_numSamples = numSamples;
				OnNumSamplesChanged();
			}
			GUILayout.Label(_numSamples.ToString());
			GUILayout.EndHorizontal();
		}*/
	}
}