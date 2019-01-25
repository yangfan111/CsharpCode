using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEditor;
using UnityEngine;
 [CustomEditor(typeof(TerrainTreeAsset))]
public class TerrainTreeAssetEditor : Editor {

    [MenuItem("地形/启动跨地形刷植被")]
    static void Init()
    {
        if (Selection.activeGameObject == null) {
            EditorUtility.DisplayDialog("提示", "要选择所有地形根节点", "确定");
            return;
        }
        Terrain[] terrains= Selection.activeGameObject.GetComponentsInChildren<Terrain>();
        if (terrains == null || terrains.Length < 4) {
            EditorUtility.DisplayDialog("提示", "地形少于4块 应该有问题", "确定");
            return;
        }
        foreach (var item in terrains)
        {
            if (item.GetComponent<TerrainTreeAsset>()==null) {
                item.gameObject.AddComponent<TerrainTreeAsset>();
            }
            item.gameObject.layer = 24;
            
        }
        EditorUtility.DisplayDialog("提示", "跨地形刷植被成功！！", "确定");
    }

    private static float lastTime=0;
   //  private static Terrain lastTerrain = null;
     private static Transform lastTerrain = null;
     private static bool myBool=true;
     private static TerrainData _terrainDataAsset;
     private int lay = -1;
     void OnSceneGUI()
     {
         if (myBool == false) return;
         if (_terrainDataAsset == null) return;
        // if (System.DateTime.Now.Second == lastTime) return;
         lastTime = System.DateTime.Now.Second;
         // Debug.Log("xx"); 
         Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
         RaycastHit hit;
         if(lay==-1)lay=1 <<24;
         if (Physics.Raycast(ray, out hit, 100.0f, lay))
         {
          
             if (lastTerrain != hit.transform)
             {
                 lastTerrain = hit.transform;
                 Selection.activeGameObject = lastTerrain.gameObject;
                 
                 Terrain terrain=  lastTerrain.GetComponent<Terrain>();
                 terrain.terrainData.treePrototypes = _terrainDataAsset.treePrototypes;
                 terrain.terrainData.detailPrototypes = _terrainDataAsset.detailPrototypes;
                 terrain.terrainData.wavingGrassAmount = _terrainDataAsset.wavingGrassAmount;
                 terrain.terrainData.wavingGrassSpeed = _terrainDataAsset.wavingGrassSpeed;
                 terrain.terrainData.wavingGrassStrength = _terrainDataAsset.wavingGrassStrength;
                 terrain.terrainData.wavingGrassTint = _terrainDataAsset.wavingGrassTint;
                // terrain.terrainData.SetDetailResolution(terrainData.detailResolution/2, 8);
                
             }

//             Terrain ct = hit.transform.GetComponent<Terrain>();
//             if (ct != null&& lastTerrain!=ct)
//             {
//                 Selection.activeGameObject = ct.gameObject;
//             }
         }
     }

     public override void OnInspectorGUI()   
     {  
        
          myBool = EditorGUILayout.Toggle("是否启用", myBool);  
         _terrainDataAsset=EditorGUILayout.ObjectField("植被模版 TerrainData", _terrainDataAsset,typeof(TerrainData)) as TerrainData;  
     }  
          
         
         /*
                  Debug.Log("ff"); 
        a=EditorGUILayout.IntField("ff", a);
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        foreach (var item in Physics.RaycastAll(ray,1000))
        {
            Debug.Log(item.transform.name); 
        }
          
          */
    

 
}
 