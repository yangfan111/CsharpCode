using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Protobuf;
using App.Shared;
using App.Shared.DebugHandle;
using App.Shared.Network;
using Core.Network;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.SessionState;
using Core.Utils;
using UnityEngine;

namespace App.Server.MessageHandler
{
    class DebugMessageHandler : AbstractServerMessageHandler<PlayerEntity, DebugCommandMessage>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(DebugMessageHandler));
        private ServerDebugCommandHandler _handler;
        private Contexts _contexts;
        private int _lastSeq = -1;
        private ServerRoom room;
        private SessionStateMachine _sessionStateMachine;

        public DebugMessageHandler(Contexts contexts, ServerRoom converter) : base(converter)
        {
            _contexts = contexts;
            room = converter;
            _handler = new ServerDebugCommandHandler(_contexts);
            _sessionStateMachine = converter.GetSessionStateMachine();
        }

        public override void DoHandle(INetworkChannel channel, PlayerEntity entity,
            EClient2ServerMessage eClient2ServerMessage, DebugCommandMessage commandMessageBody)
        {
            _logger.InfoFormat("Debug message", commandMessageBody.Command);
            DebugCommand cmd = new DebugCommand(commandMessageBody.Command, commandMessageBody.Args.ToArray());
            try
            {
                _handler.OnDebugMessage(room, cmd, entity, _sessionStateMachine);
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Error while exe debug command {0} {1} {2}", commandMessageBody, e.ToString(), e.StackTrace);
            }
        } 
    
    }
}
