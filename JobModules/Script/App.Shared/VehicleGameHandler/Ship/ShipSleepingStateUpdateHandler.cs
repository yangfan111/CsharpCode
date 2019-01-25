using App.Shared.GameModules.Vehicle;
using XmlConfig;

namespace App.Shared.VehicleGameHandler
{
    public class ShipSleepingStateUpdateHandler : VehicleUpdateHandler
    {
        public ShipSleepingStateUpdateHandler() : base(new VehicleTypeMatcher(EVehicleType.Ship))
        {
            
        }

        protected override void DoUpdate(VehicleEntity vehicle)
        {
            if (vehicle.IsActiveSelf())
            {
                bool hasPassager = vehicle.HasAnyPassager();
                bool isSleepingDisable = vehicle.IsSleepingDisable();
                if (hasPassager && !isSleepingDisable)
                {
                    vehicle.DisableSleeping(true);
                }
                else if (!hasPassager && isSleepingDisable)
                {
                    vehicle.DisableSleeping(false);
                }
            }
        }
    }
}
