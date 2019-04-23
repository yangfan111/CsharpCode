#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class InspectorBuildHelper
{
    public static void BuildEnum(UnityEditor.SerializedProperty serializedProperty, EAudioEmitType enumTag)
    {
        EAudioEmitType etype = (EAudioEmitType) serializedProperty.enumValueIndex;
        etype = (EAudioEmitType) UnityEditor.EditorGUILayout.EnumPopup("EmitType: ", etype,
            GUILayout.ExpandWidth(true));
        serializedProperty.enumValueIndex = (int) etype;
    }

    public static void BuildEnum(UnityEditor.SerializedProperty serializedProperty, EAudioEventClassify enumTag)
    {
        EAudioEventClassify etype = (EAudioEventClassify) serializedProperty.enumValueIndex;
        etype = (EAudioEventClassify) UnityEditor.EditorGUILayout.EnumPopup("EventClassify: ", etype,
            GUILayout.ExpandWidth(true));
        serializedProperty.enumValueIndex = (int) etype;
    }

    public static void BuildEnum(UnityEditor.SerializedProperty serializedProperty, EAudioTriggerType enumTag)
    {
        EAudioTriggerType etype = (EAudioTriggerType) serializedProperty.enumValueIndex;
        etype = (EAudioTriggerType) UnityEditor.EditorGUILayout.EnumPopup("TriggerType: ", etype,
            GUILayout.ExpandWidth(true));
        serializedProperty.enumValueIndex = (int) etype;
    }

    public static void BuildTip(string label)
    {
        if (!string.IsNullOrEmpty(label))
            UnityEditor.EditorGUILayout.LabelField(label, GUILayout.ExpandWidth(true));
    }

    public static void BuildSimple(UnityEditor.SerializedProperty serializedProperty, string label)
    {
        UnityEditor.EditorGUILayout.PropertyField(serializedProperty, new UnityEngine.GUIContent(label));
    }

    public static void GetClassFieldActions(FieldInfo[]  fieldInfos, UnityEditor.SerializedProperty serializedProperty,
                                            List<Action> actions)
    {
        for (int i = 0; i < fieldInfos.Length; i++)
        {
            var  fieldInfo   = fieldInfos[i];
            bool fieldIgnore = false;
            var  customAttrs = fieldInfo.GetCustomAttributes(false);
            foreach (var attr in customAttrs)
            {
                if (attr is IgnoreRelativeAttribute)
                {
                    fieldIgnore = true;
                    break;
                }
            }

            if (!fieldIgnore)
                actions.Add(() => BuildSimple(serializedProperty.FindPropertyRelative(fieldInfo.Name), fieldInfo.Name));
        }

        //  BuildVerticalBox(null, "SourceData");
    }

    public static void BuildSlider(SerializedProperty transitionDuration, Vector2 scope, string label)
    {
        UnityEditor.EditorGUILayout.Slider(transitionDuration, scope.x, scope.y,
            new UnityEngine.GUIContent(label));
    }


    public static bool BuildButton(string label)
    {
        return UnityEngine.GUILayout.Button(label, InspectorBuildConst.DefaultStyle,
            InspectorBuildConst.DefaultLayoutOption);
    }

    public static void BuildVerticalBoxRear()
    {
        UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);
    }

    public static void BuildVerticalBoxHead(Action initAction, string label)
    {
        UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);
        using (new UnityEditor.EditorGUILayout.VerticalScope("box"))
        {
            if (label.Length > 0)
                UnityEditor.EditorGUILayout.LabelField(label, GUILayout.ExpandWidth(true));
            initAction();
        }
    }

    public static void BuildVerticalBox(List<Action> actions, string label, Func<bool> activeFunc = null)
    {
        if (activeFunc != null && !activeFunc())
            return;
        UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);

        using (new UnityEditor.EditorGUILayout.VerticalScope("box"))
        {
            if (label.Length > 0)
                UnityEditor.EditorGUILayout.LabelField(label, GUILayout.ExpandWidth(true));
            //UnityEditor.EditorGUILayout.PropertyField(enableActionOnEvent, new UnityEngine.GUIContent("Action On Event: "));
            for (int i = 0; i < actions.Count; i++)
                actions[i]();
        }

        UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);
    }


}

#endif