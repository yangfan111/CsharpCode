using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using App.Shared.SceneManagement;
using App.Shared.Util;
using ArtPlugins;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core;
using Core.GameTime;
using Core.SceneManagement;
using Core.Utils;
using Entitas;
using UltimateFracturing;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using Utils.AssetManager;
using Utils.AssetManager.Converter;
using Utils.Singleton;
using Debug = UnityEngine.Debug;

namespace App.Shared.SceneTriggerObject
{
    public interface IGameObjectListener
    {
        void OnMapObjLoaded(UnityObject unityObj, int id);   //流式
        void OnMapObjUnloaded(UnityObject unityObj);
        void OnMapObjSceneLoaded_BigMap(UnityObject unity);
        void OnMapObjSceneLoaded_SmallMap(UnityObject unity);
    }

    public interface ITriggerObjectListener
    {
        IEntity CreateMapObj(int id);
        void OnTriggerObjectLoaded(int id, GameObject gameObject);
        void OnTriggerObjectUnloaded(int id);
    }

    internal interface ILastTriggerObjectAccessor
    {
        HashSet<int> LastLoadedIdSet { get; }
        HashSet<int> LastUnloadedIdSet { get; }
    }

    public interface ITriggerObjectManager : IGameObjectListener
    {
        GameObject Get(int id);
        HashSet<int> GetAllId();
    }

    internal interface ITriggerObjectInternalManger : ITriggerObjectManager, ILastTriggerObjectAccessor, IDisposable
    {
        void Clear();
    }

    public enum ETriggerObjectType
    {
        Door = 0,
        DestructibleObject,
        GlassyObject,
        MaxCount,
    }

    public class TriggerObjectManager : DisposableSingleton<TriggerObjectManager>, IGameObjectListener
    {
        private ITriggerObjectInternalManger[] _managers = new ITriggerObjectInternalManger[(int)ETriggerObjectType.MaxCount];
        private List<ITriggerObjectListener>[] _gameObjectListener = new List<ITriggerObjectListener>[(int)ETriggerObjectType.MaxCount];
        public TriggerObjectManager()
        {
            if (SharedConfig.CollectTriggerObjectDynamic)
            {
                _managers[(int)ETriggerObjectType.Door] = new TriggerObjectInternalManager<Door>(ETriggerObjectType.Door);
                _managers[(int)ETriggerObjectType.DestructibleObject] = new TriggerObjectInternalManager<FracturedObject>(ETriggerObjectType.DestructibleObject, (go) => go.GetComponent<Door>() == null);
                _managers[(int)ETriggerObjectType.GlassyObject] = new TriggerObjectInternalManager<FracturedGlassyObject>(ETriggerObjectType.GlassyObject);
            }

            for (int i = 0; i < (int)ETriggerObjectType.MaxCount; ++i)
            {
                _gameObjectListener[i] = new List<ITriggerObjectListener>();
            }

        }

        protected override void OnDispose()
        {
            foreach (var manager in _managers)
            {
                manager.Dispose();
            }
        }
        
        public void RegisterListener(ETriggerObjectType type, ITriggerObjectListener listener)
        {
            _gameObjectListener[(int)type].Add(listener);
        }

        public ITriggerObjectManager GetManager(ETriggerObjectType type)
        {
            return _managers[(int)type];
        }

        public GameObject Get(ETriggerObjectType type, int id)
        {
            return _managers[(int)type].Get(id);
        }

        public void OnMapObjLoaded(UnityObject gameObject, int id = Int32.MinValue)
        {
            foreach (var manager in _managers)
            {
                manager.OnMapObjLoaded(gameObject, id);
            }
        }

        public void OnMapObjSceneLoaded_SmallMap(UnityObject gameObject)
        {
            foreach (var manager in _managers)
            {
                manager.OnMapObjSceneLoaded_SmallMap(gameObject);
            }
        }
        
        public void OnMapObjSceneLoaded_BigMap(UnityObject gameObject)
        {
            foreach (var manager in _managers)
            {
                manager.OnMapObjSceneLoaded_BigMap(gameObject);
            }
        }


        public void Clear()
        {
            foreach (var manager in _managers)
            {
                manager.Clear();
            }
        }

        public void OnMapObjUnloaded(UnityObject unityObj)
        {
            foreach (var manager in _managers)
            {
                manager.OnMapObjUnloaded(unityObj);
            }
        }

        public void ProcessLastLoadedObjects()
        {
            for (int i = 0; i < (int)ETriggerObjectType.MaxCount; ++i)
            {
                var manager = _managers[i];
                var listeners = _gameObjectListener[i];
                var idSet = manager.LastLoadedIdSet;

                if (listeners.Count > 0)
                {
                    foreach (var id in idSet)
                    {
                        var go = manager.Get(id);
                        if (go == null)
                        {
                            continue;
                        }

                        foreach (var listener in listeners)
                        {
                            listener.OnTriggerObjectLoaded(id, go);
                        }
                    }
                }

                idSet.Clear();
            }
        }

        public void ProcessLastUnloadedObjects()
        {
            for (int i = 0; i < (int)ETriggerObjectType.MaxCount; ++i)
            {
                var manager = _managers[i];
                var listeners = _gameObjectListener[i];
                var idSet = manager.LastUnloadedIdSet;

                if (listeners.Count > 0)
                {
                    foreach (var id in idSet)
                    {
                        foreach (var listener in listeners)
                        {
                            listener.OnTriggerObjectUnloaded(id);
                        }
                    }
                }

                idSet.Clear();
            }
        }
    }

    internal class TriggerObjectPool
    {
        private Dictionary<int, GameObject> _triggerObjects = new Dictionary<int, GameObject>();

        private HashSet<int> _lastLoadedIdSet = new HashSet<int>();
        private HashSet<int> _lastUnloadedIdSet = new HashSet<int>();

        public GameObject Get(int id)
        {
            if (_triggerObjects.ContainsKey(id))
            {
                return _triggerObjects[id];
            }
            return null;
        }
        

        public void Put(int id, GameObject go)
        {
            _triggerObjects[id] = go;
            CacheNewId(id);
        }

        private void CacheNewId(int id)
        {
            if (_lastUnloadedIdSet.Contains(id))
            {
                _lastUnloadedIdSet.Remove(id);
            }

            _lastLoadedIdSet.Add(id);
        }

        public void Remove(int id)
        {
            if(_triggerObjects.Remove(id))
                CacheRemoveId(id);
        }

        public void Clear()
        {
            var ids = _triggerObjects.Keys;

            foreach (var id in ids)
            {
                CacheRemoveId(id);
            }

            _triggerObjects.Clear();
        }

        private void CacheRemoveId(int id)
        {
            if (_lastLoadedIdSet.Contains(id))
            {
                _lastLoadedIdSet.Remove(id);
            }

            _lastUnloadedIdSet.Add(id);
        }

        public HashSet<int> LastLoadedIdSet { get { return _lastLoadedIdSet; } }
        public HashSet<int> LastUnloadedIdSet { get { return _lastUnloadedIdSet; } }

    }

    internal class TriggerObjectInternalManager<TTriggerScript> : ITriggerObjectInternalManger where TTriggerScript: MonoBehaviour
    {

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(TriggerObjectInternalManager<TTriggerScript>));
        private int _typeValue;
        private TriggerObjectPool _pool = new TriggerObjectPool();

        
        private List<int> _tempIdList = new List<int>();
        private List<GameObject> _tempGameObejctList = new List<GameObject>();
        private Dictionary<int, int> _tempGameObjIdList = new Dictionary<int, int>();
        private Predicate<GameObject> _matcher;
        
        private List<TTriggerScript> _tempTriggerCompList = new List<TTriggerScript>();
        
        public TriggerObjectInternalManager(ETriggerObjectType type, Predicate<GameObject> matcher = null)
        {
            _typeValue = (int)type;
            _matcher = matcher;
        }
        
        private void FillTempGameObjectList(GameObject gameObject, int objId)
        {
            var rootObject = gameObject;
            _tempTriggerCompList.Clear();
            rootObject.GetComponentsInChildren(true, _tempTriggerCompList);
            
            foreach (var comp in _tempTriggerCompList)
            {
                var go = comp.gameObject;

                if (_matcher != null && !_matcher(go))
                {
                    continue;
                }

                _logger.InfoFormat(" loadObj : {0} ,id:{1}", go, objId);
                _tempIdList.Add(objId);
                _tempGameObejctList.Add(go);
                
                int instacneid = gameObject.GetInstanceID();
                if (!_tempGameObjIdList.ContainsKey(instacneid))
                    _tempGameObjIdList.Add(instacneid, objId);
            }

            _tempTriggerCompList.Clear();
        }

        private void FillTempGameObjectListByScene_SmallMap(GameObject gameObject)
        {
            var rootObject = gameObject;
            _tempTriggerCompList.Clear();
            rootObject.GetComponentsInChildren(true, _tempTriggerCompList);

            foreach (var comp in _tempTriggerCompList)
            {
                var go = comp.gameObject;

                if (_matcher != null && !_matcher(go))
                {
                    continue;
                }

                int objId = SceneTriggerObjectUtility.GetId(go);

                _logger.InfoFormat(" loadObj : {0} ,id:{1}", go, objId);
                _tempIdList.Add(objId);
                _tempGameObejctList.Add(go);

                int instacneid = gameObject.GetInstanceID();
                if (!_tempGameObjIdList.ContainsKey(instacneid))
                    _tempGameObjIdList.Add(instacneid, objId);
            }

            _tempTriggerCompList.Clear();
        }
        
        private void FillTempGameObjectListByScene_BigMap(GameObject gameObject)
        {
            var rootObject = gameObject;
            _tempTriggerCompList.Clear();
            rootObject.GetComponentsInChildren(true, _tempTriggerCompList);

            foreach (var comp in _tempTriggerCompList)
            {
                var go = comp.gameObject;

                if (_matcher != null && !_matcher(go))
                {
                    continue;
                }

                var tag = go.GetComponent<MultiTagBase>();
                if (tag == null) continue;
                int objId = tag.id;   

                _logger.InfoFormat(" loadObj : {0} ,id:{1}", go, objId);
                _tempIdList.Add(objId);
                _tempGameObejctList.Add(go);

                int instacneid = gameObject.GetInstanceID();
                if (!_tempGameObjIdList.ContainsKey(instacneid))
                    _tempGameObjIdList.Add(instacneid, objId);
            }

            _tempTriggerCompList.Clear();
        }
        
        private void FillTempGameObjectIdList(GameObject gameObject)
        {
            if (!_tempGameObjIdList.ContainsKey(gameObject.GetInstanceID()))
            {
                return;
            }
            var sceneId = _tempGameObjIdList[gameObject.GetInstanceID()];
            _tempIdList.Add(sceneId);
        }

        private void ClearTempGameObjectList()
        {
            _tempIdList.Clear();
            _tempGameObejctList.Clear();
        }

        private void AddTempGameObjectToPool()
        {
            int count = _tempIdList.Count;
            for (int i = 0; i < count; ++i)
            {
                _pool.Put(_tempIdList[i], _tempGameObejctList[i]);
            }
        }

        private void RemoveTempGameObjectFromPool()
        {
            foreach (var id in _tempIdList)
            {
                _pool.Remove(id);
            }
        }

        public GameObject Get(int id)
        {
            return _pool.Get(id);
        }

        public HashSet<int> GetAllId()
        {
            return _pool.LastLoadedIdSet;
        }

        public HashSet<int> LastLoadedIdSet
        {
            get { return _pool.LastLoadedIdSet; }
        }

        public HashSet<int> LastUnloadedIdSet
        {
            get { return _pool.LastUnloadedIdSet; }
        }

        public void OnMapObjLoaded(UnityObject unityObj, int id)
        {
            FillTempGameObjectList(unityObj, id);
            AddTempGameObjectToPool();
            ClearTempGameObjectList();
            if(_logger.IsDebugEnabled)
                _logger.DebugFormat("Load Scene Trigger Object Type {0} name{1}", _typeValue, unityObj.AsGameObject.name);
        }

        public void OnMapObjSceneLoaded_SmallMap(UnityObject unityObj)
        {
            FillTempGameObjectListByScene_SmallMap(unityObj);
            AddTempGameObjectToPool();
            ClearTempGameObjectList();
            if(_logger.IsDebugEnabled)
                _logger.DebugFormat("Load Scene Trigger Object Type {0} name{1}", _typeValue, unityObj.AsGameObject.name);
        }
        
        public void OnMapObjSceneLoaded_BigMap(UnityObject unityObj)
        {
            FillTempGameObjectListByScene_BigMap(unityObj);
            AddTempGameObjectToPool();
            ClearTempGameObjectList();
            if(_logger.IsDebugEnabled)
                _logger.DebugFormat("Load Scene Trigger Object Type {0} name{1}", _typeValue, unityObj.AsGameObject.name);
        }

        public void OnMapObjUnloaded(UnityObject unityObj)
        {
            FillTempGameObjectIdList(unityObj);
            RemoveTempGameObjectFromPool();
            ClearTempGameObjectList();
			if(_logger.IsDebugEnabled)
            	_logger.DebugFormat("TriggerObjManager: UnLoad Scene Trigger Object Type {0} from scene{1}", _typeValue, unityObj.AsGameObject.name);
        }

        public void Clear()
        {
            _pool.Clear();
        }

        public void Dispose()
        {
            _pool = new TriggerObjectPool();

            _tempIdList.Clear();
            _tempGameObejctList.Clear();
            _tempGameObjIdList.Clear();
        }
    }

    internal static class SceneTriggerObjectUtility
    {
        public static int GetId(GameObject gameObject)
        {
            var pos = gameObject.transform.position;
            int id = (int) (pos.x * 100) * 1000 * 1000 + (int) (pos.y * 100) * 1000 + (int) (pos.z * 100);
            return id;
        }
    }

    interface ISceneListener
    {
        void OnSceneLoaded_BigMap(Scene scene);
        void OnSceneLoaded_SmallMap(Scene scene);
        void OnSceneUnloaded(Scene scene);
    }
    
    public class SceneObjManager : ISceneListener
    {
        private TriggerObjectManager _triggerObjectManager;
        
        public SceneObjManager()
        {
            var triggerManager =  SingletonManager.Get<TriggerObjectManager>();
            _triggerObjectManager = triggerManager;
        }
        
        public void OnSceneLoaded_SmallMap(Scene scene)
        {
            var rootObjects = scene.GetRootGameObjects();
            foreach (var rootObject in rootObjects)
            {
                var unityObj = new UnityObject(rootObject,AssetInfo.EmptyInstance);
                _triggerObjectManager.OnMapObjSceneLoaded_SmallMap(unityObj);
            }
        }
        
        public void OnSceneLoaded_BigMap(Scene scene)
        {
            var rootObjects = scene.GetRootGameObjects();
            foreach (var rootObject in rootObjects)
            {
                var unityObj = new UnityObject(rootObject,AssetInfo.EmptyInstance);
                _triggerObjectManager.OnMapObjSceneLoaded_BigMap(unityObj);
            }
        }

        public void OnSceneUnloaded(Scene scene)
        {
            _triggerObjectManager.Clear();
        }
    }
}
