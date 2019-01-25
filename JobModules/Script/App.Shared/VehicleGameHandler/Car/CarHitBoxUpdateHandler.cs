using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Vehicle;
using App.Shared.GameModules.Vehicle.WheelCarrier;
using Core.GameHandler;
using Entitas;
using EVP;
using XmlConfig;

namespace App.Shared.VehicleGameHandler
{
    public class CarHitBoxUpdateHandler : VehicleUpdateHandler
    {
        public CarHitBoxUpdateHandler() : base(new VehicleTypeMatcher(EVehicleType.Car))
        {
            
        }

        protected override void DoUpdate(VehicleEntity vehicle)
        {
            var hitBoxComp = vehicle.carHitBox;

            var config = WheelEntityUtility.GetCarConfig(vehicle);
            hitBoxComp.BodyPosition = config.hitBoxRoot.position;
            hitBoxComp.BodyRotation = config.hitBoxRoot.rotation;

            var flexibleCount = config.flexibleHitBoxes.Length;
            for (int i = 0; i < flexibleCount; ++i)
            {
                var flexibleTransform = config.flexibleHitBoxes[i];
                if (flexibleTransform != null)
                {
                    hitBoxComp.FlexiblePositionList[i] = flexibleTransform.position;
                    hitBoxComp.FlexibleRotationList[i] = flexibleTransform.rotation;
                }
            }

            var hitBoxList = config.wheelHitBoxes;
            int wheelCount = hitBoxList.Length;
            for (int i = 0; i < wheelCount; i++)
            {
                var hitBoxTransform = config.wheelHitBoxes[i].GetHitBoxRoot();
                if (hitBoxTransform != null)
                {
                    hitBoxComp.WheelPositionList[i] = hitBoxTransform.position;
                    hitBoxComp.WheelRotationList[i] = hitBoxTransform.rotation;
                }
            }
        }
    }
}
