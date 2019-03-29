using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.EntityComponent;
using Core.ObjectPool;

namespace Core.Prediction.VehiclePrediction.Event
{
    public enum VehicleSyncEventType
    {
        Undefined = 0,
        Damage = 1,
    }

    public interface IVehicleSyncEvent : IRefCounter
    {
        VehicleSyncEventType EType { get; set; }
        int           SourceObjectId { get; set; }
    }
}
