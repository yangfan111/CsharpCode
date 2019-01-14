using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Vehicle.Common;
using Core.Prediction.VehiclePrediction.Cmd;
using EVP;
using EVP.Scripts;
using UnityEngine;

namespace App.Shared.GameModules.Vehicle.WheelCarrier
{
    internal static class MotorMoveInternal
    {
        public static void Move(VehicleEntity vehicle, IVehicleCmd cmd)
        {
            var target = vehicle.GetController<MotorcycleController>();

            var steerInput = Mathf.Clamp(cmd.MoveHorizontal, -1f, 1f);
            var forwardInput = Mathf.Clamp(cmd.MoveVertical, -1f, 1f);

            var horizontalShift = 0.0f;
            if (cmd.IsLeftShift)
            {
                horizontalShift = target.horizontalMassShift - 0.1f;
            }else if (cmd.IsRightShift)
            {
                horizontalShift = target.horizontalMassShift + 0.1f;
            }

            horizontalShift = Mathf.Clamp(horizontalShift, -1f, 1f);

            var stuntInput = 0.0f;
            
            if (cmd.IsHandbrake)
            {
                stuntInput = target.HasAnyWheelOnGround() ? 0.0f : - 1.0f;
            }
            else if (cmd.IsStunt)
            {
                stuntInput = target.HasAnyWheelOnGround() ? 0.0f : 1.0f;
            }


            target.SetInput(forwardInput, steerInput, cmd.IsSpeedup, cmd.IsHandbrake, cmd.IsHornOn, horizontalShift, stuntInput);
        }
    }

    public class MotorMoveUtility : IVehicleMoveUtility
    {
        public void Move(VehicleEntity vehicle, IVehicleCmd cmd)
        {
            if (!vehicle.hasGameObject)
            {
                return;
            }

            MotorMoveInternal.Move(vehicle, cmd);
        }
    }
}
