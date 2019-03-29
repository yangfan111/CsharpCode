using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.GameHandler;

namespace App.Shared.VehicleGameHandler
{
    public class ServerVehicleGameHandlerRegister : BaseGameHandlerRegister
    {

        public ServerVehicleGameHandlerRegister(Contexts contexts)
        {
            RegisterUpdateHandler(new VehicleCollisionDamageHandler(contexts));
            RegisterUpdateHandler(new VehicleSyncEventReceiveHandler(contexts));
            RegisterUpdateHandler(new VehicleFuelUpdateHandler());
            RegisterUpdateHandler(new CarHitBoxUpdateHandler());
            RegisterUpdateHandler(new ShipHitBoxUpdateHandler());
            RegisterUpdateHandler(new ShipSleepingStateUpdateHandler());
            RegisterUpdateHandler(new VehicleSoundUpdateHandler());

            RegisterEventHandler(GameEvent.VehicleSeatOccupationChange, new VehicleSeatOccupationChangeHandler(contexts.player, false, true));
            RegisterEventHandler(GameEvent.VehicleHpChange, new CarHpChangeHandler(false, true));
            RegisterEventHandler(GameEvent.VehicleFuelChange, new VehicleFuelChangeHandler());
            RegisterEventHandler(GameEvent.CarFirstWheelHpChange, new CarWheelHpChangeHandler(false, true));
            RegisterEventHandler(GameEvent.CarSecondWheelHpChange, new CarWheelHpChangeHandler(false, true));
            RegisterEventHandler(GameEvent.CarThirdWheelHpChange, new CarWheelHpChangeHandler(false, true));
            RegisterEventHandler(GameEvent.CarFourthWheelHpChange, new CarWheelHpChangeHandler(false, true));
            RegisterEventHandler(GameEvent.VehicleBrokenFlagChange, new CarBrokenFlagChangeHandler(contexts, false, true));
            RegisterEventHandler(GameEvent.VehicleHpChange, new ShipHpChangeHandler(false, true));
            RegisterEventHandler(GameEvent.VehicleBrokenFlagChange, new ShipBrokenFlagChangeHandler(contexts));
            RegisterEventHandler(GameEvent.VehicleCrash, new VehicleCrashedHandler(contexts));
        }
    }
}
