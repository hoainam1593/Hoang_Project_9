using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorUIElementCreator
{
	#region button

	public static bool CreateButton(string text)
	{
		var size = StaticUtilsEditor.CalculateTextSize(text, EditorUIComponentType.button);
		var width = size.x;
		return GUILayout.Button(text, GUILayout.Width(width));
	}

	#endregion

	#region lable field

	public static void CreateLabelField(string text)
	{
		var size = StaticUtilsEditor.CalculateTextSize(text, EditorUIComponentType.label);
		var width = size.x;
		EditorGUILayout.LabelField(text, GUILayout.Width(width));
	}

	public static void CreateLabelField_center(string text, float parentWidth, float parentHeight, int fontSize)
	{
		var style = new GUIStyle(GUI.skin.label)
		{
			alignment = TextAnchor.MiddleCenter,
			fixedWidth = parentWidth,
			fixedHeight = parentHeight,
			fontSize = fontSize,
		};

		EditorGUILayout.LabelField(text, style);
	}

	#endregion

	#region toggle

	public static bool CreateToggle(string label, bool val)
	{
		var labelWidth = StaticUtilsEditor.CalculateTextSize(label, EditorUIComponentType.label).x;
		var t = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = labelWidth;
		var result = EditorGUILayout.Toggle(label, val);
		EditorGUIUtility.labelWidth = t;
		return result;
	}

	#endregion

	#region drop-down list

	public static string CreateDropdownList_string(string label, string val, string[] options, out bool hasChanged)
	{
		EditorGUILayout.BeginHorizontal();

		if (!string.IsNullOrEmpty(label))
		{
			CreateLabelField(label);
		}
		var idx = options.IndexOf(val);
		idx = EditorGUILayout.Popup(idx, options);

		EditorGUILayout.EndHorizontal();

		var newVal = options[idx];
		hasChanged = !val.Equals(newVal);
		return newVal;
	}

	public static T CreateDropdownList_enum<T>(string label, T val, List<T> options, out bool hasChanged) where T : struct, IConvertible
	{
		var optionsAsStr = new string[options.Count];
		for (var i = 0; i < options.Count; i++)
		{
			optionsAsStr[i] = options[i].ToString();
		}

		var resultAsStr = CreateDropdownList_string(label, val.ToString(), optionsAsStr, out hasChanged);
		return StaticUtils.StringToEnum<T>(resultAsStr);
	}

	public static T CreateDropdownList_enum<T>(string label, T numVal, T val, out bool hasChanged) where T : struct, IConvertible
	{
		var numValAsInt = Convert.ToInt32(numVal);
		var options = new List<T>();
		for (var i = 0; i < numValAsInt; i++)
		{
			var enumVal = StaticUtils.IntToEnum<T>(i);
			options.Add(enumVal);
		}

		return CreateDropdownList_enum(label, val, options, out hasChanged);
	}

	#endregion
}