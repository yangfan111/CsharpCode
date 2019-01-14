using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using App.Shared.GameModules.Vehicle;
using Core.Configuration;
using UnityEngine;

namespace App.Shared.VehicleGameHandler
{
    public class VehicleSoundUpdateHandler : VehicleUpdateHandler
    {
        protected override void DoUpdate(VehicleEntity vehicle)
        {
            if (vehicle.HasGameData())
            {
                var gameData = vehicle.GetGameData();
                if (!vehicle.HasAnyPassager())
                {
                    gameData.ClearAllSounds();
                }
                else
                {
                    if (gameData.CurrentSoundId != (int) EVehicleSoundId.Invalid)
                    {
                        gameData.SetMusicSyncTime(Time.time);
                    }
                }
            }

        }
    }
}
