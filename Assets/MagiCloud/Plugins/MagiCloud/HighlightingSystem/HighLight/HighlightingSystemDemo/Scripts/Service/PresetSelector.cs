using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using HighlightingSystem;

[DisallowMultipleComponent]
public class PresetSelector : MonoBehaviour
{
	public HighlightingBase highlightingBase;

	public Dropdown dropdown;

	protected struct Preset
	{
		public string name;
		public int downsampleFactor;
		public int iterations;
		public float blurMinSpread;
		public float blurSpread;
		public float blurIntensity;
	}

	List<Preset> presets = new List<Preset>()
	{
		new Preset() { name = "Default",	downsampleFactor = 4,	iterations = 2,	blurMinSpread = 0.65f,	blurSpread = 0.25f, blurIntensity = 0.3f }, 
		new Preset() { name = "Strong",	downsampleFactor = 4,	iterations = 2,	blurMinSpread = 0.5f,	blurSpread = 0.15f,	blurIntensity = 0.325f }, 
		new Preset() { name = "Wide",		downsampleFactor = 4,	iterations = 4,	blurMinSpread = 0.5f,	blurSpread = 0.15f,	blurIntensity = 0.325f }, 
		new Preset() { name = "Speed",	downsampleFactor = 4,	iterations = 1,	blurMinSpread = 0.75f,	blurSpread = 0f,	blurIntensity = 0.35f }, 
		new Preset() { name = "Quality",	downsampleFactor = 2,	iterations = 3,	blurMinSpread = 0.5f,	blurSpread = 0.5f,	blurIntensity = 0.28f }, 
		new Preset() { name = "Solid 1px",	downsampleFactor = 1,	iterations = 1,	blurMinSpread = 1f,		blurSpread = 0f,	blurIntensity = 1f }, 
		new Preset() { name = "Solid 2px",	downsampleFactor = 1,	iterations = 2,	blurMinSpread = 1f,		blurSpread = 0f,	blurIntensity = 1f }
	};

	// 
	void Awake()
	{
		int index = -1;

		FindHighlightingBase(ref highlightingBase);

		List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
		for (int i = 0, l = presets.Count; i < l; i++)
		{
			Preset preset = presets[i];
			Dropdown.OptionData option = new Dropdown.OptionData(preset.name);
			options.Add(option);

			if (index == -1 && 
				highlightingBase != null && 
				highlightingBase.downsampleFactor == preset.downsampleFactor && 
				highlightingBase.iterations == preset.iterations && 
				highlightingBase.blurMinSpread == preset.blurMinSpread && 
				highlightingBase.blurSpread == preset.blurSpread && 
				highlightingBase.blurIntensity == preset.blurIntensity)
			{
				index = i;
			}
		}
		dropdown.options = options;
		if (index != -1)
		{
			dropdown.value = index;
		}
	}

	// 
	void OnEnable()
	{
		dropdown.onValueChanged.AddListener(OnValueChanged);
	}

	// 
	void OnDisable()
	{
		dropdown.onValueChanged.RemoveListener(OnValueChanged);
	}

	// 
	void OnValueChanged(int index)
	{
		if (index < 0 || index >= presets.Count) { return; }

		SetPresetSettings(presets[index]);
	}

	// 
	void SetPresetSettings(Preset p)
	{
		FindHighlightingBase(ref highlightingBase);

		if (highlightingBase == null) { return; }

		highlightingBase.downsampleFactor = p.downsampleFactor;
		highlightingBase.iterations = p.iterations;
		highlightingBase.blurMinSpread = p.blurMinSpread;
		highlightingBase.blurSpread = p.blurSpread;
		highlightingBase.blurIntensity = p.blurIntensity;
	}

	// 
	static private void FindHighlightingBase(ref HighlightingBase highlightingBase)
	{
		if (highlightingBase != null) { return; }

		Camera camera = Camera.main;
		if (camera != null)
		{
			highlightingBase = camera.GetComponent<HighlightingBase>();
			if (highlightingBase != null) { return; }
		}

		Camera[] allCameras = Camera.allCameras;
		for (int i = 0, l = allCameras.Length; i < l; i++)
		{
			camera = allCameras[i];
			highlightingBase = camera.GetComponent<HighlightingBase>();
			if (highlightingBase != null) { return; }
		}
	}
}
