using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SplineTools
{
    [DisallowMultipleComponent]
    [CustomEditor(typeof(SplineSower))]
    public class SplineSowerEditor : Editor
    {
        [MenuItem("GameObject/3D Object/SplineSower")]
        private static void CreateSplineSower(MenuCommand commond)
        {
            GameObject newGo = new GameObject("SplineSower", typeof(SplineSower));
            Transform tr = newGo.transform;

            Transform parent = null;
            if (commond.context != null)
            {
                parent = (commond.context as GameObject).transform;
            }

            if (parent != null)
            {
                Undo.SetTransformParent(tr, parent, "Reparent SplineSower");
                tr.localPosition = Vector3.zero;
                tr.localScale = Vector3.one;
                tr.localRotation = Quaternion.identity;
                newGo.layer = parent.gameObject.layer;
            }
            else
            {
                if (SceneView.lastActiveSceneView != null)
                {
                    SceneView.lastActiveSceneView.MoveToView(tr);
                }
            }
            Selection.activeGameObject = newGo;
        }

        [MenuItem("CONTEXT/SplineSower/Rebuild Model")]
        private static void RebuildModel(MenuCommand commond)
        {
            if (commond.context == null) return;

            SplineSower ss = commond.context as SplineSower;
            ss.Sow();
        }

        private SerializedProperty splineProp;
        private SerializedProperty fbxModelProp;

        private SerializedProperty spacingProp;
        private SerializedProperty randomSpacingProp;
        private SerializedProperty spacingRangeProp;

        private SerializedProperty scaleProp;
        private SerializedProperty randomScaleProp;
        private SerializedProperty scaleRangeProp;

        private SerializedProperty yOffsetProp;
        private SerializedProperty randomYOffsetProp;
        private SerializedProperty yOffsetRangeProp;

        private SerializedProperty zOffsetProp;
        private SerializedProperty randomZOffsetProp;
        private SerializedProperty zOffsetRangeProp;

        private SerializedProperty rotationsProp;
        private GUIContent rotationsGuiContent = new GUIContent("Rotations");
        private bool isExpandRotations = false;
        // private SerializedProperty randomRotation;
        // private SerializedProperty RotationRange;

        // private SerializedProperty randomSeed;
        private SerializedProperty forceChildNoRotationProp;
        private SerializedProperty startOffsetProp;

        // private SerializedProperty modelsProp;

        private GUIContent modelsGuiContent;
        private bool modelsExpand = false;

        private void OnEnable()
        {
            splineProp = serializedObject.FindProperty("spline");
            fbxModelProp = serializedObject.FindProperty("fbxModel");

            spacingProp = serializedObject.FindProperty("spacing");
            randomSpacingProp = serializedObject.FindProperty("randomSpacing");
            spacingRangeProp = serializedObject.FindProperty("spacingRange");

            scaleProp = serializedObject.FindProperty("scale");
            randomScaleProp = serializedObject.FindProperty("randomScale");
            scaleRangeProp = serializedObject.FindProperty("scaleRange");

            yOffsetProp = serializedObject.FindProperty("yOffset");
            randomYOffsetProp = serializedObject.FindProperty("randomYOffset");
            yOffsetRangeProp = serializedObject.FindProperty("yOffsetRange");

            zOffsetProp = serializedObject.FindProperty("zOffset");
            randomZOffsetProp = serializedObject.FindProperty("randomZOffset");
            zOffsetRangeProp = serializedObject.FindProperty("zOffsetRange");

            rotationsProp = serializedObject.FindProperty("rotations");
            // randomRotation = serializedObject.FindProperty("randomRotation");
            // RotationRange = serializedObject.FindProperty("randomRotationRange");

            // randomSeed = serializedObject.FindProperty("randomSeed");
            forceChildNoRotationProp = serializedObject.FindProperty("forceChildNoRotation");
            startOffsetProp = serializedObject.FindProperty("startOffset");

            // modelsProp = serializedObject.FindProperty("models");
            modelsGuiContent = new GUIContent("Generate Models");
        }

        public override void OnInspectorGUI()
        {
            SplineSower ss = target as SplineSower;
            Spline spline = splineProp.objectReferenceValue as Spline;
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(splineProp);
            EditorGUILayout.PropertyField(fbxModelProp);

            EditorGUI.BeginDisabledGroup(splineProp.objectReferenceValue == null || fbxModelProp.objectReferenceValue == null);

            EditorGUILayout.PropertyField(startOffsetProp);
            if (startOffsetProp.floatValue > spline.GetLength()) startOffsetProp.floatValue = spline.GetLength();
            if (startOffsetProp.floatValue < 0f) startOffsetProp.floatValue = 0f;

            EditorGUILayout.PropertyField(spacingProp);
            if (spacingProp.floatValue < 1f) spacingProp.floatValue = 1f;
            EditorGUILayout.PropertyField(randomSpacingProp);
            if (randomSpacingProp.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(spacingRangeProp);
                if (spacingRangeProp.floatValue + spacingProp.floatValue < 0f) spacingRangeProp.floatValue = -spacingProp.floatValue;
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(scaleProp);
            if (scaleProp.floatValue < 0f) scaleProp.floatValue = 0f;
            EditorGUILayout.PropertyField(randomScaleProp);
            if (randomScaleProp.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(scaleRangeProp);
                if (scaleRangeProp.floatValue + scaleProp.floatValue < 0f) scaleRangeProp.floatValue = -scaleProp.floatValue;
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(yOffsetProp);
            EditorGUILayout.PropertyField(randomYOffsetProp);
            if (randomYOffsetProp.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(yOffsetRangeProp);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(zOffsetProp);
            EditorGUILayout.PropertyField(randomZOffsetProp);
            if (randomZOffsetProp.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(zOffsetRangeProp);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(forceChildNoRotationProp);

            // rotations
            isExpandRotations = EditorGUILayout.Foldout(isExpandRotations, rotationsGuiContent);
            if (isExpandRotations)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < rotationsProp.arraySize; i++)
                {
                    SerializedProperty rotProp = rotationsProp.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(rotProp, new GUIContent("curve" + i.ToString()));
                }
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                ss.Sow();
            }

            // models
            EditorGUI.BeginDisabledGroup(true);
            // EditorGUILayout.PropertyField(modelsProp, true);
            modelsExpand = EditorGUILayout.Foldout(modelsExpand, modelsGuiContent);
            if (modelsExpand)
            {
                EditorGUI.indentLevel++;
                var models = ss.GetModels();
                for (int i = 0; i < models.Count; i++)
                {
                    GameObject model = models[i];
                    EditorGUILayout.ObjectField(model.name, model, typeof(GameObject), false);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}