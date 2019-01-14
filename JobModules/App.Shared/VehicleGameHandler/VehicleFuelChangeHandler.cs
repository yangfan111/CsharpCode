using App.Shared.GameModules.Vehicle;
using Core.GameHandler;

namespace App.Shared.VehicleGameHandler
{
    class VehicleFuelChangeHandler : VehicleStateChangeHandler
    {

        protected override void DoHandle(GameEvent evt, VehicleEntity vehicle)
        {
            if (vehicle.HasDriver())
            {
                if (vehicle.GetGameData().RemainingFuel <= 0.0f)
                {
                    vehicle.EnableInput(false);
                    vehicle.EnableEngineAudio(false);
                }
                else
                {
                    vehicle.EnableInput(true);
                    vehicle.EnableEngineAudio(true);
                }
            }
            
        }
    }
}
