using App.Shared.Components.SceneObject;
using App.Shared.Player;
using Core.CameraControl;
using Core.Enums;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.SceneObject
{
    public class DoorTriggerSystem : IUserCmdExecuteSystem
    {
        private LoggerAdapter _logger = new LoggerAdapter(typeof(DoorTriggerSystem));

        private const float OpenMinAngle = -90.0f;
        private const float OpenMaxAngle = 90.0f;

        private MapObjectContext _context;
        public DoorTriggerSystem(MapObjectContext context)
        {
            _context = context;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            if (cmd.IsUseAction && cmd.UseType == (int) EUseActionType.Door)
            {
                var door = _context.GetEntityWithEntityKey(new Core.EntityComponent.EntityKey(cmd.UseEntityId, (short)EEntityType.MapObject));
                if (door == null)
                {
                    _logger.ErrorFormat("Door Entity {0} does not exist!", cmd.UseEntityId);
                    return;
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
                        if(SharedConfig.IsServer || SharedConfig.IsOffline)
                        {
                            door.doorData.State = (int) DoorState.Rotating;
                            door.AddDoorRotate(from, from, to, endState);
                            _logger.DebugFormat("Trigger Door From {0} {1} To {2} {3}",
                                state, from, endState, to);
                            //switch((DoorState)endState)
                            //{
                            //    case DoorState.Closed:
                            //        player.soundManager.Value.PlayOnce(XmlConfig.EPlayerSoundType.CloseDoor);
                            //        break;
                            //    case DoorState.OpenMax:
                            //    case DoorState.OpenMin:
                            //        player.soundManager.Value.PlayOnce(XmlConfig.EPlayerSoundType.OpenDoor);
                            //        break;
                            //}
                        }
                    }
                }
            }
            
        }
    }
}
