// 
// Created 8/28/2015 01:41:43
// Copyright © Hexagon Star Softworks. All Rights Reserved.
// http://www.hexagonstar.com/
//  

using UnityEditor;
using UnityEngine;


namespace StatsMonitor.Util
{
	public class EditorUtil
	{
		// ----------------------------------------------------------------------------
		// Public Methods
		// ----------------------------------------------------------------------------

		public static bool PropertyChanged(SerializedProperty property)
		{
			return PropertyChanged(property, null);
		}


		public static bool PropertyChanged(SerializedProperty property, GUIContent content, params GUILayoutOption[] options)
		{
			EditorGUI.BeginChangeCheck();
			if (content == null) EditorGUILayout.PropertyField(property, options);
			else EditorGUILayout.PropertyField(property, content, options);
			return EditorGUI.EndChangeCheck();
		}


		public static bool ToggleFoldout(SerializedProperty foldout, string caption, SerializedProperty toggle)
		{
			bool changed = false;
			Rect fRect = EditorGUILayout.BeginHorizontal();
			Rect tRect = new Rect(fRect);
			tRect.width = 16;
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(tRect, toggle, new GUIContent(""));
			if (EditorGUI.EndChangeCheck()) changed = true;
			fRect.xMin = tRect.xMax + 16;
			foldout.boolValue = EditorGUI.Foldout(fRect, foldout.boolValue, caption, true);
			EditorGUILayout.LabelField("");
			EditorGUILayout.EndHorizontal();
			return changed;
		}


		public static bool Foldout(SerializedProperty foldout, string caption)
		{
			Rect fRect = EditorGUILayout.BeginHorizontal();
			fRect.xMin += 11;
			foldout.boolValue = EditorGUI.Foldout(fRect, foldout.boolValue, caption, true);
			EditorGUILayout.LabelField("");
			EditorGUILayout.EndHorizontal();
			return foldout.boolValue;
		}
	}
}
