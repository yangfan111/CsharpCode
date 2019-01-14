using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Core.Utils;
using UltimateFracturing;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Singleton;

namespace App.Shared.SceneTriggerObject
{
    public interface ISceneListener
    {
        void OnSceneLoaded(int index1, int index2, Scene scene);

        void OnSceneUnloaded(int index1, int index2);
    }

    public interface ITriggerObjectListener
    {
        void OnTriggerObjectLoaded(int id, GameObject gameObject);
        void OnTriggerObjectUnloaded(int id);
    }

    internal interface ILastTriggerObjectAccessor
    {
        HashSet<int> LastLoadedIdSet { get; }
        HashSet<int> LastUnloadedIdSet { get; }
    }

    public interface ITriggerObjectManager : ISceneListener
    {
        GameObject Get(int id);
    }

    internal interface ITriggerObjectInternalManger : ITriggerObjectManager, ILastTriggerObjectAccessor, IDisposable
    {

    }

    public enum ETriggerObjectType
    {
        Door = 0,
        DestructibleObject,
        GlassyObject,
        MaxCount,
    }

    public class TriggerObjectManager : DisposableSingleton<TriggerObjectManager>, ISceneListener
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

        public GameObject Get(ETriggerObjectType type, int id)
        {
            return _managers[(int)type].Get(id);
        }

        public void OnSceneLoaded(int index1, int index2, Scene scene)
        {
            foreach (var manager in _managers)
            {
                manager.OnSceneLoaded(index1, index2, scene);
            }
        }

        public void OnSceneUnloaded(int index1, int index2)
        {
            foreach (var manager in _managers)
            {
                manager.OnSceneUnloaded(index1, index2);
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


            if (_lastUnloadedIdSet.Contains(id))
            {
                _lastUnloadedIdSet.Remove(id);
            }

            _lastLoadedIdSet.Add(id);
        }

        public void Remove(int id)
        {
            _triggerObjects.Remove(id);

            if (_lastLoadedIdSet.Contains(id))
            {
                _lastLoadedIdSet.Remove(id);
            }

            _lastUnloadedIdSet.Add(id);
        }

        public HashSet<int> LastLoadedIdSet { get { return _lastLoadedIdSet; } }
        public HashSet<int> LastUnloadedIdSet { get { return _lastUnloadedIdSet; } }

    }

    public static class TriggerObjectLoadProfiler
    {

        private static readonly int MAX_INDEX = 8;

        private static float[,] _totallDuration = new float[MAX_INDEX, MAX_INDEX];
        private static int[,] _totalCount = new int[MAX_INDEX, MAX_INDEX];
        private static bool _isValid;
        private static int _currentIndex1;
        private static int _currentIndex2;

        private static Stopwatch _stopwatch = new Stopwatch();

        public static void Start(int index1, int index2)
        {
            _isValid = IsValid(index1, index2);
            if (_isValid)
            {
                _currentIndex1 = index1;
                _currentIndex2 = index2;
                _stopwatch.Start();
            }
        }

        public static float Stop()
        {
            if (_isValid)
            {
                _stopwatch.Stop();
                var elapsedTime = _stopwatch.ElapsedMilliseconds;
                _totallDuration[_currentIndex1, _currentIndex2] += elapsedTime;
                _totalCount[_currentIndex1, _currentIndex2] += 1;
                return elapsedTime;
            }

            return 0.0f;
        }

        private static bool IsValid(int index1, int index2)
        {
            return index1 >= 0 && index1 < MAX_INDEX &&
                   index2 >= 0 && index2 < MAX_INDEX;
        }

        public static void Iterate(Action<int, int, int, float> action)
        {
            if (action != null)
            {
                for (int i = 0; i < MAX_INDEX; ++i)
                {
                    for (int j = 0; j < MAX_INDEX; ++j)
                    {
                        action(i, j, _totalCount[i, j], _totallDuration[i, j]);
                    }
                }
            }
            
        }
    }

    internal class TriggerObjectInternalManager : ITriggerObjectInternalManger
    {

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(TriggerObjectInternalManager));
        private int _typeValue;
        private TriggerObjectPool _pool = new TriggerObjectPool();

        private List<int> _tempIdList = new List<int>();
        private List<GameObject> _tempGameObejctList = new List<GameObject>();

        private Dictionary<int, int[]> _sceneTriggerObjectIdList = new Dictionary<int, int[]>();

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

        private void FillTempGameObjectList(int index1, int index2, Scene scene)
        {
            if (_collectionObjectName != null)
            {
                FillTempGameObjectList_static(index1, index2, scene);
            }
            else
            {
                FillTempGameObjectList_dynamic(index1, index2, scene);
            }
        }

        private void FillTempGameObjectList_static(int index1, int index2, Scene scene)
        {
            var rootObjects = scene.GetRootGameObjects();

            foreach (var rootObject in rootObjects)
            {
                if (rootObject.gameObject.name.Equals(_collectionObjectName))
                {
                    _messageArg[0] = _messageArg[1] = null;
                    rootObject.gameObject.SendMessage("ObjectCollection_Data", _messageArg, SendMessageOptions.DontRequireReceiver);
                    if (_messageArg[0] == null || _messageArg[1] == null)
                    {
                        continue;
                    }

                    var idArray = (int[])_messageArg[0];
                    var gameObjectArray = (GameObject[])_messageArg[1];

                    _logger.DebugFormat("TriggerObject {0} List Load Count {1}", _collectionObjectName, idArray.Length);

                    AssertUtility.Assert(_tempIdList.Count == _tempGameObejctList.Count);

                    var sceneId = SceneTriggerObjectUtility.GetSceneId(index1, index2);
                    _sceneTriggerObjectIdList[sceneId] = idArray;

                    for (int i = 0; i < idArray.Length; ++i)
                    {
                        var go = gameObjectArray[i];
                        if (go != null)
                        {
                            _logger.DebugFormat("TriggerObject Load type {0} id {1} name {2}", _typeValue, idArray[i], go.name);
                            var id = SceneTriggerObjectUtility.GetId(_typeValue, index1, index2, idArray[i]);
                            _tempIdList.Add(id);
                            _tempGameObejctList.Add(go);
                        }
                    }
                }
            }
        }

        private void FillTempGameObjectList_dynamic(int index1, int index2, Scene scene)
        {
            var rootObjects = scene.GetRootGameObjects();
            foreach (var rootObject in rootObjects)
            {
                var comps = rootObject.GetComponentsInChildren(_triggerScriptType, true);
                for (int i = 0; i < comps.Length; ++i)
                {
                    var comp = comps[i];
                    var go = comp.gameObject;
                    if (_matcher != null && !_matcher(go))
                    {
                        continue;
                    }

                    var id = SceneTriggerObjectUtility.GetId(_typeValue, index1, index2, _tempIdList.Count);
                    _tempIdList.Add(id);
                    _tempGameObejctList.Add(go);
                }
            }
        }

        private void FillTempGameObjectIdList(int index1, int index2)
        {
            var sceneId = SceneTriggerObjectUtility.GetSceneId(index1, index2);
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

        public GameObject Get(int id)
        {
            return _pool.Get(id);
        }

        public HashSet<int> LastLoadedIdSet
        {
            get { return _pool.LastLoadedIdSet; }
        }

        public HashSet<int> LastUnloadedIdSet
        {
            get { return _pool.LastUnloadedIdSet; }
        }

        public void OnSceneLoaded(int index1, int index2, Scene scene)
        {
            TriggerObjectLoadProfiler.Start(index1, index2);
            FillTempGameObjectList(index1, index2, scene);
            AddTempGameObjectToPool();
            ClearTempGameObjectList();
            var elapsedTime = TriggerObjectLoadProfiler.Stop();
            _logger.InfoFormat("Load Scene Trigger Object Type {0} from scene{1}x{2}, cost {3}", _typeValue, index1, index2, elapsedTime);
        }

        public void OnSceneUnloaded(int index1, int index2)
        {
            FillTempGameObjectIdList(index1, index2);
            RemoveTempGameObjectFromPool();
            ClearTempGameObjectList();
        }

        public void Dispose()
        {
            _pool = new TriggerObjectPool();

            _tempIdList = new List<int>();
            _tempGameObejctList = new List<GameObject>();

            _sceneTriggerObjectIdList = new Dictionary<int, int[]>();
        }
    }

    internal static class SceneTriggerObjectUtility
    {
        private static readonly int TYPE_BIT = 28;
        private static readonly int INDEX1_BIT = 24;
        private static readonly int INDEX2_BIT = 20;


        public static int GetSceneId(int index1, int index2)
        {
            if (index1 >= 16 || index2 >= 16)
            {
                throw new Exception(String.Format("The Scene Id {0} {1} exceeds maximum limits",
                    index1, index2));
            }

            return index1 << 4 | index2;
        }

        public static int GetId(int typeVavlue, int index1, int index2, int id)
        {
            if (typeVavlue >= 16 || index1 >= 16 || index2 >= 16 || id >= (1 << INDEX2_BIT))
            {
                throw new Exception(String.Format("The Scene object Id {0} {1} {2} exceeds maximum limits",
                    index1, index2, id));
            }

            int objId = (typeVavlue << TYPE_BIT) | (index1 << INDEX1_BIT) | (index2 << INDEX2_BIT) | id;
            return objId;
        }
    }
}
