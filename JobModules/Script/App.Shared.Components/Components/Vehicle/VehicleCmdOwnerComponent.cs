using Core.GameModule.System;
using Core.Prediction.VehiclePrediction;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Player
{
    [Vehicle]
    public class VehicleCmdOwnerComponent : IComponent
    {
        public IVehicleCmdOwner OwnerAdater;
    }

    [Vehicle, Unique]
    public class SimulationTimeComponent : IComponent
    {
        public int SimulationTime;
    }

    [Vehicle]
    public class FlagOfflineComponent : IComponent
    {
    }
}