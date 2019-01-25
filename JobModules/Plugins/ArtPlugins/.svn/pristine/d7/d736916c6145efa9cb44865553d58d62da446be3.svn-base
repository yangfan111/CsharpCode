using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

namespace ArtPlugins
{
    public class TerrainItemInstanceTools {
        public static List<TerrainDetailInstance>[] getDetailObjects(TerrainData terrainData, float density) {
            return TerrainDetailInstance.ExportObjects(terrainData, density);
        }

        public static Vector3 getTerrainPos(TerrainData terrainData) {

            var index=getTerrainIndex(terrainData);


             return  new Vector3((index%8) * 1000 - 4000 ,28, (index/8) * 1000 - 4000 );
             
        }
    public    static float GetInterpolatedHeight(TerrainData terrainData, float x, float y)
{
            x /= 1000;
            y /= 1000;

    float fx = x * (terrainData.heightmapWidth - 1);
        float fy = y * (terrainData.heightmapHeight - 1);
        int lx = (int)fx;
        int ly = (int)fy;

        float u = fx - lx;
        float v = fy - ly;

    if (u > v)
    {
        float z00 = terrainData.GetHeight(lx + 0, ly + 0);
        float z01 = terrainData.GetHeight(lx + 1, ly + 0);
        float z11 = terrainData.GetHeight(lx + 1, ly + 1);
        return z00 + (z01 - z00) * u + (z11 - z01) * v;
    }
    else
    {
        float z00 = terrainData.GetHeight(lx + 0, ly + 0);
    float z10 = terrainData.GetHeight(lx + 0, ly + 1);
    float z11 = terrainData.GetHeight(lx + 1, ly + 1);
        return z00 + (z11 - z10) * u + (z10 - z00) * v;
}
}

        internal static int getTerrainIndex(TerrainData terrainData)
        {
            var sname = terrainData.name.TrimEnd("W".ToCharArray()).Split(" ".ToCharArray())[1].Split("x".ToCharArray());

            return  int.Parse(sname[0]) + int.Parse(sname[1])*8;
        }
    }


  [System.Serializable]
   public class TerrainDetailInstance :MapSectionItemBase
    {

        public int layer;
        // public Vector3 Rotation;
        // public Vector3 Scale;
   
public static List<TerrainDetailInstance>[]  ExportObjects(TerrainData terrainData,float density)
        {
    

            TerrainData data = terrainData;
         


            int detailWidth = data.detailWidth;
                int detailHeight = data.detailHeight;

            //for rect
            int rectStartX = 0;
            int rectStartY = 0;
            int rectWidth = detailWidth;
            int rectHeight = detailHeight;

                float delatilWToTerrainW = data.size.x / detailWidth;
                float delatilHToTerrainW = data.size.z / detailHeight;

            Vector3 mapPosition = TerrainItemInstanceTools.getTerrainPos(terrainData);

                
           
            

           

                DetailPrototype[] details = data.detailPrototypes;
            List<TerrainDetailInstance>[] output = new List<TerrainDetailInstance>[details.Length];

            for (int i = 0; i < details.Length; i++)
                {
                //   if (i != detailLayer) continue;
                output[i] = new List<TerrainDetailInstance>();

               var protoType= details[i];

                    int[,] map = data.GetDetailLayer(rectStartX, rectStartY, rectWidth, rectHeight, i);
                    float currentDentity = 0;
                    int detailCount = 0;
                int convertCount = 0;
                  //  List<Vector3> grasses = new List<Vector3>();
                    for (var y = 0; y < rectHeight; y++)
                    {
                        for (var x = 0; x < rectWidth; x++)
                        {
                            if (map[y,x] > 0)
                            {
                             
                                detailCount += map[ y,x];
                                currentDentity += map[ y,x]* density;
                                int drawCount = (int)currentDentity;
                            convertCount += drawCount;
                            //  float unitOffset =0;
                            //  if (drawCount > 0) {
                            //   unitOffset = 1.0f / (Mathf.Sqrt(drawCount) + 1);
                            //   }
                            // int drawCol =Mathf.FloorToInt( Mathf.Sqrt(drawCount));
                            for (int drawIndex = 0; drawIndex < drawCount; drawIndex++)
                            {
                                UnityEngine.Random.Range(1, 1);
                                
                                float _x = ((x + rectStartX + UnityEngine.Random.Range(-100, 100)*0.01f) )* delatilWToTerrainW ;
                                float _z = ((y + rectStartY + UnityEngine.Random.Range(-100, 100) * 0.01f))* delatilHToTerrainW;
                                float _y = TerrainItemInstanceTools.GetInterpolatedHeight(terrainData, _x,_z);
                                var pos = new Vector3(
                                        _x,
                                        _y,
                                        _z 
                                        ) + mapPosition;
                                // grasses.Add(pos);

                                TerrainDetailInstance e = new TerrainDetailInstance();
                                //if (i == 1 && output[i].Count % 100 == 1)
                                //{
                                //    UnityEngine.Debug.Log(pos);
                                //}
                                e.layer = i;
                                e.position = pos;
                                //e.Rotation = new Vector3(0, UnityEngine.Random.Range(0, 360), 0);
                                //e.Scale = new Vector3(UnityEngine.Random.Range(minWidth, maxWidth), UnityEngine.Random.Range(minHeight, maxHeight), UnityEngine.Random.Range(minWidth, maxWidth));

                                output[i].Add(e);
                               
                            }
                            currentDentity -= drawCount;

                        }
                        }
                    }
              //  UnityEngine.Debug.Log(string.Format("detali:layer={0},count={1},convertCount={2}", i,detailCount, convertCount));
                    //foreach (var item in grasses)
                    //{
                    
                    //}
                
            }


            return output;
        }
    }
}