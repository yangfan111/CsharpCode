using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


public enum TerrainType{
	Layer_4,
	Layer_6,
	Layer_8
}

public class LB_Terrain_2 : MonoBehaviour
{

	#region Variables
	public TerrainType terrainType = TerrainType.Layer_4;
	public Material terrainMaterial;
	public Texture2DArray textureArray;
	public Texture splatMap1,splatMap2;

	public bool failed;
	public bool initializd = false;

	public Terrain t;

	public float tessIntensity = 10f;
	public float minTess = 30f;
	public float maxTess = 100;

	public float[] disValue;
	public float[] smoothnessValue;
	public float[] normalPower;
	public Color[] specularColor;
	public float[] uvS;

	public SplatPrototype[] splats;


	// Grass system shade controller   
	public Color grassColor = Color.white;
	public float grassSmoothness = 0;
	public Color grassSpecular = Color.black;
	public Color grassTransColor = Color.white;
	public float grassTransIntensity = 0.3f;
	public float grassTransAmbient = 0;
	public float grassTransShadows  = 1f;

	public float grassWindScale = 0.03f;
	public float grassWindSpeed = 3.4f;
	public float grassWorldScale = 0.03f;

	#endregion

	public void UpdateGrass()
	{
		Shader.SetGlobalColor ("_Grass_Color", grassColor);

		Shader.SetGlobalFloat ("_Grass_Wind_Scale", grassWindScale);

		Shader.SetGlobalFloat ("_Grass_Wind_Speed", grassWindSpeed);

		Shader.SetGlobalFloat ("_Grass_World_Scale", grassWorldScale);

		Shader.SetGlobalFloat ("_Grass_Smoothness", grassSmoothness);
		Shader.SetGlobalFloat ("_Grass_TransAmbient", grassTransAmbient);
		Shader.SetGlobalFloat ("_Grass_TransShadow", grassTransShadows);
		Shader.SetGlobalFloat ("_Grass_Translucency_Intensity", grassTransIntensity);
		Shader.SetGlobalColor ("Grass_Specular_Color", grassSpecular);
		Shader.SetGlobalColor ("_Grass_Translucency_Color", grassTransColor);


	}

	#region Update Settings
	public void UpdateTessellation()
	{
		terrainMaterial.SetFloat ("_TessValue",tessIntensity);
		terrainMaterial.SetFloat ("_TessMin",minTess);
		terrainMaterial.SetFloat ("_TessMax",maxTess);
	}

	public void UpdateSettings()
	{
		if(splats == null)
			splats = t.terrainData.splatPrototypes;
		


		for (int a = 0; a < splats.Length; a++) 
		{
			terrainMaterial.SetFloat ("_Smoothness_"+(a+1).ToString(),smoothnessValue[a]);
			terrainMaterial.SetFloat ("_Normal_Scale_"+(a+1).ToString(),normalPower[a]);
			terrainMaterial.SetFloat ("_Displacement_"+(a+1).ToString(),disValue[a]);
			terrainMaterial.SetColor ("_Specular_Color_"+(a+1).ToString(),specularColor[a]);
			terrainMaterial.SetVector ("_UV_"+a.ToString(), new Vector2 (uvS [a],uvS [a]));
		}
	}
	#endregion

	public void LoadSplats()
	{
		t = GetComponent<Terrain> ();

		splats = t.terrainData.splatPrototypes;

	}

	#region Initialize
	public void Initalize()
	{
		
		t = GetComponent<Terrain> ();

		splats = t.terrainData.splatPrototypes;
	
		disValue = new float[splats.Length];

		smoothnessValue = new float[splats.Length];
		normalPower = new float[splats.Length];
		specularColor = new Color[splats.Length];

		uvS = new float[splats.Length];

		if (!terrainMaterial) {
			string path = EditorUtility.SaveFilePanelInProject ("Save As ...", "Terrain_Material_", "mat", "");

			if (path != "") {
				terrainMaterial = new Material (Shader.Find ("Standard"));
				AssetDatabase.CreateAsset (terrainMaterial, path);
				terrainMaterial = (Material)AssetDatabase.LoadAssetAtPath (path, typeof(Material)) as Material;
				AssetDatabase.Refresh ();
			}
		}

		UpdateTerrain ();

		if (!failed)
			initializd = true;
		else
			Debug.Log ("Initialize Failed");
	}
	#endregion

	#region Update Terrain
	public void UpdateTerrain ()
	{

		if (!GetComponent<Terrain> ()) {
			Debug.Log("Add this component into terrain object with terrain component    ");	
			return;
		}

		try
		{
		CheckTextureSize ();
		}
		catch{}

		if (failed)
			return;

		Material m = t.materialTemplate;

		t.materialType = Terrain.MaterialType.Custom;

		if(terrainType == TerrainType.Layer_8)
			terrainMaterial.shader = Shader.Find ("LightingBox/Terrain/Terrain 8-Layers");
		if(terrainType == TerrainType.Layer_4)
			terrainMaterial.shader = Shader.Find ("LightingBox/Terrain/Terrain 4-Layers");
		if(terrainType == TerrainType.Layer_6)
			terrainMaterial.shader = Shader.Find ("LightingBox/Terrain/Terrain 6-Layers");
		
		t.materialTemplate = terrainMaterial;

		if(t.terrainData.alphamapTextures [0])
			splatMap1 = t.terrainData.alphamapTextures [0];
		if (terrainType == TerrainType.Layer_8 || terrainType == TerrainType.Layer_6) 
		{
			if (t.terrainData.alphamapTextures [1])
				splatMap2 = t.terrainData.alphamapTextures [1];
		}

		if (!textureArray) {
			
			Texture2DArray textureArrayNew = new Texture2DArray (t.terrainData.splatPrototypes[0].texture.width, t.terrainData.splatPrototypes[0].texture.height, 16, TextureFormat.RGBA32, true);

			if (terrainType == TerrainType.Layer_8) {
				for (int i = 0; i < t.terrainData.splatPrototypes.Length; i++) {
					if (t.terrainData.splatPrototypes [i].texture)
						textureArrayNew.SetPixels (t.terrainData.splatPrototypes [i].texture.GetPixels (0), i, 0);	
				}

				for (int i = 0; i < t.terrainData.splatPrototypes.Length; i++) {
					if (t.terrainData.splatPrototypes [i].normalMap) {
							textureArrayNew.SetPixels (t.terrainData.splatPrototypes [i].normalMap.GetPixels (0), 8 + i, 0);		
					}

				}
			} 
			if (terrainType == TerrainType.Layer_4) {
				for (int i = 0; i < 4; i++) {
					if (t.terrainData.splatPrototypes [i].texture)
						textureArrayNew.SetPixels (t.terrainData.splatPrototypes [i].texture.GetPixels (0), i, 0);	
				}

				for (int i = 0; i < 4; i++) {
					if (t.terrainData.splatPrototypes [i].normalMap) {
						textureArrayNew.SetPixels (t.terrainData.splatPrototypes [i].normalMap.GetPixels (0), 4 + i, 0);		
					}
				}
			}
			if (terrainType == TerrainType.Layer_6) {
				for (int i = 0; i < 6; i++) {
					if (t.terrainData.splatPrototypes [i].texture)
						textureArrayNew.SetPixels (t.terrainData.splatPrototypes [i].texture.GetPixels (0), i, 0);	
				}

				for (int i = 0; i < 6; i++) {
					if (t.terrainData.splatPrototypes [i].normalMap) {
						textureArrayNew.SetPixels (t.terrainData.splatPrototypes [i].normalMap.GetPixels (0), 6 + i, 0);		
					}
				}
			}

			textureArrayNew.Apply ();

			terrainMaterial.SetTexture ("_AlbedoMaps", textureArrayNew);
		
			terrainMaterial.SetTexture ("_SplatMap1", splatMap1);

			if (terrainType == TerrainType.Layer_8 || terrainType == TerrainType.Layer_6)				
				terrainMaterial.SetTexture ("_SplatMap2", splatMap2);

			textureArray = textureArrayNew;

			var path = AssetDatabase.GetAssetPath (terrainMaterial);
			AssetDatabase.CreateAsset (textureArrayNew,System.IO.Path.GetDirectoryName(path)+"/"+
				System.IO.Path.GetFileNameWithoutExtension( path )+"_Data.asset");

		} else {
			
			if (terrainType == TerrainType.Layer_8) {
				for (int i = 0; i < t.terrainData.splatPrototypes.Length; i++) {
					if (t.terrainData.splatPrototypes [i].texture)
						textureArray.SetPixels (t.terrainData.splatPrototypes [i].texture.GetPixels (0), i, 0);	
				}

				for (int i = 0; i < t.terrainData.splatPrototypes.Length; i++) {
					if (t.terrainData.splatPrototypes [i].normalMap) {
							textureArray.SetPixels (t.terrainData.splatPrototypes [i].normalMap.GetPixels (0), 8 + i, 0);					
					}
				}
			}
			if (terrainType == TerrainType.Layer_4) {
				for (int i = 0; i < t.terrainData.splatPrototypes.Length; i++) {
					if (t.terrainData.splatPrototypes [i].texture)
						textureArray.SetPixels (t.terrainData.splatPrototypes [i].texture.GetPixels (0), i, 0);	
				}

				for (int i = 0; i < t.terrainData.splatPrototypes.Length; i++) {
					if (t.terrainData.splatPrototypes [i].normalMap) {
							textureArray.SetPixels (t.terrainData.splatPrototypes [i].normalMap.GetPixels (0), 4 + i, 0);
					}
				}
			}
			if (terrainType == TerrainType.Layer_6) {
				for (int i = 0; i < t.terrainData.splatPrototypes.Length; i++) {
					if (t.terrainData.splatPrototypes [i].texture)
						textureArray.SetPixels (t.terrainData.splatPrototypes [i].texture.GetPixels (0), i, 0);	
				}

				for (int i = 0; i < t.terrainData.splatPrototypes.Length; i++) {
					if (t.terrainData.splatPrototypes [i].normalMap) {
						textureArray.SetPixels (t.terrainData.splatPrototypes [i].normalMap.GetPixels (0), 6 + i, 0);					
					}
				}
			}

			textureArray.Apply ();

			terrainMaterial.SetTexture ("_AlbedoMaps", textureArray);

			terrainMaterial.SetTexture ("_SplatMap1", splatMap1);

			if (terrainType == TerrainType.Layer_8 || terrainType == TerrainType.Layer_6)				
				terrainMaterial.SetTexture ("_SplatMap2", splatMap2);

		}
		initializd = true;
	}
	#endregion

	#region Debug
	void CheckTextureSize()
	{
		Terrain t = GetComponent<Terrain> ();

		if (t.terrainData.splatPrototypes.Length == 2) {

			if (t.terrainData.splatPrototypes [0].texture.texelSize == t.terrainData.splatPrototypes [1].texture.texelSize) {
			} else {
				Debug.Log ("You must use same texture size for all terrain splats");
				failed = true;
			}

			if (t.terrainData.splatPrototypes [0].normalMap.texelSize == t.terrainData.splatPrototypes [1].normalMap.texelSize) {
			} else {
				Debug.Log ("You must use same normal map  texture size for all terrain splats");
				failed = true;
			}
		}
		if (t.terrainData.splatPrototypes.Length == 3) {
			if (t.terrainData.splatPrototypes [0].texture.texelSize == t.terrainData.splatPrototypes [1].texture.texelSize
				&& t.terrainData.splatPrototypes [1].texture.texelSize == t.terrainData.splatPrototypes [2].texture.texelSize) {
			} else {
				Debug.Log ("You must use same texture size for all terrain splats");
				failed = true;
			}

			if (t.terrainData.splatPrototypes [0].normalMap.texelSize == t.terrainData.splatPrototypes [1].normalMap.texelSize
				&& t.terrainData.splatPrototypes [1].normalMap.texelSize == t.terrainData.splatPrototypes [2].normalMap.texelSize) {
			} else {
				Debug.Log ("You must use same normal map  texture size for all terrain splats");
				failed = true;
			}

		}
		if (t.terrainData.splatPrototypes.Length == 4) {

			if (t.terrainData.splatPrototypes [0].texture.texelSize == t.terrainData.splatPrototypes [1].texture.texelSize
				&& t.terrainData.splatPrototypes [1].texture.texelSize == t.terrainData.splatPrototypes [2].texture.texelSize
				&& t.terrainData.splatPrototypes [2].texture.texelSize == t.terrainData.splatPrototypes [3].texture.texelSize) {
			} else {
				Debug.Log ("You must use same texture size for all terrain splats");
				failed = true;
			}

			if (t.terrainData.splatPrototypes [0].normalMap.texelSize == t.terrainData.splatPrototypes [1].normalMap.texelSize
				&& t.terrainData.splatPrototypes [1].normalMap.texelSize == t.terrainData.splatPrototypes [2].normalMap.texelSize
				&& t.terrainData.splatPrototypes [2].normalMap.texelSize == t.terrainData.splatPrototypes [3].normalMap.texelSize) {
			} else {
				Debug.Log ("You must use same normal map  texture size for all terrain splats");
				failed = true;
			}
		}
		if (terrainType == TerrainType.Layer_8 || terrainType == TerrainType.Layer_6) {
			if (t.terrainData.splatPrototypes.Length == 5) {

				if (t.terrainData.splatPrototypes [0].texture.texelSize == t.terrainData.splatPrototypes [1].texture.texelSize
				    && t.terrainData.splatPrototypes [1].texture.texelSize == t.terrainData.splatPrototypes [2].texture.texelSize
				    && t.terrainData.splatPrototypes [2].texture.texelSize == t.terrainData.splatPrototypes [3].texture.texelSize
				    && t.terrainData.splatPrototypes [3].texture.texelSize == t.terrainData.splatPrototypes [4].texture.texelSize) {
				} else {
					Debug.Log ("You must use same texture size for all terrain splats");
					failed = true;
				}

				if (t.terrainData.splatPrototypes [0].normalMap.texelSize == t.terrainData.splatPrototypes [1].normalMap.texelSize
				    && t.terrainData.splatPrototypes [1].normalMap.texelSize == t.terrainData.splatPrototypes [2].normalMap.texelSize
				    && t.terrainData.splatPrototypes [2].normalMap.texelSize == t.terrainData.splatPrototypes [3].normalMap.texelSize
				    && t.terrainData.splatPrototypes [3].normalMap.texelSize == t.terrainData.splatPrototypes [4].normalMap.texelSize) {
				} else {
					Debug.Log ("You must use same normal map  texture size for all terrain splats");
					failed = true;
				}
			}
			if (t.terrainData.splatPrototypes.Length == 6) {

				if (t.terrainData.splatPrototypes [0].texture.texelSize == t.terrainData.splatPrototypes [1].texture.texelSize
				    && t.terrainData.splatPrototypes [1].texture.texelSize == t.terrainData.splatPrototypes [2].texture.texelSize
				    && t.terrainData.splatPrototypes [2].texture.texelSize == t.terrainData.splatPrototypes [3].texture.texelSize
				    && t.terrainData.splatPrototypes [3].texture.texelSize == t.terrainData.splatPrototypes [4].texture.texelSize
				    && t.terrainData.splatPrototypes [4].texture.texelSize == t.terrainData.splatPrototypes [5].texture.texelSize) {
				} else {
					Debug.Log ("You must use same texture size for all terrain splats");
					failed = true;
				}

				if (t.terrainData.splatPrototypes [0].normalMap.texelSize == t.terrainData.splatPrototypes [1].normalMap.texelSize
				    && t.terrainData.splatPrototypes [1].normalMap.texelSize == t.terrainData.splatPrototypes [2].normalMap.texelSize
				    && t.terrainData.splatPrototypes [2].normalMap.texelSize == t.terrainData.splatPrototypes [3].normalMap.texelSize
				    && t.terrainData.splatPrototypes [3].normalMap.texelSize == t.terrainData.splatPrototypes [4].normalMap.texelSize
				    && t.terrainData.splatPrototypes [4].normalMap.texelSize == t.terrainData.splatPrototypes [5].normalMap.texelSize) {
				} else {
					Debug.Log ("You must use same normal map  texture size for all terrain splats");
					failed = true;
				}
			}
		}
		if (terrainType == TerrainType.Layer_8){
			if (t.terrainData.splatPrototypes.Length == 7) {

				if (t.terrainData.splatPrototypes [0].texture.texelSize == t.terrainData.splatPrototypes [1].texture.texelSize
				   && t.terrainData.splatPrototypes [1].texture.texelSize == t.terrainData.splatPrototypes [2].texture.texelSize
				   && t.terrainData.splatPrototypes [2].texture.texelSize == t.terrainData.splatPrototypes [3].texture.texelSize
				   && t.terrainData.splatPrototypes [3].texture.texelSize == t.terrainData.splatPrototypes [4].texture.texelSize
				   && t.terrainData.splatPrototypes [4].texture.texelSize == t.terrainData.splatPrototypes [5].texture.texelSize
				   && t.terrainData.splatPrototypes [5].texture.texelSize == t.terrainData.splatPrototypes [6].texture.texelSize) {
				} else {
					Debug.Log ("You must use same texture size for all terrain splats");
					failed = true;
				}

				if (t.terrainData.splatPrototypes [0].normalMap.texelSize == t.terrainData.splatPrototypes [1].normalMap.texelSize
				   && t.terrainData.splatPrototypes [1].normalMap.texelSize == t.terrainData.splatPrototypes [2].normalMap.texelSize
				   && t.terrainData.splatPrototypes [2].normalMap.texelSize == t.terrainData.splatPrototypes [3].normalMap.texelSize
				   && t.terrainData.splatPrototypes [3].normalMap.texelSize == t.terrainData.splatPrototypes [4].normalMap.texelSize
				   && t.terrainData.splatPrototypes [4].normalMap.texelSize == t.terrainData.splatPrototypes [5].normalMap.texelSize
				   && t.terrainData.splatPrototypes [5].normalMap.texelSize == t.terrainData.splatPrototypes [6].normalMap.texelSize) {
				} else {
					Debug.Log ("You must use same normal map  texture size for all terrain splats");
					failed = true;
				}
			}
			if (t.terrainData.splatPrototypes.Length == 7) {


				if (t.terrainData.splatPrototypes [0].texture.texelSize == t.terrainData.splatPrototypes [1].texture.texelSize
				   && t.terrainData.splatPrototypes [1].texture.texelSize == t.terrainData.splatPrototypes [2].texture.texelSize
				   && t.terrainData.splatPrototypes [2].texture.texelSize == t.terrainData.splatPrototypes [3].texture.texelSize
				   && t.terrainData.splatPrototypes [3].texture.texelSize == t.terrainData.splatPrototypes [4].texture.texelSize
				   && t.terrainData.splatPrototypes [4].texture.texelSize == t.terrainData.splatPrototypes [5].texture.texelSize
				   && t.terrainData.splatPrototypes [5].texture.texelSize == t.terrainData.splatPrototypes [6].texture.texelSize
				   && t.terrainData.splatPrototypes [6].texture.texelSize == t.terrainData.splatPrototypes [7].texture.texelSize) {
				} else {
					Debug.Log ("You must use same texture size for all terrain splats");
					failed = true;
				}

				if (t.terrainData.splatPrototypes [0].normalMap.texelSize == t.terrainData.splatPrototypes [1].normalMap.texelSize
				   && t.terrainData.splatPrototypes [1].normalMap.texelSize == t.terrainData.splatPrototypes [2].normalMap.texelSize
				   && t.terrainData.splatPrototypes [2].normalMap.texelSize == t.terrainData.splatPrototypes [3].normalMap.texelSize
				   && t.terrainData.splatPrototypes [3].normalMap.texelSize == t.terrainData.splatPrototypes [4].normalMap.texelSize
				   && t.terrainData.splatPrototypes [4].normalMap.texelSize == t.terrainData.splatPrototypes [5].normalMap.texelSize
				   && t.terrainData.splatPrototypes [5].normalMap.texelSize == t.terrainData.splatPrototypes [6].normalMap.texelSize
				   && t.terrainData.splatPrototypes [6].normalMap.texelSize == t.terrainData.splatPrototypes [7].normalMap.texelSize) {
				} else {
					Debug.Log ("You must use same normal map  texture size for all terrain splats");
					failed = true;
				}
			}
		}
	}
	#endregion

}
#endif