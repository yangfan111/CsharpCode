using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Vehicle;
using Core.GameHandler;

namespace App.Shared.VehicleGameHandler
{
    public abstract class VehicleHpChangeHandler : VehicleStateChangeHandler
    {
        private bool _isOffline;
        private bool _isServer;
        public VehicleHpChangeHandler(bool isOffline, bool isServer, VehicleTypeMatcher matcher) : base(matcher)
        {
            _isOffline = isOffline;
            _isServer = isServer;
        }

        protected override void DoHandle(GameEvent evt, VehicleEntity vehicle)
        {
            var gameData = vehicle.GetGameData();
            if (gameData.Hp <= 0.0f)
            {
                OnHpZero(vehicle);

                if (_isServer)
                {
                    vehicle.vehicleBrokenFlag.SetBodyBroken();
                  
                }
                else
                {
                    vehicle.SetEngineEffectPercent(0);
                    vehicle.PlayExplosionEffect();

                    if (_isOffline)
                    {
                        vehicle.vehicleBrokenFlag.SetBodyBroken();
                    }
                }
            }
            else
            {
                if (!_isServer)
                {
                    var percent = gameData.Hp / gameData.MaxHp;
                    vehicle.SetEngineEffectPercent(percent);
                }
            }
        }

        protected abstract void OnHpZero(VehicleEntity vehicle);
    }
}
