using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Prediction.VehiclePrediction.Cmd;

namespace App.Shared.GameModules.Vehicle.Common
{
    public interface IVehicleMoveUtility
    {
        void Move(VehicleEntity vehicle, IVehicleCmd cmd);
    }
}
