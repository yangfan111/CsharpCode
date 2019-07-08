using App.Shared.GameModules.Attack;
using App.Shared.GameModules.Weapon;
using Core.GameModule.System;
using Core.Network;
using Entitas;

namespace App.Shared.GameModules.Bullet
{
    public class ServerBulletInfoCollectSystem : ReactiveEntityCleanUpSystem<BulletEntity>
    {
        private PlayerContext _playerContext;
        public ServerBulletInfoCollectSystem(BulletContext bulletContext, PlayerContext playerContext) : base(bulletContext)
        {
            _playerContext       = playerContext;
        }

        public override void SingleExecute(BulletEntity entity)
        {
            CollectBulletInfo(entity);
        }

        private void CollectBulletInfo(BulletEntity entity)
        {
            var player = _playerContext.GetEntityWithEntityKey(entity.ownerId.Value);
            if (null == player)
            {
                entity.isFlagDestroy = true;
                return;
            }
            BulletStatisticsUtil.CollectBulletInfoS(entity, player.network.NetworkChannel);
        }

        protected override bool Filter(BulletEntity entity)
        {
            return entity.hasBulletData;
        }

        protected override ICollector<BulletEntity> GetTrigger(IContext<BulletEntity> context)
        {
            return context.CreateCollector(BulletMatcher.FlagDestroy.Added());
        }
    }
}