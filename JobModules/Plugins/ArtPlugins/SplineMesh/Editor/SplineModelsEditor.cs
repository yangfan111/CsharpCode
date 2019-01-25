using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SplineTools
{
    [CustomEditor(typeof(SplineModels))]
    public class SplineModelsEditor : Editor
    {
        [MenuItem("GameObject/3D Object/SplineModels")]
        private static void CreateSplineModels(MenuCommand command)
        {
            GameObject newGo = new GameObject("SplineModels", typeof(SplineModels));
            Transform tr = newGo.transform;

            Transform parent = null;
            if (command.context != null)
            {
                parent = (command.context as GameObject).transform;
            }

            if (parent != null)
            {
                Undo.SetTransformParent(tr, parent, "Reparent SplineModels");
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

        [MenuItem("CONTEXT/SplineModels/Rebuild Model")]
        private static void RebuildModel(MenuCommand command)
        {
            if (command.context == null) return;

            SplineModels sm = command.context as SplineModels;
            sm.RebuildMeshes();
        }

        private SplineModels splineModels;
        private GUIContent splineGuiContent = new GUIContent("Spline");

        private SerializedProperty curvesProp;
        private GUIContent curvesGuiContent = new GUIContent("Curves");
        private bool isExpandCurves = false;

        private GUIContent OffsetGui = new GUIContent("Offset");

        private GUIContent modelsGuiContent = new GUIContent("Models");

        private void OnEnable()
        {
            splineModels = target as SplineModels;
            splineModels.InitCurves();
            curvesProp = serializedObject.FindProperty("curves");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // spline
            EditorGUILayout.ObjectField(splineGuiContent, splineModels.spline, typeof(Spline), false);

            // curves
            isExpandCurves = EditorGUILayout.Foldout(isExpandCurves, curvesGuiContent);
            if (isExpandCurves)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < curvesProp.arraySize; i++)
                {
                    var curve = splineModels.curves[i];
                    curve.isExpandCurveGui = EditorGUILayout.Foldout(curve.isExpandCurveGui, "curve" + i.ToString());
                    if (curve.isExpandCurveGui)
                    {
                        EditorGUI.indentLevel++;
                        var curveProp = curvesProp.GetArrayElementAtIndex(i);
                        var fbxProp = curveProp.FindPropertyRelative("modelPrefab");
                        var numProp = curveProp.FindPropertyRelative("numbers");
                        var rotationProp = curveProp.FindPropertyRelative("rotation");
                        var startScaleProp = curveProp.FindPropertyRelative("startScale");
                        var endScaleProp = curveProp.FindPropertyRelative("endScale");
                        var yOffsetProp = curveProp.FindPropertyRelative("yOffset");
                        var zOffsetProp = curveProp.FindPropertyRelative("zOffset");
                        var startRollProp = curveProp.FindPropertyRelative("startRoll");
                        var endRollProp = curveProp.FindPropertyRelative("endRoll");
                        var startOffsetProp = curveProp.FindPropertyRelative("startOffset");
                        var endOffsetProp = curveProp.FindPropertyRelative("endOffset");
                        var spaceProp = curveProp.FindPropertyRelative("space");
                        var forceChildNoRotationProp = curveProp.FindPropertyRelative("forceChildNoRotation");

                        bool needRepro = false;

                        EditorGUI.BeginChangeCheck();
                        var oldFbxValue = fbxProp.objectReferenceValue;
                        EditorGUILayout.PropertyField(fbxProp);
                        if (!ReferenceEquals(fbxProp.objectReferenceValue, oldFbxValue)) needRepro = true;
                        if (fbxProp.objectReferenceValue != null)
                        {
                            GameObject fbxGo = fbxProp.objectReferenceValue as GameObject;

                            // 碰撞器网格
                            var mcs = fbxGo.GetComponentsInChildren<MeshCollider>(true);
                            if (mcs.Length > 0)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.LabelField("Base Collider Meshes");
                                EditorGUI.BeginDisabledGroup(true);
                            }
                            foreach (var mc in mcs)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.ObjectField(mc.sharedMesh, typeof(Mesh), false);
                                EditorGUI.indentLevel--;
                            }
                            if (mcs.Length > 0)
                            {
                                EditorGUI.EndDisabledGroup();
                                EditorGUI.indentLevel--;
                            }

                            // 渲染网格
                            var mfs = fbxGo.GetComponentsInChildren<MeshFilter>(true);
                            if (mfs.Length > 0)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.LabelField("Base Render Meshes");
                                EditorGUI.BeginDisabledGroup(true);
                            }
                            foreach (var mf in mfs)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.ObjectField(mf.sharedMesh, typeof(Mesh), false);
                                EditorGUI.indentLevel--;
                            }
                            if (mfs.Length > 0)
                            {
                                EditorGUI.EndDisabledGroup();
                                EditorGUI.indentLevel--;
                            }
                        }
                        EditorGUI.BeginDisabledGroup(fbxProp.objectReferenceValue == null);
                        var oldNumValue = numProp.intValue;
                        EditorGUILayout.PropertyField(numProp);
                        numProp.intValue = Mathf.Clamp(numProp.intValue, 1, 1000);
                        if (numProp.intValue != oldNumValue) needRepro = true;
                        EditorGUILayout.PropertyField(rotationProp);
                        EditorGUILayout.PropertyField(startScaleProp);
                        EditorGUILayout.PropertyField(endScaleProp);
                        EditorGUILayout.PropertyField(yOffsetProp);
                        EditorGUILayout.PropertyField(zOffsetProp);
                        EditorGUILayout.PropertyField(startRollProp);
                        EditorGUILayout.PropertyField(endRollProp);
                        EditorGUILayout.PropertyField(forceChildNoRotationProp);
                        float oldMin = startOffsetProp.floatValue, oldMax = endOffsetProp.floatValue;
                        float min = oldMin, max = oldMax;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(OffsetGui);
                        min = EditorGUILayout.FloatField(min, GUILayout.MaxWidth(150f));
                        min = Mathf.Clamp(min, 0f, max);
                        EditorGUILayout.MinMaxSlider(ref min, ref max, 0f, 1f);
                        max = EditorGUILayout.FloatField(max, GUILayout.MaxWidth(150f));
                        max = Mathf.Clamp(max, min, 1f);
                        EditorGUILayout.EndHorizontal();
                        if (max - min < 0.01f)
                        {
                            if (min + 0.01f <= 1f) max = min + 0.01f;
                            else min = max - 0.01f;
                        }
                        startOffsetProp.floatValue = min;
                        endOffsetProp.floatValue = max;
                        EditorGUILayout.PropertyField(spaceProp);
                        float maxSpace = 0f;
                        if (numProp.intValue != 1) maxSpace = (endOffsetProp.floatValue - startOffsetProp.floatValue - 0.01f) / (numProp.intValue - 1);
                        spaceProp.floatValue = Mathf.Clamp(spaceProp.floatValue, 0f, maxSpace);
                        if (EditorGUI.EndChangeCheck())
                        {
                            serializedObject.ApplyModifiedProperties();
                            splineModels.RebuildCurve(i, needRepro);
                        }
                        EditorGUI.EndDisabledGroup();
                        curve.isExpandModelsGui = EditorGUILayout.Foldout(curve.isExpandModelsGui, modelsGuiContent);
                        if (curve.isExpandModelsGui)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUI.BeginDisabledGroup(true);
                            foreach (var model in curve.models)
                            {
                                EditorGUILayout.ObjectField(model, typeof(GameObject), false);
                            }
                            EditorGUI.EndDisabledGroup();
                            EditorGUI.indentLevel--;
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}