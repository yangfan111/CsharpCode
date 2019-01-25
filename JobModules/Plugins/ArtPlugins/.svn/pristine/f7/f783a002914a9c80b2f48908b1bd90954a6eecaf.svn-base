using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ArtPlugins.MapConfig;
using NUnit.Framework.Internal;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class EmbedPrefabUpdateEditor : EditorWindow
{
    [MenuItem("Window/EmbedPrefab")]
    static void init()
    {
        GetWindow<EmbedPrefabUpdateEditor>();
    }
    private GameObject asset;
    private void OnGUI()
    {
        if (GUILayout.Button("添加嵌套脚本"))
        {
            foreach (var item in Selection.gameObjects)
            {
                var ep = item.GetComponent<EmbedPrefab>();
                if (ep == null) { ep = item.AddComponent<EmbedPrefab>(); }
                ep.PrefabInAssets = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(item));
            }
        }
        asset = EditorGUILayout.ObjectField("恢复指定预设连接", asset, typeof(GameObject)) as GameObject;
        if (GUILayout.Button("执行"))
        {
            foreach (var item in Selection.gameObjects)
            {
                ReconnectPrefab(item, asset);

            }
            asset = null;
        }
        if (GUILayout.Button("恢复嵌套对象连接"))
        {
            foreach (var item in Selection.gameObjects)
            {
                var house = item.GetComponent<EmbedPrefab>();
                if (house == null || string.IsNullOrEmpty(house.PrefabInAssets))
                {
  
                    continue;
                }
                var _asset = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(house.PrefabInAssets));

                ReconnectPrefab(item, _asset);
            }
        }
        if (GUILayout.Button("根据名字替换")) {
            foreach (var item in Selection.gameObjects)
            {
                var pfname = item.name;
                if (pfname.IndexOf(" (") > -1)
                {
                    pfname = pfname.Substring(0, pfname.IndexOf(" ("));
                }
           
                var items = AssetDatabase.FindAssets(pfname+" t:prefab", new string[] { "Assets/Maps/Prefabs" } );
                foreach (var _asset in items)
                {
                    var go = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(_asset));
                  //  Debug.LogError(AssetDatabase.GUIDToAssetPath(_asset));
                    if (go!=null&&go.name==pfname)
                    {

                         ReconnectPrefab(item, go);
                        break;
                    }
                       
                }
              

            }
        }
        //if (GUILayout.Button("恢复嵌套对象连接(强制 更慢)"))
        //{
        //    foreach (var item in Selection.gameObjects)
        //    {
        //        var house = item.GetComponent<EmbedPrefab>();
        //        if (house == null || string.IsNullOrEmpty(house.PrefabInAssets))
        //        {

            //            continue;
            //        }
            //        var _asset = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(house.PrefabInAssets));

            //        ReconnectPrefab(item, _asset,true);
            //    }
            //}





    }

    public void ReconnectPrefab(GameObject gameObject, GameObject _asset,bool forced=false)

    {
        var select = gameObject;
        if (_asset == null) return;


        var ab = PrefabUtility.GetPrefabParent(select);

     //   if (ab != null)

        //    return;



        var tarprefab = _asset;



        var gname = select.name;

        var enable = select.activeInHierarchy;

        var pos = select.transform.localPosition;

        var rot = select.transform.localRotation;

        var scale = select.transform.localScale;
        EditorSceneManager.MarkSceneDirty(select.scene);
        GameObject go = null;
        if (forced == false)
        {
            PrefabUtility.DisconnectPrefabInstance(select);

            go = PrefabUtility.ConnectGameObjectToPrefab(select, tarprefab);
        }
        else {
            go = PrefabUtility.InstantiatePrefab(tarprefab) as GameObject;
            go.transform.SetParent(select.transform.parent);
            go.transform.SetSiblingIndex(select.transform.GetSiblingIndex());
           // DestroyImmediate(select);
        }
        go.transform.localPosition = pos;

        go.transform.localRotation = rot;

        go.transform.localScale = scale;

        go.name = gname;

        go.SetActive(enable);





    }


}
