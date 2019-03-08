#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(CaptureFromCamera360))]
	public class CaptureFromCamera360Editor : CaptureBaseEditor
	{
		//private CaptureFromCamera360 _capture;

		private SerializedProperty _propCamera;

		private SerializedProperty _propRenderResolution;
		private SerializedProperty _propRenderSize;
		private SerializedProperty _propAntiAliasing;

		private SerializedProperty _propCubemapResolution;
		private SerializedProperty _propCubemapDepth;
		private SerializedProperty _propSupportGUI;
		private SerializedProperty _propSupporCameraRotation;
		private SerializedProperty _propStereoRendering;
		private SerializedProperty _propRender180Degrees;
		private SerializedProperty _propIPD;

		protected override void OnEnable()
		{
			base.OnEnable();

			//_capture = (this.target) as CaptureFromCamera360;

			_propCamera = serializedObject.FindProperty("_camera");

			_propRenderResolution = serializedObject.FindProperty("_renderResolution");
			_propRenderSize = serializedObject.FindProperty("_renderSize");
			_propAntiAliasing = serializedObject.FindProperty("_renderAntiAliasing");

			_propCubemapResolution = serializedObject.FindProperty("_cubemapResolution");
			_propCubemapDepth = serializedObject.FindProperty("_cubemapDepth");
			_propSupportGUI = serializedObject.FindProperty("_supportGUI");
			_propSupporCameraRotation = serializedObject.FindProperty("_supportCameraRotation");
			_propRender180Degrees = serializedObject.FindProperty("_render180Degrees");
			_propStereoRendering = serializedObject.FindProperty("_stereoRendering");
			_propIPD = serializedObject.FindProperty("_ipd");
		}

		protected void GUI_Camera()
		{
			EditorGUILayout.PropertyField(_propCamera);

			EditorUtils.EnumAsDropdown("Resolution", _propRenderResolution, CaptureBaseEditor.ResolutionStrings);

			if (_propRenderResolution.enumValueIndex == (int)CaptureBase.Resolution.Custom)
			{
				EditorGUILayout.PropertyField(_propRenderSize, new GUIContent("Size"));
				_propRenderSize.vector2Value = new Vector2(Mathf.Clamp((int)_propRenderSize.vector2Value.x, 1, NativePlugin.MaxRenderWidth), Mathf.Clamp((int)_propRenderSize.vector2Value.y, 1, NativePlugin.MaxRenderHeight));
			}
			{
				string currentAA = "None";
				if (QualitySettings.antiAliasing > 1)
				{
					currentAA = QualitySettings.antiAliasing.ToString() + "x";
				}
				EditorUtils.IntAsDropdown("Anti-aliasing", _propAntiAliasing, new string[] { "Current (" + currentAA + ")", "None", "2x", "4x", "8x" }, new int[] { -1, 1, 2, 4, 8 });
			}

			EditorUtils.IntAsDropdown("Cubemap Resolution", _propCubemapResolution, new string[] { "256", "512", "1024", "2048", "4096", "8192" }, new int[] { 256, 512, 1024, 2048, 4096, 8192 });
			EditorUtils.IntAsDropdown("Cubemap Depth", _propCubemapDepth, new string[] { "0", "16", "24" }, new int[] { 0, 16, 24 });
			EditorGUILayout.PropertyField(_propSupportGUI, new GUIContent("Capture GUI"));
			EditorGUILayout.PropertyField(_propSupporCameraRotation);
			EditorGUILayout.PropertyField(_propRender180Degrees);
			EditorGUILayout.PropertyField(_propStereoRendering);
			if (_propStereoRendering.enumValueIndex != (int)StereoPacking.None)
			{
				EditorGUILayout.PropertyField(_propIPD, new GUIContent("Interpupillary distance"));
			}
		}

		protected override void GUI_User()
		{
			if (_baseCapture != null && !_baseCapture.IsCapturing())
			{
				serializedObject.Update();

				bool boolTrue = true;
				EditorUtils.DrawSection("Capture From Camera 360+Stereo", ref boolTrue, GUI_Camera);

				if (serializedObject.ApplyModifiedProperties())
				{
					EditorUtility.SetDirty(target);
				}
			}
		}
	}
}
#endif