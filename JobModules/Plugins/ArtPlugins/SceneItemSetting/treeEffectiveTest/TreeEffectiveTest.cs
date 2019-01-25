using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArtPlugins
{
    public class TreeEffectiveTest : MonoBehaviour
    {
        TerrainData terrainData;
        // Use this for initialization
        [ContextMenu("calculatetress")]
        void calculatetress()
        {
            var treeCount = new Dictionary<int, int>();


            foreach (var item in GetComponent<Terrain>().terrainData.treeInstances)
            {

                if (treeCount.ContainsKey(item.prototypeIndex) == false)
                {
                    treeCount.Add(item.prototypeIndex, 0);
                }
                treeCount[item.prototypeIndex]++;
            }

            foreach (var item in treeCount)
            {
                print(item.Key + ":" + item.Value);
            }
        }
        private void Start()
        {
            terrainData = GetComponent<Terrain>().terrainData;
        }
        private void OnGUI()
        {

            GUI.skin.button.fontSize = 36;
            for (int t = 0; t < terrainData.treePrototypes.Length; t++)
            {
                if (GUILayout.Button("change to " + t))
                {

                    List<TreeInstance> trees = new List<TreeInstance>();
                    for (int i = 0; i < terrainData.treeInstanceCount; i++)
                    {
                        var tree = terrainData.treeInstances[i];
                        tree.prototypeIndex = t;
                        trees.Add(tree);

                    }
                    terrainData.treeInstances = trees.ToArray();

                }
            }

        }


    }

}