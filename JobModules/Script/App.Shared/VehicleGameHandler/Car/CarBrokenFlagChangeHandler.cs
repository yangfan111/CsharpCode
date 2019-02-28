using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Vehicle;
using App.Shared.GameModules.Vehicle.WheelCarrier;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using EVP;
using EVP.Scripts;
using UnityEngine;
using XmlConfig;

namespace App.Shared.VehicleGameHandler
{
    public class CarBrokenFlagChangeHandler : VehicleBrokenFlagChangeHandler
    {
        private bool _isOffline;
        private bool _isServer;
        public CarBrokenFlagChangeHandler(Contexts contexts, bool isOffline, bool isServer) : base(contexts, new VehicleTypeMatcher(EVehicleType.Car))
        {
            _isOffline = isOffline;
            _isServer = isServer;
        }

        protected override void OnBodyBroken(VehicleEntity vehicle)
        {
            var comp = vehicle.vehicleBrokenFlag;
            var indexArray = VehicleIndexHelper.GetWheelIndexArray();
            
            foreach (var index in indexArray)
            {
                if (WheelEntityUtility.HasWheel(vehicle, index) &&
                    comp.IsVehiclePartBroken(index) && 
                    !comp.IsVehiclePartColliderBroken(index))
                {
                    SetWheelBroken(vehicle, index);
                }
            }
        }

        private void SetWheelBroken(VehicleEntity vehicle, VehiclePartIndex index)
        {
            AssertUtility.Assert(index != VehiclePartIndex.Body);

            var controller = vehicle.GetController<VehicleAbstractController>();
            var controllerIndex = VehicleIndexHelper.ToVehicleControllerWheelIndex(index);
            controller.SetWheelBroken(controllerIndex);

            if (!controller.IsKinematic)
            {
                var config = controller.GetComponent<VehicleConfig>();
                var impulse = config.wheelExplosionImpulse;
                if (impulse > 0)
                {
                    var force = controller.transform.up * impulse * controller.cachedRigidbody.mass;
                    vehicle.AddImpulseAtPosition(force, controller.GetWheel(controllerIndex).wheelTransform.position);
                }

            }
            
            vehicle.vehicleBrokenFlag.SetVehiclePartColliderBroken(index);
        }
    }
}
