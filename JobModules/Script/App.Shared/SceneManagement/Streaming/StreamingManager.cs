using System.Collections.Generic;
using App.Shared.SceneTriggerObject;
using ArtPlugins;
using Core.Components;
using Core.Utils;
using Shared.Scripts.SceneManagement;
using Shared.Scripts.ScenesIndoorCull;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;
using Utils.Singleton;

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
        private readonly StreamingData _sceneDescription;
        private readonly ScenesLightmapData.MeshRecords _scenesLightmapRecords;
        private readonly ScenesIndoorCullData.IndoorCullRecords _scenesIndoorCullRecords;

        private readonly Dictionary<string, int> _sceneIndex = new Dictionary<string, int>();

        private readonly WorldCompositionManager _worldComposition;
        private readonly IStreamingGoManager _streamingGo;

        private int _concurrentLimit = ConcurrentLimit;
        private const int AsapLimit = 500;
        private const int ConcurrentLimit = 5;
        private int _concurrentCount;

        private int _destroyingCount;
        private const int DestroyLimit = 20;

        private readonly Queue<LoadingGo> _goRequestQueue = new Queue<LoadingGo>();
        private readonly Queue<AssetInfo> _sceneRequestQueue = new Queue<AssetInfo>();
        private readonly Dictionary<AssetInfo, Queue<LoadingGo>> _loadingGoes = new Dictionary<AssetInfo, Queue<LoadingGo>>();

        private readonly Dictionary<string, Queue<UnityObject>> _toBeDestroyedGo = new Dictionary<string, Queue<UnityObject>>();
        private readonly Dictionary<string, Queue<UnityObject>> _unloadingScene = new Dictionary<string, Queue<UnityObject>>();
        private TriggerObjectManager _triggerObjectManager;

        public StreamingManager(ISceneResourceRequestHandler requestHandler,
                                IStreamingGoManager streamingGo,
                                StreamingData sceneDescription,
                                ScenesLightmapData.MeshRecords scenesLightmapRecords,
                                ScenesIndoorCullData.IndoorCullRecords scenesIndoorCullRecords,
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

            _requestHandler.SceneLoaded += SceneLoaded;
            _requestHandler.SceneUnloaded += SceneUnloaded;
            _requestHandler.GoLoaded += GoLoaded;
            _requestHandler.GoUnloaded += GoUnloaded;
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

        #endregion

        private bool EnoughRoom()
        {
            return _concurrentCount < _concurrentLimit;
        }

        private void RequestForLoad()
        {
            while (EnoughRoom() && _sceneRequestQueue.Count > 0)
            {
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

                    var multiTag = go.GetComponent<MultiTagBase>();
                    if (multiTag != null)
                    {
                        multiTag.renderId = data.RenderId;
                    }

                    if (!loadedGoes.ContainsKey(data.Id)) loadedGoes.Add(data.Id, go);

                    if (!SharedConfig.IsServer)
                    {
                        // 烘焙贴图数据的复原
                        var mrs = go.GetComponentsInChildren<MeshRenderer>();
                        foreach (MeshRenderer mr in mrs)
                        {
                            if (mr != null)
                            {
                                Transform tr = mr.transform;
                                string pos = tr.position.WorldPosition().ToString("F4");
                                string identify = string.Format("{0}|{1}", tr.name, pos);
                                ScenesLightmapData.MeshRecord record;
                                _scenesLightmapRecords.recordsDict.TryGetValue(identify, out record);
                                if (record != null)
                                {
                                    mr.lightmapScaleOffset = record.lightMapScaleOffset;

                                    // request lightmap to apply
                                    LevelManager levelManager = _requestHandler as LevelManager;
                                    if (levelManager != null)
                                    {
                                        List<AssetInfo> infos = new List<AssetInfo> { new AssetInfo(record.bundleName, record.colorMapAssetName) };
                                        if (!string.IsNullOrEmpty(record.dirMapAssetName)) infos.Add(new AssetInfo(record.bundleName, record.dirMapAssetName));
                                        levelManager.assetManager.LoadAssetsAsync(mr, infos, LightmapLoaded);
                                    }
                                }
                            }
                        }

                        // 室内灯光剔除组件的应用
                        ScenesIndoorCullData.IndoorCullRecord indoorCullRecord;
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
                            cull.SetGetLightsFunc(GetLights);
                            cull.SetGetRefProbesFunc(GetReflectionProbes);
                        }
                    }
                    RequestForLoad();
                }
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

        private void LightmapLoaded(MeshRenderer mr, IEnumerable<UnityObject> uObjs)
        {
            if (mr != null)
            {
                int count = 0;
                var iterator = uObjs.GetEnumerator();
                Texture2D colorMap = null, dirMap = null;
                while (iterator.MoveNext())
                {
                    Texture2D tex = iterator.Current.As<Texture2D>();
                    if (count == 0) colorMap = tex;
                    else if (count == 1) dirMap = tex;
                    ++count;
                }
                int index = GetLightmapIndex(colorMap, dirMap);
                if (index != -1)
                {
                    mr.lightmapIndex = index;

                    var refs = mr.transform.parent.GetComponentsInChildren<global::Shared.Scripts.ScenesLightmaps.LightmapRefLod>();
                    for (int i = 0; i < refs.Length; i++)
                    {
                        var @ref = refs[i];
                        if (@ref != null)
                        {
                            @ref.Apply();
                        }
                    }
                }
            }
        }

        private int GetLightmapIndex(Texture2D texColor, Texture2D texDir)
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
                data.lightmapDir = texDir;
                lightmaps.Add(data);
                LightmapSettings.lightmaps = lightmaps.ToArray();
                index = lightmaps.Count - 1;
            }

            return index;
        }

        private List<Light> GetLights(int bakeId, List<int> indexes)
        {
            if (indexes == null || indexes.Count <= 0) return null;

            GameObject go = null;
            if (!loadedGoes.TryGetValue(bakeId, out go) || go == null) return null;

            List<Light> lights = new List<Light>();
            for (int i = 0; i < indexes.Count; i++)
            {
                if (indexes[i] < go.transform.childCount)
                {
                    Light light = go.transform.GetChild(indexes[i]).GetComponent<Light>();
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