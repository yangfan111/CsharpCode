using App.Shared.Components.SceneObject;
using App.Shared.Player;
using App.Shared.SceneTriggerObject;
using App.Shared.Util;
using Core;
using Core.CameraControl;
using Core.EntityComponent;
using Core.Enums;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;

namespace App.Shared.GameModules.SceneObject
{
    public class DoorTriggerSystem : IUserCmdExecuteSystem
    {
        private LoggerAdapter _logger = new LoggerAdapter(typeof(DoorTriggerSystem));

        private const float OpenMinAngle = -90.0f;
        private const float OpenMaxAngle = 90.0f;

        private Contexts _contexts;
        private MapObjectContext _mapContext;
        private IMapObjectEntityFactory _mapFactory;
        private ITriggerObjectListener _listener;
        protected ITriggerObjectManager _objectManager;
        
        public DoorTriggerSystem(Contexts context, ITriggerObjectListener listener = null)
        {
            _listener = listener;
            _contexts = context;
            _mapContext = context.mapObject;
            _mapFactory = _contexts.session.entityFactoryObject.MapObjectEntityFactory;
            
            var triggerObjectManager = SingletonManager.Get<TriggerObjectManager>();
            _objectManager = triggerObjectManager.GetManager(ETriggerObjectType.Door);  
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            if (cmd.IsUseAction && cmd.UseType == (int) EUseActionType.Door)
            {
                var entity =
                    _mapContext.GetEntityWithEntityKey(new EntityKey(cmd.UseEntityId, (int) EEntityType.MapObject));
                
                if (entity == null && (SharedConfig.IsServer||SharedConfig.IsOffline))
                {
                    _listener.CreateMapObj(cmd.UseEntityId);
                    entity = _mapContext.GetEntityWithEntityKey(new EntityKey(cmd.UseEntityId, (int) EEntityType.MapObject));
                    if (entity == null)
                    {
                        _logger.ErrorFormat("Door Entity {0} does not exist!", cmd.UseEntityId);
                        return;
                    }
                }

                if (!SharedConfig.IsServer && !SharedConfig.IsOffline)
                {
                    var player = (PlayerEntity)owner.OwnerEntity;
                    player.autoMoveInterface.PlayerAutoMove.StopAutoMove();
                    player.stateInterface.State.OpenDoor();
                        player.AudioController().PlaySimpleAudio(EAudioUniqueId.OpenDoor);
                    return;
                }
                
                var door = entity as MapObjectEntity;
                if (door == null)
                {
                    _logger.ErrorFormat("entity {0} is not door", cmd.UseEntityId);
                }

                if (!door.hasDoorRotate && door.doorData.IsOpenable())
                {
                    var player = (PlayerEntity)owner.OwnerEntity;
                    player.autoMoveInterface.PlayerAutoMove.StopAutoMove();
                    var go = door.rawGameObject.Value;
                    var playerPosition = player.RootGo().transform.position;
                    var doorPosition = go.transform.position;

                    var direction = (playerPosition - doorPosition);
                    var dot = Vector3.Dot(direction, go.transform.forward);

                    var rot = go.transform.localEulerAngles.y;
                    rot = YawPitchUtility.Normalize(rot);
                    float from = rot, to = 0;
                    var state = door.doorData.State;
                    var endState = state;
                    if (dot > 0)
                    {
                        switch (state)
                        {
                            case (int)DoorState.Closed:
                                to = from + OpenMaxAngle;
                                endState = (int)DoorState.OpenMax;
                                break;
                            case (int) DoorState.OpenMin:
                                to = from - OpenMinAngle;
                                endState = (int)DoorState.Closed;
                                break;
                            case (int)DoorState.OpenMax:
                                to = from - OpenMaxAngle;
                                endState = (int)DoorState.Closed;
                                break;
                        }
                    }
                    else if (dot < 0)
                    {
                        switch (state)
                        {
                            case (int)DoorState.Closed:
                                to = from + OpenMinAngle;
                                endState = (int)DoorState.OpenMin;
                                break;
                            case (int)DoorState.OpenMin:
                                to = from - OpenMinAngle;
                                endState = (int)DoorState.Closed;
                                break;
                            case (int)DoorState.OpenMax:
                                to = from - OpenMaxAngle;
                                endState = (int)DoorState.Closed;
                                break;
                        }
                    }

                    if (endState != state)
                    {
                        player.stateInterface.State.OpenDoor();
                            player.AudioController().PlaySimpleAudio(EAudioUniqueId.OpenDoor);

                        if(SharedConfig.IsServer || SharedConfig.IsOffline)
                        {
                            door.doorData.State = (int) DoorState.Rotating;
                            door.AddDoorRotate(from, from, to, endState);
                            _logger.DebugFormat("Trigger Door From {0} {1} To {2} {3}",
                                state, from, endState, to);
                        }
                    }
                }
            }
            
        }
    }
}
