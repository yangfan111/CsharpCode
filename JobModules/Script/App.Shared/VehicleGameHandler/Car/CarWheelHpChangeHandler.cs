using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Vehicle.WheelCarrier;
using Core.GameHandler;
using Core.Prediction.VehiclePrediction.Cmd;
using Entitas;

namespace App.Shared.VehicleGameHandler
{
    public class CarWheelHpChangeHandler : VehicleStateChangeHandler
    {
        private bool _isOffline;
        private bool _isServer;
        public CarWheelHpChangeHandler(bool isOffline, bool isServer)
        {
            _isOffline = isOffline;
            _isServer = isServer;
        }

        protected override void DoHandle(GameEvent evt, VehicleEntity vehicle)
        {
            switch (evt)
            {
                case GameEvent.CarFirstWheelHpChange:
                    OnWheelHpChange(vehicle, VehiclePartIndex.FirstWheel);
                    break;
                case GameEvent.CarSecondWheelHpChange:
                    OnWheelHpChange(vehicle, VehiclePartIndex.SecondWheel);
                    break;
                case GameEvent.CarThirdWheelHpChange:
                    OnWheelHpChange(vehicle, VehiclePartIndex.ThirdWheel);
                    break;
                case GameEvent.CarFourthWheelHpChange:
                    OnWheelHpChange(vehicle, VehiclePartIndex.FourthWheel);
                    break;
            }
        }

        private void OnWheelHpChange(VehicleEntity vehicle, VehiclePartIndex index)
        {
            if (!WheelEntityUtility.HasWheel(vehicle, index))
            {
                return;
            }

            if (vehicle.carGameData.GetWheelHp(index) <= 0.0f)
            {
                if (_isServer)
                {
                    vehicle.vehicleBrokenFlag.SetVehiclePartBroken(index);
                }
                else
                {
                    WheelEntityEffectUtility.PlayWheelExplosion(vehicle, index);

                    if (_isOffline)
                    {
                        vehicle.vehicleBrokenFlag.SetVehiclePartBroken(index);
                    }
                }
            }
            else
            {
                if (!_isServer && !_isOffline)
                {
                    WheelEntityEffectUtility.EnableWheelRender(vehicle, index, true);
                }
            }
        }
    }
}
