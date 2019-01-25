using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Vehicle.Common;
using Core.Prediction.VehiclePrediction.Cmd;
using DWP;
using UnityEngine;

namespace App.Shared.GameModules.Vehicle.Ship
{
    internal static class ShipMoveInternal
    {
        public static void Move(VehicleEntity vehicle, IVehicleCmd cmd)
        {
            var target = vehicle.GetController<AdvancedShipController>();

            var steerInput = Mathf.Clamp(cmd.MoveHorizontal, -1f, 1f);
            var forwardInput = Mathf.Clamp(cmd.MoveVertical, -1f, 1f);

            target.SetInput(forwardInput, steerInput, cmd.IsSpeedup);
        }
    }

    public class ShipMoveUtility : IVehicleMoveUtility
    {
        public void Move(VehicleEntity vehicle, IVehicleCmd cmd)
        {
            if (!vehicle.hasGameObject)
            {
                return;
            }

            ShipMoveInternal.Move(vehicle, cmd);
        }
    }
}
