using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
[InitializeOnLoad]
public class SlopePenEditor : Editor {
    static SlopePenEditor() {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }
     [MenuItem("地形/启动斜坡功能")]
    private static void init() {
        Debug.LogError(getTerrainPaintSize());
        if (colliderSlopePen == null)
        {
            GameObject SlopePen = GameObject.Find("SlopePen");
            if (SlopePen == null) {
                SlopePen= Instantiate( Resources.Load<GameObject>("SlopePen")) ;
                SlopePen.name = "SlopePen";
                }

            colliderSlopePen = SlopePen.GetComponent<Collider>();
            colliderSlopePen.gameObject.layer = 30;
        }
    
    }

     private static Collider colliderSlopePen;
    private static Vector3 startSlopPos;
    private static Vector3 endSlopPos;
    private static  bool usingSlopeRect=false;
    private static int sideLevel=0;
    private void OnGUI()
    {
       // slopeForwardPower = EditorGUILayout.Slider("坡度方向力度", slopeForwardPower,0,1);
       // slopeSidePower = EditorGUILayout.Slider("坡度横向力度", slopeSidePower, 0, 1);
    }
    private static void OnSceneGUI(SceneView sceneView)
    {

        if (Application.isPlaying) return;
       
        if (startSlopPos != Vector3.zero) {
            Handles.color = Color.red;
            Handles.DrawSolidDisc(startSlopPos, Vector3.up, 0.2f);

        }
        if (endSlopPos != Vector3.zero)
        {
            Handles.color = Color.green;
            Handles.DrawSolidDisc(endSlopPos, Vector3.up, 0.2f);

        }

        if (Selection.activeGameObject == null) return;

        Terrain terrain = Selection.activeGameObject.GetComponent<Terrain>();
        if (terrain == null) return;
        if (Event.current.keyCode >= KeyCode.Alpha1&& Event.current.keyCode <= KeyCode.Alpha9 && Event.current.type == EventType.KeyDown){
            if (Event.current.shift)
            {
                slopwCreate(terrain, true, ((Event.current.keyCode- KeyCode.Alpha1)*0.1f+0.1f));
            }
            return;
        }
            if ((Event.current.keyCode == KeyCode.K|| Event.current.keyCode == KeyCode.L) && Event.current.type == EventType.KeyDown)
        {
       
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            foreach (var item in Physics.RaycastAll(ray, 1000))
            {
                if (item.collider.gameObject == terrain.gameObject)
                {
                    if (Event.current.keyCode == KeyCode.K)
                    {
                        startSlopPos = item.point;
                    }

                    else
                    {
                        endSlopPos = item.point;
                        movePenRect(terrain);
                    }

                    break;
                }
            }
        }


        if (colliderSlopePen == null) return;
        Handles.color = Color.red;
        //返回旋转角度,绘制三个方向上的比例  
        float scale = 1;
        scale = Handles.ScaleValueHandle(colliderSlopePen.transform.localScale.x, startSlopPos, Quaternion.identity, 1, Handles.CubeCap, 0);
         
        if (GUI.changed)
        {
        
                EditorUtility.SetDirty(colliderSlopePen.gameObject);
                colliderSlopePen.transform.localScale = new Vector3(scale, (endSlopPos - startSlopPos).magnitude, 0);
 
        }
        if (Event.current.button == 0 && (Event.current.type==EventType.mouseUp))
        {
            if (usingSlopeRect == false) return;
 
            slopwCreate(terrain);
        }
  

    }

    private static void movePenRect(Terrain terrain)
    {
        if (colliderSlopePen == null)
        {
            return;
        }
        colliderSlopePen.transform.position = (startSlopPos + endSlopPos) / 2;
        colliderSlopePen.transform.rotation = Quaternion.LookRotation(endSlopPos - startSlopPos);
        colliderSlopePen.transform.Rotate(90, 0, 0);
        //var lpos = colliderSlopePen.transform.InverseTransformPoint(endSlopPos - startSlopPos);
        colliderSlopePen.transform.localScale = new Vector3(getTerrainPaintSize(), (endSlopPos - startSlopPos).magnitude, 0);
        usingSlopeRect = true;
    }

    private static void slopwCreate(  Terrain terrain,bool forceToMesh=false,float sidePower=1)
    {


    
        float baseY =  terrain.GetPosition().y;
        float[,] heights = terrain.terrainData.GetHeights(0, 0,terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight);// new float[terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight];
        HashSet<int> donePoint = new HashSet<int>(); 
        for (float x = colliderSlopePen.bounds.min.x; x < colliderSlopePen.bounds.max.x; x+=0.2f)
        {
            for (float z= colliderSlopePen.bounds.min.z; z < colliderSlopePen.bounds.max.z; z+=0.2f)
            {
             Ray ray=new Ray(new Vector3(x,baseY,z),Vector3.up );
               
                RaycastHit hitInfo;
                if(Physics.Raycast(ray,out hitInfo,500,1<<30))
                {
                    
                  Vector3 startPosXY= worldPositionToArrayIndex(terrain, hitInfo.point);
                    if (donePoint.Contains((int)startPosXY.z * 10000 + (int)startPosXY.x)) continue;
                    donePoint.Add((int)startPosXY.z * 10000 + (int)startPosXY.x);
                    if (forceToMesh) {
                        float power = 1f;
                      Vector3 lpos=  colliderSlopePen.transform.InverseTransformPoint(hitInfo.point);

                    power=  Mathf.Lerp(1,0, ( Mathf.Abs(lpos.x) * 2 - sidePower) /(1.001f- sidePower));

                        heights[(int)startPosXY.z, (int)startPosXY.x] = Mathf.Lerp(heights[(int)startPosXY.z, (int)startPosXY.x], (hitInfo.point.y - baseY) / terrain.terrainData.size.y, power);
                    } else
                    {
                        heights[(int)startPosXY.z, (int)startPosXY.x] = Mathf.Min(heights[(int)startPosXY.z, (int)startPosXY.x], (hitInfo.point.y - baseY) / terrain.terrainData.size.y);

                    }
                }
            }
            } 

        terrain.terrainData.SetHeights(0,0,heights); 
    }


    public static int  getTerrainPaintSize()
    {
        Assembly editorAssem = typeof(UnityEditor.AudioImporter).Assembly;
        Type aiType1 = editorAssem.GetType("UnityEditor.AudioImporter");
        Type aiType2 = typeof(UnityEditor.AudioImporter);

        Type tiType1 = editorAssem.GetType("UnityEditor.TerrainInspector");
        UnityEngine.Object[] tis = UnityEngine.Object.FindObjectsOfTypeAll(tiType1);
        if (tis.Length > 0)
        {
            UnityEngine.Object ti = tis[0];
            var sizeField = ti.GetType().GetField("m_Size", BindingFlags.NonPublic | BindingFlags.Instance);
            if (sizeField != null)
            {
                var sizeValue = sizeField.GetValue(ti);
                
                return (int)sizeValue;
            }
        }
    
        return 1;
    }
        private static Vector2 translateAxis(int x, int y, float alpha)
    {
        float beta = Mathf.Atan2(y, x);
        return  new Vector2(x*Mathf.Cos(alpha-beta)/Mathf.Cos(beta),y*Mathf.Sin(alpha-beta)/Mathf.Sin(beta));
    }

    static Vector3 worldPositionToArrayIndex(Terrain terrain,Vector3 wpos) {
        var startPosXY = (wpos - terrain.GetPosition());
        startPosXY.x /= terrain.terrainData.size.x;
        startPosXY.z /= terrain.terrainData.size.z;
        int offsetX = (int)(startPosXY.x * terrain.terrainData.heightmapResolution);
        int offsetZ = (int)(startPosXY.z * terrain.terrainData.heightmapResolution);
        int offsetY = (int)(startPosXY.y * terrain.terrainData.size.y);
        return new Vector3(offsetX, offsetY, offsetZ);
    }
}
