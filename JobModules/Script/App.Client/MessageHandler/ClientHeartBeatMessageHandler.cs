using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.Console.MessageHandler;
using App.Protobuf;

namespace App.Client.MessageHandler
{
    public class ClientHeartBeatMessageHandler : AbstractClientMessageHandler<HeartBeatMessage>
    {
        public override void DoHandle(int messageType, HeartBeatMessage messageBody)
        {
            //Do Nothing
            //UnityEngine.Debug.Log("Receive HearBeat Message!");
        }
    }
}
