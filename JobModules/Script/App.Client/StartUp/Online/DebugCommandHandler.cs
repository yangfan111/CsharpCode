using App.Protobuf;
using App.Shared;
using App.Shared.DebugHandle;
using Core.SessionState;

namespace App.Client.Console
{
    public class DebugCommandHandler : IDebugCommandHandler
    {
        private readonly ClientDebugCommandHandler _clientDebugCommandHandler;
        private readonly Contexts _contexts;
        private readonly SessionStateMachine _clientSessionStateMachine;

        public DebugCommandHandler(SessionStateMachine clientSessionStateMachine, Contexts contexts)
        {
            _clientSessionStateMachine = clientSessionStateMachine;
            _contexts = contexts;
            _clientDebugCommandHandler = new ClientDebugCommandHandler(contexts);
        }

        public string OnDebugMessage(DebugCommand message)
        {
            var channel = _contexts.session.clientSessionObjects.NetworkChannel;
            if (channel != null)
            {
                var msg = DebugCommandMessage.Allocate();
                msg.Command = message.Command;
                if (message.Args != null && message.Command != DebugCommands.TestMap &&
                    message.Command != DebugCommands.ClientMove)
                {
                    msg.Args.AddRange(message.Args);
                }

                channel.SendReliable((int) EClient2ServerMessage.DebugCommand, msg);
                msg.ReleaseReference();
            }

            return _clientDebugCommandHandler.OnDebugMessage(message, _clientSessionStateMachine);
        }
    }
}