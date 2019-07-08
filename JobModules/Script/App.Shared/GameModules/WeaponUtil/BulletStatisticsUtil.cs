using System;
using App.Protobuf;
using App.Shared.Network;
using Core.Network;
using Core.Utils;
using WeaponConfigNs;
using Vector3 = UnityEngine.Vector3;

namespace App.Shared.GameModules.Weapon
{
    public static class BulletStatisticsUtil
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BulletStatisticsUtil));
        public static void SetPlayerDamageInfoC(PlayerEntity source, PlayerEntity target, float damage, EBodyPart part)
        {
            if(Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("SetPlayerDamageInfo {0} in {1}", damage, part);
            }
            if(!source.isFlagSelf)
            {
                return;
            }
            if(!source.hasAttackDamage)
            {
                source.AddAttackDamage(part, damage);
            }
            else
            {
                source.attackDamage.HitPart = part;
                source.attackDamage.Damage  = damage;
            }
        }
        public static void SetPlayerDamageInfoS(PlayerEntity source, PlayerEntity target, float damage, EBodyPart part)
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

            var msg = PlayerDamageInfoMessage.Allocate();
            msg.EntityId = source.entityKey.Value.EntityId;
            msg.Damage   = damage;
            msg.PosX     = pos.x; 
            msg.PosZ     = pos.z;
            SendMessage(msg, target);
        }
        private static void SendMessage(PlayerDamageInfoMessage msg, PlayerEntity target)
        {
            target.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.DamageInfo, msg);
            Logger.DebugFormat("send damage info entityid :{0} damage :{1} posx : {2} posz: {3}", msg.EntityId, msg.Damage, msg.PosX, msg.PosZ);
        }
        public static void CollectBulletInfoC(BulletEntity entity, INetworkChannel networkChannel)
        {
            if (networkChannel == null)
                return;
            var cmd     = entity.bulletData.CmdSeq;
            var start   = entity.bulletData.StartPoint;
            var emit    = entity.bulletData.EmitPoint;
            var dir     = entity.bulletData.StartDir;
            var hit     = entity.bulletData.HitPoint;
            var hitType = entity.bulletData.HitType;
            var msg     = FireInfoMessage.Allocate();
            msg.Seq        = cmd;
            msg.StartPoint = Vector3Converter.UnityToProtobufVector3(start);
            msg.EmitPoint  = Vector3Converter.UnityToProtobufVector3(emit);
            msg.StartDir   = Vector3Converter.UnityToProtobufVector3(dir);
            msg.HitPoint   = Vector3Converter.UnityToProtobufVector3(hit);
            msg.HitType    = (int) hitType;
            networkChannel.SendReliable((int) EClient2ServerMessage.FireInfo, msg);
            msg.ReleaseReference();
        }
        [Obsolete]
        public static void CollectBulletInfoS(BulletEntity entity, INetworkChannel networkChannel)
        {
            
        }
        
        [Obsolete]
        public static void HandleMissMatch(int cmdSeq)
        {
        }

        [Obsolete]
        public static void HandleFireInfo(string content)
        {
        }

        [Obsolete]
        public static void HandleClientFireInfoToSrv(int messageBodySeq, Vector3 protobufToUnityVector3, Vector3 vector3,
                                           Vector3 protobufToUnityVector4, Vector3 vector4, int messageBodyHitType,
                                           INetworkChannel channel)
        {
        }
    }
}