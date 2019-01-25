using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakeShareSceneItem : MonoBehaviour {

    [Header("该物件要分配给哪些场景做烘焙协助 多个用 英文逗号 , 隔开")]
    public string  forSceneNames;
#if UNITY_EDITOR
    [UnityEditor.MenuItem("场景物件/烘焙/大场景烘焙选择场景")]
    private static void bake()
    {
  
        UnityEditor.Lightmapping.completed = onCmp ;
        var addScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
      //  CopyLightingSettings.CopySettings();
 


  
        //change active scene
        var activeScene = UnityEditor.Selection.activeGameObject.scene;// UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

         UnityEditor.SceneManagement.EditorSceneManager.SetActiveScene(activeScene);
       // CopyLightingSettings.PasteSettings();
 
         

        GameObject bakeTemp = GameObject.Find("BakeTemp");
        if (bakeTemp != null) GameObject.DestroyImmediate(bakeTemp);
        bakeTemp = new GameObject("BakeTemp");
        
      var items=  GameObject.FindObjectsOfType<BakeShareSceneItem>();
         
        foreach (var item in items)
        {
            if (item.gameObject.scene == activeScene) continue;
            if ((","+item.forSceneNames+",").IndexOf(activeScene.name) > -1) {
                print(item);
                //  item.transform.SetParent(null, true);
                // UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(item.gameObject, activeScene);
                var newItem = Instantiate(item.gameObject);
                newItem.transform.SetParent(bakeTemp.transform,true);
                newItem.transform.position = item.transform.position;
                newItem.transform.rotation = item.transform.rotation;
                //newItem.transform.lossyScale = item.transform.lossyScale;
            }
        }
        foreach (var scene in UnityEditor.SceneManagement.EditorSceneManager.GetAllScenes())
        {
            if (scene == activeScene) continue;
          //  UnityEditor.SceneManagement.EditorSceneManager.UnloadScene(scene);
            UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, false);

        }
       UnityEditor.Lightmapping.BakeAsync();

        

    }

    private static void onCmp()
    {
        GameObject bakeTemp = GameObject.Find("BakeTemp");
       if (bakeTemp != null) GameObject.DestroyImmediate(bakeTemp);
    }
#endif
}
