using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.EntityComponent;
using Core.ObjectPool;

namespace Core.Prediction.VehiclePrediction.Event
{
   public class VehicleDamangeSyncEvent : BaseRefCounter, IVehicleSyncEvent
    {
        public VehicleSyncEventType EType { get; set; }
        public int SourceObjectId { get; set; }
        public EntityKey TargetObject;
        public float Damage;

        public static VehicleDamangeSyncEvent Allocate()
        {
            return ObjectAllocatorHolder<VehicleDamangeSyncEvent>.Allocate();
        }

        protected override void OnCleanUp()
        {
            EType = VehicleSyncEventType.Undefined;
            ObjectAllocatorHolder<VehicleDamangeSyncEvent>.Free(this);
        }
    }
}
