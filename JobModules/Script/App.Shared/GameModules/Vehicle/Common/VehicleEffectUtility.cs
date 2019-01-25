using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Vehicle.WheelCarrier;
using Core.Prediction.VehiclePrediction.Cmd;
using EVP;
using EVP.Scripts;
using UnityEngine;

namespace App.Shared.GameModules.Vehicle
{
    public static class VehicleEffectUtility
    {


        private static IVehicleEntityEffectUtility[] EntityAPI =
        {
            new WheelEntityEffectUtility(),
            new ShipEntityEffectUtility()
        };

        public static void PlayExplosionEffect(this VehicleEntity vehicle)
        {
            EntityAPI[vehicle.GetTypeValue()].PlayExplosionEffect(vehicle);
        }

        public static void SetEngineEffectPercent(this VehicleEntity vehicle, float percent)
        {
            EntityAPI[vehicle.GetTypeValue()].SetEngineEffectPercent(vehicle, percent);
        }

        public static void EnableEngineAudio(this VehicleEntity vehicle, bool enabled)
        {
            EntityAPI[vehicle.GetTypeValue()].EnableEngineAudio(vehicle, enabled);
        }

    }
}
