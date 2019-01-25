using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LB_Wind : MonoBehaviour {


	// Diffuse materials
	Material[] mats0;
	// Transparent materials
	Material[] mats1;

	public float Scale = 1f, speed = 1f;

	public void Start()
	{		
		UpdateWind ();
	}

	void FindMaterials()
	{

		// Find all snow shader's materials on game start    
		List<Material> mTransparented = new List<Material>();

		mTransparented = FindShader("LightingBox/Nature/Snow-Leave Standard");

		mats1 = new Material[mTransparented.Count];

		// Transparented snow shaders    
		for(int a = 0;a < mTransparented.Count;a++)
		{
			mats1[a] = mTransparented[a];
		}

	}

	[ContextMenu("Update Wind")]
	public void UpdateWind()
	{
		
		FindMaterials ();
		Shader.SetGlobalFloat("_Scale", Scale);
		Shader.SetGlobalFloat("_Speed", speed);
		foreach (Material m1 in mats1) {
			
		}
	}

	// Find and return all supported shaders (we want to find snow shaders in this case)   
	List<Material> FindShader(string shaderName)
	{

		List<Material> armat = new List<Material>();
		List<Material> exportMats = new List<Material>();

		Renderer[] arrend = (Renderer[])Resources.FindObjectsOfTypeAll(typeof(Renderer));


		foreach (Renderer rend in arrend)
		{
			foreach (Material mat in rend.sharedMaterials)
			{
				if (!armat.Contains (mat)) 
				{
					armat.Add (mat);
				}
			}
		}

		foreach (Material mat in armat) 
		{
			if (mat != null && mat.shader != null && mat.shader.name != null && mat.shader.name == shaderName)
			{
				exportMats.Add(mat);
			}
		}

		return exportMats;
	}

}
