
// AliyerEdon@gmail.com/
// Lighting Box is an "paid" asset. Don't share it for free

#if UNITY_EDITOR   
using UnityEngine;   
using System.Collections;
using UnityEditor;
using SharpConfig;

[ExecuteInEditMode]
public class LB_CheckforUpdates : EditorWindow
{
	// Add menu named "My Window" to the Window menu
	[MenuItem("Assets/Lighting Box Updates")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		LB_CheckforUpdates window = (LB_CheckforUpdates)EditorWindow.GetWindow(typeof(LB_CheckforUpdates));
		window.Show();
		window.autoRepaintOnSceneChange = true;
	}
 

	void OnGUI()
	{		
			if (GUILayout.Button ("Check for updates")) 
			{
					downloadFinished = false;

					System.Net.WebClient client = new System.Net.WebClient ();
					client.DownloadFileAsync (new System.Uri ("http://89.163.206.23/LightingBox_Update_N37.txt"), Application.dataPath + "/"
				+ "LightingBox2/Resources/Config/LightingBox_Update_N37.txt");
					client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler (DownloadFileCompleted);
					EditorApplication.ExecuteMenuItem ("Assets/Refresh");
			}

			if (EditorPrefs.GetInt ("RateLB") != 3) {
				
				if (GUILayout.Button ("Rate Lighting Box")) {
					EditorPrefs.SetInt ("RateLB", 3);
					Application.OpenURL ("http://u3d.as/Se9");
				}
			}

			if (downloadFinished) 
			{
				if (System.IO.File.Exists (Application.dataPath + "/"
				+ "LightingBox2/Resources/Config/LightingBox_Update_N37.txt")) {
					Configuration lb_config = Configuration.LoadFromFile (Application.dataPath + "/"
					                         + "LightingBox2/Resources/Config/LightingBox_Config.txt");

					Configuration lb_update = Configuration.LoadFromFile (Application.dataPath + "/"
					+ "LightingBox2/Resources/Config/LightingBox_Update_N37.txt");


				Debug.Log (lb_update ["SampleProjects"] ["sampleVersions"].FloatValueArray);


					if (lb_update ["SampleProjects"] ["version"].FloatValue > lb_config ["SampleProjects"] ["version"].FloatValue) {
						EditorGUILayout.LabelField ("New Update is available : " + lb_update ["SampleProjects"] ["version"].FloatValue);

						EditorGUILayout.Space ();
						EditorGUILayout.Space ();

						if (GUILayout.Button ("Download Update " + lb_update ["SampleProjects"] ["version"].FloatValue.ToString ())) {
							Application.OpenURL ("http://u3d.as/Se9");
						}
					}
					EditorGUILayout.Space ();
					EditorGUILayout.Space ();

					for (int a = 0; a < lb_update ["SampleProjects"] ["sampleLinks"].StringValueArray.Length; a++) {
						if ((a + 1) > lb_config ["SampleProjects"] ["sampleLinks"].StringValueArray.Length) {
						if (GUILayout.Button (lb_update ["SampleProjects"] ["sampleNames"].StringValueArray [a])) {						
								Application.OpenURL (lb_update ["SampleProjects"] ["sampleLinks"].StringValueArray [a]);						
							}
						} else {
						if (GUILayout.Button (lb_update ["SampleProjects"] ["sampleNames"].StringValueArray [a])) {

								Application.OpenURL (lb_update ["SampleProjects"] ["sampleLinks"].StringValueArray [(a)]);						
							}
						}
					}
				}

			}
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
		}

	bool downloadFinished = true;
	void DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
	{
		if (e.Error == null)
			downloadFinished = true;
		else 
			Debug.Log (e.Error);
	}

}
#endif