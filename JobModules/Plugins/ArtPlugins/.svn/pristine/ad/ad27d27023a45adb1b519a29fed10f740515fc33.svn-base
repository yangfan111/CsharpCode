using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LB_Grass_Controll))]
public class LB_Grass_ControllEditor : Editor 
{
	LB_Grass_Controll targetObject;
	   

	void OnEnable()
	{
		targetObject = (LB_Grass_Controll)target;

		targetObject.UpdateGrass ();
	}

	Color col = Color.white;
	public override void OnInspectorGUI()
	{
		
		serializedObject.Update ();

		targetObject = (LB_Grass_Controll)target;

		GUILayout.BeginVertical ("", GUI.skin.box);

		EditorGUILayout.LabelField ("Grass Settings",GUI.skin.box);

		var grassColorRef = targetObject.grassColor;
		var grassWindScaleRef = targetObject.grassWindScale;
		var grassWindSpeedRef = targetObject.grassWindSpeed;

		var grassSpecularRef = targetObject.grassSpecular;
		var grassSmoothnessRef = targetObject.grassSmoothness;
		var grassTransColorRef = targetObject.grassTransColor;
		var grassTransIntensityRef = targetObject.grassTransIntensity;
		var grassTransAmbientRef = targetObject.grassTransAmbient;
		var grassTransShadowsRef = targetObject.grassTransShadows;
		var grassWorldScaleRef = targetObject.grassWorldScale;

		targetObject.grassColor = EditorGUILayout.ColorField ("Grass Color", targetObject.grassColor);
		targetObject.grassSpecular = EditorGUILayout.ColorField ("Specular Color", targetObject.grassSpecular);
		targetObject.grassSmoothness = EditorGUILayout.Slider ("Smoothness", targetObject.grassSmoothness,0,1);
		EditorGUILayout.Space ();
		targetObject.grassTransColor = EditorGUILayout.ColorField ("Trasnlucency Color", targetObject.grassTransColor);
		targetObject.grassTransIntensity = EditorGUILayout.Slider ("Trasnlucency Intensity", targetObject.grassTransIntensity,0,1);
		targetObject.grassTransAmbient = EditorGUILayout.Slider ("Trasnlucency Ambient", targetObject.grassTransAmbient,0,1);
		targetObject.grassTransShadows = EditorGUILayout.Slider ("Trasnlucency Shadows ", targetObject.grassTransShadows,0,1);
		EditorGUILayout.Space ();
		targetObject.grassWindScale = EditorGUILayout.Slider ("Wind Scale", targetObject.grassWindScale,.03f,0.5f);
		targetObject.grassWindSpeed = EditorGUILayout.Slider ("Wind Speed", targetObject.grassWindSpeed,0,10);
		targetObject.grassWorldScale = EditorGUILayout.Slider ("World Scale", targetObject.grassWorldScale,0.03f,1f);

		if (grassColorRef != targetObject.grassColor || grassWindScaleRef != targetObject.grassWindScale
			|| grassWindSpeedRef != targetObject.grassWindSpeed || grassSpecularRef != targetObject.grassSpecular
			|| grassSmoothnessRef != targetObject.grassSmoothness || grassTransColorRef != targetObject.grassTransColor
			|| grassTransIntensityRef != targetObject.grassTransIntensity || grassTransAmbientRef != targetObject.grassTransAmbient
			|| grassTransShadowsRef != targetObject.grassTransShadows || grassWorldScaleRef != targetObject.grassWorldScale)
			targetObject.UpdateGrass ();
		
		GUILayout.EndVertical ();

		serializedObject.ApplyModifiedProperties ();
	}
}
