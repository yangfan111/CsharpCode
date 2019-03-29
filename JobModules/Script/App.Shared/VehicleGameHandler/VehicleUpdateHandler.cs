using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Vehicle;
using Core.GameHandler;
using Entitas;

namespace App.Shared.VehicleGameHandler
{
    public abstract class VehicleUpdateHandler : IGameStateUpdateHandler
    {
        private VehicleTypeMatcher _matcher;

        public VehicleUpdateHandler()
        {
            _matcher = new VehicleTypeMatcher();
        }

        public VehicleUpdateHandler(VehicleTypeMatcher matcher)
        {
            _matcher = matcher;
        }

        protected abstract void DoUpdate(VehicleEntity vehicle);

        public void Update(Entity entity)
        {
            var vehicle = (VehicleEntity) entity;
            if (_matcher.IsMatched(vehicle.vehicleType.VType) && 
                vehicle.IsActiveSelf())
            {
                DoUpdate(vehicle);
            }
        }

        
    }
}
