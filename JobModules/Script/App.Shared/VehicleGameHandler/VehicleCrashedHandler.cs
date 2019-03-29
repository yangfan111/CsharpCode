using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle;
using Core.EntityComponent;
using Core.GameHandler;
using Core.GameModule.System;
using Core.GameTime;

namespace App.Shared.VehicleGameHandler
{
    public class VehicleCrashedHandler : VehicleStateChangeHandler
    {
        private PlayerContext _context;
        private ICurrentTime _currentTimeObject;
        public VehicleCrashedHandler(Contexts contexts)
        {
            _context = contexts.player;
          
                _currentTimeObject = contexts.session.currentTimeObject;
        }

        protected override void DoHandle(GameEvent evt, VehicleEntity entity)
        {
            var seats = entity.vehicleSeat;
            ForcePlayerRideOff(seats.SeatedEntityKey1);
            ForcePlayerRideOff(seats.SeatedEntityKey2);
            ForcePlayerRideOff(seats.SeatedEntityKey3);
            ForcePlayerRideOff(seats.SeatedEntityKey4);
            ForcePlayerRideOff(seats.SeatedEntityKey5);
            ForcePlayerRideOff(seats.SeatedEntityKey6);
            entity.vehicleSeat.ClearOccupation();
        }

        private void ForcePlayerRideOff(EntityKey entityKey)
        {
            if (entityKey.EntityType == (short) EEntityType.Player &&
                entityKey.EntityId > 0)
            {
                var player = _context.GetEntityWithEntityKey(entityKey);
                if (player != null && player.IsOnVehicle())
                {
                    player.controlledVehicle.RideOff(_currentTimeObject.CurrentTime);
                }
            }
        }
    }
}
