using System;
using System.Collections.Generic;
using App.Shared;
using App.Shared.Configuration;
using App.Shared.Terrains;
using Core.Compare;
using Core.Components;
using Core.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;
using Utils.Singleton;

namespace App.Client.GameModules.SceneManagement
{
    public class WorldShiftManager : DisposableSingleton<WorldShiftManager>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(WorldShiftManager));

        private readonly Dictionary<int, Transform> _worldShiftTransforms = new Dictionary<int, Transform>(100);
        private readonly Dictionary<string, List<int>> _sceneToTransforms = new Dictionary<string, List<int>>();
        private List<int> _toRemove = new List<int>();
        private CustomProfileInfo _profile;

        public WorldShiftManager()
        {
            _profile = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("WorldShiftManager");
        }

        public void SetOrgin(Vector3 orgin)
        {
            _toRemove.Clear();
            orgin = new Vector3((int) orgin.x, (int) orgin.y, (int) orgin.z);
          
            if (!CompareUtility.IsApproximatelyEqual(WorldOrigin.Origin, orgin, 1f))
            {
               
                var shiftSceneObject = 0f;
                var shiftDefaultGoChilds = 0f;
                var shiftStreamingChilds = 0f;
                var diff = WorldOrigin.Origin - orgin;
                WorldOrigin.Origin = orgin;
                try
                {
                    _profile.BeginProfile();
                    shiftSceneObject = ShiftSceneObject(diff);

                    shiftDefaultGoChilds = ShiftDefaultGoChilds(diff);

                    shiftStreamingChilds = ShiftStreamingChilds(diff);
//                    TerrainCommonData.leftMinPos =
//                        SingletonManager.Get<MapConfigManager>().SceneParameters.OriginPosition - orgin;
                    
                }
                finally
                {
                   var t= _profile.EndProfile();
                    _logger.InfoFormat("Move World From {0} to {1} with scene:{2} default:{3} stream:{4} in {5}ms",
                        WorldOrigin.Origin + diff, WorldOrigin.Origin, shiftSceneObject, shiftDefaultGoChilds,
                        shiftStreamingChilds, t);
                }
            }
        }

        private static int ShiftStreamingChilds2(Vector3 diff)
        {
            DefaultGo.StreamingRoot.transform.localPosition = -WorldOrigin.Origin;
            return 1;
        }
        private static int ShiftStreamingChilds(Vector3 diff)
        {
            int ret = 0;
            int count = DefaultGo.StreamingRoot.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    var child = DefaultGo.StreamingRoot.transform.GetChild(i);
                    if (child.gameObject.activeSelf)
                    {
                        var localPosition = child.localPosition;
                        child.localPosition = localPosition + diff;
                        ret++;
                        if (_logger.IsDebugEnabled)
                            _logger.DebugFormat("move stream {0} {3} from {1} to {1}", child.name,
                                child.GetInstanceID(), localPosition,
                                child.localPosition);
                    }
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("ShiftStreamingChilds :{0}{1} ", i, e);
                }
            }

            return ret;
        }

        private static int ShiftDefaultGoChilds(Vector3 diff)
        {
            int ret = 0;
            int count = DefaultGo.DefaultParent.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    var child = DefaultGo.DefaultParent.transform.GetChild(i);
                    if (child.gameObject.activeSelf)
                    {
                        var localPosition = child.localPosition;
                        child.localPosition = localPosition + diff;
                        ret++;
                        if (_logger.IsDebugEnabled)
                            _logger.DebugFormat("move defaultgo {0} {3} from {1} to {1}", child.name,
                                child.GetInstanceID(), localPosition,
                                child.localPosition);
                    }
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("ShiftDefaultGoChilds :{0}{1} ", i, e);
                }
            }

            return ret;
        }

        private int ShiftSceneObject(Vector3 diff)
        {
            int ret = 0;
            foreach (var kv in _worldShiftTransforms)
            {
                var transform = kv.Value;
                try
                {
                    if (transform.gameObject.activeSelf)
                    {
                        var localPosition = transform.localPosition;
                        transform.localPosition = localPosition + diff;
                        ret++;
                        if (_logger.IsDebugEnabled)
                            _logger.DebugFormat("move scene {0} {3} from {1} to {1}", transform.name,
                                transform.GetInstanceID(), localPosition,
                                transform.localPosition);
                        if(SharedConfig.WorldShiftDisableTerrainDetail)
                            DisableTerrainDetail(transform);
                       
                        Camera.worldShiftOrigin = -WorldOrigin.Origin;

                    }
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("WorldShiftTransforms :{0}{1} ", kv.Key, e);
                    _toRemove.Add(kv.Key);
                }
            }

            foreach (int i in _toRemove)
            {
                _worldShiftTransforms.Remove(i);
            }

            return ret;
        }

        private static void DisableTerrainDetail(Transform transform)
        {
            var terrain = transform.GetComponent<UnityEngine.Terrain>();
            if (terrain != null)
            {
                terrain.detailObjectDistance = SharedConfig.WorldShiftTerrainDetailDistance;
                
            }
        }  
        
        private static void SetNewTerrainColliderPhysicMaterial(Transform transform)
        {
            var terrain = transform.GetComponent<UnityEngine.TerrainCollider>();
            if (terrain != null)
            {
                terrain.material=new PhysicMaterial();
                
            }
        }


        public void SceneUnloaded(Scene scene)
        {
            var name = scene.name;
            if (_sceneToTransforms.ContainsKey(name))
            {
                foreach (var i in _sceneToTransforms[name])
                {
                    _worldShiftTransforms.Remove(i);
                }

                _sceneToTransforms[name].Clear();
            }
        }

        public void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var name = scene.name;
            if (_sceneToTransforms.ContainsKey(name))
            {
                foreach (var i in _sceneToTransforms[name])
                {
                    _worldShiftTransforms.Remove(i);
                }

                _sceneToTransforms[name].Clear();
            }
            else
            {
                _sceneToTransforms[name] = new List<int>();
            }

            foreach (var go in scene.GetRootGameObjects())
            {
                var t = go.transform;
                var id = go.GetInstanceID();
                if (!_worldShiftTransforms.ContainsKey(id))
                {
                    _worldShiftTransforms[id] = t;
                    _sceneToTransforms[name].Add(id);
                    var localPosition = t.localPosition;
                    t.localPosition = localPosition - WorldOrigin.Origin;
                    if (_logger.IsDebugEnabled)
                        _logger.DebugFormat("move scene {0} {3} from {1} to {1}", t.name, t.GetInstanceID(),
                            localPosition, t.localPosition);
                }
                SetNewTerrainColliderPhysicMaterial(t);
                if(SharedConfig.WorldShiftDisableTerrainDetail)
                    DisableTerrainDetail(t);
            }
        }

        protected override void OnDispose()
        {
            _worldShiftTransforms.Clear();
            _toRemove.Clear();
            _sceneToTransforms.Clear();
            WorldOrigin.Origin = Vector3.zero;
        }
    }
}