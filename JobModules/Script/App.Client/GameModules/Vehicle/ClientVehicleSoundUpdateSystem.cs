using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle;
using App.Shared.Sound;
using Core.Configuration;
using Core.GameModule.Interface;
using Core.IFactory;
using Core.Sound;
using Core.Utils;
using Entitas;

namespace App.Client.GameModules.Vehicle
{
    public class ClientVehicleSoundUpdateSystem : IGamePlaySystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientVehicleSoundUpdateSystem));

        private VehicleContext _vehicleContext;
        private PlayerContext _playerContext;
        private ClientVehicleSoundManager _soundManager;
        public ClientVehicleSoundUpdateSystem(Contexts contexts,ISoundEntityFactory soundEntityFactory, ISoundPlayer soundPlayer)
        {
            _vehicleContext = contexts.vehicle;
            _playerContext = contexts.player;
            _soundManager = new ClientVehicleSoundManager(soundEntityFactory, soundPlayer);;
        }

        public void OnGamePlay()
        {
            var selfPlayer = _playerContext.flagSelfEntity;
            if (selfPlayer.IsOnVehicle())
            {
                var controlledVehicle = selfPlayer.controlledVehicle;
                if (selfPlayer.IsOnVehicle())
                {
                    var vehicle = _vehicleContext.GetEntityWithEntityKey(selfPlayer.controlledVehicle.EntityKey);
                    if (vehicle != null && vehicle.HasGameData())
                    {
                        var gameData = vehicle.GetGameData();
                       
                        if (controlledVehicle.CurrentSoundId != gameData.CurrentSoundId)
                        {
                            if (_soundManager.Play(gameData.CurrentSoundId, gameData.SoundSyncTime))
                            {
                                _logger.DebugFormat("Change Music Sound From {0} to {1}",
                                    controlledVehicle.CurrentSoundId, gameData.CurrentSoundId);

                                controlledVehicle.CurrentSoundId = gameData.CurrentSoundId;
                            }
                            else
                            {
                                controlledVehicle.CurrentSoundId = (int) EVehicleSoundId.WaitingForPlay;
                            }
                        }
                    }
                }
                else
                {
                    _soundManager.StopSound();
                    controlledVehicle.CurrentSoundId = (int)EVehicleSoundId.Invalid;
                }
            }
            
        }
    }
}
