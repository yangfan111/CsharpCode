using App.Client.GameModules.Ui;
using App.Shared.DebugHandle;
using Core.SessionState;
using Core.Utils;

namespace App.Client
{
    public class ClientDebugCommandHandler
    {
        private Contexts _contexts;
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(ClientDebugCommandHandler));
        
        public ClientDebugCommandHandler(Contexts contexts)
        {
            _contexts = contexts;
        }

        public string OnDebugMessage(DebugCommand message,SessionStateMachine stateMachine)
        {
            var sb = new System.Text.StringBuilder();
            foreach(var arg in message.Args)
            {
                sb.Append(arg);
                sb.Append(",");
            }
            Logger.InfoFormat("OnDebugMessage {0} with arg {1}", message.Command, sb.ToString()); 
            var result = string.Empty;
            result += SharedCommandHandler.ProcessGameSettingCommnands(message,stateMachine);
            if(!string.IsNullOrEmpty(result))
            {
                return result;
            }
            SharedCommandHandler.ProcessHitBoxCommands(message);
            result += SharedCommandHandler.ProcessDebugCommand(message,_contexts);
            if(!string.IsNullOrEmpty(result))
            {
                return result;
            }
            result += UiModule.ProcessDebugCommand(message);
            if(!string.IsNullOrEmpty(result))
            {
                return result;
            }

            PlayerEntity self = _contexts.player.flagSelfEntity;
            if (self != null)
            {
                result = SharedCommandHandler.ProcessPlayerCommands(message, _contexts, self, _contexts.session.commonSession,  _contexts.session.currentTimeObject);
                SharedCommandHandler.ProcessVehicleCommand(message, _contexts.vehicle, self);
                SharedCommandHandler.ProcessSceneObjectCommand(
                    message, 
                    _contexts.sceneObject, 
                   _contexts.session.entityFactoryObject.SceneObjectEntityFactory,
                    self);
                SharedCommandHandler.ProcessMapObjectCommand(message, _contexts.mapObject,
                    _contexts.session.entityFactoryObject.MapObjectEntityFactory, self);
                result = SharedCommandHandler.ProcessCommands(message, _contexts, self);
            }
            return result;
        }
    }
}