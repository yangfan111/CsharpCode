using System;
using System.Collections.Generic;
using System.Linq;
using Core.GameModule.System;
using Core.Utils;
using Entitas;
using App.Shared.Player;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameModules.Player.ResourceLoad
{
    public class PlayerResourceLoadSystem : ReactiveResourceLoadSystem<PlayerEntity>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerResourceLoadSystem));
        private readonly PlayerContext _player;
        private readonly FirstPersonModelLoadHandler _p1Handler;
        private readonly ThirdPersonModelLoadHandler _p3Handler;
        private readonly InterceptPool _interceptPool = new InterceptPool();

        public PlayerResourceLoadSystem(Contexts contexts) : base(contexts.player)
        {
            _player = contexts.player;
            _p1Handler = new FirstPersonModelLoadHandler(contexts);
            _p3Handler = new ThirdPersonModelLoadHandler(contexts);
        }

        protected override ICollector<PlayerEntity> GetTrigger(IContext<PlayerEntity> context)
        {
            return context.CreateCollector(PlayerMatcher.Position.Added());
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return !entity.hasThirdPersonModel && !entity.hasFirstPersonModel;
        }

        public override void SingleExecute(PlayerEntity player)
        {
            AssetManager.LoadAssetAsync(
                player,
                AssetConfig.GetCharacterModelAssetInfo(player.playerInfo.RoleModelId),
                _p3Handler.OnLoadSucc);

            if (player.isFlagSelf && HasFirstPerson(player.playerInfo.RoleModelId))
            {
                AssetManager.LoadAssetAsync(
                    player,
                    AssetConfig.GetCharacterHandAssetInfo(player.playerInfo.RoleModelId),
                    _p1Handler.OnLoadSucc);
            }

            Logger.InfoFormat("created client player entity {0}, id:{1}, avatarIds:{2}", player.entityKey,
                player.playerInfo.RoleModelId,
                string.Join(",", player.playerInfo.AvatarIds.Select(i => i.ToString()).ToArray()));
        }

        public override void OnLoadResources(IUnityAssetManager assetManager)
        {
            base.OnLoadResources(assetManager);
            foreach (var entity in _player.GetEntities())
            {
                if (entity.hasAppearanceInterface)
                {
                    var loadRequests = entity.appearanceInterface.Appearance.GetLoadRequests();
                    foreach (var request in loadRequests)
                    {
                        var intercept = _interceptPool.Get();
                        intercept.SetParam(entity, request.GetHandler<PlayerEntity>());
                        assetManager.LoadAssetAsync(entity, request.AssetInfo, intercept.Call);
                    }

                    var recycleRequests = entity.appearanceInterface.Appearance.GetRecycleRequests();
                    foreach (var request in recycleRequests)
                    {
                        entity.RemoveAsset(request);
                        assetManager.Recycle(request);
                    }

                    entity.appearanceInterface.Appearance.ClearRequests();
                }
            }
        }

        private static bool HasFirstPerson(int roleId)
        {
            return SingletonManager.Get<RoleConfigManager>().GetRoleItemById(roleId).HasFirstPerson;
        }

        private void ChangeRole(PlayerEntity player, int roleId)
        {
            if(player.hasPlayerInfo)
                player.playerInfo.ChangeNewRole(roleId);
            
            SingleExecute(player);
        }
        
        //private void Clear
        
        private class LoadResourceIntercept
        {
            private readonly InterceptPool _pool;

            private PlayerEntity _player;
            private Action<PlayerEntity, UnityObject> _handler;

            public LoadResourceIntercept(InterceptPool pool)
            {
                _pool = pool;
            }

            public void SetParam(PlayerEntity player, Action<PlayerEntity, UnityObject> handler)
            {
                _player = player;
                _handler = handler;
            }

            public void Call(PlayerEntity player, UnityObject unityObj)
            {
                _player.AddAsset(unityObj);
                _handler(player, unityObj);
                _pool.Free(this);
            }
        }

        private class InterceptPool
        {
            private readonly Queue<LoadResourceIntercept> _pool = new Queue<LoadResourceIntercept>();

            public LoadResourceIntercept Get()
            {
                if (_pool.Count <= 0)
                    return new LoadResourceIntercept(this);

                return _pool.Dequeue();
            }

            public void Free(LoadResourceIntercept item)
            {
                _pool.Enqueue(item);
            }
        }
    }
}
