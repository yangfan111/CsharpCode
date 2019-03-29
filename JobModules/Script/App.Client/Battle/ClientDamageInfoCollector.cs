using App.Shared.GameModules.Bullet;
using Core.Utils;
using WeaponConfigNs;

namespace App.Client.Battle
{
    public class ClientDamageInfoCollector : IDamageInfoCollector
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(ClientDamageInfoCollector));
        private PlayerContext _playerContext;
        public ClientDamageInfoCollector(PlayerContext playerContext)
        {
            _playerContext = playerContext;
        }

        public void SetPlayerDamageInfo(PlayerEntity source, PlayerEntity target, float damage, EBodyPart part)
        {
            if(Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("SetPlayerDamageInfo {0} in {1}", damage, part);
            }
            var myPlayerEntity = _playerContext.flagSelfEntity;
            if(source != myPlayerEntity)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.DebugFormat("source is not myself diff {0} {1}", source.entityKey.Value, myPlayerEntity.entityKey.Value);
                }
                return;
            }
            if(!myPlayerEntity.hasAttackDamage)
            {
                myPlayerEntity.AddAttackDamage(part, damage);
            }
            else
            {
                myPlayerEntity.attackDamage.HitPart = part;
                myPlayerEntity.attackDamage.Damage = damage;
            }
        }
    }
}
