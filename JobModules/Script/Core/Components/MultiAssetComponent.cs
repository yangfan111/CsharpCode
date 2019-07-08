using System.Collections.Generic;
using System.Linq;
using Utils.AssetManager;
using Core.EntityComponent;
using Core.ObjectPool;
using UnityEngine;
using Core.Utils;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Core.Components
{
    public abstract class MultiAssetComponent : IGameComponent, IAssetComponent, IResetableComponent
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(MultiAssetComponent));

        [DontInitilize] public Dictionary<AssetInfo, UnityObject> LoadedAssets { get; private set; }

        public void CacheAssetObject(UnityObject unityObject)
        {
            loadingAssetInfos.Remove(unityObject.Address);
            LoadedAssets[unityObject.Address] = unityObject;
        }
        

        public HashSet<AssetInfo> loadingAssetInfos { get; private set; }

        public bool AddLoadingAssets(AssetInfo assetInfo)
        {
            UnityObject uo;
            LoadedAssets.TryGetValue(assetInfo, out uo);
            if (uo == null)
            {
                loadingAssetInfos.Add(assetInfo);
                return true;
            }

            return false;
        }
        
        protected MultiAssetComponent()
        {
            loadingAssetInfos = new HashSet<AssetInfo>();
            LoadedAssets      = new Dictionary<AssetInfo, UnityObject>(AssetInfo.AssetInfoComparer.Instance);
        }
        
        public void Prepare()
        {
            loadingAssetInfos.Clear();
            
        }


        public virtual void Recycle(IUnityAssetManager assetManager)
        {
            foreach (var asset in LoadedAssets)
            {
                if (asset.Value != null)
                    assetManager.Recycle(asset.Value);
            }
        }

        public GameObject FirstAsset
        {
            get
            {
                if (LoadedAssets.Count > 0)
                {
                    var first = LoadedAssets.First();
                    if (first.Value != null)
                        return first.Value.AsGameObject;
                }

                return null;
            }
        }

        public abstract int GetComponentId();

        public void Reset()
        {
            LoadedAssets.Clear();
            loadingAssetInfos.Clear();
        }
    }
}