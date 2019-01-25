using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MapMagic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TerrainEditCell : MonoBehaviour
{
    public GameObject terransGroup;
    public int cellNumberX = 2, cellNumberZ = 2;
    
    private TerrainData terrainData;
    private int cellXIndex = 0, cellZIndex = 0;
    public bool loadCellData = false;
    public bool saveCellData = false;
    [HideInInspector] 
    public  int lastAllIndex=-1;

    private int defaultWid = 1000;
    public   Terrain[] terrans=new Terrain[4];
    private void SaveCellData()
    {
      
      
    
 
        
        terrainData = GetComponent<Terrain>().terrainData; 
 
        List<TreeInstance>[] treeLists = new List<TreeInstance>[4];
        SplatPrototype[] splatProtos = terrainData.splatPrototypes;
        int[] rectIndex = {2,3,0,1};
        for (int i = 0; i < 4; i++)
        {
            treeLists[i] = new List<TreeInstance>();
            copyTerrainSet(terrainData, terrans[i].terrainData, false);
 
      

            
            //设置地形原型
            SplatPrototype[] newSplats = new SplatPrototype[splatProtos.Length];
            for (int j = 0; j < splatProtos.Length;  ++j)
            {
                newSplats[j] = new SplatPrototype();
                newSplats[j].texture = splatProtos[j].texture;
                newSplats[j].tileSize = splatProtos[j].tileSize;

                float offsetX = (terrans[i].terrainData.size.x * (rectIndex[i]%2)) % splatProtos[j].tileSize.x + splatProtos[j].tileOffset.x;
                float offsetY = (terrans[i].terrainData.size.z * (rectIndex[i]/2)) % splatProtos[j].tileSize.y + splatProtos[j].tileOffset.y;
                newSplats[j].tileOffset = new Vector2(offsetX, offsetY);
            }
            terrans[i].terrainData.splatPrototypes = newSplats;
           
        }
        int wid = 512;
        int hei = 512;
           
        
         terrans[0].terrainData.SetHeights(0, 0, terrainData.GetHeights(0, hei, wid + 1, hei + 1)); 
        terrans[1].terrainData.SetHeights(0, 0,  terrainData.GetHeights(wid, hei, wid + 1, hei + 1));
        terrans[2].terrainData.SetHeights(0, 0,  terrainData.GetHeights(0, 0, wid + 1, hei + 1));
        terrans[3].terrainData.SetHeights(0, 0,  terrainData.GetHeights(wid, 0, wid + 1, hei + 1));

        wid = hei = terrainData.alphamapResolution/2;
        
         terrans[0].terrainData.SetAlphamaps(0, 0, terrainData.GetAlphamaps(0, hei, wid , hei )); 
        terrans[1].terrainData.SetAlphamaps(0, 0,  terrainData.GetAlphamaps(wid, hei, wid , hei));
        terrans[2].terrainData.SetAlphamaps(0, 0,  terrainData.GetAlphamaps(0, 0, wid, hei));
         terrans[3].terrainData.SetAlphamaps(0, 0,  terrainData.GetAlphamaps(wid, 0, wid, hei));

        wid = hei = terrainData.detailResolution/2;

//      int[,] empty=  new int[wid, hei];
//                    foreach (int i in terrainData.GetSupportedLayers(0, 0,  terrans[0].terrainData.detailResolution,
//            terrans[0].terrainData.detailResolution))
//        {
//            terrans[0].terrainData.SetDetailLayer(0, 0, i,empty);
//        }

        foreach (int i  in  terrainData.GetSupportedLayers(0,0,terrainData.detailResolution,terrainData.detailResolution))
        {
         
                terrans[0].terrainData.SetDetailLayer(0, 0, i, terrainData.GetDetailLayer(0, hei, wid, hei,i));
                terrans[1].terrainData.SetDetailLayer(0, 0, i, terrainData.GetDetailLayer(wid, hei, wid, hei,i));
                terrans[2].terrainData.SetDetailLayer(0, 0, i, terrainData.GetDetailLayer(0, 0, wid, hei,i));
                terrans[3].terrainData.SetDetailLayer(0, 0, i, terrainData.GetDetailLayer(wid, 0, wid, hei,i));
 
        }
        
         
        for (int i = 0; i < terrainData.treeInstanceCount; i++)
        {
            TreeInstance treeInstance = terrainData.treeInstances[i];
            int cellIndex = get4CellIndex(treeInstance.position.x, treeInstance.position.z);
           // print( treeInstance.position );
          //  switch (cellIndex)
          //  {
                    
                  //  case 0:
                    float realX = treeInstance.position.x * 2;
                    float realZ = treeInstance.position.z * 2;
                    if (realX > 1) realX -= 1;
                    if (realZ > 1) realZ -= 1;
                        treeInstance.position =new Vector3(realX, treeInstance.position.y,realZ); 

            treeLists[cellIndex].Add(treeInstance);
        }
        
        for (int i = 0; i < 4; i++)
        {
           terrans[i].terrainData.treeInstances = treeLists[i].ToArray();
        }

       

       
    }

    private void copyTerrainSet(TerrainData src, TerrainData des, bool fourToOne)
    {
        des.treePrototypes = src.treePrototypes;
        des.detailPrototypes = src.detailPrototypes;
     
        des.wavingGrassAmount = src.wavingGrassAmount;
        des.wavingGrassSpeed = src.wavingGrassSpeed;
        des.wavingGrassStrength = src.wavingGrassStrength;
        des.wavingGrassTint = src.wavingGrassTint;

        if (fourToOne)
        {
            des.heightmapResolution = (src.heightmapResolution - 1) * 2 + 1;

            des.SetDetailResolution(src.detailResolution * 2, 8);
            des.alphamapResolution = src.alphamapResolution * 2;
            des.baseMapResolution = src.baseMapResolution * 2;
            des.size = new Vector3(src.size.x*2,src.size.y,src.size.z*2);
        }
        else
        {
            des.heightmapResolution = (src.heightmapResolution - 1) / 2 + 1;

            des.SetDetailResolution(src.detailResolution / 2, 8);
            des.alphamapResolution = src.alphamapResolution / 2;
            des.baseMapResolution = src.baseMapResolution / 2;
            des.size = new Vector3(src.size.x / 2, src.size.y, src.size.z / 2);

        }
       
    }

    void LoadCellData()
    {

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {

//                var items = terransGroup.transform.Find("SplittedTerrain_" + i + "_" + j).GetComponent<Terrain>()
//                    .terrainData.detailPrototypes;
//                    foreach (var item in items 
//                    )
//                    {
//                        item.prototypeTexture = null;
//                        item.prototype = null;
//                    }
//               terransGroup.transform.Find("SplittedTerrain_" + i + "_" + j).GetComponent<Terrain>()
//                    .terrainData.detailPrototypes=items;
                
              
           

            }
        }

       Terrain base00 = terransGroup.transform.Find("SplittedTerrain_0_0").GetComponent<Terrain>();
        Terrain terrainCell= GetComponent<Terrain>();
        terrainData = terrainCell.terrainData;

        copyTerrainSet(base00.terrainData, terrainData, true);

     

            List<TreeInstance> treeLists = new List<TreeInstance>();
                   SplatPrototype[] splatProtos = base00.terrainData.splatPrototypes;
                  int[] rectIndex = { 2, 3, 0, 1 };
        //            for (int i = 0; i < 4; i++)
        //            {
        //  
        //               
        //
        //
        //
                        //设置地形原型
//                        SplatPrototype[] newSplats = new SplatPrototype[splatProtos.Length];
                     for (int j = 0; j < splatProtos.Length; ++j)
                       {
//                            newSplats[j] = new SplatPrototype();
//                            newSplats[j].texture = splatProtos[j].texture;
//                            newSplats[j].tileSize = splatProtos[j].tileSize;
//        
//                            float offsetX = (terrans[i].terrainData.size.x * (rectIndex[i] % 2)) % splatProtos[j].tileSize.x + splatProtos[j].tileOffset.x;
//                            float offsetY = (terrans[i].terrainData.size.z * (rectIndex[i] / 2)) % splatProtos[j].tileSize.y + splatProtos[j].tileOffset.y;
//                            newSplats[j].tileOffset = new Vector2(offsetX, offsetY);
                       }
                        terrainData.splatPrototypes = base00.terrainData.splatPrototypes;
        //
        //            }
                    int wid = base00.terrainData.heightmapResolution-1;
                    int hei = wid;
       
         
        terrainData.SetHeights(0, 0, terrans[0].terrainData.GetHeights(0,0,wid,hei));
         terrainData.SetHeights(0, hei, terrans[1].terrainData.GetHeights(0, 0, wid, hei));
       terrainData.SetHeights(wid, 0, terrans[2].terrainData.GetHeights(0, 0, wid, hei));
        terrainData.SetHeights(wid, hei, terrans[3].terrainData.GetHeights(0, 0, wid, hei));

 

            wid = hei = terrainData.alphamapResolution / 2;

        terrainData.SetAlphamaps(0, 0, terrans[0].terrainData.GetAlphamaps(0, 0, wid, hei));
        terrainData.SetAlphamaps(0, hei, terrans[1].terrainData.GetAlphamaps(0, 0, wid, hei));
        terrainData.SetAlphamaps(wid, 0, terrans[2].terrainData.GetAlphamaps(0, 0, wid, hei));
        terrainData.SetAlphamaps(wid, hei, terrans[3].terrainData.GetAlphamaps(0, 0, wid, hei));

 

           wid = hei = terrainData.detailResolution / 2;
 
 
            foreach (int i in terrans[0].terrainData.GetSupportedLayers(0, 0, terrans[0].terrainData.detailResolution, terrans[0].terrainData.detailResolution))
            {
                terrainData.SetDetailLayer(0, 0,i, terrans[0].terrainData.GetDetailLayer(0, 0, wid, hei,i));
            }

        foreach (int i in terrans[1].terrainData.GetSupportedLayers(0, 0, terrans[1].terrainData.detailResolution,
            terrans[1].terrainData.detailResolution))
        {
            terrainData.SetDetailLayer(0, hei, i, terrans[1].terrainData.GetDetailLayer(0, 0, wid, hei, i));
        }
        foreach (int i in terrans[2].terrainData.GetSupportedLayers(0, 0, terrans[2].terrainData.detailResolution,
            terrans[2].terrainData.detailResolution))
        {
            terrainData.SetDetailLayer(wid, 0, i, terrans[2].terrainData.GetDetailLayer(0, 0, wid, hei, i));
        }
        foreach (int i in terrans[3].terrainData.GetSupportedLayers(0, 0, terrans[3].terrainData.detailResolution,
            terrans[3].terrainData.detailResolution))
        {
            terrainData.SetDetailLayer(wid, hei, i, terrans[3].terrainData.GetDetailLayer(0, 0, wid, hei, i));
        }


//
//
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < terrans[j].terrainData.treeInstanceCount; i++)
            {
                TreeInstance treeInstance = terrans[j].terrainData.treeInstances[i];

                float realX = treeInstance.position.x / 2;
                float realZ = treeInstance.position.z / 2;
             
                        realZ +=(j%2)*0.5f;
                realX += (j /2) * 0.5f;
                treeInstance.position = new Vector3(realX, treeInstance.position.y, realZ);

                treeLists.Add(treeInstance);
            }
        }
           

          
               terrainData.treeInstances = treeLists.ToArray();
           

        }

        private int get4CellIndex(float positionX, float positionZ)
    {
        if (positionX <= 0.5)
        {
            return positionZ > 0.5 ? 0 : 2;
        }

        return positionZ > 0.5 ? 1 : 3;
    }

    private void Update()
    {
        
        if (loadCellData)
        {
            loadCellData = false;
            LoadCellData();
            return;
        }
        if (saveCellData)
        {
            SaveCellData();
            saveCellData = false;
            return;
        }
        terransGroup.transform.localPosition=Vector3.zero;
        
        terrainData = GetComponent<Terrain>().terrainData;
        Vector3 dpos = transform.localPosition;
        cellXIndex = (int)(dpos.x / terrainData.size.x*2+0.5f);
        cellZIndex = (int)(dpos.z / terrainData.size.z*2+0.5f);
        if (cellXIndex < 0) cellXIndex = 0;
        if (cellXIndex> cellNumberX-2) cellXIndex = cellNumberX-2;
        if (cellZIndex < 0) cellZIndex = 0;
        if (cellZIndex > cellNumberZ-2) cellZIndex = cellNumberZ-2;
        
        transform.localPosition = new Vector3(cellXIndex*terrainData.size.x/2, 0, 
                                 (cellZIndex)*terrainData.size.z/2);

        if (lastAllIndex != cellZIndex * 100 + cellXIndex)
        {
            print(lastAllIndex);
            lastAllIndex = cellZIndex * 100 + cellXIndex;
       
            UpdateMoveCell();
        }
    }

    private void OnDisable()
    {
        UpdateMoveCell(true);
    }

    private void OnEnable()
    {
        UpdateMoveCell(false);
    }
    private void UpdateMoveCell(bool hideSelf=false)
    {
        print("UpdateMoveCell");

        foreach (Transform t in terransGroup.transform)
        {
            if(t.gameObject.activeSelf==false)t.gameObject.SetActive(true);
        }
if(hideSelf) return;
        for (int i = 0; i < 2 ; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                
                terrans[i*2+j] = terransGroup.transform.Find("SplittedTerrain_"+(cellXIndex+i)+"_"+(cellZIndex +j)).GetComponent<Terrain>();
                terrans[i*2+j].gameObject.SetActive(false);

            }

        }
    }
}