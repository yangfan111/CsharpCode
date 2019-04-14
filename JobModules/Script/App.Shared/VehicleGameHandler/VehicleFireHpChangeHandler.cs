using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Vehicle;

using Core.Prediction.VehiclePrediction.Cmd;
using EVP;
using UnityEngine;
using EVP.Scripts;
using VehicleCommon;

namespace App.Shared.VehicleGameHandler
{
    public class VehicleFireHpChangeHandler: VehicleUpdateHandler
    {
        enum FireLevel
        {
            None,
            FirstLevel,
            SecondLevel,
            ThirdLevel,
            FourthLevel
        }

        private FireLevel GetFireLevel(VehicleEntity vehicle)
        {

            var config = vehicle.GetController<VehicleCommonController>().GetComponent<VehicleBaseConfig>();
            float [] engineEffectOfVehicle = config.engineFumesRange;
            var vehicleCurrentHp = vehicle.GetGameData().Hp;
            var vehicleMaxHp = vehicle.GetGameData().MaxHp;
            var percent = vehicleCurrentHp / vehicleMaxHp;
            if (config.Equals( null))
                return FireLevel.None;
            if (!vehicleCurrentHp.Equals(0))
            {
                if (percent >= engineEffectOfVehicle[0] + engineEffectOfVehicle[1] + engineEffectOfVehicle[2])
                {
                    return FireLevel.FirstLevel;
                }
                
                if (percent >= engineEffectOfVehicle[0] + engineEffectOfVehicle[1]&& percent < engineEffectOfVehicle[0] + engineEffectOfVehicle[1]+engineEffectOfVehicle[2])
                {
                    return FireLevel.SecondLevel;
                }
                
                if (percent >= engineEffectOfVehicle[0] && percent < engineEffectOfVehicle[0] + engineEffectOfVehicle[1])
                {
                    return FireLevel.ThirdLevel ;
                }
                
                if (percent > 0 && percent < engineEffectOfVehicle[0])
                {
                    return FireLevel.FourthLevel;
                }

            }

            return FireLevel.None;
        }

        private float GetFireHpCost(VehicleEntity vehicle)
        {
            var costType = GetFireLevel(vehicle);
            var maxHp = vehicle.GetGameData().MaxHp;
            switch (costType)
            {
                case FireLevel.ThirdLevel:
                    return Time.deltaTime * maxHp*0.01f;
                case FireLevel.FourthLevel:
                    return Time.deltaTime * maxHp *0.02f;
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
            gameData.DecreaseHp(VehiclePartIndex.Body,GetFireHpCost(vehicle));
        }
    }
}
