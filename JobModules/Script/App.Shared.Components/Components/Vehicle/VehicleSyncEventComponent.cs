using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Prediction.VehiclePrediction.Event;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Vehicle
{
    [Vehicle]
    public class VehicleSyncEventComponent : IResetableComponent, IComponent, IVehicleResetableComponent
    {
        public Queue<IVehicleSyncEvent> SyncEvents = new Queue<IVehicleSyncEvent>();

        public void Reset()
        {
            while(SyncEvents.Count > 0)
            {
                var e = SyncEvents.Dequeue();
                e.ReleaseReference();
            }
        }
    }
}
