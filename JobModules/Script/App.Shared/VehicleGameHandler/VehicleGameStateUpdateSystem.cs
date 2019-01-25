using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Vehicle;
using Core.GameHandler;
using Core.GameModule.System;
using Entitas;

namespace App.Shared.VehicleGameHandler
{
    public class VehicleGameStateUpdateSystem : BaseGameStateUpdateSystem<VehicleEntity>
    {
        private VehicleContext _context;
        private IGroup<VehicleEntity> _vehicles;
        public VehicleGameStateUpdateSystem(VehicleContext context, BaseGameHandlerRegister handlerRegister) : base(handlerRegister)
        {
            _context = context;
            _vehicles = context.GetGroup(VehicleMatcher.AllOf(VehicleMatcher.VehicleGameEvent, VehicleMatcher.GameObject));
        }

        protected override VehicleEntity[] GetEntities()
        {
            return _vehicles.GetEntities();
        }

        protected override GameEventComponent GetGameEventComponent(VehicleEntity entity)
        {
            return entity.vehicleGameEvent;
        }

        protected override void CollectGameEvents(VehicleEntity vehicle, List<GameEvent> outEvents)
        {
            if (!vehicle.IsActiveSelf())
            {
                return;
            }

            if (vehicle.HasAnyPassager() && vehicle.IsCrashed())
            {
                vehicle.AddGameEvent(GameEvent.VehicleCrash);
            }

            if (vehicle.hasVehicleSeat)
            {
                if (vehicle.CompareAndStore("VehicleSeatComponent.OccupationFlag",
                    vehicle.vehicleSeat.OccupationFlag))
                {
                    outEvents.Add(GameEvent.VehicleSeatOccupationChange);
                }
            }

            if(vehicle.HasGameData())
            {
                var gameData = vehicle.GetGameData();
                if (vehicle.CompareAndStore("VehicleBaseGameDataComponent.Hp", gameData.Hp))
                {
                    outEvents.Add(GameEvent.VehicleHpChange);
                }

                if (vehicle.CompareAndStore("VehicleBaseGameDataComponent.RemainingFuel",
                    gameData.RemainingFuel))
                {
                    outEvents.Add(GameEvent.VehicleFuelChange);
                }
            }

            if (vehicle.hasCarGameData)
            { 
                if (vehicle.CompareAndStore("CarGameDataComponent.FirstWheelHp",
                    vehicle.carGameData.FirstWheelHp))
                {
                    outEvents.Add(GameEvent.CarFirstWheelHpChange);
                }

                if (vehicle.CompareAndStore("CarGameDataComponent.SecondWheelHp",
                    vehicle.carGameData.SecondWheelHp))
                {
                    outEvents.Add(GameEvent.CarSecondWheelHpChange);
                }

                if (vehicle.CompareAndStore("CarGameDataComponent.ThirdWheelHp",
                    vehicle.carGameData.ThirdWheelHp))
                {
                    outEvents.Add(GameEvent.CarThirdWheelHpChange);
                }

                if (vehicle.CompareAndStore("CarGameDataComponent.FourthWheelHp",
                    vehicle.carGameData.FourthWheelHp))
                {
                    outEvents.Add(GameEvent.CarFourthWheelHpChange);
                }
            }

            if (vehicle.hasVehicleBrokenFlag)
            {
                if (vehicle.CompareAndStore("VehicleBrokenFlagComponent.Flag",
                    vehicle.vehicleBrokenFlag.Flag))
                {
                    outEvents.Add(GameEvent.VehicleBrokenFlagChange);
                }
            }

        }

    }
}
