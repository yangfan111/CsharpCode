using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;

namespace Core.Prediction.VehiclePrediction
{
    public interface IVehicleExecutionSelector 
    {
        void Update();
        void LateUpdate();
        int ActiveCount { get; }

        IList<Entity> ActiveVehicles { get; } 
    }
}
