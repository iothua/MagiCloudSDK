#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(CaptureFromScreen))]
	public class CaptureFromScreenEditor : CaptureBaseEditor
	{
		//private CaptureFromScreen _capture;

		private SerializedProperty _propCaptureMouseCursor;
		private SerializedProperty _propMouseCursor;

		protected override void GUI_Misc()
		{
			GUI_MouseCursor();
			base.GUI_Misc();
		}

		protected void GUI_MouseCursor()
		{
			EditorGUILayout.PropertyField(_propCaptureMouseCursor);
			if (_propCaptureMouseCursor.boolValue)
			{
				EditorGUILayout.PropertyField(_propMouseCursor);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			_propCaptureMouseCursor = serializedObject.FindProperty("_captureMouseCursor");
			_propMouseCursor = serializedObject.FindProperty("_mouseCursor");
		}
	}
}
#endif