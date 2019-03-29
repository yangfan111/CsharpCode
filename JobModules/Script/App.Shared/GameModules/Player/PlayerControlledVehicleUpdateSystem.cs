using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Entitas;

namespace App.Shared.GameModules.Player
{
    public class PlayerControlledVehicleUpdateSystem : IGamePlaySystem
    {
        private VehicleContext _vehicle;
        private IGroup<PlayerEntity> _players;
        private Contexts _contexts;
        public PlayerControlledVehicleUpdateSystem(Contexts context)
        {
            _vehicle = context.vehicle;
            _players = context.player.GetGroup(PlayerMatcher.ThirdPersonModel);
            _contexts = context;
        }

        public void OnGamePlay()
        {
            var playerEntities = _players.GetEntities();
            for(int i = 0; i < playerEntities.Length; ++i)
            {
                var player = playerEntities[i];
                player.SetCharacterStateWithVehicle(_contexts, _vehicle);
            }
        }
    }
}
