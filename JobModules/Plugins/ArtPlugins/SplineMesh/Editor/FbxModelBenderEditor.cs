using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SplineTools
{
    [CustomEditor(typeof(FbxModelBender))]
    public class FbxModelBenderEditor : Editor
    {
        private List<GameObject> colliderMeshes = new List<GameObject>();
        private List<GameObject> rendererMeshes = new List<GameObject>();

        private string colliderMeshesUI = "ColliderMeshes";
        private string rendererMeshesUI = "RendererMeshes";
        private bool isExpandCollider = true;
        private bool isExpandRenderer = true;

        public override void OnInspectorGUI()
        {
            FbxModelBender bender = target as FbxModelBender;
            var meshes = bender.GetMeshes();
            colliderMeshes.Clear();
            rendererMeshes.Clear();
            foreach (var mesh in meshes.Values)
            {
                if (mesh.isCollider) colliderMeshes.Add(mesh.go);
                else rendererMeshes.Add(mesh.go);
            }
            isExpandCollider = EditorGUILayout.Foldout(isExpandCollider, colliderMeshesUI);
            if (isExpandCollider)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(true);
                foreach (var go in colliderMeshes)
                {
                    EditorGUILayout.ObjectField(go, typeof(GameObject), false);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }
            isExpandRenderer = EditorGUILayout.Foldout(isExpandRenderer, rendererMeshesUI);
            if (isExpandRenderer)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(true);
                foreach (var go in rendererMeshes)
                {
                    EditorGUILayout.ObjectField(go, typeof(GameObject), false);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }
        }
    }
}
