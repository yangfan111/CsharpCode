using App.Shared;
using App.Shared.GameModules.SceneObject;
using Core.GameModule.Module;
using Core.GameModule.System;
using UnityEngine;

namespace App.Client.GameModules.SceneObject
{
    public class ClientSceneObjectModule : GameModule
    {
        public ClientSceneObjectModule(Contexts contexts, ICommonSessionObjects sessionObjects)
        {
            AddSystem(new SceneObjectLoadSystem(contexts, sessionObjects));
            AddSystem(new SceneObjectCastTargetLoadSystem(contexts.sceneObject));
            AddSystem(new SceneObjectCleanupSystem(contexts));
            AddSystem(new FreeObjectLoadSystem(contexts.freeMove));
            AddSystem(new FreeObjectCleanUpSystem(contexts.freeMove));
            AddSystem(new CreateMapObjSystem(contexts));
            AddSystem(new FreeObjectPositionUpdateSystem(contexts));
            if (!SharedConfig.DisableDoor)
            {
                AddSystem(new TriggerObjectUpdateSystem(contexts));
            }

            AddSystem(new DoorPlaybackSystem(contexts));
            
            AddSystem(new ClientDestructibleObjectUpdateSystem(contexts));

            if (SharedConfig.IsOffline)
            {
                AddSystem(new DoorRotateSystem(contexts));
            }

            AddSystem(new DoorTriggerSystem(contexts,new ClientDoorListener(contexts)));
            AddSystem(new ClientSceneObjectRenderSystem(contexts));
            AddSystem(new ColliderCounterSystem(contexts));
        }
    }
}
