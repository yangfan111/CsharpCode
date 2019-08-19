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
            AddSystem(new CreateMapObjByEventSystem(contexts));
            AddSystem(new TriggerObjectUpdateSystem(contexts));
            AddSystem(new DoorRotateSystem(contexts));
            AddSystem(new DoorTriggerSystem(contexts,new ServerDoorListener(contexts)));
            AddSystem(new ServerDestructibleObjectUpdateSystem(contexts));
            AddSystem(new ServerFreeCastSceneEntityDestroySystem(contexts));
            AddSystem(new ServerSceneObjectThrowingSystem(contexts.sceneObject, 
                contexts.session.currentTimeObject));
            AddSystem(new SceneObjectLimitSystem(contexts));
#if UNITY_EDITOR
        AddSystem(new ServerDebugSystem(contexts));
#endif
           
        }
    }
}