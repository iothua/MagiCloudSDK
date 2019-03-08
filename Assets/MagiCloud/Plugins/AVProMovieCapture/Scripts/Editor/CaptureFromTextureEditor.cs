#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(CaptureFromTexture))]
	public class CaptureFromTextureEditor : CaptureBaseEditor
	{
		private SerializedProperty _propManualUpdate;

		protected override void OnEnable()
		{
			base.OnEnable();

			_propManualUpdate = serializedObject.FindProperty("_manualUpdate");
		}

		protected void GUI_Camera()
		{
			EditorGUILayout.PropertyField(_propManualUpdate);
		}

		protected override void GUI_User()
		{
			if (_baseCapture != null && !_baseCapture.IsCapturing())
			{
				serializedObject.Update();

				bool boolTrue = true;
				EditorUtils.DrawSection("Capture From Texture", ref boolTrue, GUI_Camera);

				if (serializedObject.ApplyModifiedProperties())
				{
					EditorUtility.SetDirty(target);
				}
			}
		}
	}
}
#endif