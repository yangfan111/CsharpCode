using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SplineTools
{
    [CustomEditor(typeof(SplineModel))]
    public class SplineModelEditor : Editor
    {
        [MenuItem("GameObject/3D Object/SplineModel")]
        private static void CreateSplineModel(MenuCommand command)
        {
            GameObject newGo = new GameObject("SplineModel", typeof(SplineModel));
            Transform tr = newGo.transform;

            Transform parent = null;
            if (command.context != null)
            {
                parent = (command.context as GameObject).transform;
            }

            if (parent != null)
            {
                Undo.SetTransformParent(tr, parent, "Reparent SplineModel");
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

        [MenuItem("CONTEXT/SplineModel/Rebuild Model")]
        private static void RebuildModel(MenuCommand command)
        {
            if (command.context == null) return;

            SplineModel sm = command.context as SplineModel;
            sm.RebuildMeshes();
        }

        private SerializedProperty modelPrefabProp;
        private GUIContent modelPrefabGuiContent;

        private GUIContent specialModelsGuiContent;
        private bool specialModelsExpand = false;

        private SerializedProperty splineProp;
        private GUIContent splineGuiContent;

        private SerializedProperty rotationProp;
        private GUIContent rotationGuiContent;

        private SerializedProperty startScaleProp;
        private GUIContent startScaleGuiContent;

        private SerializedProperty endScaleProp;
        private GUIContent endScaleGuiContent;

        private SerializedProperty yOffsetProp;
        private GUIContent yOffsetGuiContent;

        private SerializedProperty zOffsetProp;
        private GUIContent zOffsetGuiContent;

        private SerializedProperty startRollProp;
        private SerializedProperty endRollProp;

        private SerializedProperty curveStartOffsetProp;
        private SerializedProperty curveEndOffsetProp;
        private GUIContent curveOffsetGuiContent;

        private SerializedProperty forceChildNoRotationProp;

        // private SerializedProperty modelsProp;
        private GUIContent modelsGuiContent;
        private bool modelsExpand = false;

        private void OnEnable()
        {
            modelPrefabProp = serializedObject.FindProperty("modelPrefab");
            modelPrefabGuiContent = new GUIContent("Fbx Model");

            specialModelsGuiContent = new GUIContent("Curve Models");

            splineProp = serializedObject.FindProperty("spline");
            splineGuiContent = new GUIContent("Spline");

            rotationProp = serializedObject.FindProperty("rotation");
            rotationGuiContent = new GUIContent("Rotation");

            startScaleProp = serializedObject.FindProperty("startScale");
            startScaleGuiContent = new GUIContent("Start Scale");

            endScaleProp = serializedObject.FindProperty("endScale");
            endScaleGuiContent = new GUIContent("End Scale");

            yOffsetProp = serializedObject.FindProperty("yOffset");
            yOffsetGuiContent = new GUIContent("Y Offset");

            zOffsetProp = serializedObject.FindProperty("zOffset");
            zOffsetGuiContent = new GUIContent("Z Offset");

            startRollProp = serializedObject.FindProperty("startRoll");
            endRollProp = serializedObject.FindProperty("endRoll");

            curveStartOffsetProp = serializedObject.FindProperty("curveStartOffset");
            curveEndOffsetProp = serializedObject.FindProperty("curveEndOffset");
            curveOffsetGuiContent = new GUIContent("Curve Offset");

            forceChildNoRotationProp = serializedObject.FindProperty("forceChildNoRotation");

            // modelsProp = serializedObject.FindProperty("models");
            modelsGuiContent = new GUIContent("Generate Models");
        }

        public override void OnInspectorGUI()
        {
            SplineModel sm = target as SplineModel;
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            // spline field
            EditorGUILayout.PropertyField(splineProp, splineGuiContent);

            // fbx model field
            EditorGUILayout.PropertyField(modelPrefabProp, modelPrefabGuiContent);

            // base meshes
            GameObject modelGo = modelPrefabProp.objectReferenceValue as GameObject;
            if (modelGo != null)
            {
                // 碰撞器网格列表
                var mcs = modelGo.GetComponentsInChildren<MeshCollider>(true);
                if (mcs.Length > 0)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Base Collider Meshes");
                    EditorGUI.BeginDisabledGroup(true);
                }
                foreach (var mc in mcs)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.ObjectField(mc.sharedMesh, typeof(Mesh), true);
                    EditorGUI.indentLevel--;
                }
                if (mcs.Length > 0)
                {
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.indentLevel--;
                }

                // 渲染网格列表
                var mfs = modelGo.GetComponentsInChildren<MeshFilter>(true);
                if (mfs.Length > 0)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Base Render Meshes");
                    EditorGUI.BeginDisabledGroup(true);
                }
                foreach (var mf in mfs)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.ObjectField(mf.sharedMesh, typeof(Mesh), true);
                    EditorGUI.indentLevel--;
                }
                if (mfs.Length > 0)
                {
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.indentLevel--;
                }
            }

            // EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(splineProp.objectReferenceValue == null || modelPrefabProp.objectReferenceValue == null);

            // rotation field
            EditorGUILayout.PropertyField(rotationProp, rotationGuiContent);

            // scale field
            EditorGUILayout.PropertyField(startScaleProp, startScaleGuiContent);
            EditorGUILayout.PropertyField(endScaleProp, endScaleGuiContent);

            // yOffset field
            EditorGUILayout.PropertyField(yOffsetProp, yOffsetGuiContent);

            // zOffset field
            EditorGUILayout.PropertyField(zOffsetProp, zOffsetGuiContent);

            // roll field
            EditorGUILayout.PropertyField(startRollProp);
            EditorGUILayout.PropertyField(endRollProp);

            // curve x offset 
            float left = curveStartOffsetProp.floatValue;
            float right = curveEndOffsetProp.floatValue;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(curveOffsetGuiContent);
            GUILayout.Space(-60f);
            left = EditorGUILayout.FloatField(left, GUILayout.MaxWidth(100f));
            EditorGUILayout.MinMaxSlider(ref left, ref right, 0f, 1f);
            right = EditorGUILayout.FloatField(right, GUILayout.MaxWidth(100f));
            EditorGUILayout.EndHorizontal();
            if (left >= right) left = right;
            left = Mathf.Clamp01(left);
            right = Mathf.Clamp01(right);
            curveStartOffsetProp.floatValue = left;
            curveEndOffsetProp.floatValue = right;

            // force child no rotation
            EditorGUILayout.PropertyField(forceChildNoRotationProp);

            // curve models
            bool expandChange = false;
            EditorGUI.BeginChangeCheck();
            specialModelsExpand = EditorGUILayout.Foldout(specialModelsExpand, specialModelsGuiContent);
            if (EditorGUI.EndChangeCheck()) expandChange = true;
            if (specialModelsExpand)
            {
                int curvesCount = sm.spline.GetCurves().Count;

                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Size", curvesCount.ToString());
                EditorGUI.EndDisabledGroup();
                for (int i = 0; i < curvesCount; i++)
                {
                    GameObject baseGo = null;
                    sm.specialModels.TryGetValue(i, out baseGo);
                    if (baseGo == null) baseGo = sm.modelPrefab;
                    EditorGUILayout.BeginHorizontal();
                    GameObject newBaseGo = EditorGUILayout.ObjectField("Curve" + i, baseGo, typeof(GameObject), false) as GameObject;
                    if (GUILayout.Button("Reset", GUILayout.MaxWidth(50f)))
                    {
                        newBaseGo = modelPrefabProp.objectReferenceValue as GameObject;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (!ReferenceEquals(newBaseGo, baseGo))
                    {
                        if (sm.specialModels.ContainsKey(i))
                        {
                            sm.specialModels[i] = newBaseGo;
                        }
                        else
                        {
                            sm.specialModels.Add(i, newBaseGo);
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck() && !expandChange)
            {
                serializedObject.ApplyModifiedProperties();
                sm.RebuildMeshes();
            }

            // models
            EditorGUI.BeginDisabledGroup(true);
            modelsExpand = EditorGUILayout.Foldout(modelsExpand, modelsGuiContent);
            if (modelsExpand)
            {
                EditorGUI.indentLevel++;
                var models = sm.GetModels();
                for (int i = 0; i < models.Count; i++)
                {
                    GameObject model = models[i];
                    EditorGUILayout.ObjectField(model.name, model, typeof(GameObject), false);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            // save button
            if (GUILayout.Button("Build"))
            {
                string path = EditorUtility.SaveFilePanelInProject("title", "splinemodel", "asset", "save the final asset");
                if (!string.IsNullOrEmpty(path))
                {
                    // save meshes
                    Mesh mesh = new Mesh();
                    AssetDatabase.CreateAsset(mesh, path);
                    var models = sm.GetModels();
                    foreach (var model in models)
                    {
                        var mfs = model.GetComponentsInChildren<MeshFilter>(true);
                        foreach (var mf in mfs)
                        {
                            if (mf != null && mf.sharedMesh != null)
                            {
                                mf.sharedMesh.name = string.Format("{0}_{1}", model.name, mf.name);
                                AssetDatabase.AddObjectToAsset(mf.sharedMesh, mesh);
                            }
                        }
                        var mcs = model.GetComponentsInChildren<MeshCollider>(true);
                        foreach (var mc in mcs)
                        {
                            if (mc != null && mc.sharedMesh != null)
                            {
                                mc.sharedMesh.name = string.Format("{0}_{1}", model.name, mc.name);
                                AssetDatabase.AddObjectToAsset(mc.sharedMesh, mesh);
                            }
                        }
                    }
                    AssetDatabase.ImportAsset(path);
                    AssetDatabase.Refresh();

                    // remove spline scripts
                    foreach (var model in models)
                    {
                        FbxModelBender bender = model.GetComponent<FbxModelBender>();
                        DestroyImmediate(bender);
                    }
                    GameObject go = (target as SplineModel).gameObject;
                    DestroyImmediate(sm);
                    Spline sp = go.GetComponent<Spline>();
                    DestroyImmediate(sp);
                }
            }
        }
    }
}
