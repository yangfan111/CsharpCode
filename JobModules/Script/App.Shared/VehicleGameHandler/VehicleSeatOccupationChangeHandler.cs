using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Vehicle;
using Core.GameHandler;

namespace App.Shared.VehicleGameHandler
{
    public class VehicleSeatOccupationChangeHandler : VehicleStateChangeHandler
    {
        private bool _isOffline;
        private bool _isServer;
        private PlayerContext _playerContext;
        public VehicleSeatOccupationChangeHandler(PlayerContext playerContext, bool isOffline, bool isServer)
        {
            _isOffline = isOffline;
            _isServer = isServer;
            _playerContext = playerContext;
        }

        protected override void DoHandle(GameEvent evt, VehicleEntity vehicle)
        {
            if (_isServer)
            {
                if (!SharedConfig.DynamicPrediction)
                {
                    if (vehicle.HasDriver())
                    {
                        vehicle.SetKinematic(true);
                    }
                    else
                    {
                        vehicle.SetKinematic(false, true);
                    }

                }


            }
            else
            {
                if (!SharedConfig.DynamicPrediction)
                {
                    if (_isOffline || IsCurrentPlayerDriver(vehicle))
                    {
                        vehicle.SetKinematic(false);
                    }
                    else
                    {
                        vehicle.SetKinematic(true);
                    }
                }
            }

            bool active = vehicle.HasDriver();
            if (vehicle.GetGameData().RemainingFuel > 0)
            {
                vehicle.EnableInput(active);
            }
           
            if (!vehicle.HasAnyPassager())
            {
                vehicle.SetHandBrakeOn(true);
            }
        }

        private bool IsCurrentPlayerDriver(VehicleEntity vehicle)
        {
            if (vehicle.HasDriver() && vehicle.hasOwnerId)
            {
                return vehicle.ownerId.Value.EntityId == _playerContext.flagSelfEntity.entityKey.Value.EntityId;
            }

            return false;
        }
    }
}
