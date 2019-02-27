using UnityEngine;
using UnityEngine.VR;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class VRSettingsUI : MonoBehaviour
{
	#region Inspector Fields
	public Toggle toggle;
	public Dropdown dropdown;
	#endregion

	#region Private Fields
	//private Dictionary<int, VRDeviceType> indexToVRDeviceType = new Dictionary<int, VRDeviceType>();
	//private Dictionary<VRDeviceType, int> vrDeviceTypeToIndex = new Dictionary<VRDeviceType, int>();
	#endregion

	#region MonoBehaviour
	// 
	void Awake()
	{
		//if (dropdown != null)
		//{
		//	Array vrDeviceTypes = Enum.GetValues(typeof(VRDeviceType));

		//	List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
		//	for (int i = 0; i < vrDeviceTypes.Length; i++)
		//	{
		//		VRDeviceType vrDeviceType = (VRDeviceType)vrDeviceTypes.GetValue(i);
		//		Dropdown.OptionData optionData = new Dropdown.OptionData(vrDeviceType.ToString());
		//		options.Add(optionData);
		//		indexToVRDeviceType.Add(i, vrDeviceType);
		//		vrDeviceTypeToIndex.Add(vrDeviceType, i);
		//	}

		//	dropdown.options = options;
		//	dropdown.value = vrDeviceTypeToIndex[UnityEngine.XR.XRSettings.loadedDevice];
		//}
	}

	// 
	void OnEnable()
	{
		if (dropdown != null) { dropdown.onValueChanged.AddListener(OnDropdownValueChanged); }
		if (toggle != null) { toggle.onValueChanged.AddListener(OnToggleValueChanged); }
	}

	// 
	void OnDisable()
	{
		if (dropdown != null) { dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged); }
		if (toggle != null) { toggle.onValueChanged.RemoveListener(OnToggleValueChanged); }
	}

	// 
	void Update()
	{
		//if (dropdown != null)
		//{
		//	if (VRHelper.isVRScene) { dropdown.value = vrDeviceTypeToIndex[UnityEngine.XR.XRSettings.loadedDevice]; }
		//	else if (dropdown.gameObject.activeSelf) { dropdown.gameObject.SetActive(false); }
		//}

		//if (toggle != null)
		//{
		//	if (VRHelper.isVRScene) { toggle.isOn = (UnityEngine.XR.XRSettings.loadedDevice != VRDeviceType.None); }
		//	else if (toggle.gameObject.activeSelf) { toggle.gameObject.SetActive(false); }
		//}
	}
	#endregion

	#region Private Methods
	// 
	void OnDropdownValueChanged(int index)
	{
		//VRHelper.SetVRDeviceType(indexToVRDeviceType[index]);
	}

	// 
	void OnToggleValueChanged(bool value)
	{
		//VRHelper.SetVRDeviceType(value ? VRDeviceType.Split : VRDeviceType.None);
	}
	#endregion
}