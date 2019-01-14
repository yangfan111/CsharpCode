using App.Shared.Components.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlConfig;

namespace App.Shared.VehicleGameHandler
{
    public class ShipHpChangeHandler : VehicleHpChangeHandler
    {
        public ShipHpChangeHandler(bool isOffline, bool isServer) :
           base(isOffline, isServer, new VehicleTypeMatcher(EVehicleType.Ship))
        {

        }

        protected override void OnHpZero(VehicleEntity vehicle)
        {
            
        }
    }
}
