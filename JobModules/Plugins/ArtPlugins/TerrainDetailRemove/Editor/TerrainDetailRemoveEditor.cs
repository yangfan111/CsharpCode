using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ArtPlugins
{
    [UnityEditor.CustomEditor(typeof(TerrainDetailRemove))]
    public class TerrainDetailRemoveEditor: Editor
    {
        static int[][,] oldDetails;
        public override void OnInspectorGUI()
        {
            TerrainDetailRemove item = this.target as TerrainDetailRemove;
            if (item.maskParams == null || item.maskParams.Length < 8)
            {
                item.maskParams = new TerrainDetailRemoveParams[8];
            }
            item.showCount = EditorGUILayout.IntSlider("贴图数量", item.showCount, 1, 8);
            EditorGUILayout.LabelField("地表贴图\t剔除贴图");
            for (int i = 0; i < item.showCount; i++)
            {
                var p = item.maskParams[i];

                EditorGUILayout.BeginHorizontal();
                p.enabled = EditorGUILayout.Toggle(p.enabled);
                p.texture = EditorGUILayout.ObjectField(p.texture, typeof(Texture2D)) as Texture2D;
                p.textureMask = EditorGUILayout.ObjectField(p.textureMask, typeof(Texture2D)) as Texture2D;
                EditorGUILayout.EndHorizontal();
                p.removeAlpha = EditorGUILayout.Slider("达到多少透明度剔除", p.removeAlpha, 0, 1);
            }
            if (GUILayout.Button("剔除"))
            {
                updateterrain(item);
            }
            if (GUILayout.Button("取消剔除"))
            {
                revert(item.GetComponent<Terrain>().terrainData);
            }
        }
        public void updateterrain(TerrainDetailRemove item)
        {



            var tdata = item.GetComponent<Terrain>().terrainData;
            var layerData = new int[tdata.alphamapWidth, tdata.alphamapHeight];
            for (int dealCount = 0; dealCount < item.showCount; dealCount++)
            {
                if (item.maskParams[dealCount].enabled == false) continue;
                Color[] mask = null;
                var textureMask = item.maskParams[dealCount].textureMask;
                var texture = item.maskParams[dealCount].texture;
                var removeAlpha = item.maskParams[dealCount].removeAlpha;



                if (textureMask != null)
                {
                    mask = textureMask.GetPixels();
                }

                int textureIndex = -1;
                for (int i = 0; i < tdata.splatPrototypes.Length; i++)
                {
                    if (tdata.splatPrototypes[i].texture == texture) { textureIndex = i; break; }
                }

                if (textureIndex == -1) continue;
                float[,,] alphas = tdata.GetAlphamaps(0, 0, tdata.alphamapWidth, tdata.alphamapHeight);


                

                for (int i = 0; i < tdata.alphamapHeight; i++)
                {
                    for (int j = 0; j < tdata.alphamapWidth; j++)
                    {


                        if (alphas[j, i, textureIndex] >= removeAlpha)
                        {
                            //  Debug.Log(alphas[j, i, textureIndex]);
                            if (mask == null)
                            {
                                layerData[j, i] = 1;
                            }
                            else
                            {
                                float px = (((float)i) / tdata.alphamapWidth * tdata.size.x + tdata.splatPrototypes[textureIndex].tileOffset.x) / tdata.splatPrototypes[textureIndex].tileSize.x;
                                float py = (((float)j) / tdata.alphamapHeight * tdata.size.z + tdata.splatPrototypes[textureIndex].tileOffset.y) / tdata.splatPrototypes[textureIndex].tileSize.y;
                                px %= 1;
                                py %= 1;
                                if (px < 0) px += 1;
                                if (py < 0) py += 1;
                                //   py = 1 - py;


                                if (mask[(int)(textureMask.height * py) * textureMask.width + (int)(px * textureMask.width)].r > 0.5)
                                {
                                    layerData[j, i] = 1;
                                }
                            }
                        }
                    }
                }
            }


            oldDetails = new int[tdata.detailPrototypes.Length][,];
            for (int l = 0; l < tdata.detailPrototypes.Length; l++)
            {
                oldDetails[l] = tdata.GetDetailLayer(0, 0, tdata.detailResolution, tdata.detailResolution, l);
                var oldData = tdata.GetDetailLayer(0, 0, tdata.detailResolution, tdata.detailResolution, l);

                for (int i = 0; i < tdata.detailResolution; i++)
                {
                    for (int j = 0; j < tdata.detailResolution; j++)
                    {
                        if (layerData[j * tdata.alphamapWidth / tdata.detailResolution, i * tdata.alphamapHeight / tdata.detailResolution] > 0)
                            oldData[j, i] = 0;

                    }
                }
                tdata.SetDetailLayer(0, 0, l, oldData);
            }

        }

        private void revert(TerrainData tdata)
        {
            for (int l = 0; l < tdata.detailPrototypes.Length; l++)
            {
                tdata.SetDetailLayer(0, 0, l, oldDetails[l]);
            }
        }
    }



}