using Utils.AssetManager;
using Core.EntityComponent;
using UnityEngine;

namespace Core.Components
{
    public interface IAssetComponent: IGameComponent
    {
        void Recycle(IGameObjectPool gameObjectPool);
    }

    public abstract class SingleAssetComponent : IAssetComponent, IGameComponent
    {
        public AssetInfo AssetInfo { get; set; }
        public UnityObjectWrapper<GameObject> UnityObjWrapper { get; set; }
        public virtual void Recycle(IGameObjectPool gameObjectPool)
        {
            if (UnityObjWrapper != null)
            {
                gameObjectPool.Add(UnityObjWrapper);
            }
        }

        public abstract int GetComponentId();
    }
}