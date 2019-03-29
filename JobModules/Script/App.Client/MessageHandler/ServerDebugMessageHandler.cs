using App.Client.Bullet;
using App.Client.Console.MessageHandler;
using App.Protobuf;

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
            var collector = _contexts.session.commonSession.BulletInfoCollector as ClientBulletInfoCollector;
            if(null != collector)
            {
                collector.SetServerFireInfo(messageBody.Content);
            }
        }
    }
}
