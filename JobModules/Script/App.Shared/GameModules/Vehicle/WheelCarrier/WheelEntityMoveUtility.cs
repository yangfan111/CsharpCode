using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Vehicle.Common;
using Core.Prediction.VehiclePrediction.Cmd;
using EVP.Scripts;

namespace App.Shared.GameModules.Vehicle.WheelCarrier
{
    public class WheelEntityMoveUtility : IVehicleMoveUtility
    {
        private static IVehicleMoveUtility[] EntityAPI =
        {
            new CarMoveUtility(),
            new MotorMoveUtility(),
        };

        public static void MoveByCmd(VehicleEntity vehicle, IVehicleCmd cmd)
        {
            if (!vehicle.hasGameObject)
            {
                return;
            }

            var controllerType = vehicle.GetController<VehicleAbstractController>().ControllerType;
            EntityAPI[(int)controllerType].Move(vehicle, cmd);
        }

        public void Move(VehicleEntity vehicle, IVehicleCmd cmd)
        {
            MoveByCmd(vehicle, cmd);
        }
    }
}
