using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Prediction.VehiclePrediction.Cmd;
using Entitas;

namespace App.Shared.GameModules.Vehicle
{
    public interface IVehicleStateUtility
    {
        bool IsReadyForSync(VehicleEntity vehicle);
        void SyncOnPlayerRideOn(VehicleEntity vehicle);
        void SyncFromComponent(VehicleEntity vehicle);
        void FixedUpdate(VehicleEntity vehicle);
        void Update(VehicleEntity vehicle);
        void SyncToComponent(VehicleEntity vehicle);

        void SetVehicleStateToCmd(VehicleEntity vehicle, IVehicleCmd cmd);
        void ApplyVehicleCmdAndState(VehicleEntity vehicle, IVehicleCmd cmd);

        void SetVehicleSyncLatest(VehicleEntity vheicle, bool isSyncLatest);

//        string[] GetDebugInfo(VehicleEntity vehicle, string stateStr, string filterStr);
    }
}