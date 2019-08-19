using System.Collections.Generic;
using System.Diagnostics;
using App.Shared.Configuration;
using App.Shared.DebugHandle;
using App.Shared.SceneTriggerObject;
using ArtPlugins;
using Core.Components;
using Core.Utils;
using Shared.Scripts.SceneManagement;
using Shared.Scripts.ScenesIndoorCull;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using Utils.AssetManager;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.SceneManagement.Streaming
{
    class StreamingManager : ISceneResourceManager, IStreamingResourceHandler
    {
        struct LoadingGo
        {
            public AssetInfo Addr;
            public int SceneIndex;
            public int GoIndex;
        }

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(StreamingManager));

        private readonly Dictionary<int, GameObject> loadedGoes = new Dictionary<int, GameObject>();

        private readonly ISceneResourceRequestHandler _requestHandler;
        /// <summary>
        /// 流数据，包含 List<StreamingScene> Scenes
        /// </summary>
        private readonly StreamingData _sceneDescription;
        private readonly MeshRecords _scenesLightmapRecords;
        private readonly IndoorCullRecords _scenesIndoorCullRecords;

        private readonly Dictionary<string, int> _sceneIndex = new Dictionary<string, int>();

        private readonly WorldCompositionManager _worldComposition;
        private readonly IStreamingGoManager _streamingGo;

        private int _concurrentLimit = ConcurrentLimit;
        private const int AsapLimit = 500;
        //TESTCODE
        //private const int ConcurrentLimit = int.MaxValue;
        private const int ConcurrentLimit = 5;
        private int _concurrentCount;

        private int _destroyingCount;
        private const int DestroyLimit = 20;

        private readonly Queue<LoadingGo> _goRequestQueue = new Queue<LoadingGo>();
        private readonly Queue<AssetInfo> _sceneRequestQueue = new Queue<AssetInfo>();
        private readonly Queue<IEnumerable<AssetInfoEx<MeshRenderer>>> _lightmapsRequestQueue = new Queue<IEnumerable<AssetInfoEx<MeshRenderer>>>();

        private readonly Dictionary<AssetInfo, Queue<LoadingGo>> _loadingGoes = new Dictionary<AssetInfo, Queue<LoadingGo>>();
        private readonly Dictionary<string, Queue<UnityObject>> _toBeDestroyedGo = new Dictionary<string, Queue<UnityObject>>();
        private readonly Dictionary<string, Queue<UnityObject>> _unloadingScene = new Dictionary<string, Queue<UnityObject>>();
        private TriggerObjectManager _triggerObjectManager;

        public StreamingManager(ISceneResourceRequestHandler requestHandler,
                                IStreamingGoManager streamingGo,
                                StreamingData sceneDescription,
                                MeshRecords scenesLightmapRecords,
                                IndoorCullRecords scenesIndoorCullRecords,
                                WorldCompositionParam param,
                                int preloadSceneCount)
        {
            _requestHandler = requestHandler;
            _sceneDescription = sceneDescription;
            _scenesLightmapRecords = scenesLightmapRecords;
            _scenesIndoorCullRecords = scenesIndoorCullRecords;
            _concurrentCount = preloadSceneCount;

            _worldComposition = new WorldCompositionManager(this, param);
            _streamingGo = streamingGo ?? new StreamingGoByScene();

            _streamingGo.SetResourceHandler(this);
            _triggerObjectManager = SingletonManager.Get<TriggerObjectManager>();
            if (_sceneDescription != null)
            {
                var count = _sceneDescription.Scenes.Count;
                for (int i = 0; i < count; i++)
                    _sceneIndex.Add(_sceneDescription.Scenes[i].SceneName, i);
            }
            
            //TESTCODE

            
            _requestHandler.SceneLoaded += SceneLoaded;
            _requestHandler.SceneUnloaded += SceneUnloaded;
            _requestHandler.GoLoaded += GoLoaded;
            _requestHandler.GoUnloaded += GoUnloaded;
            _requestHandler.LightmapLoaded += LightmapsLoaded;
            _requestHandler.LightmapUnloaded += LightmapsUnloaded;
            
        }

        #region ISceneResourceManager

        public void UpdateOrigin(Vector3 value)
        {
            _worldComposition.UpdateOrigin(value);
            _streamingGo.UpdateOrigin(value);
        }

        public void SetAsapMode(bool value)
        {
            _concurrentLimit = value ? AsapLimit : ConcurrentLimit;
            _streamingGo.SetAsapMode(value);
        }

        #endregion

        #region IStreamingResourceHandler

        /// <summary>
        /// 将 AssetInfo 加入到 _sceneRequestQueue 加载场景请求队列中
        /// </summary>
        /// <param name="addr"></param>
        public void LoadScene(AssetInfo addr)
        {
            _sceneRequestQueue.Enqueue(addr);
            RequestForLoad();
        }

        public void UnloadScene(string sceneName)
        {
            if (_toBeDestroyedGo.ContainsKey(sceneName))
            {
                _unloadingScene.Add(sceneName, _toBeDestroyedGo[sceneName]);
                _toBeDestroyedGo.Remove(sceneName);
            }
            else
                _unloadingScene.Add(sceneName, new Queue<UnityObject>());

            _streamingGo.SceneUnloaded(sceneName);
            RequestForUnload();
        }

        public void LoadGo(int sceneIndex, int goIndex)
        {
            var streamingObject = _sceneDescription.Scenes[sceneIndex].Objects[goIndex];
            _goRequestQueue.Enqueue(new LoadingGo
            {
                Addr = new AssetInfo
                {
                    BundleName = streamingObject.BundleName,
                    AssetName = streamingObject.AssetName
                },
                SceneIndex = sceneIndex,
                GoIndex = goIndex
            });

            RequestForLoad();
        }

        public void UnloadGo(UnityObject go, int sceneIndex)
        {
            var sceneName = _sceneDescription.Scenes[sceneIndex].SceneName;
            if (_unloadingScene.ContainsKey(sceneName))
                _unloadingScene[sceneName].Enqueue(go);
            else
            {
                if (!_toBeDestroyedGo.ContainsKey(sceneName))
                    _toBeDestroyedGo.Add(sceneName, new Queue<UnityObject>());

                _toBeDestroyedGo[sceneName].Enqueue(go);
                RequestForUnload();
            }
        }

        public void LoadLightmaps(IEnumerable<AssetInfoEx<MeshRenderer>> infos)
        {
            _lightmapsRequestQueue.Enqueue(infos);
            RequestForLoad();
        }

        public void UnloadLightmaps(IEnumerable<UnityObject> uObjs)
        {
            throw new System.NotImplementedException("not implement UnloadLightmaps");
        }
        #endregion

        private bool EnoughRoom()
        {
            return _concurrentCount < _concurrentLimit;
        }

        private void RequestForLoad()
        {
            while (EnoughRoom() && _sceneRequestQueue.Count > 0)
            {
                // 即 LevelManager.AddLoadSceneRequest
                _requestHandler.AddLoadSceneRequest(_sceneRequestQueue.Dequeue());
                ++_concurrentCount;
            }

            while (EnoughRoom() && _goRequestQueue.Count > 0)
            {
                var loadingGo = _goRequestQueue.Dequeue();
                _requestHandler.AddLoadGoRequest(loadingGo.Addr);

                ++_concurrentCount;

                if (!_loadingGoes.ContainsKey(loadingGo.Addr))
                    _loadingGoes.Add(loadingGo.Addr, new Queue<LoadingGo>(16));

                _loadingGoes[loadingGo.Addr].Enqueue(loadingGo);
            }

            while (EnoughRoom() && _lightmapsRequestQueue.Count > 0)
            {
                _requestHandler.AddLoadLightmapsRequest(_lightmapsRequestQueue.Dequeue());
                ++_concurrentCount;
            }
        }

        private void RequestForUnload()
        {
            foreach (var pair in _toBeDestroyedGo)
            {
                while (_destroyingCount < DestroyLimit && pair.Value.Count > 0)
                {
                    _requestHandler.AddUnloadGoRequest(pair.Value.Dequeue());
                    ++_destroyingCount;
                }
            }

            string emptyScene = null;

            foreach (var pair in _unloadingScene)
            {
                while (_destroyingCount < DestroyLimit && pair.Value.Count > 0)
                {
                    _requestHandler.AddUnloadGoRequest(pair.Value.Dequeue());
                    ++_destroyingCount;
                }

                if (pair.Value.Count <= 0)
                {
                    emptyScene = pair.Key;
                    break;
                }
            }

            if (emptyScene != null)
            {
                _unloadingScene.Remove(emptyScene);
                _requestHandler.AddUnloadSceneRequest(emptyScene);
            }
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var sceneIndex = -1;
            if (_sceneIndex.ContainsKey(scene.name))
                sceneIndex = _sceneIndex[scene.name];

            --_concurrentCount;
            RequestForLoad();

            if (sceneIndex != -1)
            {
                _worldComposition.SceneLoaded(scene);
                _streamingGo.SceneLoaded(scene.name,
                    sceneIndex,
                    scene,
                    _sceneDescription.Scenes[sceneIndex],
                    _worldComposition.GetDimensionOfScene(scene.name));
            }
        }

        private void SceneUnloaded(Scene scene)
        {
            _worldComposition.SceneUnloaded(scene);
        }

        private void GoLoaded(UnityObject unityObj)
        {
            --_concurrentCount;
            if (_loadingGoes.ContainsKey(unityObj.Address))
            {
                var loadingGo = _loadingGoes[unityObj.Address].Dequeue();
                var sceneName = _sceneDescription.Scenes[loadingGo.SceneIndex].SceneName;

                if (_unloadingScene.ContainsKey(sceneName))
                {
                    _unloadingScene[sceneName].Enqueue(unityObj);
                    RequestForUnload();
                }
                else
                {
                    _streamingGo.GoLoaded(loadingGo.SceneIndex, loadingGo.GoIndex, unityObj);
                    var data = _sceneDescription.Scenes[loadingGo.SceneIndex].Objects[loadingGo.GoIndex];
                    var go = unityObj.AsGameObject;
                    go.transform.localPosition = data.Position.ShiftedPosition();
                    go.transform.localEulerAngles = data.Rotation;
                    go.transform.localScale = data.Scale;
                    unityObj.SceneObjAttr.Id = data.Id;

                    Transform prefabTf = null;
                    if (go.transform.childCount > 0)
                    {
                        prefabTf = go.transform.GetChild(0);

                        // 还原各个 AbstractSaveMono
                        string errorStr = data.LoadCompDatas(prefabTf);
                        if (!string.IsNullOrEmpty(errorStr))
                        {
                            _logger.Error(errorStr);
                        }
                    }

                    if (!loadedGoes.ContainsKey(data.Id)) loadedGoes.Add(data.Id, go);

                    if (!SharedConfig.IsServer)
                    {
                        // 烘焙贴图数据的复原
                        TryApplyLightmaps(data.Id);

                        // 室内灯光剔除组件的应用
                        IndoorCullRecord indoorCullRecord;
                        _scenesIndoorCullRecords.recordsDict.TryGetValue(data.Id, out indoorCullRecord);
                        if (indoorCullRecord != null)
                        {
                            HouseLightsCull cull = go.GetComponent<HouseLightsCull>();
                            if (cull == null) cull = go.gameObject.AddComponent<HouseLightsCull>();
                            cull.Clear();
                            cull.id = indoorCullRecord.id;
                            cull.bakeId = indoorCullRecord.bakeId;
                            cull.bound = indoorCullRecord.bounds;
                            cull.lightIndexes = indoorCullRecord.lights;
                            cull.refProbeIndexes = indoorCullRecord.refProbes;
                            cull.SetGetLightsFunc(GetLightsFun);
                            cull.SetGetRefProbesFunc(GetReflectionProbesFun);
                        }

                    }
                    if (hideImportObject)
                    {
                        checkObjectVisible(go, data);
                    }
                    RequestForLoad();
                }
            }
        }

        bool hideImportObject = true;

        Dictionary<string, int> qualitDic = new Dictionary<string, int>();
        void checkObjectVisible(GameObject obj, StreamingObject data)
        {
            string qualityName = GameSettingUtility.GetQualityName();
            var allMaps = SingletonManager.Get<MapsDescription>();
            if (allMaps.CurrentLevelType != LevelType.BigMap)
            {
                return;
            }
            MultiTagBase mul = obj.GetComponentInChildren<MultiTagBase>();
            if (mul == null)
            {
                return;
            }

            if (mul.btags[(int)MultiTagBase.TagEnum.House])
            {
                return;
            }

            mul.importantLevel = data.importantLevel;
            SceneConfig config = allMaps.BigMapParameters;
            if(config.PreMapName != "004")
            {
                return;
            }


            int level = 0;
            //TEST Code
            /*
            int level = 0;
            if(qualitDic.Count <= 0)
            {
                qualitDic.Add("QL_Low", 0);
                qualitDic.Add("QL_MediumLow", 1);
                qualitDic.Add("QL_Medium", 2);
                qualitDic.Add("QL_MediumHigh", 3);
                qualitDic.Add("QL_High", 4);
            }

            if(!qualitDic.ContainsKey(qualityName))
            {
                return;
            }

            level = qualitDic[qualityName];
            */
            if(mul.importantLevel <= level)
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false);
            }
        }


        private void GoUnloaded(UnityObject unityObj)
        {
            --_destroyingCount;

            int id = unityObj.SceneObjAttr.Id;
            if (loadedGoes.ContainsKey(id)) loadedGoes.Remove(id);

            RequestForUnload();
            _triggerObjectManager.OnMapObjUnloaded(unityObj);
        }

        private void LightmapsLoaded(MeshRenderer mr, IEnumerable<UnityObject> uObjs)
        {
            --_concurrentCount;
            RequestForLoad();

            if (mr == null) return;

            int count = 0;
            var iterator = uObjs.GetEnumerator();
            Texture2D colorMap = null, shadowMap = null, dirMap = null;
            while (iterator.MoveNext())
            {
                Texture2D tex = iterator.Current.As<Texture2D>();
                if (count == 0)
                    colorMap = tex;
                else if (count == 1)
                    shadowMap = tex;
                else if (count == 2)
                    dirMap = tex;
                
                ++count;
            }
            int index = GetLightmapIndex(colorMap, shadowMap, dirMap);
            if (index == -1) return;

            mr.lightmapIndex = index;

            // lod烘焙的继承
            var refs = mr.transform.parent.GetComponentsInChildren<global::Shared.Scripts.ScenesLightmaps.LightmapRefLod>();
            for (int i = 0; i < refs.Length; i++)
            {
                var @ref = refs[i];
                if (@ref != null) @ref.Apply();
            }
        }

        private void LightmapsUnloaded(IEnumerable<UnityObject> uObjs)
        {
            throw new System.NotImplementedException("not implement LightmapsUnloaded");
        }

        private void TryApplyLightmaps(int id)
        {
            var meshRecords = _scenesLightmapRecords.GetMeshRecords(id);
            if (meshRecords == null) return;

            GameObject go = null;
            if (!loadedGoes.TryGetValue(id, out go) || go == null) return;

            int num = meshRecords.Count;
            for (int i = 0; i < num; i++)
            {
                var record = meshRecords[i];

                if (go.transform.childCount > 0)
                {
                    Transform prefabTF = go.transform.GetChild(0);
                    if (record != null && record.siblingIndex < prefabTF.childCount)
                    {
                        Transform child = prefabTF.GetChild(record.siblingIndex);

                        MeshRenderer mr = child.GetComponent<MeshRenderer>();
                        //_logger.ErrorFormat(child.name + "#####"+(mr!=null? "有MeshRenderer": "没有MeshRenderer"));
                        if (mr == null) continue;

                        mr.lightmapScaleOffset = record.lightMapScaleOffset;

                        // request load lightmaps to apply 
                        List<AssetInfoEx<MeshRenderer>> infos = new List<AssetInfoEx<MeshRenderer>>();
                        if (!string.IsNullOrEmpty(record.colorMapAssetName))
                        {
                            AssetInfoEx<MeshRenderer> info = new AssetInfoEx<MeshRenderer>();
                            info.asset = new AssetInfo(record.bundleName, record.colorMapAssetName);
                            info.data = mr;
                            infos.Add(info);
                        }

                        if (!string.IsNullOrEmpty(record.shadowMapAssetName))
                        {
                            AssetInfoEx<MeshRenderer> info = new AssetInfoEx<MeshRenderer>();
                            info.asset = new AssetInfo(record.bundleName, record.shadowMapAssetName);
                            info.data = mr;
                            infos.Add(info);
                        }

                        if (!string.IsNullOrEmpty(record.dirMapAssetName))
                        {
                            AssetInfoEx<MeshRenderer> info = new AssetInfoEx<MeshRenderer>();
                            info.asset = new AssetInfo(record.bundleName, record.dirMapAssetName);
                            info.data = mr;
                            infos.Add(info);
                        }
                       

                        LoadLightmaps(infos);
                    }
                }

            }
        }

        private int GetLightmapIndex(Texture2D texColor, Texture2D shadowMap, Texture2D texDir)
        {
            if (texColor == null)
            {
                _logger.Error("StreamingManager GetLightmapIndex error, tex is null");
                return -1;
            }

            int index = -1;
            for (int i = 0; i < LightmapSettings.lightmaps.Length; i++)
            {
                var lightmap = LightmapSettings.lightmaps[i];
                if (lightmap != null && ReferenceEquals(texColor, lightmap.lightmapColor))
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                List<LightmapData> lightmaps = new List<LightmapData>();
                lightmaps.AddRange(LightmapSettings.lightmaps);
                LightmapData data = new LightmapData();
                data.lightmapColor = texColor;
                data.shadowMask = shadowMap;
                data.lightmapDir = texDir;
                lightmaps.Add(data);
                LightmapSettings.lightmaps = lightmaps.ToArray();
                index = lightmaps.Count - 1;
            }

            return index;
        }

        #region 防止 delegate 产生GC
   
        private System.Func<int, List<int>, List<Light>> GetLightsFun
        {
            get {
                if (m_GetLightsFun == null)
                {
                    m_GetLightsFun = new System.Func<int, List<int>, List<Light>>(GetLights);
                }
                return m_GetLightsFun;
            }
        }
        private System.Func<int, List<int>, List<Light>> m_GetLightsFun;

        private System.Func<int, List<int>, List<ReflectionProbe>> GetReflectionProbesFun
        {
            get
            {
                if (m_GetReflectionProbesFun == null)
                {
                    m_GetReflectionProbesFun = new System.Func<int, List<int>, List<ReflectionProbe>>(GetReflectionProbes);
                }
                return m_GetReflectionProbesFun;
            }
        }
        private System.Func<int, List<int>, List<ReflectionProbe>> m_GetReflectionProbesFun;

        #endregion

        private List<Light> GetLights(int bakeId, List<int> indexes)
        {
            if (indexes == null || indexes.Count <= 0) return null;

            GameObject go = null;
            if (!loadedGoes.TryGetValue(bakeId, out go) || go == null) return null;

            if (go.transform.childCount < 1)
            {
                return null;
            }

            Transform prefabTF = go.transform.GetChild(0);

            List<Light> lights = new List<Light>();
            for (int i = 0; i < indexes.Count; i++)
            {
                if (indexes[i] < prefabTF.childCount)
                {
                    Light light = prefabTF.GetChild(indexes[i]).GetComponent<Light>();
                    if (light != null) lights.Add(light);
                }
            }
            return lights;
        }

        private List<ReflectionProbe> GetReflectionProbes(int bakeId, List<int> indexes)
        {
            if (indexes == null || indexes.Count <= 0) return null;

            GameObject go = null;
            if (!loadedGoes.TryGetValue(bakeId, out go) || go == null) return null;

            List<ReflectionProbe> probes = new List<ReflectionProbe>();
            for (int i = 0; i < indexes.Count; i++)
            {
                if (indexes[i] < go.transform.childCount)
                {
                    ReflectionProbe probe = go.transform.GetChild(indexes[i]).GetComponent<ReflectionProbe>();
                    if (probe != null) probes.Add(probe);
                }
            }
            return probes;
        }
    }
}