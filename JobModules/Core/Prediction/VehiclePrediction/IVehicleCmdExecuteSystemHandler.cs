using System.Collections.Generic;

namespace Core.Prediction.VehiclePrediction
{
    public interface IVehicleCmdExecuteSystemHandler
    {
        List<IVehicleCmdOwner> VehicleCmdOwnerList { get; }
        int LastSimulationTime { get; set; }
        bool IsReady();
    }
}