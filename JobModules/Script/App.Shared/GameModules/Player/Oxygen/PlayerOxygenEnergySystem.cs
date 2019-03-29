using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.GameModules.Vehicle;
using App.Shared.Player;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlConfig;

namespace App.Shared.GameModules.Player.Oxygen
{
    public class PlayerOxygenEnergySystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerOxygenEnergySystem));

        private readonly VehicleContext _vehicleContext;
        public PlayerOxygenEnergySystem(VehicleContext vehicleContex)
        {
            _vehicleContext = vehicleContex;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = owner.OwnerEntity as PlayerEntity;
            if (player.gamePlay.IsLifeState(EPlayerLifeState.Dead))
            {
                return;
            }

            player.oxygenEnergyInterface.Oxygen.InDivingState = CheckDiving(player);

            var oxygenEnergy = player.oxygenEnergyInterface.Oxygen;

            oxygenEnergy.SyncFrom(player.oxygenEnergy);

            oxygenEnergy.UpdateOxygenEnergy(cmd.FrameInterval / 1000.0f);
            oxygenEnergy.ResetOxygen(player.gamePlay.GameState == GameState.AirPlane);

            oxygenEnergy.SyncTo(player.oxygenEnergy);

            UpdatePlayerState(player);
        }

        private bool CheckDiving(PlayerEntity player)
        {
            var state = player.stateInterface.State;
            PostureInConfig currentPosture = state.GetCurrentPostureState();
            var ret = PostureInConfig.Dive == currentPosture;

            var vehicle = PlayerVehicleUtility.GetVehicle(player, _vehicleContext);
            if (null != vehicle)
                ret = ret || vehicle.IsPassagerInWater();
            return ret;
        }

        private void UpdatePlayerState(PlayerEntity player)
        {
            // 窒息状态
            var oxygen = player.oxygenEnergyInterface.Oxygen;
            if (oxygen.InDivingDeffState && !PlayerStateUtil.HasPlayerState(EPlayerGameState.DivingChok, player.gamePlay))
            {
                PlayerStateUtil.AddPlayerState(EPlayerGameState.DivingChok, player.gamePlay);
            }
            else if (PlayerStateUtil.HasPlayerState(EPlayerGameState.DivingChok, player.gamePlay))
            {
                PlayerStateUtil.RemoveGameState(EPlayerGameState.DivingChok, player.gamePlay);
            }
        }
    }
}
