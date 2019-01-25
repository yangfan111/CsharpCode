using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using I2.Loc;
using UIComponent.UI;

public class PrefabTool : EditorWindow
{
    [MenuItem("Tools/UI/perfab删除空脚本")]
    static void DelNullScrip()
    {
        string[] ids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/ArtSubmit/Hall", "Assets/ArtSubmit/Ui" });
        for (int i = 0; i < ids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

            Transform [] allTransform = instance.GetComponentsInChildren<Transform>(true);
            bool needChange = false;
            for(int j = 0; j < allTransform.Length;j++)
            {
                SerializedObject so = new SerializedObject(allTransform[j].gameObject);
                var soProperties = so.FindProperty("m_Component");
                var components = allTransform[j].GetComponents<Component>();
                List<int> nullIndex = new List<int>();
                int propertyIndex = 0;
                foreach (var c in components)
                {
                    if (c == null)
                    {
                        Debug.Log(instance.name);
                        Debug.Log(allTransform[j].name);
                        nullIndex.Add(propertyIndex);
                        needChange = true;              
                    }
                    propertyIndex++;
                }
                nullIndex.Reverse();
                for(int index = 0; index < nullIndex.Count; index++)
                {
                    soProperties.DeleteArrayElementAtIndex(nullIndex[index]);
                }
                so.ApplyModifiedProperties();
            }

            if(needChange)
            {
                PrefabUtility.ReplacePrefab(instance, prefab, ReplacePrefabOptions.ConnectToPrefab);
            }

            DestroyImmediate(instance);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("清理完成!");
    }


    [MenuItem("Tools/UI/Text Font 整理")]
    static void CheckTextFont()
    {
        Font msyhUnicode = AssetDatabase.LoadAssetAtPath("Assets/ArtSubmit/Font/msyh.ttf", typeof(Font)) as Font;
        Font msyhbdUnicode = AssetDatabase.LoadAssetAtPath("Assets/ArtSubmit/Font/msyhbd.ttf", typeof(Font)) as Font;
        Font customblacknormalUnicode = AssetDatabase.LoadAssetAtPath("Assets/ArtSubmit/Font/customblacknormal.OTF", typeof(Font)) as Font;
        string[] ids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/ArtSubmit/Hall", "Assets/ArtSubmit/Ui" });
        for (int i = 0; i < ids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            bool needChange = false;

            Text[] allText = instance.GetComponentsInChildren<Text>(true);
            for (int j = 0; j < allText.Length; j++)
            {
                Text text = allText[j];
                if(text.font.name.Equals("msyh"))
                {
                    if (text.font.dynamic)
                    {
                        if(text.fontStyle == FontStyle.Bold || text.fontStyle == FontStyle.BoldAndItalic)
                        {
                            text.font = msyhbdUnicode;
                            Debug.Log(instance.name + "_" + text.name + "_更换为微软雅黑unicode");
                        }
                        else
                        {
                            text.font = msyhUnicode;
                            Debug.Log(instance.name + "_" + text.name + "_更换为微软雅黑粗体unicode");
                        }
                        needChange = true;
                    }                
                }

                if(text.font.name.Equals("zzgf"))
                {
                    text.font = customblacknormalUnicode;
                    Debug.Log(instance.name + "_" + text.name + "_更换为customblacknormal");
                    needChange = true;
                }

                if(text.fontStyle != FontStyle.Normal)
                {
                    text.fontStyle = FontStyle.Normal;
                    needChange = true;
                }
            }

            if (needChange)
            {
                PrefabUtility.ReplacePrefab(instance, prefab, ReplacePrefabOptions.ConnectToPrefab);
            }
            DestroyImmediate(instance);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("字体整理完成！");
    }


    [MenuItem("Tools/UI/Scale归一")]
    static void CheckScale()
    {
        float diff = 0.001f;
        string[] ids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/ArtSubmit/Hall" });
        for (int i = 0; i < ids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            bool needChange = false;

            Transform[] allTransform = instance.GetComponentsInChildren<Transform>(true);
            for (int j = 0; j < allTransform.Length; j++)
            {
                Transform trans = allTransform[j];
                if(trans.localScale != Vector3.one)
                {
                    if(Mathf.Abs(trans.localScale.x - 1) < diff && Mathf.Abs(trans.localScale.y - 1) < diff && Mathf.Abs(trans.localScale.z - 1) < diff)
                    {
                        trans.localScale = Vector3.one;
                        needChange = true;
                    }
                    else
                    {
                        Debug.Log("x:" + trans.localScale.x + "y:" + trans.localScale.y + "z:" + trans.localScale.z);
                        Debug.Log(instance.name + "_" + trans.name);
                    }
                }
            }

            if (needChange)
            {
                PrefabUtility.ReplacePrefab(instance, prefab, ReplacePrefabOptions.ConnectToPrefab);
            }
            DestroyImmediate(instance);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("缩放归一化完成！");
    }

    [MenuItem("Tools/UI/SetNavigation2None")]
    static void SetNavigation2None()
    {
        string[] ids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/ArtSubmit/Hall/Prefabs", "Assets/ArtSubmit/Ui"});
        for (int i = 0; i < ids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            bool needChange = false;

            Scrollbar[] allBar = instance.GetComponentsInChildren<Scrollbar>(true);
            for (int j = 0; j < allBar.Length; j++)
            {
                Scrollbar text = allBar[j];
                if (text.navigation.mode.Equals(Navigation.Mode.None) == false)
                {
                    var nav = text.navigation;
                    nav.mode = Navigation.Mode.None;
                    text.navigation = nav;
                    needChange = true;
                }
            }
            if (needChange)
            {
                PrefabUtility.ReplacePrefab(instance, prefab, ReplacePrefabOptions.ConnectToPrefab);
            }

            DestroyImmediate(instance);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Navigation设置完成！");
    }

    [MenuItem("Tools/UI/ReMountComponent")]
    static void ReMountComponent()
    {
        string[] ids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/ArtSubmit/Hall/Prefabs/Base" });
        for (int i = 0; i < ids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            Debug.Log(path);
            var splitPath = path.Split('/');
            var category = splitPath[splitPath.Length - 2].ToLower();
            GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            bool needChange = false;

            var allC = instance.GetComponentsInChildren<Outline>(true);
            if (allC.Length > 0)
            {
                var aaa = allC[0].gameObject.GetComponentsInChildren<MonoBehaviour>(true);
                Debug.Log(aaa.Length);
            }
            Localize[] allText = instance.GetComponentsInChildren<Localize>(true);
            for (int j = 0; j < allText.Length; j++)
            {
                Localize text = allText[j];
//                var localizes = text.gameObject.GetComponents<Localize>();
//                if (localizes.Length == 0)
//                {
//                    
//
//                }
            }
            if (needChange)
            {
                PrefabUtility.ReplacePrefab(instance, prefab, ReplacePrefabOptions.ConnectToPrefab);
            }

            DestroyImmediate(instance);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Localized完成！");
    }

    [MenuItem("Tools/UI/LocalizedInPrefabs")]
    static void LocalizeInPrefabs()
    {
        var index = 1;
        var i2GameObject = Resources.Load("I2Languages");
        LanguageSource languageSource = (i2GameObject as GameObject).GetComponent<LanguageSource>();

        var terms = languageSource.GetTermsList();
        foreach (var term in terms)
        {
            string result = System.Text.RegularExpressions.Regex.Replace(term, @"[^0-9]+", "");
            if (string.IsNullOrEmpty(result)) continue;
            var id = int.Parse(result);
            if (index < id)
            {
                index = id;
            }
        }
        index++;

        //        string[] ids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/ArtSubmit/Hall/Prefabs", "Assets/ArtSubmit/Ui" });
        string[] ids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/ArtSubmit/Hall/Prefabs/WeaponUnlock" });

        var newTermDic = new Dictionary<string, string>();

        for (int i = 0; i < ids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(ids[i]);
            Debug.Log(path);
            var splitPath = path.Split('/');
            var category = splitPath[splitPath.Length - 2].ToLower();
            GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            bool needChange = false;

            Text[] allText = instance.GetComponentsInChildren<Text>(true);
            for (int j = 0; j < allText.Length; j++)
            {
                Text text = allText[j];
                var localizes = text.gameObject.GetComponents<Localize>();
                if (localizes.Length == 0)
                {
                    needChange = true;
                    Debug.Log("font:" + text.font);
                    Debug.Log("text:" + text.text);
                    var localize = text.gameObject.AddComponent<Localize>();
                    string fontname = null;
                    if (text.font.name == "msyh")
                    {
                        fontname = "font";
                    }
                    else if (text.font.name == "customblacknormal")
                    {
                        fontname = "customfont";
                    }
                    else if (text.font.name == "msyhbd")
                    {
                        fontname = "boldfont";
                    }

                    var newtermname = string.Empty;
                    if (newTermDic.ContainsValue(text.text) == false)
                    {
                        index++;
                        newtermname = "hall/" + category + "/word" + index;
                        newTermDic[newtermname] = text.text;
                    }
                    else
                    {
                        foreach (var pair in newTermDic)
                        {
                            if (pair.Value.Equals(text.text)) newtermname = pair.Key;
                        }
                        
                    }


                    if (string.IsNullOrEmpty(fontname))
                    {
                        localize.SetTerm(newtermname);
                    }
                    else
                    {
                        localize.SetTerm(newtermname, fontname);
                    }
                    
                }
            }
            if (needChange)
            {
                PrefabUtility.ReplacePrefab(instance, prefab, ReplacePrefabOptions.ConnectToPrefab);
            }

            DestroyImmediate(instance);
        }

        foreach (var pair in newTermDic)
        {
            Debug.Log(pair.Key + ":" + pair.Value);
            var term = languageSource.AddTerm(pair.Key, eTermType.Text, false);
            term.Languages[0] = pair.Value;
        }

        languageSource.UpdateDictionary();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Localized完成！");
    }


    //[MenuItem("Tools/UI/TTTTEEEEE")]
    //static void TTTTEEEEE()
    //{
    //    string[] ids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/ArtSubmit/Hall/Prefabs", "Assets/ArtSubmit/Ui" });
    //    for (int i = 0; i < ids.Length; i++)
    //    {
    //        string path = AssetDatabase.GUIDToAssetPath(ids[i]);
    //        GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
    //        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
    //        bool needChange = false;

    //        UITabBehaviour[] allBar = instance.GetComponentsInChildren<UITabBehaviour>(true);
    //        for (int j = 0; j < allBar.Length; j++)
    //        {
    //            UITabBehaviour text = allBar[j];
    //            if (text.activeFontSize > 0)
    //            {
    //                text.activeFontSizeList.Add(text.activeFontSize);
    //                needChange = true;
    //            }
    //        }
    //        if (needChange)
    //        {
    //            PrefabUtility.ReplacePrefab(instance, prefab, ReplacePrefabOptions.ConnectToPrefab);
    //        }

    //        DestroyImmediate(instance);
    //    }
    //    AssetDatabase.SaveAssets();
    //    AssetDatabase.Refresh();
    //    Debug.Log("UITabBehaviour！");
    //}

}