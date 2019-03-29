using System;
using Core.CommonResource;
using Entitas;

namespace App.Shared.CommonResource.Base
{
    public class CommonResourceActions : ICommonResourceActions
    {
        private readonly Func<IEntity, AssetStatus, bool> _canInitFunc;
        private readonly Action<IEntity, AssetStatus> _initFunc;
        private readonly Action<IEntity, AssetStatus> _onLoadFailed;
        private readonly Action<AssetStatus> _recycle;

        public CommonResourceActions(Func<IEntity, AssetStatus, bool> canInitFunc, Action<IEntity, AssetStatus> initFunc, Action<AssetStatus> recycle, Action<IEntity, AssetStatus> onLoadFailed)
        {
            _canInitFunc = canInitFunc;
            _initFunc = initFunc;
            _recycle = recycle;
            _onLoadFailed = onLoadFailed;
        }

        public void OnLoadFailed(IEntity entity, AssetStatus status)
        {
            _onLoadFailed(entity, status);
        }

        public void Recycle(AssetStatus status)
        {
            _recycle(status);
        }

        public void Init(IEntity entity, AssetStatus status)
        {
            _initFunc(entity, status);
        }

        public bool CanInit(IEntity entity, AssetStatus status)
        {
            return _canInitFunc(entity, status);
        }

        public static void DefaultOnLoadFailed(IEntity entity, AssetStatus status)
        {
        }

        public static void DefaultRecycle(AssetStatus status)
        {
        }

        public static void DefaultInitFunc(IEntity entity, AssetStatus status)
        {
        }

        public static bool DefaultCanInitFunc(IEntity entity, AssetStatus status)
        {
            return true;
        }
    }
}