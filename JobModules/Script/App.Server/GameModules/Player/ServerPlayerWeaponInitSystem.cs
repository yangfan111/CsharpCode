using App.Shared;

using Core.GameModule.System;
using Core.Utils;
using Entitas;

namespace App.Server.GameModules.Player
{
    public class ServerPlayerWeaponInitSystem : ReactiveEntityInitSystem<PlayerEntity>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ServerPlayerWeaponInitSystem));
        
        private ICommonSessionObjects _commonSessionObjects;
        public ServerPlayerWeaponInitSystem(IContext<PlayerEntity> playerContext, 
            ICommonSessionObjects commonSessionObjects) : base(playerContext)
        {
            _commonSessionObjects = commonSessionObjects;
        }

        public override void SingleExecute(PlayerEntity playerEntity)
        {
            playerEntity.ModeController().RecoverPlayerWeapon(playerEntity);
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return entity.hasThirdPersonModel && entity.hasFirstPersonModel;
        }

        protected override ICollector<PlayerEntity> GetTrigger(IContext<PlayerEntity> context)
        {
            return context.CreateCollector(PlayerMatcher.ThirdPersonModel.Added(), PlayerMatcher.FirstPersonModel.Added());
        }
    }
}
