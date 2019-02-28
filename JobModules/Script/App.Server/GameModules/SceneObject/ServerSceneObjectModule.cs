using App.Shared.Components;
using App.Shared.GameModules.SceneObject;
using Core.GameModule.Module;
using Core.SessionState;

namespace App.Server.GameModules.SceneObject
{
    public class ServerSceneObjectModule : GameModule
    {
        public ServerSceneObjectModule(Contexts contexts, ISessionState sessionState, IEntityIdGenerator equipmentIdGenerator)
        {
            //AddSystem(new EquipmentInitSystem(contexts, sessionState, equipmentIdGenerator));
            AddSystem(new TriggerObjectUpdateSystem(contexts));
            AddSystem(new DoorRotateSystem(contexts, new ServerDoorListener(contexts)));
            AddSystem(new DoorTriggerSystem(contexts.mapObject));
            AddSystem(new ServerDestructibleObjectUpdateSystem(contexts));
            AddSystem(new ServerFreeCastSceneEntityDestroySystem(contexts));
            AddSystem(new ServerSceneObjectThrowingSystem(contexts.sceneObject, 
                contexts.session.currentTimeObject,
                contexts.session.commonSession.RuntimeGameConfig));
#if UNITY_EDITOR
        AddSystem(new ServerDebugSystem(contexts));
#endif
           
        }
    }
}