using App.Shared.GameModules.Player;
using App.Shared.Player;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Entitas;
using UnityEngine;

namespace App.Shared.GameModules.Vehicle
{
    public class UpdatePlayerPositionOnVehicle : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(UpdatePlayerPositionOnVehicle));
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
            for(int i = 0; i < playerEntities.Length; ++i)
            {
                var playerEntity = playerEntities[i];
                if (!playerEntity.IsOnVehicle())
                    continue;

                Transform seat = playerEntity.GetVehicleSeatTransform(_vehicleContext);
                if (null == seat) continue;
                var characterTransform = playerEntity.RootGo().transform;
                if (seat != characterTransform.parent)
                {
                    characterTransform.parent = seat;
                    characterTransform.localPosition = Vector3.zero;
                    characterTransform.localRotation = Quaternion.identity;
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
