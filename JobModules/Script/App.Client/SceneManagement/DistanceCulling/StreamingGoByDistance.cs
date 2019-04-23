using System.Collections.Generic;
using System.Linq;
using App.Client.SceneManagement.DistanceCulling.Factory;
using App.Shared.SceneManagement.Streaming;
using Core.SceneManagement;
using Core.Utils;
using Shared.Scripts.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;

namespace App.Client.SceneManagement.DistanceCulling
{
    public class StreamingGoByDistance : IStreamingGoManager
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(StreamingGoByDistance));
        
        private readonly Dictionary<string, int> _sceneNameToIndex = new Dictionary<string, int>();
        private readonly Dictionary<int, GameObject> _goRoot = new Dictionary<int, GameObject>();
        private readonly Dictionary<int, StreamingScene> _sceneDesc = new Dictionary<int, StreamingScene>();
        private readonly Dictionary<int, OctreeNode> _sceneOctrees = new Dictionary<int, OctreeNode>();
        private List<OctreeNode> _sceneOctreesCache;

        private IStreamingResourceHandler _resourceHandler;
        private readonly StreamingGoBuilder _goBuilder;
        private readonly CullingItemFactory _cullingItemFactory = new CullingItemFactory();

        private bool _asap;
        private Vector3 _asapPosition;

        #region IStreamingGoManager
    
        public StreamingGoByDistance()
        {
            _goBuilder = new StreamingGoBuilder(NewOctreeForScene);
        }

        public void SetResourceHandler(IStreamingResourceHandler handler)
        {
            _resourceHandler = handler;
        }

        public void UpdateOrigin(Vector3 pos)
        {
            if (_asap)
                _asapPosition = pos;

            _goBuilder.Update();

            DebugPreparation();

            if (_sceneOctreesCache == null)
                return;

            var count = _sceneOctreesCache.Count;
            for (int i = 0; i < count; i++)
            {
                _sceneOctreesCache[i].OnCameraMovement(pos, 1, _resourceHandler);
            }
            
            Debug();
        }

        public void SetAsapMode(bool value)
        {
            _asap = value;
        }

        public void SceneLoaded(string sceneName, int sceneIndex, Scene scene, StreamingScene sceneStruct, Vector4 sceneDimension)
        {
            if (sceneIndex < 0)
                return;

            _sceneNameToIndex.Add(sceneName, sceneIndex);
            _sceneDesc.Add(sceneIndex, sceneStruct);
                
//            var go = new GameObject("StreamingRoot");
//            SceneManager.MoveGameObjectToScene(go, scene);
//            _goRoot.Add(sceneIndex, go);
            _goRoot.Add(sceneIndex, DefaultGo.StreamingRoot);
                
            if (_asap)
            {
                var root = _goBuilder.GetOctreeForScene(
                    new Vector3(sceneDimension.x, sceneDimension.y, sceneDimension.z),
                    sceneDimension.w, sceneStruct);
                root.OnCameraMovement(_asapPosition, 1, _resourceHandler);

                NewOctreeForScene(root, sceneName);
            }
            else
                _goBuilder.CreateOctreeForScene(new Vector3(sceneDimension.x, sceneDimension.y, sceneDimension.z),
                    sceneDimension.w, sceneStruct);
        }

        public void SceneUnloaded(string sceneName)
        {
            if (_sceneNameToIndex.ContainsKey(sceneName))
            {
                var sceneIndex = _sceneNameToIndex[sceneName];
                _sceneNameToIndex.Remove(sceneName);
                _sceneDesc.Remove(sceneIndex);
                _goRoot.Remove(sceneIndex);

                if (_sceneOctrees.ContainsKey(sceneIndex))
                {
                    _sceneOctrees[sceneIndex].Delete(_resourceHandler);
                    _sceneOctrees[sceneIndex].Free();

                    _sceneOctrees.Remove(sceneIndex);
                    _sceneOctreesCache = _sceneOctrees.Values.ToList();
                }
            }
        }

        public void GoLoaded(int sceneIndex, int goIndex, UnityObject unityObj)
        {
            if (_sceneOctrees.ContainsKey(sceneIndex))
            {
                var sceneDesc = _sceneDesc[sceneIndex];
                var goDesc = sceneDesc.Objects[goIndex];

                var handlerHead = _cullingItemFactory.CreateCullingHandlers(unityObj, goDesc.SceneTag);
                
                _sceneOctrees[sceneIndex].AssignGo(unityObj, handlerHead,
                    new StreamingGoKey
                    {
                        SceneIndex = sceneIndex,
                        GoIndex = goIndex
                    },
                    goDesc.Position, goDesc.Size, _resourceHandler);
                
                unityObj.AsGameObject.transform.SetParent(_goRoot[sceneIndex].transform);
            }
            else
                _resourceHandler.UnloadGo(unityObj, sceneIndex);
        }

        #endregion

        private void NewOctreeForScene(OctreeNode root, string sceneName)
        {
            if (_sceneNameToIndex.ContainsKey(sceneName))
            {
                _sceneOctrees.Add(_sceneNameToIndex[sceneName], root);
                _sceneOctreesCache = _sceneOctrees.Values.ToList();
            }
        }
        
        #region Debug
        
        public static bool Log;
        public static int[,] Total = new int[8,3];
        public static int[,] Show = new int[8,3];

        private void DebugPreparation()
        {
            if (Log)
            {
                var countOne = Total.GetLength(0);
                var countTwo = Total.GetLength(1);
                
                for (int i = 0; i < countOne; i++)
                {
                    for (int j = 0; j < countTwo; j++)
                    {
                        Total[i, j] = 0;
                        Show[i, j] = 0;
                    }
                }
            }
        }

        private void Debug()
        {
            if (Log)
            {
                foreach (var root in _sceneOctrees)
                    root.Value.Log();
                
                for (int i = 0; i < Total.GetLength(0); i++)
                {
                    _logger.InfoFormat("Depth: {0}, near: {1}/{2} mid: {3}/{4} far: {5}/{6}",
                        i, Show[i, 0], Total[i, 0], Show[i, 1], Total[i, 1], Show[i, 2], Total[i, 2]);
                }
            }

            Log = false;
        }
        
        public static void SetCullingDistance(string key, float value)
        {
            switch (key)
            {
                case "near":
                    Constants.SetCullingDistance(DistCullingCat.Near, value);
                    break;
                case "median":
                    Constants.SetCullingDistance(DistCullingCat.Median, value);
                    break;
                case "far":
                    Constants.SetCullingDistance(DistCullingCat.Far, value);
                    break;
                case "all":
                    Constants.SetCullingDistance(DistCullingCat.Near, value);
                    Constants.SetCullingDistance(DistCullingCat.Median, value);
                    Constants.SetCullingDistance(DistCullingCat.Far, value);
                    break;
            }
        }
        
        #endregion
    }
}