using App.Protobuf;
using App.Shared;
using App.Shared.GameModules.Attack;
using App.Shared.GameModules.Weapon;
using App.Shared.Network;
using Core.Network;

namespace App.Server.MessageHandler
{
    public class FireInfoMessageHandler : AbstractServerMessageHandler<PlayerEntity, FireInfoMessage>
    {

        public FireInfoMessageHandler(Contexts contexts, IPlayerEntityDic<PlayerEntity> converter) : base(converter)
        {
        }

        public override void DoHandle(INetworkChannel channel, PlayerEntity entity, EClient2ServerMessage eClient2ServerMessage, FireInfoMessage messageBody)
        {
            BulletStatisticsUtil.HandleClientFireInfoToSrv(messageBody.Seq, 
                Vector3Converter.ProtobufToUnityVector3(messageBody.StartPoint), 
                Vector3Converter.ProtobufToUnityVector3(messageBody.EmitPoint), 
                Vector3Converter.ProtobufToUnityVector3(messageBody.StartDir),
                Vector3Converter.ProtobufToUnityVector3(messageBody.HitPoint), 
                messageBody.HitType,
                channel);
        }
    }
}
