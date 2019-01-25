
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace ArtPlugins.MapConfig
{
    [CustomEditor(typeof(MapPointRoot))]

    public class MapPointRootEditor : Editor
    {
        private static DefaultAsset item;
        private static DefaultAsset tempBindFolder;
        private static Scene currentDataScene;
        private static Dictionary<string, MapPointGroup> groupAssetsMap;
#if NewVersion
        public class SavedPointData {
            public Vector3 pos;
            public float dir;
            public float cylinderVolR;
            public float cylinderVolH;
        }
#endif
    
            private void OnEnable()
        {
            item = null;
         
            if (_config.groupAssetsGuid != null)
            {
              
               
                item = AssetDatabase.LoadAssetAtPath<DefaultAsset>(AssetDatabase.GUIDToAssetPath(_config.groupAssetsGuid));
            }
        }

        private MapPointRoot _config {
            get { return target as MapPointRoot; }
        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            GUILayout.Label("demoPointGroup 目录 测试用 正式自己建立目录 对应建筑 不要放插件目录");
            var newItem = EditorGUILayout.ObjectField(("点组预设目录"), item, typeof(DefaultAsset)) as DefaultAsset;

            if (newItem != item)
            {
                item = newItem;
                _config.groupAssetsGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(item));
            }
    
                if (GUILayout.Button("刷新场景"))
            {
                currentDataScene = EditorSceneManager.GetActiveScene();
                if (string.IsNullOrEmpty(currentDataScene.name) || string.IsNullOrEmpty(currentDataScene.path)) return;
                var groups = cleanAndReCreateChild("groups",_config.transform, false, "EditorOnly");
                var isolatedGroups = cleanAndReCreateChild("isolatedGroups", _config.transform, false, "EditorOnly");
                if(groupAssetsMap==null)
                groupAssetsMap = resetGroupAssetsMap();

                var oneSceneGroup = cleanAndReCreateChild(currentDataScene.name, groups, false, "EditorOnly");
           
                createAllGroup(oneSceneGroup, groupAssetsMap);

            }
            _config.dataFileName= EditorGUILayout.TextField("保存数据的xml文件名:",_config.dataFileName);
            if (GUILayout.Button("保存数据"))
            {
                int zeroCount;
                 saveRuntimeData(out zeroCount);
                 //PrefabUtility.(_config.gameObject);
                EditorUtility.DisplayDialog("保存成功", "完成数据保存 记得 保存场景,"+"id=0 数量为:"+zeroCount,"ok");
            }

            tempBindFolder = EditorGUILayout.ObjectField(("绑定的建筑物目录"), tempBindFolder, typeof(DefaultAsset)) as DefaultAsset;
            if (GUILayout.Button("创建新的点组并绑定到建筑")) {
                tempBindPoints(AssetDatabase.GetAssetPath(tempBindFolder));
            }

            if (GUILayout.Button("重新刷新绑定数据"))
            {
                groupAssetsMap = resetGroupAssetsMap();
            }

        }

        private void tempBindPoints(string buildingPath)
        {

 
      
       
            if (string.IsNullOrEmpty( buildingPath)) return;
            if (string.IsNullOrEmpty(_config.groupAssetsGuid)) return;

            var items = AssetDatabase.FindAssets(" t:prefab", new string[] { buildingPath });
            
            int count = 0;
            GameObject buildingDemo=Resources.Load<GameObject>("buildingDemo");
            int allItems = items.Length;
            int i = 0;
            foreach (var item in items)
            {



                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(item));
                if (asset.tag != "MapPointHouse")
                {

                    continue;
                }



                //PrefabUtility
                var mpGo = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(_config.groupAssetsGuid) + "/" + asset.name + ".prefab");
          
                if (mpGo==null){
                    mpGo = PrefabUtility.CreatePrefab(AssetDatabase.GUIDToAssetPath(_config.groupAssetsGuid) + "/" + asset.name + ".prefab", buildingDemo, ReplacePrefabOptions.Default);
            }
                MapPointGroup mp =mpGo.GetComponent<MapPointGroup>();
                mp.buildingAsset= asset;
                 
                count++;
                if (EditorUtility.DisplayCancelableProgressBar("绑定中...", "绑定 " + asset.name, i * 1.0f / allItems)) {
                    break;
                }
                i++;
                
            }
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("绑定完成", "创建新组 " + count + "个", "ok");
        }


#if NewVersion
        private void saveRuntimeData(out int zeroCount)
        {
            zeroCount = 0;
         

              Dictionary<int, List<SavedPointData>> idPoints = new Dictionary<int, List<SavedPointData>>();
           
          
            foreach (var pointGroup in _config.transform.Find("groups").GetComponentsInChildren<MapPointGroup>(false))
            {
                if (pointGroup.groupID <= 0)
                {
                    zeroCount++;
                    continue;
                }
          
                foreach (Transform item in pointGroup.transform)
                {
                    if (item.childCount < 1) continue;
                    int finalID = pointGroup.groupID * 100 + int.Parse(item.name);

                    if (idPoints.ContainsKey(finalID) == false)
                    {
                        idPoints[finalID] = new List<SavedPointData>();
                    }
                    List<SavedPointData> points = new List<SavedPointData>();
                    foreach (Transform child in item.transform)
                    {
                        Vector3 pos = child.position;
                        pos *= 100;
                        pos = new Vector3((int)pos.x, (int)pos.y, (int)pos.z) / 100;
                        SavedPointData sp=  new SavedPointData();
                        sp.pos = pos;
                        points.Add(sp);
                    }
                    idPoints[finalID].AddRange(points);

                }
            }

            foreach (var pointIso in _config.transform.Find("isolatedGroups").GetComponentsInChildren<MapPointISO>(false))
            {



                int finalID = pointIso.PointID;
                if (finalID == 0) { zeroCount++; continue; }
                    if (idPoints.ContainsKey(finalID) == false)
                    {
                        idPoints[finalID] = new List<SavedPointData>();
                    }
                   
                        Vector3 pos = pointIso.transform.position;
                        pos *= 100;
                        pos = new Vector3((int)pos.x, (int)pos.y, (int)pos.z) / 100;
                SavedPointData sp = new SavedPointData();
                sp.pos = pos;
                if (pointIso.enableDir) sp.dir = pointIso.dir;
                if (pointIso.enableCylinderVol)
                {
                    sp.cylinderVolR = pointIso.cylinderRadius;
                    sp.cylinderVolH = pointIso.cylinderHeight;
                }
                idPoints[finalID].Add(sp);
 
            }


            MapConfigPoints data = new MapConfigPoints();
            foreach (var item in idPoints)
            {
                data.IDPints.Add( new MapConfigPoints.ID_Point(item.Key, item.Value));
            }
          System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(MapConfigPoints));
            var folder = Directory.GetParent(AssetDatabase.GUIDToAssetPath(_config.groupAssetsGuid));
          
            Directory.CreateDirectory(folder.ToString() + "/data");
            StreamWriter writer = new StreamWriter(folder.ToString() + "/data/" +_config.dataFileName+".xml");
            xmlSerializer.Serialize(writer,data);
			writer.Close ();
            //  File.WriteAllText(AssetDatabase.GUIDToAssetPath(_config.groupAssetsGuid)+"/data/");
            // mapConfig.IDPints = saveDataList.ToArray();
            AssetDatabase.Refresh();
            
                }
#else

        private void saveRuntimeData(out int zeroCount)
        {
            zeroCount = 0;


            Dictionary<int, List<Vector3>> idPoints = new Dictionary<int, List<Vector3>>();


            foreach (var pointGroup in _config.transform.Find("groups").GetComponentsInChildren<MapPointGroup>(false))
            {
                if (pointGroup.groupID <= 0)
                {
                    zeroCount++;
                    continue;
                }

                foreach (Transform item in pointGroup.transform)
                {
                    if (item.childCount < 1) continue;
                    int finalID = pointGroup.groupID * 100 + int.Parse(item.name);

                    if (idPoints.ContainsKey(finalID) == false)
                    {
                        idPoints[finalID] = new List<Vector3>();
                    }
                    List<Vector3> points = new List<Vector3>();
                    foreach (Transform child in item.transform)
                    {
                        Vector3 pos = child.position;
                        pos *= 100;
                        pos = new Vector3((int)pos.x, (int)pos.y, (int)pos.z) / 100;
                        points.Add(pos);
                    }
                    idPoints[finalID].AddRange(points);

                }
            }

            foreach (var pointIso in _config.transform.Find("isolatedGroups").GetComponentsInChildren<MapPointISO>(false))
            {



                int finalID = pointIso.PointID;
                if (finalID == 0) { zeroCount++; continue; }
                if (idPoints.ContainsKey(finalID) == false)
                {
                    idPoints[finalID] = new List<Vector3>();
                }

                Vector3 pos = pointIso.transform.position;
                pos *= 100;
                pos = new Vector3((int)pos.x, (int)pos.y, (int)pos.z) / 100;
                idPoints[finalID].Add(pos);

            }


            MapConfigPoints data = new MapConfigPoints();
            foreach (var item in idPoints)
            {
                data.IDPints.Add(new MapConfigPoints.ID_Point(item.Key, item.Value));
            }
            System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(MapConfigPoints));
            var folder = Directory.GetParent(AssetDatabase.GUIDToAssetPath(_config.groupAssetsGuid));

            Directory.CreateDirectory(folder.ToString() + "/data");
            StreamWriter writer = new StreamWriter(folder.ToString() + "/data/" + _config.dataFileName + ".xml");
            xmlSerializer.Serialize(writer, data);
            writer.Close();
            //  File.WriteAllText(AssetDatabase.GUIDToAssetPath(_config.groupAssetsGuid)+"/data/");
            // mapConfig.IDPints = saveDataList.ToArray();
            AssetDatabase.Refresh();

        }
#endif

        private void createAllGroup(Transform groups, Dictionary<string, MapPointGroup> map)
        {
            int tick = System.Environment.TickCount;
             Dictionary<string, int> scenePointGroupIDs = new Dictionary<string, int>();
           
            foreach (var item in groups.GetComponentsInChildren<MapPointGroup>())
            {
                if(string.IsNullOrEmpty(item.buildingName)==false )
                    scenePointGroupIDs[item.buildingName] =item.groupID;
   
            }
            //clear all
            int count = groups.childCount;
            for (int i = 0; i < count; i++)
            {
                DestroyImmediate(groups.GetChild(0).gameObject);

            }


            //add all points
            var cscene = EditorSceneManager.GetActiveScene();
            foreach (var item in GameObject.FindGameObjectsWithTag("MapPointHouse"))
            {
                if (item.scene != cscene) continue;
               
                Debug.Log(item);
                var guid=AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(item)));
                if (map.ContainsKey(guid) == false) continue;
                MapPointGroup pointGroupInstance = Instantiate(map[guid].gameObject).GetComponent<MapPointGroup>();
                pointGroupInstance.transform.parent = groups;
                pointGroupInstance.transform.rotation = item.transform.rotation;

                pointGroupInstance.transform.position = item.transform.position;
                pointGroupInstance.buildingName = item.name;
                if (scenePointGroupIDs.ContainsKey(pointGroupInstance.buildingName))
                {
                    pointGroupInstance.groupID = scenePointGroupIDs[pointGroupInstance.buildingName];
                };
            }
            //    foreach (var root in roots)
            //    {

            //        foreach (var ep in root.GetComponentsInChildren<EmbedPrefab>())
            //        {
            //            if (ep.localIDInScene == 0) continue;
            //            if (map.ContainsKey(ep.PrefabInAssets) == false) continue;
            //            //
            //            MapPointGroup pointGroupInstance = Instantiate(map[ep.PrefabInAssets].gameObject).GetComponent<MapPointGroup>();
            //            pointGroupInstance.transform.parent = groups;
            //            pointGroupInstance.transform.rotation = ep.transform.rotation;

            //            pointGroupInstance.transform.position = ep.transform.position;
            //            pointGroupInstance.buildingInstanceID = ep.localIDInScene;
            //            if (scenePointGroupIDs.ContainsKey(ep.localIDInScene))
            //            {
            //                pointGroupInstance.groupID = scenePointGroupIDs[ep.localIDInScene];
            //            };


            //        }
            //    }
            //    Debug.Log("tick1=" + (System.Environment.TickCount - tick));

        }

        private Dictionary<string, MapPointGroup> resetGroupAssetsMap()
        {

            int tick = System.Environment.TickCount;
            Dictionary<string, MapPointGroup> groupAssetsMap = new Dictionary<string, MapPointGroup>();

            var items = AssetDatabase.FindAssets(" t:prefab", new string[] { AssetDatabase.GUIDToAssetPath(_config.groupAssetsGuid) });
            Debug.Log("tick2=" + (System.Environment.TickCount - tick));
            foreach (var item in items)
            {
                
                MapPointGroup pg = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(item)).GetComponent<MapPointGroup>();
                if (pg == null) continue;
                //prefab in prefab 
              
                if (pg.buildingAsset==null||pg.buildingAsset.transform.parent != null) {
                    Debug.LogError(pg + ",building =null");
                   // Debug.LogError(AssetDatabase.GetAssetOrScenePath( pg.building) +"==="+ pg.building.PrefabInAssets);
                    continue;
                }
                groupAssetsMap[AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(pg.buildingAsset))] = pg;

            }
            Debug.Log("tick3=" + (System.Environment.TickCount - tick));
            return groupAssetsMap;
        }

        private Transform cleanAndReCreateChild(string childName,Transform inParent,bool recreate,string childTag=null)
        {
            Transform groups = inParent.transform.Find(childName);
            if (groups)
            {
                if (recreate == false) return groups;
                DestroyImmediate(groups.gameObject);
            }
            groups = new GameObject(childName).transform;
            groups.parent = inParent.transform;
            groups.position = Vector3.zero;
            groups.localScale = Vector3.one;
            groups.rotation = Quaternion.identity;
            if(childTag!=null)
            groups.tag = childTag;
            return groups;
        }
    }
}