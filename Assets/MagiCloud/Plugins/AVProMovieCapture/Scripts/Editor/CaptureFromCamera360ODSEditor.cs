#if UNITY_5_5_OR_NEWER
	#define AVPRO_MOVIECAPTURE_UNITYPROFILER_55
#endif

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(CaptureFromCamera360ODS))]
	public class CaptureFromCamera360ODSEditor : CaptureBaseEditor
	{
		//private CaptureFromCamera360ODS _capture;

		private SerializedProperty _propCamera;
		private SerializedProperty _propRenderResolution;
		private SerializedProperty _propRenderSize;
		private SerializedProperty _propAntiAliasing;

		private SerializedProperty _propRender180Degrees;
		private SerializedProperty _propIPD;
		private SerializedProperty _propPixelSliceSize;
		private SerializedProperty _propPaddingSize;
		private SerializedProperty _propCameraClearMode;
		private SerializedProperty _propCameraClearColor;
		private SerializedProperty _propCameraImageEffects;

		protected override void OnEnable()
		{
			base.OnEnable();

			//_capture = (this.target) as CaptureFromCamera360ODS;

			_propCamera = serializedObject.FindProperty("_settings.camera");

			_propRenderResolution = serializedObject.FindProperty("_renderResolution");
			_propRenderSize = serializedObject.FindProperty("_renderSize");
			_propAntiAliasing = serializedObject.FindProperty("_renderAntiAliasing");

			_propRender180Degrees = serializedObject.FindProperty("_settings.render180Degrees");
			_propIPD = serializedObject.FindProperty("_settings.ipd");
			_propPixelSliceSize = serializedObject.FindProperty("_settings.pixelSliceSize");
			_propPaddingSize = serializedObject.FindProperty("_settings.paddingSize");
			_propCameraClearMode = serializedObject.FindProperty("_settings.cameraClearMode");
			_propCameraClearColor = serializedObject.FindProperty("_settings.cameraClearColor");
			_propCameraImageEffects = serializedObject.FindProperty("_settings.cameraImageEffects");
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

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(_propRender180Degrees);
			EditorGUILayout.PropertyField(_propIPD, new GUIContent("Interpupillary distance"));
			EditorGUILayout.PropertyField(_propPixelSliceSize);
			EditorGUILayout.PropertyField(_propPaddingSize);
			EditorGUILayout.PropertyField(_propCameraClearMode);
			EditorGUILayout.PropertyField(_propCameraClearColor);
			EditorGUILayout.PropertyField(_propCameraImageEffects, true);
		}

		protected override void GUI_User()
		{
			if (_baseCapture != null && !_baseCapture.IsCapturing())
			{
				serializedObject.Update();

				bool boolTrue = true;
				EditorUtils.DrawSection("Capture from Camera 360 + ODS", ref boolTrue, GUI_Camera);

#if AVPRO_MOVIECAPTURE_UNITYPROFILER_55
				// This component makes the profiler use a TON of memory, so warn the user to disable it
				if (UnityEngine.Profiling.Profiler.enabled)
				{
					ShowNoticeBox(MessageType.Warning, "Having the Unity profiler enabled while using the CaptureFromCamera360ODS component is not recommended.\n\nToo many samples are generated which can make the system run out of memory\n\nDisable the profiler, close the window and remove the tab.  A Unity restart may be required after disabling the profiler recording.");
				}
#endif

				if (serializedObject.ApplyModifiedProperties())
				{
					EditorUtility.SetDirty(target);
				}
			}
		}
	}
}
#endif