// Update snow intensity for all supported shaders in the current scene:
// LightingBox/Snow Standard (Specular)
// LightingBox/Nature/Snow-Leave Standard
//
//
// Example Usage :
// GameObject.FindObjectOfType<LB_Snow_Controller>().UpdateSnow(3f);
//
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LB_Snow_Controller : MonoBehaviour
{

	// Diffuse materials
	Material[] mats0;
	// Transparent materials
	Material[] mats1;

	void Start ()
	{
		// Find all snow shader's materials on game start    
		List<Material> mDiffuse = new List<Material>();
		List<Material> mTransparented = new List<Material>();

		mDiffuse = FindShader("LightingBox/Snow Standard (Specular)");
		mTransparented = FindShader("LightingBox/Nature/Snow-Leave Standard");

		mats0 = new Material[mDiffuse.Count];
		mats1 = new Material[mTransparented.Count];

		// Difuse snow shaders
 		for(int a = 0;a < mDiffuse.Count;a++)
		{
			mats0[a] = mDiffuse[a];
		}

		// Transparented snow shaders    
		for(int a = 0;a < mTransparented.Count;a++)
		{
			mats1[a] = mTransparented[a];
		}

	}

	// Add this public function into your UI.Slider compoennt events (OnValueChanger() event in slider inspector)    
	public void Change_Snow_Intensity()
	{		
		UpdateSnow (GetComponent<Slider>().value);
	}

	public void UpdateSnow(float intensity)
	{
		foreach(Material m0 in mats0)
			m0.SetFloat("_SnowIntensity",intensity);

		foreach(Material m1 in mats1)
			m1.SetFloat("_SnowIntensity",intensity);
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
