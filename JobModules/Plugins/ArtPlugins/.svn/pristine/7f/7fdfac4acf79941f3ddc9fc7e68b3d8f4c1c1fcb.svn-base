#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;


// Hotkeys handler (this is static member, so you don't need to add this to scene, always works on   project)
[InitializeOnLoad]
public static class EditorHotkeysTracker
{
	static EditorHotkeysTracker()
	{
		SceneView.onSceneGUIDelegate += view =>
		{
			var e = Event.current;
			if (e != null && e.keyCode != KeyCode.None)
			{
				if(e.keyCode == KeyCode.B  &&  e.shift)
				{
					if (!Lightmapping.isRunning)
						Lightmapping.BakeAsync ();
				}
				if(e.keyCode == KeyCode.C  &&  e.shift)
				{
					if (Lightmapping.isRunning)
						Lightmapping.Cancel ();
				}
				if(e.keyCode == KeyCode.E  &&  e.shift)
				{
					if (!Lightmapping.isRunning)
						Lightmapping.Clear ();
				}
				if(e.keyCode == KeyCode.H  &&  e.shift)
				{
					EditorPrefs.SetInt(PlayerSettings.productName+SceneManager.GetActiveScene().name+"FirstTime",0);
				}
				if(e.keyCode == KeyCode.F  &&  e.alt && e.control)
				{
					EditorApplication.ExecuteMenuItem ("GameObject/Move To View");
				}
				if(e.keyCode == KeyCode.F  &&  e.shift)
				{
					EditorApplication.ExecuteMenuItem ("Window/Lighting/Settings");
				}
				if(e.keyCode == KeyCode.E  &&  e.shift)
				{
					EditorApplication.ExecuteMenuItem ("Lighting/Lighting Box");
				}
			}
		};
	}
}

#endif   