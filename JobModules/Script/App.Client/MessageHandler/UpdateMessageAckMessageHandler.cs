using App.Client.Console.MessageHandler;
using App.Protobuf;
using Core.UpdateLatest;

namespace App.Client.MessageHandler
{
    public class UpdateMessageAckMessageHandler:AbstractClientMessageHandler<Protobuf.UpdateMessageAck>
    {
        private IUpdateLatestHandler _updateLatestHandler;

        public UpdateMessageAckMessageHandler(IUpdateLatestHandler updateLatestHandler)
        {
            _updateLatestHandler = updateLatestHandler;
        }

        public override void DoHandle(int messageType, UpdateMessageAck messageBody)
        {
            _updateLatestHandler.BaseUserCmdSeq = messageBody.AckSeq;
           
        }
    }
}