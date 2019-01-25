using System.Collections.Generic;
using Core.GameModule.System;
using Entitas;
using EVP;

namespace App.Shared.GameModules.Vehicle
{
    public class VehicleEntityDeactiveSystem : ReactiveEntityInitSystem<VehicleEntity>
    {
        public VehicleEntityDeactiveSystem(VehicleContext contexts) : base(contexts)
        {

        }

        protected override ICollector<VehicleEntity> GetTrigger(IContext<VehicleEntity> context)
        {
            return context.CreateCollector(VehicleMatcher.OwnerId.Removed());
        }

        protected override bool Filter(VehicleEntity entity)
        {
            return true;
        }

        public override void SingleExecute(VehicleEntity vehicle)
        {
            if (vehicle.hasGameObject)
            {
                //clear input
                vehicle.ClearInput();
            }
        }

       

    }
}
