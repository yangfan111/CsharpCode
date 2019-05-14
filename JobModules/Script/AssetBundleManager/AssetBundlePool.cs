using AssetBundleManager.Operation;
using AssetBundleManager.Warehouse;
using AssetBundles;
using Common;
using Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetBundleManagement
{
    public class AssetLoadedEvent
    {
        public string BundleName;
        public string AssetName;
        public Object LoadedAsset;
        public bool Processed;
    }

    public enum AssetLoadingPattern
    {
        Unkown,
        Async,
        Sync,
        Simulation,
        Scene
    }

    static class ListDebug
    {
        public static void AddLastExt(this LinkedList<AssetLoading> list, AssetLoading assetLoading)
        {
            if (assetLoading == null)
            {
                throw new Exception("Null AssetLoading!!");
            }
            list.AddLast(assetLoading);
        }
    }

    public class AssetBundlePool
    {
        enum AssetBundleStatus
        {
            Blank, Loading, Loaded, Failed
        }
       
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AssetBundlePool));
        
        private IAssetBundleStat _statRecorder;

        // AssetBundle Address
        private AssetBundleWarehouseAddr _defaultWarehouseAddr;
        private Dictionary<AssetBundleWarehouseAddr, List<string>> _supplementaryWarehouseAddr = new Dictionary<AssetBundleWarehouseAddr, List<string>>(AssetBundleWarehouseAddr.Comparer);
        private HashSet<string> _supplementaryBundles = new HashSet<string>();

        private AssetBundleWarehouse _defaultWarehouse;
        private Dictionary<string, AssetBundleWarehouse> _supplementaryWarehouses = new Dictionary<string, AssetBundleWarehouse>();

        private Dictionary<string, string[]> _allDependencies = new Dictionary<string, string[]>();
        
        private LinkedList<AssetBundleLoading> _loadingAssetBundles = new LinkedList<AssetBundleLoading>();
        private HashSet<string> _loadingAssetBundleNames = new HashSet<string>();
        private LinkedList<AssetBundleLoading> _assetBundleWaitForDependencies = new LinkedList<AssetBundleLoading>();
        private Dictionary<string, LoadedAssetBundle> _loadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();
        private HashSet<string> _failedAssetBundleNames = new HashSet<string>();
        
        private LinkedList<AssetLoading> _assetsWaitForBundle = new LinkedList<AssetLoading>();
        private LinkedList<AssetLoading> _loadingAssets = new LinkedList<AssetLoading>();
        private LinkedList<AssetLoading> _loadedAssets = new LinkedList<AssetLoading>();

        private bool _loadDependency = true;

        public AssetBundlePool(IAssetBundleStat stat = null)
        {
            if (stat == null)
                _statRecorder = new AssetBundleStatPlaceHolder();
            else
                _statRecorder = stat; 
        }

        public bool IsIdle
        {
            get
            {
                return _loadingAssets.Count == 0
                       && _loadedAssets.Count == 0
                       && _assetsWaitForBundle.Count == 0 
                       && _loadingAssetBundleNames.Count == 0;
            }
        }

        public void SetDefaultWarehouse(AssetBundleWarehouseAddr addr)
        {
            if (addr == null)
                throw new ArgumentNullException();
            
            if (addr.Pattern == AssetBundleLoadingPattern.EndOfTheWorld
                || addr.Pattern != AssetBundleLoadingPattern.Simulation && string.IsNullOrEmpty(addr.Path))
                throw new ArgumentException(addr.ToString());

            if (string.IsNullOrEmpty(addr.Manifest))
                addr.Manifest = Utility.GetPlatformName();

            addr.Path = Utility.ProcessAssetBundleBasePath(addr.Path);

            _defaultWarehouseAddr = addr;
        }

        public void SetSupplementaryWarehouse(AssetBundleWarehouseAddr addr, string[] bundles)
        {
            if (addr == null || bundles == null)
                throw new ArgumentNullException();
                
            if (addr.Pattern == AssetBundleLoadingPattern.EndOfTheWorld
                || addr.Pattern != AssetBundleLoadingPattern.Simulation && string.IsNullOrEmpty(addr.Path))
                throw new ArgumentException(addr.ToString());

            if (bundles.Length == 0)
                throw new ArgumentException("bundles.Length equals zero");

            for (var i = 0; i < bundles.Length; i++)
            {
                if (_supplementaryBundles.Contains(bundles[i]))
                    throw new ArgumentException("duplicate supplementary bundle: " + bundles[i]);

                _supplementaryBundles.Add(bundles[i]);
            }
            
            if (string.IsNullOrEmpty(addr.Manifest))
                addr.Manifest = Utility.GetPlatformName();

            addr.Path = Utility.ProcessAssetBundleBasePath(addr.Path);
            
            if (!_supplementaryWarehouseAddr.ContainsKey(addr))
                _supplementaryWarehouseAddr.Add(addr, new List<string>());
            
            _supplementaryWarehouseAddr[addr].AddRange(bundles);
        }

        public IEnumerator Init(bool isLow)
        {
            _defaultWarehouse = WarehouseFactory.CreateWarehouse(_defaultWarehouseAddr, isLow);
            var ret = _defaultWarehouse.Init();
            if (ret != null)
            {
                _loadingAssetBundles.AddLast(ret.AssetBundleLoadingOperation);
                _assetsWaitForBundle.AddLastExt(ret.ManifestLoadingOperation);
                yield return ret.ManifestLoadingOperation;
                
                SetManifest(ret.ManifestLoadingOperation.LoadedAsset as AssetBundleManifest);
                _defaultWarehouse.SetManifest(ret.ManifestLoadingOperation.LoadedAsset as AssetBundleManifest);
                yield return Clear();
            }

            foreach (var addr in _supplementaryWarehouseAddr)
            {
                var warehouse = WarehouseFactory.CreateWarehouse(addr.Key, isLow);
                ret = warehouse.Init();
                if (ret != null)
                {
                    _loadingAssetBundles.AddLast(ret.AssetBundleLoadingOperation);
                    _assetsWaitForBundle.AddLastExt(ret.ManifestLoadingOperation);
                    yield return ret.ManifestLoadingOperation;
                    
                    SetManifest(ret.ManifestLoadingOperation.LoadedAsset as AssetBundleManifest);
                    warehouse.SetManifest(ret.ManifestLoadingOperation.LoadedAsset as AssetBundleManifest);
                    yield return Clear();
                }
                
                foreach (var bundle in addr.Value)
                    _supplementaryWarehouses.Add(bundle, warehouse);
            }
        }
        
        public void Update()
        {
            UpdateAssetBundleLoading();
            UpdateAssetLoading();
        }
        
        public void FetchAsset(Action<AssetLoadedEvent> callBack)
        {
            var assetItor = _loadedAssets.First;

            if (assetItor != null)
            {
                while (assetItor != null)
                {
                    var operation = assetItor.Value;
                    var evt = new AssetLoadedEvent()
                    {
                        BundleName = operation.BundleName,
                        AssetName =  operation.Name,
                        LoadedAsset = operation.LoadedAsset,
                        Processed = false
                    };
                    callBack(evt);
                    assetItor = assetItor.Next;
                }

                _loadedAssets.Clear();
            }
        }

        public HashSet<string> AllAssetBundleNames
        {
            get
            {
                var result = new HashSet<string>();

                foreach (var bundleName in _allDependencies.Keys)
                {
                    result.Add(bundleName);
                }

                return result;
            }
        }


        public Dictionary<string, LoadedAssetBundle> LoadedAssetBundles
        {
            get { return _loadedAssetBundles;}
        }

        public void LoadAssetBundle(string bundleName)
        {
            string bundleNameWithVariant;
            LoadAssetBundle(bundleName, out bundleNameWithVariant);
        }

        public void LoadAsset(string bundleName, string assetName, Type ObjectType)
        {
            string bundleNameWithVariant;
            AssetLoadingPattern LoadingPattern = AssetLoadingPattern.Unkown;
            if (LoadAssetBundle(bundleName, out bundleNameWithVariant) != AssetBundleStatus.Failed)
            {
                var warehouse = FindWarehouse(bundleName, _defaultWarehouse);
                var assetLoading = warehouse.LoadAsset(bundleNameWithVariant, assetName, ObjectType);
                if(warehouse is SimulationWarehouse)
                {
                    LoadingPattern = AssetLoadingPattern.Simulation;
                }
                if (assetLoading != null)
                {
                    _assetsWaitForBundle.AddLastExt(assetLoading);
                    return;
                }
            }

            _assetsWaitForBundle.AddLastExt(AssetBundleWarehouse.LoadFailed(LoadingPattern,false, bundleName, assetName, ObjectType));
        }

        public void LoadScene(string bundleName, string assetName, bool isAdditive)
        {
            string bundleNameWithVariant;
            if (LoadAssetBundle(bundleName, out bundleNameWithVariant) != AssetBundleStatus.Failed)
            {
                var warehouse = FindWarehouse(bundleName, _defaultWarehouse);
                _assetsWaitForBundle.AddLastExt(warehouse.LoadScene(bundleNameWithVariant, assetName, isAdditive));
            }
            else
            {
                _assetsWaitForBundle.AddLastExt(AssetBundleWarehouse.LoadFailed(AssetLoadingPattern.Unkown,true, bundleName, assetName,null));
            }
        }

        private AssetBundleStatus LoadAssetBundle(string bundleName, out string bundleNameWithVariant)
        {
            AssetBundleLoading operation;

            var preferedWarehouse = _defaultWarehouse;
            var status = LoadAssetBundle(bundleName, _defaultWarehouse, out operation, out bundleNameWithVariant, out preferedWarehouse);

            if (status != AssetBundleStatus.Failed && _loadDependency)
            {
                if (status == AssetBundleStatus.Blank)
                {
                    if (_allDependencies.ContainsKey(operation.Name))
                    {
                        AssetBundleLoading dependency;
                        foreach (var v in _allDependencies[operation.Name])
                        {
                            string dependencyBundleNameWithVariant;
                            AssetBundleWarehouse dependentWarehouse;
                            preferedWarehouse = _defaultWarehouse;
                            status = LoadAssetBundle(v, preferedWarehouse, out dependency, out dependencyBundleNameWithVariant, out dependentWarehouse);
                            if (status == AssetBundleStatus.Blank || status == AssetBundleStatus.Loading)
                                operation.AddDependency(dependencyBundleNameWithVariant);
                        }
                    }
                }
            }

            return status;
        }

        private void SetManifest(AssetBundleManifest manifest)
        {
            foreach (var v in manifest.GetAllAssetBundles())
            {
                if (_allDependencies.ContainsKey(v))
                    throw new ArgumentException("duplicate assetbundle");
                
                _allDependencies.Add(v, manifest.GetAllDependencies(v));
            }
        }

        private AssetBundleWarehouse FindWarehouse(string bundleName, AssetBundleWarehouse defaultWarehouse)
        {
            var warehouse = defaultWarehouse;

            var baseName = Utility.GetNameWithoutVariant(bundleName);

            if (_supplementaryWarehouses.ContainsKey(baseName))
                warehouse = _supplementaryWarehouses[baseName];
//            else
//            {
//                foreach (var v in _supplementaryWarehouses)
//                {
//                    if (baseName.StartsWith(v.Key))
//                    {
//                        warehouse = v.Value;
//                        break;
//                    }
//                }
//            }

            return warehouse;
        }

        private AssetBundleStatus LoadAssetBundle(string bundleName, AssetBundleWarehouse defaultWarehouse, out AssetBundleLoading operation,
            out string nameWithVariant, out AssetBundleWarehouse preferedWarehouse)
        {
            preferedWarehouse = FindWarehouse(bundleName, defaultWarehouse);
            nameWithVariant = preferedWarehouse.RemapBundleName(bundleName);

            if (_loadedAssetBundles.ContainsKey(nameWithVariant))
            {
                operation = null;
                return AssetBundleStatus.Loaded;
            }

            if (_loadingAssetBundleNames.Contains(nameWithVariant))
            {
                operation = null;
                return AssetBundleStatus.Loading;
            }

            if (_failedAssetBundleNames.Contains(nameWithVariant))
            {
                operation = null;
                return AssetBundleStatus.Failed;
            }
            _logger.InfoFormat("LoadAssetBundle :{0}", nameWithVariant);

            _loadingAssetBundleNames.Add(nameWithVariant);
            
            operation = preferedWarehouse.LoadAssetBundle(nameWithVariant);
            _loadingAssetBundles.AddLast(operation);
 
            return AssetBundleStatus.Blank;
        }

        private void UpdateAssetBundleLoading()
        {
            var bundleItor = _loadingAssetBundles.First;
            while (bundleItor != null)
            {
                var nextBundle = bundleItor.Next;
                
                var operation = bundleItor.Value;
                if (operation.IsDone())
                {
                    operation.Process();

                    RemoveDependency(operation.Name);

                    _loadingAssetBundles.Remove(bundleItor);

                    if (string.IsNullOrEmpty(operation.Error))
                    {
                        _statRecorder.AssetBundleLoaded(operation.Name);
                        if (operation.NoMoreDependencies())
                        {
                            _loadingAssetBundleNames.Remove(operation.Name);
                            _loadedAssetBundles.Add(operation.Name, operation.Content);
                        }   
                        else
                            _assetBundleWaitForDependencies.AddLast(bundleItor);
                    }
                    else
                    {
                        _statRecorder.AssetBundleNotFound(operation.Name, operation.Error);
                        _loadingAssetBundleNames.Remove(operation.Name);
                        _failedAssetBundleNames.Add(operation.Name);
                    }
                }

                bundleItor = nextBundle;
            }           
        }

        private void RemoveDependency(string loadedBundleName)
        {
            var itor = _assetBundleWaitForDependencies.First;
            while (itor != null)
            {
                var next = itor.Next;

                itor.Value.RemoveDependency(loadedBundleName);
                if (itor.Value.NoMoreDependencies())
                {
                    _loadingAssetBundleNames.Remove(itor.Value.Name);
                    _loadedAssetBundles.Add(itor.Value.Name, itor.Value.Content);
                    _assetBundleWaitForDependencies.Remove(itor);
                }

                itor = next;
            }

            itor = _loadingAssetBundles.First;
            while (itor != null)
            {
                if (!itor.Value.NoMoreDependencies())
                    itor.Value.RemoveDependency(loadedBundleName);

                itor = itor.Next;
            }
        }

        private void UpdateAssetLoading()
        {
            UpdateLoadingAsset();
            if(_loadingAssets.Count==0)
                LoadWaitAsset();
          
        }

        private void UpdateLoadingAsset()
        {
            var assetItor = _loadingAssets.First;
            while (assetItor != null)
            {
                var next = assetItor.Next;

                var operation = assetItor.Value;
                if (operation.IsDone())
                {
                    operation.Process();
                    _loadingAssets.Remove(assetItor);

                    if (!operation.IsLoadFailed)
                    {
                        if (operation.IsSceneLoading)
                        {
                            _statRecorder.SceneLoaded(operation.BundleName, operation.Name);
                        }
                        else
                        {
                            _loadedAssets.AddLast(assetItor);
                            _statRecorder.AssetLoaded(operation.LoadingPattern, operation.BundleName, operation.Name);
                        }
                    }
                    else
                    {
                        if (operation.IsSceneLoading)
                        {
                            if (operation.IsLoadFailed)
                                _statRecorder.SceneNotFound(operation.BundleName, operation.Name);
                            else
                                _statRecorder.SceneLoaded(operation.BundleName, operation.Name);
                        }

                        else
                        {
                            _statRecorder.AssetNotFound(operation.LoadingPattern, operation.BundleName, operation.Name);
                            IfUseErrorType(operation); //是否使用了错误的类型进行加载//
                        }
                    }
                }
                assetItor = next;
            }
        }

        void IfUseErrorType(AssetLoading operation)
        {
            bool needLog = false;
            if (operation.LoadingPattern == AssetLoadingPattern.Simulation)
            {
#if UNITY_EDITOR
                string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(operation.BundleName, operation.Name);
                if (assetPaths != null && assetPaths.Length > 0)
                {
                    needLog = true;
                }
#endif
            }
            else
            {
                if (_loadedAssetBundles.ContainsKey(operation.BundleName))
                {
                    var loadedBundle = _loadedAssetBundles[operation.BundleName];
                    if (loadedBundle.Bundle != null)
                    {
                        if (loadedBundle.Bundle.Contains(operation.Name))
                        {
                            needLog = true;
                        }
                    }
                }
            }
            if (needLog)
            {
                _logger.WarnFormat("AssetBundles {0} Has {1} But You Use The Error Type {2} To Loader", operation.BundleName, operation.Name, operation.ObjectType);
            }
        }

        private bool LoadWaitAsset()
        {
            var assetItor = _assetsWaitForBundle.First;
            if (assetItor != null)
            {
                var next = assetItor.Next;

                var operation = assetItor.Value;

                if (_loadedAssetBundles.ContainsKey(operation.BundleName))
                {
                    try
                    {
                        operation.SetAssetBundle(_loadedAssetBundles[operation.BundleName]);
                    }
                    catch (Exception e)
                    {
                        _logger.Error(String.Format("Can not set asset bundle {0}", operation.BundleName), e);
                    }

                    _assetsWaitForBundle.Remove(assetItor);
                    _loadingAssets.AddLast(assetItor);
                }

                if (_failedAssetBundleNames.Contains(operation.BundleName))
                {
                    _assetsWaitForBundle.Remove(assetItor);

                    if (operation.IsSceneLoading)
                        _statRecorder.SceneNotFound(operation.BundleName, operation.Name);
                    else
                        _statRecorder.AssetNotFound(operation.LoadingPattern, operation.BundleName, operation.Name);
                }

                assetItor = next;
            }

            return assetItor != null;
        }

        public IEnumerator Clear()
        {
            _assetsWaitForBundle.Clear();
            _loadingAssets.Clear();
            _loadedAssets.Clear();

            _failedAssetBundleNames.Clear();
            _loadingAssetBundleNames.Clear();

            foreach (var v in _loadedAssetBundles)
            {
                v.Value.Bundle.Unload(true);
            }
            _loadedAssetBundles.Clear();

            foreach (var bundle in _loadingAssetBundles)
            {
                yield return bundle.Cancel();
            }
            _loadingAssetBundles.Clear();
        }


        public void Reset()
        {
            
            foreach (var v in _loadedAssetBundles)
            {
                if (v.Value != null && v.Value.Bundle != null) 
                    v.Value.Bundle.Unload(false);
            }

            _loadedAssetBundles.Clear();
        }
    }
}