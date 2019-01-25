using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace MultiTerrain
{
	/// <summary>
	/// Multi terrain editor.
	/// </summary>
	public class MultiTerrainEditor : EditorWindow
	{


		#region variables editing terrain

		/// <summary>
		/// The first position for terrain tile management.
		/// </summary>
		public Vector2 firstPosition;

		/// <summary>
		/// The projector transform.
		/// </summary>
		Transform projectorTransform;
		/// <summary>
		/// The projector mouse.
		/// </summary>
		Projector projectorMouse;
		/// <summary>
		/// The projector material.
		/// </summary>
		Material projectorMaterial;
		/// <summary>
		/// The new terrain height.
		/// </summary>
		float newHeight = 5;

		/// <summary>
		/// The terrain max height.
		/// </summary>
		float maxHeight = 100;

		/// <summary>
		/// The drawing opacity.
		/// </summary>
		float opacity = 5;
		/// <summary>
		/// The range to be changed.
		/// </summary>
		int guiRange = 50;
		/// <summary>
		/// The draw range.
		/// </summary>
		int brushRange = 50;
		/// <summary>
		/// The current splat map.
		/// </summary>
		int currentSplatMap = 1;
		/// <summary>
		/// The size of the blur.
		/// </summary>
		float blurSize = 5;

		/// <summary>
		/// The brush texture.
		/// </summary>
		Texture2D brushTexture;
		/// <summary>
		/// The edit.
		/// </summary>
		bool edit = false;
		/// <summary>
		/// The selected method.
		/// </summary>
		int selectedMethod = 0;
		/// <summary>
		/// The _terrain dict.
		/// </summary>
		Dictionary<int[],Terrain> _terrainDict = null;
		/// <summary>
		/// The _terrains list.
		/// </summary>
		Terrain[] _terrains;
		/// <summary>
		/// The big picture to draw.
		/// </summary>
		Color[,] bigPicture;

		/// <summary>
		/// The terrain to height.
		/// </summary>
		float terrainToheight = 1;
		/// <summary>
		/// The terrain to width.
		/// </summary>
		float terrainTowidth = 1;

		//splatmaps
		/// <summary>
		/// The splats texture.
		/// </summary>
		List<Texture2D> splatsTexture = new List<Texture2D> ();
		List<Texture2D> splatsTextureNoAlpha = new List<Texture2D> ();
		/// <summary>
		/// The splat prototypes.
		/// </summary>
		List<SplatPrototype> splatPrototypes = new List<SplatPrototype> ();

		/// <summary>
		/// The label message important.
		/// </summary>
		string lbMessageImportant = "";
		/// <summary>
		/// The no terrain message.
		/// </summary>
		string noTerrainMessage = "No Terrain to edit in scene.";

		List<Terrain> terrainsChanged = new List<Terrain> ();


		#endregion

		#region variables gui

		/// <summary>
		/// The brush icons.
		/// </summary>
		GUIContent[] brushIcons;
		/// <summary>
		/// The selected brush icon.
		/// </summary>
		public int selbrushIcon = 0;
		/// <summary>
		/// The button style grid.
		/// </summary>
		GUIStyle buttonStyleGrid;
		/// <summary>
		/// The splat style grid.
		/// </summary>
		GUIStyle splatStyleGrid;
		/// <summary>
		/// The button editing style.
		/// </summary>
		GUIStyle buttonEditingStyle;

	
	
		#endregion

		/// <summary>
		/// Init this instance.
		/// </summary>
		[MenuItem ("Window/Multi Terrain Editor")]
		static void ShowWindow ()
		{

			EditorWindow.GetWindow (typeof(MultiTerrainEditor), false, "Multi Terrain");

		}

		/// <summary>
		/// Raises the focus event.
		/// </summary>
		void OnFocus ()
		{
			Undo.undoRedoPerformed -= this.CreateTerrainArrays;
			Undo.undoRedoPerformed += this.CreateTerrainArrays;
			Undo.undoRedoPerformed -= this.CreateTerrainAlphaArrays;
			Undo.undoRedoPerformed += this.CreateTerrainAlphaArrays;
			

			SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

			SceneView.onSceneGUIDelegate += this.OnSceneGUI;
			List<GUIContent> listIcon = new List<GUIContent> ();
			for (int i = 1; i <= 20; i++) {

				listIcon.Add (EditorGUIUtility.IconContent ("builtin_brush_" + i));

			}
			brushIcons = listIcon.ToArray ();
			if (projectorTransform == null) {
				GameObject projectorGO = GameObject.Find ("ProjectorParent");
				if (projectorGO != null)
					projectorTransform = projectorGO.transform;
			}

		}

		/// <summary>
		/// Raises the lost focus event.
		/// </summary>
		void OnLostFocus ()
		{
		
			if (projectorTransform != null)
				DestroyProjector ();
		}

		/// <summary>
		/// Raises the destroy event.
		/// </summary>
		void OnDestroy ()
		{
			Undo.undoRedoPerformed -= this.CreateTerrainArrays;
			Undo.undoRedoPerformed -= this.CreateTerrainAlphaArrays;
			
			SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
			if (projectorTransform != null)
				DestroyProjector ();
		}

		//////////////////////////New Version

		float[,] terrainDataArray;
		float[,,] terrainAlphaArray;
		Terrain[,] terrainArray;
		int terrainWidth;
		int terrainWidthAlpha;
		float xMin = float.MaxValue;
		float zMin = float.MaxValue;

		void CreateTerrainArrays ()
		{
			//znajdujemy dolny lewy rog
			//znajdujemy prawy gorny rog
			//znajdujemy liczbe terenow w x oraz z
			// tworzymy tablice terenow i floatow
			// uzupelniamy tablice float wartoscia -1
			// szukamy lewego dolnego
			// 

			//Debug.Log ("create terrain arrays");

			Terrain[] terrains = Terrain.activeTerrains;
			if (terrains.Length == 0)
				return;

			List<Terrain> terrainsTocheck = new List<Terrain> ();
			terrainsTocheck.AddRange (terrains);

		

			terrainWidth = terrains [0].terrainData.heightmapHeight;
			float heightToPaint = _terrains [0].terrainData.alphamapHeight / (float)_terrains [0].terrainData.heightmapHeight;
			terrainToheight = (1 / _terrains [0].terrainData.size.x * terrainWidth) * (selectedMethod == 4 ? heightToPaint : 1);
			terrainTowidth = (1 / _terrains [0].terrainData.size.z * terrainWidth) * (selectedMethod == 4 ? heightToPaint : 1);

			int terrainsX;
			int terrainsZ;
			xMin = float.MaxValue;
			zMin = float.MaxValue;
		
			float xMax = float.MinValue;
			float zMax = float.MinValue;
			
			foreach (var item in terrains) {

				if (item.transform.position.x < xMin)
					xMin = item.transform.position.x;
				if (item.transform.position.z < zMin)
					zMin = item.transform.position.z;

				if (item.transform.position.x + item.terrainData.size.x > xMax)
					xMax = item.transform.position.x + item.terrainData.size.x;
				if (item.transform.position.z + item.terrainData.size.z > zMax)
					zMax = item.transform.position.z + item.terrainData.size.z;



			}


			//Debug.Log (xMin + " " + xMax + " " + zMin + " " + zMax);

			terrainsX = (int)((xMax - xMin) / terrains [0].terrainData.size.x);
			terrainsZ = (int)((zMax - zMin) / terrains [0].terrainData.size.z);




			terrainDataArray = new float[terrainWidth * terrainsX - terrainsX + 1, terrainWidth * terrainsZ - terrainsZ + 1];
			terrainArray = new Terrain[terrainsX, terrainsZ];

			for (int x = 0; x < terrainDataArray.GetLength (0); x++) {
				for (int z = 0; z < terrainDataArray.GetLength (1); z++) {
					terrainDataArray [x, z] = -1;
				}
			}

			for (int xStep = 0; xStep < terrainArray.GetLength (0); xStep++) {
				for (int zStep = 0; zStep < terrainArray.GetLength (1); zStep++) {
					float xPosTerrain = xStep * terrains [0].terrainData.size.x + xMin;
					float zPosTerrain = zStep * terrains [0].terrainData.size.z + zMin;

					Terrain terrainFound = null;

					foreach (var item in terrainsTocheck) {
                        // if (item.transform.position.x == xPosTerrain && item.transform.position.z == zPosTerrain) {
						// 适配生死2，修复浮点数导致的地形不能修改bug
                        if (Mathf.RoundToInt(item.transform.position.x) == Mathf.RoundToInt(xPosTerrain) && Mathf.RoundToInt(item.transform.position.z) == Mathf.RoundToInt(zPosTerrain)){
							terrainFound = item;
							break;
						}
					}


					if (terrainFound != null) {
						
						terrainsTocheck.Remove (terrainFound);
						terrainArray [xStep, zStep] = terrainFound;

						float[,] heights = terrainFound.terrainData.GetHeights (0, 0, terrainWidth, terrainWidth);
						int x = 0;
						int z = 0;
						for (x = 0; x < terrainWidth; x++) {
							for (z = 0; z < terrainWidth; z++) {
								terrainDataArray [xStep * terrainWidth + x - xStep, zStep * terrainWidth + z - zStep] = heights [z, x];
							}
						}
						

					}

				}
			}

		}




		void DataToTerrains (int xStart, int xEnd, int zStart, int zEnd)
		{
//			string array = "";
//			for (int i = terrainDataArray.GetLength (1) - 1; i >= 0; i--) {
//				for (int j = 0; j < terrainDataArray.GetLength (0); j++) {
//					array += (int)(terrainDataArray [j, i] * 9);
//				}
//				array += '\n';
//			}
//
//			Debug.Log (array);


			int xStepStart = xStart / terrainWidth;
			int zStepStart = zStart / terrainWidth;
			int xStepEnd = xEnd / terrainWidth;
			int zStepEnd = zEnd / terrainWidth;

			for (int xStep = xStepStart; xStep <= xStepEnd; xStep++) {
				for (int zStep = zStepStart; zStep <= zStepEnd; zStep++) {
					Terrain tt = terrainArray[xStep,zStep];
					if (terrainArray.GetLength (0) > xStep && terrainArray.GetLength (1) > zStep && terrainArray [xStep, zStep] != null) {
						
						if (!terrainsChanged.Contains (terrainArray [xStep, zStep])) {

							terrainsChanged.Add (terrainArray [xStep, zStep]);

							Undo.RegisterCompleteObjectUndo (terrainArray [xStep, zStep].terrainData, "Terrain draw end");

							if (selectedMethod == 4) {
								Undo.RegisterCompleteObjectUndo (terrainArray [xStep, zStep], "Terrain draw texture");
								Undo.RegisterCompleteObjectUndo (terrainArray [xStep, zStep].terrainData.alphamapTextures, "alpha");
							}
						}		
					

						int xStartPart = Mathf.Clamp (xStart - (terrainWidth - 1) * xStep, 0, terrainWidth);
						int zStartPart = Mathf.Clamp (zStart - (terrainWidth - 1) * zStep, 0, terrainWidth);
											
						int xEndPart = Mathf.Clamp (xEnd - (terrainWidth - 1) * xStep, 0, terrainWidth);
						int zEndPart = Mathf.Clamp (zEnd - (terrainWidth - 1) * zStep, 0, terrainWidth);


						float[,] heights = new float[zEndPart - zStartPart, xEndPart - xStartPart];


						for (int x = xStartPart; x < xEndPart; x++) {			
							for (int z = zStartPart; z < zEndPart; z++) {		
								
								heights [z - zStartPart, x - xStartPart] = terrainDataArray [x + (terrainWidth - 1) * xStep, z + (terrainWidth - 1) * zStep]; 
								
							}
						}


						terrainArray [xStep, zStep].terrainData.SetHeightsDelayLOD (xStartPart, zStartPart, heights);
						terrainArray [xStep, zStep].Flush ();

	
					}

				}
			}

		
		}



		void AllDataToTerrains ()
		{
			for (int xStep = 0; xStep < terrainArray.GetLength (0); xStep++) {
				for (int zStep = 0; zStep < terrainArray.GetLength (1); zStep++) {
					
					if (terrainArray [xStep, zStep] != null) {
						
						float[,] heights = new float[terrainWidth, terrainWidth];

						for (int x = 0; x < terrainWidth; x++) {

							for (int z = 0; z < terrainWidth; z++) {
								heights [z, x] = terrainDataArray [xStep * terrainWidth + x - xStep, zStep * terrainWidth + z - zStep]; 

							}
						}

						terrainArray [xStep, zStep].terrainData.SetHeightsDelayLOD (0, 0, heights);
						terrainArray [xStep, zStep].Flush ();
						terrainArray [xStep, zStep].ApplyDelayedHeightmapModification ();
					}

				}
			}


		}


		void CreateTerrainAlphaArrays ()
		{


			Terrain[] terrains = Terrain.activeTerrains;

			if (terrains.Length == 0)
				return;

			List<Terrain> terrainsTocheck = new List<Terrain> ();
			terrainsTocheck.AddRange (terrains);

			xMin = float.MaxValue;
			zMin = float.MaxValue;
			float xMax = float.MinValue;
			float zMax = float.MinValue;

			terrainWidthAlpha = terrains [0].terrainData.alphamapHeight;
			int terrainsX;
			int terrainsZ;

			foreach (var item in terrains) {

				if (item.transform.position.x < xMin)
					xMin = item.transform.position.x;
				if (item.transform.position.z < zMin)
					zMin = item.transform.position.z;

				if (item.transform.position.x + item.terrainData.size.x > xMax)
					xMax = item.transform.position.x + item.terrainData.size.x;
				if (item.transform.position.z + item.terrainData.size.z > zMax)
					zMax = item.transform.position.z + item.terrainData.size.z;



			}


			terrainsX = (int)((xMax - xMin) / terrains [0].terrainData.size.x);
			terrainsZ = (int)((zMax - zMin) / terrains [0].terrainData.size.z);

			//map [i, h, currentSplatMap]
			terrainAlphaArray = new float[terrainWidthAlpha * terrainsX - terrainsX + 1, terrainWidthAlpha * terrainsZ - terrainsZ + 1, splatPrototypes.Count];

			terrainArray = new Terrain[terrainsX, terrainsZ];

			for (int x = 0; x < terrainAlphaArray.GetLength (0); x++) {
				for (int z = 0; z < terrainAlphaArray.GetLength (1); z++) {
					for (int l = 0; l < terrainAlphaArray.GetLength (2); l++) {
						terrainAlphaArray [x, z, l] = 0;
					}
				
				}
			}

			for (int xStep = 0; xStep < terrainArray.GetLength (0); xStep++) {
				for (int zStep = 0; zStep < terrainArray.GetLength (1); zStep++) {

					float xPosTerrain = xStep * terrains [0].terrainData.size.x + xMin;
					float zPosTerrain = zStep * terrains [0].terrainData.size.z + zMin;

					Terrain terrainFound = null;
					foreach (var item in terrainsTocheck) {
                        // if (item.transform.position.x == xPosTerrain && item.transform.position.z == zPosTerrain) {
						// 适配生死2，修复浮点数导致的地形不能修改bug
                        if (Mathf.RoundToInt(item.transform.position.x) == Mathf.RoundToInt(xPosTerrain) && Mathf.RoundToInt(item.transform.position.z) == Mathf.RoundToInt(zPosTerrain)){
							terrainFound = item;
							break;
						}
					}

					if (terrainFound != null) {
						terrainsTocheck.Remove (terrainFound);
						terrainArray [xStep, zStep] = terrainFound;
						//Debug.Log (terrainFound.name);

						float[,,] alphas = terrainFound.terrainData.GetAlphamaps (0, 0, terrainWidthAlpha, terrainWidthAlpha);
						int x = 0;
						int z = 0;

						for (int l = 0; l < Mathf.Min (alphas.GetLength (2), splatPrototypes.Count); l++) {
							for (x = 0; x < terrainWidthAlpha; x++) {
								for (z = 0; z < terrainWidthAlpha; z++) {
									
									terrainAlphaArray [xStep * terrainWidthAlpha + x - xStep, zStep * terrainWidthAlpha + z - zStep, l] = alphas [z, x, l];

								}
							}


						}
					}
				}
			}

		}

		void DataToTerrainsAlphas (int xStart, int xEnd, int zStart, int zEnd)
		{


			int xStepStart = xStart / terrainWidthAlpha;
			int zStepStart = zStart / terrainWidthAlpha;
			int xStepEnd = xEnd / terrainWidthAlpha;
			int zStepEnd = zEnd / terrainWidthAlpha;

			for (int xStep = xStepStart; xStep <= xStepEnd; xStep++) {
				for (int zStep = zStepStart; zStep <= zStepEnd; zStep++) {


					if (terrainArray.GetLength (0) > xStep && terrainArray.GetLength (1) > zStep && terrainArray [xStep, zStep] != null) {
						if (terrainArray [xStep, zStep].terrainData.alphamapLayers > currentSplatMap) {
							if (!terrainsChanged.Contains (terrainArray [xStep, zStep])) {

								terrainsChanged.Add (terrainArray [xStep, zStep]);

								Undo.RegisterCompleteObjectUndo (terrainArray [xStep, zStep].terrainData, "Terrain draw end");

								if (selectedMethod == 4) {
									Undo.RegisterCompleteObjectUndo (terrainArray [xStep, zStep], "Terrain draw texture");
									Undo.RegisterCompleteObjectUndo (terrainArray [xStep, zStep].terrainData.alphamapTextures, "alpha");
								}
							}



							int xStartPart = Mathf.Clamp (xStart - (terrainWidthAlpha - 1) * xStep, 0, terrainWidthAlpha);
							int zStartPart = Mathf.Clamp (zStart - (terrainWidthAlpha - 1) * zStep, 0, terrainWidthAlpha);

							int xEndPart = Mathf.Clamp (xEnd - (terrainWidthAlpha - 1) * xStep, 0, terrainWidthAlpha);
							int zEndPart = Mathf.Clamp (zEnd - (terrainWidthAlpha - 1) * zStep, 0, terrainWidthAlpha);


							float[,,] alphas = new float[zEndPart - zStartPart, xEndPart - xStartPart, terrainArray [xStep, zStep].terrainData.alphamapLayers ];



							for (int x = xStartPart; x < xEndPart; x++) {
								for (int z = zStartPart; z < zEndPart; z++) {
								
									for (int l = 0; l < alphas.GetLength (2); l++) {
										alphas [z - zStartPart, x - xStartPart, l] = terrainAlphaArray [x + (terrainWidthAlpha - 1) * xStep, z + (terrainWidthAlpha - 1) * zStep, l];

									}
							
								}
							}

					

							terrainArray [xStep, zStep].terrainData.SetAlphamaps (xStartPart, zStartPart, alphas);
							terrainArray [xStep, zStep].Flush ();


						}
					}
				}

			}

		}

		////////////////////////////////
		/// 
		void ExitEditMode ()
		{
			edit = false;
			splatPrototypes.Clear ();
			splatsTexture.Clear ();
			splatsTextureNoAlpha.Clear ();
			if (projectorTransform != null)
				DestroyProjector ();
		}

		/// <summary>
		/// Raises the GU event.
		/// </summary>
		void OnGUI ()
		{
          
			if (buttonStyleGrid == null) {
				buttonStyleGrid = new GUIStyle (GUI.skin.GetStyle ("GridList"));
			
			
				buttonStyleGrid.margin.right = 2;
				buttonStyleGrid.margin.left = 2;
				buttonStyleGrid.padding.right = 0;
				buttonStyleGrid.padding.left = 0;
				buttonStyleGrid.padding.top = 0;
				buttonStyleGrid.padding.bottom = 0;
				buttonStyleGrid.fixedWidth = 30;
				buttonStyleGrid.fixedHeight = 30;
				buttonStyleGrid.stretchWidth = false;
				buttonStyleGrid.stretchHeight = false;
			
			}
		
			if (splatStyleGrid == null) {
			
			
				splatStyleGrid = new GUIStyle (GUI.skin.GetStyle ("GridList"));
			
				splatStyleGrid.margin.right = 4;
				splatStyleGrid.margin.left = 4;
				splatStyleGrid.padding.right = 0;
				splatStyleGrid.padding.left = 0;
				splatStyleGrid.padding.top = 0;
				splatStyleGrid.padding.bottom = 4;
				splatStyleGrid.fixedWidth = 60;
				splatStyleGrid.fixedHeight = 60;

			
			}

		
			buttonEditingStyle = new GUIStyle (GUI.skin.GetStyle ("Button"));
			buttonEditingStyle.fontStyle = FontStyle.Bold;
			buttonEditingStyle.fontSize = 15;

			EditorGUILayout.Space ();

			if (EditorApplication.isPlaying) {
				ExitEditMode ();

			}

			if (edit) {
				if (_terrainDict == null || _terrains.Length < 1 || _terrains [0] == null || terrainDataArray == null) {
					GetTerrains ();
					CreateTerrainArrays ();
					if (splatPrototypes.Count > 0)
						CreateTerrainAlphaArrays ();
					TextureCreate ();
				}
			}


			if (!edit) {
				GUI.color = Color.green;


				if (!EditorApplication.isPlaying) {
					if (GUILayout.Button ("Start editing", buttonEditingStyle, GUILayout.Height (30))) {
						
						edit = true;
						Repaint ();
						Selection.objects = new UnityEngine.Object[0]{ };
						GetTerrains ();

						CreateTerrainArrays ();
						if (splatPrototypes.Count > 0)
							CreateTerrainAlphaArrays ();

						int brushScaledRangeX = (int)(brushRange * terrainTowidth);
						int brushScaledRangeZ = (int)(brushRange * terrainToheight);



						selbrushIcon = 0;
					
						ScaleTexture ((Texture2D)brushIcons [selbrushIcon].image, brushScaledRangeX, brushScaledRangeZ);

						Color[] colors = brushTexture.GetPixels ();

						if (colors.Length != brushScaledRangeX * brushScaledRangeZ) {
							ScaleTexture ((Texture2D)brushIcons [selbrushIcon].image, brushScaledRangeX, brushScaledRangeZ);
							colors = brushTexture.GetPixels ();
						}

						bigPicture = new Color[brushScaledRangeX, brushScaledRangeZ];

						for (int i = 0; i < brushScaledRangeX; i++) {

							for (int h = 0; h < brushScaledRangeZ; h++) {

								bigPicture [i, h] = colors [i + h * brushScaledRangeX];

							}

						}

							

					}
				} else if (GUILayout.Button ("Stop Play to Edit", buttonEditingStyle, GUILayout.Height (30))) {
				}
			} else if (edit) {

			
				Color baseCol = GUI.color;
				GUI.color = Color.red;

				if (GUILayout.Button ("Stop editing", buttonEditingStyle, GUILayout.Height (30))) {

					ExitEditMode ();
				}
				GUI.color = baseCol;
			 
        


			    EditorGUILayout.Space ();

				EditorGUILayout.BeginHorizontal ();


		
				GUILayout.FlexibleSpace ();

				GUIStyle buttonStyle;
				buttonStyle = new GUIStyle (UnityEditor.EditorStyles.miniButtonLeft);
				if (selectedMethod == 0)
					buttonStyle.normal.background = buttonStyle.onActive.background;

			
				if (GUILayout.Button (EditorGUIUtility.IconContent ("TerrainInspector.TerrainToolRaise"), buttonStyle)) {
				
					selectedMethod = 0;
				}

				buttonStyle = new GUIStyle (UnityEditor.EditorStyles.miniButtonMid);
				if (selectedMethod == 1)
					buttonStyle.normal.background = buttonStyle.onActive.background;
				if (GUILayout.Button (EditorGUIUtility.IconContent ("TerrainInspector.TerrainToolSetheight"), buttonStyle)) {
				
					selectedMethod = 1;
				}

				buttonStyle = new GUIStyle (UnityEditor.EditorStyles.miniButtonMid);
				if (selectedMethod == 2)
					buttonStyle.normal.background = buttonStyle.onActive.background;
				if (GUILayout.Button (EditorGUIUtility.IconContent ("TerrainInspector.TerrainToolSmoothheight"), buttonStyle)) {
				
					selectedMethod = 2;
				}

				buttonStyle = new GUIStyle (UnityEditor.EditorStyles.miniButtonRight);
				if (selectedMethod == 4)
					buttonStyle.normal.background = buttonStyle.onActive.background;
			
			
				if (GUILayout.Button (EditorGUIUtility.IconContent ("TerrainInspector.TerrainToolSplat"), buttonStyle)) {
				
					selectedMethod = 4;
				}
				GUILayout.FlexibleSpace ();
				EditorGUILayout.EndHorizontal ();

			
				GUILayout.Label ("Brushes", EditorStyles.boldLabel);

				EditorGUILayout.BeginHorizontal ("Box");
		


				selbrushIcon = GUILayout.SelectionGrid (selbrushIcon, brushIcons, Screen.width / 32, buttonStyleGrid, GUILayout.Width (Screen.width - 20));



			
				EditorGUILayout.EndHorizontal ();



				EditorGUILayout.Space ();


				if (selectedMethod == 4) {

					//////SplatMaps
				
					GUILayout.Label ("Textures", EditorStyles.boldLabel);
					EditorGUILayout.BeginHorizontal ("Box");
				
				
				 
					currentSplatMap = GUILayout.SelectionGrid (currentSplatMap, splatsTextureNoAlpha.ToArray (), Screen.width / 64, splatStyleGrid, GUILayout.Width (Screen.width - 20));
					//EditorGUI.DrawPreviewTexture (new Rect (100, 100, 100, 100), splatsTexture [2]);


					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					if (GUILayout.Button ("Add Texture")) {
				
						SplatTextureEditor splatTextureEditor = (SplatTextureEditor)EditorWindow.GetWindow (typeof(SplatTextureEditor), false, "Splat Texture Editor");
						splatTextureEditor.add = true;
						splatTextureEditor.multiTerrainEditor = this;

					}

					if (GUILayout.Button ("Edit Texture")) {
					
						SplatTextureEditor splatTextureEditor = (SplatTextureEditor)EditorWindow.GetWindow (typeof(SplatTextureEditor), false, "Splat Texture Editor");
						splatTextureEditor.add = false;
						splatTextureEditor.multiTerrainEditor = this;
						splatTextureEditor.splatTexture = splatsTexture [currentSplatMap];
						splatTextureEditor.normalTexture = splatPrototypes [currentSplatMap].normalMap;
						splatTextureEditor.tileSize = splatPrototypes [currentSplatMap].tileSize;
						splatTextureEditor.tileOffset = splatPrototypes [currentSplatMap].tileOffset;
						splatTextureEditor.metallic = splatPrototypes [currentSplatMap].metallic;
						splatTextureEditor.smoothness = splatPrototypes [currentSplatMap].smoothness;
					}
					if (GUILayout.Button ("Remove Texture")) {
						RemoveSplatPrototype ();
					}
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.Space ();
			


					if (splatsTexture == null) {
						splatsTexture = new List<Texture2D> ();
						splatsTextureNoAlpha = new List<Texture2D> ();
					}
				

			

					if (GUILayout.Button ("Get splat textures from selected terrains")) {
						GetSplatFromSelected ();
					}

					if (GUILayout.Button ("Set splat textures in selected terrains")) {
						SetSelectedTerrainSplatPrototypes ();
					}

					if (GUILayout.Button ("Set splat textures in all terrains")) {

						if (EditorUtility.DisplayDialog ("Confirmation", "Do You want to set  splat textures to all terrains?", "Yes", "No"))
							SetTerrainSplatPrototypes ();
					}
				
					EditorGUILayout.Space ();
				
				}

			
				EditorGUILayout.Space ();
				GUILayout.Label ("Settings", EditorStyles.boldLabel);

				EditorGUILayout.BeginHorizontal ("Label");
				guiRange = EditorGUILayout.IntSlider ("Brush Size", (int)guiRange, 1, 200);
				brushRange = (guiRange % 2 == 1) ? guiRange + 1 : guiRange;
				EditorGUILayout.EndHorizontal ();

				if (selectedMethod != 0) {
					EditorGUILayout.BeginHorizontal ("Label");
					opacity = EditorGUILayout.Slider ("Opacity", opacity, 0.1f, 100);
					
					EditorGUILayout.EndHorizontal ();

				}

				if (selectedMethod == 0) {
					EditorGUILayout.BeginHorizontal ("Label");
					newHeight = EditorGUILayout.Slider ("Height", newHeight, 0.1f, 100);
				
					EditorGUILayout.EndHorizontal ();
				}
			
				if (selectedMethod == 1) {
					EditorGUILayout.BeginHorizontal ("Label");
					newHeight = EditorGUILayout.Slider ("Height", newHeight, 0, maxHeight);
					if (GUILayout.Button ("Flatten", GUILayout.Width (50))) {
						if (EditorUtility.DisplayDialog ("Confirmation", "Do You want to flatten all terrains, undo maybe impossible?", "Yes", "No"))
							Flatten ();
					}
				
					EditorGUILayout.EndHorizontal ();
				}

				int brushScaledRangeX = (int)(brushRange * terrainTowidth);
				int brushScaledRangeZ = (int)(brushRange * terrainToheight);
		

				ScaleTexture ((Texture2D)brushIcons [selbrushIcon].image, brushScaledRangeX, brushScaledRangeZ);


			}
			if (!string.IsNullOrEmpty (lbMessageImportant)) {
				EditorGUILayout.Space ();

				EditorGUILayout.BeginHorizontal ("Label");

				GUIStyle style = new GUIStyle (EditorStyles.boldLabel);
				style.alignment = TextAnchor.UpperCenter;
				style.normal.textColor = Color.red;
				style.fontSize = 14;
				style.fixedHeight = 50;

				EditorGUILayout.LabelField (lbMessageImportant, style);

				EditorGUILayout.EndHorizontal ();
			}
		}

		/// <summary>
		/// Destroies the projector.
		/// </summary>
		void DestroyProjector ()
		{

			DestroyImmediate (brushTexture);
			DestroyImmediate (projectorMaterial);

			DestroyImmediate (projectorTransform.gameObject);
			System.GC.Collect ();
			Resources.UnloadUnusedAssets ();
		
		}

		/// <summary>
		/// Sets the terrain splat prototypes.
		/// </summary>
		public void SetTerrainSplatPrototypes ()
		{

			if (_terrains.Length > 0) {
				foreach (var t in _terrains) {
					TerrainData data = t.terrainData;

					float[,,] newMap = ClearAlphaMaps (t);

					data.splatPrototypes = splatPrototypes.ToArray ();
					data.RefreshPrototypes ();

					if (newMap.GetLength (2) == t.terrainData.alphamapLayers)
						t.terrainData.SetAlphamaps (0, 0, newMap);

					t.Flush ();
				}

				CreateTerrainAlphaArrays ();
			}
		}

		/// <summary>
		/// Sets the selected terrains splat prototypes.
		/// </summary>
		public void SetSelectedTerrainSplatPrototypes ()
		{
			List<Terrain> terrainsSelected = new List<Terrain> ();

			foreach (var item in Selection.gameObjects) {
				Terrain terrain = item.GetComponent<Terrain> ();
				if (terrain != null)
					terrainsSelected.Add (terrain);
			}
					
			if (terrainsSelected.Count > 0) {
				foreach (var t in terrainsSelected) {
					TerrainData data = t.terrainData;
					float[,,] newMap = ClearAlphaMaps (t);
					data.splatPrototypes = splatPrototypes.ToArray ();

					data.RefreshPrototypes ();

					if (newMap.GetLength (2) == t.terrainData.alphamapLayers)
						t.terrainData.SetAlphamaps (0, 0, newMap);

					t.Flush ();
				}

				CreateTerrainAlphaArrays ();
			}
		}

		public float[,,] ClearAlphaMaps (Terrain t)
		{
			float[,,] map = t.terrainData.GetAlphamaps (0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);
	
			if (splatPrototypes.Count == 0 || splatPrototypes.Count >= t.terrainData.alphamapLayers) {
				return map;
			}

			float[,,] newMap = new float[t.terrainData.alphamapWidth, t.terrainData.alphamapHeight, splatPrototypes.Count];

			for (int i = 0; i < splatPrototypes.Count; i++) {
				for (int y = 0; y < t.terrainData.alphamapHeight; y++) {
					for (int x = 0; x < t.terrainData.alphamapWidth; x++) {
						newMap [x, y, i] = map [x, y, i];
					}
				}
			}

			for (int y = 0; y < t.terrainData.alphamapHeight; y++) {
				for (int x = 0; x < t.terrainData.alphamapWidth; x++) {
					for (int i = splatPrototypes.Count; i < t.terrainData.alphamapLayers; i++) {

						newMap [x, y, 0] += map [x, y, i];
						if (newMap [x, y, splatPrototypes.Count - 1] > 1)
							newMap [x, y, splatPrototypes.Count - 1] = 1;
					}
				}
			}

			return newMap;

		}

		void CopyTextureSplatTexturesNoAlpha (Texture2D texture)
		{
			Texture2D text = new Texture2D (texture.width, texture.height, texture.format, true);
			text.LoadRawTextureData (texture.GetRawTextureData ());



			Color[] pix = text.GetPixels (0, 0, texture.width, texture.height);
			for (int i = 0; i < pix.Length; i++) {
				pix [i].a = 1;
			}

			text = new Texture2D (texture.width, texture.height, TextureFormat.RGB24, true);
			text.SetPixels (pix);
			text.Apply ();


			splatsTextureNoAlpha.Add (text);
		}

		/// <summary>
		/// Adds the splat prototype.
		/// </summary>
		/// <param name="prototype">Prototype.</param>
		public void AddSplatPrototype (SplatPrototype prototype)
		{
			splatPrototypes.Add (prototype);
			splatsTexture.Clear ();
			splatsTextureNoAlpha.Clear ();
			foreach (var item in splatPrototypes) {
				splatsTexture.Add (item.texture);
				CopyTextureSplatTexturesNoAlpha (item.texture);
			}
			CreateTerrainAlphaArrays ();
		}

		/// <summary>
		/// Edits the splat prototype.
		/// </summary>
		/// <param name="prototype">Prototype.</param>
		public void EditSplatPrototype (SplatPrototype prototype)
		{
			splatPrototypes [currentSplatMap] = prototype;
			splatsTexture.Clear ();
			splatsTextureNoAlpha.Clear ();
			foreach (var item in splatPrototypes) {

				splatsTexture.Add (item.texture);
				CopyTextureSplatTexturesNoAlpha (item.texture);
			}
		}

		/// <summary>
		/// Remove the splat prototype.
		/// </summary>
		/// <param name="prototype">Prototype.</param>
		public void RemoveSplatPrototype ()
		{
			if (splatPrototypes.Count < currentSplatMap)
				return;
			splatPrototypes.Remove (splatPrototypes [currentSplatMap]);

			splatsTexture.Clear ();
			splatsTextureNoAlpha.Clear ();
			foreach (var item in splatPrototypes) {
				splatsTexture.Add (item.texture);
				CopyTextureSplatTexturesNoAlpha (item.texture);
			}
			CreateTerrainAlphaArrays ();
		}

		/// <summary>
		/// Gets the splat from selected.
		/// </summary>
		void GetSplatFromSelected ()
		{
			if (Selection.activeGameObject == null)
				return;
			Terrain terrain = Selection.activeGameObject.GetComponent<Terrain> ();
			if (terrain != null) {
				splatPrototypes.Clear ();
				splatPrototypes.AddRange (terrain.terrainData.splatPrototypes);
				splatsTexture.Clear ();
				splatsTextureNoAlpha.Clear ();
				foreach (var item in splatPrototypes) {
					splatsTexture.Add (item.texture);
					CopyTextureSplatTexturesNoAlpha (item.texture);
				}
				CreateTerrainAlphaArrays ();
			}
		}






		/// <summary>
		/// Raises the scene GU event.
		/// </summary>
		/// <param name="sceneView">Scene view.</param>
		public void OnSceneGUI (SceneView sceneView)
		{


			if (edit) {

				if (_terrainDict == null || _terrains.Length < 1 || _terrains [0] == null) {
					GetTerrains ();
					return;
				}
			    if (Event.current.type == EventType.keyDown)

			    {

			        if (Event.current.keyCode == KeyCode.Z)
			            selectedMethod = 0;
			        if (Event.current.keyCode == KeyCode.X)
			            selectedMethod = 1;
			        if (Event.current.keyCode == KeyCode.C)
			            selectedMethod = 2;
                    if (Event.current.keyCode == KeyCode.V)
			            selectedMethod = 4;
 
                    int addBrushRange = 0;
			   
			        if (Event.current.keyCode == KeyCode.RightBracket)
			        {
			            addBrushRange = 1;
             
                    }
			            
			        if (Event.current.keyCode == KeyCode.LeftBracket)
			        {
			            addBrushRange = -1;

                    }
			        if (addBrushRange != 0)
			        {
			            int step= guiRange/10+1;
			            guiRange += step * addBrushRange ;
			            guiRange = Mathf.Clamp(guiRange, 1, 200);
                        brushRange = (guiRange % 2 == 1) ? guiRange + 1 : guiRange;
                      
                    }

                    int addHeight = 0;
			        if (Event.current.keyCode == KeyCode.Equals)
			        {
			            addHeight = 1;

			        }

			        if (Event.current.keyCode == KeyCode.Minus)
			        {
			            addHeight = -1;

			        }
			        if (addHeight != 0)
			        {
			            float step = newHeight / 10 + 0.1f;
			            newHeight += step * addHeight;
			            newHeight = Mathf.Clamp(newHeight, 0.1f, 100f);
			         
                    }
 

                }
			    GUILayout.BeginArea(new Rect(0, 0, 100, 50));
			    GUILayout.Label("bursh size:" + guiRange);
                GUILayout.Label("height:"+newHeight.ToString("F1"));
            
			    GUILayout.EndArea();
       
                HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));
				Camera sceneCamera = SceneView.lastActiveSceneView.camera;
				Vector2 mousePos = Event.current.mousePosition;
				mousePos.y = Screen.height - mousePos.y - 40;
				Ray ray = sceneCamera.ScreenPointToRay (mousePos);

				RaycastHit[] hits = Physics.RaycastAll (ray, Mathf.Infinity);




				if (hits.Length > 0) {
					Terrain terrain = null;
					Vector3 brushPos = Vector3.zero;
					foreach (var hit in hits) {
						if (hit.collider is TerrainCollider) {
							brushPos = hit.point;
							terrain = hit.collider.GetComponent<Terrain> ();
							break;
						}
					}

					if (!terrain)
						return;

					if (projectorTransform == null) {
						GameObject projGO = GameObject.Find ("ProjectorParent");

						if (projGO != null)
							projectorTransform = projGO.transform;
						else
							projectorTransform = new GameObject ("ProjectorParent").transform;	
						GameObject child = new GameObject ("Projector");
						child.transform.parent = projectorTransform;
						child.transform.localPosition = new Vector3 (0, 20000, 0);
						child.transform.eulerAngles = new Vector3 (90, -90, 0);
						projectorMouse = child.AddComponent<Projector> ();
						projectorMouse.farClipPlane = 30000;
						projectorMouse.nearClipPlane = 1000;
						projectorMouse.orthographicSize = 100;
						projectorMouse.orthographic = true;
						if (projectorMaterial == null) {
							projectorMaterial = new Material (Shader.Find ("Projector/Projector MultiplyMine"));

						}
						projectorMaterial.color = new Color32 (0, 86, 255, 0);
						projectorMouse.material = projectorMaterial;
				

					} else {

						if (projectorMouse && projectorMaterial != null) {
							projectorMaterial.SetTexture ("_ShadowTex", brushTexture);
							projectorMouse.orthographicSize = brushRange * 0.5f;

					
						}
					
					}

					projectorTransform.position = brushPos;
					//projectorTransform.hideFlags = HideFlags.HideAndDontSave;

					if (Event.current.alt)
						return;

					if (Event.current.type == EventType.MouseUp && Event.current.button == 0) {

						foreach (var item in _terrains) {
							item.ApplyDelayedHeightmapModification ();
							
						}
						terrainsChanged.Clear ();
						return;

					}

				
					if (!(Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) || Event.current.button != 0)
						return;

					if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
//						System.GC.Collect ();
//						Resources.UnloadUnusedAssets ();					

					}
				
				
					TextureCreate ();
				
					if (brushTexture.width != 0) {
						
						DrawOnTerrain (brushPos, terrain);
						
					}
				
				}

			}
		}

		void TextureCreate ()
		{
			int brushScaledRangeX = (int)(brushRange * terrainTowidth);
			int brushScaledRangeZ = (int)(brushRange * terrainToheight);
			if (brushTexture == null || brushTexture.width == 0)
				ScaleTexture ((Texture2D)brushIcons [selbrushIcon].image, brushScaledRangeX, brushScaledRangeZ);
			if (brushTexture.width != 0) {
				Color[] colors = brushTexture.GetPixels ();
				if (colors.Length != brushScaledRangeX * brushScaledRangeZ) {
					ScaleTexture ((Texture2D)brushIcons [selbrushIcon].image, brushScaledRangeX, brushScaledRangeZ);
					colors = brushTexture.GetPixels ();
				}
				bigPicture = new Color[brushScaledRangeX, brushScaledRangeZ];
				for (int i = 0; i < brushScaledRangeX; i++) {
					for (int h = 0; h < brushScaledRangeZ; h++) {
						bigPicture [i, h] = colors [i + h * brushScaledRangeX];
					}
				}
			}
		}

		/// <summary>
		/// Scales the texture.
		/// </summary>
		/// <returns>The texture.</returns>
		/// <param name="source">Source.</param>
		/// <param name="targetWidth">Target width.</param>
		/// <param name="targetHeight">Target height.</param>
		private Texture2D ScaleTexture (Texture2D source, int targetWidth, int targetHeight)
		{


			if (brushTexture == null)
				brushTexture = new Texture2D (targetWidth, targetHeight, source.format, false);
			else
				brushTexture.Resize (targetWidth, targetHeight, source.format, false);
		
	
			for (int i = 0; i < brushTexture.height; ++i) {
				for (int j = 0; j < brushTexture.width; ++j) {
					Color newColor = source.GetPixelBilinear ((float)j / (float)brushTexture.width, (float)i / (float)brushTexture.height);
					brushTexture.SetPixel (j, i, newColor);
				}
			}
		
			brushTexture.Apply ();
			return brushTexture;
		}


		#region terrain editing



		/// <summary>
		/// Gets the terrains.
		/// </summary>
		public void GetTerrains ()
		{
			_terrains = Terrain.activeTerrains;

			if (_terrains.Length > 0) {




				float yHeight = _terrains [0].transform.position.y;

				Vector3 pos = _terrains [0].transform.position;

				float width = _terrains [0].terrainData.size.x;
				float height = _terrains [0].terrainData.size.y;
				float length = _terrains [0].terrainData.size.z;

				float hWidth = _terrains [0].terrainData.heightmapWidth;
				float hHeight = _terrains [0].terrainData.heightmapHeight;

				bool heightGood = true;
				bool size = true;
				bool resolution = true;
				bool position = true;

				foreach (var item in _terrains) {
					if (yHeight != item.transform.position.y) {

						heightGood = false;
					}
					if (width != item.terrainData.size.x || height != item.terrainData.size.y || length != item.terrainData.size.z) {

						size = false;
					}
					if (hWidth != item.terrainData.heightmapWidth || hHeight != item.terrainData.heightmapHeight) {
						resolution = false;
					}

					Vector3 posTemp = item.transform.position - pos;
                    // if (posTemp.x % width != 0 || posTemp.z % length != 0)
					// 适配生死2，修复浮点数导致的地形不能修改bug
                    if (Mathf.RoundToInt(posTemp.x) % Mathf.RoundToInt(width) != 0 || Mathf.RoundToInt(posTemp.z) % Mathf.RoundToInt(length) != 0)
						position = false;
				}

				if (!heightGood)
					EditorUtility.DisplayDialog ("Terrain errors", "Not all terrains are at the same height, this could effect holes on the borders.", "OK");
				if (!size)
					EditorUtility.DisplayDialog ("Terrain errors", "Not all terrains are the same size, this could effect in errors. Turn off or fix mismatched terrians.", "OK");
				if (!resolution)
					EditorUtility.DisplayDialog ("Terrain errors", "Not all terrains are the same resolution, this could effect in errors. Turn off or fix mismatched terrians", "OK");
				if (!position)
					EditorUtility.DisplayDialog ("Terrain errors", "Not all terrains have good positions. Turn off or fix mismatched terrians", "OK");

			}

		
			if (_terrainDict == null)
				_terrainDict = new Dictionary<int[], Terrain> (new IntArrayComparer ());
			else {
				_terrainDict.Clear ();
			}
		
			if (_terrains.Length > 0) {
				int sizeX = (int)_terrains [0].terrainData.size.x;
				int sizeZ = (int)_terrains [0].terrainData.size.z;

				float maxHeightTemp = float.MinValue;

				if (_terrains.Length > 0)
					firstPosition = new Vector2 (_terrains [0].transform.position.x, _terrains [0].transform.position.z);

				foreach (var terrain in _terrains) {
				
					int[] posTer = new int[] {
						(int)(Mathf.RoundToInt ((terrain.transform.position.x - firstPosition.x) / sizeX)),
						(int)(Mathf.RoundToInt ((terrain.transform.position.z - firstPosition.y) / sizeZ))
					};
					//Debug.Log (terrain.transform.position.ToString () + " " + posTer [0] + " " + posTer [1]);
					if (!_terrainDict.ContainsKey (posTer))
						_terrainDict.Add (posTer, terrain);
					else {
						EditorUtility.DisplayDialog ("Terrain errors", terrain.name + " has unexpected position.", "OK");
					}
				
					if (terrain.terrainData.size.y > maxHeightTemp)
						maxHeightTemp = terrain.terrainData.size.y;
				
				}
				maxHeight = maxHeightTemp;
			
				foreach (var item in _terrainDict) {
					int[] posTer = item.Key;
					Terrain top = null;
					Terrain left = null;
					Terrain right = null;
					Terrain bottom = null;
					_terrainDict.TryGetValue (new int[] {
						posTer [0],
						posTer [1] + 1
					}, out top);
					_terrainDict.TryGetValue (new int[] {
						posTer [0] - 1,
						posTer [1]
					}, out left);
					_terrainDict.TryGetValue (new int[] {
						posTer [0] + 1,
						posTer [1]
					}, out right);
					_terrainDict.TryGetValue (new int[] {
						posTer [0],
						posTer [1] - 1
					}, out bottom);
				
					item.Value.SetNeighbors (left, top, right, bottom);
				
					item.Value.Flush ();
				
				
				}

				lbMessageImportant = "";
			} else {
				edit = false;
				lbMessageImportant = noTerrainMessage;
			}
		}

		/// <summary>
		/// Flatten terrains.
		/// </summary>
		public void Flatten ()
		{
			try {
				foreach (var t in _terrains) {
					Undo.RegisterCompleteObjectUndo (t.terrainData, "Terrain draw end");

				
				}
			} catch (System.Exception) {
				Debug.Log ("Too many terrains to undo flatten");
			}

			float scale = newHeight / Terrain.activeTerrains [0].terrainData.size.y;

			for (int i = 0; i < terrainDataArray.GetLength (0); i++) {
				for (int h = 0; h < terrainDataArray.GetLength (1); h++) {
					terrainDataArray [i, h] = scale;
				}
			}
			AllDataToTerrains ();


		}

		void DrawOnTerrain (Vector3 brushPos, Terrain terrain, bool checkTerrainSides = true)
		{
			
			
			if (terrainDataArray == null)
				CreateTerrainArrays ();
			if (terrainAlphaArray == null)
				CreateTerrainAlphaArrays ();

			TerrainData data = terrain.terrainData;


			int xTerrainCount = terrainArray.GetLength (0);
			int zTerrainCount = terrainArray.GetLength (1);

			if (selectedMethod == 4) {
				terrainToheight = (1 / (data.size.x * xTerrainCount) * (terrainWidthAlpha * xTerrainCount - xTerrainCount + 1));
				terrainTowidth = (1 / (data.size.z * zTerrainCount) * (terrainWidthAlpha * zTerrainCount - zTerrainCount + 1));
			} else {
				
				terrainToheight = (1 / (data.size.x * xTerrainCount) * (terrainWidth * xTerrainCount - xTerrainCount + 1));
				terrainTowidth = (1 / (data.size.z * zTerrainCount) * (terrainWidth * zTerrainCount - zTerrainCount + 1));
			}
			int rangeTerrainX = (int)(brushRange * terrainToheight);
			int rangeTerrainZ = (int)(brushRange * terrainTowidth);


			Vector3 terrainSpaceCoords = brushPos;

			float posX = (terrainSpaceCoords.x - xMin) * terrainToheight;
			float posZ = (terrainSpaceCoords.z - zMin) * terrainTowidth;

		

			int posXFinal = (int)(posX - 0.5f * rangeTerrainX);
			int posZFinal = (int)(posZ - 0.5f * rangeTerrainZ);

			//Debug.Log (posXFinal + " " + posZFinal);

			int shift = (Event.current.shift) ? -1 : 1;
			if (selectedMethod == 1 && shift == -1 && checkTerrainSides) {
				newHeight = terrain.terrainData.GetHeight ((int)posX, (int)posZ);
				Repaint ();
			}

			float scale = newHeight / data.size.y;

			int startPosX = Mathf.Clamp (posXFinal, 0, terrainDataArray.GetLength (0));
			int endPosX = Mathf.Clamp (posXFinal + rangeTerrainX, 0, terrainDataArray.GetLength (0));
			int startPosZ = Mathf.Clamp (posZFinal, 0, terrainDataArray.GetLength (1));
			int endPosZ = Mathf.Clamp (posZFinal + rangeTerrainZ, 0, terrainDataArray.GetLength (1));
		
		
			//Debug.Log (startPosX + " " + endPosX + " " + startPosZ + " " + endPosZ);

			if (selectedMethod < 3) {

				float sum = 0;
				float count = 0;
				float average = 0;

				if (selectedMethod == 2) {
					for (int i = startPosX; i < endPosX; i++) {
						for (int h = startPosZ; h < endPosZ; h++) {
							sum += terrainDataArray [i, h];
							count++;
						}
					}
					average = sum / count;
				}


				for (int i = startPosX; i < endPosX; i++) {
					for (int h = startPosZ; h < endPosZ; h++) {
						
						float imagePixel = 1;
						try {
							imagePixel = Mathf.Clamp01 (bigPicture [h - posZFinal, bigPicture.GetLength (1) - (i - posXFinal) - 1].a - 0.05f);
						} catch {
							
						}


						switch (selectedMethod) {
						case 0:

							terrainDataArray [i, h] = Mathf.Clamp01 (terrainDataArray [i, h] + shift * imagePixel * scale);
							break;				
						case 1:
							if (shift == 1)
								terrainDataArray [i, h] = Mathf.Clamp01 (Mathf.Lerp (terrainDataArray [i, h], scale, imagePixel * opacity / 100.0f));						
							break;
						case 2:
						
							sum = 0;
							count = 0;

							for (int xBlur = (i - blurSize * 0.5f) >= 0 ? (int)(i - blurSize * 0.5f) : i; (xBlur < blurSize * 0.5f + i && xBlur < endPosX); xBlur++) {
								for (int yBlur = (h - blurSize * 0.5f) >= 0 ? (int)(h - blurSize * 0.5f) : h; (yBlur < blurSize * 0.5f + h && yBlur < endPosZ); yBlur++) {
									sum += terrainDataArray [xBlur, yBlur];
									count++;
								}
							}

							float averageLocal = sum / count;


							terrainDataArray [i, h] = Mathf.Lerp (terrainDataArray [i, h], average * 0.1f + averageLocal * 0.9f, imagePixel * opacity * 0.01f);


							break;
						default:
							break;
						}

					}


				}
				DataToTerrains (startPosX, endPosX, startPosZ, endPosZ);
			} else {
				if (splatPrototypes.Count == 0)
					return;


				startPosX = Mathf.Clamp (posXFinal, 0, terrainAlphaArray.GetLength (0));
				endPosX = Mathf.Clamp (posXFinal + rangeTerrainX, 0, terrainAlphaArray.GetLength (0));
				startPosZ = Mathf.Clamp (posZFinal, 0, terrainAlphaArray.GetLength (1));
				endPosZ = Mathf.Clamp (posZFinal + rangeTerrainZ, 0, terrainAlphaArray.GetLength (1));


				float oldValue = 0;
				for (int i = startPosX; i < endPosX; i++) {
					for (int h = startPosZ; h < endPosZ; h++) {
						
						float imagePixel = 0;
						try {				
							imagePixel = bigPicture [h - posZFinal, bigPicture.GetLength (1) - (i - posXFinal) - 1].a;
						} catch {

						}
						oldValue =	terrainAlphaArray [i, h, currentSplatMap];
						terrainAlphaArray [i, h, currentSplatMap] = Mathf.Lerp (terrainAlphaArray [i, h, currentSplatMap], 1, imagePixel * opacity / 100.0f);


						for (int l = 0; l < splatPrototypes.Count; l++) {
							if (l != currentSplatMap) {
								terrainAlphaArray [i, h, l] = oldValue == 1 ? 0 : Mathf.Clamp01 (terrainAlphaArray [i, h, l] * ((1 - terrainAlphaArray [i, h, currentSplatMap]) / (1 - oldValue)));

							}

						}

					}
				}



				DataToTerrainsAlphas (startPosX, endPosX, startPosZ, endPosZ);
			}

		

		}

	

		#endregion
	}
}