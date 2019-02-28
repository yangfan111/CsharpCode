using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using App.Shared.GameModules.Vehicle;
using App.Shared.UserPhysics;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.Prediction.VehiclePrediction;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using Entitas;
using UnityEngine;
using VehicleCommon;

namespace App.Server.GameModules.Vehicle
{
    public abstract class VehicleCmdExecuteSystem : IVehicleCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(VehicleCmdExecuteSystem));

        private VehicleTimer _vehicleTimer;
        protected bool EnableSyncFromComponent;
        public VehicleCmdExecuteSystem(VehicleTimer vehicleTimer)
        {
            _vehicleTimer = vehicleTimer;
            EnableSyncFromComponent = true;
        }

        private bool _first = true;

        public bool IsEntityValid(Entity entity)
        {
            var vehicleEntity = entity as VehicleEntity;
            if (vehicleEntity == null || !vehicleEntity.hasVehicleType || !vehicleEntity.HasDynamicData())
            {
                //_logger.InfoFormat("VehicleEntity {0} is invalid!", vehicleEntity);
                return false;
            }

            return true;
        }

        public virtual void UpdateSimulationTime(int simulationTime)
        {
            _vehicleTimer.SetCurrentTime(simulationTime);
        }

        public virtual void SyncFromComponent(Entity entity)
        {
            var vehicle = (VehicleEntity) entity;
            if (_first)
            {
                _first = false;
                SyncToComponent(vehicle, true);
            }

            if (!SharedConfig.IsOffline)
            {
                VehicleStateUtility.SyncFromComponent(vehicle);
            }
        }

        public virtual void ExecuteVehicleCmd(Entity entity, IVehicleCmd cmd)
        {
            var vehicle = (VehicleEntity)entity;
            vehicle.Move(cmd);
        }

        public virtual void FixedUpdate(Entity entity)
        {
            var vehicle = (VehicleEntity)entity;
            VehicleStateUtility.FixedUpdate(vehicle);
        }

        public virtual void Update(Entity entity)
        {
            var vehicle = (VehicleEntity) entity;
            VehicleStateUtility.Update(vehicle);
        }

        public virtual void SyncToComponent(Entity entity, bool forceSync = false)
        {
            var vehicle = (VehicleEntity)entity;
            VehicleStateUtility.SyncToComponent(vehicle, forceSync);
        }
    }
}
