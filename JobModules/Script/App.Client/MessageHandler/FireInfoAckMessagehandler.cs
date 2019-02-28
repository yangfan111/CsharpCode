using App.Client.Bullet;
using App.Client.Console.MessageHandler;
using App.Protobuf;
using App.Shared;

namespace App.Client.MessageHandler
{
    class FireInfoAckMessageHandler : AbstractClientMessageHandler<FireInfoAckMessage>
    {
        private ClientBulletInfoCollector _bulletInfoCollector;
        private PlayerContext _playerContext;
        public FireInfoAckMessageHandler(PlayerContext playerContext, ClientBulletInfoCollector bulletInfoCollector)
        {
            _playerContext = playerContext;
            _bulletInfoCollector = bulletInfoCollector;
        } 

        public override void DoHandle(int messageType, FireInfoAckMessage messageBody)
        {
            var playerEntity = _playerContext.flagSelfEntity;
            if(null != playerEntity)
            {
                if(SharedConfig.ShowHitFeedBack)
                {
                    playerEntity.tip.Content = string.Format("{0} missmatched", messageBody.Seq);
                }
            }
            _bulletInfoCollector.OnMissMatch(messageBody.Seq);
        }
    }
}
