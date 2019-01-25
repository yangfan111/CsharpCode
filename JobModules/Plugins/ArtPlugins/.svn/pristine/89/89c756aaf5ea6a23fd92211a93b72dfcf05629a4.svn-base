using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

/**
* IMPORTANT NOTE:
* Unity's terrain data co-ordinates are not setup as you might expect.
* Assuming the terrain is not rotated, this is the terrain strides vs axis:
* [  0,      0  ] = -X, -Z
* [width,    0  ] = +X, -Z
* [  0,   height] = -X, +Z
* [width, height] = +X, +Z
* 
* This means that the that X goes out to the 3D Z-Axis, and Y goes out into the 3D X-Axis.
* This also means that a world space position such as the mouse position from a raycast needs 
*   its worldspace X-Axis position mapped to Z, and the worldspace Y-Axis mapped to X
*/

namespace JesseStiller.TerrainFormerExtension {
    [CustomEditor(typeof(TerrainFormer))]
    internal class TerrainFormerEditor : Editor {
        // The first tool in order from left to right that is not a scultping tool.
        private const Tool firstNonMouseTool = Tool.Heightmap;
        
        // Brush fields
        private const float minBrushSpeed = 0.001f;
        private const float maxBrushSpeed = 2f;

        internal static Settings settings;
        
        internal static float[,] toolScratchArray = new float[32, 32];

        // Reflection fields
        private List<object> unityTerrainInspectors = new List<object>();
        private static PropertyInfo unityTerrainSelectedTool;
        private readonly static PropertyInfo guiUtilityTextFieldInput;
        private readonly static MethodInfo terrainDataSetBasemapDirtyMethodInfo;
        private readonly static MethodInfo inspectorWindowRepaintAllInspectors;
#if !UNITY_5_2_OR_NEWER && !UNITY_2017_1_OR_NEWER
        private readonly static MethodInfo setHeightsDelayLODMethodInfo;
        private readonly static MethodInfo applyDelayedHeightmapModificationMethodInfo;
#endif
        
        // Instance/Editor related fields
        private static int activeInspectorInstanceID = 0;
        internal static TerrainFormerEditor Instance;
        private TerrainFormer terrainFormer;
        
        internal static float[,] allTerrainHeights, allUnmodifiedTerrainHeights;
        internal static float[,,] allTextureSamples;
        internal static SplatPrototype[] splatPrototypes;
        internal int heightmapWidth, heightmapHeight; // Grid heightmap samples
        internal int toolSamplesHorizontally, toolSamplesVertically; // Heightmap samples for sculpting, splatmap samples for painting
        private int heightmapResolution;
        private int alphamapResolution;
        private int currentToolsResolution;
        internal Vector3 terrainSize; // Size of a single terrain (not a terrain grid)

        // Terrain Grid specific fields
        internal List<TerrainInformation> terrainInformations;
        internal int numberOfTerrainsHorizontally = 1;
        internal int numberOfTerrainsVertically = 1;

        // The first terrain (either the bottom left most one in the grid, or the only terrain).
        internal Transform firstTerrainTransform;
        private Terrain firstTerrain;
        private TerrainData firstTerrainData;

        private TerrainCommand currentCommand;
        internal CommandArea globalCommandArea;

        private bool isTerrainGridParentSelected = false;
        
        // Heightfield fields
        private Texture2D heightmapTexture;

        // States and Information
        private int lastHeightmapResolultion;
        internal bool isSelectingBrush = false;
        private bool behaviourGroupUnfolded = true;

        private SamplesDirty samplesDirty = SamplesDirty.None;
        
        // Projector and other visual cursor-like fields
        private GameObject gridPlane;
        private Material gridPlaneMaterial;
        private GameObject brushProjectorGameObject;
        private Projector brushProjector;
        private Material brushProjectorMaterial;
        private GameObject topPlaneGameObject; // Used to show the current height of "Flatten" and "Set Height"
        private Material topPlaneMaterial;
        internal static Texture2D brushProjectorTexture;
        
        private static GUIContent[] toolsGUIContents;
        
        // Mouse related fields
        private bool mouseIsDown; 
        private Vector2 mousePosition = new Vector2(); // The current screen-space position of the mouse. This position is used for raycasting
        private Vector2 lastMousePosition;
        private Vector3 lastWorldspaceMousePosition;
        private float mouseSpacingDistance = 0f;
        private Vector3 lastClickPosition; // The point of the terrain the mouse clicked on
        
        private float randomSpacing;
        internal float currentTotalMouseDelta = 0f;
#if(UNITY_2017_1_OR_NEWER == false)
        private bool isShowingToolTooltip = false;
#endif

        private SavedTool currentTool;
        internal Tool CurrentTool {
            get {
                if(UnityEditor.Tools.current == UnityEditor.Tool.None && GetInstanceID() == activeInspectorInstanceID) {
                    return currentTool.Value;
                }
                return Tool.None;
            }
            private set {
                if(value == CurrentTool) return;
                if(value != Tool.None) UnityEditor.Tools.current = UnityEditor.Tool.None;

                Tool previousTool = currentTool.Value;
                currentTool.Value = value;
                CurrentToolChanged(previousTool);
            }
        }
        
        private TerrainBrush CurrentBrush {
            get {
                if(string.IsNullOrEmpty(settings.modeSettings[CurrentTool].selectedBrushId)) {
                    settings.modeSettings[CurrentTool].selectedBrushId = BrushCollection.defaultFalloffBrushId;
                    SelectedBrushChanged();
                } else if(BrushCollection.brushes.ContainsKey(settings.modeSettings[CurrentTool].selectedBrushId) == false) {
                    settings.modeSettings[CurrentTool].selectedBrushId = BrushCollection.brushes.Keys.First();
                    SelectedBrushChanged();
                }

                return BrushCollection.brushes[settings.modeSettings[CurrentTool].selectedBrushId];
            }
        }

        // The minimum brush size is set to the total length of five heightmap segments (with one segment being the length from one sample to its neighbour)
        private float MinBrushSize {
            get {
                return Mathf.CeilToInt((terrainSize.x / heightmapResolution) * 3f);
            }
        }
        
        private float[,] temporarySamples;
        private float halfBrushSizeInSamples;
        private int brushSizeInSamples;
        private int BrushSizeInSamples {
            set {
                Debug.Assert(value > 0);

                if(brushSizeInSamples == value) return;
                brushSizeInSamples = value;
                halfBrushSizeInSamples = brushSizeInSamples * 0.5f;
            }
        }
        
        static TerrainFormerEditor() {
            guiUtilityTextFieldInput = typeof(GUIUtility).GetProperty("textFieldInput", BindingFlags.NonPublic | BindingFlags.Static);
            inspectorWindowRepaintAllInspectors = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow").GetMethod(
                "RepaintAllInspectors", BindingFlags.Static | BindingFlags.NonPublic);
            terrainDataSetBasemapDirtyMethodInfo = typeof(TerrainData).GetMethod("SetBasemapDirty", BindingFlags.Instance | BindingFlags.NonPublic);

#if !UNITY_5_2_OR_NEWER && !UNITY_2017_1_OR_NEWER
            applyDelayedHeightmapModificationMethodInfo = typeof(Terrain).GetMethod("ApplyDelayedHeightmapModification", BindingFlags.Instance | BindingFlags.NonPublic);
            setHeightsDelayLODMethodInfo = typeof(TerrainData).GetMethod("SetHeightsDelayLOD", BindingFlags.NonPublic | BindingFlags.Instance);
#endif
        }
        internal void OnEnable()
        { }
            // Simple initialization logic that doesn't rely on any secondary data
            internal void loadAndInit() {
            // Sometimes it's possible Terrain Former thinks the mouse is still pressed down as not every event is detected by Terrain Former
            mouseIsDown = false; 
            terrainFormer = (TerrainFormer)target;
            currentTool = new SavedTool("TerrainFormer/CurrentTool", Tool.None);
            
            // Forcibly re-initialize just in case variables were lost during an assembly reload
            if(Initialize(true) == false) return;
            
            if(toolsGUIContents == null) {
                toolsGUIContents = new GUIContent[] {
                    new GUIContent(null, AssetDatabase.LoadAssetAtPath<Texture2D>(settings.mainDirectory + "Textures/Icons/RaiseLower.png"), "Raise/Lower"),
                    new GUIContent(null, AssetDatabase.LoadAssetAtPath<Texture2D>(settings.mainDirectory + "Textures/Icons/Smooth.png"), "Smooth"),
                    new GUIContent(null, AssetDatabase.LoadAssetAtPath<Texture2D>(settings.mainDirectory + "Textures/Icons/SetHeight.png"), "Set Height"),
                    new GUIContent(null, AssetDatabase.LoadAssetAtPath<Texture2D>(settings.mainDirectory + "Textures/Icons/Flatten.png"), "Flatten"),
                    new GUIContent(null, AssetDatabase.LoadAssetAtPath<Texture2D>(settings.mainDirectory + "Textures/Icons/Mould.psd"), "Mould"),
                    new GUIContent(null, AssetDatabase.LoadAssetAtPath<Texture2D>(settings.mainDirectory + "Textures/Icons/PaintTexture.psd"), "Paint Texture"),
                    new GUIContent(null, AssetDatabase.LoadAssetAtPath<Texture2D>(settings.mainDirectory + "Textures/Icons/Heightmap.psd"), "Heightmap"),
                    new GUIContent(null, AssetDatabase.LoadAssetAtPath<Texture2D>(settings.mainDirectory + "Textures/Icons/Generate.png"), "Generate"),
                    new GUIContent(null, AssetDatabase.LoadAssetAtPath<Texture2D>(settings.mainDirectory + "Textures/Icons/Settings.png"), "Settings")
                };
            }
            
            // Set the Terrain Former component icon
            Type editorGUIUtilityType = typeof(EditorGUIUtility);
            MethodInfo setIcon = editorGUIUtilityType.GetMethod("SetIconForObject", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic, null, 
                new Type[] { typeof(UnityEngine.Object), typeof(Texture2D) }, null);
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(settings.mainDirectory + "Textures/Icon.png");
            setIcon.Invoke(null, new object[] { target, icon});
            
            Undo.undoRedoPerformed += UndoRedoPerformed;
            SceneView.onSceneGUIDelegate += OnSceneGUICallback;

            if(activeInspectorInstanceID == 0) {
                CurrentToolChanged(Tool.Ignore);
            }


        }
        
        /**
        * Initialize contains logic that is intrinsically tied to this entire terrain tool. If any of these fields and 
        * other things are missing, then the entire editor will break. An attempt will be made every GUI frame to find them.
        * Returns true if the initialization was successful or if everything is already initialized, false otherwise.
        * If the user moves Terrain Former's Editor folder away and brings it back, the brushProjector dissapears. This is why
        * it is checked for on Initialization.
        */
        private bool Initialize(bool forceReinitialize = false) {
            if(forceReinitialize == false && terrainFormer != null && brushProjector != null) {
                return true;
            }
            Debug.LogError("forceReinitialize" + forceReinitialize);
            Debug.LogError("terrainFormer" + terrainFormer);
            Debug.LogError("brushProjector" + brushProjector);
            /**
            * If there is more than one object selected, do not even bother initializing. This also fixes a strange 
            * exception occurance when two terrains or more are selected; one with Terrain Former and one without
            */
            if(Selection.objects.Length != 1) return false;

            // Make sure there is only ever one Terrain Former on the current object
            TerrainFormer[] terrainFormerInstances = terrainFormer.GetComponents<TerrainFormer>();
            if(terrainFormerInstances.Length > 1) {
                for(int i = terrainFormerInstances.Length - 1; i > 0; i--) {
                    DestroyImmediate(terrainFormerInstances[i]);
                }
                EditorUtility.DisplayDialog("Terrain Former", "You can't add multiple Terrain Former components to a single Terrain object.", "Close");
                return false;
            }
            
            if(settings == null) {
                InitializeSettings();
            }

            if(settings == null) return false;
            
            settings.AlwaysShowBrushSelectionChanged = AlwaysShowBrushSelectionValueChanged;
            settings.brushColour.ValueChanged = BrushColourChanged;

            if(UpdateTerrainRelatedFields() == false) return false;
            UpdateAllHeightsFromSourceAssets();
            
            BrushCollection.Initilize();

            CreateProjector();

            CreateGridPlane();

            /**
            * Get an instance of the built-in Unity Terrain Inspector so we can override the selectedTool property
            * when the user selects a different tool in Terrain Former. This makes it so the user can't accidentally
            * use two terain tools at once (eg. Unity Terrain's raise/lower, and Terrain Former's raise/lower)
            */
            Type unityTerrainInspectorType = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.TerrainInspector");
            unityTerrainSelectedTool = unityTerrainInspectorType.GetProperty("selectedTool", BindingFlags.NonPublic | BindingFlags.Instance);
            
            UnityEngine.Object[] terrainInspectors = Resources.FindObjectsOfTypeAll(unityTerrainInspectorType);
            // Iterate through each Unity terrain inspector to find the Terrain Inspector(s) that belongs to this object
            foreach(UnityEngine.Object inspector in terrainInspectors) {
                Editor inspectorAsEditor = (Editor)inspector;
                GameObject inspectorGameObject = ((Terrain)inspectorAsEditor.target).gameObject;
                
                if(inspectorGameObject == terrainFormer.gameObject) {
                    unityTerrainInspectors.Add(inspector);
                }
            }
            
            AssetWatcher.OnAssetsImported = OnAssetsImported;
            AssetWatcher.OnAssetsMoved = OnAssetsMoved;
            AssetWatcher.OnAssetsDeleted = OnAssetsDeleted;
            AssetWatcher.OnWillSaveAssetsAction = OnWillSaveAssets;
            
            return true;
        }

        // Returns false if there was an error.
        private bool UpdateTerrainRelatedFields() {
            Terrain terrainComponentOfTarget = terrainFormer.GetComponent<Terrain>();
            terrainInformations = new List<TerrainInformation>();

            if(terrainComponentOfTarget) {
                // If there is a terrain component attached to this object, check if it's one of many terrains inside of a grid.
                if(terrainFormer.transform.parent != null && terrainFormer.transform.parent.childCount > 0) {
                    Terrain[] terrainComponentsInChildren = terrainFormer.transform.parent.GetComponentsInChildren<Terrain>();
                    foreach(Terrain terrain in terrainComponentsInChildren) {
                        if(terrain.terrainData == null) continue;
                        if(AreTerrainsContiguous(terrain, terrainComponentOfTarget) == false) continue;
                        terrainInformations.Add(new TerrainInformation(terrain));
                    }
                } else if(terrainComponentOfTarget.terrainData != null) {
                    terrainInformations.Add(new TerrainInformation(terrainComponentOfTarget));
                }
            } else {
                // If Terrain Former is attached to a game object with children that contain Terrains, allow Terrain Former to look into the child terrain objects.
                Terrain[] terrainChildren = terrainFormer.GetComponentsInChildren<Terrain>();
                if(terrainChildren != null && terrainChildren.Length > 0) {
                    isTerrainGridParentSelected = true;
                } else {
                    return false;
                }

                foreach(Terrain terrain in terrainChildren) {
                    if(terrain.terrainData == null) continue;

                    /**
                    * Assume the first terrain component found is the one we want to match since it's impossible to know exactly what the user will want.
                    */
                    if(AreTerrainsContiguous(terrain, terrainChildren[0]) == false) continue;

                    terrainInformations.Add(new TerrainInformation(terrain));
                }
            }

            if(terrainInformations.Count == 0) return false;

            // Assume the first terrain information has the correct parameters
            terrainSize = terrainInformations[0].terrainData.size;
            heightmapResolution = terrainInformations[0].terrainData.heightmapResolution;
            alphamapResolution = terrainInformations[0].terrainData.alphamapResolution;
            lastHeightmapResolultion = heightmapResolution;

            if(terrainInformations.Count == 1) {
                if(terrainComponentOfTarget) {
                    firstTerrainTransform = terrainComponentOfTarget.transform;
                } else {
                    firstTerrainTransform = terrainInformations[0].transform;
                }
            } else {
                // If there is more than one terrain, find the top-left most terrain to determine grid coordinates
                Vector3 bottomLeftMostValue = new Vector3(float.MaxValue, 0f, float.MaxValue);
                Vector3 currentTerrainPosition;
                foreach(TerrainInformation ti in terrainInformations) {
                    currentTerrainPosition = ti.transform.position;
                    if(currentTerrainPosition.x <= bottomLeftMostValue.x && currentTerrainPosition.z <= bottomLeftMostValue.z) {
                        bottomLeftMostValue = currentTerrainPosition;
                        firstTerrainTransform = ti.terrain.transform;
                    }
                }

                foreach(TerrainInformation terrainInformation in terrainInformations) {
                    terrainInformation.gridXCoordinate = Mathf.RoundToInt((terrainInformation.transform.position.x - bottomLeftMostValue.x) / terrainSize.x);
                    terrainInformation.gridYCoordinate = Mathf.RoundToInt((terrainInformation.transform.position.z - bottomLeftMostValue.z) / terrainSize.z);
                    terrainInformation.alphamapsXOffset = terrainInformation.gridXCoordinate * alphamapResolution;
                    terrainInformation.alphamapsYOffset = terrainInformation.gridYCoordinate * alphamapResolution;
                    terrainInformation.heightmapXOffset = terrainInformation.gridXCoordinate * heightmapResolution - terrainInformation.gridXCoordinate;
                    terrainInformation.heightmapYOffset = terrainInformation.gridYCoordinate * heightmapResolution - terrainInformation.gridYCoordinate;

                    if(terrainInformation.gridXCoordinate + 1 > numberOfTerrainsHorizontally) {
                        numberOfTerrainsHorizontally = terrainInformation.gridXCoordinate + 1;
                    } else if(terrainInformation.gridYCoordinate + 1 > numberOfTerrainsVertically) {
                        numberOfTerrainsVertically = terrainInformation.gridYCoordinate + 1;
                    }
                }
            }

            firstTerrain = firstTerrainTransform.GetComponent<Terrain>();
            if(firstTerrain == null) return false;
            firstTerrainData = firstTerrain.terrainData;
            if(firstTerrainData == null) return false;

            heightmapWidth = numberOfTerrainsHorizontally * heightmapResolution - (numberOfTerrainsHorizontally - 1);
            heightmapHeight = numberOfTerrainsVertically * heightmapResolution - (numberOfTerrainsVertically - 1);

            if(TerrainMismatchManager.IsInitialized == false) TerrainMismatchManager.Initialize(terrainInformations);
            if(TerrainMismatchManager.IsMismatched) return false;

            splatPrototypes = firstTerrainData.splatPrototypes;

            allTerrainHeights = new float[heightmapHeight, heightmapWidth];
            return true;
        }

        internal static void InitializeSettings() {
            settings = Settings.Create();
        }

        public void  OnDisable() {
        }
       
        public void unloadThis() {
            if(settings != null) settings.Save();
            Debug.Log("unloadThis");
            // Destroy all gizmos
            if(brushProjectorGameObject != null) {
                DestroyImmediate(brushProjectorMaterial);
                DestroyImmediate(brushProjectorGameObject);
                DestroyImmediate(topPlaneMaterial);
                DestroyImmediate(topPlaneGameObject);
                brushProjector = null;
            }

            if(gridPlane != null) {
                DestroyImmediate(gridPlaneMaterial);
                DestroyImmediate(gridPlane.gameObject);
                gridPlaneMaterial = null;
                gridPlane = null;
            }

            Undo.undoRedoPerformed -= UndoRedoPerformed;
            AssetWatcher.OnAssetsImported -= OnAssetsImported;
            AssetWatcher.OnAssetsMoved -= OnAssetsMoved;
            AssetWatcher.OnAssetsDeleted -= OnAssetsDeleted;
            AssetWatcher.OnWillSaveAssetsAction -= OnWillSaveAssets;

            if(settings != null) {
                settings.AlwaysShowBrushSelectionChanged = null;
                settings.brushColour.ValueChanged = null;
            }
            
            Instance = null;
            if(activeInspectorInstanceID == GetInstanceID()) activeInspectorInstanceID = 0;

            SceneView.onSceneGUIDelegate -= OnSceneGUICallback;
        }
        
        private bool neighboursFoldout = false;
        bool _loadAndInit=false;
        bool _FinishEdit=false;
        GameObject wcRoot;
        bool _loadwcRoot = false;
        public override void OnInspectorGUI() {
            bool displayingProblem = false;
            _loadAndInit = EditorGUILayout.Toggle("startEdit",_loadAndInit);
            if (_loadAndInit) {
                _loadAndInit = false;
                loadAndInit();
                UpdateAllUnmodifiedHeighOnLoad();
            }
           
            wcRoot = EditorGUILayout.ObjectField("WC root GameObject", wcRoot,typeof(GameObject)) as GameObject;
            if (_loadwcRoot = EditorGUILayout.Toggle("load from WC", _loadwcRoot)) {
                _loadwcRoot = false;
                loadwcRoot(wcRoot);

              
            }
            //_FinishEdit = EditorGUILayout.Toggle("FinishEdit", _FinishEdit);
            //if (_FinishEdit)
            //{
            //    _FinishEdit = false;
            //    unloadThis();
            //}
            // Stop if the initialization was unsuccessful

                if (terrainInformations == null || terrainInformations.Count == 0) {
          
          
                EditorGUILayout.HelpBox("There is no terrain attached to this object, nor are there any terrain objects as children to this object.", MessageType.Info);
                return;
            }
            else if(firstTerrainData == null) {
                EditorGUILayout.HelpBox("Missing terrain data asset. Reassign the terrain asset in the Unity Terrain component.", MessageType.Error);
                displayingProblem = true;
            }

            bool containsAtleastOneTerrainCollider = false;
            bool hasOneOrMoreTerrainCollidersDisabled = false;
            foreach(TerrainInformation terrainInformation in terrainInformations) {
                if(terrainInformation.collider == null) continue;
                containsAtleastOneTerrainCollider = true;

                if(terrainInformation.collider.enabled == false) hasOneOrMoreTerrainCollidersDisabled = true;

                break;
            }
            if(containsAtleastOneTerrainCollider == false) {
                if(terrainInformations.Count > 1) {
                    EditorGUILayout.HelpBox("There aren't any terrain colliders attached to any of the terrains in the terrain grid.", MessageType.Error);
                } else {
                    EditorGUILayout.HelpBox("This terrain object doesn't have a terrain collider attached to it.", MessageType.Error);
                }
                displayingProblem = true;
            }
            
            if(hasOneOrMoreTerrainCollidersDisabled) {
                EditorGUILayout.HelpBox("There is at least one terrain that has an inactive collider. Terrain editing functionality won't work on the affected terrain(s).", MessageType.Warning);
                displayingProblem = true;
            }

            if(target == null) {
                EditorGUILayout.HelpBox("There is no target object. Make sure Terrain Former is a component of a terrain object.", MessageType.Error);
                displayingProblem = true;
            }

            if(TerrainMismatchManager.IsInitialized) TerrainMismatchManager.Draw();

            if(settings == null) {
                EditorGUILayout.HelpBox("The Settings.tf file couldn't load and attempts to create a new one failed.", MessageType.Error);
                displayingProblem = true;
            }
            
            if(displayingProblem) return;
            
            if(Initialize() == false) return;
            
            TerrainFormerStyles.Update();
            
            EditorGUIUtility.labelWidth = CurrentTool == Tool.Settings ? 188f : 128f;

            CheckKeyboardShortcuts(Event.current);
            
            // The user couldn't modified the heightmap resolution outside of Terrain Former, so check for it here
            int heightmapResolution = firstTerrainData.heightmapResolution;
            if(lastHeightmapResolultion != -1 && lastHeightmapResolultion != heightmapResolution) {
                BrushSizeChanged();
                lastHeightmapResolultion = heightmapResolution;
            }
            
            /** 
            * Get the current Unity Terrain Inspector tool, and set the Terrain Former tool to none if the Unity Terrain
            * Inspector tool is not none.
            */
            if(unityTerrainInspectors != null && CurrentTool != Tool.None) {
                foreach(object inspector in unityTerrainInspectors) {
                    int unityTerrainTool = (int)unityTerrainSelectedTool.GetValue(inspector, null);
                    // If the tool is not "None" (-1), then the Terrain Former tool must be set to none
                    if(unityTerrainTool != -1) {
                        currentTool.Value = Tool.None;
                    }
                }
            }

            /*
            * Draw the toolbar
            */
            Rect toolbarRect = EditorGUILayout.GetControlRect(false, 22f, GUILayout.MaxWidth(285f));
            toolbarRect.x = Mathf.Round(Screen.width * 0.5f - toolbarRect.width * 0.5f); // Rounding is required to stop blurriness in older versions of Unity
            CurrentTool = (Tool)GUI.Toolbar(toolbarRect, (int)CurrentTool, toolsGUIContents);
            
            // Display a tooltip showing the current tool being hovered over
#if(UNITY_2017_1_OR_NEWER == false)
            Event currentEvent = Event.current;
            if(currentEvent.type == EventType.Repaint) {
                if(toolbarRect.Contains(currentEvent.mousePosition)) {
                    float mouseHorizontalDelta = currentEvent.mousePosition.x - toolbarRect.x;
                    float tabWidth = toolbarRect.width / toolsGUIContents.Length;
                    int toolIndex = Mathf.FloorToInt((mouseHorizontalDelta / toolbarRect.width) * toolsGUIContents.Length);
                    float centerOfTabHoveredOver = toolIndex * tabWidth + tabWidth * 0.5f + toolbarRect.x;

                    Vector2 tooltipBoxSize = GUI.skin.box.CalcSize(new GUIContent(toolsGUIContents[toolIndex].tooltip));

                    /**
                    * The GUI.box style in the dark skin has a transparent background and incorrect text colour. If the Unity Pro skin is 
                    * being used, we need to rebuild the GUISkin.box style based on the "OL box" style.
                    */
                    GUIStyle tooltipStyle = new GUIStyle(GUI.skin.box);
                    if(EditorGUIUtility.isProSkin) {
                        tooltipStyle.normal.background = GUI.skin.GetStyle("OL box").normal.background;
                        tooltipStyle.normal.textColor = new Color(0.82f, 0.82f, 0.82f, 1f);
                    }

                    tooltipStyle.Draw(new Rect(centerOfTabHoveredOver - tooltipBoxSize.x * 0.5f, toolbarRect.y - 20f, tooltipBoxSize.x + 6f, tooltipBoxSize.y),
                        toolsGUIContents[toolIndex].tooltip, false, false, false, false);

                    isShowingToolTooltip = true;
                } else if(isShowingToolTooltip) {
                    isShowingToolTooltip = false;
                    Repaint();
                }
            }
#endif

            if(CurrentTool == Tool.None || activeInspectorInstanceID != GetInstanceID()) return;

            if((CurrentTool != Tool.None && CurrentTool < firstNonMouseTool) && (Event.current.type == EventType.MouseUp || Event.current.type == EventType.KeyUp)) {
                UpdateDirtyBrushSamples();
            }
            
            // Big bold label showing the current tool
            GUILayout.Label(toolsGUIContents[(int)CurrentTool].tooltip, TerrainFormerStyles.largeBoldLabel);

            switch(CurrentTool) {
                case Tool.Smooth:
                    settings.boxFilterSize = (EditorGUILayout.IntSlider(GUIContents.boxFilterSize, settings.boxFilterSize * 2 + 1, 3, 11) -1) / 2;

                    GUILayout.Label("Smooth All", EditorStyles.boldLabel);

                    if(GUIUtilities.LeftFillAndRightButton(
                        fillControl: r => {
                            settings.smoothingIterations = EditorGUI.IntSlider(r, GUIContents.smoothAllIterations, settings.smoothingIterations, 1, 10);
                        },
                        buttonContent: GUIContents.smoothAllTerrain,
                        buttonWidth: 90
                    )) {
                        RunCommandOnTerrainGrid(new SmoothCommand(null, settings.boxFilterSize, heightmapWidth, heightmapHeight), settings.smoothingIterations);
                    }
                    break;
                case Tool.SetHeight:
                    if(GUIUtilities.LeftFillAndRightButton(
                        fillControl: r => {
                            settings.setHeight = EditorGUI.Slider(r, "Set Height", settings.setHeight, 0f, terrainSize.y);
                        },
                        buttonContent: new GUIContent("Apply to Terrain"),
                        buttonWidth: 116
                    )) {
                        SetHeightAll(settings.setHeight / terrainSize.y);
                    }

                    break;
                case Tool.Flatten:
                    settings.flattenMode = (FlattenMode)EditorGUILayout.EnumPopup(GUIContents.flattenMode, settings.flattenMode);
                    break;
                case Tool.Mould:
#if !UNITY_5_3_OR_NEWER
                    EditorGUILayout.HelpBox("Terrain Former's Mould does not have its full set of optimizations in Unity versions older than " +
                        "Unity 5.3, as a result you may experience poor performance.", MessageType.Warning);
#endif
                    settings.mouldToolBoxFilterSize = (EditorGUILayout.IntSlider(GUIContents.boxFilterSize, settings.mouldToolBoxFilterSize * 2 + 1, 3, 11) - 1) / 2;

                    settings.mouldToolRaycastOffset = EditorGUILayout.FloatField(GUIContents.mouldHeightOffset, settings.mouldToolRaycastOffset);
                    settings.mouldToolRaycastTopDown = GUIUtilities.RadioButtonsControl(GUIContents.mouldToolRaycastTopDownContent, 
                        settings.mouldToolRaycastTopDown ? 0 : 1, GUIContents.mouldToolRaycastDirectionContents) == 0;

                    EditorGUILayout.LabelField("Mould All", EditorStyles.boldLabel);

                    if(GUIUtilities.LeftFillAndRightButton(
                        fillControl: r => {
                            settings.mouldAllIterations = EditorGUI.IntSlider(r, GUIContents.mouldAllIterations, settings.mouldAllIterations, 1, 10);
                        },
                        buttonContent: GUIContents.mouldAllTerrain,
                        buttonWidth: 80
                    )) {
                        MouldCommand mouldCommand = new MouldCommand(null, settings.mouldToolBoxFilterSize, heightmapWidth, heightmapHeight);
                        RunCommandOnTerrainGrid(mouldCommand, settings.mouldAllIterations);
                    }
                    
                    break;
                case Tool.PaintTexture:
                    EditorGUILayout.LabelField("Textures", EditorStyles.boldLabel);

                    Texture2D[] splatIcons = new Texture2D[splatPrototypes.Length];
                    for(int i = 0; i < splatIcons.Length; ++i) {
                        splatIcons[i] = AssetPreview.GetAssetPreview(splatPrototypes[i].texture) ?? splatPrototypes[i].texture;
                    }

                    settings.selectedTextureIndex = GUIUtilities.TextureSelectionGrid(settings.selectedTextureIndex, splatIcons);

                    settings.targetOpacity = EditorGUILayout.Slider("Target Opacity", settings.targetOpacity, 0f, 1f);
                    
                    break;
                case Tool.Heightmap:
                    EditorGUILayout.LabelField("Modification", EditorStyles.boldLabel);
                    if(GUIUtilities.LeftFillAndRightButton(
                        fillControl: r => {
                            settings.heightmapHeightOffset = EditorGUI.FloatField(r, "Offset Height", settings.heightmapHeightOffset);
                        },
                        buttonContent: new GUIContent("Apply to Terrain"),
                        buttonWidth: 115
                    )) {
                        OffsetTerrainGridHeight(settings.heightmapHeightOffset);
                    }

                    EditorGUILayout.LabelField("Import", EditorStyles.boldLabel);

                    settings.heightmapSourceIsAlpha = GUIUtilities.RadioButtonsControl(new GUIContent("Source"), settings.heightmapSourceIsAlpha ? 1 : 0, GUIContents.heightmapSources) == 1;
                    
                    // Calling the Layout version of ObjectField will make a 64px sized picker with lots of dead space.
                    heightmapTexture = (Texture2D)EditorGUI.ObjectField(EditorGUILayout.GetControlRect(), "Heightmap Texture", heightmapTexture, typeof(Texture2D), false);

                    GUILayout.Space(4f);

                    GUI.enabled = heightmapTexture != null;
                    Rect importHeightmapButtonRect = EditorGUILayout.GetControlRect(GUILayout.Width(140f), GUILayout.Height(22f));
                    importHeightmapButtonRect.x = Screen.width * 0.5f - 70f;
                    if(GUI.Button(importHeightmapButtonRect, "Import Heightmap")) {
                        ImportHeightmap();
                    }
                    GUI.enabled = true;

#if UNITY_2017_1_OR_NEWER
                    EditorGUILayout.LabelField("Export", EditorStyles.boldLabel);
                    if(GUILayout.Button("Export as EXR…", GUILayout.Width(125f), GUILayout.Height(22f))) {
                        Texture2D heightmapTex = new Texture2D(heightmapWidth, heightmapHeight, TextureFormat.RGBAFloat, false);

                        string saveDestination = EditorUtility.SaveFilePanel("Export as EXR", string.Empty, "Heightmap", "exr");

                        if(string.IsNullOrEmpty(saveDestination) == false) {
                            ExportHeightmap(ref heightmapTex);
                            File.WriteAllBytes(saveDestination, heightmapTex.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat));
                        }

                        DestroyImmediate(heightmapTex);
                    }
#endif
                    break;
                case Tool.Generate:
                    settings.generateRampCurve = EditorGUILayout.CurveField("Falloff", settings.generateRampCurve);
                    if(Event.current.commandName == "CurveChanged") { 
                        ClampAnimationCurve(settings.generateRampCurve);
                        if(Event.current.type != EventType.Used && Event.current.type != EventType.Layout) Event.current.Use();
                    }
                    
                    settings.generateHeight = EditorGUILayout.Slider("Height", settings.generateHeight, 0f, terrainSize.y);

                    EditorGUILayout.LabelField("Linear Ramp", EditorStyles.boldLabel);
                    settings.generateRampCurveInXAxis = GUIUtilities.RadioButtonsControl(new GUIContent("Ramp Axis"), settings.generateRampCurveInXAxis ? 0 : 1,
                        GUIContents.generateRampCurveOptions) == 0;
                    Rect createLinearRampRect = EditorGUILayout.GetControlRect(GUILayout.Height(22f));
                    if(GUI.Button(new Rect(createLinearRampRect.xMax - 150f, createLinearRampRect.y, 145f, 22f), "Create Linear Ramp")) {
                        CreateLinearRamp(settings.generateHeight);
                    }

                    EditorGUILayout.LabelField("Circular Ramp", EditorStyles.boldLabel);
                    Rect createCircularRampRect = EditorGUILayout.GetControlRect(GUILayout.Height(22f));
                    if(GUI.Button(new Rect(createCircularRampRect.xMax - 160f, createCircularRampRect.y, 155f, 22f), "Create Circular Ramp")) {
                        CreateCircularRamp(settings.generateHeight);
                    }
                    
                    break;
                case Tool.Settings:
                    Rect goToPreferencesButtonRect = EditorGUILayout.GetControlRect(false, 22f);
                    goToPreferencesButtonRect.xMin = goToPreferencesButtonRect.xMax - 190f;
                    if(GUI.Button(goToPreferencesButtonRect, "Terrain Former Preferences")) {
                        Type preferencesWindowType = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.PreferencesWindow");
                        MethodInfo showPreferencesWindowMethodInfo = preferencesWindowType.GetMethod("ShowPreferencesWindow", BindingFlags.NonPublic | BindingFlags.Static);
                        FieldInfo selectedSectionIndexFieldInfo = preferencesWindowType.GetField("m_SelectedSectionIndex", BindingFlags.NonPublic | BindingFlags.Instance);

                        FieldInfo sectionsFieldInfo = preferencesWindowType.GetField("m_Sections", BindingFlags.NonPublic | BindingFlags.Instance);

                        Type sectionType = preferencesWindowType.GetNestedType("Section", BindingFlags.NonPublic);

                        showPreferencesWindowMethodInfo.Invoke(null, null);
                        EditorWindow preferencesWindow = EditorWindow.GetWindowWithRect(preferencesWindowType, new Rect(100f, 100f, 500f, 400f), true, "Unity Preferences");

                        // Call PreferencesWindow.OnGUI method force it to add the custom sections so we have access to all sections.
                        MethodInfo preferencesWindowOnGUIMethodInfo = preferencesWindowType.GetMethod("OnGUI", BindingFlags.NonPublic | BindingFlags.Instance);
                        preferencesWindowOnGUIMethodInfo.Invoke(preferencesWindow, null);

                        IList sections = (IList)sectionsFieldInfo.GetValue(preferencesWindow);
                        for(int i = 0; i < sections.Count; i++) {
                            GUIContent sectionsContent = (GUIContent)sectionType.GetField("content").GetValue(sections[i]);
                            string sectionText = sectionsContent.text;
                            if(sectionText == "Terrain Former") {
                                selectedSectionIndexFieldInfo.SetValue(preferencesWindow, i);
                                break;
                            }
                        }
                    }
                    
                    EditorGUILayout.LabelField("Size", EditorStyles.boldLabel);

                    float newTerrainLateralSize = Mathf.Max(DelayedFloatField("Terrain Width/Length", firstTerrainData.size.x), 0f);
                    float newTerrainHeight = Mathf.Max(DelayedFloatField("Terrain Height", firstTerrainData.size.y), 0f);

                    bool terrainSizeChangedLaterally = newTerrainLateralSize != firstTerrainData.size.x;
                    if(terrainSizeChangedLaterally || newTerrainHeight != firstTerrainData.size.y) {
                        List<UnityEngine.Object> objectsThatWillBeModified = new List<UnityEngine.Object>();
                        
                        foreach(TerrainInformation ti in terrainInformations) {
                            objectsThatWillBeModified.Add(ti.terrainData);
                            if(terrainSizeChangedLaterally) objectsThatWillBeModified.Add(ti.transform);
                        }

                        // Calculate the center of the terrain grid and use that to decide where how to resposition the terrain grid cells.
                        Vector2 previousTerrainGridSize = new Vector2(numberOfTerrainsHorizontally * terrainSize.x, numberOfTerrainsVertically * terrainSize.z);
                        Vector3 centerOfTerrainGrid = new Vector3(firstTerrainTransform.position.x + previousTerrainGridSize.x * 0.5f, firstTerrainTransform.position.y,
                            firstTerrainTransform.position.z + previousTerrainGridSize.y * 0.5f);
                        Vector3 newTerrainGridSizeHalf = new Vector3(numberOfTerrainsHorizontally * newTerrainLateralSize * 0.5f, 0f, 
                            numberOfTerrainsVertically * newTerrainLateralSize * 0.5f);
                        
                        Undo.RegisterCompleteObjectUndo(objectsThatWillBeModified.ToArray(), terrainInformations.Count == 1 ? "Terrain Size Changed" : "Terrain Sizes Changed");
                        
                        foreach(TerrainInformation ti in terrainInformations) {
                            // Reposition the terrain grid (if there is more than one terrain) because the terrain size has changed laterally
                            if(terrainSizeChangedLaterally) {
                                ti.transform.position = new Vector3(
                                    (centerOfTerrainGrid.x - newTerrainGridSizeHalf.x) + ti.gridXCoordinate * newTerrainLateralSize, 
                                    ti.transform.position.y,
                                    (centerOfTerrainGrid.z - newTerrainGridSizeHalf.z) + ti.gridYCoordinate * newTerrainLateralSize
                                );
                            }

                            ti.terrainData.size = new Vector3(newTerrainLateralSize, newTerrainHeight, newTerrainLateralSize);
                        }

                        terrainSize = new Vector3(newTerrainLateralSize, newTerrainHeight, newTerrainLateralSize);
                    }

                    /**
                    * The following code is highly repetitive, but it must be written in this fashion. Writing this code in a more generalized fashion
                    * requires Reflection, but unfortunately virtually all properties are attributed with "MethodImplOptions.InternalCall", which as far as I
                    * know are not possible to be invoked using Reflection. As such, these properties must be set the manual way for all of their behaviours 
                    * to be executed.
                    */

                    EditorGUI.BeginChangeCheck();

                    // Base Terrain
                    bool newDrawHeightmap = EditorGUILayout.BeginToggleGroup("Base Terrain", firstTerrain.drawHeightmap);
                    if(firstTerrain.drawHeightmap != newDrawHeightmap) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrain.drawHeightmap = newDrawHeightmap;
                    }

                    EditorGUI.indentLevel = 1;
                    float newHeightmapPixelError = EditorGUILayout.Slider("Pixel Error", firstTerrain.heightmapPixelError, 1f, 200f);
                    if(firstTerrain.heightmapPixelError != newHeightmapPixelError) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrain.heightmapPixelError = newHeightmapPixelError;
                    }
                    
                    bool newCastShadows = EditorGUILayout.Toggle("Cast Shadows", firstTerrain.castShadows);
                    if(firstTerrain.castShadows != newCastShadows) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrain.castShadows = newCastShadows;
                    }
                    
                    Terrain.MaterialType newMaterialType = (Terrain.MaterialType)EditorGUILayout.EnumPopup("Material Type", firstTerrain.materialType);
                    if(firstTerrain.materialType != newMaterialType) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrain.materialType = newMaterialType;
                    }

                    switch(newMaterialType) {
                        case Terrain.MaterialType.BuiltInLegacySpecular:
                            EditorGUI.indentLevel++;
                            Color newLegacySpecular = EditorGUILayout.ColorField("Specular Colour", firstTerrain.legacySpecular);
                            if(firstTerrain.legacySpecular != newLegacySpecular) {
                                foreach(TerrainInformation ti in terrainInformations) ti.terrain.legacySpecular = newLegacySpecular;
                            }

                            float newLegacyShininess = EditorGUILayout.Slider("Shininess", firstTerrain.legacyShininess, 0.03f, 1f);
                            if(firstTerrain.legacyShininess != newLegacyShininess) {
                                foreach(TerrainInformation ti in terrainInformations) ti.terrain.legacyShininess = newLegacyShininess;
                            }
                            EditorGUI.indentLevel--;
                            break;
                        case Terrain.MaterialType.Custom:
                            EditorGUI.indentLevel++;
                            Material newMaterialTemplate = (Material)EditorGUILayout.ObjectField("Custom Material", firstTerrain.materialTemplate, typeof(Material), false);
                            if(firstTerrain.materialTemplate != newMaterialTemplate) {
                                foreach(TerrainInformation ti in terrainInformations) ti.terrain.materialTemplate = newMaterialTemplate;
                            }

                            if(firstTerrain.materialTemplate != null && TerrainSettings.ShaderHasTangentChannel(firstTerrain.materialTemplate.shader))
                                EditorGUILayout.HelpBox("Materials with shaders that require tangent geometry shouldn't be used on terrains. " +
                                    "Instead, use one of the shaders found under Nature/Terrain.", MessageType.Warning, true);
                            EditorGUI.indentLevel--;
                            break;
                    }

                    if(newMaterialType == Terrain.MaterialType.BuiltInStandard || newMaterialType == Terrain.MaterialType.Custom) {
                        ReflectionProbeUsage newReflectionProbeUsage = (ReflectionProbeUsage)EditorGUILayout.EnumPopup("Reflection Probes", firstTerrain.reflectionProbeUsage);

                        List<ReflectionProbeBlendInfo> tempClosestReflectionProbes = new List<ReflectionProbeBlendInfo>();
                        foreach(TerrainInformation ti in terrainInformations) {
                            ti.terrain.reflectionProbeUsage = newReflectionProbeUsage;
                        }
                        
                        if(firstTerrain.reflectionProbeUsage != ReflectionProbeUsage.Off) {
                            GUI.enabled = false;

                            foreach(TerrainInformation ti in terrainInformations) {
                                ti.terrain.GetClosestReflectionProbes(tempClosestReflectionProbes);
                                
                                for(int i = 0; i < tempClosestReflectionProbes.Count; i++) {
                                    Rect controlRect = EditorGUILayout.GetControlRect(GUILayout.Height(16f));
                                    
                                    float xOffset = controlRect.x + 32f;
                                    
                                    if(terrainInformations.Count > 1) {
                                        GUI.Label(new Rect(xOffset, controlRect.y, 105f, 16f), new GUIContent(ti.terrain.name, ti.terrain.name), EditorStyles.miniLabel);
                                        xOffset += 105f;
                                    } else {
                                        GUI.Label(new Rect(xOffset, controlRect.y, 16f, 16f), "#" + i, EditorStyles.miniLabel);
                                        xOffset += 16f;
                                    }
                                    
                                    float objectFieldWidth = controlRect.width - 50f - xOffset;
                                    EditorGUI.ObjectField(new Rect(xOffset, controlRect.y, objectFieldWidth, 16f), tempClosestReflectionProbes[i].probe, typeof(ReflectionProbe), true);
                                    xOffset += objectFieldWidth;
                                    GUI.Label(new Rect(xOffset, controlRect.y, 65f, 16f), "Weight " + tempClosestReflectionProbes[i].weight.ToString("f2"), EditorStyles.miniLabel);
                                }
                            }
                            GUI.enabled = true;
                        }
                    }

                    float newThickness = EditorGUILayout.FloatField("Thickness", firstTerrainData.thickness);
                    if(firstTerrainData.thickness != newThickness) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrainData.thickness = newThickness;
                    }
                    EditorGUI.indentLevel = 0;

                    EditorGUILayout.EndToggleGroup();

                    // Tree and Detail Objects
                    bool newDrawTreesAndFoliage = EditorGUILayout.BeginToggleGroup("Tree and Detail Objects", firstTerrain.drawTreesAndFoliage);
                    if(firstTerrain.drawTreesAndFoliage != newDrawTreesAndFoliage) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrain.drawTreesAndFoliage = newDrawTreesAndFoliage;
                    }

                    EditorGUI.indentLevel = 1;
                    bool newBakeLightProbesForTrees = EditorGUILayout.Toggle("Bake Light Probes for Trees", firstTerrain.bakeLightProbesForTrees);
                    if(firstTerrain.bakeLightProbesForTrees != newBakeLightProbesForTrees) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrain.bakeLightProbesForTrees = newBakeLightProbesForTrees;
                    }

                    float newDetailObjectDistance = EditorGUILayout.Slider("Detail Distance", firstTerrain.detailObjectDistance, 0f, 250f);
                    if(firstTerrain.detailObjectDistance != newDetailObjectDistance) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrain.detailObjectDistance = newDetailObjectDistance;
                    }

                    bool newCollectDetailPatches = EditorGUILayout.Toggle(GUIContents.collectDetailPatches, firstTerrain.collectDetailPatches);
                    if(firstTerrain.collectDetailPatches != newCollectDetailPatches) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrain.collectDetailPatches = newCollectDetailPatches;
                    }

                    float newDetailObjectDensity = EditorGUILayout.Slider("Detail Density", firstTerrain.detailObjectDensity, 0f, 1f);
                    if(firstTerrain.detailObjectDensity != newDetailObjectDensity) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrain.detailObjectDensity = newDetailObjectDensity;
                    }

                    float newTreeDistance = EditorGUILayout.Slider("Tree Distance", firstTerrain.treeDistance, 0f, 2000f);
                    if(firstTerrain.treeDistance != newTreeDistance) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrain.treeDistance = newTreeDistance;
                    }
                    
                    float newTreeBillboardDistance = EditorGUILayout.Slider("Billboard Start", firstTerrain.treeBillboardDistance, 5f, 2000f);
                    if(firstTerrain.treeBillboardDistance != newTreeBillboardDistance) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrain.treeBillboardDistance = newTreeBillboardDistance;
                    }

                    float newTreeCrossFadeLength = EditorGUILayout.Slider("Fade Length", firstTerrain.treeCrossFadeLength, 0f, 200f);
                    if(firstTerrain.treeCrossFadeLength != newTreeCrossFadeLength) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrain.treeCrossFadeLength = newTreeCrossFadeLength;
                    }

                    int newTreeMaximumFullLODCount = EditorGUILayout.IntSlider("Max. Mesh Trees", firstTerrain.treeMaximumFullLODCount, 0, 10000);
                    if(firstTerrain.treeMaximumFullLODCount != newTreeMaximumFullLODCount) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrain.treeMaximumFullLODCount = newTreeMaximumFullLODCount;
                    }

                    EditorGUI.indentLevel = 0;

                    EditorGUILayout.EndToggleGroup();
                    // If any tree/detail/base terrain settings have changed, redraw the scene view
                    if(EditorGUI.EndChangeCheck()) SceneView.RepaintAll();

                    GUILayout.Label("Wind Settings for Grass", EditorStyles.boldLabel);

                    float newWavingGrassStrength = EditorGUILayout.Slider("Strength", firstTerrainData.wavingGrassStrength, 0f, 1f);
                    if(firstTerrainData.wavingGrassStrength != newWavingGrassStrength) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrainData.wavingGrassStrength = newWavingGrassStrength;
                    }

                    float newWavingGrassSpeed = EditorGUILayout.Slider("Speed", firstTerrainData.wavingGrassSpeed, 0f, 1f);
                    if(firstTerrainData.wavingGrassSpeed != newWavingGrassSpeed) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrainData.wavingGrassSpeed = newWavingGrassSpeed;
                    }

                    float newWavingGrassAmount = EditorGUILayout.Slider("Bending", firstTerrainData.wavingGrassAmount, 0f, 1f);
                    if(firstTerrainData.wavingGrassAmount != newWavingGrassAmount) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrainData.wavingGrassAmount = newWavingGrassAmount;
                    }

                    Color newWavingGrassTint = EditorGUILayout.ColorField("Tint", firstTerrainData.wavingGrassTint);
                    if(firstTerrainData.wavingGrassTint != newWavingGrassTint) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrainData.wavingGrassTint = newWavingGrassTint;
                    }
                    
                    GUILayout.Label("Resolution", EditorStyles.boldLabel);

                    int newHeightmapResolution = EditorGUILayout.IntPopup(TerrainSettings.heightmapResolutionContent, firstTerrainData.heightmapResolution, 
                        TerrainSettings.heightmapResolutionsContents, TerrainSettings.heightmapResolutions);
                    if(firstTerrainData.heightmapResolution != newHeightmapResolution && 
                        EditorUtility.DisplayDialog("Terrain Former", "Changing the heightmap resolution will reset the heightmap.\n\nDo " +
                            "you want to change the heightmap resolution?", "Change Anyway", "Cancel")) {
                        RegisterUndoForTerrainGrid("Changed heightmap resolution");
                        foreach(TerrainInformation ti in terrainInformations) {
                            ti.terrainData.heightmapResolution = newHeightmapResolution;
                            ti.terrainData.size = terrainSize;
                        }
                        heightmapResolution = newHeightmapResolution;
                        Initialize(true);
                        UpdateAllHeightsFromSourceAssets();
                    }

                    int newAlphamapResolution = EditorGUILayout.IntPopup(TerrainSettings.alphamapResolutionContent, firstTerrainData.alphamapResolution, 
                        TerrainSettings.validTextureResolutionsContent, TerrainSettings.validTextureResolutions);
                    if(firstTerrainData.alphamapResolution != newAlphamapResolution &&
                        EditorUtility.DisplayDialog("Terrain Former", "Changing the alphamap resolution will reset the alphamap.\n\nDo you " +
                            "want to change the alphamap resolution?", "Change Anyway", "Cancel")) {
                        RegisterUndoForTerrainGrid("Changed alphmap resolution");
                        foreach(TerrainInformation ti in terrainInformations) ti.terrainData.alphamapResolution = newAlphamapResolution;
                        alphamapResolution = newAlphamapResolution;
                        Initialize(true);
                        UpdateAllAlphamapSamplesFromSourceAssets();
                    }

                    int newBaseMapResolution = EditorGUILayout.IntPopup(TerrainSettings.basemapResolutionContent, firstTerrainData.baseMapResolution, 
                        TerrainSettings.validTextureResolutionsContent, TerrainSettings.validTextureResolutions);
                    if(firstTerrainData.baseMapResolution != newBaseMapResolution) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrainData.baseMapResolution = newBaseMapResolution;
                    }
                    
                    float newBasemapDistance = EditorGUILayout.Slider(TerrainSettings.basemapDistanceContent, firstTerrain.basemapDistance, 0f, 2000f);
                    if(firstTerrain.basemapDistance != newBasemapDistance) {
                        foreach(TerrainInformation ti in terrainInformations) ti.terrain.basemapDistance = newBasemapDistance;
                    }

                    // Detail Resolution
                    int newDetailResolution = Utilities.RoundToNearestAndClamp(DelayedIntField(TerrainSettings.detailResolutionContent, firstTerrainData.detailResolution),
                        8, 0, 4048);
                    // Update all detail layers if the detail resolution has changed.
                    if(newDetailResolution != firstTerrainData.detailResolution &&
                        EditorUtility.DisplayDialog("Terrain Former", "Changing the detail map resolution will clear all details.\n\nDo you " +
                            "want to change the detail map resolution?", "Change Anyway", "Cancel")) {
                        List<int[,]> detailLayers = new List<int[,]>();
                        for(int i = 0; i < firstTerrainData.detailPrototypes.Length; i++) {
                            detailLayers.Add(firstTerrainData.GetDetailLayer(0, 0, firstTerrainData.detailWidth, firstTerrainData.detailHeight, i));
                        }
                        foreach(TerrainInformation terrainInformation in terrainInformations) {
                            terrainInformation.terrainData.SetDetailResolution(newDetailResolution, 8);
                            for(int i = 0; i < detailLayers.Count; i++) {
                                terrainInformation.terrainData.SetDetailLayer(0, 0, i, detailLayers[i]);
                            }
                        }
                    }

                    // Detail Resolution Per Patch
                    int currentDetailResolutionPerPatch = TerrainSettings.GetDetailResolutionPerPatch(firstTerrainData);
                    int newDetailResolutionPerPatch = Mathf.Clamp(DelayedIntField(TerrainSettings.detailResolutionPerPatchContent, currentDetailResolutionPerPatch), 8, 128);
                    if(newDetailResolutionPerPatch != currentDetailResolutionPerPatch) {
                        foreach(TerrainInformation terrainInformation in terrainInformations) {
                            terrainInformation.terrainData.SetDetailResolution(firstTerrainData.detailResolution, newDetailResolutionPerPatch);
                        }
                    }

                    if(firstTerrain.materialType != Terrain.MaterialType.Custom) {
                        firstTerrain.materialTemplate = null;
                    }

                    if(terrainInformations.Count == 1) {
                        terrainInformations[0].terrainData = (TerrainData)EditorGUILayout.ObjectField("Terrain Data Asset", terrainInformations[0].terrainData, typeof(TerrainData), false);
                    } else {
                        EditorGUILayout.LabelField("Terrain Data Assets", EditorStyles.boldLabel);
                        foreach(TerrainInformation ti in terrainInformations) {
                            ti.terrainData = (TerrainData)EditorGUILayout.ObjectField(ti.transform.name, ti.terrainData, typeof(TerrainData), false);
                        }
                    }

                    // Draw the terrain informations as visual representations
                    if(terrainInformations.Count > 1) {
                        GUILayout.Space(5f);
                        neighboursFoldout = GUIUtilities.FullClickRegionFoldout("Neighbours", neighboursFoldout, EditorStyles.foldout);
                        if(neighboursFoldout) {
                            Rect hoverRect = new Rect();
                            string hoverText = null;

                            const int neighboursCellSize = 30;
                            const int neighboursCellSizeMinusOne = neighboursCellSize - 1;

                            Rect neighboursGridRect = GUILayoutUtility.GetRect(Screen.width - 35f, numberOfTerrainsVertically * neighboursCellSize + 15f);
                            int neighboursGridRectWidth = neighboursCellSizeMinusOne * numberOfTerrainsHorizontally;
                            int neighboursGridRectHeight = neighboursCellSizeMinusOne * numberOfTerrainsVertically;
                            neighboursGridRect.yMin += 15f;
                            neighboursGridRect.xMin = Screen.width * 0.5f - neighboursGridRectWidth * 0.5f;
                            neighboursGridRect.width = neighboursGridRectWidth;

                            if(neighboursGridRect.Contains(Event.current.mousePosition)) Repaint();

                            GUIStyle boldLabelWithoutPadding = new GUIStyle(EditorStyles.boldLabel);
                            boldLabelWithoutPadding.padding = new RectOffset();
                            boldLabelWithoutPadding.alignment = TextAnchor.MiddleCenter;
                            // Axis Labels
                            GUI.Label(new Rect(Screen.width * 0.5f - 9f, neighboursGridRect.y - 15f, 20f, 10f), "Z", boldLabelWithoutPadding);
                            GUI.Label(new Rect(neighboursGridRect.xMax + 7f, neighboursGridRect.y + neighboursGridRectHeight * 0.5f - 6f, 10f, 10f), "X", boldLabelWithoutPadding);

                            foreach(TerrainInformation terrainInformation in terrainInformations) {
                                GUI.color = terrainInformation.terrain == firstTerrain && !isTerrainGridParentSelected ? new Color(0.4f, 0.4f, 0.75f) : Color.white;
                                Rect cellRect = new Rect(neighboursGridRect.x + terrainInformation.gridXCoordinate * neighboursCellSizeMinusOne, neighboursGridRect.y + 
                                    (numberOfTerrainsVertically - 1 - terrainInformation.gridYCoordinate) * neighboursCellSizeMinusOne, neighboursCellSize, neighboursCellSize);
                                
                                if(cellRect.Contains(Event.current.mousePosition)) {
                                    if(Event.current.type == EventType.MouseUp) {
                                        EditorGUIUtility.PingObject(terrainInformation.terrain.gameObject);
                                    } else {
                                        hoverText = terrainInformation.terrain.name;
                                        if(terrainInformation.terrain == firstTerrain && isTerrainGridParentSelected == false) hoverText += " (selected)";
                                        Vector2 calculatedSize = GUI.skin.box.CalcSize(new GUIContent(hoverText));
                                        hoverRect = new Rect(Mathf.Max(cellRect.x + 15f - calculatedSize.x * 0.5f, 0f), cellRect.y + calculatedSize.y + 5f, calculatedSize.x, calculatedSize.y);
                                    }
                                }
                                
                                if(numberOfTerrainsHorizontally >= 10 || numberOfTerrainsVertically >= 10) {
                                    TerrainFormerStyles.neighboursCellBox.fontSize = 8;
                                } else {
                                    TerrainFormerStyles.neighboursCellBox.fontSize = 8;
                                }
                                GUI.Box(cellRect, (terrainInformation.gridXCoordinate + 1) + "x" + (terrainInformation.gridYCoordinate + 1), TerrainFormerStyles.neighboursCellBox);
                            }

                            GUI.color = Color.white;

                            if(hoverText != null) {
                                GUI.Box(hoverRect, hoverText);
                            }
                        }
                    }
                    break;
            }

            float lastLabelWidth = EditorGUIUtility.labelWidth;
            
            if(CurrentTool >= firstNonMouseTool) return;

            GUILayout.Space(3f);

            /**
            * Brush Selection
            */
            if(settings.AlwaysShowBrushSelection || isSelectingBrush) {
                Rect brushesTitleRect = EditorGUILayout.GetControlRect();
                GUI.Label(brushesTitleRect, settings.AlwaysShowBrushSelection ? "Brushes" : "Select Brush", EditorStyles.boldLabel);

                if(settings.AlwaysShowBrushSelection) {
                    brushesTitleRect.xMin = brushesTitleRect.xMax - 300f;
                    GUI.Label(brushesTitleRect, CurrentBrush.name, TerrainFormerStyles.brushNameAlwaysShowBrushSelection);
                }
                
                if(settings.brushSelectionDisplayType == BrushSelectionDisplayType.Tabbed) {
                    string newBrushTab = GUIUtilities.BrushTypeToolbar(settings.modeSettings[CurrentTool].selectedBrushTab);
                    if(newBrushTab != settings.modeSettings[CurrentTool].selectedBrushTab) {
                        settings.modeSettings[CurrentTool].selectedBrushTab = newBrushTab;
                    }
                }

                string newlySelectedBrush = GUIUtilities.BrushSelectionGrid(settings.modeSettings[CurrentTool].selectedBrushId);
                if(newlySelectedBrush != settings.modeSettings[CurrentTool].selectedBrushId) {
                    settings.modeSettings[CurrentTool].selectedBrushId = newlySelectedBrush;
                    SelectedBrushChanged();
                }
            }

            if(settings.AlwaysShowBrushSelection) {
                GUILayout.Space(6f);
            } else if(isSelectingBrush) return;

            if(CurrentTool != Tool.RaiseOrLower) {
                GUILayout.Label("Brush", EditorStyles.boldLabel);
            }

            // The width of the area used to show the button to select a brush. Only applicable when AlwaysShowBrushSelection is false.
            float brushSelectionWidth = Mathf.Clamp(settings.brushPreviewSize + 28f, 80f, 84f);

            EditorGUILayout.BeginHorizontal(); // Brush Parameter Editor Horizontal Group
            
            // Draw Brush Paramater Editor
            if(settings.AlwaysShowBrushSelection) {
                EditorGUILayout.BeginVertical();
            } else {
                EditorGUILayout.BeginVertical(GUILayout.Width(Screen.width - brushSelectionWidth - 15f));
            }

            bool isBrushProcedural = CurrentBrush is ImageBrush == false;
            
            float newBrushSize = EditorGUILayout.Slider("Size", settings.modeSettings[CurrentTool].brushSize, MinBrushSize, terrainSize.x);
            if(newBrushSize != settings.modeSettings[CurrentTool].brushSize) {
                settings.modeSettings[CurrentTool].brushSize = newBrushSize;
                BrushSizeChanged();
            }

            float newBrushSpeed;
            switch(CurrentTool) {
                case Tool.PaintTexture:
                    newBrushSpeed = EditorGUILayout.Slider("Strength", settings.modeSettings[CurrentTool].brushSpeed, minBrushSpeed, 1f);
                    break;
                case Tool.Smooth:
                case Tool.Mould:
                    newBrushSpeed = EditorGUILayout.Slider("Speed", settings.modeSettings[CurrentTool].brushSpeed, minBrushSpeed, 1f);
                    break;
                default:
                    newBrushSpeed = EditorGUILayout.Slider("Speed", settings.modeSettings[CurrentTool].brushSpeed, minBrushSpeed, maxBrushSpeed);
                    break;
            }
            
            if(newBrushSpeed != settings.modeSettings[CurrentTool].brushSpeed) {
                settings.modeSettings[CurrentTool].brushSpeed = newBrushSpeed;
                BrushSpeedChanged();
            }
            
            GUIUtilities.LeftFillAndRightControl(
                fillControl: r => {
                    if(isBrushProcedural) {
                        EditorGUI.PrefixLabel(r, new GUIContent("Falloff"));
                    } else { 
                        Rect falloffToggleRect = new Rect(r);
                        falloffToggleRect.xMax = EditorGUIUtility.labelWidth;
                        bool newUseFalloffForCustomBrushes = EditorGUI.Toggle(falloffToggleRect, settings.modeSettings[CurrentTool].useFalloffForCustomBrushes);
                        if(newUseFalloffForCustomBrushes != settings.modeSettings[CurrentTool].useFalloffForCustomBrushes) {
                            settings.modeSettings[CurrentTool].useFalloffForCustomBrushes = newUseFalloffForCustomBrushes;
                            UpdateAllNecessaryPreviewTextures();
                            UpdateBrushProjectorTextureAndSamples();
                        }

                        Rect falloffToggleLabelRect = new Rect(falloffToggleRect);
                        falloffToggleLabelRect.xMin += 15f;
                        EditorGUI.PrefixLabel(falloffToggleLabelRect, new GUIContent("Falloff"));
                    }
                    Rect falloffAnimationCurveRect = new Rect(r);
                    falloffAnimationCurveRect.xMin = EditorGUIUtility.labelWidth + 14f;
                    settings.modeSettings[CurrentTool].brushFalloff = EditorGUI.CurveField(falloffAnimationCurveRect, settings.modeSettings[CurrentTool].brushFalloff);
                    if(Event.current.commandName == "CurveChanged") {
                        BrushFalloffChanged();
                        if(Event.current.type != EventType.Used && Event.current.type != EventType.Layout) Event.current.Use();
                    }
                },
                rightControl: r => {
                    using(new GUIUtilities.GUIEnabledBlock(isBrushProcedural || settings.modeSettings[CurrentTool].useFalloffForCustomBrushes)) {
                        Rect alphaFalloffLabelRect = new Rect(r);
                        alphaFalloffLabelRect.xMin += 13;
                        alphaFalloffLabelRect.xMax += 3;
                        GUI.Label(alphaFalloffLabelRect, "Invert");

                        Rect alphaFalloffRect = new Rect(r);
                        alphaFalloffRect.xMin--;
                        bool newUseAlphaFalloff = EditorGUI.Toggle(alphaFalloffRect, settings.modeSettings[CurrentTool].invertFalloff);
                        if(newUseAlphaFalloff != settings.modeSettings[CurrentTool].invertFalloff) {
                            settings.modeSettings[CurrentTool].invertFalloff = newUseAlphaFalloff;
                            UpdateAllNecessaryPreviewTextures();
                            UpdateBrushProjectorTextureAndSamples();
                        }
                    }
                },
                rightControlWidth: 54
            );

            if(isBrushProcedural == false && settings.modeSettings[CurrentTool].useFalloffForCustomBrushes == false) {
                GUI.enabled = false;
            }

            EditorGUI.indentLevel = 1;
            float newBrushRoundness = EditorGUILayout.Slider("Roundness", settings.modeSettings[CurrentTool].brushRoundness, 0f, 1f);
            if(newBrushRoundness != settings.modeSettings[CurrentTool].brushRoundness) {
                settings.modeSettings[CurrentTool].brushRoundness = newBrushRoundness;
                BrushRoundnessChanged();
            }
            EditorGUI.indentLevel = 0;

            if(isBrushProcedural == false && settings.modeSettings[CurrentTool].useFalloffForCustomBrushes == false) {
                GUI.enabled = true;
            }

            /**
            * Custom Brush Angle
            */
            GUI.enabled = CanBrushRotate();
            float newBrushAngle = EditorGUILayout.Slider("Angle", settings.modeSettings[CurrentTool].brushAngle, -180f, 180f);
            if(newBrushAngle != settings.modeSettings[CurrentTool].brushAngle) {
                float delta = settings.modeSettings[CurrentTool].brushAngle - newBrushAngle;
                settings.modeSettings[CurrentTool].brushAngle = newBrushAngle;
                BrushAngleDeltaChanged(delta);
            }
            GUI.enabled = true;

            /**
            * Invert Brush (for custom brushes only)
            */
            if(isBrushProcedural == false) {
                if(settings.invertBrushTexturesGlobally) {
                    GUI.enabled = false;
                    EditorGUILayout.Toggle("Invert", true);
                    GUI.enabled = true;
                } else {
                    bool newInvertBrushTexture = EditorGUILayout.Toggle("Invert", settings.modeSettings[CurrentTool].invertBrushTexture);
                    if(newInvertBrushTexture != settings.modeSettings[CurrentTool].invertBrushTexture) {
                        settings.modeSettings[CurrentTool].invertBrushTexture = newInvertBrushTexture;
                        InvertBrushTextureChanged();
                    }
                }
            }

            /**
            * Noise Brush Parameters
            */
            if(CurrentBrush is PerlinNoiseBrush) {
                EditorGUILayout.LabelField("Perlin Noise Brush", EditorStyles.boldLabel);
                EditorGUI.BeginChangeCheck();
                settings.perlinNoiseScale = EditorGUILayout.Slider("Scale", settings.perlinNoiseScale, 5f, 750f);
                if(EditorGUI.EndChangeCheck()) {
                    samplesDirty |= SamplesDirty.ProjectorTexture;
                    UpdateAllNecessaryPreviewTextures();
                }
                
                bool perlinNoiseMinMaxChanged = GUIUtilities.MinMaxWithFloatFields("Clipping", ref settings.perlinNoiseMin, ref settings.perlinNoiseMax, 0f, 1f, 3);
                if(perlinNoiseMinMaxChanged) {
                    samplesDirty |= SamplesDirty.ProjectorTexture;
                    UpdateAllNecessaryPreviewTextures();
                }
            }
            
            EditorGUILayout.EndVertical();

            if(settings.AlwaysShowBrushSelection == false) {
                GUILayout.Space(-4f);

                EditorGUILayout.BeginVertical(GUILayout.Width(brushSelectionWidth));

                GUILayout.Space(-2f);

                using(new EditorGUILayout.HorizontalScope()) {
                    GUILayout.FlexibleSpace();
                    GUILayout.Box(CurrentBrush.name, TerrainFormerStyles.miniBoldLabelCentered, GUILayout.Width(brushSelectionWidth - 17f), GUILayout.Height(24f));
                    GUILayout.FlexibleSpace();
                }

                // Draw Brush Preview
                using(new EditorGUILayout.HorizontalScope()) {
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button(CurrentBrush.previewTexture, GUIStyle.none)) {
                        ToggleSelectingBrush();
                    }
                    GUILayout.FlexibleSpace();
                }

                // Draw Select/Cancel Brush Selection Button
                using(new EditorGUILayout.HorizontalScope()) {
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button("Select", TerrainFormerStyles.miniButtonWithoutMargin, GUILayout.Width(60f), GUILayout.Height(18f))) {
                        ToggleSelectingBrush();
                    }
                    GUILayout.FlexibleSpace();
                }

                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndHorizontal(); // Brush Parameter Editor Horizontal Group

            /**
            * Behaviour
            */
            behaviourGroupUnfolded = GUIUtilities.FullClickRegionFoldout("Behaviour", behaviourGroupUnfolded, TerrainFormerStyles.behaviourGroupFoldout);

            if(behaviourGroupUnfolded) {
                /**
                * TODO: Versions older than Unity 2017.1 don't handle out and ref keywords in blocks with parameters labels for some reason, so no
                * pretty parameter labels :(
                */
                const float minSpacingBounds = 0.1f;
                const float maxSpacingBounds = 30f;
                if(GUIUtilities.TogglMinMaxWithFloatFields(
                    "Random Spacing",                                       // label
                    ref settings.modeSettings[CurrentTool].useBrushSpacing, // toggleValue
                    ref settings.modeSettings[CurrentTool].minBrushSpacing, // minValue
                    ref settings.modeSettings[CurrentTool].maxBrushSpacing, // maxValue
                    minSpacingBounds,                                       // minValueBoundary
                    maxSpacingBounds,                                       // maxValueBoundary
                    5                                                       // significantDigits
                )) {
                    // If the min/max values were changed, assume the user wants brush spacing to be enabled.
                    settings.modeSettings[CurrentTool].useBrushSpacing = true;
                }

                const float minRandomOffset = 0.001f;
                float maxRandomOffset = Mathf.Min(firstTerrainData.heightmapWidth, firstTerrainData.heightmapHeight) * 0.5f;
                GUIUtilities.ToggleAndControl(
                    new GUIContent("Random Offset"),                        // label
                    ref settings.modeSettings[CurrentTool].useRandomOffset, // enableFillControl
                    r => {                                                // fillControl
                    EditorGUI.BeginChangeCheck();
                        settings.modeSettings[CurrentTool].randomOffset = EditorGUI.Slider(r, settings.modeSettings[CurrentTool].randomOffset, minRandomOffset, maxRandomOffset);
                        if(EditorGUI.EndChangeCheck()) {
                            settings.modeSettings[CurrentTool].useRandomOffset = true;
                        }
                    }
                );

                GUI.enabled = CanBrushRotate();
                if(GUIUtilities.TogglMinMaxWithFloatFields(
                    "Random Rotation",                                        // label
                    ref settings.modeSettings[CurrentTool].useRandomRotation, // toggleValue
                    ref settings.modeSettings[CurrentTool].minRandomRotation, // minValue
                    ref settings.modeSettings[CurrentTool].maxRandomRotation, // maxValue
                    -180f,                                  // minValueBoundary
                    180f,                                  // maxValueBoundary
                    5                                                         // significantDigits
                )) {
                    settings.modeSettings[CurrentTool].useRandomRotation = true;
                }
                GUI.enabled = true;
            }
            TerrainMismatchManager.Draw();

            EditorGUIUtility.labelWidth = lastLabelWidth;
        }

        private void loadwcRoot(GameObject wcRoot)
        {
          //  Terrain[] wcTerrains = wcRoot.GetComponentsInChildren<Terrain>();
        
            Terrain[] tfTerrains = (target as TerrainFormer).GetComponentsInChildren<Terrain>();
             
            for (int i = 0; i < 64; i++)
            {

             
                //  Debug.Log( AssetDatabase.GetAssetPath(item.terrainData)+" ===> " + AssetDatabase.GetAssetPath(tfTerrains[i++].terrainData));
                int col=i%8;
                int row = i / 8;

                Terrain item = wcRoot.transform.Find("SplittedTerrain_"+col+"_"+row).GetComponent<Terrain>();
                int j = i;// col * 8 + row;
                string newPath = AssetDatabase.GetAssetPath(tfTerrains[j].terrainData);
                AssetDatabase.MoveAssetToTrash(newPath);
                
              AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(item.terrainData), newPath);
                tfTerrains[j].terrainData = AssetDatabase.LoadAssetAtPath<TerrainData>(newPath);
                tfTerrains[j].GetComponent<TerrainCollider>().terrainData = tfTerrains[j].terrainData;
                
            }
        }

        // Warpper for the DelayedInt/Float fields, with older versions of Unity using normal non-delayed fields
        private float DelayedFloatField(string label, float value) {
#if UNITY_5_3_OR_NEWER
            return EditorGUILayout.DelayedFloatField(label, value);
#else
            return EditorGUILayout.FloatField(label, value);
#endif
        }

        
        private int DelayedIntField(GUIContent content, int value) {
#if UNITY_5_3_OR_NEWER
            return EditorGUILayout.DelayedIntField(content, value);
#else
            return EditorGUILayout.IntField(content, value);
#endif
        }

        private void RunCommandOnTerrainGrid(TerrainCommand terrainCommand, int iterations) {
            if(toolSamplesHorizontally * toolSamplesVertically > 25000000) {
                if(EditorUtility.DisplayDialog("Terrain Former", string.Format("Are you sure you want to execute {0} on the terrain grid " + 
                    "knowing this might take a long time?", terrainCommand.GetName()), "Execute Anyway", "Cancel") == false) { 
                    return;
                }
            }

            Type commandType = terrainCommand.GetType();
            if(commandType != typeof(SmoothCommand) && commandType != typeof(MouldCommand)) {
                Debug.LogError(string.Format("The type {0} is not supported to run across an entire terrain grid.", terrainCommand.GetName()));
            }

            // Create a dummy dialog as early as possible to show that progress has started.
            bool cancelEarly = EditorUtility.DisplayCancelableProgressBar("Terrain Former", string.Format("Applying {0} to the entire terrain grid…", 
                terrainCommand.GetName()), 0f);

            globalCommandArea = new CommandArea(0, 0, 0, 0, heightmapWidth, heightmapHeight);
            
            // Tell each TerrainInformation we are modifying to the entirety of it.
            foreach(TerrainInformation terrainInformation in terrainInformations) {
                terrainInformation.commandArea = new CommandArea(0, 0, 0, 0, heightmapResolution, heightmapResolution);
            }

            if(terrainCommand is MouldCommand) {
                IgnoreRaycastsOnTerrains();
            }
            
            UpdateAllUnmodifiedHeights();

            Event fakeEvent = new Event();
            float progress;
            for(int i = 0; i < iterations; i++) {
                if(terrainCommand is MouldCommand) {
                    UpdateMouldToolRaycastMask(true);
                }

                terrainCommand.Execute(fakeEvent, globalCommandArea, true);

                UpdateAllUnmodifiedHeights();

                progress = (float)(i + 1) / iterations;
                if(EditorUtility.DisplayCancelableProgressBar("Terrain Former", string.Format("Applying {0} to the entire terrain grid…", 
                    terrainCommand.GetName()), progress) || cancelEarly) {
                    EditorUtility.ClearProgressBar();
                    Undo.PerformUndo();
                    return;
                }                
            }

            EditorUtility.ClearProgressBar();
            
            ApplyTerrainCommandChanges(true);

            if(terrainCommand is MouldCommand) {
                ResetLayerOfTerrains();
            }

            SceneView.lastActiveSceneView.Repaint();
        }

#if(UNITY_2017_1_OR_NEWER == false)
        // Make sure the toolbar tooltip actually is hidden after the mouse has left the area.
        public override bool RequiresConstantRepaint() {
            return isShowingToolTooltip;
        }
#endif

        private void OnSceneGUICallback(SceneView sceneView) {
            if(this == null) {
                OnDisable();
                return;
            }

            // There are magical times where Terrain Former didn't receive the OnDisable message and continues to subscribe to OnSceneGUI
            if(terrainFormer == null) {
                OnDisable();
                return;
            }
            if(Initialize() == false) return;

            if(CurrentTool == Tool.None) {
                SetCursorEnabled(false);
            } else if((Event.current.control && mouseIsDown) == false) {
                UpdateProjector();
            }
            
            Event currentEvent = Event.current;
            
            const int terrainEditorHash = 769275123; // The constant result of "TerrainFormerEditor".GetHashCode(); 
            int controlId = GUIUtility.GetControlID(terrainEditorHash, FocusType.Passive);
            
            /**
            * Draw scene-view information
            */
            if(IsCurrentModeSculptive() && settings.showSceneViewInformation && 
                (settings.displaySceneViewCurrentHeight || settings.displaySceneViewCurrentTool || settings.displaySceneViewSculptOntoMode)) {
                /**
                * For some reason this must be set for the SceneViewPanel to be rendered correctly - this won't be an issue if it was simple called in a 
                * OnSceneGUI "message". However multiple OnSceneGUI calls don't come through if there are multiple inspectors tabs/windwos at once.
                */
                GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

                Handles.BeginGUI();

                if(TerrainFormerStyles.sceneViewInformationArea == null) {
                    TerrainFormerStyles.sceneViewInformationArea = new GUIStyle(GUI.skin.box);
                    TerrainFormerStyles.sceneViewInformationArea.padding = new RectOffset(5, 0, 5, 0);
                }
                if(TerrainFormerStyles.sceneViewInformationArea.normal.background == null || TerrainFormerStyles.sceneViewInformationArea.normal.background.name == "OL box") {
                    TerrainFormerStyles.sceneViewInformationArea.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>(settings.mainDirectory + "Textures/SceneInfoPanel.PSD");
                    TerrainFormerStyles.sceneViewInformationArea.border = new RectOffset(12, 12, 12, 12);
                }

                int lines = settings.displaySceneViewCurrentHeight ? 1 : 0;
                lines += settings.displaySceneViewCurrentTool ? 1 : 0;
                lines += settings.displaySceneViewSculptOntoMode ? 1 : 0;

                GUILayout.BeginArea(new Rect(5f, 5f, 190f, 15f * lines + 14f), TerrainFormerStyles.sceneViewInformationArea);

                const float parameterLabelOffset = 7f;
                const float valueParameterLeftOffset = 90f;
                float yOffset = 7f;

                if(settings.displaySceneViewCurrentTool) {
                    EditorGUI.LabelField(new Rect(parameterLabelOffset, yOffset, 135f, 18f), "Tool:");
                    string toolNameDisplay = toolsGUIContents[(int)currentTool.Value].tooltip;
                    if(CurrentTool == Tool.Flatten) toolNameDisplay += string.Format(" ({0})", settings.flattenMode);
                    GUI.Label(new Rect(valueParameterLeftOffset, yOffset, 135f, 18f), toolNameDisplay);
                    yOffset += 15f;
                }
                if(settings.displaySceneViewCurrentHeight) {
                    float height;
                    EditorGUI.LabelField(new Rect(parameterLabelOffset, yOffset, 135f, 18f), "Height:");
                    if(currentEvent.control && mouseIsDown) {
                        EditorGUI.LabelField(new Rect(valueParameterLeftOffset, yOffset, 135f, 18f), lastClickPosition.y.ToString("0.00"));
                    } else if(GetTerrainHeightAtMousePosition(out height)) {
                        EditorGUI.LabelField(new Rect(valueParameterLeftOffset, yOffset, 135f, 18f), height.ToString("0.00"));
                    } else {
                        EditorGUI.LabelField(new Rect(valueParameterLeftOffset, yOffset, 135f, 18f), "0.00");
                    }
                    yOffset += 15f;
                }
                if(settings.displaySceneViewSculptOntoMode) {
                    EditorGUI.LabelField(new Rect(parameterLabelOffset, yOffset, 135f, 18f), "Sculpt Onto:");
                    if(CurrentTool == Tool.SetHeight || CurrentTool == Tool.Flatten) {
                        EditorGUI.LabelField(new Rect(valueParameterLeftOffset, yOffset, 135f, 18f), "Plane (locked)");
                    } else {
                        EditorGUI.LabelField(new Rect(valueParameterLeftOffset, yOffset, 135f, 18f), GUIContents.raycastModes[settings.raycastOntoFlatPlane ? 0 : 1]);
                    }
                }
                GUILayout.EndArea();
                Handles.EndGUI();
            }

            if(firstTerrain == null || firstTerrainData == null) return;

            CheckKeyboardShortcuts(currentEvent);

            if(GUIUtility.hotControl != 0 && GUIUtility.hotControl != controlId) return;
            if(IsCurrentModeSculptive() == false) return;
            
            Vector3 mouseWorldspacePosition;
            bool doesMouseRaycastHit = GetMousePositionInWorldSpace(out mouseWorldspacePosition); 

            /**
            * Frame Selected (Shortcut: F)
            */
            if(currentEvent.type == EventType.ExecuteCommand && currentEvent.commandName == "FrameSelected") {
                if(doesMouseRaycastHit) {
                    SceneView.currentDrawingSceneView.LookAt(pos: mouseWorldspacePosition, rot: SceneView.currentDrawingSceneView.rotation, 
                        newSize: GetCurrentToolSettings().brushSize * 1.2f);
                } else {
                    float largestTerrainAxis = Mathf.Max(numberOfTerrainsHorizontally * terrainSize.x, numberOfTerrainsVertically * terrainSize.z);
                    Vector3 centerOfTerrainGrid = firstTerrainTransform.position + new Vector3(numberOfTerrainsHorizontally * terrainSize.x * 0.5f, 0f, 
                        numberOfTerrainsVertically * terrainSize.z * 0.5f);
                    SceneView.currentDrawingSceneView.LookAt(centerOfTerrainGrid, SceneView.currentDrawingSceneView.rotation, largestTerrainAxis * 1f);
                }
                currentEvent.Use();
            }

            EventType editorEventType = currentEvent.GetTypeForControl(controlId);
            // Update mouse-related fields
            if(currentEvent.isMouse) {
                if(mousePosition == Vector2.zero) {
                    lastMousePosition = currentEvent.mousePosition;
                } else {
                    lastMousePosition = mousePosition;
                }

                mousePosition = currentEvent.mousePosition;

                if(editorEventType == EventType.MouseDown) {
                    currentTotalMouseDelta = 0;
                } else if(mouseIsDown) {
                    currentTotalMouseDelta += mousePosition.y - lastMousePosition.y;
                }
            }
            
            // Only accept left clicks
            if(currentEvent.button != 0) return;

            switch(editorEventType) {
                case EventType.MouseMove:
                    // TODO: Only repaint if the brush cursor is visible
                    HandleUtility.Repaint();
                    break;
                // MouseDown will execute the same logic as MouseDrag
                case EventType.MouseDown:
                case EventType.MouseDrag:
                    /*
                    * Break if any of the following rules are true:
                    * 1) The event happening for this window is a MouseDrag event and the hotControl isn't this window
                    * 2) Alt + Click have been executed
                    * 3) The HandleUtllity finds a control closer to this control
                    */
                    if((editorEventType == EventType.MouseDrag && GUIUtility.hotControl != controlId) ||
                        (currentEvent.alt || currentEvent.button != 0) ||
                        HandleUtility.nearestControl != controlId) {
                        break;
                    }

                    if(currentEvent.type != EventType.MouseDown) {
                        currentEvent.Use();
                        break;
                    }
                    currentEvent.Use();

                    /**
                    * To make sure the initial press down always sculpts the terrain while spacing is active, set 
                    * the mouseSpacingDistance to a high value to always activate it straight away
                    */
                    mouseSpacingDistance = float.MaxValue;

                    UpdateRandomSpacing();
                    GUIUtility.hotControl = controlId;

                    switch(CurrentTool) {
                        case Tool.RaiseOrLower:
                            currentCommand = new RaiseOrLowerCommand(GetBrushSamplesWithSpeed());
                            break;
                        case Tool.Smooth:
                            currentCommand = new SmoothCommand(GetBrushSamplesWithSpeed(), settings.boxFilterSize, heightmapWidth, heightmapHeight);
                            break;
                        case Tool.SetHeight:
                            currentCommand = new SetHeightCommand(GetBrushSamplesWithSpeed());
                            break;
                        case Tool.Flatten:
                            currentCommand = new FlattenCommand(GetBrushSamplesWithSpeed(), (mouseWorldspacePosition.y - firstTerrain.transform.position.y) / terrainSize.y);
                            break;
                        case Tool.Mould:
                            currentCommand = new MouldCommand(GetBrushSamplesWithSpeed(), settings.mouldToolBoxFilterSize, heightmapWidth, heightmapHeight);
                            IgnoreRaycastsOnTerrains();
                            break;
                        case Tool.PaintTexture:
                            currentCommand = new PaintTextureCommand(GetBrushSamplesWithSpeed());
                            break;
                    }

                    if(CurrentTool != Tool.PaintTexture) {
                        UpdateAllUnmodifiedHeights();
                    }

                    Vector3 hitPosition;
                    Vector2 uv;
                    if(Raycast(out hitPosition, out uv)) {
                        lastWorldspaceMousePosition = hitPosition;
                        lastClickPosition = hitPosition;
                        mouseIsDown = true;
                    } else if(currentEvent.shift && CurrentTool == Tool.SetHeight) {
                        mouseIsDown = true;
                    }
                    break;
                case EventType.MouseUp:
                    // Reset the hotControl to nothing as long as it matches the TerrainEditor controlID
                    if(GUIUtility.hotControl != controlId) break;

                    GUIUtility.hotControl = 0;
                    
                    foreach(TerrainInformation terrainInformation in terrainInformations) {
                        if(terrainInformation.commandArea == null) continue;

                        // Render all aspects of terrain (heightmap, trees and details)
                        terrainInformation.terrain.editorRenderFlags = TerrainRenderFlags.all;
                        
                        if(CurrentTool == Tool.PaintTexture) {
                            terrainDataSetBasemapDirtyMethodInfo.Invoke(terrainInformation.terrainData, new object[] { true });
                        }
                        
                        if(settings.alwaysUpdateTerrainLODs == false) {
#if UNITY_5_2_OR_NEWER || UNITY_2017_1_OR_NEWER
                            terrainInformation.terrain.ApplyDelayedHeightmapModification();
#else
                            applyDelayedHeightmapModificationMethodInfo.Invoke(terrainInformation.terrain, null);
#endif
                        }
                    }

                    gridPlane.SetActive(false);
                    
                    mouseIsDown = false;

                    if(CurrentTool == Tool.Mould) {
                        ResetLayerOfTerrains();
                    }

                    currentCommand = null;
                    lastClickPosition = Vector3.zero;
                    
                    currentEvent.Use();
                    break;
                case EventType.KeyUp:
                    // If a kew has been released, make sure any keyboard shortcuts have their changes applied via UpdateDirtyBrushSamples
                    UpdateDirtyBrushSamples();
                    break;
                case EventType.Repaint:
                    SetCursorEnabled(false);
                    break;
                case EventType.Layout:
                    if(CurrentTool == Tool.None) break;

                    // Sets the ID of the default control. If there is no other handle being hovered over, it will choose this value
                    HandleUtility.AddDefaultControl(controlId);
                    break;
            }
            
            if(mouseIsDown == false || doesMouseRaycastHit == false || editorEventType != EventType.Layout) return;

            if(settings.modeSettings[CurrentTool].useBrushSpacing) {
                mouseSpacingDistance += (new Vector2(lastWorldspaceMousePosition.x, lastWorldspaceMousePosition.z) -
                    new Vector2(mouseWorldspacePosition.x, mouseWorldspacePosition.z)).magnitude;
            }

            Vector3 finalMousePosition;
            if(CurrentTool < firstNonMouseTool && currentEvent.control) {
                finalMousePosition = lastClickPosition;
            } else {
                finalMousePosition = mouseWorldspacePosition;
            }

            // Apply the random offset to the mouse position (if necessary)
            if(currentEvent.control == false && settings.modeSettings[CurrentTool].useRandomOffset) {
                Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * settings.modeSettings[CurrentTool].randomOffset;
                finalMousePosition += new Vector3(randomOffset.x, 0f, randomOffset.y);
            }

            /**
            * Calculate the command coordinates for each terrain information which determines which area of a given terrain (if at all) 
            * will have the current command applied to it
            */
            UpdateCommandCoordinatesForAllTerrains(finalMousePosition);

            UpdateGlobalCommandCoordinates(finalMousePosition);

            /**
            * Update the grid position
            */
            if(settings.showSculptingGridPlane) {
                if(gridPlane.activeSelf == false) {
                    gridPlane.SetActive(true);
                }

                Vector3 gridPosition;
                // If the current tool is interactive, keep the grid at the lastGridPosition
                if(currentEvent.control) {
                    gridPosition = new Vector3(lastClickPosition.x, lastClickPosition.y + 0.001f, lastClickPosition.z);
                } else {
                    gridPosition = new Vector3(mouseWorldspacePosition.x, lastClickPosition.y + 0.001f, mouseWorldspacePosition.z);
                }
                float gridPlaneDistance = Mathf.Abs(lastClickPosition.y - SceneView.currentDrawingSceneView.camera.transform.position.y);
                float gridPlaneSize = settings.modeSettings[CurrentTool].brushSize * 1.2f;
                gridPlane.transform.position = gridPosition;
                gridPlane.transform.localScale = Vector3.one * gridPlaneSize;

                // Get the Logarithm of base 10 from the distance to get a power to mutliple the grid scale by
                float power = Mathf.Round(Mathf.Log10(gridPlaneDistance) - 1);

                // Make the grid appear as if it's being illuminated by the cursor but keeping the grids remain within unit size tiles
                gridPlaneMaterial.mainTextureOffset = new Vector2(gridPosition.x, gridPosition.z) / Mathf.Pow(10f, power);

                gridPlaneMaterial.mainTextureScale = new Vector2(gridPlaneSize, gridPlaneSize) / Mathf.Pow(10f, power);
            }

            /**
            * Only allow the various Behaviours to be active when control isn't pressed to make these behaviours 
            * not occur while using interactive tools
            */
            if(currentEvent.control == false) {
                float spacing = settings.modeSettings[CurrentTool].brushSize * randomSpacing;

                // If brush spacing is enabled, do not update the current command until the cursor has exceeded the required distance
                if(settings.modeSettings[CurrentTool].useBrushSpacing && mouseSpacingDistance < spacing) {
                    lastWorldspaceMousePosition = mouseWorldspacePosition;
                    return;
                } else {
                    UpdateRandomSpacing();
                    mouseSpacingDistance = 0f;
                }

                if(settings.modeSettings[CurrentTool].useRandomRotation && CanBrushRotate()) {
                    RotateTemporaryBrushSamples();
                    currentCommand.brushSamples = temporarySamples;
                }
            }

            UpdateDirtyBrushSamples();

            if(globalCommandArea != null && CurrentTool == Tool.Mould) {
                // Update the raycast mask before running the jobs since raycasting currently only can be done on the main thread.
                UpdateMouldToolRaycastMask(false);
            }

            /**
            * Execute the current event
            */
            if(globalCommandArea != null) {
                currentCommand.Execute(currentEvent, globalCommandArea, false);
            }

            if(CurrentTool == Tool.Smooth || CurrentTool == Tool.Mould) {
                UpdateAllUnmodifiedHeights();
            }

            brushProjectorGameObject.SetActive(true);

            ApplyTerrainCommandChanges(false);

            lastWorldspaceMousePosition = mouseWorldspacePosition;
        }

        private bool CanBrushRotate() {
            return CurrentBrush.GetType() != typeof(FalloffBrush) || settings.modeSettings[CurrentTool].brushRoundness != 1f;
        }

        private void UpdateMouldToolRaycastMask(bool applyToEntireTerrainGrid) {
            ResizeToolScratchArray(globalCommandArea.width, globalCommandArea.height);

            Vector3 firstTerrainPosition = firstTerrainTransform.position;
            float gridPositionX = (1f / (heightmapWidth - 1)) * numberOfTerrainsHorizontally * terrainSize.x;
            float gridPositionY = (1f / (heightmapHeight - 1)) * numberOfTerrainsVertically * terrainSize.z;
                        
            const int boxCastSizeInSamples = 16;
#if UNITY_5_3_OR_NEWER
            const float halfBoxCastSizeInSamples = boxCastSizeInSamples / 2f;
            const float extraBoxCastGap = 0.2501f; // There must be a slight gap since things at the exact y-position of the boxcast will be ignored.
            float boxCastLength = terrainSize.y + extraBoxCastGap;
            float boxCastHalfSize = (halfBoxCastSizeInSamples / heightmapResolution) * terrainSize.x;
            Vector3 boxCastExtents = new Vector3(boxCastHalfSize, 0.125f, boxCastHalfSize);
#endif

            float raycastYOrigin = (settings.mouldToolRaycastTopDown ? firstTerrainPosition.y + terrainSize.y : firstTerrainPosition.y);

            float brushSample;
            RaycastHit hitInfo;
            int tilesY = Mathf.CeilToInt((float)globalCommandArea.height / boxCastSizeInSamples);
            int tilesX = Mathf.CeilToInt((float)globalCommandArea.width / boxCastSizeInSamples);
            for(int tY = 0; tY < tilesY; tY++) {
                for(int tX = 0; tX < tilesX; tX++) {
                    int yStart = tY * boxCastSizeInSamples;
                    int yEnd = Math.Min(yStart + boxCastSizeInSamples, globalCommandArea.height);
                    int xStart = tX * boxCastSizeInSamples;
                    int xEnd = Math.Min(xStart + boxCastSizeInSamples, globalCommandArea.width);

#if UNITY_5_3_OR_NEWER
                    // Instead of doing a raycast per segment, do a boxcast first to see if there is even anything there
                    Vector3 boxCastHighOrigin = new Vector3(
                        x: firstTerrainPosition.x + (tX * boxCastSizeInSamples + halfBoxCastSizeInSamples + globalCommandArea.leftOffset) * gridPositionX,
                        y: firstTerrainPosition.y + terrainSize.y + extraBoxCastGap,
                        z: firstTerrainPosition.z + (tY * boxCastSizeInSamples + halfBoxCastSizeInSamples + globalCommandArea.bottomOffset) * gridPositionY
                    );
                    Vector3 boxCastLowOrigin = new Vector3(
                        x: firstTerrainPosition.x + (tX * boxCastSizeInSamples + halfBoxCastSizeInSamples + globalCommandArea.leftOffset) * gridPositionX,
                        y: firstTerrainPosition.y,
                        z: firstTerrainPosition.z + (tY * boxCastSizeInSamples + halfBoxCastSizeInSamples + globalCommandArea.bottomOffset) * gridPositionY
                    );
                    
                    /**
                    * Unfortunately we need to box cast in both directions to handle the cases where the box cast fails while raycasts would hit such as 
                    * when there is protruding objects going past the bounds of the terrain
                    */
                    if(Physics.BoxCast(boxCastHighOrigin, boxCastExtents, Utilities.downDirection, out hitInfo, Quaternion.identity, boxCastLength,
                        Utilities.ignoreRaycastLayerMask, QueryTriggerInteraction.Ignore) == false &&
                        Physics.BoxCast(boxCastLowOrigin, boxCastExtents, Utilities.upDirection, out hitInfo, Quaternion.identity, boxCastLength,
                        Utilities.ignoreRaycastLayerMask, QueryTriggerInteraction.Ignore) == false
                        ) {
                        for(int y = yStart; y < yEnd; y++) {
                            for(int x = xStart; x < xEnd; x++) {
                                toolScratchArray[x, y] = -1f;
                            }
                        }
                        continue;
                    }
#endif

                    for(int y = yStart; y < yEnd; y++) {
                        for(int x = xStart; x < xEnd; x++) {
                            brushSample = applyToEntireTerrainGrid ? 1f : currentCommand.brushSamples[x + globalCommandArea.clippedLeft, y + globalCommandArea.clippedBottom];
                            if(brushSample == 0f) continue;

                            if(Physics.Raycast(
                                new Vector3(                                           // origin
                                    x: firstTerrainPosition.x + (x + globalCommandArea.leftOffset) * gridPositionX,
                                    y: raycastYOrigin,
                                    z: firstTerrainPosition.z + (y + globalCommandArea.bottomOffset) * gridPositionY),
                                settings.mouldToolRaycastTopDown ? Utilities.downDirection : Utilities.upDirection, // direction
                                out hitInfo,                                           // hitInfo
                                terrainSize.y,                                         // maxDistance
                                Utilities.ignoreRaycastLayerMask                       // layerMask
#if UNITY_5_2_OR_NEWER
                                ,QueryTriggerInteraction.Ignore                        // queryTriggerInteraction
#endif
                                )) {
                                toolScratchArray[x, y] = hitInfo.point.y;
                            } else {
                                toolScratchArray[x, y] = -1f;
                            }
                        }
                    }
                }
            }
        }

        internal static void ResizeToolScratchArray(int width, int height) {
            // Grow the generic tool scratch like a list, but we still have the performance of an array.
            if(width > toolScratchArray.GetLength(0) || height > toolScratchArray.GetLength(1)) {
                int longestCommandAreaAxis = Math.Max(width, height);
                int nearestRoundedSize = (int)Math.Pow(2d, Math.Ceiling(Math.Log(longestCommandAreaAxis - 1, 2d))) + 1;
                toolScratchArray = new float[nearestRoundedSize, nearestRoundedSize];
            }
        }

        // We don't want to allocate this every frame, but we do have to pollute this class even more with a one use field
        private static readonly object[] objectArrayWithFalseValue = { false }; 
        private static float[,,] newAlphamaps;
        private static float[,] newHeights;
        private void ApplyTerrainCommandChanges(bool overrideAlwaysUpdateTerrainLODs) {
            // Update each terrainInfo's updated terrain region
            foreach(TerrainInformation terrainInfo in terrainInformations) {
                if(terrainInfo.commandArea == null || terrainInfo.hasChangedSinceLastSetHeights == false) continue;

                terrainInfo.hasChangedSinceLastSetHeights = false;
               // Debug.Log("ApplyTerrainCommandChanges," + terrainInfo.commandArea.height + "," + terrainInfo.commandArea.width);

                terrainInfo.terrain.editorRenderFlags = TerrainRenderFlags.heightmap;

                if(currentCommand is PaintTextureCommand) {
                    if(newAlphamaps == null || newAlphamaps.GetLength(0) != terrainInfo.commandArea.height || newAlphamaps.GetLength(1) != terrainInfo.commandArea.width) {
                        newAlphamaps = new float[terrainInfo.commandArea.height, terrainInfo.commandArea.width, firstTerrainData.alphamapLayers];
                    }
                    for(int l = 0; l < firstTerrainData.alphamapLayers; l++) {
                        for(int x = 0; x < terrainInfo.commandArea.width; x++) {
                            for(int y = 0; y < terrainInfo.commandArea.height; y++) {
                                newAlphamaps[y, x, l] = allTextureSamples[terrainInfo.toolCentricYOffset + y + terrainInfo.commandArea.bottomOffset,
                                    terrainInfo.toolCentricXOffset + x + terrainInfo.commandArea.leftOffset, l];
                            }
                        }
                    }
                    
                    terrainInfo.terrainData.SetAlphamaps(terrainInfo.commandArea.leftOffset, terrainInfo.commandArea.bottomOffset, newAlphamaps);
                    terrainDataSetBasemapDirtyMethodInfo.Invoke(terrainInfo.terrainData, objectArrayWithFalseValue);
                } else {
                    if(newHeights == null || newHeights.GetLength(0) != terrainInfo.commandArea.height || newHeights.GetLength(1) != terrainInfo.commandArea.width) {
                        newHeights = new float[terrainInfo.commandArea.height, terrainInfo.commandArea.width];
                    }
                    for(int x = 0; x < terrainInfo.commandArea.width; x++) {
                        for(int y = 0; y < terrainInfo.commandArea.height; y++) {
                            newHeights[y, x] = allTerrainHeights[terrainInfo.toolCentricYOffset + y + terrainInfo.commandArea.bottomOffset, 
                                terrainInfo.toolCentricXOffset + x + terrainInfo.commandArea.leftOffset];
                        }
                    }

                    if(settings.alwaysUpdateTerrainLODs || overrideAlwaysUpdateTerrainLODs) {
                        terrainInfo.terrainData.SetHeights(terrainInfo.commandArea.leftOffset, terrainInfo.commandArea.bottomOffset, newHeights);
                    } else {
#if UNITY_5_2_OR_NEWER || UNITY_2017_1_OR_NEWER
                        terrainInfo.terrainData.SetHeightsDelayLOD(terrainInfo.commandArea.leftOffset, terrainInfo.commandArea.bottomOffset, newHeights);
#else
                        setHeightsDelayLODMethodInfo.Invoke(terrainInfo.terrainData, new object[] { terrainInfo.commandArea.leftOffset,
                            terrainInfo.commandArea.bottomOffset, newHeights });
#endif
                    }
                }
            }
        }

        private bool IsCurrentModeSculptive() {
            return CurrentTool != Tool.None && CurrentTool < firstNonMouseTool;
        }

        private static Vector2 preferencesItemScrollPosition;
        [PreferenceItem("Terrain Former")]
        private static void DrawPreferences() {
            if(settings == null) {
                InitializeSettings();
            }

            if(settings == null) {
                EditorGUILayout.HelpBox("There was a problem in initializing Terrain Former's settings.", MessageType.Warning);
                return;
            }

            EditorGUIUtility.labelWidth = 185f;

            preferencesItemScrollPosition = EditorGUILayout.BeginScrollView(preferencesItemScrollPosition);
            GUILayout.Label("General", EditorStyles.boldLabel);

            // Raycast Onto Plane
            settings.raycastOntoFlatPlane = GUIUtilities.RadioButtonsControl(GUIContents.raycastModeLabel, settings.raycastOntoFlatPlane ? 
                0 : 1, GUIContents.raycastModes) == 0;
                        
            // Show Sculpting Grid Plane
            EditorGUI.BeginChangeCheck();
            settings.showSculptingGridPlane = EditorGUILayout.Toggle(GUIContents.showSculptingGridPlane, settings.showSculptingGridPlane);
            if(EditorGUI.EndChangeCheck()) {
                SceneView.RepaintAll();
            }

            EditorGUIUtility.fieldWidth += 5f;
            settings.brushColour.Value = EditorGUILayout.ColorField("Brush Colour", settings.brushColour);
            EditorGUIUtility.fieldWidth -= 5f;
            
            settings.alwaysUpdateTerrainLODs = GUIUtilities.RadioButtonsControl(GUIContents.alwaysUpdateTerrainLODs, settings.alwaysUpdateTerrainLODs ? 
                0 : 1, GUIContents.alwaysUpdateTerrain) == 0;

            bool newInvertBrushTexturesGlobally = EditorGUILayout.Toggle("Invert Brush Textures Globally", settings.invertBrushTexturesGlobally);
            if(newInvertBrushTexturesGlobally != settings.invertBrushTexturesGlobally) {
                settings.invertBrushTexturesGlobally = newInvertBrushTexturesGlobally;
                if(Instance != null) {
                    Instance.UpdateAllNecessaryPreviewTextures();
                    Instance.UpdateBrushProjectorTextureAndSamples();
                    Instance.Repaint();
                }
            }

            GUILayout.Label("User Interface", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            bool newAlwaysShowBrushSelection = EditorGUILayout.Toggle(GUIContents.alwaysShowBrushSelection, settings.AlwaysShowBrushSelection);
            if(newAlwaysShowBrushSelection != settings.AlwaysShowBrushSelection) {
                settings.AlwaysShowBrushSelection = newAlwaysShowBrushSelection;
                if(Instance != null) Instance.UpdateAllNecessaryPreviewTextures();
            }

            settings.brushSelectionDisplayType = (BrushSelectionDisplayType)EditorGUILayout.Popup("Brush Selection Display Type",
                (int)settings.brushSelectionDisplayType, GUIContents.brushSelectionDisplayTypeLabels);

            Rect previewSizeRect = EditorGUILayout.GetControlRect();
            Rect previewSizePopupRect = EditorGUI.PrefixLabel(previewSizeRect, new GUIContent("Brush Preview Size"));
            previewSizePopupRect.xMax -= 2;
            
            int newBrushPreviewSize = EditorGUI.IntPopup(previewSizePopupRect, settings.brushPreviewSize, GUIContents.previewSizesContent, new int[] { 32, 48, 64 });
            if(newBrushPreviewSize != settings.brushPreviewSize) {
                settings.brushPreviewSize = newBrushPreviewSize;
                if(Instance != null) Instance.UpdateAllNecessaryPreviewTextures();
            }
            if(EditorGUI.EndChangeCheck()) {
                if(Instance != null) Instance.Repaint();
            }

            GUILayout.Space(2f);

            EditorGUI.BeginChangeCheck();
            settings.showSceneViewInformation = EditorGUILayout.BeginToggleGroup("Show Scene View Information", settings.showSceneViewInformation);
            EditorGUI.indentLevel = 1;
            GUI.enabled = settings.showSceneViewInformation;
            settings.displaySceneViewCurrentTool = EditorGUILayout.Toggle("Display Current Tool", settings.displaySceneViewCurrentTool);
            settings.displaySceneViewCurrentHeight = EditorGUILayout.Toggle("Display Current Height", settings.displaySceneViewCurrentHeight);
            settings.displaySceneViewSculptOntoMode = EditorGUILayout.Toggle("Display Sculpt Onto", settings.displaySceneViewSculptOntoMode);
            EditorGUILayout.EndToggleGroup();
            EditorGUI.indentLevel = 0;
            GUI.enabled = true;
            if(EditorGUI.EndChangeCheck()) {
                SceneView.RepaintAll();
            }
            
            GUILayout.Label("Shortcuts", EditorStyles.boldLabel);
            foreach(Shortcut shortcut in Shortcut.Shortcuts.Values) {
                shortcut.DoShortcutField();
            }

            using(new EditorGUILayout.HorizontalScope()) {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Press Spacebar/Enter to unbind shortcut, Escape to cancel", EditorStyles.centeredGreyMiniLabel);
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // If all the settings are at their default value, disable the "Restore Defaults"
            bool shortcutsNotDefault = false;
            foreach(Shortcut shortcut in Shortcut.Shortcuts.Values) {
                if(shortcut.Binding != shortcut.defaultBinding) {
                    shortcutsNotDefault = true;
                    break;
                }
            }
            
            if(settings.AreSettingsDefault() && shortcutsNotDefault == false) {
                GUI.enabled = false;
            }
            if(GUILayout.Button("Restore Defaults", GUILayout.Width(120f), GUILayout.Height(20))) {
                if(EditorUtility.DisplayDialog("Restore Defaults", "Are you sure you want to restore all settings to their defaults?", "Restore Defaults", "Cancel")) {
                    settings.RestoreDefaultSettings();

                    // Reset shortcuts to defaults
                    foreach(Shortcut shortcut in Shortcut.Shortcuts.Values) {
                        shortcut.waitingForInput = false;
                        shortcut.Binding = shortcut.defaultBinding;
                    }

                    if(Instance != null) Instance.Repaint();
                }
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }
        
        private void UpdateCommandCoordinatesForAllTerrains(Vector3 mousePosition) {
            foreach(TerrainInformation ti in terrainInformations) {
                // Note the decrement of one at the end of both cursor calculations, this is because it's 0 based, not 1 based
                int cursorLeft = Mathf.RoundToInt(((mousePosition.x - ti.transform.position.x) / terrainSize.x) * (currentToolsResolution - 1));
                int cursorBottom = Mathf.RoundToInt(((mousePosition.z - ti.transform.position.z) / terrainSize.z) * (currentToolsResolution - 1f));

                // The bottom-left segments of where the brush samples will start.
                int leftOffset = Mathf.Max(cursorLeft - Mathf.FloorToInt(halfBrushSizeInSamples), 0);
                int bottomOffset = Mathf.Max(cursorBottom - Mathf.FloorToInt(halfBrushSizeInSamples), 0);

                // Check if there aren't any segments that will even be sculpted/painted
                if(leftOffset >= currentToolsResolution || bottomOffset >= currentToolsResolution || cursorLeft + halfBrushSizeInSamples < 0 || cursorBottom + halfBrushSizeInSamples < 0) {
                    ti.commandArea = null;
                    continue;
                }

                if(ti.commandArea == null) ti.commandArea = new CommandArea();

                /** 
                * Create a paint patch used for offsetting the terrain samples.
                * Clipped left contains how many segments are being clipped to the left side of the terrain. The value is 0 if there 
                * are no segments being clipped. This same pattern applies to clippedBottom, clippedWidth, and clippedHeight respectively.
                */
                int clippedLeft;
                if(cursorLeft - halfBrushSizeInSamples < 0) {
                    clippedLeft = Mathf.CeilToInt(Mathf.Abs(cursorLeft - halfBrushSizeInSamples));
                } else clippedLeft = 0;

                int clippedBottom;
                if(cursorBottom - halfBrushSizeInSamples < 0) {
                    clippedBottom = Mathf.CeilToInt(Mathf.Abs(cursorBottom - halfBrushSizeInSamples));
                } else clippedBottom = 0;
                
                if(leftOffset + brushSizeInSamples > currentToolsResolution) {
                    ti.commandArea.width = currentToolsResolution - leftOffset - clippedLeft;
                } else {
                    ti.commandArea.width = brushSizeInSamples - clippedLeft;
                }
                
                if(bottomOffset + brushSizeInSamples > currentToolsResolution) {
                    ti.commandArea.height = currentToolsResolution - bottomOffset - clippedBottom;
                } else {
                    ti.commandArea.height = brushSizeInSamples - clippedBottom;
                }

                ti.commandArea.leftOffset = leftOffset;
                ti.commandArea.bottomOffset = bottomOffset;
                ti.commandArea.clippedLeft = clippedLeft;
                ti.commandArea.clippedBottom = clippedBottom;
            }
        }

        /**
        * Command Coordinates for Terrain Grid returns coordinates taking into account the entire terrain grid, and not taking into account per-terrain coordinates
        * which vary due to the fact the terrain grid has 1 redundant sample per axis. For code comments refer to UpdateCommandCoordinatesForAllTerrains.
        */
        private void UpdateGlobalCommandCoordinates(Vector3 mousePosition) {
            float terrainGridHorizontalSize = numberOfTerrainsHorizontally * terrainSize.x;
            float terrainGridVerticalSize = numberOfTerrainsVertically * terrainSize.z;

            Vector2 mousePositionVersusBottomLeftPosition = new Vector2(mousePosition.x - firstTerrainTransform.position.x,
                mousePosition.z - firstTerrainTransform.position.z);
            
            int cursorLeft = Mathf.RoundToInt((mousePositionVersusBottomLeftPosition.x / terrainGridHorizontalSize) * (toolSamplesHorizontally - 1));
            int cursorBottom = Mathf.RoundToInt((mousePositionVersusBottomLeftPosition.y / terrainGridVerticalSize) * (toolSamplesVertically - 1));
            
            int leftOffset = Mathf.Max(cursorLeft - Mathf.FloorToInt(halfBrushSizeInSamples), 0);
            int bottomOffset = Mathf.Max(cursorBottom - Mathf.FloorToInt(halfBrushSizeInSamples), 0);
            
            if(leftOffset >= toolSamplesHorizontally || bottomOffset >= toolSamplesVertically || cursorLeft + halfBrushSizeInSamples < 0 || 
                cursorBottom + halfBrushSizeInSamples < 0) {
                globalCommandArea = null;
                return;
            }

            if(globalCommandArea == null) globalCommandArea = new CommandArea();

            int clippedLeft;
            if(cursorLeft - halfBrushSizeInSamples < 0) {
                clippedLeft = Mathf.CeilToInt(Mathf.Abs(cursorLeft - halfBrushSizeInSamples));
            } else clippedLeft = 0;

            int clippedBottom;
            if(cursorBottom - halfBrushSizeInSamples < 0) {
                clippedBottom = Mathf.CeilToInt(Mathf.Abs(cursorBottom - halfBrushSizeInSamples));
            } else clippedBottom = 0;
            
            if(leftOffset + brushSizeInSamples > toolSamplesHorizontally) {
                globalCommandArea.width = toolSamplesHorizontally - leftOffset - clippedLeft;
            } else {
                globalCommandArea.width = brushSizeInSamples - clippedLeft;
            }
            
            if(bottomOffset + brushSizeInSamples > toolSamplesVertically) {
                globalCommandArea.height = toolSamplesVertically - bottomOffset - clippedBottom;
            } else {
                globalCommandArea.height = brushSizeInSamples - clippedBottom;
            }

            globalCommandArea.leftOffset = leftOffset;
            globalCommandArea.bottomOffset = bottomOffset;
            globalCommandArea.clippedLeft = clippedLeft;
            globalCommandArea.clippedBottom = clippedBottom;
        }

        private void UpdateRandomSpacing() {
            randomSpacing = UnityEngine.Random.Range(settings.modeSettings[CurrentTool].minBrushSpacing, settings.modeSettings[CurrentTool].maxBrushSpacing);
        }

        /**
        * Caching the terrain brush is especially useful for RotateTemporaryBrushSamples. It would take >500ms when accessing the terrain brush
        * through the property. Using it in when it's been cached makes roughly a 10x speedup and doesn't allocated ~3 MB of garbage.
        */
        private TerrainBrush cachedTerrainBrush;
        private void RotateTemporaryBrushSamples() {
            cachedTerrainBrush = BrushCollection.brushes[settings.modeSettings[CurrentTool].selectedBrushId];

            if(temporarySamples == null || temporarySamples.GetLength(0) != brushSizeInSamples) {
                temporarySamples = new float[brushSizeInSamples, brushSizeInSamples];
            }
            
            float angle = settings.modeSettings[CurrentTool].brushAngle + UnityEngine.Random.Range(settings.modeSettings[CurrentTool].minRandomRotation, settings.modeSettings[CurrentTool].maxRandomRotation);
            Vector2 newPoint;
            PointRotator pointRotator = new PointRotator(angle, new Vector2(brushSizeInSamples * 0.5f, brushSizeInSamples * 0.5f));

            for(int x = 0; x < brushSizeInSamples; x++) {
                for(int y = 0; y < brushSizeInSamples; y++) {
                    newPoint = pointRotator.Rotate(new Vector2(x, y));
                    temporarySamples[x, y] = GetInteropolatedBrushSample(newPoint.x, newPoint.y) * settings.modeSettings[CurrentTool].brushSpeed;
                }
            }
        }

        private float GetInteropolatedBrushSample(float x, float y) {
            int flooredX = Mathf.FloorToInt(x);
            int flooredY = Mathf.FloorToInt(y);
            int flooredXPlus1 = flooredX + 1;
            int flooredYPlus1 = flooredY + 1;

            if(flooredX < 0 || flooredX >= brushSizeInSamples || flooredY < 0 || flooredY >= brushSizeInSamples) return 0f;

            float topLeftSample = cachedTerrainBrush.samples[flooredX, flooredY];
            float topRightSample = 0f;
            float bottomLeftSample = 0f;
            float bottomRightSample = 0f;

            if(flooredXPlus1 < brushSizeInSamples) {
                topRightSample = cachedTerrainBrush.samples[flooredXPlus1, flooredY];
            }

            if(flooredYPlus1 < brushSizeInSamples) {
                bottomLeftSample = cachedTerrainBrush.samples[flooredX, flooredYPlus1];

                if(flooredXPlus1 < brushSizeInSamples) {
                    bottomRightSample = cachedTerrainBrush.samples[flooredXPlus1, flooredYPlus1];
                }
            }

            return Utilities.LerpUnclamped(Utilities.LerpUnclamped(topLeftSample, topRightSample, x % 1f), 
                Utilities.LerpUnclamped(bottomLeftSample, bottomRightSample, x % 1f), y % 1f);
        }

        private void UpdateDirtyBrushSamples() {
            if(samplesDirty == SamplesDirty.None || CurrentBrush == null) return;
            
            // Update only the brush samples, and don't even update the projector texture
            if((samplesDirty & SamplesDirty.BrushSamples) == SamplesDirty.BrushSamples) {
                CurrentBrush.UpdateSamplesWithSpeed(brushSizeInSamples);
            }
            if((samplesDirty & SamplesDirty.ProjectorTexture) == SamplesDirty.ProjectorTexture) {
                UpdateBrushProjectorTextureAndSamples();
            }
            if((samplesDirty & SamplesDirty.InspectorTexture) == SamplesDirty.InspectorTexture) {
                UpdateBrushInspectorTexture();
            }
            
            // Since the underlying burhshProjector pixels have been rotated, set the temporary rotation to zero.
            brushProjector.transform.eulerAngles = new Vector3(90f, 0f, 0f);
            topPlaneGameObject.transform.eulerAngles = brushProjector.transform.eulerAngles;

            samplesDirty = SamplesDirty.None;
        }
        
        private void CheckKeyboardShortcuts(Event currentEvent) {
            if(GUIUtility.hotControl != 0) return;
            if(activeInspectorInstanceID != 0 && activeInspectorInstanceID != GetInstanceID()) return;
            if(currentEvent.type != EventType.KeyDown) return;

            // Only check for shortcuts when no terrain command is active
            if(currentCommand != null) return;

            /**
            * Check to make sure there is no textField focused. This will ensure that shortcut strokes will not override
            * typing in text fields. Through testing however, all textboxes seem to mark the event as Used. This is simply
            * here as a precaution.
            */
            if((bool)guiUtilityTextFieldInput.GetValue(null, null)) return;

            Shortcut.wasExecuted = false;

            if(Shortcut.Shortcuts["Select Raise/Lower Tool"].WasExecuted(currentEvent)) {
                CurrentTool = Tool.RaiseOrLower;
            } else if(Shortcut.Shortcuts["Select Smooth Tool"].WasExecuted(currentEvent)) {
                CurrentTool = Tool.Smooth;
            } else if(Shortcut.Shortcuts["Select Set Height Tool"].WasExecuted(currentEvent)) {
                CurrentTool = Tool.SetHeight;
            } else if(Shortcut.Shortcuts["Select Flatten Tool"].WasExecuted(currentEvent)) {
                CurrentTool = Tool.Flatten;
            } else if(Shortcut.Shortcuts["Select Paint Texture Tool"].WasExecuted(currentEvent)) {
                CurrentTool = Tool.PaintTexture;
            } else if(Shortcut.Shortcuts["Select Mould Tool"].WasExecuted(currentEvent)) {
                CurrentTool = Tool.Mould;
            } else if(Shortcut.Shortcuts["Select Settings Tool"].WasExecuted(currentEvent)) {
                CurrentTool = Tool.Settings;
            }

            // Tool centric shortcuts
            if(CurrentTool == Tool.None || CurrentTool >= firstNonMouseTool) {
                if(Shortcut.wasExecuted) {
                    Repaint();
                    currentEvent.Use();
                }
                return;
            }

            if(Shortcut.Shortcuts["Decrease Brush Size"].WasExecuted(currentEvent)) {
                settings.modeSettings[CurrentTool].brushSize = 
                    Mathf.Clamp(settings.modeSettings[CurrentTool].brushSize - GetBrushSizeIncrement(settings.modeSettings[CurrentTool].brushSize), MinBrushSize, terrainSize.x);
                settings.modeSettings[CurrentTool].brushSize = Mathf.Round(settings.modeSettings[CurrentTool].brushSize / 0.1f) * 0.1f;
                BrushSizeChanged();
            } else if(Shortcut.Shortcuts["Increase Brush Size"].WasExecuted(currentEvent)) {
                settings.modeSettings[CurrentTool].brushSize = 
                    Mathf.Clamp(settings.modeSettings[CurrentTool].brushSize + GetBrushSizeIncrement(settings.modeSettings[CurrentTool].brushSize), MinBrushSize, terrainSize.x);
                settings.modeSettings[CurrentTool].brushSize = Mathf.Round(settings.modeSettings[CurrentTool].brushSize / 0.1f) * 0.1f;
                BrushSizeChanged();
            } else if(Shortcut.Shortcuts["Decrease Brush Speed"].WasExecuted(currentEvent)) {
                settings.modeSettings[CurrentTool].brushSpeed = 
                    Mathf.Clamp(Mathf.Round((settings.modeSettings[CurrentTool].brushSpeed - GetBrushSpeedIncrement(settings.modeSettings[CurrentTool].brushSpeed)) / 0.001f) * 0.001f, 
                    minBrushSpeed, maxBrushSpeed);
                BrushSpeedChanged();
            } else if(Shortcut.Shortcuts["Increase Brush Speed"].WasExecuted(currentEvent)) {
                settings.modeSettings[CurrentTool].brushSpeed = 
                    Mathf.Clamp(Mathf.Round((settings.modeSettings[CurrentTool].brushSpeed + GetBrushSpeedIncrement(settings.modeSettings[CurrentTool].brushSpeed)) / 0.001f) * 0.001f, 
                    minBrushSpeed, maxBrushSpeed);
                BrushSpeedChanged();
            } else if(Shortcut.Shortcuts["Next Brush"].WasExecuted(currentEvent)) {
                IncrementSelectedBrush(1);
            } else if(Shortcut.Shortcuts["Previous Brush"].WasExecuted(currentEvent)) {
                IncrementSelectedBrush(-1);
            }

            // Brush angle doesn't apply to a circular falloff brush
            if(CurrentBrush != null && CanBrushRotate()) {
                if(Shortcut.Shortcuts["Reset Brush Rotation"].WasExecuted(currentEvent)) {
                    float angleDeltaChange = settings.modeSettings[CurrentTool].brushAngle;
                    settings.modeSettings[CurrentTool].brushAngle = 0f;
                    if(angleDeltaChange != 0f) BrushAngleDeltaChanged(angleDeltaChange);
                } else if(Shortcut.Shortcuts["Rotate Brush Anticlockwise"].WasExecuted(currentEvent)) {
                    float newBrushAngle = Mathf.Clamp(settings.modeSettings[CurrentTool].brushAngle + 2f, -180f, 180f);
                    if(newBrushAngle != settings.modeSettings[CurrentTool].brushAngle) {
                        float delta = settings.modeSettings[CurrentTool].brushAngle - newBrushAngle;
                        settings.modeSettings[CurrentTool].brushAngle = newBrushAngle;
                        BrushAngleDeltaChanged(delta);
                    }
                } else if(Shortcut.Shortcuts["Rotate Brush Clockwise"].WasExecuted(currentEvent)) {
                    float newBrushAngle = Mathf.Clamp(settings.modeSettings[CurrentTool].brushAngle - 2f, -180f, 180f);
                    if(newBrushAngle != settings.modeSettings[CurrentTool].brushAngle) {
                        float delta = settings.modeSettings[CurrentTool].brushAngle - newBrushAngle;
                        settings.modeSettings[CurrentTool].brushAngle = newBrushAngle;
                        BrushAngleDeltaChanged(delta);
                    }
                }
            }

            if(Shortcut.Shortcuts["Toggle Sculpt Onto Mode"].WasExecuted(currentEvent)) {
                settings.raycastOntoFlatPlane = !settings.raycastOntoFlatPlane;
            } else if(Shortcut.Shortcuts["Flatten Terrain"].WasExecuted(currentEvent)) {
                SetHeightAll(0f);
            }

            switch(CurrentTool) {
                case Tool.Flatten:
                    int totalFlattenModeValues = Enum.GetValues(typeof(FlattenMode)).Length;
                    if(Shortcut.Shortcuts["Previous Flatten Mode"].WasExecuted(currentEvent)) {
                        if(--settings.flattenMode < 0) settings.flattenMode = (FlattenMode)(totalFlattenModeValues - 1);
                    } else if(Shortcut.Shortcuts["Next Flatten Mode"].WasExecuted(currentEvent)) {
                        if((int)++settings.flattenMode >= totalFlattenModeValues) settings.flattenMode = 0;
                    }
                    break;
                case Tool.PaintTexture:
                    int splatPrototypesCount = splatPrototypes.Length;
                    if(Shortcut.Shortcuts["Previous Texture"].WasExecuted(currentEvent)) {
                        if(--settings.selectedTextureIndex < 0) settings.selectedTextureIndex = splatPrototypesCount - 1;
                    } else if(Shortcut.Shortcuts["Next Texture"].WasExecuted(currentEvent)) {
                        if(++settings.selectedTextureIndex >= splatPrototypesCount) settings.selectedTextureIndex = 0;
                    }
                    break;
            }

            if(Shortcut.wasExecuted) {
                Repaint();
                currentEvent.Use();
            }
        }

        private float GetBrushSizeIncrement(float currentBrushSize) {
            float currentSizeCoefficient = currentBrushSize / terrainSize.x;

            if(currentSizeCoefficient > 0.5f) {
                return terrainSize.x * 0.025f;
            } else if(currentSizeCoefficient > 0.25f) {
                return terrainSize.x * 0.01f;
            } else if(currentSizeCoefficient > 0.1f) {
                return terrainSize.x * 0.005f;
            } else if(currentSizeCoefficient > 0.08f) {
                return terrainSize.x * 0.002f;
            } else {
                return terrainSize.x * 0.001f;
            }
        }

        private float GetBrushSpeedIncrement(float currentBrushSpeed) {
            if(currentBrushSpeed > 1f) {
                return 0.1f;
            } else if(currentBrushSpeed > 0.5f) {
                return 0.05f;
            } else if(currentBrushSpeed > 0.18f) {
                return 0.02f;
            } else if(currentBrushSpeed > 0.02f) {
                return 0.01f;
            } else {
                return 0.001f;
            }
        }

        private void IncrementSelectedBrush(int incrementFactor) {
            if(BrushCollection.brushes.Count == 0) return;

            string currentSelectedBrushIndex = settings.modeSettings[CurrentTool].selectedBrushId;

            // Return if the increment/decrement will be out of bounds
            if((incrementFactor == -1 && currentSelectedBrushIndex == BrushCollection.brushes.First().Value.id) ||
                (incrementFactor == 1 && currentSelectedBrushIndex == BrushCollection.brushes.Last().Value.id)) return;
            
            // Continue to increment the current index until the current brush is found by Key (a unique string)
            int currentBrushIndex = 0;
            foreach(KeyValuePair<string, TerrainBrush> brush in BrushCollection.brushes) {
                if(brush.Key != currentSelectedBrushIndex) {
                    currentBrushIndex++;
                    continue;
                }
                break;
            }

            settings.modeSettings[CurrentTool].selectedBrushId = BrushCollection.brushes.ElementAt(currentBrushIndex + incrementFactor).Value.id;
            SelectedBrushChanged();
        }

        private void BrushFalloffChanged() {
            ClampAnimationCurve(settings.modeSettings[CurrentTool].brushFalloff);

            samplesDirty |= SamplesDirty.ProjectorTexture;

            if(settings.AlwaysShowBrushSelection) {
                BrushCollection.UpdatePreviewTextures();
            } else {
                UpdateBrushInspectorTexture();
            }
        }

        private void ToggleSelectingBrush() {
            isSelectingBrush = !isSelectingBrush;

            // Update the brush previews if the user is now selecting brushes
            if(isSelectingBrush) {
                BrushCollection.UpdatePreviewTextures();
            }
        }

        private void CurrentToolChanged(Tool previousValue) {
            // Sometimes it's possible Terrain Former thinks the mouse is still pressed down as not every event is detected by Terrain Former
            mouseIsDown = false;

            bool unityTerrainInspectorWasActive = false;
            // If the built-in Unity tools were active, make them inactive by setting their tool to None (-1)
            foreach(object terrainInspector in unityTerrainInspectors) {
                if((int)unityTerrainSelectedTool.GetValue(terrainInspector, null) != -1) {
                    unityTerrainSelectedTool.SetValue(terrainInspector, -1, null);

                    unityTerrainInspectorWasActive = true;
                }
            }

            if(unityTerrainInspectorWasActive && CurrentTool != Tool.None) {
                // Update the heights of the terrain editor in case they were edited in the Unity terrain editor
                UpdateAllHeightsFromSourceAssets();
            }
            
            /**
            * All inspector windows must be updated to reflect across multiple inspectors that there is only one Terrain Former instance active at
            * once, and that also stops those Terrain Former instance(s) that are no longer active to not call OnInspectorGUI.
            */
            inspectorWindowRepaintAllInspectors.Invoke(null, null);

            UnityEditor.Tools.current = UnityEditor.Tool.None;
            activeInspectorInstanceID = GetInstanceID();
            Instance = this;

            if(settings == null) return;

            if(CurrentTool == Tool.None) {
                if(brushProjectorGameObject.activeSelf) brushProjectorGameObject.SetActive(false);
                return;
            }
            
            if(previousValue == Tool.None) Initialize(true);
            if(CurrentTool >= firstNonMouseTool) return;
            
            splatPrototypes = firstTerrainData.splatPrototypes;
            
            settings.modeSettings[CurrentTool] = settings.modeSettings[CurrentTool];
            
            switch(CurrentTool) {
                case Tool.PaintTexture:
                    currentToolsResolution = firstTerrainData.alphamapResolution;
                    toolSamplesHorizontally = currentToolsResolution * numberOfTerrainsHorizontally;
                    toolSamplesVertically = currentToolsResolution * numberOfTerrainsVertically;
                    break;
                default:
                    currentToolsResolution = heightmapResolution;
                    toolSamplesHorizontally = currentToolsResolution * numberOfTerrainsHorizontally - (numberOfTerrainsHorizontally - 1);
                    toolSamplesVertically = currentToolsResolution * numberOfTerrainsVertically - (numberOfTerrainsVertically - 1);
                    break;
            }
            
            foreach(TerrainInformation terrainInfo in terrainInformations) {
                terrainInfo.toolCentricXOffset = terrainInfo.gridXCoordinate * currentToolsResolution;
                terrainInfo.toolCentricYOffset = terrainInfo.gridYCoordinate * currentToolsResolution;
                if(CurrentTool != Tool.PaintTexture) {
                    terrainInfo.toolCentricXOffset -= terrainInfo.gridXCoordinate;
                    terrainInfo.toolCentricYOffset -= terrainInfo.gridYCoordinate;
                }
            }
            
            if(CurrentTool == Tool.PaintTexture) {
                UpdateAllAlphamapSamplesFromSourceAssets();
            } else {
                allTextureSamples = null;
            }
            
            brushProjector.orthographicSize = settings.modeSettings[CurrentTool].brushSize * 0.5f;
            float brushSize = settings.modeSettings[CurrentTool].brushSize;
            topPlaneGameObject.transform.localScale = new Vector3(brushSize, brushSize, 1f);
            BrushSizeInSamples = GetSegmentsFromUnits(settings.modeSettings[CurrentTool].brushSize);
            
            UpdateAllNecessaryPreviewTextures();
            UpdateBrushProjectorTextureAndSamples();
        }
        
        private void SelectedBrushChanged() {
            UpdateBrushTextures();
        }
        
        private void InvertBrushTextureChanged() {
            UpdateBrushTextures();

            if(settings.AlwaysShowBrushSelection) BrushCollection.UpdatePreviewTextures();
        }

        private void BrushSpeedChanged() {
            samplesDirty |= SamplesDirty.BrushSamples;
        }

        private void BrushColourChanged() {
            brushProjector.material.color = settings.brushColour;
            topPlaneMaterial.color = settings.brushColour.Value * 0.9f;
        }

        private void BrushSizeChanged() {
            if(CurrentTool == Tool.None || CurrentTool >= firstNonMouseTool) return;

            BrushSizeInSamples = GetSegmentsFromUnits(settings.modeSettings[CurrentTool].brushSize);

            /**
            * HACK: Another spot where objects are seemingly randomly destroyed. The top plane and projector are (seemingly) destroyed between
            * switching from one terrain with Terrain Former to another.
            */
            if(topPlaneGameObject == null || brushProjector == null) {
                CreateProjector();
            }

            float brushSize = settings.modeSettings[CurrentTool].brushSize;
            topPlaneGameObject.transform.localScale = new Vector3(brushSize, brushSize, 1f);
            brushProjector.orthographicSize = settings.modeSettings[CurrentTool].brushSize * 0.5f;

            samplesDirty |= SamplesDirty.ProjectorTexture;
        }

        private void BrushRoundnessChanged() {
            samplesDirty |= SamplesDirty.ProjectorTexture;

            UpdateAllNecessaryPreviewTextures();
        }

        private void BrushAngleDeltaChanged(float delta) {
            UpdateAllNecessaryPreviewTextures();

            brushProjector.transform.eulerAngles = new Vector3(90f, brushProjector.transform.eulerAngles.y + delta, 0f);
            topPlaneGameObject.transform.eulerAngles = brushProjector.transform.eulerAngles;

            samplesDirty = SamplesDirty.BrushSamples | SamplesDirty.ProjectorTexture;
        }

        private void AlwaysShowBrushSelectionValueChanged() {
            /**
            * If the brush selection should always be shown, make sure isSelectingBrush is set to false because
            * when changing to AlwaysShowBrushSelection while the brush selection was active, it will return back to
            * selecting a brush.
            */
            if(settings.AlwaysShowBrushSelection) {
                isSelectingBrush = false;
            }
        }
        
        private void UpdateAllNecessaryPreviewTextures() {
            if(CurrentTool == Tool.None || CurrentTool >= firstNonMouseTool) return;

            if(settings.AlwaysShowBrushSelection || isSelectingBrush) {
                BrushCollection.UpdatePreviewTextures();
            } else {
                UpdateBrushInspectorTexture();
            }
        }
        
        internal void ApplySplatPrototypes() {
            for(int i = 0; i < terrainInformations.Count; i++) {
                terrainInformations[i].terrainData.splatPrototypes = splatPrototypes;
            }
        }

        /**
        * Update the heights and alphamaps every time an Undo or Redo occurs - since we must rely on storing and managing the 
        * heights data manually for better editing performance.
        * This is quite slow, and I've seriously run into a dead end into trying to fix it perfectly.
        */
        private void UndoRedoPerformed() {
            if(target == null) return;
            
            if(terrainSize != firstTerrainData.size || heightmapResolution != firstTerrainData.heightmapResolution || alphamapResolution != firstTerrainData.alphamapResolution) {
                UpdateTerrainRelatedFields();
                CurrentToolChanged(CurrentTool);
            }
            
            lastHeightmapResolultion = heightmapResolution;

            UpdateAllHeightsFromSourceAssets();

            if(CurrentTool == Tool.PaintTexture) {
                splatPrototypes = firstTerrainData.splatPrototypes;
                UpdateAllAlphamapSamplesFromSourceAssets();
            }
        }

        private void RegisterUndoForTerrainGrid(string description, bool includeAlphamapTextures = false, List<UnityEngine.Object> secondaryObjectsToUndo = null) {
            List<UnityEngine.Object> objectsToRegister = new List<UnityEngine.Object>();
            if(secondaryObjectsToUndo != null) objectsToRegister.AddRange(secondaryObjectsToUndo);

            for(int i = 0; i < terrainInformations.Count; i++) {
                objectsToRegister.Add(terrainInformations[i].terrainData);
                                
                if(includeAlphamapTextures) {
                    objectsToRegister.AddRange(terrainInformations[i].terrainData.alphamapTextures);
                }
            }
            
            Undo.RegisterCompleteObjectUndo(objectsToRegister.ToArray(), description);
        }

        private void CreateGridPlane() {
            if(gridPlane == null) {
                gridPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
                gridPlane.name = "GridPlane";
                gridPlane.transform.Rotate(90f, 0f, 0f);
                gridPlane.transform.localScale = Vector3.one * 20f;
                gridPlane.hideFlags = HideFlags.HideAndDontSave;
                gridPlane.SetActive(false);
            }

            Shader gridShader = Shader.Find("Hidden/TerrainFormer/Grid");
            if(gridShader == null) {
                Debug.LogError("Terrain Former couldn't find its grid shader.");
                return;
            }

            if(gridPlaneMaterial == null) {
                gridPlaneMaterial = new Material(gridShader);
                gridPlaneMaterial.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(settings.mainDirectory + "Textures/Tile.psd");
                gridPlaneMaterial.mainTexture.wrapMode = TextureWrapMode.Repeat;
                gridPlaneMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
                gridPlaneMaterial.hideFlags = HideFlags.HideAndDontSave;
                gridPlaneMaterial.mainTextureScale = new Vector2(8f, 8f); // Set texture scale to create 8x8 tiles
                gridPlane.GetComponent<Renderer>().sharedMaterial = gridPlaneMaterial;
            }
        }

        private void CreateProjector() {
            /**
            * Create the brush projector
            */
            if(brushProjectorGameObject == null) {
                brushProjectorGameObject = EditorUtility.CreateGameObjectWithHideFlags("TerrainFormerProjector", HideFlags.HideAndDontSave);
            }

            if(brushProjector == null) {
                brushProjector = brushProjectorGameObject.AddComponent<Projector>();
                brushProjector.nearClipPlane = -1000f;
                brushProjector.farClipPlane = 1000f;
                brushProjector.orthographic = true;
                brushProjector.orthographicSize = 10f;
                brushProjector.transform.Rotate(90f, 0f, 0f);
            }

            if(brushProjectorMaterial == null) {
                brushProjectorMaterial = new Material(Shader.Find("Hidden/TerrainFormer/Terrain Brush Preview"));
                brushProjectorMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
                brushProjectorMaterial.hideFlags = HideFlags.HideAndDontSave;
                brushProjectorMaterial.color = settings.brushColour;
                brushProjector.material = brushProjectorMaterial;
            }

            Texture2D outlineTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(settings.mainDirectory + "Textures/BrushOutline.png");
            if(brushProjectorMaterial.GetTexture("_OutlineTex") == null) {    
                brushProjectorMaterial.SetTexture("_OutlineTex", outlineTexture);
            }

            /**
            * Create the top plane
            */
            if(topPlaneGameObject == null) {
                topPlaneGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                topPlaneGameObject.name = "Top Plane";
                topPlaneGameObject.hideFlags = HideFlags.HideAndDontSave;
                DestroyImmediate(topPlaneGameObject.GetComponent<MeshCollider>());
                topPlaneGameObject.transform.Rotate(90f, 0f, 0f);
            }

            if(topPlaneMaterial == null) {
                topPlaneMaterial = new Material(Shader.Find("Hidden/TerrainFormer/BrushPlaneTop"));
                topPlaneMaterial.hideFlags = HideFlags.HideAndDontSave;
                topPlaneMaterial.color = settings.brushColour.Value * 0.9f;
                topPlaneMaterial.SetTexture("_OutlineTex", outlineTexture);
                topPlaneGameObject.GetComponent<MeshRenderer>().sharedMaterial = topPlaneMaterial;
            }
            
            SetCursorEnabled(false);
        }

        private void UpdateProjector() {
            if(brushProjector == null) return;

            if(CurrentTool == Tool.None || CurrentTool >= firstNonMouseTool) {
                SetCursorEnabled(false);
                return;
            }
            
            Vector3 position;
            if(GetMousePositionInWorldSpace(out position)) {
                // Always make sure the projector is positioned as high as necessary
                brushProjector.transform.position = new Vector3(position.x, firstTerrain.transform.position.y + firstTerrainData.size.y + 1f, position.z);
                brushProjector.farClipPlane = firstTerrainData.size.y + 2f;
                brushProjectorGameObject.SetActive(true);

                float minHeightDifferenceToShowTopPlane = firstTerrainData.heightmapScale.y * 0.002f;

                if(CurrentTool == Tool.Flatten) {
                    topPlaneGameObject.SetActive(position.y >= minHeightDifferenceToShowTopPlane);
                    topPlaneGameObject.transform.position = new Vector3(position.x, position.y, position.z);
                } else if(CurrentTool == Tool.SetHeight) {
                    topPlaneGameObject.SetActive(settings.setHeight >= minHeightDifferenceToShowTopPlane);
                    topPlaneGameObject.transform.position = new Vector3(position.x, firstTerrain.transform.position.y + settings.setHeight, position.z);
                } else {
                    topPlaneGameObject.SetActive(false);
                }
            } else {
                SetCursorEnabled(false);
            }
            
            HandleUtility.Repaint();
        }

        private void UpdateBrushTextures() {
            UpdateBrushInspectorTexture();
            UpdateBrushProjectorTextureAndSamples();
        }

        private void UpdateBrushProjectorTextureAndSamples() {
            CurrentBrush.UpdateSamplesAndMainTexture(brushSizeInSamples);

            // HACK: Projector objects are destroyed (seemingly randomly), so recreate them if necessary
            if(brushProjectorGameObject == null || brushProjectorMaterial == null) {
                CreateProjector();
            }

            brushProjectorMaterial.mainTexture = brushProjectorTexture;
            topPlaneMaterial.mainTexture = brushProjectorTexture;

            if(currentCommand != null) {
                currentCommand.brushSamples = GetBrushSamplesWithSpeed();
            }
        }

        private void UpdateBrushInspectorTexture() {
            CurrentBrush.CreatePreviewTexture();
        }

        internal void DeleteSplatTexture(int indexToDelete) {
            RegisterUndoForTerrainGrid("Delete Splat Texture", true);
            
            int allTextureSamplesHorizontally = allTextureSamples.GetLength(0);
            int allTextureSamplesVertically = allTextureSamples.GetLength(1);
            int textureCount = allTextureSamples.GetLength(2);
            int newTextureCount = textureCount - 1;

            float[,,] oldTextureSamples = new float[allTextureSamplesVertically, allTextureSamplesHorizontally, textureCount];
            Array.Copy(allTextureSamples, oldTextureSamples, allTextureSamples.Length);
            
            // Duplicate the alphamaps array, except the part of the 3rd dimension whose index is the one to be deleted
            allTextureSamples = new float[allTextureSamplesVertically, allTextureSamplesHorizontally, newTextureCount];
            
            for(int x = 0; x < allTextureSamplesHorizontally; x++) {
                for(int y = 0; y < allTextureSamplesVertically; y++) {
                    for(int l = 0; l < indexToDelete; l++) {
                        allTextureSamples[y, x, l] = oldTextureSamples[y, x, l];
                    }
                    for(int l = indexToDelete + 1; l < textureCount; l++) {
                        allTextureSamples[y, x, l - 1] = oldTextureSamples[y, x, l];
                    }
                }
            }
            
            for(int x = 0; x < allTextureSamplesHorizontally; x++) {
                for(int y = 0; y < allTextureSamplesVertically; y++) {
                    float sum = 0f;

                    for(int l = 0; l < newTextureCount; l++) {
                        sum += allTextureSamples[y, x, l];
                    }

                    if(sum >= 0.01f) {
                        float sumCoefficient = 1f / sum;
                        for(int l = 0; l < newTextureCount; l++) {
                            allTextureSamples[y, x, l] *= sumCoefficient;
                        }
                    } else {
                        for(int l = 0; l < newTextureCount; l++) {
                            allTextureSamples[y, x, l] = l != 0 ? 0f : 1f;
                        }
                    }
                }
            }

            List<SplatPrototype> splatPrototypesList = new List<SplatPrototype>(splatPrototypes);
            splatPrototypesList.RemoveAt(indexToDelete);
            splatPrototypes = splatPrototypesList.ToArray();
            ApplySplatPrototypes();
            UpdateAllAlphamapSamplesInSourceAssets();
        }

        private void SetHeightAll(float setHeight) {
            Debug.Log("SetHeightAll");
            RegisterUndoForTerrainGrid("Flatten Terrain");

            for(int x = 0; x < heightmapWidth; x++) {
                for(int y = 0; y < heightmapHeight; y++) {
                    allTerrainHeights[y, x] = setHeight;
                }
            }

            // Create the silly array that we must send every terrain as we don't have any other choice
            float[,] newHeights = new float[heightmapResolution, heightmapResolution];
            for(int x = 0; x < heightmapResolution; x++) {
                for(int y = 0; y < heightmapResolution; y++) {
                    newHeights[x, y] = setHeight;
                }
            }

            foreach(TerrainInformation ti in terrainInformations) {
                ti.terrainData.SetHeights(0, 0, newHeights);
            }
        }

        private void CreateLinearRamp(float maxHeight) {
            RegisterUndoForTerrainGrid("Created Ramp");
            
            float heightCoefficient = maxHeight / terrainSize.y;
            float height;
            /**
            * It might seem wasteful not not make these loops generic enough to avoid duplication, but the simplicity that brings as
            * well as the speed is far more important
            */
            if(settings.generateRampCurveInXAxis) {
                for(int x = 0; x < heightmapWidth; x++) {
                    height = settings.generateRampCurve.Evaluate((float)x / heightmapWidth) * heightCoefficient;
                    for(int y = 0; y < heightmapHeight; y++) {
                        allTerrainHeights[y, x] = height;
                    }
                }
            } else {
                for(int y = 0; y < heightmapHeight; y++) {
                    height = settings.generateRampCurve.Evaluate((float)y / heightmapHeight) * heightCoefficient;
                    for(int x = 0; x < heightmapWidth; x++) {
                        allTerrainHeights[y, x] = height;
                    }
                }
            }
            
            UpdateAllHeightsInSourceAssets();
        }
        
        private void CreateCircularRamp(float maxHeight) {
            RegisterUndoForTerrainGrid("Created Circular Ramp");
            
            float heightCoefficient = maxHeight / terrainSize.y;
            float halfTotalTerrainSize = Mathf.Min(heightmapWidth, heightmapHeight) * 0.5f;
            float distance;
            for(int x = 0; x < heightmapWidth; x++) {
                for(int y = 0; y < heightmapHeight; y++) {
                    distance = CalculateDistance(x, y, halfTotalTerrainSize, halfTotalTerrainSize);
                    allTerrainHeights[y, x] = settings.generateRampCurve.Evaluate(1f - (distance / halfTotalTerrainSize)) * heightCoefficient;
                }
            }
            
            UpdateAllHeightsInSourceAssets();
        }

        private void OffsetTerrainGridHeight(float heightmapHeightOffset) {
            RegisterUndoForTerrainGrid("Height Offset");

            for(int x = 0; x < heightmapWidth; x++) {
                for(int y = 0; y < heightmapHeight; y++) {
                    allTerrainHeights[y, x] = Mathf.Clamp01(allTerrainHeights[y, x] + heightmapHeightOffset / terrainSize.y);
                }
            }

            foreach(TerrainInformation terrainInfo in terrainInformations) {
                float[,] newHeights = new float[heightmapResolution, heightmapResolution];
                for(int x = 0; x < heightmapResolution; x++) {
                    for(int y = 0; y < heightmapResolution; y++) {
                        newHeights[y, x] = allTerrainHeights[terrainInfo.heightmapYOffset + y, terrainInfo.heightmapXOffset + x];
                    }
                }

                terrainInfo.terrainData.SetHeights(0, 0, newHeights);
            }
        }

        private void ExportHeightmap(ref Texture2D tex) {
            for(int x = 0; x < heightmapWidth; x++) {
                for(int y = 0; y < heightmapHeight; y++) {
                    float grey = allTerrainHeights[y, x] / terrainSize.y;
                    tex.SetPixel(y, x, new Color(grey, grey, grey));
                }
            }
            tex.Apply();
        }
                
        private void ImportHeightmap() {
            TextureImporter heightmapTextureImporter = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(heightmapTexture));
            if(heightmapTextureImporter.isReadable == false) {
                heightmapTextureImporter.isReadable = true;
                heightmapTextureImporter.SaveAndReimport();
            }

            float uPosition;
            float vPosition = 0f;
            Color bilinearSample;
            const float oneThird = 1f / 3f;
            for(int y = 0; y < heightmapHeight; y++) {
                for(int x = 0; x < heightmapWidth; x++) {
                    uPosition = (float)x / heightmapWidth;
                    vPosition = (float)y / heightmapHeight;
                    if(settings.heightmapSourceIsAlpha) {
                        allTerrainHeights[x, y] = heightmapTexture.GetPixelBilinear(uPosition, vPosition).a;
                    } else {
                        bilinearSample = heightmapTexture.GetPixelBilinear(uPosition, vPosition);
                        allTerrainHeights[x, y] = (bilinearSample.r + bilinearSample.g + bilinearSample.b) * oneThird;
                    }
                }

                if(EditorUtility.DisplayCancelableProgressBar("Terrain Former", "Applying heightmap to terrain", vPosition * 0.9f)) {
                    EditorUtility.ClearProgressBar();
                    return;
                }
            }
            UpdateAllHeightsInSourceAssets();

            EditorUtility.ClearProgressBar();
        }

        // If there have been changes to a given terrain in Terrain Former, don't reimport its heights on OnAssetsImported.
        private void OnWillSaveAssets(string[] assetPaths) {
            if(settings != null) settings.Save();

            foreach(TerrainInformation ti in terrainInformations) {
                if(ti.hasChangedSinceLastSave == false) continue;

                foreach(string assetPath in assetPaths) {
                    if(ti.terrainAssetPath != assetPath) continue;

                    ti.ignoreOnAssetsImported = true;
                    ti.hasChangedSinceLastSave = false;
                }
            }
        }
        
        private void OnAssetsImported(string[] assetPaths) {
            // There's a possibility of no terrainInformations because of being no terrains on the object.
            if(terrainInformations == null) return;

            List<string> customBrushPaths = new List<string>();

            Type texture2DType = typeof(Texture2D);
            Type terrainDataType = typeof(TerrainData);

            foreach(string path in assetPaths) {
                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                if(asset == null) continue;

                Type assetType = asset.GetType();
                if(assetType == texture2DType) {
                    /**
                    * If there are custom textures that have been update, keep a list of which onces have changed and update the brushCollection.
                    */
                    if(path.StartsWith(BrushCollection.localCustomBrushPath)) {
                        customBrushPaths.Add(path);
                    }
                } else if(assetType == terrainDataType) {
                    /**
                    * Check if the terrain has been modified externally. If this terrain's path matches this any terrain grid terrain,
                    * update the heights array.
                    */
                    foreach(TerrainInformation terrainInformation in terrainInformations) {
                        if(terrainInformation.ignoreOnAssetsImported) {
                            terrainInformation.ignoreOnAssetsImported = false;
                            continue;
                        }

                        if(terrainInformation.terrainData == null) continue;

                        float[,] temporaryHeights;
                        if(terrainInformation.terrainAssetPath == path && terrainInformation.terrainData.heightmapResolution == heightmapResolution) {
                            temporaryHeights = terrainInformation.terrainData.GetHeights(0, 0, heightmapResolution, heightmapResolution);
                            for(int x = 0; x < heightmapResolution; x++) {
                                for(int y = 0; y < heightmapResolution; y++) {
                                    allTerrainHeights[terrainInformation.heightmapYOffset + y, terrainInformation.heightmapXOffset + x] = temporaryHeights[y, x];
                                }
                            }
                        }
                    }
                }
            }

            if(customBrushPaths.Count > 0) {
                BrushCollection.RefreshCustomBrushes(customBrushPaths.ToArray());
                BrushCollection.UpdatePreviewTextures();
            }
        }
        
        // Check if the terrain asset has been moved.
        private void OnAssetsMoved(string[] sourcePaths, string[] destinationPaths) {
            for(int i = 0; i < sourcePaths.Length; i++) {
                foreach(TerrainInformation terrainInfo in terrainInformations) {
                    if(sourcePaths[i] == terrainInfo.terrainAssetPath) {
                        terrainInfo.terrainAssetPath = destinationPaths[i];
                    }
                }
            }
        }

        private void OnAssetsDeleted(string[] paths) {
            List<string> deletedCustomBrushPaths = new List<string>();

            foreach(string path in paths) {
                if(path.StartsWith(BrushCollection.localCustomBrushPath)) {
                    deletedCustomBrushPaths.Add(path);
                }
            }

            if(deletedCustomBrushPaths.Count > 0) {
                BrushCollection.RemoveDeletedBrushes(deletedCustomBrushPaths.ToArray());
                BrushCollection.UpdatePreviewTextures();
            }
        }

        private int[] previousTerrainLayers;
        // HACK: Ignore the terrain colliders by setting their layer to IgnoreRaycast temporarily.
        internal void IgnoreRaycastsOnTerrains() {
            previousTerrainLayers = new int[terrainInformations.Count];
            for(int t = 0; t < terrainInformations.Count; t++) {
                previousTerrainLayers[t] = terrainInformations[t].terrain.gameObject.layer;
                terrainInformations[t].terrain.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        }

        internal void ResetLayerOfTerrains() {
            // Reset all terrain layers to their previous values.
            for(int t = 0; t < terrainInformations.Count; t++) {
                terrainInformations[t].terrain.gameObject.layer = previousTerrainLayers[t];
            }
        }
        
        /// <summary>
        /// Checks if the secondary Terrain will be able to become part of a contiguous grid to the primary Terrain object.
        /// </summary>
        private static bool AreTerrainsContiguous(Terrain primary, Terrain secondary) {
            Vector3 terrainSize = primary.terrainData.size;
            return
                (primary.transform.position.x - secondary.transform.position.x) % terrainSize.x == 0f &&
                (primary.transform.position.z - secondary.transform.position.z) % terrainSize.z == 0f &&
                secondary.transform.position.y == primary.transform.position.y &&
                secondary.terrainData.size == terrainSize;
        }

        // Clamp the falloff curve's values from time 0-1 and value 0-1
        private static void ClampAnimationCurve(AnimationCurve curve) {
            for(int i = 0; i < curve.keys.Length; i++) {
                Keyframe keyframe = curve.keys[i];
                curve.MoveKey(i, new Keyframe(Mathf.Clamp01(keyframe.time), Mathf.Clamp01(keyframe.value), keyframe.inTangent, keyframe.outTangent));
            }
        }

        /**
        * A modified version of the LinePlaneIntersection method from the 3D Math Functions script found on the Unify site 
        * Credit to Bit Barrel Media: http://wiki.unity3d.com/index.php?title=3d_Math_functions
        * This code has been modified to fit my needs and coding style.
        *---
        * Get the intersection between a line and a XZ facing plane. 
        * If the line and plane are not parallel, the function outputs true, otherwise false.
        */
        private bool LinePlaneIntersection(out Vector3 intersectingPoint) {
            Vector3 planePoint = new Vector3(0f, lastClickPosition.y, 0f);

            Ray mouseRay = HandleUtility.GUIPointToWorldRay(mousePosition);

            // Calculate the distance between the linePoint and the line-plane intersection point
            float dotNumerator = Vector3.Dot((planePoint - mouseRay.origin), Vector3.up);
            float dotDenominator = Vector3.Dot(mouseRay.direction, Vector3.up);

            // Check if the line and plane are not parallel
            if(dotDenominator != 0f) {
                float length = dotNumerator / dotDenominator;

                // Create a vector from the linePoint to the intersection point and set the vector length by normalizing and multiplying by the length
                Vector3 vector = mouseRay.direction * length;

                // Get the coordinates of the line-plane intersection point
                intersectingPoint = mouseRay.origin + vector;

                return true;
            } else {
                intersectingPoint = Vector3.zero;
                return false;
            }
        }
        
        // Checks if the cursor is hovering over the terrain
        private bool Raycast(out Vector3 pos, out Vector2 uv) {
            RaycastHit hitInfo;

            float closestSqrDistance = float.MaxValue;
            pos = Vector3.zero;
            uv = Vector2.zero;
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            foreach(TerrainInformation terrainInformation in terrainInformations) {
                if(terrainInformation.collider.Raycast(mouseRay, out hitInfo, float.PositiveInfinity)) {
                    float sqrDistance = (mouseRay.origin - hitInfo.point).sqrMagnitude;
                    if(sqrDistance < closestSqrDistance) {
                        closestSqrDistance = sqrDistance;
                        pos = hitInfo.point;
                        uv = hitInfo.textureCoord;
                    }
                }
            }

            return closestSqrDistance != float.MaxValue;
        }

        private static float CalculateDistance(float x1, float y1, float x2, float y2) {
            float deltaX = x1 - x2;
            float deltaY = y1 - y2;

            float magnitude = deltaX * deltaX + deltaY * deltaY;

            return Mathf.Sqrt(magnitude);
        }

        internal void UpdateSetHeightAtMousePosition() {
            RaycastHit hitInfo;
            if(Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out hitInfo)) {
                settings.setHeight = hitInfo.point.y - firstTerrainTransform.position.y;
                Repaint();
            }
        }

        internal float[,] GetBrushSamplesWithSpeed() { 
            return BrushCollection.brushes[settings.modeSettings[CurrentTool].selectedBrushId].samplesWithSpeed;
        }

        private bool GetTerrainHeightAtMousePosition(out float height) {
            RaycastHit hitInfo;
            foreach(TerrainInformation terrainInformation in terrainInformations) {
                if(terrainInformation.collider.Raycast(HandleUtility.GUIPointToWorldRay(mousePosition), out hitInfo, float.PositiveInfinity)) {
                    height = hitInfo.point.y - terrainInformation.transform.position.y;
                    return true;
                }
            }
            
            height = 0f;
            return false;
        }

        /**
        * Gets the mouse position in world space. This is a utlity method used to automatically get the position of 
        * the mouse depending on if it's being held down or not. Returns true if the terrain or plane was hit, 
        * returns false otherwise.
        */
        private bool GetMousePositionInWorldSpace(out Vector3 position) {
            if(mouseIsDown && 
                (CurrentTool != Tool.SetHeight && Event.current.shift == false) &&
                (settings.raycastOntoFlatPlane || CurrentTool == Tool.SetHeight || CurrentTool == Tool.Flatten)) {
                if(LinePlaneIntersection(out position) == false) {
                    SetCursorEnabled(false);
                    return false;
                }
            } else {
                Vector2 uv;
                if(Raycast(out position, out uv) == false) {
                    SetCursorEnabled(false);
                    return false;
                }
            }

            return true;
        }

        private void SetCursorEnabled(bool enabled) {
            brushProjectorGameObject.SetActive(enabled);
            topPlaneGameObject.SetActive(enabled);
        }

        private int GetSegmentsFromUnits(float units) {
            float segmentDensity = currentToolsResolution / terrainSize.x;

            return Mathf.RoundToInt(units * segmentDensity);
        }
        
        internal void UpdateAllAlphamapSamplesFromSourceAssets() {
            Debug.Log("UpdateAllAlphamapSamplesFromSourceAssets");

            allTextureSamples = new float[firstTerrainData.alphamapHeight * numberOfTerrainsVertically, 
                firstTerrainData.alphamapWidth * numberOfTerrainsHorizontally, firstTerrainData.alphamapLayers];
            float[,,] currentAlphamapSamples;

            foreach(TerrainInformation terrainInfo in terrainInformations) {
                currentAlphamapSamples = terrainInfo.terrainData.GetAlphamaps(0, 0, firstTerrainData.alphamapWidth, firstTerrainData.alphamapHeight);

                for(int l = 0; l < firstTerrainData.alphamapLayers; l++) {
                    for(int x = 0; x < firstTerrainData.alphamapWidth; x++) {
                        for(int y = 0; y < firstTerrainData.alphamapHeight; y++) {
                            allTextureSamples[terrainInfo.alphamapsYOffset + y, terrainInfo.alphamapsXOffset + x, l] =
                                currentAlphamapSamples[y, x, l];
                        }
                    }
                }
            }
        }

        private void UpdateAllAlphamapSamplesInSourceAssets() {
            float[,,] newAlphamaps = new float[alphamapResolution, alphamapResolution, firstTerrainData.alphamapLayers];
            Debug.Log("UpdateAllAlphamapSamplesInSourceAssets");
            foreach (TerrainInformation terrainInfo in terrainInformations) {
                for(int l = 0; l < firstTerrainData.alphamapLayers; l++) {
                    for(int x = 0; x < alphamapResolution; x++) {
                        for(int y = 0; y < alphamapResolution; y++) {
                            newAlphamaps[x, y, l] = allTextureSamples[x + terrainInfo.alphamapsXOffset, y + terrainInfo.alphamapsYOffset, l];
                        }
                    }
                }
                terrainInfo.terrainData.SetAlphamaps(0, 0, newAlphamaps);
            }
        }

        private void UpdateAllHeightsFromSourceAssets() {
            Debug.Log("UpdateAllHeightsFromSourceAssets");
            float[,] temporaryHeights;
            foreach(TerrainInformation terrainInformation in terrainInformations) {
                temporaryHeights = terrainInformation.terrainData.GetHeights(0, 0, heightmapResolution, heightmapResolution);
                for(int y = 0; y < heightmapResolution; y++) {
                    for(int x = 0; x < heightmapResolution; x++) {
                        allTerrainHeights[terrainInformation.heightmapYOffset + y, terrainInformation.heightmapXOffset + x] = temporaryHeights[y, x];
                    }
                }
            }
        }

        private void UpdateAllHeightsInSourceAssets() {
            Debug.Log("UpdateAllHeightsInSourceAssets");
            float[,] temporaryHeights = new float[heightmapResolution, heightmapResolution];
            foreach(TerrainInformation terrainInformation in terrainInformations) {
                for(int x = 0; x < heightmapResolution; x++) {
                    for(int y = 0; y < heightmapResolution; y++) {
                        temporaryHeights[y, x] = allTerrainHeights[y + terrainInformation.heightmapYOffset, x + terrainInformation.heightmapXOffset];
                    }
                }

                terrainInformation.terrainData.SetHeights(0, 0, temporaryHeights);
            }
        }
        //   HashSet<int> visibleCells = new HashSet<int>();
        // int lastGetVisibleTime;
        private void UpdateAllUnmodifiedHeighOnLoad()
        {
            if (allUnmodifiedTerrainHeights == null || allUnmodifiedTerrainHeights.GetLength(0) < heightmapWidth ||
           allUnmodifiedTerrainHeights.GetLength(1) < heightmapHeight)
            {
                allUnmodifiedTerrainHeights = new float[heightmapWidth, heightmapHeight];
            }
            for (int x = 0; x < heightmapWidth; x++)
            {
                for (int y = 0; y < heightmapHeight; y++)
                {
                    allUnmodifiedTerrainHeights[x, y] = allTerrainHeights[x, y];
                }
            }
        }
       
            private void UpdateAllUnmodifiedHeights() {

            if(allUnmodifiedTerrainHeights == null || allUnmodifiedTerrainHeights.GetLength(0) < heightmapWidth || 
                allUnmodifiedTerrainHeights.GetLength(1) < heightmapHeight) {
                allUnmodifiedTerrainHeights = new float[heightmapWidth, heightmapHeight];
            }
            //if (System.DateTime.Now.Second != lastGetVisibleTime )
            //{
            //    visibleCells.Clear();

            //      lastGetVisibleTime = System.DateTime.Now.Second;
            //    foreach (Transform item in this.firstTerrainTransform.parent)
            //    {
            //        if (item.gameObject.activeInHierarchy)
            //        {
            //            string[] childName = item.name.Split(" ".ToArray());
            //            string[] xy = childName[childName.Length - 1].Split("x".ToArray());
            //            // Debug.Log(int.Parse(xy[0])+","+int.Parse(xy[1]));
            //            visibleCells.Add(int.Parse(xy[0]) - 1 + (int.Parse(xy[1]) - 1) * 100);
            //        }
            //    }
            //}
            // Debug.Log("allUnmodifiedTerrainHeights:" + allUnmodifiedTerrainHeights.GetLength(0) + "," + allUnmodifiedTerrainHeights.GetLength(1));
            // Debug.Log("allTerrainHeights:" + allTerrainHeights.GetLength(0) + "," + allTerrainHeights.GetLength(1));
            //Debug.Log("heightmapWidth:" + heightmapWidth + ",heightmapHeight" + heightmapHeight);
          //  int xStart =Math.Max(0, (int)TerrainFormerCell.cellRect.xMin);
          //  int xEnd = Math.Min(heightmapWidth, (int)TerrainFormerCell.cellRect.xMax);
          //  int yStart = Math.Max(0, (int)TerrainFormerCell.cellRect.yMin);
           // int yEnd = Math.Min(heightmapHeight, (int)TerrainFormerCell.cellRect.yMax);
           // for (int x = xStart; x < xEnd; x++) {
             
                
             //   for (int y = yStart; y < yEnd; y++) {
                   
                  //  allUnmodifiedTerrainHeights[x, y] = allTerrainHeights[y,x];
     
              //  }
           // }
        
        }

        internal static ModeSettings GetCurrentToolSettings() {
            return settings.modeSettings[Instance.CurrentTool];
        }
        
        [Flags]
        private enum SamplesDirty {
            None = 0,
            InspectorTexture = 1,
            ProjectorTexture = 2,
            BrushSamples = 4,
        }
    }
}
