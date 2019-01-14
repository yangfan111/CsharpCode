using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Vehicle;
using UnityEngine;

namespace App.Shared.VehicleGameHandler
{
    public class VehicleFuelUpdateHandler : VehicleUpdateHandler
    {
        enum FuelCostType
        {
            None,
            Normal,
            Acceleration
        }

        private FuelCostType GetFuelCostType(VehicleEntity vehicle)
        {
            var throttleInput = vehicle.GetThrottleInput();
            if (!throttleInput.Equals(0))
            {
                if (throttleInput > 0.0f)
                {
                    return vehicle.IsAccelerated() ? FuelCostType.Acceleration : FuelCostType.Normal;
                }

                if (throttleInput < 0.0f)
                {
                    return FuelCostType.Normal;
                }

            }
           
            return FuelCostType.None;
        }

        private float GetFuelCost(VehicleEntity vehicle)
        {
            var costType = GetFuelCostType(vehicle);
            var gameData = vehicle.GetGameData();
            switch (costType)
            {
                case FuelCostType.Normal:
                    return Time.deltaTime * gameData.FuelCost;
                case FuelCostType.Acceleration:
                    return Time.deltaTime * gameData.FuleCostOnAcceleration;
                default:
                    return 0.0f;
            }
        }

        protected override void DoUpdate(VehicleEntity vehicle)
        {
            if (!vehicle.hasGameObject || !vehicle.HasGameData())
            {
                return;
            }

            var gameData = vehicle.GetGameData();
            gameData.DecreaseRemainingFuel(GetFuelCost(vehicle));
        }  
    }
}
