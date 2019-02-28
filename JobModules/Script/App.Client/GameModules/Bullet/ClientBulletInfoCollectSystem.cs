using Core.BulletSimulation;
using Core.GameModule.System;
using Core.Network;
using Entitas;

namespace App.Shared.GameModules.Bullet
{
    public class ClientBulletInfoCollectSystem : ReactiveEntityCleanUpSystem<BulletEntity>
    {
        private IBulletInfoCollector _bulletInfoCollector;
        private INetworkChannel _networkChannel;
        public ClientBulletInfoCollectSystem(BulletContext bulletContext, INetworkChannel networkChannel, IBulletInfoCollector bulletInfoCollector) : base(bulletContext)
        {
            _bulletInfoCollector = bulletInfoCollector;
            _networkChannel = networkChannel;
        }

        public override void SingleExecute(BulletEntity entity)
        {
            CollectBulletInfo(entity);
        }

        private void CollectBulletInfo(BulletEntity entity)
        {
            if(null == _bulletInfoCollector)
            {
                return;
            }
            var cmd = entity.bulletData.CmdSeq;
            var start = entity.bulletData.StartPoint;
            var emit = entity.bulletData.EmitPoint;
            var dir = entity.bulletData.StartDir;
            var hit = entity.bulletData.HitPoint;
            var hitType = entity.bulletData.HitType;
            _bulletInfoCollector.AddBulletData(cmd, start, emit, dir, hit, (int)hitType, _networkChannel);
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
