using AssetBundleManagement;
using Core;
using Core.ObjectPool;
using Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using AssetBundleManager.Warehouse;
using Common;
using UnityEngine;
using Utils.Singleton;
using XmlConfig.BootConfig;
using Object = UnityEngine.Object;

namespace Utils.AssetManager
{
    public interface IAssetPostProcessor
    {
        void LoadFromAsset(UnityObject unityObject);
        void LoadFromPool(UnityObject unityObject);
        void Recyle(UnityObject unityObject);
        void OnDestory(UnityObject unityObject);
    }

    public struct AssetLoadOption
    {
        public GameObject Parent;
        public bool DontAutoActive;
        public bool Recyclable;
        public Func<UnityObject, IAssetPostProcessor> PostProcessorFactory;

        public AssetLoadOption(bool dontAutoActive = false, GameObject parent = null, bool recyclable = false,
            Func<UnityObject, IAssetPostProcessor> postProcessorFactory = null)
        {
            DontAutoActive = dontAutoActive;
            Parent = parent;
            Recyclable = recyclable;
            PostProcessorFactory = postProcessorFactory;
        }

        public static AssetLoadOption Default = new AssetLoadOption();
    }

    public interface IUnityAssetManager
    {
        IEnumerator Init(ResourceConfig config, ICoRoutineManager coRoutineManager, bool isLow=false);

        HashSet<string> AllAssetBundleNames { get; }

        Dictionary<string, LoadedAssetBundle> LoadedAssetBundles { get; }

        void LoadAssetBundleAsync(string bundleName);


        void LoadAssetAsync<T>(T source, AssetInfo assetInfo, Action<T, UnityObject> handlerAction,
            AssetLoadOption assetLoadOption = default(AssetLoadOption));


        void LoadAssetsAsync<T>(T source, IEnumerable<AssetInfo> assetInfos,
            Action<T, IEnumerable<UnityObject>> handlerAction,
            AssetLoadOption assetLoadOption = default(AssetLoadOption));

        void LoadSceneAsync(AssetInfo assetInfo, bool isAdditive);

        void LoadCancel(object source);

        void LoadCancel(AssetInfo assetInfo);

        void Recycle(UnityObject unityObject);

        void Update();

        IEnumerator Clear();
    }

    public class UnityAssetManager : IUnityAssetManager, IAssetLoadStat
    {
        private enum ELoadRequestType
        {
            Bundle,
            Scene,
            Asset,
        }

        private abstract class AbstractAssetLoadRequest
        {
            protected IObjectAllocator Allocator;

            public ELoadRequestType RequestType;

            public abstract object SourceObject { get; }

            public bool AutoActive
            {
                get { return !Option.DontAutoActive; }
            }

            public GameObject Parent
            {
                get { return Option.Parent; }
            }

            public bool IsDisposed;
            public AssetInfo AssetInfo;


            public bool IsAdditiveScene;

            public AssetLoadOption Option;
            public UnityObject LoadedObject;


            public abstract void InvokeOnLoaded(Action<AbstractAssetLoadRequest> disposeOperation);

            public virtual void Dispose()
            {
                IsDisposed = false;
                LoadedObject = null;
                Option = AssetLoadOption.Default;
            }

            public static void Free(AbstractAssetLoadRequest request, bool destroyLoadedObj = false)
            {
                if (destroyLoadedObj)
                {
                    var loadedObj = request.LoadedObject;
                    if (loadedObj != null && loadedObj.AsGameObject != null)
                    {
                        Object.Destroy(loadedObj.AsGameObject);
                    }
                }

                request.Dispose();
                var allocator = request.Allocator;
                if (allocator == null)
                {
                    throw new Exception(String.Format("Can not found allocator for load request {0} {1}!",
                        request.RequestType, request.AssetInfo));
                }


                request.Allocator = null;
                allocator.Free(request);
            }
        }

        private class AssetLoadRequestNode
        {
            public AbstractAssetLoadRequest Value;
            public AssetLoadRequestNode Next;

            public AssetLoadRequestNode()
            {
            }

            public static AssetLoadRequestNode Alloc(AbstractAssetLoadRequest request)
            {
                var node = ObjectAllocatorHolder<AssetLoadRequestNode>.Allocate();
                node.Value = request;
                return node;
            }

            public static void Free(AssetLoadRequestNode node)
            {
                node.Value = null;
                node.Next = null;
                ObjectAllocatorHolder<AssetLoadRequestNode>.Free(node);
            }
        }

        private class AssetLoadRequest<T> : AbstractAssetLoadRequest
        {
            public override object SourceObject
            {
                get { return Source; }
            }

            public T Source;

            private Action<T, UnityObject> _onLoaded;

            public Action<T, UnityObject> OnLoaded
            {
                set { _onLoaded = value; }
            }

            private AssetLoadRequestBatch<T> _requestBatch;

            public AssetLoadRequestBatch<T> ReferenceBatch
            {
                set { _requestBatch = value; }
            }


            public override void InvokeOnLoaded(Action<AbstractAssetLoadRequest> disposeOperation)
            {
                var loadedObject = LoadedObject;
                if (loadedObject == null)
                {
                    loadedObject = new UnityObject(null, AssetInfo);
                }
                else if (IsDisposed)
                {
                    disposeOperation(this);
                }

                if (_requestBatch != null || _onLoaded != null)
                {
                    var profiler = SingletonManager.Get<LoadRequestProfileHelp>().GetProfile(AssetInfo);
                    profiler.StartWatch();

                    if (_requestBatch != null)
                    {
                        _requestBatch.UnpackRequest(this, loadedObject);
                    }
                    else if (_onLoaded != null && !IsDisposed)
                    {
                        _onLoaded(Source, loadedObject);
                    }

                    profiler.TotalHandlerTime += profiler.StopWatch();
                }
            }

            public static AssetLoadRequest<T> Alloc()
            {
                var req = ObjectAllocatorHolder<AssetLoadRequest<T>>.Allocate();
                if (req.Allocator == null)
                    req.Allocator = ObjectAllocatorHolder<AssetLoadRequest<T>>.GetAllocator();
                return req;
            }

            public override void Dispose()
            {
                base.Dispose();

                if (_requestBatch != null)
                {
                    AssetLoadRequestBatch<T>.Free(_requestBatch);
                    _requestBatch = null;
                }

                Source = default(T);
                OnLoaded = null;
            }
        }

        private class AssetLoadRequestBatch<T>
        {
            private static LoggerAdapter _logger = new LoggerAdapter(typeof(AssetLoadRequestBatch<T>));

            private T _source;
            private List<UnityObject> _loadedUnityObjects = new List<UnityObject>();

            private Action<T, IEnumerable<UnityObject>> _onLoaded;

            public Action<T, IEnumerable<UnityObject>> OnLoaded
            {
                set { _onLoaded = value; }
            }

            private bool _isDisposed;
            private int _referenceCount;

            public void PackRequest(AssetLoadRequest<T> req)
            {
                if (_source == null)
                {
                    _source = req.Source;
                }

                req.ReferenceBatch = this;
                _referenceCount++;
            }

            public void UnpackRequest(AssetLoadRequest<T> req, UnityObject unityObj)
            {
                if (_isDisposed)
                {
                    _logger.Error("Call UnpackRequest On Disposed Asset Load Request Batch!");
                    return;
                }

                _loadedUnityObjects.Add(unityObj);
                req.ReferenceBatch = null;
                _referenceCount--;
                if (_referenceCount <= 0)
                {
                    _referenceCount = 0;

                    if (_onLoaded != null)
                    {
                        try
                        {
                            _onLoaded(_source, _loadedUnityObjects);
                        }
                        catch (Exception e)
                        {
                            _logger.Error("Batch OnLoaded Callback Error", e);
                        }
                    }

                    Free(this);
                }
            }

            public static AssetLoadRequestBatch<T> Alloc()
            {
                var batch = ObjectAllocatorHolder<AssetLoadRequestBatch<T>>.Allocate();
                batch._isDisposed = false;
                return batch;
            }

            public static void Free(AssetLoadRequestBatch<T> batch)
            {
                if (!batch._isDisposed)
                {
                    batch._isDisposed = true;
                    batch._source = default(T);
                    batch.OnLoaded = null;
                    batch._loadedUnityObjects.Clear();
                    ObjectAllocatorHolder<AssetLoadRequestBatch<T>>.Free(batch);
                }
            }
        }

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(UnityAssetManager));
        private static AssetBundleStat _bundleStat = new AssetBundleStat();
        private static AssetBundlePool _bundlePool = new AssetBundlePool(_bundleStat);
        private static event Action<AssetLoadedEvent> LoadedCallback;
        private static int _lockCount = 0;
        private static bool _isWareHouseConfig;
        private static bool _isInitialized;

        private readonly int _maxLoadingAssetNum;

        private static Dictionary<AssetInfo, Object> _assetPool =
            new Dictionary<AssetInfo, Object>(AssetInfo.AssetInfoIngoreCaseComparer
                .Instance); //assetpool only contains the loaded asset for GameObject

        private LinkedList<AbstractAssetLoadRequest> _pendingRequests = new LinkedList<AbstractAssetLoadRequest>();

        private Dictionary<AssetInfo, AssetLoadRequestNode> _loadingRequests =
            new Dictionary<AssetInfo, AssetLoadRequestNode>(AssetInfo.AssetInfoComparer.Instance);

        private int _loadingRequestCount = 0;
        private Queue<AbstractAssetLoadRequest> _loadedRequests = new Queue<AbstractAssetLoadRequest>();
        private HashSet<AssetInfo> _loadingScenes = new HashSet<AssetInfo>();
        private readonly IUnityObjectPool _objectPool;
        private bool _useAssetPool;
        private CustomProfileInfo _sceneLoadProfile;
        private CustomProfileInfo _assetLoadProfile;
        private CustomProfileInfo _bundleLoadProfile;

        public UnityAssetManager(IUnityObjectPool objectPool, int maxLoadingAssetNum = Int32.MaxValue,
            bool useAssetPool = true)
        {
            _objectPool = objectPool;
            _maxLoadingAssetNum = Math.Max(1, maxLoadingAssetNum);
            _useAssetPool = useAssetPool;
            LoadedCallback += AssetFound;
            _bundleStat.Add(this);
            _sceneLoadProfile = DurationHelp.Instance.GetCustomProfileInfo("UpdateLoadRequests_Scene");
            _assetLoadProfile = DurationHelp.Instance.GetCustomProfileInfo("UpdateLoadRequests_Asset");
            _bundleLoadProfile = DurationHelp.Instance.GetCustomProfileInfo("UpdateLoadRequests_Bundle");
        }

        public class SupplementaryWarehouse
        {
            public AssetBundleWarehouseAddr WarehouseAddr;
            public string[] Bundles;
        }

        public IEnumerator Init(ResourceConfig config, ICoRoutineManager coRoutineManager, bool isServer= false)
        {
            coRoutineManager.StartCoRoutine(InitUpdate());
            coRoutineManager.StartCoRoutine(Init(config, !isServer && SettingManager.SettingManager.GetInstance().GetQualityBeforeInit() == SettingManager.QualityLevel.Low));

            yield return new WaitUntil(() => _isInitialized);
        }

        private IEnumerator InitUpdate()
        {
            while (!_isInitialized)
            {
                Update();
                yield return null;
            }
        }

        private IEnumerator Init(ResourceConfig config, bool isLow)
        {
            var defaultWarehouseAddr = new AssetBundleWarehouseAddr()
            {
                Path = config.BaseUrl,
                Pattern = config.BasePattern,
                Manifest = config.Manifests,
            };

            var supplementaryWarehouses = new List<SupplementaryWarehouse>();
            var supplements = config.Supplement;
            if (supplements != null)
            {
                foreach (var supplement in supplements)
                {
#if !UNITY_EDITOR
                    if (supplement.Pattern == AssetBundleLoadingPattern.Simulation) continue;
#endif
                    if (supplement.BundleName != null && supplement.Pattern != null)
                    {
                        supplementaryWarehouses.Add(new SupplementaryWarehouse()
                        {
                            WarehouseAddr = new AssetBundleWarehouseAddr()
                            {
                                Path = supplement.Url,
                                Pattern = supplement.Pattern,
                                Manifest = supplement.Manifests
                            },
                            Bundles = new string[] {supplement.BundleName},
                        });
                    }
                }
            }

            yield return InitWarehouse(defaultWarehouseAddr, supplementaryWarehouses, isLow);
        }

        //For Test Purpose.
        public IEnumerator InitWarehouse(AssetBundleWarehouseAddr defaultWarehouseAddr,
            List<SupplementaryWarehouse> supplementaryWarehouses = null, bool isLow = false)
        {
            if (!_isWareHouseConfig)
            {
                _bundlePool.SetDefaultWarehouse(defaultWarehouseAddr);
                if (supplementaryWarehouses != null)
                {
                    foreach (var supplementaryWarehouse in supplementaryWarehouses)
                    {
                        _bundlePool.SetSupplementaryWarehouse(supplementaryWarehouse.WarehouseAddr,
                            supplementaryWarehouse.Bundles);
                    }
                }

                yield return _bundlePool.Init(isLow);
                _isWareHouseConfig = true;
                //yield return _bundlePool.Init(/*"Low"*/SettingManager.SettingManager.GetInstance().GetQualityBeforeInit().ToString());

                _isInitialized = true;
            }
            else
            {
                _logger.WarnFormat("The warehouse for asset loader has been configured!");
            }
        }

        public static void SetLoadLocked(bool locked)
        {
            if (locked)
                _lockCount++;
            else
            {
                if (_lockCount <= 0)
                {
                    throw new Exception("Release lock of asset manager multiple times!");
                }

                _lockCount--;
            }
        }

        public HashSet<string> AllAssetBundleNames
        {
            get { return _bundlePool.AllAssetBundleNames; }
        }

        public Dictionary<string, LoadedAssetBundle> LoadedAssetBundles
        {
            get { return _bundlePool.LoadedAssetBundles; }
        }

        public void LoadAssetBundleAsync(string bundleName)
        {
            var req = AssetLoadRequest<object>.Alloc();
            req.Source = null;
            req.OnLoaded = null;
            req.AssetInfo = new AssetInfo() {BundleName = bundleName, AssetName = null};
            req.RequestType = ELoadRequestType.Bundle;
            _pendingRequests.AddLast(req);
        }

        public void LoadAssetAsync<T>(T source, AssetInfo assetInfo, Action<T, UnityObject> handlerAction,
            AssetLoadOption assetLoadOption = default(AssetLoadOption))
        {
            AppendAssetRequest(source, assetInfo, handlerAction, assetLoadOption);
        }


        public void LoadAssetsAsync<T>(T source,
            IEnumerable<AssetInfo> assetInfos,
            Action<T, IEnumerable<UnityObject>> handlerAction,
            AssetLoadOption assetLoadOption = default(AssetLoadOption))
        {
            if (assetInfos != null)
            {
                var batch = AssetLoadRequestBatch<T>.Alloc();
                batch.OnLoaded = handlerAction;
                int count = 0;
                foreach (var assetInfo in assetInfos)
                {
                    var req = AppendAssetRequest(source, assetInfo, null, assetLoadOption);
                    batch.PackRequest(req);
                    count++;
                }

                if (count == 0)
                {
                    AssetLoadRequestBatch<T>.Free(batch);
                }
            }
        }

        private AssetLoadRequest<T> AppendAssetRequest<T>(T source,
            AssetInfo assetInfo,
            Action<T, UnityObject> handlerAction,
            AssetLoadOption option)
        {
            _logger.DebugFormat("Loading Asset {0}", assetInfo);

            var req = AssetLoadRequest<T>.Alloc();
            req.Source = source;

            req.AssetInfo = assetInfo;
            req.OnLoaded = handlerAction;
            req.RequestType = ELoadRequestType.Asset;
            req.Option = option;
            if (assetInfo.BundleName == null || assetInfo.AssetName == null)
            {
                _logger.ErrorFormat("Loading Asset {0} is Invalid from {1}", assetInfo, source);
                req.IsDisposed = true;
                return req;
            }

            var profiler = SingletonManager.Get<LoadRequestProfileHelp>().GetProfile(assetInfo);
            profiler.RequestTimes++;
            var unityObj = _objectPool.GetOrNull(assetInfo, req.AutoActive);
            if (unityObj != null)
            {
                profiler.PooledTimes++;

                if (req.Parent != null)
                {
                    var transform = req.Parent.transform;
                    var go = unityObj.AsGameObject;
                    if (go != null && go.transform.parent != transform)
                    {
                        go.transform.SetParent(transform, false);
                    }
                }

                unityObj.OnLoadFromPool();
                req.LoadedObject = unityObj;
                _loadedRequests.Enqueue(req);
                _logger.DebugFormat("Load resource for object pool {0}", assetInfo);
            }
            else
            {
                _pendingRequests.AddLast(req);
            }

            return req;
        }

        public void LoadSceneAsync(AssetInfo assetInfo, bool isAdditive)
        {
            _logger.InfoFormat("LoadSeneAsync {0}", assetInfo);
            var req = AssetLoadRequest<object>.Alloc();
            req.Source = null;
            req.AssetInfo = assetInfo;
            req.OnLoaded = null;
            req.RequestType = ELoadRequestType.Scene;
            req.IsAdditiveScene = isAdditive;
            _pendingRequests.AddLast(req);
        }

        public void LoadCancel(object source)
        {
            if (source != null)
            {
                CancelInGroup(source, _pendingRequests);
                CancelInGroup(source, _loadingRequests.Values);
                CancelInGroup(source, _loadedRequests);
            }
        }

        public void LoadCancel(AssetInfo assetInfo)
        {
            CancelInGroup(assetInfo, _pendingRequests);
            CancelInGroup(assetInfo, _loadingRequests.Values);
            CancelInGroup(assetInfo, _loadedRequests);
        }

        private void CancelInGroup(object source, IEnumerable<AbstractAssetLoadRequest> group)
        {
            foreach (var req in group)
            {
                if (req.SourceObject == source)
                {
                    req.IsDisposed = true;
                }
            }
        }

        private void CancelInGroup(object source, IEnumerable<AssetLoadRequestNode> group)
        {
            foreach (var node in group)
            {
                var curNode = node;
                while (curNode != null && curNode.Value.SourceObject == source)
                {
                    curNode.Value.IsDisposed = true;
                    curNode = curNode.Next;
                }
            }
        }

        private void CancelInGroup(AssetInfo assetInfo, IEnumerable<AbstractAssetLoadRequest> group)
        {
            foreach (var req in group)
            {
                if (req.AssetInfo == assetInfo)
                {
                    req.IsDisposed = true;
                }
            }
        }

        public void Recycle(UnityObject unityObject)
        {
            if (unityObject != null && unityObject.AsObject != null)
            {
                unityObject.OnRecyle();
                var profiler = SingletonManager.Get<LoadRequestProfileHelp>().GetProfile(unityObject.Address);
                profiler.RecycleTimes++;

                _objectPool.Add(unityObject);
            }
        }

        public void Update()
        {
            UpdateBundlePool();
            SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.UnityAssetManagerUpdateLoadRequest);
            UpdateLoadRequests();
            SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.UnityAssetManagerUpdateLoadRequest);
        }

        private void UpdateLoadRequests()
        {
            if (_lockCount <= 0 && _pendingRequests.Count > 0)
            {
                var pendingIt = _pendingRequests.First;
                while (pendingIt != null)
                {
                    if (_loadingRequestCount > _maxLoadingAssetNum)
                    {
                        break;
                    }

                    var req = pendingIt.Value;
                    var next = pendingIt.Next;

                    var assetInfo = req.AssetInfo;
                    if (IsPendingReuqest(req))
                    {
                        pendingIt = next;
                        //waiting for previous identical assets loaded.
                        continue;
                    }

                    if (req.IsDisposed)
                    {
                        _loadedRequests.Enqueue(req);
                    }
                    else
                    {
                        switch (req.RequestType)
                        {
                            case ELoadRequestType.Bundle:
                            {
                                try
                                {
                                    _bundleLoadProfile.BeginProfileOnlyEnableProfile();
                                    _bundlePool.LoadAssetBundle(req.AssetInfo.BundleName);
                                }
                                finally
                                {
                                    _bundleLoadProfile.EndProfileOnlyEnableProfile();
                                }

                                break;
                            }
                            case ELoadRequestType.Scene:
                            {
                                try
                                {
                                    _sceneLoadProfile.BeginProfileOnlyEnableProfile();
                                    _bundlePool.LoadScene(assetInfo.BundleName, assetInfo.AssetName,
                                        req.IsAdditiveScene);
                                    _loadingScenes.Add(assetInfo);
                                    _loadedRequests.Enqueue(req);
                                    req.IsDisposed = true;
                                }
                                finally
                                {
                                    _sceneLoadProfile.EndProfileOnlyEnableProfile();
                                }

                                break;
                            }
                            case ELoadRequestType.Asset:
                            {
                                try
                                {
                                    _assetLoadProfile.BeginProfileOnlyEnableProfile();
                                    if (TryLoadFromAssetPool(req))
                                    {
                                        _loadedRequests.Enqueue(req);
                                    }
                                    else
                                    {
                                        _bundlePool.LoadAsset(assetInfo.BundleName, assetInfo.AssetName);
                                        var node = AssetLoadRequestNode.Alloc(req);
                                        AssetLoadRequestNode cachedNode;
                                        if (_loadingRequests.TryGetValue(req.AssetInfo, out cachedNode))
                                        {
                                            node.Next = cachedNode.Next;
                                            cachedNode.Next = node;
                                        }
                                        else
                                        {
                                            _loadingRequests[req.AssetInfo] = node;
                                        }

                                        _loadingRequestCount++;
                                    }
                                }
                                finally
                                {
                                    _assetLoadProfile.EndProfileOnlyEnableProfile();
                                }

                                break;
                            }
                        }
                    }

                    _pendingRequests.Remove(pendingIt);
                    pendingIt = next;
                }
            }

            ;
            while (_loadedRequests.Count > 0)
            {
                var req = _loadedRequests.Dequeue();
                try
                {
                    req.InvokeOnLoaded(DisposeOperation);
                }
                catch (Exception e)
                {
                    _logger.Error("OnLoaded Callback Error", e);
                }

                AbstractAssetLoadRequest.Free(req);
            }
        }

        private bool IsPendingReuqest(AbstractAssetLoadRequest req)
        {
            return req.RequestType == ELoadRequestType.Asset && _loadingRequests.ContainsKey(req.AssetInfo) &&
                   !_assetPool.ContainsKey(req.AssetInfo) ||
                   req.RequestType == ELoadRequestType.Scene && _loadingScenes.Count > 0;
        }

        private void DisposeOperation(AbstractAssetLoadRequest req)
        {
            var loadedObject = req.LoadedObject;
            if (req.Option.Recyclable)
            {
                Recycle(loadedObject);
            }
            else
            {
                loadedObject.OnDestory();
                loadedObject.Destroy();
            }
        }

        private bool TryLoadFromAssetPool(AbstractAssetLoadRequest request)
        {
            var assetInfo = request.AssetInfo;
            Object obj;
            if (_assetPool.TryGetValue(assetInfo, out obj))
            {
                _logger.DebugFormat("asset found in AssetPool {0} ", assetInfo);
                TryLoadAsGameObject(obj, assetInfo, request);
                return true;
            }

            return false;
        }

        public bool AssetNotFound(string bundleName, string assetName)
        {
            _logger.ErrorFormat("asset not found  {0} {1}", bundleName, assetName);
            return OnAssetLoaded(bundleName, assetName, null);
        }

        private void AssetFound(AssetLoadedEvent evt)
        {
            if (!evt.Processed)
            {
                _logger.DebugFormat("asset found  {0} {1}", evt.BundleName, evt.AssetName);
                evt.Processed = OnAssetLoaded(evt.BundleName, evt.AssetName, evt.LoadedAsset);
            }
        }

        public void SceneLoaded(string bundleName, string sceneName, bool loadSuccess)
        {
            _loadingScenes.Remove(new AssetInfo(bundleName, sceneName));
        }

        private bool OnAssetLoaded(string bunldeName, string assetName, Object obj)
        {
            var assetInfo = GetAssetInfo(bunldeName, assetName);
            AssetLoadRequestNode node;
            if (_loadingRequests.TryGetValue(assetInfo, out node))
            {
                //empty
                if (node.Next == null)
                    _loadingRequests.Remove(assetInfo);
                else
                {
                    var rmNode = node.Next;
                    node.Next = rmNode.Next;
                    node = rmNode;
                }

                var req = node.Value;
                AssetLoadRequestNode.Free(node);
                _loadedRequests.Enqueue(req);

                _loadingRequestCount--;

                if (_useAssetPool && !_assetPool.ContainsKey(assetInfo))
                    _assetPool.Add(assetInfo, obj);

                TryLoadAsGameObject(obj, assetInfo, req);

                return true;
            }

            return false;
        }

        private bool TryLoadAsGameObject(Object obj, AssetInfo assetInfo, AbstractAssetLoadRequest req)
        {
            var go = obj as GameObject;
            if (go != null)
            {
                var profiler = SingletonManager.Get<LoadRequestProfileHelp>().GetProfile(assetInfo);
                profiler.InstantiateTimes++;

                obj = Object.Instantiate(go, GetGameObjectParent(req), false);
                go = obj as GameObject;
            }

            var unityObj = new UnityObject(obj, assetInfo);
            req.LoadedObject = unityObj;
            if (req.AutoActive)
                unityObj.SetActive(true);

            if (go != null)
            {
                unityObj.WithPostProcessor(req.Option.PostProcessorFactory).OnLoadFromAsset();
                if (req.Option.Recyclable)
                {
                    unityObj.AddUnityObjectReference();
                }

                var profiler = SingletonManager.Get<LoadRequestProfileHelp>().GetProfile(assetInfo);
                profiler.TotalInstantiateTime += profiler.StopWatch();

                return true;
            }

            return false;
        }

        private Transform GetGameObjectParent(AbstractAssetLoadRequest request)
        {
            return request.Parent != null ? request.Parent.transform : UnityObject.DefaultParent;
        }

        public IEnumerator Clear()
        {
            _logger.InfoFormat("Clear UnityAssetManager: Wait Asset loaded...");

            if (!_bundlePool.IsIdle)
            {
                yield return new WaitUntil(() =>
                {
                    UpdateBundlePool();
                    return _bundlePool.IsIdle;
                });
            }

            _logger.InfoFormat("Clear UnityAssetManager: Start...");

            foreach (var req in _pendingRequests)
            {
                AbstractAssetLoadRequest.Free(req);
            }

            foreach (var node in _loadingRequests.Values)
            {
                var curNode = node;
                while (curNode != null)
                {
                    AbstractAssetLoadRequest.Free(curNode.Value);
                    AssetLoadRequestNode.Free(curNode);

                    curNode = curNode.Next;
                }

                _loadingRequestCount = 0;
            }

            foreach (var req in _loadedRequests)
            {
                AbstractAssetLoadRequest.Free(req, true);
            }

            _assetPool.Clear();

            _loadingScenes.Clear();
            _objectPool.Clear();
            _bundlePool.Reset();
            _logger.InfoFormat("Clear UnityAssetManager End");
        }

        private static void UpdateBundlePool()
        {
            SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.UnityAssetManagerUpdateBundlePool);
            _bundlePool.Update();
            SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.UnityAssetManagerUpdateBundlePool);
            SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.UnityAssetManagerFetchResult);
            _bundlePool.FetchAsset(LoadedCallback);
            SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.UnityAssetManagerFetchResult);
        }

        private static AssetInfo GetAssetInfo(string bundleName, string assetName)
        {
            return new AssetInfo(bundleName, assetName);
        }
    }
}