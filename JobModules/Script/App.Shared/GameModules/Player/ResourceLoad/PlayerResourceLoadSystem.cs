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
        private readonly AssetsAsyncCallPool _assetsAsyncCallPool = new AssetsAsyncCallPool();
        private readonly List<AssetInfo> _assetInfos = new List<AssetInfo>();
        
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
            
            var audioController = player.AudioController();
            if (audioController != null)
                audioController.LoadMapAmbient(AssetManager);
            Logger.InfoFormat("CharacterLog-- created client player entity {0}, id:{1}, avatarIds:{2}", player.entityKey,
                player.playerInfo.RoleModelId,
                string.Join(",", player.playerInfo.AvatarIds.Select(i => i.ToString()).ToArray()));
        }

        public override void OnLoadResources(IUnityAssetManager assetManager)
        {
            base.OnLoadResources(assetManager);
            foreach (var entity in _player.GetEntities())
            {
                if (!entity.hasAppearanceInterface) continue;
                var loadRequests = entity.appearanceInterface.Appearance.GetLoadRequests();
                if (loadRequests.Count > 0)
                {
                    var call = _assetsAsyncCallPool.Get();
                    call.CreateCallFuncMapping(entity, loadRequests);
                    CreateAssetInfos(loadRequests);
                    assetManager.LoadAssetsAsync(entity, _assetInfos, 
                        call.CallFunc); 
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

        private void CreateAssetInfos(List<AbstractLoadRequest> loadRequests)
        {
            _assetInfos.Clear();
            foreach (var request in loadRequests)
                _assetInfos.Add(request.AssetInfo);
        }

        private class AssetsAsyncCall
        {
            private readonly AssetsAsyncCallPool _assetsAsyncCallPool;
            private readonly InterceptPool _interceptPool;
            private readonly List<Action<PlayerEntity, UnityObject>> _list = 
                new List<Action<PlayerEntity, UnityObject>>();

            public AssetsAsyncCall(AssetsAsyncCallPool assetsAsyncCallPool, InterceptPool interceptPool)
            {
                _assetsAsyncCallPool = assetsAsyncCallPool;
                _interceptPool = interceptPool;
            }
            
            public void CreateCallFuncMapping(PlayerEntity player, List<AbstractLoadRequest> loadRequests)
            {
                if (null == loadRequests)
                {
                    Logger.ErrorFormat("loadRequestsList is Null");
                    return;
                }
                
                foreach (var request in loadRequests)
                {
                    var intercept = _interceptPool.Get();
                    intercept.SetParam(player, request.GetHandler<PlayerEntity>());
                    _list.Add(intercept.Call);
                }
            }
            
            public void CallFunc(PlayerEntity player, List<UnityObject> objects)
            {
                if(objects.Count != _list.Count)
                    Logger.ErrorFormat("Batch Load UnityObject Number Not Match Request  RequestsCount:{0}, objectsCount:{1}",
                        _list.Count, objects.Count);
                
                for (var i = 0; i < objects.Count; ++i)
                {
                    if (_list.Count <= i || null == _list[i]) continue;
                    _list[i].Invoke(player, objects[i]);
                    _list[i] = null;
                }

                // Free Remain PoolObj
                foreach (var action in _list)
                {
                    if(null == action) continue;
                    action.Invoke(player, null);
                }
                
                _list.Clear();
                _assetsAsyncCallPool.Free(this);
            }
        }
        
        private class AssetsAsyncCallPool
        {
            private readonly Queue<AssetsAsyncCall> _pool = new Queue<AssetsAsyncCall>();
            private readonly InterceptPool _interceptPool = new InterceptPool();
            
            public AssetsAsyncCall Get()
            {
                if (_pool.Count <= 0)
                    return new AssetsAsyncCall(this, _interceptPool);
                
                return _pool.Dequeue();
            }

            public void Free(AssetsAsyncCall item)
            {
                _pool.Enqueue(item);
            }
        }

        private static bool HasFirstPerson(int roleId)
        {
            return SingletonManager.Get<RoleConfigManager>().GetRoleItemById(roleId).HasFirstPerson;
        }
        
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
                _handler.Invoke(player, unityObj);
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
