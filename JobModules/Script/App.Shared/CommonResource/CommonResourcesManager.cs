using System.Collections.Generic;
using App.Shared.CommonResource.Updaters;
using App.Shared.Components.Player;
using Shared.Scripts;
using Utils.Appearance;
using Utils.AssetManager;

namespace App.Shared.CommonResource
{
    public class CommonResourcesManager
    {
        private readonly Queue<RemoveAssetStatus> _queue = new Queue<RemoveAssetStatus>(32);

        public CommonResourcesManager(Contexts contexts)
        {
            _playerLogic = new PlayerCommonResourceUpdate(contexts, _queue);
        }

        public void Update(IUnityAssetManager assetManager)
        {
            _playerLogic.Update(assetManager);
        }

        #region Player

        private readonly ICommonResourceUpdate<PlayerEntity> _playerLogic;

        public void LoadResource(PlayerEntity entity, EPlayerCommonResourceType index, AssetInfo assetInfo, bool forceReload = false)
        {
            _playerLogic.LoadResource(entity, (int) index, assetInfo, forceReload);
        }

        public void UnloadResource(PlayerEntity entity, EPlayerCommonResourceType index)
        {
            _playerLogic.UnloadResource(entity, (int) index);
        }

        public void LoadResource(PlayerEntity entity, Wardrobe index, AssetInfo assetInfo, bool forceReload = false)
        {
            _playerLogic.LoadResource(entity, index.GetCommonResourceIndex(), assetInfo, forceReload);
        }

        public void UnloadResource(PlayerEntity entity, Wardrobe index)
        {
            _playerLogic.UnloadResource(entity, index.GetCommonResourceIndex());
        }

        public void LoadResource(PlayerEntity entity, LatestWeaponStateIndex index, AssetInfo assetInfo, bool forceReload = false)
        {
            _playerLogic.LoadResource(entity, index.GetCommonResourceIndex(), assetInfo, forceReload);
        }

        public void UnloadResource(PlayerEntity entity, LatestWeaponStateIndex index)
        {
            _playerLogic.UnloadResource(entity, index.GetCommonResourceIndex());
        }

        #endregion
    }
}