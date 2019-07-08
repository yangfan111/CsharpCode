using App.Client.Console.MessageHandler;
using App.Protobuf;
using App.Shared.GameModules.Attack;
using App.Shared.GameModules.Weapon;

namespace App.Client.MessageHandler
{
    class FireInfoAckMessageHandler : AbstractClientMessageHandler<FireInfoAckMessage>
    {

        public override void DoHandle(int messageType, FireInfoAckMessage messageBody)
        {
            BulletStatisticsUtil.HandleMissMatch(messageBody.Seq);
        }
    }
}
