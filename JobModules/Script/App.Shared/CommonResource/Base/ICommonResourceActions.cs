using Core.CommonResource;
using Entitas;

namespace App.Shared.CommonResource.Base
{
    public interface ICommonResourceActions
    {
        void OnLoadFailed(IEntity entity, AssetStatus status);

        void Recycle(AssetStatus status);

        void Init(IEntity entity, AssetStatus status);

        bool CanInit(IEntity entity, AssetStatus status);
    }
}