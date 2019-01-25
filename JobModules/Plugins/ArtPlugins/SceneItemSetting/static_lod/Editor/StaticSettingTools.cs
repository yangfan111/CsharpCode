using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class StaticSettingTools : Editor
{
    [MenuItem("场景物件/静态与lod/关闭批次和不需要的静态合并")]
    private static void disableSomeStatic()
    {
        foreach (var item in FindObjectsOfType<GameObject>())
        {
            var flag = GameObjectUtility.GetStaticEditorFlags(item);
            flag &= ~StaticEditorFlags.BatchingStatic;
            flag &= ~StaticEditorFlags.NavigationStatic;
            flag &= ~StaticEditorFlags.OccludeeStatic;
            flag &= ~StaticEditorFlags.OccluderStatic;
            flag &= ~StaticEditorFlags.OffMeshLinkGeneration;

            GameObjectUtility.SetStaticEditorFlags(item, flag);
            //EditorUtility.SetDirty(item);
            //EditorApplication.MarkSceneDirty(item.scene);

            //   GameObjectUtility.SetStaticEditorFlags(item,);
        }
        EditorSceneManager.MarkAllScenesDirty();
    }

    [MenuItem("场景物件/静态与lod/只烘焙房子的静态设置")]
    private static void onlyHouseLightMap()
    {
        foreach (var item in FindObjectsOfType<GameObject>())
        {
            StaticEditorFlags flag = 0;
            if (item.name.IndexOf("L001_Bud") != -1)
            {
                flag = StaticEditorFlags.LightmapStatic | StaticEditorFlags.ReflectionProbeStatic;
            }
            GameObjectUtility.SetStaticEditorFlags(item, flag);
        }
        EditorSceneManager.MarkAllScenesDirty();
    }

    [MenuItem("场景物件/静态与lod/删除lodgroup（运行时才用）")]
    private static void delLodGroups()
    {
        var lodgroups = GameObject.FindObjectsOfType<LODGroup>();
        Debug.LogError("lods=" + lodgroups.Length);
        foreach (var item in lodgroups)
        {
            Destroy(item.gameObject);
        }
    }

    [MenuItem("场景物件/静态与lod/统计lodgroup")]
    private static void getLodGroups()
    {
        var lodgroups = GameObject.FindObjectsOfType<LODGroup>();
        Debug.LogError("lods=" + lodgroups.Length);
        foreach (var item in lodgroups)
        {
            Destroy(item.gameObject);
            // item.enabled=false;
            //for (int i = 0; i < item.GetLODs().Length; i++)
            //{
            //    bool show = i == 2;
            //    foreach (var r in item.GetLODs()[i].renderers)
            //    {
            //        if (r != null)
            //        r.gameObject.SetActive(show);
            //    }
            //}
        }
    }

    [MenuItem("场景物件/静态与lod/灯光探头静态设置")]
    private static void SetLightProbeGroupStaticFlags()
    {
        var objs = FindObjectsOfType<LightProbeGroup>();
        foreach (var obj in objs)
        {
            if (obj == null || obj.gameObject == null) continue;

            GameObject go = obj.gameObject;
            var flags = GameObjectUtility.GetStaticEditorFlags(go);
            flags |= StaticEditorFlags.LightmapStatic;
            GameObjectUtility.SetStaticEditorFlags(go, flags);
        }
        EditorSceneManager.MarkAllScenesDirty();
    }
}
