#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(CaptureFromCamera))]
	public class CaptureFromCameraEditor : CaptureBaseEditor
	{
		//private CaptureFromCamera _capture;

		private SerializedProperty _propLastCamera;
		private SerializedProperty _propContribCameras;
		private SerializedProperty _propUseContribCameras;

		private SerializedProperty _propRenderResolution;
		private SerializedProperty _propRenderSize;
		private SerializedProperty _propAntiAliasing;

		private static readonly GUIContent _guiItemCamera = new GUIContent("Camera", "The top level camera you want to capture");
		private static readonly GUIContent _guiItemContribCamera = new GUIContent("Contributing Cameras", "Cameras in render order from first to last that contribute to the rendering of the main camera above");

		protected override void OnEnable()
		{
			base.OnEnable();

			//_capture = (this.target) as CaptureFromCamera;

			_propLastCamera = serializedObject.FindProperty("_lastCamera");
			_propContribCameras = serializedObject.FindProperty("_contribCameras");
			_propContribCameras.isExpanded = true;
			_propUseContribCameras = serializedObject.FindProperty("_useContributingCameras");

			_propRenderResolution = serializedObject.FindProperty("_renderResolution");
			_propRenderSize = serializedObject.FindProperty("_renderSize");
			_propAntiAliasing = serializedObject.FindProperty("_renderAntiAliasing");
		}

		protected void GUI_Camera()
		{
			Camera prevLastCamera = (Camera)_propLastCamera.objectReferenceValue;

			EditorGUILayout.PropertyField(_propLastCamera, _guiItemCamera);

			Camera lastCamera = (Camera)_propLastCamera.objectReferenceValue;

			// If the user has changed the camera, reset the contributing cameras
			if (lastCamera != prevLastCamera)
			{
				_propContribCameras.arraySize = 0;
				if (lastCamera == null)
				{
					_propUseContribCameras.boolValue = false;
				}
			}

			if (lastCamera != null)
			{
				_propUseContribCameras.boolValue = EditorGUILayout.ToggleLeft("Use Contributing Cameras", _propUseContribCameras.boolValue);

				if (_propUseContribCameras.boolValue)
				{
					if (GUILayout.Button("Find Contributing Cameras", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false)))
					{
						bool hasContribCameras = false;
						if (Utils.HasContributingCameras(lastCamera))
						{
							Camera[] cameras = Utils.FindContributingCameras(lastCamera);
							if (cameras != null && cameras.Length > 0)
							{
								hasContribCameras = true;
								_propContribCameras.arraySize = cameras.Length;
								for (int slotIndex = 0; slotIndex < cameras.Length; slotIndex++)
								{
									_propContribCameras.GetArrayElementAtIndex(slotIndex).objectReferenceValue = cameras[slotIndex];
								}
							}
						}
						
						if (!hasContribCameras)
						{
							_propContribCameras.arraySize = 0;
							_propUseContribCameras.boolValue = false;
						}
					}

					EditorGUILayout.PropertyField(_propContribCameras, _guiItemContribCamera, true);
					EditorGUILayout.Space();
				}
			}


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
		}

		protected override void GUI_User()
		{
			if (_baseCapture != null && !_baseCapture.IsCapturing())
			{
				serializedObject.Update();

				bool boolTrue = true;
				EditorUtils.DrawSection("Capture From Camera", ref boolTrue, GUI_Camera);

				if (serializedObject.ApplyModifiedProperties())
				{
					EditorUtility.SetDirty(target);
				}
			}
		}

		/*
		public override void OnInspectorGUI()
		{
			GUI_Header();



			GUI_BaseOptions();
		}*/
	}
}
#endif