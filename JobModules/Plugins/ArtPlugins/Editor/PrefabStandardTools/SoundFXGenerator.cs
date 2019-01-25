using Assets.Plugins.ArtPlugins.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoundFxGenerator
{
    private static string _srcPath = "Assets/Assets/Sound/ModeSound";
    private static string _dstPath = "Assets/Assets/Sound/AudioSources";

    [MenuItem("Assets/资源生成检查/根据Clip生成音效Prefab")]
    static void Generate()
    {
        var path = _srcPath;
        if (EditorUtility.DisplayDialog("Warning", string.Format("将要对文件夹{0}进行操作", path), "确定", "取消"))
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            var files = new List<string>();
            CommonEditorUtility.GetAllFilePath(CommonEditorUtility.GetFullPath(path), files);
            Debug.LogFormat("{0} asset in {1}", files.Count, path);
            foreach (var filePath in files)
            {
                var isAudioClip = filePath.EndsWith(".mp3") || filePath.EndsWith(".wav");
                if (isAudioClip)
                {
                    var relativePath = CommonEditorUtility.GetRelativePath(filePath);
                    var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(relativePath);
                    if (null != audioClip)
                    {
                        var dstAssetPath = _dstPath + "/" + audioClip.name + ".prefab";
                        var dst = AssetDatabase.LoadAssetAtPath<GameObject>(dstAssetPath);
                        if (null == dst)
                        {
                            var go = new GameObject(audioClip.name);
                            var audio = go.AddComponent<AudioSource>();
                            audio.clip = audioClip;
                            PrefabUtility.CreatePrefab(dstAssetPath, go);
                            GameObject.DestroyImmediate(go);
                        }
                    }
                    else
                    {
                        Debug.LogErrorFormat("asset in path {0} is not prefab !", relativePath);
                    }
                }
            }
        }
    }
}