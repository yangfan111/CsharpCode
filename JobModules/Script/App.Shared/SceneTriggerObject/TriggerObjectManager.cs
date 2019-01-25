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
        void OnMapObjLoaded(UnityObjectWrapper<GameObject> gameObject);
        void OnMapObjUnloaded(UnityObjectWrapper<GameObject> gameObject);
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
                _managers[(int)ETriggerObjectType.Door] = new TriggerObjectInternalManager(ETriggerObjectType.Door, typeof(Door));
                _managers[(int)ETriggerObjectType.DestructibleObject] = new TriggerObjectInternalManager(ETriggerObjectType.DestructibleObject, typeof(FracturedObject), (go) => go.GetComponent<Door>() == null);
                _managers[(int)ETriggerObjectType.GlassyObject] = new TriggerObjectInternalManager(ETriggerObjectType.GlassyObject, typeof(FracturedGlassyObject));
            }
            else
            {
                _managers[(int)ETriggerObjectType.Door] = new TriggerObjectInternalManager(ETriggerObjectType.Door, "DoorCollection");
                _managers[(int)ETriggerObjectType.DestructibleObject] = new TriggerObjectInternalManager(ETriggerObjectType.DestructibleObject, "FracturedObjectCollection");
                _managers[(int)ETriggerObjectType.GlassyObject] = new TriggerObjectInternalManager(ETriggerObjectType.GlassyObject, "FracturedGlassyObjectCollection");
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

        public void OnMapObjLoaded(UnityObjectWrapper<GameObject> gameObject)
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

        public void OnMapObjUnloaded(UnityObjectWrapper<GameObject> gameObject)
        {
            foreach (var manager in _managers)
            {
                manager.OnMapObjUnloaded(gameObject);
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
            _triggerObjects.Remove(id);
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

    public static class TriggerObjectLoadProfiler
    {
        
        private static Dictionary<GameObject,float> _totallDuration = new Dictionary<GameObject, float>();
        private static Dictionary<GameObject, int> _totalCount = new Dictionary<GameObject, int>();
        private static bool _isValid;
        private static GameObject _currentSceneObject;

        private static Stopwatch _stopwatch = new Stopwatch();

        public static void Start(GameObject gameObject)
        {
            _isValid = IsValid(gameObject);
            if (_isValid)
            {
                _currentSceneObject = gameObject;
                _stopwatch.Start();
            }
        }

        public static float Stop()
        {
            if (_isValid)
            {
                _stopwatch.Stop();
                var elapsedTime = _stopwatch.ElapsedMilliseconds;
                
                if (!_totallDuration.ContainsKey(_currentSceneObject))
                    _totallDuration[_currentSceneObject] = 0;
                if (!_totalCount.ContainsKey(_currentSceneObject))
                    _totalCount[_currentSceneObject] = 0;
                
                _totallDuration[_currentSceneObject] += elapsedTime;
                _totalCount[_currentSceneObject] += 1;
                return elapsedTime;
            }

            return 0.0f;
        }

        private static bool IsValid(GameObject gameObject)
        {
            return true;
        }

        public static void Iterate(Action<int, int, int, float> action)
        {
            if (action != null)
            {
                action(0, 0, _totalCount[_currentSceneObject], _totallDuration[_currentSceneObject]);
            }
        }
    }

    internal class TriggerObjectInternalManager : ITriggerObjectInternalManger
    {

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(TriggerObjectInternalManager));
        private int _typeValue;
        private TriggerObjectPool _pool = new TriggerObjectPool();

        private List<string> _tempIdList = new List<string>();
        private List<GameObject> _tempGameObejctList = new List<GameObject>();

        private Dictionary<string, string[]> _sceneTriggerObjectIdList = new Dictionary<string, string[]>();

        private string _collectionObjectName;
        private object[] _messageArg;

        private Type _triggerScriptType;
        private Predicate<GameObject> _matcher;

        public TriggerObjectInternalManager(ETriggerObjectType type, Type triggerScriptType, Predicate<GameObject> matcher = null)
        {
            _typeValue = (int)type;
            _triggerScriptType = triggerScriptType;
            _matcher = matcher;
        }

        public TriggerObjectInternalManager(ETriggerObjectType type, string collectionObjectName)
        {
            _typeValue = (int)type;

            _messageArg = new object[2];
            _collectionObjectName = collectionObjectName;
        }
        
        private void FillTempGameObjectList(GameObject gameObject)
        {
            if (_collectionObjectName != null)
            {
                FillTempGameObjectList_static(gameObject);
            }
            else
            {
                FillTempGameObjectList_dynamic(gameObject);
            }
        }

        private void FillTempGameObjectList_static(GameObject gameObject)
        {
            var rootObject = gameObject;

            if (rootObject.gameObject.name.Equals(_collectionObjectName))
            {
                _messageArg[0] = _messageArg[1] = null;
                rootObject.gameObject.SendMessage("ObjectCollection_Data", _messageArg,
                    SendMessageOptions.DontRequireReceiver);
                if (_messageArg[0] == null || _messageArg[1] == null)
                {
                    return;
                }

                var idArray = (string[]) _messageArg[0];
                var gameObjectArray = (GameObject[]) _messageArg[1];

                _logger.DebugFormat("TriggerObject {0} List Load Count {1}", _collectionObjectName, idArray.Length);

                AssertUtility.Assert(_tempIdList.Count == _tempGameObejctList.Count);

                var sceneId = SceneTriggerObjectUtility.GetId(rootObject);
                _sceneTriggerObjectIdList[sceneId] = idArray;

                for (int i = 0; i < idArray.Length; ++i)
                {
                    var go = gameObjectArray[i];
                    if (go != null)
                    {
                        _logger.DebugFormat("TriggerObject Load type {0} id {1} name {2}", _typeValue, idArray[i],
                            go.name);
                        var id = SceneTriggerObjectUtility.GetId(go);
                        _tempIdList.Add(id);
                        _tempGameObejctList.Add(go);
                    }
                }
            }
        }

        private void FillTempGameObjectList_dynamic(GameObject gameObject)
        {
            var rootObject = gameObject;
                var comps = rootObject.GetComponentsInChildren(_triggerScriptType, true);
            for (int i = 0; i < comps.Length; ++i)
            {
                var comp = comps[i];
                var go = comp.gameObject;
                if (_matcher != null && !_matcher(go))
                {
                    continue;
                }

                var id = SceneTriggerObjectUtility.GetId(go);
                _tempIdList.Add(id);
                _tempGameObejctList.Add(go);
            }
        }

        private void FillTempGameObjectIdList(GameObject gameObject)
        {
            var sceneId = SceneTriggerObjectUtility.GetId(gameObject);
            if (_sceneTriggerObjectIdList.ContainsKey(sceneId))
            {
                var idArray = _sceneTriggerObjectIdList[sceneId];
                if (idArray != null)
                {
                    var idCount = idArray.Length;
                    for (int i = 0; i < idCount; ++i)
                    {
                        _tempIdList.Add(idArray[i]);
                    }
                }

            }
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

        public void OnMapObjLoaded(UnityObjectWrapper<GameObject> gameObject)
        {
            TriggerObjectLoadProfiler.Start(gameObject.Value);
            FillTempGameObjectList(gameObject.Value);
            AddTempGameObjectToPool();
            ClearTempGameObjectList();
            var elapsedTime = TriggerObjectLoadProfiler.Stop();
            _logger.DebugFormat("Load Scene Trigger Object Type {0} from scene{1}, cost {2}", _typeValue, gameObject, elapsedTime);
        }

        public void OnMapObjUnloaded(UnityObjectWrapper<GameObject> gameObject)
        {
            FillTempGameObjectIdList(gameObject.Value);
            RemoveTempGameObjectFromPool();
            ClearTempGameObjectList();
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

            _sceneTriggerObjectIdList = new Dictionary<string, string[]>();
        }
    }

    internal static class SceneTriggerObjectUtility
    {
      
        public static string GetId(GameObject gameObject)
        {
            StringBuilder sb = new StringBuilder();
            var pos = gameObject.transform.position;
            sb.Append(pos.x.ToString("0.00"));
            sb.Append(" ");
            sb.Append(pos.y.ToString("0.00"));
            sb.Append(" ");
            sb.Append(pos.z.ToString("0.00"));
            return sb.ToString();
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
                var obj = new UnityObjectWrapper<GameObject>(rootObject, AssetInfo.EmptyInstance, 0);
                _triggerObjectManager.OnMapObjLoaded(obj);
            }
        }
        public void OnSceneUnloaded(Scene scene)
        {
            _triggerObjectManager.Clear();
        }
    }
}
