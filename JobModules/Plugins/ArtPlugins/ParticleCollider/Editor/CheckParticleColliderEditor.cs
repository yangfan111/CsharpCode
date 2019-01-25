using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

[CustomEditor(typeof(CheckParticleCollider))]
public class CheckParticleColliderEditor : Editor
{
    [MenuItem("CONTEXT/CheckParticleCollider/ApplyTrigger")]
    private static void ExcuteCheckCollider(MenuCommand command)
    {
        CheckParticleCollider autoPC = command.context as CheckParticleCollider;
        if (autoPC == null)
        {
            Debug.LogError("ExcuteCheckCollider error, autoPC is null");
            return;
        }

        autoPC.ApplyTrigger();
    }

    [MenuItem("CONTEXT/CheckParticleCollider/CancelTrigger")]
    private static void CancelTrigger(MenuCommand command)
    {
        CheckParticleCollider autoPC = command.context as CheckParticleCollider;
        if (autoPC == null)
        {
            Debug.LogError("ExcuteCheckCollider error, autoPC is null");
            return;
        }

        autoPC.CancelTrigger();
    }

    private List<string> layers = new List<string>();
    private int currentMask = -1;

    private SerializedProperty listProp;

    private SerializedProperty cullingMaskProp;
    private SerializedProperty colliderModeProp;
    private SerializedProperty showBoundProp;

    private SerializedProperty boxCenterProp;
    private SerializedProperty boxSizeProp;
    private SerializedProperty boxRotationSpaceProp;
    private SerializedProperty boxRotation;
    private GUIContent boxRotationSpaceGuiContent;
    private GUIContent boxRotationGuiContent;

    private SerializedProperty sphereCenterProp;
    private SerializedProperty sphereRadiusProp;

    private SerializedProperty alphaProp;

    private void OnEnable()
    {
        listProp = serializedObject.FindProperty("colliders");

        cullingMaskProp = serializedObject.FindProperty("cullingMask");
        colliderModeProp = serializedObject.FindProperty("colliderMode");
        showBoundProp = serializedObject.FindProperty("showBound");
        boxCenterProp = serializedObject.FindProperty("boxCenter");
        boxSizeProp = serializedObject.FindProperty("boxSize");
        boxRotationSpaceProp = serializedObject.FindProperty("boxRotationSpace");
        boxRotation = serializedObject.FindProperty("boxRotation");

        boxRotationSpaceGuiContent = new GUIContent("Space");
        boxRotationGuiContent = new GUIContent("Rotation");

        sphereCenterProp = serializedObject.FindProperty("sphereCenter");
        sphereRadiusProp = serializedObject.FindProperty("sphereRadius");
        alphaProp = serializedObject.FindProperty("alpha");
    }

    public override void OnInspectorGUI()
    {
        layers.Clear();
        for (int i = 0; i < 32; i++)
        {
            string name = LayerMask.LayerToName(i);
            if (!string.IsNullOrEmpty(name))
            {
                layers.Add(name);
            }
        }
        string[] names = layers.ToArray();

        serializedObject.Update();

        EditorGUILayout.Space();

        int oldMask = currentMask;
        currentMask = EditorGUILayout.MaskField("Culling Mask", currentMask, names);
        if (oldMask != currentMask) cullingMaskProp.intValue = currentMask;
        EditorGUILayout.PropertyField(colliderModeProp);
        EditorGUI.indentLevel++;
        switch (colliderModeProp.enumValueIndex)
        {
            case 0:                 // box
                {
                    EditorGUILayout.PropertyField(boxCenterProp);
                    EditorGUILayout.PropertyField(boxSizeProp);
                    if (boxSizeProp.floatValue < 0f) boxSizeProp.floatValue = 0f;
                    EditorGUILayout.PropertyField(boxRotationSpaceProp, boxRotationSpaceGuiContent);
                    EditorGUILayout.PropertyField(boxRotation, boxRotationGuiContent);
                }
                break;
            case 1:                 // sphere
                {
                    EditorGUILayout.PropertyField(sphereCenterProp);
                    EditorGUILayout.PropertyField(sphereRadiusProp);
                    if (sphereRadiusProp.floatValue < 0f) sphereRadiusProp.floatValue = 0f;
                }
                break;
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.PropertyField(showBoundProp);
        if (showBoundProp.boolValue)
        {
            EditorGUI.indentLevel++;
            alphaProp.floatValue = EditorGUILayout.Slider("Alpha", alphaProp.floatValue, 0f, 1f);
            EditorGUI.indentLevel--;
        }

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(listProp, true);
        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        if (!showBoundProp.boolValue) return;

        CheckParticleCollider pc = target as CheckParticleCollider;
        Handles.color = pc.enabled ? new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, alphaProp.floatValue)
                        : new Color(Color.red.r, Color.red.g, Color.red.b, alphaProp.floatValue);

        // draw bound
        switch (colliderModeProp.enumValueIndex)
        {
            case 0:                         // box 
                {
                    if (boxSizeProp.floatValue <= 0f) return;

                    Vector3 pos = pc.transform.TransformPoint(boxCenterProp.vector3Value);
                    Quaternion rot = Quaternion.Euler(boxRotation.vector3Value);
                    if (boxRotationSpaceProp.enumValueIndex == 1)               // local Space
                    {
                        rot = pc.transform.rotation * rot;
                    }
                    float size = boxSizeProp.floatValue;
                    Handles.CubeHandleCap(0, pos, rot, size, EventType.repaint);
                }
                break;
            case 1:                         // sphere
                {
                    float r = sphereRadiusProp.floatValue;
                    if (r <= 0f) return;

                    Vector3 pos = pc.transform.TransformPoint(sphereCenterProp.vector3Value);
                    Quaternion rot = pc.transform.rotation;
                    float size = 2 * sphereRadiusProp.floatValue;
                    Handles.SphereHandleCap(1, pos, rot, size, EventType.repaint);
                }
                break;
        }
    }

    private void OnDisable()
    {
        layers.Clear();
    }
}
