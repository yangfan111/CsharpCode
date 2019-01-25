using System.Collections.Generic;
using App.Shared.GameModules.HitBox;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.Utils;
using Entitas;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace App.Shared.GameModules.Vehicle
{
    public class VehicleSyncPositionSystem : IPhysicsPostUpdateSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(VehicleSyncPositionSystem));

        private IGroup<VehicleEntity> _carContext;
        private IGroup<VehicleEntity> _shipContext;
        public VehicleSyncPositionSystem(Contexts contexts) : base()
        {
            _carContext = contexts.vehicle.GetGroup(VehicleMatcher.AllOf(VehicleMatcher.Position, VehicleMatcher.VehicleType, VehicleMatcher.CarRewindData));
            _shipContext = contexts.vehicle.GetGroup(VehicleMatcher.AllOf(VehicleMatcher.Position, VehicleMatcher.VehicleType, VehicleMatcher.ShipDynamicData));
        }


        public void PostUpdate()
        {
            foreach (var car in _carContext.GetEntities())
            {
                car.position.Value = car.carRewindData.Position;
            }

            foreach (var ship in _shipContext.GetEntities())
            {
                ship.position.Value = ship.shipDynamicData.Position;
            }
        }
    }
}
