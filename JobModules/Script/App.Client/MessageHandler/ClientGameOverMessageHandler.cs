﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.Console;
using App.Client.Console.MessageHandler;
using App.Client.Utility;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Utils;

namespace App.Client.MessageHandler
{
    public class ClientGameOverMessageHandler : AbstractClientMessageHandler<GameOverMessage>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientGameOverMessageHandler));

        public override void DoHandle(int messageType, GameOverMessage messageBody)
        {
            _logger.Info("Client Receive GameOver Message From Server");
            //##TODO show player pose after game
            //HallUtility.GameOver();
        }
    }
}
