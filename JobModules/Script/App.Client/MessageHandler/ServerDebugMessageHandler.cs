
using App.Client.Console.MessageHandler;
using App.Protobuf;
using App.Shared.GameModules.Attack;
using App.Shared.GameModules.Weapon;

namespace App.Client.MessageHandler
{
    class ServerDebugMessageHandler : AbstractClientMessageHandler<ServerDebugMessage>
    {
        private Contexts _contexts;
        public ServerDebugMessageHandler(Contexts contexts)
        {
            _contexts = contexts; 
        }

        public override void DoHandle(int messageType, ServerDebugMessage messageBody)
        {
            BulletStatisticsUtil.HandleFireInfo(messageBody.Content);
        }
    }
}
