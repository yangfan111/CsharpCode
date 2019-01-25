using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace ArtPlugins
{
    public class TreeInstanceEditor 
    {

        [MenuItem("地形/可编辑树/将树转为可编辑/0")]
        static private void toInstance0()
        {
            toInstance(0);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/1")]
        static private void toInstance1() {
            toInstance(1);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/2")]
        static private void toInstance2() {
            toInstance(2);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/3")]
        static private void toInstance3()
        {
            toInstance(3);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/4")]
        static private void toInstance4()
        {
            toInstance(4);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/5")]
        static private void toInstance5()
        {
            toInstance(5);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/6")]
        static private void toInstance6()
        {
            toInstance(6);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/7")]
        static private void toInstance7()
        {
            toInstance(7);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/8")]
        static private void toInstance8()
        {
            toInstance(8);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/9")]
        static private void toInstance9()
        {
            toInstance(9);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/10")]
        static private void toInstance10()
        {
            toInstance(10);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/11")]
        static private void toInstance11()
        {
            toInstance(11);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/12")]
        static private void toInstance12()
        {
            toInstance(12);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/13")]
        static private void toInstance13()
        {
            toInstance(13);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/14")]
        static private void toInstance14()
        {
            toInstance(14);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/15")]
        static private void toInstance15()
        {
            toInstance(15);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/16")]
        static private void toInstance16()
        {
            toInstance(16);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/17")]
        static private void toInstance17()
        {
            toInstance(17);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/18")]
        static private void toInstance18()
        {
            toInstance(18);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/19")]
        static private void toInstance19()
        {
            toInstance(19);
        }
        [MenuItem("地形/可编辑树/将树转为可编辑/2")]
        static private void toInstance20()
        {
            toInstance(20);
        }

        static private void toInstance(int treeIndex) {
            //    Debug.Log(GameObject.Find("Prop_rubbish008/Prop_rubbish008_LOD0").SetActive(false));
            Terrain terrain = null;
            if(Selection.activeGameObject)terrain= Selection.activeGameObject.GetComponent<Terrain>();
            if (terrain == null) {

                var ts = GameObject.FindObjectsOfType<Terrain>();
                if (ts.Length == 1)
                {
                    terrain = ts[0];
                }
                else {
                    EditorUtility.DisplayDialog("error", "需要选中场景上一个地型", "ok");
                    return;
                }
            }
         var edit_tree=   terrain.transform.Find("edit_tree");

            if (edit_tree != null) GameObject.DestroyImmediate(edit_tree.gameObject);
            edit_tree = new GameObject("edit_tree").transform;
            edit_tree.gameObject.tag = "EditorOnly";
            edit_tree.SetParent(terrain.transform);
            edit_tree.localPosition = Vector3.zero;
            for (int i = 0; i < terrain.terrainData.treeInstanceCount; i++)
            {
                var item = terrain.terrainData.treeInstances[i];
                if (item.prototypeIndex != treeIndex) continue;
                var pos = new Vector3(item.position.x * terrain.terrainData.size.x, item.position.y * terrain.terrainData.size.y, item.position.z * terrain.terrainData.size.z);
                // edit_tree
                var instance = GameObject.Instantiate(terrain.terrainData.treePrototypes[item.prototypeIndex].prefab);
                instance.name = i + "";
                instance.transform.parent = edit_tree;
                instance.transform.localPosition = pos;
                instance.transform.rotation = Quaternion.Euler(0, item.rotation * 180 / Mathf.PI, 0);
                instance.transform.localScale =   Vector3.one*(item.heightScale);
                //item.heightScale = 0.01f;
                terrain.terrainData.SetTreeInstance(i, item);
            }
            terrain.drawTreesAndFoliage = false;


        }  [MenuItem("地形/可编辑树/更新到地形")]
        static private void toTerrain() {
            //    Debug.Log(GameObject.Find("Prop_rubbish008/Prop_rubbish008_LOD0").SetActive(false));
         var edit_tree= GameObject.Find("edit_tree");
            if (edit_tree == null) return;

            Terrain terrain= edit_tree.transform.parent.GetComponent<Terrain>();
            var trees = terrain.terrainData.treeInstances;
            foreach (Transform item in edit_tree.transform)
            {
                int index = int.Parse(item.name);
                var tree = terrain.terrainData.GetTreeInstance(index);
               var pos= new Vector3(item.localPosition.x / terrain.terrainData.size.x, item.localPosition.y / terrain.terrainData.size.y, item.localPosition.z / terrain.terrainData.size.z);
                 tree.rotation = item.transform.rotation.eulerAngles.y / 180 * Mathf.PI;
                tree.heightScale = item.localScale.y;
                tree.widthScale = item.localScale.y;
                tree.position = pos;
             //   Debug.Log(index+","+tree.prototypeIndex+":"+ pos.x+","+pos.y+","+pos.z);
                trees[index] = tree;
              //  terrain.terrainData.SetTreeInstance(index, tree);
            }

            terrain.terrainData.treeInstances = trees;
            terrain.drawTreesAndFoliage = true;
               GameObject.DestroyImmediate(edit_tree);
           


        }
         

    }

}