using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Vehicle;
using App.Shared.GameModules.Vehicle.Ship;
using XmlConfig;

namespace App.Shared.VehicleGameHandler
{
    public class ShipHitBoxUpdateHandler : VehicleUpdateHandler
    {

        public ShipHitBoxUpdateHandler() : base(new VehicleTypeMatcher(EVehicleType.Ship))
        {
            
        }

        protected override void DoUpdate(VehicleEntity vehicle)
        {
            var hitBoxComp = vehicle.shipHitBox;

            var config = ShipEntityUtility.GetShipConfig(vehicle);
            hitBoxComp.BodyPosition = config.hitBoxRoot.position;
            hitBoxComp.BodyRotation = config.hitBoxRoot.rotation;

            var hitBoxList = config.rudderHitBoxes;
            int wheelCount = hitBoxList.Length;
            for (int i = 0; i < wheelCount; i++)
            {
                var hitBoxTransform = config.rudderHitBoxes[i];
                if (hitBoxTransform != null)
                {
                    hitBoxComp.RudderPositionList[i] = hitBoxTransform.position;
                    hitBoxComp.RudderRotationList[i] = hitBoxTransform.rotation;
                }
            }
        }
    }
}
