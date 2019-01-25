using App.Shared.EntityFactory;
using Core.GameModule.System;
using Core.Utils;
using Entitas;

namespace App.Shared.GameModules.Player
{
    public class PlayerEntityInitSystem : ReactiveEntityInitSystem<PlayerEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerEntityInitSystem));
        private Contexts _contexts;
        
        public PlayerEntityInitSystem(Contexts contexts) : base(contexts.player)
        {
            _contexts = contexts;
        }

        protected override ICollector<PlayerEntity> GetTrigger(IContext<PlayerEntity> context)
        {
            // 只针对预测或服务端entity
            return context.CreateCollector(PlayerMatcher.UserCmdSeq.Added());
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return true;
        }

        public override void SingleExecute(PlayerEntity player)
        {
            PlayerEntityFactory.PostCreateNewPlayerEntity(
                player,
                _contexts);
            _logger.InfoFormat("created client player entity {0}", player.entityKey);
        }
    }
}

