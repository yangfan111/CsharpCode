using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArtPlugins;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BakeOc
{
    public static bool LoadScenes(IList<string> bundles, IList<string> scenes)
    {
        if (bundles.Count < 2 || scenes.Count < 2 || bundles.Count != scenes.Count)
        {
            Debug.LogErrorFormat("Invalid OC scene parameter, bundle count: {0}, scene count: {1}", bundles.Count, scenes.Count);
            return false;
        }

        List<string> scenePath = new List<string>();

        for (int i = 0; i < bundles.Count; i++)
        {
            var availableScenes = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundles[i], scenes[i]);
            if (availableScenes != null && availableScenes.Length > 0)
                scenePath.Add(availableScenes[0]);
            else
                return false;
        }

        EditorSceneManager.OpenScene(scenePath[0], OpenSceneMode.Single);

        EditorSceneManager.sceneOpened += OnSceneOpened;

        for (int i = 1; i < scenePath.Count; i++)
            EditorSceneManager.OpenScene(scenePath[i], OpenSceneMode.Additive);

        EditorSceneManager.sceneOpened -= OnSceneOpened;

        return true;
    }

    public static void Bake()
    {
        Debug.LogFormat("Begin BakeOC: {0}", DateTime.Now);

        StaticOcclusionCulling.Clear();
        SetOcBackeParam();
        StaticOcclusionCulling.Compute();

        EditorSceneManager.SaveOpenScenes();

        var activeScene = SceneManager.GetActiveScene();
        List<Scene> loadedScenes = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (activeScene.path != scene.path)
                loadedScenes.Add(SceneManager.GetSceneAt(i));
        }

        foreach (var scene in loadedScenes)
        {
            EditorSceneManager.CloseScene(scene, true);
        }

        Debug.LogFormat("End BakeOC: {0}", DateTime.Now);
    }

    private static void SetOcBackeParam()
    {
        StaticOcclusionCulling.smallestOccluder = 5.0f;
        StaticOcclusionCulling.smallestHole = 0.1f;
        StaticOcclusionCulling.backfaceThreshold = 100f;
    }

    private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        foreach (var go in scene.GetRootGameObjects())
        {
            SetOcclusionFlag(go);
        }
    }

    private static void SetOcclusionFlag(GameObject go)
    {
        var tag = go.GetComponent<MultiTagBase>();
        if (tag != null)
        {
            if (tag.IsDoor())
            {
                foreach (var render in go.GetComponentsInChildren<MeshRenderer>())
                {
                    var flag = GameObjectUtility.GetStaticEditorFlags(render.gameObject);
                    GameObjectUtility.SetStaticEditorFlags(render.gameObject,
                        flag & ~StaticEditorFlags.OccluderStatic & ~StaticEditorFlags.OccludeeStatic);
                }

                return;
            }
        }

        var renderer = go.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            var flag = GameObjectUtility.GetStaticEditorFlags(go);
            GameObjectUtility.SetStaticEditorFlags(go,
                flag | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic);
        }


        for (int i = 0; i < go.transform.childCount; i++)
            SetOcclusionFlag(go.transform.GetChild(i).gameObject);
    }
}
