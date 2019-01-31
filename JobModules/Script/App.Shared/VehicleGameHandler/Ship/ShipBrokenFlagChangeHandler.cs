using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Vehicle;
using Core.Prediction.VehiclePrediction.Cmd;
using DWP;
using XmlConfig;

namespace App.Shared.VehicleGameHandler
{
    public class ShipBrokenFlagChangeHandler : VehicleBrokenFlagChangeHandler
    {
        public ShipBrokenFlagChangeHandler(Contexts contexts) : 
            base(contexts, new VehicleTypeMatcher(EVehicleType.Ship))
        {

        }

        protected override void OnBodyBroken(VehicleEntity vehicle)
        {
            var comp = vehicle.vehicleBrokenFlag;
            var indexArray = VehicleIndexHelper.GetRudderIndexArray();
            foreach (var index in indexArray)
            {
                if (comp.IsVehiclePartBroken(index) && !comp.IsVehiclePartColliderBroken(index))
                {
                    SetRudderBroken(vehicle, index);
                }
            }
        }

        private void SetRudderBroken(VehicleEntity vehicle, VehiclePartIndex index)
        {
            var controller = vehicle.GetController<AdvancedShipController>();
            var controllerIndex = VehicleIndexHelper.ToVehicleControllerRudderIndex(index);
            controller.rudders[controllerIndex].IsBroken = true;
        }
    }
}
