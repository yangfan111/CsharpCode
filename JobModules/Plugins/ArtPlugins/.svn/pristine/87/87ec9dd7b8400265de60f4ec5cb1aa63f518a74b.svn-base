using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class LB_Grass_Controll : MonoBehaviour
{

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


	void Start()
	{
		UpdateGrass ();
	}
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
}
