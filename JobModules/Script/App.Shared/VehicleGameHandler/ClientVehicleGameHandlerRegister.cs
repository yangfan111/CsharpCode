using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.GameHandler;

namespace App.Shared.VehicleGameHandler
{
    public class ClientVehicleGameHandlerRegister : BaseGameHandlerRegister
    {
        public ClientVehicleGameHandlerRegister(Contexts contexts)
        {
            RegisterUpdateHandler(new VehicleCollisionDamageHandler(contexts));
            RegisterUpdateHandler(new CarHitBoxUpdateHandler());
            RegisterUpdateHandler(new ShipHitBoxUpdateHandler());


            bool isOffline = SharedConfig.IsOffline;
            if (SharedConfig.IsOffline)
            {
                RegisterUpdateHandler(new VehicleFuelUpdateHandler());
                RegisterEventHandler(GameEvent.VehicleCrash, new VehicleCrashedHandler(contexts));
                RegisterUpdateHandler(new ShipSleepingStateUpdateHandler());
                RegisterUpdateHandler(new VehicleSoundUpdateHandler());
            }
            RegisterUpdateHandler(new VehicleSyncEventSendHandler(contexts.session.clientSessionObjects, isOffline));

            RegisterEventHandler(GameEvent.VehicleSeatOccupationChange, new VehicleSeatOccupationChangeHandler(contexts.player, isOffline, false));
            RegisterEventHandler(GameEvent.VehicleHpChange, new CarHpChangeHandler(isOffline, false));
            RegisterEventHandler(GameEvent.VehicleFuelChange, new VehicleFuelChangeHandler());
            RegisterEventHandler(GameEvent.CarFirstWheelHpChange, new CarWheelHpChangeHandler(isOffline, false));
            RegisterEventHandler(GameEvent.CarSecondWheelHpChange, new CarWheelHpChangeHandler(isOffline, false));
            RegisterEventHandler(GameEvent.CarThirdWheelHpChange, new CarWheelHpChangeHandler(isOffline, false));
            RegisterEventHandler(GameEvent.CarFourthWheelHpChange, new CarWheelHpChangeHandler(isOffline, false));
            RegisterEventHandler(GameEvent.VehicleBrokenFlagChange, new CarBrokenFlagChangeHandler(contexts, isOffline, false));
            RegisterEventHandler(GameEvent.VehicleHpChange, new ShipHpChangeHandler(isOffline, false));
            RegisterEventHandler(GameEvent.VehicleBrokenFlagChange, new ShipBrokenFlagChangeHandler(contexts));   
        }
    }
}
