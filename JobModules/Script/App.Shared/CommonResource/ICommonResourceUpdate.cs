using Entitas;
using Utils.AssetManager;

namespace App.Shared.CommonResource
{
    public interface ICommonResourceUpdate<in TEntity> where TEntity : class, IEntity
    {
        void Update(IUnityAssetManager assetManager);
        void LoadResource(TEntity entity, int index, AssetInfo assetInfo, bool forceReload);
        void UnloadResource(TEntity entity, int index);
    }
}