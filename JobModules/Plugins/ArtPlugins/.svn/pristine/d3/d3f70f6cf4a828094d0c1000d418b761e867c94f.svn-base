using UnityEngine;
using UnityEditor;
using System.Collections;
//using UnityEngine.PostProcessing;

public class AQUAS_RiverSetup : EditorWindow {

    public static AQUAS_RiverSetup window;

    public float waterLevel;
    public GameObject terrain;
    public GameObject waterPlane = null;
    public GameObject camera = null;

    //Add menu item to show window
    [MenuItem("Window/AQUAS/River Setup")]

    public static void OpenWindow() {
        window = (AQUAS_RiverSetup)EditorWindow.GetWindow(typeof(AQUAS_RiverSetup));
        window.titleContent.text = "AQUAS River";
    }

    void OnGUI(){
        if (window == null)
        {
            OpenWindow();
        }

        GUILayout.Label("River Setup", EditorStyles.boldLabel);
        waterLevel = EditorGUI.FloatField(new Rect(5, 30, position.width - 10, 16), "Water Level", waterLevel);
        terrain = (GameObject)EditorGUI.ObjectField(new Rect(5, 50, position.width - 10, 16), "Terrain", terrain, typeof(GameObject), true);
        camera = (GameObject)EditorGUI.ObjectField(new Rect(5, 70, position.width - 10, 16), "Camera", camera, typeof(GameObject), true);

        waterPlane = (GameObject)EditorGUI.ObjectField(new Rect(5, 180, position.width - 10, 16), "Water Plane", waterPlane, typeof(GameObject), true);
        
        if (GUI.Button(new Rect(5, 100, position.width - 10, 32), "About"))
        {
            About();
        }

        if (GUI.Button(new Rect(5, 140, position.width - 10, 32), "Add River Plane"))
        {
            AddRiverPlane();
        }

        if (GUI.Button(new Rect(5, 200, position.width - 10, 32), "Create river reference image"))
        {
            CreateRiverReference();
        }

        if (GUI.Button(new Rect(5, 240, position.width - 10, 32), "Add Underwater Effects"))
        {
            AddUnderwaterEffects();
        }

        if (GUI.Button(new Rect(5, 280, position.width / 2 - 5, 32), "Hook Up Camera"))
        {
            HookupCamera();
        }

        if (GUI.Button(new Rect(position.width / 2 + 5, 280, position.width / 2 - 5, 32), "<== Info"))
        {
            HookupCameraInfo();
        }

        if (GUI.Button(new Rect(5, 320, position.width - 10, 32), "Hints"))
        {
            Hints();
        }
    }

    void Update() {
        Repaint();
    }

    //<summary>
    //Gives some info on the river setup feature
    //</summary>
    void About()
    {
        EditorUtility.DisplayDialog("AQUAS River Setup", "The river setup will guide you through the process of creating a river with AQUAS. You can add water, underwater effects and export a river reference image to help you paint functional flowmaps. Flowmaps can be painted using the free tool 'Flowmap Painter'. Please keep in mind that this is a basic setup. More advanced setups (e.g. multiple water levels) have to be done manually.", "Got It");
    }

    void AddRiverPlane()
    {
        if (terrain == null || camera == null)
        {
            EditorUtility.DisplayDialog("Specify Parameters", "Please specify the required parameters (terrain, camera & water level)", "Ok");
            return;
        }

        //Get scene info
        float terrainBounds = terrain.GetComponent<Terrain>().terrainData.size.x;
        Vector3 waterPosition = new Vector3(terrain.transform.position.x, 0, terrain.transform.position.z) + new Vector3(terrainBounds / 2, waterLevel, terrainBounds / 2);

        //Check if AQUAS is already in the scene
        if (GameObject.Find("AQUAS Waterplane") != null)
        {
            EditorUtility.DisplayDialog("Can't add river", "AQUAS is already in the scene", "OK");
            return;
        }

        //Try to add AQUAS river
        GameObject aquasObj = GameObject.CreatePrimitive(PrimitiveType.Plane);

        if (aquasObj == null)
        {
            Debug.LogWarning("Unable to create AQUAS object - Aborting!");
            return;
        }

        //Position and configure the waterplane correctly
        else
        {
            aquasObj.name = "AQUAS Waterplane";
            aquasObj.transform.position = waterPosition;
            aquasObj.transform.localScale = new Vector3(terrainBounds * 0.1f, terrainBounds * 0.1f, terrainBounds * 0.1f);
            DestroyImmediate(aquasObj.GetComponent<MeshCollider>());
            aquasObj.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            aquasObj.GetComponent<Renderer>().material = (Material)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Materials/Water/Desktop&Web/River.mat", typeof(Material));
            aquasObj.AddComponent<AQUAS_Reflection>();
            aquasObj.AddComponent<AQUAS_RenderQueueEditor>();

            if (waterPlane == null)
            {
                waterPlane = aquasObj;
            }

            if (camera.gameObject.GetComponent<AQUAS_Camera>() == null)
            {
                camera.gameObject.AddComponent<AQUAS_Camera>();
            }


            //Check if Tenkoku is in the scene and set reflection probe accordingly
            if (GameObject.Find("Tenkoku DynamicSky") != null)
            {

            }

            else
            {
                GameObject refProbe = new GameObject("Reflection Probe");
                refProbe.transform.position = waterPosition;
                refProbe.AddComponent<ReflectionProbe>();
                refProbe.GetComponent<ReflectionProbe>().intensity = 0.3f;
                refProbe.transform.SetParent(aquasObj.transform);
                refProbe.GetComponent<ReflectionProbe>().mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
            }

            //Add caustics
            GameObject primaryCausticsPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Prefabs/PrimaryCausticsProjector.prefab", typeof(GameObject));
            GameObject primaryCausticsObj = Instantiate(primaryCausticsPrefab);
            primaryCausticsObj.name = "PrimaryCausticsProjector";
            GameObject secondaryCausticsPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Prefabs/SecondaryCausticsProjector.prefab", typeof(GameObject));
            GameObject secondaryCausticsObj = Instantiate(secondaryCausticsPrefab);
            secondaryCausticsObj.name = "SecondaryCausticsProjector";

            primaryCausticsObj.transform.position = waterPosition;
            secondaryCausticsObj.transform.position = waterPosition;

            primaryCausticsObj.GetComponent<Projector>().orthographicSize = terrainBounds/2;
            secondaryCausticsObj.GetComponent<Projector>().orthographicSize = terrainBounds/2;

            primaryCausticsObj.transform.SetParent(aquasObj.transform);
            secondaryCausticsObj.transform.SetParent(aquasObj.transform);

            //Set some material parameters based on whether Tenkoku is in the scene or not
            if (GameObject.Find("Tenkoku DynamicSky") != null)
            {
                aquasObj.GetComponent<Renderer>().sharedMaterial.SetFloat("_EnableCustomFog", 1);
                aquasObj.GetComponent<Renderer>().sharedMaterial.SetFloat("_Specular", 0.5f);
            }

            else
            {
                aquasObj.GetComponent<Renderer>().sharedMaterial.SetFloat("_EnableCustomFog", 0);
                aquasObj.GetComponent<Renderer>().sharedMaterial.SetFloat("_Specular", 1);
            }
        }
    }

    //<summary>
    //Create a river reference image to help paint the flowmap on
    //</summary>
    void CreateRiverReference() {

        //Check if required objects (camera & river plane) are specified and abort if not
        if(camera==null)
        {
            EditorUtility.DisplayDialog("Camera Missing!", "Please specify the main camera before creating a river reference.", "OK");
            return;
        }

        if(waterPlane==null)
        {
            EditorUtility.DisplayDialog("River Plane Missing", "Please add a river plane before creating a river reference.", "OK");
            return;
        }

        #region Screenshot setup
        //Cache the camera's parameters
        Vector3 cameraPosition = camera.transform.position;
        Quaternion cameraRotation = camera.transform.rotation;
        bool projection = camera.GetComponent<Camera>().orthographic;
        float orthographicSize = camera.GetComponent<Camera>().orthographicSize;

        //Set the camera's position and parameters for taking a screenshot
        camera.transform.position = new Vector3(waterPlane.transform.position.x, waterPlane.transform.position.y + (camera.GetComponent<Camera>().farClipPlane-50), waterPlane.transform.position.z);
        camera.transform.rotation = Quaternion.Euler(90, 0, 0);
        camera.GetComponent<Camera>().orthographic = true;
        camera.GetComponent<Camera>().orthographicSize = waterPlane.transform.localScale.x * 5;

        //Cache the material from the river plane and set the material with the reference texture on it
        Material cachedMaterial = waterPlane.GetComponent<UnityEngine.Renderer>().sharedMaterial;
        Material referenceTexMat = (Material)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Materials/RiverRef.mat", typeof(Material));
        waterPlane.GetComponent<UnityEngine.Renderer>().sharedMaterial = referenceTexMat;
        #endregion

        #region Take & save screenshot
        //Take a screenshot from the river plane and save it to use as a reference texture to help paint flowmaps
        //Source: http://answers.unity3d.com/questions/22954/how-to-save-a-picture-take-screenshot-from-a-camer.html
        RenderTexture rt = new RenderTexture(1024, 1024, 24);

        camera.GetComponent<Camera>().targetTexture = rt;

        Texture2D riverReference = new Texture2D(1024, 1024, TextureFormat.RGB24, false);

        camera.GetComponent<Camera>().Render();
        RenderTexture.active = rt;
        riverReference.ReadPixels(new Rect(0, 0, 1024, 1024), 0, 0);
        camera.GetComponent<Camera>().targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);

        byte[] bytes = riverReference.EncodeToJPG();
        string filename = string.Format("{0}/RiverReferences/Reference_{1}.jpg", Application.dataPath, System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));

        if (!System.IO.Directory.Exists(string.Format("{0}/RiverReferences", Application.dataPath)))
        {
            System.IO.Directory.CreateDirectory(string.Format("{0}/RiverReferences", Application.dataPath));
        }

        System.IO.File.WriteAllBytes(filename, bytes);
        #endregion

        #region Revert scene to original state
        camera.transform.position = cameraPosition;
        camera.transform.rotation = cameraRotation;
        camera.GetComponent<Camera>().orthographic = projection;
        camera.GetComponent<Camera>().orthographicSize = orthographicSize;

        waterPlane.GetComponent<UnityEngine.Renderer>().sharedMaterial = cachedMaterial;
        #endregion

        AssetDatabase.Refresh();
    }

    //<summary>
    //Adds Underwater Effects to the scene
    //</summary>
    void AddUnderwaterEffects()
    {

        GameObject underwaterPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/AQUAS/Prefabs/UnderWaterCameraEffects.prefab", typeof(GameObject));

        //Check if AQUAS is already in the scene
        if (GameObject.Find("AQUAS Waterplane") == null)
        {
            EditorUtility.DisplayDialog("AQUAS is not in the scene", "Please add water first", "Ok");
            return;
        }

        //Try to add underwater effects
        else
        {

            float terrainBounds = terrain.GetComponent<Terrain>().terrainData.size.x;

            if (GameObject.Find("UnderWaterCameraEffects") != null)
            {
                EditorUtility.DisplayDialog("Can't add underwater effects", "Underwater Effects are already in the scene", "Ok");
                return;
            }

            GameObject underwaterObj = Instantiate(underwaterPrefab);
            underwaterObj.name = "UnderWaterCameraEffects";

            camera.GetComponent<Camera>().farClipPlane = terrainBounds;

            //Underwater effects setup
            underwaterObj.transform.SetParent(camera.transform);
            underwaterObj.transform.localPosition = new Vector3(0, 0, 0);
            underwaterObj.transform.localEulerAngles = new Vector3(0, 0, 0);
            underwaterObj.GetComponent<AQUAS_LensEffects>().gameObjects.mainCamera = camera.gameObject;
            underwaterObj.GetComponent<AQUAS_LensEffects>().gameObjects.waterPlanes[0] = GameObject.Find("AQUAS Waterplane");
            underwaterObj.GetComponent<AQUAS_LensEffects>().gameObjects.useSquaredPlanes = true;

            //Add and configure image effects neccessary for AQUAS
            //if (camera.gameObject.GetComponent<PostProcessingBehaviour>() == null)
            //{
            //    camera.gameObject.AddComponent<PostProcessingBehaviour>();
            //}

            if (camera.gameObject.GetComponent<AudioSource>() == null)
            {
                camera.gameObject.AddComponent<AudioSource>();
            }

            //Add borders to the scene for correct fog

            if (GameObject.Find("Borders") != null)
            {
                return;
            }

            GameObject borders = new GameObject();
            borders.name = "Borders";

            Vector3 waterPosition = waterPlane.transform.position;

            GameObject borderLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            borderLeft.name = "Left Border";
            borderLeft.transform.localScale = new Vector3(terrainBounds, waterLevel, 0.1f);
            borderLeft.transform.position = new Vector3(waterPosition.x, waterLevel - (waterLevel / 2), waterPosition.z + terrainBounds / 2);

            GameObject borderRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            borderRight.name = "Right Border";
            borderRight.transform.localScale = new Vector3(terrainBounds, waterLevel, 0.1f);
            borderRight.transform.position = new Vector3(waterPosition.x, waterLevel - (waterLevel / 2), waterPosition.z - terrainBounds / 2);

            GameObject borderTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            borderTop.name = "Top Border";
            borderTop.transform.localScale = new Vector3(0.1f, waterLevel, terrainBounds);
            borderTop.transform.position = new Vector3(waterPosition.x + terrainBounds / 2, waterLevel - (waterLevel / 2), waterPosition.z);

            GameObject borderBottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
            borderBottom.name = "Bottom Border";
            borderBottom.transform.localScale = new Vector3(0.1f, waterLevel, terrainBounds);
            borderBottom.transform.position = new Vector3(waterPosition.x - terrainBounds / 2, waterLevel - (waterLevel / 2), waterPosition.z);

            borderLeft.transform.SetParent(borders.transform);
            borderRight.transform.SetParent(borders.transform);
            borderTop.transform.SetParent(borders.transform);
            borderBottom.transform.SetParent(borders.transform);

            borders.transform.SetParent(GameObject.Find("AQUAS Waterplane").transform);
        }
    }

    //<summary>
    //Adds image effects to camera and configures them
    //Does NOT add underwater effects & borders
    //</summary>
    void HookupCamera()
    {
     //   camera.gameObject.AddComponent<PostProcessingBehaviour>();

        //if (camera.gameObject.GetComponent<AudioSource>() == null)
        //{
        //    camera.gameObject.AddComponent<AudioSource>();
        //}
    }

    //<summary>
    //Displays info on hooking up the camera
    //</summary>
    void HookupCameraInfo()
    {
        EditorUtility.DisplayDialog("Hook Up Camera", "Hooking up the camera will configure it for underwater effects, but won't import the Effects prefab to the scene, nor will it create borders.", "Ok");
    }

    //<summary>
    //Add hints dialog
    //</summary>
    void Hints()
    {
        EditorUtility.DisplayDialog("Hints", "The quality of the river flow depends almost entirely on how well the flowmap is set up. High flow speeds will lead to visible edges. Smoothen and refine your flowmap with image editing software to get the best possible effect. Also make sure to set the maximum texture resolution to 4096 for best quality.", "Got It!");
    }
}
    

