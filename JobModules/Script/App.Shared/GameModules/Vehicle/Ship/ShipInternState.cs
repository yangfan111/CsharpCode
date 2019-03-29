using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Shared.GameModules.Vehicle.Ship
{
    public struct RudderInternState
    {
        public float Angle;
    }

    public class ShipInternState : VehicleInternState
    {
        public ShipInternState()
        {
            RudderStates = new RudderInternState[2];
        }

        public RudderInternState[] RudderStates;
    }
}
