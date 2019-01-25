﻿using System;
using System.IO;
using Shared.Scripts.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class StreamingLevelBuilder
{
    private const string DataFileName = "streaminglevel.xml";
    private const string DataPath = "Assets/Assets/templatefrombuilding/" + DataFileName;
    private static readonly string DataFullPath = Application.dataPath + "/Assets/templatefrombuilding/" + DataFileName;
    private static StreamingData _existData;

    public static void BuildFromScracth(List<string> scenePaths)
    {
        _existData = new StreamingData();
        DeleteExistFile();

        BuildSceneByScene(scenePaths);
    }

    public static void Build(List<string> scenePaths)
    {
        ReadExistData();
        DeleteExistFile();

        BuildSceneByScene(scenePaths);
    }

    private static void BuildSceneByScene(List<string> scenePaths)
    {
        foreach (var scenePath in scenePaths)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            TraverseScene(scene);
        }

        StreamingSerialization.Serialize(_existData, DataFullPath);
        AssetDatabase.ImportAsset(DataPath);
        AssetImporter importer = AssetImporter.GetAtPath(DataPath);
        importer.assetBundleName = "tablesfrombuilding";
    }

    private static void TraverseScene(Scene scene)
    {
        HierarchyRecorder recorder = new HierarchyRecorder();
        recorder.StreamingScene.SceneName = scene.name;

        var rootGos = scene.GetRootGameObjects();
        foreach (var rootGo in rootGos)
        {
            var countBefore = recorder.StreamingScene.Objects.Count;
            Traverse(rootGo, recorder);
            var countAfter = recorder.StreamingScene.Objects.Count;

            if (countBefore != countAfter)
                Object.DestroyImmediate(rootGo);
        }

        EditorSceneManager.SaveScene(scene);

        for (int i = 0; i < _existData.Scenes.Count; i++)
        {
            var streamingScene = _existData.Scenes[i];
            if (streamingScene.SceneName == scene.name)
            {
                _existData.Scenes[i] = recorder.StreamingScene;
                return;
            }
        }

        _existData.Scenes.Add(recorder.StreamingScene);
    }

    private static void Traverse(GameObject go, HierarchyRecorder recorder)
    {
        var flag = go.GetComponent<EmbedPrefab>();

        if (flag != null)
            ParseEmbedPrefab(go, flag, recorder);
        else
        {
            var count = go.transform.childCount;
            for (int i = 0; i < count; i++)
                Traverse(go.transform.GetChild(i).gameObject, recorder);
        }
    }

    private static void ParseEmbedPrefab(GameObject go, EmbedPrefab comp, HierarchyRecorder recorder)
    {
        var assetPath = AssetDatabase.GUIDToAssetPath(comp.PrefabInAssets);
        if (string.IsNullOrEmpty(assetPath))
            return;

        var bundleName = AssetDatabase.GetImplicitAssetBundleName(assetPath);
        if (string.IsNullOrEmpty(bundleName))
            return;

        var postfixIndex = assetPath.LastIndexOf('.');
        var nameStartIndex = assetPath.LastIndexOf('/') + 1;
        var assetName = assetPath.Substring(nameStartIndex, postfixIndex - nameStartIndex);

        var count = go.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            var renderer = go.transform.GetChild(i).GetComponent<MeshRenderer>();
            if (renderer)
            {
                recorder.Record(bundleName, assetName, StreamingObjectCategory.Default, go.transform);
                return;
            }
        }

        recorder.Record(bundleName, assetName, StreamingObjectCategory.Default, go.transform);
    }

    private static void ReadExistData()
    {
        _existData = StreamingSerialization.Deserialize<StreamingData>(DataFullPath) ?? new StreamingData();
    }

    private static void DeleteExistFile()
    {
        if (File.Exists(DataFullPath))
            File.Delete(DataFullPath);
    }

    public static List<string> DefaultSceneList()
    {
        List<string> ret = new List<string>();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var sceneName = string.Format("002 {0}x{1}", i, j);
                var availableScenes = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName("dynamicscene", sceneName);
                if (availableScenes != null && availableScenes.Length > 0)
                    ret.Add(availableScenes[0]);
            }
        }

        return ret;
    }
}
