using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle;
using Core.GameModule.System;
using Entitas;

namespace App.Server.GameModules.Vehicle
{
    public class RideOffVehicleOnPlayerDestroy : ReactiveEntityInitSystem<PlayerEntity>
    {
        private Contexts _contexts;

        public RideOffVehicleOnPlayerDestroy(Contexts contexts) : base(contexts.player)
        {
            _contexts = contexts;
        }

        protected override ICollector<PlayerEntity> GetTrigger(IContext<PlayerEntity> context)
        {
            return context.CreateCollector(PlayerMatcher.FlagDestroy.Added());
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return entity.IsOnVehicle();
        }

        public override void SingleExecute(PlayerEntity player)
        {
            var vehicleContext = _contexts.vehicle;
            var vehicle = vehicleContext.GetEntityWithEntityKey(player.controlledVehicle.EntityKey);
            if (vehicle != null)
            {
                vehicle.UnseatPlayer(player);
            }
        }
    }
}