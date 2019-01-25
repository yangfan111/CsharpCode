using Assets.Plugins.ArtPlugins.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine; 

public class WeaponAttachmentSocketChecker : AssetPostprocessor
{
    

    void OnPreprocessModel()
    {
        ModelImporter importer = (ModelImporter)assetImporter;
    }

    [MenuItem("Assets/资源生成检查/检查武器挂点")]
    private static void CheckAttachmentSocket()
    {
        List<string> attachmentSockets = new List<string>
        {
            "RightHandLocator",             //手部对位点
            "EjectionSocket",               //抛壳点
            "SightsLocator",                //P1机瞄点
            "UpperAttachmentSocket",        //上挂点
            "MuzzleSocket",                 //枪口特效点
            "MuzzleAttachmentSocket",       //枪口挂点
            "MagazineAttachmentSocket",     //弹夹挂点
            "UnderAttachmentSocket",        //下挂点
        };
        var sb = new System.Text.StringBuilder();
        var dic = new Dictionary<string, bool>();
        var prefabs = CommonEditorUtility.GetAllPrefabInSelectedFolder();
        foreach(var prefab in prefabs)
        {
            //不检查近战和投掷武器
            if(prefab.RelativePath.IndexOf("/Grenade/") > 0 || prefab.RelativePath.IndexOf("/Melee/") > 0)
            {
                continue;
            }
            for (int i = 0; i < attachmentSockets.Count; i++)
            {
                dic[attachmentSockets[i]] = false;
            }

            //P3 没有机瞄点
            if(prefab.RelativePath.IndexOf("_P3") > 0)
            {
                dic["SightsLocator"] = true;
            }
            var childs = prefab.Go.GetComponentsInChildren<Transform>();
            foreach(var child in childs)
            {
                if(dic.ContainsKey(child.name))
                {
                    dic[child.name] = true;
                }
            }
            foreach(var pair in dic)
            {
                if(!pair.Value)
                {
                    sb.AppendLine(string.Format("{0} 中没有 {1} ", prefab.Go.name, pair.Key));
                }
            }
        }
        if(sb.Length > 0)
        {
            EditorUtility.DisplayDialog("Error", "有不合法的资源, 请查看console里的log信息", "ok" );
        }
        Debug.Log(sb.ToString());
    }
}
