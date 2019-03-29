using App.Protobuf;
using App.Server.Bullet;
using App.Shared;
using App.Shared.Network;
using Core.Network;

namespace App.Server.MessageHandler
{
    public class FireInfoMessageHandler : AbstractServerMessageHandler<PlayerEntity, FireInfoMessage>
    {
        private ServerBulletInfoCollector _bulletInfoCollector;

        public FireInfoMessageHandler(Contexts contexts, IPlayerEntityDic<PlayerEntity> converter) : base(converter)
        {
            _bulletInfoCollector = contexts.session.commonSession.BulletInfoCollector as ServerBulletInfoCollector;
        }

        public override void DoHandle(INetworkChannel channel, PlayerEntity entity, EClient2ServerMessage eClient2ServerMessage, FireInfoMessage messageBody)
        {
            _bulletInfoCollector.AddClientBulletData(messageBody.Seq, 
                Vector3Converter.ProtobufToUnityVector3(messageBody.StartPoint), 
                Vector3Converter.ProtobufToUnityVector3(messageBody.EmitPoint), 
                Vector3Converter.ProtobufToUnityVector3(messageBody.StartDir),
                Vector3Converter.ProtobufToUnityVector3(messageBody.HitPoint), 
                messageBody.HitType,
                channel);
        }
    }
}
