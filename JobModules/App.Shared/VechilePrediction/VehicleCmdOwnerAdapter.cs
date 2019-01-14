
using System.Collections.Generic;
using System.Linq;
using Core.EntityComponent;
using Core.GameModule.System;
using Core.Prediction.VehiclePrediction;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using Entitas;

namespace App.Shared.GameModules.Player
{

    public class VehicleCmdOwnerAdapter : IVehicleCmdOwner
    {
        private VehicleEntity _playerEntity;

        public VehicleCmdOwnerAdapter(VehicleEntity playerEntity)
        {
            _playerEntity = playerEntity;
        }



        public EntityKey OwnerEntityKey { get { return _playerEntity.entityKey.Value; } }
        public List<IVehicleCmd> GetCmdList(int simulationStartTime, int simulationEndTime)
        {
            if (!_playerEntity.hasVehicleCmd)
                _playerEntity.AddVehicleCmd();

            return _playerEntity.vehicleCmd.GetBetween(simulationStartTime, simulationEndTime);
        }

        public IVehicleCmd LatestCmd
        {
            get
            {
                if (!_playerEntity.hasVehicleCmd)
                {
                    _playerEntity.AddVehicleCmd();
                    return null;
                }

                return _playerEntity.vehicleCmd.Latest;
            }
        }

        public void ClearCmdList()
        {
            if (_playerEntity.hasVehicleCmd)
            {
                _playerEntity.vehicleCmd.Reset();
            }

        }

        public Entity Entity { get { return _playerEntity; } }
    }
}