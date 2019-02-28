using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using App.Shared.SceneManagement;
using Core.SceneManagement;
using Core.Utils;
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
        void OnMapObjLoaded(UnityObject unityObj);
        void OnMapObjUnloaded(UnityObject unityObj);
    }

    public interface ITriggerObjectListener
    {
        void OnTriggerObjectLoaded(string id, GameObject gameObject);
        void OnTriggerObjectUnloaded(string id);
    }

    internal interface ILastTriggerObjectAccessor
    {
        HashSet<string> LastLoadedIdSet { get; }
        HashSet<string> LastUnloadedIdSet { get; }
    }

    public interface ITriggerObjectManager : IGameObjectListener
    {
        GameObject Get(string id);
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

        public GameObject Get(ETriggerObjectType type, string id)
        {
            return _managers[(int)type].Get(id);
        }

        public void OnMapObjLoaded(UnityObject gameObject)
        {
            foreach (var manager in _managers)
            {
                manager.OnMapObjLoaded(gameObject);
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
        private Dictionary<string, GameObject> _triggerObjects = new Dictionary<string, GameObject>();

        private HashSet<string> _lastLoadedIdSet = new HashSet<string>();
        private HashSet<string> _lastUnloadedIdSet = new HashSet<string>();

        public GameObject Get(string id)
        {
            if (_triggerObjects.ContainsKey(id))
            {
                return _triggerObjects[id];
            }
            return null;
        }
        

        public void Put(string id, GameObject go)
        {
            _triggerObjects[id] = go;
            CacheNewId(id);
        }

        private void CacheNewId(string id)
        {
            if (_lastUnloadedIdSet.Contains(id))
            {
                _lastUnloadedIdSet.Remove(id);
            }

            _lastLoadedIdSet.Add(id);
        }

        public void Remove(string id)
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

        private void CacheRemoveId(string id)
        {
            if (_lastLoadedIdSet.Contains(id))
            {
                _lastLoadedIdSet.Remove(id);
            }

            _lastUnloadedIdSet.Add(id);
        }

        public HashSet<string> LastLoadedIdSet { get { return _lastLoadedIdSet; } }
        public HashSet<string> LastUnloadedIdSet { get { return _lastUnloadedIdSet; } }

    }

    internal class TriggerObjectInternalManager<TTriggerScript> : ITriggerObjectInternalManger where TTriggerScript: MonoBehaviour
    {

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(TriggerObjectInternalManager<TTriggerScript>));
        private int _typeValue;
        private TriggerObjectPool _pool = new TriggerObjectPool();

        private List<string> _tempIdList = new List<string>();
        private List<GameObject> _tempGameObejctList = new List<GameObject>();

        private Predicate<GameObject> _matcher;

        public TriggerObjectInternalManager(ETriggerObjectType type, Predicate<GameObject> matcher = null)
        {
            _typeValue = (int)type;
            _matcher = matcher;
        }
        
        private void FillTempGameObjectList(GameObject gameObject)
        {
            FillTempGameObjectList_dynamic(gameObject);
        }

        private List<TTriggerScript> _tempTriggerCompList = new List<TTriggerScript>();
        private void FillTempGameObjectList_dynamic(GameObject gameObject)
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

                var id = SceneTriggerObjectUtility.GetId(go);
                _tempIdList.Add(id);
                _tempGameObejctList.Add(go);
            }

            _tempTriggerCompList.Clear();
        }

        private void FillTempGameObjectIdList(GameObject gameObject)
        {
            var sceneId = SceneTriggerObjectUtility.GetId(gameObject);
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

        public GameObject Get(string id)
        {
            return _pool.Get(id);
        }

        public HashSet<string> LastLoadedIdSet
        {
            get { return _pool.LastLoadedIdSet; }
        }

        public HashSet<string> LastUnloadedIdSet
        {
            get { return _pool.LastUnloadedIdSet; }
        }

        public void OnMapObjLoaded(UnityObject unityObj)
        {
            FillTempGameObjectList(unityObj);
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

            _tempIdList = new List<string>();
            _tempGameObejctList = new List<GameObject>();
        }
    }

    internal static class SceneTriggerObjectUtility
    {
      
        public static string GetId(GameObject gameObject)
        {
           
           
            var pos = gameObject.transform.position;
            long id = (int) (pos.x * 100) * 1000000L * 1000000L + (int) (pos.y * 100) * 1000000L + (int) (pos.z * 100);
            return id.ToString();
        }
    }

    interface ISceneListener
    {
        void OnSceneLoaded(Scene scene);
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
        
        public void OnSceneLoaded(Scene scene)
        {
            var rootObjects = scene.GetRootGameObjects();
            foreach (var rootObject in rootObjects)
            {
                var obj = new UnityObject(rootObject, AssetInfo.EmptyInstance);
                _triggerObjectManager.OnMapObjLoaded(obj);
            }
        }
        public void OnSceneUnloaded(Scene scene)
        {
            _triggerObjectManager.Clear();
        }
    }
}
