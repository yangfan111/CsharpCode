using System.Collections.Generic;
using Core.Prediction.VehiclePrediction.Cmd;
using Entitas;

namespace Core.Prediction.VehiclePrediction
{
    public interface IVehicleCmdOwner
    {
        List<IVehicleCmd> GetCmdList(int simulationStartTime, int simulationEndTime);

        IVehicleCmd LatestCmd { get; }
        void ClearCmdList();

        Entity Entity { get; }
    }
}