using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Vehicle;
using Core.EntityComponent;
using Core.Enums;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;

namespace App.Shared.VehicleGameHandler
{
    public class VehicleCollisionDamageHandler : VehicleUpdateHandler
    {

        private static LoggerAdapter _logger = _logger = new LoggerAdapter(typeof(VehicleCollisionDamageHandler));

        private PlayerContext _playerContext;
        private VehicleContext _vehicleContext;
        private Contexts _contexts;
        public VehicleCollisionDamageHandler(Contexts contexts)
        {
            _playerContext = contexts.player;
            _vehicleContext = contexts.vehicle;
            _contexts = contexts;
        }

        protected override void DoUpdate(VehicleEntity vehicle)
        {
            if (vehicle.hasVehicleCollisionDamage)
            {
                var collisionDamages = vehicle.vehicleCollisionDamage.CollisionDamages;
                ProcessCollisionDamages(collisionDamages);
            }
            
        }

        private void ProcessCollisionDamages(Dictionary<EntityKey, VehicleCollisionDamage> damageToVehicles)
        {
            if ((SharedConfig.IsServer && !SharedConfig.DynamicPrediction))
            {
                return;
            }

            foreach (var pair in damageToVehicles)
            {
                var vehicle = _vehicleContext.GetEntityWithEntityKey(pair.Key);

                if (vehicle != null)
                {
                    bool sendToServer = false;
                    if (SharedConfig.CalcVehicleCollisionDamageOnClient)
                    {
                        if (SharedConfig.IsServer)
                        {
                            //the collision damage for server-side vehicle with driver is calculated on client-side of driver player.  
                            if (vehicle.HasDriver())
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (!SharedConfig.IsOffline && SharedConfig.DynamicPrediction)
                            {
                                var selfEntity = _playerContext.flagSelfEntity;
                                //the collision damage for client-side vehicle with not-self driver is calculated on server-side.  
                                if (!vehicle.IsVehicleDriver(selfEntity))
                                {
                                    continue;
                                }
                            }

                            sendToServer = true;
                        }
                    }
                    
                    DoCollisionDamageToVehicle(vehicle, pair.Value.AverageVehicleDamage, sendToServer);
                    DoCollisionDamageToPassagers(vehicle, pair.Value.AveragePassagerDamage, sendToServer);
                }
            }

            damageToVehicles.Clear();;
        }

        private void DoCollisionDamageToVehicle(VehicleEntity vehicle, float damage, bool sendToServer)
        {
            if (damage > 0)
            {
                var gameData = vehicle.GetGameData();
                var curHp = gameData.Hp;
                var maxHp = gameData.MaxHp;
                gameData.DecreaseHp(VehiclePartIndex.Body, damage * maxHp);

                if (sendToServer)
                {
                    VehicleDamageUtility.SendDamageToServer(vehicle, vehicle.entityKey.Value, damage * maxHp);
                }
                _logger.InfoFormat("Vehicle {0} AVERAGE collision damage Hp {1} -> {2}", vehicle.entityKey.Value.EntityId, curHp, gameData.Hp);
            }
        }

        private void DoCollisionDamageToPassagers(VehicleEntity vehicle, float damage, bool sendToServer)
        {
            if (damage > 0)
            {
                VehicleDamageUtility.DoDamageToAllPassgers(_contexts, vehicle, damage, EUIDeadType.VehicleHit, null, true, sendToServer);

                _logger.InfoFormat("Vehicle {0} AVERAGE collision damage {1} to all passagers", vehicle.entityKey.Value.EntityId, damage);
            }
        }
    }
}
