using App.Client.Console.MessageHandler;
using App.Protobuf;
using Core.UpdateLatest;

namespace App.Client.MessageHandler
{
    public class UpdateMessageAckMessageHandler:AbstractClientMessageHandler<Protobuf.UpdateMessageAck>
    {
        private ClientUpdateLatestManager _clientUpdateLatestHandler;

        public UpdateMessageAckMessageHandler(ClientUpdateLatestManager clientUpdateLatestHandler)
        {
            _clientUpdateLatestHandler = clientUpdateLatestHandler;
        }

        public override void DoHandle(int messageType, UpdateMessageAck messageBody)
        {
            _clientUpdateLatestHandler.LastAckUserCmdSeq = messageBody.AckSeq;
           
        }
    }
}