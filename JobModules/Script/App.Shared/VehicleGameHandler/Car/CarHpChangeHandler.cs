using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Vehicle;
using XmlConfig;

namespace App.Shared.VehicleGameHandler
{
    public class CarHpChangeHandler : VehicleHpChangeHandler
    {
        public CarHpChangeHandler(bool isOffline, bool isServer) : 
            base(isOffline, isServer, new VehicleTypeMatcher(EVehicleType.Car))
        {
            
        }

        protected override void OnHpZero(VehicleEntity vehicle)
        {
            var gameData = vehicle.carGameData;
            if (gameData.FirstWheelHp > 0.0f)
            {
                gameData.FirstWheelHp = 0.0f;
            }

            if (gameData.SecondWheelHp > 0.0f)
            {
                gameData.SecondWheelHp = 0.0f;

            }

            if (gameData.ThirdWheelHp > 0.0f)
            {
                gameData.ThirdWheelHp = 0.0f;
            }

            if (gameData.FourthWheelHp > 0.0f)
            {
                gameData.FourthWheelHp = 0.0f;
            }
        }
    }
}
