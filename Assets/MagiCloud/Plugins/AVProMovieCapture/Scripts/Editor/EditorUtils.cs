#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture.Editor
{
	/*public static class Utils
	{
		public static T GetCopyOf<T>(this Component comp, T other) where T : Component
		{
			System.Type type = comp.GetType();
			if (type != other.GetType())
			{
				return null; // type mis-match
			}
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
			PropertyInfo[] pinfos = type.GetProperties(flags);
			for (int i = 0; i < pinfos.Length; i++)
			{
				PropertyInfo pinfo = pinfos[i];
				if (pinfo.CanWrite)
				{
					try
					{
						pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
					}
					catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
				}
			}
			FieldInfo[] finfos = type.GetFields(flags);
			foreach (var finfo in finfos)
			{
				finfo.SetValue(comp, finfo.GetValue(other));
			}
			return comp as T;
		}
	}*/

	public static class EditorUtils
	{
		public static void CentreLabel(string text, GUIStyle style = null)
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (style == null)
			{
				GUILayout.Label(text);
			}
			else
			{
				GUILayout.Label(text, style);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		public static void BoolAsDropdown(string name, SerializedProperty prop, string trueOption, string falseOption)
		{
			string[] popupNames = { trueOption, falseOption };
			int popupIndex = 0;
			if (!prop.boolValue)
			{
				popupIndex = 1;
			}
			popupIndex = EditorGUILayout.Popup(name, popupIndex, popupNames);
			prop.boolValue = (popupIndex == 0);
		}

		public static void EnumAsDropdown(string name, SerializedProperty prop, string[] options)
		{
			prop.enumValueIndex = EditorGUILayout.Popup(name, prop.enumValueIndex, options);
		}

		public static void IntAsDropdown(string name, SerializedProperty prop, string[] options, int[] values)
		{
			int index = 0;
			for (int i = 0; i < values.Length; i++)
			{
				if (values[i] == prop.intValue)
				{
					index = i;
					break;
				}
			}
			index = EditorGUILayout.Popup(name, index, options);
			prop.intValue = values[index];
		}

		public static void DrawSection(string name, ref bool isExpanded, System.Action action)
		{
			Color boxbgColor = new Color(0.8f, 0.8f, 0.8f, 0.1f);
			if (EditorGUIUtility.isProSkin)
			{
				boxbgColor = Color.black;
			}
				DrawSectionColored(name, ref isExpanded, action, boxbgColor, Color.white, Color.white);
		}

		public static void DrawSectionColored(string name, ref bool isExpanded, System.Action action, Color boxbgcolor, Color bgcolor, Color color)
		{
			GUI.color = Color.white;
			GUI.backgroundColor = Color.clear;
			//GUI.backgroundColor = bgcolor;
			if (isExpanded)
			{
				GUI.color = Color.white;
				GUI.backgroundColor = boxbgcolor;
			}

			GUILayout.BeginVertical("box");
			GUI.color = color;
			GUI.backgroundColor = bgcolor;
			
			if (GUILayout.Button(name, EditorStyles.toolbarButton))
			{
				isExpanded = !isExpanded;
			}
			//GUI.backgroundColor = Color.white;
			//GUI.color = Color.white;

			if (isExpanded)
			{
				action.Invoke();
			}

			GUI.backgroundColor = Color.white;
			GUI.color = Color.white;

			GUILayout.EndVertical();
		}
	}
}
#endif