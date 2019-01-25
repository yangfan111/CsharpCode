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

        public Dictionary<AssetInfo, UnityObjectWrapper<GameObject>> LoadedAssets { get; private set; }

        protected MultiAssetComponent()
        {
            LoadedAssets = new Dictionary<AssetInfo, UnityObjectWrapper<GameObject>>(AssetInfo.AssetInfoComparer.Instance);
        }

        public virtual void Recycle(IGameObjectPool gameObjectPool)
        {
            foreach (var asset in LoadedAssets)
            {
                gameObjectPool.Add(asset.Value);
            }
        }

        public GameObject FirstAsset
        {
            get { return LoadedAssets.Count > 0 ? LoadedAssets.First().Value : null; }
        }

        public abstract int GetComponentId();

       
        public void Reset()
        {
            if (LoadedAssets == null)
                LoadedAssets =
                    new Dictionary<AssetInfo, UnityObjectWrapper<GameObject>>(AssetInfo.AssetInfoComparer.Instance);
            else
            {
                LoadedAssets.Clear();
            }
        }
    }
}