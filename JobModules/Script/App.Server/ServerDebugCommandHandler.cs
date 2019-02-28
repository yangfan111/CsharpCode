using App.Server.GameModules.GamePlay.Free.client;
using App.Shared;
using App.Shared.DebugHandle;
using Core.SessionState;

namespace App.Server
{
    public class ServerDebugCommandHandler 
    {
        
        private Contexts _contexts;

        public ServerDebugCommandHandler(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void OnDebugMessage(ServerRoom room, DebugCommand message, PlayerEntity player,SessionStateMachine sessionStateMachine)
        {
            FreeDebugCommandHandler.Handle(room, message, player);
            SharedCommandHandler.ProcessGameSettingCommnands(message,sessionStateMachine);
            SharedCommandHandler.ProcessDebugCommand(message,_contexts);
            SharedCommandHandler.ProcessHitBoxCommands(message);
            SharedCommandHandler.ProcessPlayerCommands(message, _contexts, player, _contexts.session.commonSession,
                _contexts.session.currentTimeObject);
            SharedCommandHandler.ProcessVehicleCommand(message, _contexts.vehicle, player);
            SharedCommandHandler.ProcessSceneObjectCommand(message, _contexts.sceneObject,
                _contexts.session.entityFactoryObject.SceneObjectEntityFactory, player);
            SharedCommandHandler.ProcessMapObjectCommand(message, _contexts.mapObject,
                _contexts.session.entityFactoryObject.MapObjectEntityFactory, player);
            SharedCommandHandler.ProcessCommands(message, _contexts, player);
        }
    }
}