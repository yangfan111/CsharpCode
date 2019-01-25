using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class Terrain8ShaderGUI : ShaderGUI {
    public Material material;
     public TerrainData terrainData;
     public Terrain terrain;
    public float terrainUVScale = 1;
    public List<float> mList=new List<float>();
    public List<float> gList=new List<float>();



    
    private void makeData(TerrainData terrainData )
    {
        if (terrainData == null) return;
        mList.Clear();
        gList.Clear();
        List<Vector4> uvList = new List<Vector4>();

        List<Texture2D> texList = new List<Texture2D>();
        List<Texture2D> nmrList = new List<Texture2D>();
        int index = -1;
        foreach (var item in terrainData.splatPrototypes)
        {
            index++;
            material.SetTexture("_TextureArray" + (index + 1), item.texture);
            material.SetTextureOffset("_TextureArray" + (index + 1), item.tileOffset);
            material.SetTextureScale("_TextureArray" + (index + 1), item.tileSize * terrainUVScale);
            material.SetTexture("_NormalArray" + (index + 1), item.normalMap);

            //   uvList.Add(new Vector4(item.tileOffset.x, item.tileOffset.y, item.tileSize.x, item.tileSize.y));
            mList.Add(item.metallic);
            gList.Add(item.smoothness);
            //  texList.Add(item.texture);
            // texList.Add(item.normalMap);

        }
        for (int i = 0; i < 8 - terrainData.splatPrototypes.Length; i++)
        {
            index++;
            material.SetTexture("_TextureArray" + (index + 1), null);
            material.SetTextureOffset("_TextureArray" + (index + 1), Vector2.zero);
            material.SetTextureScale("_TextureArray" + (index + 1), Vector2.zero);
            material.SetTexture("_NormalArray" + (index + 1), null);
            mList.Add(0);
            gList.Add(0);
        }



        // material.SetVectorArray("_UVArray", uvList.ToArray());
        material.SetVector("_Metallic1", new Vector4(mList[0], mList[1], mList[2], mList[3]));
        material.SetVector("_Metallic2", new Vector4(mList[4], mList[5], mList[6], mList[7]));

        material.SetVector("_Smooth1", new Vector4(gList[0], gList[1], gList[2], gList[3]));
        material.SetVector("_Smooth2", new Vector4(gList[4], gList[5], gList[6], gList[7]));
        material.SetTexture("_Split1Tex", terrainData.alphamapTextures[0]);
        material.SetTexture("_Split2Tex", terrainData.alphamapTextures[1]);

    }
    
     public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props) {
        material = materialEditor.target as Material;
        terrainUVScale = EditorGUILayout.FloatField("terrainUVScale", terrainUVScale);
        terrainData = EditorGUILayout.ObjectField("link terrain data", terrainData, typeof(TerrainData)) as TerrainData;
        terrain = EditorGUILayout.ObjectField("link terrain", terrain, typeof(Terrain)) as Terrain;
        if (terrain != null) {
            terrainData = terrain.terrainData;
            terrain= null;
        }

        if (terrainData != null)
        {
            makeData(terrainData);
            terrainData = null;
        }
        base.OnGUI(materialEditor, props);
      
    }

    


  
}
