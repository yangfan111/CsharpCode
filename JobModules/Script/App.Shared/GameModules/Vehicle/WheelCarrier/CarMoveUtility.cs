using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Vehicle.Common;
using Core.Prediction.VehiclePrediction.Cmd;
using EVP;
using UnityEngine;

namespace App.Shared.GameModules.Vehicle.WheelCarrier
{
    internal static class CarMoveInternal
    {
        public static void Move(VehicleEntity vehicle, IVehicleCmd cmd)
        {
            var target = vehicle.GetController<VehicleController>();

            var steerInput = Mathf.Clamp(cmd.MoveHorizontal, -1f, 1f);
            var forwardInput = Mathf.Clamp(cmd.MoveVertical, -1f, 1f);
            var reverseInput = -forwardInput;

            float throttleInput = 0.0f;
            float brakeInput = 0.0f;

            float minSpeed = 0.1f;
            float minInput = 0.1f;

            if (target.speed > minSpeed)
            {
                throttleInput = forwardInput;
                brakeInput = reverseInput;
            }
            else
            {
                if (reverseInput > minInput)
                {
                    throttleInput = -reverseInput;
                    brakeInput = 0.0f;
                }
                else if (forwardInput > minInput)
                {
                    if (target.speed < -minSpeed)
                    {
                        throttleInput = 0.0f;
                        brakeInput = forwardInput;
                    }
                    else
                    {
                        throttleInput = forwardInput;
                        brakeInput = 0;
                    }
                }
            }

            target.SetInput(steerInput, throttleInput, brakeInput, cmd.IsSpeedup, cmd.IsHandbrake, cmd.IsHornOn);
        }
    }

    public class CarMoveUtility : IVehicleMoveUtility
    {
        public void Move(VehicleEntity vehicle, IVehicleCmd cmd)
        {
            if (!vehicle.hasGameObject)
            {
                return;
            }

            CarMoveInternal.Move(vehicle, cmd);
        }
    }
}
