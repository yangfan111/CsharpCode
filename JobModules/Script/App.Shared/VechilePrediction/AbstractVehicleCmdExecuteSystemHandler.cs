using System.Collections.Generic;
using App.Shared.Components.Player;
using Core.GameModule.System;
using Core.Prediction.VehiclePrediction;

namespace App.Server
{
    public abstract class AbstractVehicleCmdExecuteSystemHandler : IVehicleCmdExecuteSystemHandler
    {
        
        private List<IVehicleCmdOwner> _vehicleCmdOwners = new List<IVehicleCmdOwner>();
        private VehicleEntity[] _lastVehicleEntities;
        private VehicleContext _vehicleContext;
       

        protected AbstractVehicleCmdExecuteSystemHandler(VehicleContext vehicleContext)
        {
            _vehicleContext = vehicleContext;
            
        }

        public List<IVehicleCmdOwner> VehicleCmdOwnerList
        {
            get
            {
                var vehicleEntities = GetLatestEntities();
                if (_lastVehicleEntities != vehicleEntities)
                {
                    _lastVehicleEntities = vehicleEntities;
                    _vehicleCmdOwners.Clear();
                    int count = vehicleEntities.Length;
                    for (int i = 0; i < count; i++)
                    {
                        var vehicle = vehicleEntities[i];
                        _vehicleCmdOwners.Add(vehicle.vehicleCmdOwner.OwnerAdater);
                    }
                }
                return _vehicleCmdOwners;
            }
            
        }

        protected abstract VehicleEntity[] GetLatestEntities();
        public bool IsReady()
        {
            return _vehicleContext.hasSimulationTime;
        }

        public int LastSimulationTime 
        {
            get { return _vehicleContext.simulationTime.SimulationTime; }
            set { _vehicleContext.simulationTime.SimulationTime = value; }
        }

    }
}