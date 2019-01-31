using Utils.AssetManager;
using Core.EntityComponent;
using UnityEngine;

namespace Core.Components
{
    public interface IAssetComponent: IGameComponent
    {
        void Recycle(IUnityAssetManager assetManager);
    }

    public abstract class SingleAssetComponent : IAssetComponent, IGameComponent
    {
        public UnityObject UnityObject { get; set; }
        public virtual void Recycle(IUnityAssetManager assetManager)
        {
            if (UnityObject != null)
            {
                assetManager.Recycle(UnityObject);
            }
        }

        public abstract int GetComponentId();
    }
}