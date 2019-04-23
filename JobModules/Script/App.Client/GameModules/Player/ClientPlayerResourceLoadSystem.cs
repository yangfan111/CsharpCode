using Utils.AssetManager;
using Core.GameModule.System;
using Core.Utils;
using Entitas;
using Object = UnityEngine.Object;


namespace App.Shared.GameModules.Player
{
    public class ClientPlayerResourceLoadSystem : ReactiveResourceLoadSystem<PlayerEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientPlayerResourceLoadSystem));


        public ClientPlayerResourceLoadSystem(PlayerContext contexts) : base(contexts)
        {
        }


        protected override ICollector<PlayerEntity> GetTrigger(IContext<PlayerEntity> context)
        {
            return context.CreateCollector(PlayerMatcher.Position.Added());
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return entity.isFlagSelf;
        }

        public override void SingleExecute(PlayerEntity player)
        {
           
            AssetManager.LoadAssetAsync(player,
                AssetConfig.GetCharacterModelAssetInfo(player.playerInfo.RoleModelId),
                (new ModelLoadResponseHandler()).OnLoadSucc);
            _logger.InfoFormat("created client player entity {0}", player.entityKey);
        }


        class ModelLoadResponseHandler 
        {
            public ModelLoadResponseHandler()
            {
            }

            public void OnLoadSucc(object source, UnityObject unityObj)
            {
            }
        }
    }
}
