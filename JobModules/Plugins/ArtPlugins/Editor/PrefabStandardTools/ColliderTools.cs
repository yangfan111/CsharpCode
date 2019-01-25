using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Assets.Plugins.ArtPlugins.Editor;

public class ColliderTools
{
    [MenuItem("Assets/GenerateRaycastCollider")]
    public static void GenerateRaycastCollider()
    {
        var prefabList = CommonEditorUtility.GetAllPrefabInSelectedFolder();
        foreach(var prefabInfo in prefabList)
        {
            try
            {
                if(prefabInfo.RelativePath.IndexOf("Prefabs/Weapons") > 0)
                {
                    GenerateWeaponCollider(prefabInfo.Go);
                }
                else
                {
                    GenerateAvatarCollider(prefabInfo.Go); 
                }
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.ToString() + "\n" + e.StackTrace);
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private const string NormalColliderName = "NormalCollider";
    private const string MeshColliderName = "MeshCollider";
    public static void GenerateWeaponCollider(GameObject go, bool force = true)
    {
        AssembleWeapon();
        //不是以WPN开头可能不是武器，或者命令错误
        if(!go.name.StartsWith("WPN"))
        {
            Debug.LogErrorFormat("weapon {0} doesn't start with WPN", go.name);
        }
        //一人称暂时不添加
        if(go.name.EndsWith("_P1"))
        {
            return;
        }
        GenerateCollider(go, force);
    }

    private static void GenerateAvatarCollider(GameObject go, bool force = true)
    {
        GenerateCollider(go, force);
    }

    private static void GenerateCollider(GameObject go, bool force)
    {
        //如果不是强制生成，之前生成过的跳过
        if(!force)
        {
            var trans = go.transform.Find(NormalColliderName);
            if(null != trans)
            {
                return;
            }
            trans = go.transform.Find(MeshColliderName);
            if(null != trans)
            {
                return;
            }
        }
        
        var tmp = Object.Instantiate(go);
        var baseChilds = new List<Transform>();
        //强制生成的话删除之前生成的collider
        if(force)
        {
            var needToByDestroy = new List<GameObject>();
            var childs = tmp.transform.GetComponentsInChildren<Transform>();
            foreach(var child in childs)
            {
                if(child.name == NormalColliderName || child.name == MeshColliderName)
                {
                    needToByDestroy.Add(child.gameObject);
                }
                else
                {
                    baseChilds.Add(child);
                }
            }
            foreach(var garbage in needToByDestroy)
            {
                Object.DestroyImmediate(garbage);
            }
        }
        Debug.LogFormat("generate collider for {0}", go.name);

        AttachBoxCollider(tmp);
        //AttachMeshCollider(tmp);

        PrefabUtility.ReplacePrefab(tmp, go);
        Object.DestroyImmediate(tmp);
    }

    private static void AttachBoxCollider(GameObject go)
    {
        //生成大的collider
        var normalColGo = new GameObject("NormalCollider");
        normalColGo.transform.parent = go.transform;
        normalColGo.transform.localPosition = Vector3.zero;
        normalColGo.transform.localScale = Vector3.one;
        normalColGo.transform.localRotation = Quaternion.identity;
        normalColGo.layer = LayerMask.NameToLayer("UserInputRaycast");
        var normalCol = normalColGo.AddComponent<BoxCollider>();
        normalCol.size = Vector3.one;
        normalCol.isTrigger = true;
        normalCol.enabled = false;
        var renderers = go.GetComponentsInChildren<Renderer>();
        if(renderers.Length < 1)
        {
            EditorUtility.DisplayDialog("error", string.Format("no rendere in prefab {0}", go.name), "get it");
            Debug.LogErrorFormat("no rendere in prefab {0}", go.name);
            return;
        }
        if(renderers.Length > 0)
        {
            Debug.LogWarningFormat("renderer in prefab {0} is more than one use the first to generate collider ", go.name);
        }
        var renderer = renderers[0];
        var bottom = renderer.bounds.min.y;
        normalCol.center = new Vector3(0, bottom + 0.5f, 0);
    }

    private static void AttachMeshCollider(GameObject go)
    {
        //生成贴合的collider
        var meshColGo = new GameObject("MeshCollider");
        meshColGo.transform.parent = go.transform;
        var meshCol = meshColGo.AddComponent<MeshCollider>();
        var mesh = CombineMesh(go);
        meshCol.sharedMesh = mesh;
        meshColGo.transform.localRotation = Quaternion.identity;
        meshColGo.transform.localScale = Vector3.one;
        meshColGo.transform.position = Vector3.zero;
        meshColGo.layer = LayerMask.NameToLayer("UserInputRaycast");
     
        meshCol.convex = true;
        meshCol.inflateMesh = true;
        meshCol.skinWidth = 0.0001f;
        meshCol.isTrigger = true;
    }

    private static Mesh CombineMesh(GameObject root)
    {
        MeshFilter[] meshFilters = root.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }
        var mesh = new Mesh();
        mesh.CombineMeshes(combine);
        mesh = SaveMesh(mesh, AssetDatabase.GetAssetPath(root));
        return mesh;
    }

    private static Mesh SaveMesh(Mesh mesh, string path)
    {
        Debug.LogFormat("save mesh to {0}", path);
        var realPath = path.Replace(".prefab", "_CombineMesh.asset");
        AssetDatabase.CreateAsset(mesh, realPath);
        var newMesh = AssetDatabase.LoadAssetAtPath<Mesh>(realPath);
        return newMesh;
    }

    /// <summary>
    /// 组装默认配件
    /// </summary>
    private static void AssembleWeapon()
    {
        //如果有默认配件的手动拖一下，然后生成
    }
}
