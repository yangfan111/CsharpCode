using App.Shared;
using App.Shared.GameModules.Bullet;
using Core.Utils;
using WeaponConfigNs;

namespace App.Server.Damage
{
    public class ServerDamageInfoCollector : IDamageInfoCollector
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ServerDamageInfoCollector));
        private PlayerContext _playerContext;

        public ServerDamageInfoCollector(PlayerContext playerContext)
        {
            _playerContext = playerContext;
        }

        public void SetPlayerDamageInfo(PlayerEntity source, PlayerEntity target, float damage, EBodyPart part)
        {
            if(!source.hasPosition)
            {
                Logger.Error("damage source player has no position");
                return;
            }
            if(!source.hasEntityKey)
            {
                Logger.Error("damage source has no entity key");
                return;
            }
            if(!target.hasNetwork)
            {
                Logger.Error("damage target has no network component");
                return;
            }
            var pos = source.position.Value;

            var msg = Protobuf.PlayerDamageInfoMessage.Allocate();
            msg.EntityId = source.entityKey.Value.EntityId;
            msg.Damage = damage;
            msg.PosX = pos.x; 
            msg.PosZ = pos.z;
            SendMessage(msg, target);
        }

        public void SendMessage(Protobuf.PlayerDamageInfoMessage msg, PlayerEntity target)
        {
            target.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.DamageInfo, msg);
            Logger.DebugFormat("send damage info entityid :{0} damage :{1} posx : {2} posz: {3}", msg.EntityId, msg.Damage, msg.PosX, msg.PosZ);
        }
    }
}