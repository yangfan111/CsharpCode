using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.GameHandler;
using Entitas;

namespace App.Shared.VehicleGameHandler
{
    public abstract class VehicleStateChangeHandler : IGameEventHandler
    {
        private VehicleTypeMatcher _matcher;

        public VehicleStateChangeHandler()
        {
            _matcher = new VehicleTypeMatcher();
        }

        public VehicleStateChangeHandler(VehicleTypeMatcher matcher)
        {
            _matcher = matcher;
        }

        protected abstract void DoHandle(GameEvent evt, VehicleEntity entity);

        public void Handle(GameEvent evt, Entity entity)
        {
            var vehicle = (VehicleEntity) entity;
            if (_matcher.IsMatched(vehicle.vehicleType.VType))
            {
                DoHandle(evt, vehicle);
            }
        }
    }
}
