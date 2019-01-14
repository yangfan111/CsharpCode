using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using App.Shared.GameModules.Player;
using Core.Configuration;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Vehicle
{
    public class VehicleSoundSelectSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(VehicleSoundSelectSystem));

        private VehicleContext _context;
        public VehicleSoundSelectSystem(VehicleContext context)
        {
            _context = context;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var playerEntity = (PlayerEntity) owner.OwnerEntity;
            if (playerEntity.IsOnVehicle())
            {

                var channel = cmd.ChangeChannel;
                if (channel > 0 && playerEntity.IsVehicleDriver())
                {

                    var vehicle = _context.GetEntityWithEntityKey(playerEntity.controlledVehicle.EntityKey);
                    if (vehicle != null)
                    {
                        var assetInfo = vehicle.vehicleAssetInfo;
                        if (assetInfo.HasRadio)
                        {
                            var gameData = vehicle.GetGameData();
                            if (gameData.SoundChannel != channel)
                            {
                                gameData.SetSoundChannel((EVehicleChannel)channel);
                            }
                            else
                            {
                                gameData.SetSoundChannel(EVehicleChannel.None);
                            }

                            _logger.DebugFormat("Change Vehicle {0} Sound Channel {1} Music Sound Id {2}",
                                vehicle.entityKey.Value, gameData.SoundChannel, gameData.CurrentSoundId);
                        }
                    }
                }
            }
        }
    }
}
