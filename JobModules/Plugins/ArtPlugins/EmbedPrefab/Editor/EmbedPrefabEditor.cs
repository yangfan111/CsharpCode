﻿using ArtPlugins;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
//using Assets.Editor.packageUtils;

[CustomEditor(typeof(EmbedPrefab))]
[CanEditMultipleObjects]
public class EmbedPrefabEditor : Editor
{
    // [MenuItem("CONTEXT/EmbedPrefab/ReplaceMultipleEmbed")]
    // private static void ReplaceMultipleEmbed(MenuCommand command)
    // {
    //     if (command.context == null)
    //     {
    //         Debug.LogError("ReplaceMultipleEmbed error, command.context is null");
    //         return;
    //     }

    //     EmbedPrefab ep = command.context as EmbedPrefab;
    //     if (ep == null)
    //     {
    //         Debug.LogError("ReplaceMultipleEmbed error, ep is null");
    //         return;
    //     }
    // }

    private EmbedPrefab instance = null;
    private GameObject tempAsset;
    private static DefaultAsset updateFolder;

    public static void SetUpdateFolder(DefaultAsset folder)
    {
        updateFolder = folder;
    }

    public void SetInstance(EmbedPrefab ep)
    {
        instance = ep;
    }

    private void updateTempAsset()
    {
        if (tempAsset == null && !string.IsNullOrEmpty(instance.PrefabInAssets))
        {
            string path = AssetDatabase.GUIDToAssetPath(instance.PrefabInAssets);
            tempAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
    }

    public override void OnInspectorGUI()
    {
        instance = target as EmbedPrefab;

        if (GUILayout.Button("在 Assets 中查看"))
        {
            updateTempAsset();
            Selection.activeObject = tempAsset;
        }

        if (string.IsNullOrEmpty(instance.PrefabInAssets))
        {
            GUILayout.Label("Error:无效来源");
            return;
        }

        if (GUILayout.Button("更新所有使用我的预制"))
        {
            updateAllUseThis(instance);
        }

        // if(GUILayout.Button("跟新"))
        if (Selection.gameObjects.Length > 1)
        {
            bool validate = true;
            List<EmbedPrefab> eps = new List<EmbedPrefab>();
            foreach (var go in Selection.gameObjects)
            {
                if (go == null)
                {
                    validate = false;
                    break;
                }

                EmbedPrefab ep = go.GetComponent<EmbedPrefab>();
                if (ep == null)
                {
                    validate = false;
                    break;
                }

                eps.Add(ep);
            }
            if (validate)
            {
                if (GUILayout.Button("更新所有使用我们的预制"))
                {
                    foreach (var ep in eps) updateAllUseThis(ep);
                }
            }
        }

        updateFolder = EditorGUILayout.ObjectField("更新目录", updateFolder, typeof(DefaultAsset), false) as DefaultAsset;
    }

    private static void dealPrefab(GameObject prefab, EmbedPrefab checkAsset)
    {
        foreach (var item in prefab.GetComponentsInChildren<EmbedPrefab>())
        {
            if (item.PrefabInAssets != checkAsset.PrefabInAssets) continue;

            GameObject newGo = GameObject.Instantiate(checkAsset.gameObject);

            //canvas对transfom的值有影响
            bool hasCanvas = false;
            if(newGo.transform.GetComponent<Canvas>() != null)
            {
                GameObject.DestroyImmediate(newGo.transform.GetComponent<Canvas>());
                hasCanvas = true;
            }

            newGo.transform.parent = item.gameObject.transform.parent;
            newGo.transform.name = "tempGo";

            newGo.transform.localPosition = item.gameObject.transform.localPosition;
            newGo.transform.localScale = item.gameObject.transform.localScale;
            newGo.transform.localRotation = item.gameObject.transform.localRotation;

            //2d保留
            if(item.gameObject.transform is RectTransform)
            {
                RectTransform newRt = newGo.transform as RectTransform;
                RectTransform oldRt = item.gameObject.transform as RectTransform;
                newRt.sizeDelta = oldRt.sizeDelta;
                newRt.offsetMax = oldRt.offsetMax;
                newRt.offsetMin= oldRt.offsetMin;
                newRt.anchorMax = oldRt.anchorMax;
                newRt.anchorMin = oldRt.anchorMin;
                newRt.pivot = oldRt.pivot;
            }

            string name = item.name;
            int sbIndex = item.transform.GetSiblingIndex();
            newGo.transform.SetSiblingIndex(sbIndex);

            // 检测MultiTag组件，保持原标签设置不变
            MultiTag oldMt = item.GetComponent<MultiTag>();
            if (oldMt != null)
            {
                MultiTag newMt = newGo.AddComponent<MultiTag>();
                MultiTag.CopyValue(newMt, oldMt);
            }

            GameObject.DestroyImmediate(item.gameObject);
            newGo.name = name;
            if(hasCanvas)
            {
                newGo.AddComponent<Canvas>();
            }
        }
    }

    public static void updateAllUseThis(EmbedPrefab instance)
    {
        if (updateFolder == null)
        {
            EditorUtility.DisplayDialog("error", "需要指定更新目录", "ok");
            return;
        }

        if (instance == null)
        {
            Debug.LogError("updateAllUseThis error, instance is null");
            return;
        }

        var items = AssetDatabase.FindAssets(" t:prefab", new string[] { AssetDatabase.GetAssetPath(updateFolder) });
        foreach (var item in items)
        {
            //self asset 
            if (item == instance.PrefabInAssets)
            {
                continue;
            }

            string subPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            string assetPath = AssetDatabase.GUIDToAssetPath(item);
            string text = File.ReadAllText(subPath + assetPath);
            bool needupdate = text.IndexOf(instance.PrefabInAssets) > -1;

            if (!needupdate) continue;

            Debug.Log("updated:" + assetPath);
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            var goInstance = PrefabUtility.InstantiatePrefab(go) as GameObject;
            dealPrefab(goInstance, instance);
            PrefabUtility.ReplacePrefab(goInstance, go);
            GameObject.DestroyImmediate(goInstance);
        }
    }

    [MenuItem("Tools/验证美术资源/嵌套预设有效性检测")]
    private static void ValidateEPs()
    {
        List<PathNode> paths = new List<PathNode>();
        GameObject[] goes = Selection.gameObjects;
        for (int i = 0; i < goes.Length; i++)
        {
            GameObject go = goes[i];
            if (go == null) continue;
            ValidateEmbedPrefab(go, paths);
        }

        foreach (PathNode path in paths)
        {
            Debug.LogError("invalid EmbedPrefab path:" + path.Type + ":" + path.Path);
        }
    }

    /// <summary>
    /// 检测整个工程中的invalid EmbedPrefab
    /// </summary>
    private static void ValidateProject()
    {
        string[] dirs = new string[] { "Assets" };
        List<PathNode> paths = new List<PathNode>();

        // check all prefab
        string[] guids = AssetDatabase.FindAssets("t:Prefab", dirs);
        foreach (string guid in guids)
        {
            if (!string.IsNullOrEmpty(guid)) continue;
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            ValidateEmbedPrefab(prefab, paths);
        }

        // check all scenes
        guids = AssetDatabase.FindAssets("t:Scene", dirs);
        foreach (string guid in guids)
        {
            if (!string.IsNullOrEmpty(guid)) continue;
            string path = AssetDatabase.GUIDToAssetPath(guid);
            SceneAsset asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            if (asset == null) continue;

            //unity
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            GameObject[] roots = scene.GetRootGameObjects();
            foreach (GameObject root in roots)
            {
                if (root != null)
                {
                    ValidateEmbedPrefab(root, paths);
                }
            }
            EditorSceneManager.CloseScene(scene, true);
            Resources.UnloadUnusedAssets();
        }

        // print check result
        foreach (PathNode path in paths)
        {
            Debug.LogError("invalid EmbedPrefab path:" + path.Type + ":" + path.Path);
        }
    }

    private static void ValidateEmbedPrefab(GameObject go, List<PathNode> paths)
    {
        if (go == null)
        {
            Debug.LogError("ValidateEmbedPrefab error, go is null");
            return;
        }

        EmbedPrefab[] eps = go.GetComponentsInChildren<EmbedPrefab>(true);
        if (eps.Length <= 0) return;

        for (int i = 0; i < eps.Length; i++)
        {
            EmbedPrefab ep = eps[i];
            if (ep == null) continue;

            bool valid = true;
            if (string.IsNullOrEmpty(ep.PrefabInAssets))
            {
                valid = false;
            }
            else
            {
                string path = AssetDatabase.GUIDToAssetPath(ep.PrefabInAssets);
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    valid = false;
                }
            }
            if (!valid)
            {
                // string path = GetFullPath(ep.gameObject);
                paths.Add(new PathNode(ep.gameObject));
            }
        }
    }

    private static string GetFullPath(GameObject go)
    {
        if (go == null) return string.Empty;

        string scenePath = string.Empty;
        if (!AssetDatabase.IsMainAsset(go) && !AssetDatabase.IsSubAsset(go))
        {
            scenePath = go.scene.name;
        }
        else
        {
            scenePath = AssetDatabase.GetAssetPath(go);
            scenePath = Path.GetDirectoryName(scenePath);
        }

        string path = go.name;
        Transform parent = go.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }

        if (!string.IsNullOrEmpty(scenePath))
            path = scenePath + ":" + path;

        return path;
    }
}
