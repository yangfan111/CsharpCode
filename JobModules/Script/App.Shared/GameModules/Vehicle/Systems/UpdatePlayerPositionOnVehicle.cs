using App.Shared.GameModules.Player;
using App.Shared.Player;
using BehaviorDesigner.Runtime.Tasks.Movement;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Entitas;
using UnityEngine;
using Utils.Appearance;

namespace App.Shared.GameModules.Vehicle
{
    public class UpdatePlayerPositionOnVehicle : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(UpdatePlayerPositionOnVehicle));
        private BoneMount _mount = new BoneMount();
        private IGroup<PlayerEntity> _players;
        private VehicleContext _vehicleContext;
        private PlayerContext _playerContext;

        public UpdatePlayerPositionOnVehicle(Contexts contexts)
        {
            _vehicleContext = contexts.vehicle;
            _playerContext = contexts.player;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var playerEntities = _playerContext.GetEntities();
            var ownerEntity = owner.OwnerEntity as PlayerEntity;
            for(var i = 0; i < playerEntities.Length; ++i)
            {
                var playerEntity = playerEntities[i];
                if (!playerEntity.IsOnVehicle())
                    continue;

                var seat = playerEntity.GetVehicleSeatTransform(_vehicleContext);
                if (null == seat) continue;
                var characterTransform = playerEntity.RootGo().transform;
                var character = playerEntity.RootGo();
                if (seat != characterTransform.parent)
                {
                    _mount.MountCharacterToVehicleSeat(character, seat);
                    
                    // 切换座位，并且换到主驾驶位，设置IK
                    if (playerEntity.IsVehicleDriver())
                    {
                        var vehicle = _vehicleContext.GetEntityWithEntityKey(playerEntity.controlledVehicle.EntityKey);
                        playerEntity.SetSteeringWheelIK(vehicle);
                    }
                    else
                    {
                        playerEntity.EndSteeringWheelIK();
                    }
                }

                if (ownerEntity == playerEntity)
                {
                    playerEntity.position.Value = characterTransform.position;
                    playerEntity.orientation.Yaw = characterTransform.rotation.eulerAngles.y;
                }
            }
        }
    }
}
