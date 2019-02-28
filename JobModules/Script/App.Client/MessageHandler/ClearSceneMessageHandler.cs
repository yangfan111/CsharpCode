using App.Client.Console.MessageHandler;
using App.Protobuf;

namespace App.Client.MessageHandler
{
    class ClearSceneMessageHandler : AbstractClientMessageHandler<ClearSceneMessage>
    {
        private ClientEffectContext _clientEffectContext;
        private ThrowingContext _throwingContext;
        public ClearSceneMessageHandler(Contexts contexts)
        {
            _clientEffectContext = contexts.clientEffect;
            _throwingContext = contexts.throwing;
        }

        public override void DoHandle(int messageType, ClearSceneMessage messageBody)
        {
            foreach(var entity in _clientEffectContext.GetEntities())
            {
                entity.isFlagDestroy = true;
            }
            foreach(var entity in _throwingContext.GetEntities())
            {
                entity.isFlagDestroy = true;
            }
        }
    }
}
