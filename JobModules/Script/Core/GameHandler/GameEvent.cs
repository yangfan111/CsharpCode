using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.GameHandler
{
    public enum GameEvent
    {
        None = 0,
        VehicleSeatOccupationChange,
        VehicleHpChange,
        VehicleFuelChange,
        VehicleBrokenFlagChange,
        VehicleCrash,
        CarFirstWheelHpChange,
        CarSecondWheelHpChange,
        CarThirdWheelHpChange,
        CarFourthWheelHpChange,

        MaxGameEventCount
    }
}
