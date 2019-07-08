using App.Shared.GameModules.Weapon;
using Core.GameModule.System;
using Core.Network;
using Entitas;

namespace App.Shared.GameModules.Attack
{
    public class ClientBulletRemoteCollectSystem : ReactiveEntityCleanUpSystem<BulletEntity>
    {
        private INetworkChannel _networkChannel;
        public ClientBulletRemoteCollectSystem(BulletContext bulletContext, INetworkChannel networkChannel) : base(bulletContext)
        {
            _networkChannel = networkChannel;
        }

        public override void SingleExecute(BulletEntity entity)
        {
            BulletStatisticsUtil.CollectBulletInfoC(entity,_networkChannel);
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
