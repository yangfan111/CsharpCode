using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LB_LightingBoxTerrain : MonoBehaviour {

	public SplatPrototype[] splats;

	public Material tMaterial;
	public Texture2D layer0,layer1,layer2,layer3,layer4;
	public Texture2D layer_Normal_0, layer_Normal_1, layer_Normal_2, layer_Normal_3, layer_Normal_4;
	public bool autoUpdate = true;
	public Texture splatMap1,splatMap2;
	public float sUV0 = 30f,sUV1 = 30f,sUV2 = 30f,sUV3 = 30f,sUV4 = 30f;
	public float sSmoothness0 = 0.3f,sSmoothness1 = 0.3f,sSmoothness2 = 0.3f,sSmoothness3 = 0.3f,sSmoothness4 = 0.3f;
	public float sDisplacement0,sDisplacement1,sDisplacement2,sDisplacement3,sDisplacement4;
	public float sNormal0 = 1f,sNormal1 = 1f,sNormal2 = 1f,sNormal3 = 1f,sNormal4 = 1f;
	public bool initialized;
	public float tesValue = 10f,tesMin= 30f,tesMax = 30f;

	public void Init()
	{

		Terrain t = GetComponent<Terrain> ();

		t.materialType = Terrain.MaterialType.Custom;
		tMaterial.shader = Shader.Find ("LightingBox/Terrain/Terrain 5-Layers");

		t.materialTemplate = tMaterial;

		splats = t.terrainData.splatPrototypes;

		initialized = true;

		LoadSettings ();

	}

	public void LoadSettings()
	{
		if (initialized) 
		{
			Terrain t = GetComponent<Terrain> ();
			splats = t.terrainData.splatPrototypes;

			Material m = t.materialTemplate;

			layer0 = splats [0].texture;
			layer1 = splats [1].texture;
			layer2 = splats [2].texture;
			layer3 = splats [3].texture;
			layer4 = splats [4].texture;

			layer_Normal_0 = splats [0].normalMap;
			layer_Normal_1 = splats [1].normalMap;
			layer_Normal_2 = splats [2].normalMap;
			layer_Normal_3 = splats [3].normalMap;
			layer_Normal_4 = splats [4].normalMap;

			sDisplacement0 = m.GetFloat ("_Displacement_0");
			sDisplacement1 = m.GetFloat ("_Displacement_1");
			sDisplacement2 = m.GetFloat ("_Displacement_2");
			sDisplacement3 = m.GetFloat ("_Displacement_3");
			sDisplacement4 = m.GetFloat ("_Displacement_4");

			sSmoothness0 = m.GetFloat ("_Smoothness_0");
			sSmoothness1 = m.GetFloat ("_Smoothness_1");
			sSmoothness2 = m.GetFloat ("_Smoothness_2");
			sSmoothness3 = m.GetFloat ("_Smoothness_3");
			sSmoothness4 = m.GetFloat ("_Smoothness_4");

			sNormal0 = m.GetFloat ("_Normal_Power_0");
			sNormal1 = m.GetFloat ("_Normal_Power_1");
			sNormal2 = m.GetFloat ("_Normal_Power_2");
			sNormal3 = m.GetFloat ("_Normal_Power_3");
			sNormal4 = m.GetFloat ("_Normal_Power_4");

			splatMap1 = m.GetTexture ("_Control_1");
			splatMap2 = m.GetTexture ("_Control_2");

			sUV0 = m.GetTextureScale ("_Layer_0").x;
			sUV1 = m.GetTextureScale ("_Layer_1").x;
			sUV2 = m.GetTextureScale ("_Layer_2").x;
			sUV3 = m.GetTextureScale ("_Layer_3").x;
			sUV4 = m.GetTextureScale ("_Layer_4").x;

			UpdateTerrain ();

		}
	}

	public void UpdateTerrain()
	{
		if (!GetComponent<Terrain> ())
			return;
		
		Terrain t = GetComponent<Terrain> ();

		Material m = t.materialTemplate;

		splatMap1 = t.terrainData.alphamapTextures [0];
		splatMap2 = t.terrainData.alphamapTextures [1];

		SplatPrototype[] sp =  t.terrainData.splatPrototypes;

		if(splatMap1)
			m.SetTexture ("_Control_1",splatMap1 );
		if(splatMap2)
			m.SetTexture ("_Control_2",splatMap2 );
		
		m.SetTexture ("_Normal_0", layer_Normal_0);
		m.SetTexture ("_Normal_1", layer_Normal_1);
		m.SetTexture ("_Normal_2", layer_Normal_2);
		m.SetTexture ("_Normal_3", layer_Normal_3);
		m.SetTexture ("_Normal_4", layer_Normal_4);

		m.SetTexture ("_Layer_0", layer0);
		m.SetTexture ("_Layer_1", layer1);
		m.SetTexture ("_Layer_2", layer2);
		m.SetTexture ("_Layer_3", layer3);
		m.SetTexture ("_Layer_4", layer4);

		m.SetFloat ("_Displacement_0", sDisplacement0);
		m.SetFloat ("_Displacement_1", sDisplacement1);
		m.SetFloat ("_Displacement_2", sDisplacement2);
		m.SetFloat ("_Displacement_3", sDisplacement3);
		m.SetFloat ("_Displacement_4", sDisplacement4);

		m.SetFloat ("_Smoothness_0", sSmoothness0);
		m.SetFloat ("_Smoothness_1", sSmoothness1);
		m.SetFloat ("_Smoothness_2", sSmoothness2);
		m.SetFloat ("_Smoothness_3", sSmoothness3);
		m.SetFloat ("_Smoothness_4", sSmoothness4);

		m.SetFloat ("_Normal_Power_0", sNormal0);
		m.SetFloat ("_Normal_Power_1", sNormal1);
		m.SetFloat ("_Normal_Power_2", sNormal2);
		m.SetFloat ("_Normal_Power_3", sNormal3);
		m.SetFloat ("_Normal_Power_4", sNormal4);

		m.SetTextureScale ("_Layer_0", new Vector2 (sUV0, sUV0));
		m.SetTextureScale ("_Layer_1", new Vector2 (sUV1, sUV1));
		m.SetTextureScale ("_Layer_2", new Vector2 (sUV2, sUV2));
		m.SetTextureScale ("_Layer_3", new Vector2 (sUV3, sUV3));
		m.SetTextureScale ("_Layer_4", new Vector2 (sUV4, sUV4));

		m.SetTextureScale ("_Normal_0", new Vector2 (sUV0, sUV0));
		m.SetTextureScale ("_Normal_1", new Vector2 (sUV1, sUV1));
		m.SetTextureScale ("_Normal_2", new Vector2 (sUV2, sUV2));
		m.SetTextureScale ("_Normal_3", new Vector2 (sUV3, sUV3));
		m.SetTextureScale ("_Normal_4", new Vector2 (sUV4, sUV4));

		m.SetFloat ("_TessValue", tesValue);
		m.SetFloat ("_TessMin", tesMin);
		m.SetFloat ("_TessMax", tesMax);

		sp [0].texture = layer0;
		sp [1].texture = layer1;
		sp [2].texture = layer2;
		sp [3].texture = layer3;
		sp [4].texture = layer4;

		sp [0].normalMap = layer_Normal_0;
		sp [1].normalMap = layer_Normal_1;
		sp [2].normalMap = layer_Normal_2;
		sp [3].normalMap = layer_Normal_3;
		sp [4].normalMap = layer_Normal_4;

		sp [0].smoothness = sSmoothness0;
		sp [1].smoothness = sSmoothness1;
		sp [2].smoothness = sSmoothness2;
		sp [3].smoothness = sSmoothness3;
		sp [4].smoothness = sSmoothness4;

	}
}
