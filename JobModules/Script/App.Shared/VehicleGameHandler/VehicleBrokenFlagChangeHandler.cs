using System.Collections.Generic;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle;
using Core.EntityComponent;
using Core.Enums;
using Core.GameHandler;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using Entitas;
using UnityEngine;

namespace App.Shared.VehicleGameHandler
{
    public abstract class VehicleBrokenFlagChangeHandler : VehicleStateChangeHandler
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(VehicleBrokenFlagChangeHandler));
        private const float MaxOverlapRadius = 20.0f;

        private PlayerContext _playerContext;
        private Contexts _contexts;

        private HashSet<Entity> _processedEntities;

        public VehicleBrokenFlagChangeHandler(Contexts contexts, VehicleTypeMatcher matcher) : base(matcher)
        {
            _contexts = contexts;
            _playerContext = contexts.player;
            _processedEntities = new HashSet<Entity>();
        }

        protected override void DoHandle(GameEvent evt, VehicleEntity vehicle)
        {
            var comp = vehicle.vehicleBrokenFlag;

            if (comp.IsBodyBroken() && !comp.IsBodyColliderBroken())
            {
                SetBodyBroken(vehicle);
            }

            OnBodyBroken(vehicle);

            _processedEntities.Clear();
        }

        private void SetBodyBroken(VehicleEntity vehicle)
        {
            vehicle.SetControllerBroken();
            vehicle.vehicleSeat.SetAllSeatBroken();
            vehicle.vehicleBrokenFlag.SetBodyColliderBroken();
            DoExplosionDamangeToPassager(vehicle);
            DoExlopsionDamageToNeighboringObjects(vehicle);
            SetStatisticsData(vehicle);
        }

        private void DoExplosionDamangeToPassager(VehicleEntity vehicle)
        {
            PlayerEntity sourcePlayer = null;
            var damageType = GetDamageType(vehicle, out sourcePlayer);
            VehicleDamageUtility.DoDamageToAllPassgers(_contexts, vehicle, float.MaxValue, damageType, sourcePlayer);
        }

        private void DoExlopsionDamageToNeighboringObjects(VehicleEntity vehicle)
        {
            var explosionCenter = vehicle.GetExplosionCenter();
            var layerMask = UnityLayerManager.GetLayerMask(EUnityLayerName.Player) | UnityLayerManager.GetLayerMask(EUnityLayerName.Vehicle);
            Collider[] colliders = Physics.OverlapSphere(explosionCenter, MaxOverlapRadius, layerMask);
            DoExplosionDamageToColliders(vehicle, colliders, explosionCenter);

        }

        private void DoExplosionDamageToColliders(VehicleEntity vehicle, Collider[] colliders, Vector3 explosionCenter)
        {
            
            foreach (var collider in colliders)
            {

                if (collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.Player))
                {
                   
                    DoExplosionDamangeToPlayer(vehicle, collider, explosionCenter);
                }
                else if (collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.Vehicle))
                {
                    DoExplosionDamangeToVehicle(vehicle, collider, explosionCenter);
                }
            }
        }

        private float CalcExplosionDamageToPlayer(VehicleEntity vehicle, PlayerEntity player, Vector3 explosionCenter)
        {
            var dist = (player.position.Value - explosionCenter).magnitude;
            float factorA, factorB;
            vehicle.GetExplosionToPlayerFactor(out factorA, out factorB);

            var damage = Mathf.Max((factorA - dist), 0) / factorA * factorB;

            if (_logger.IsDebugEnabled && damage > 0)
            {
                Debug.LogFormat("Explosion Damage to player {0}, dist {1}, damage {2}", 
                    player.entityKey.Value.EntityId, 
                    dist,
                    damage);
            }

            return damage;
        }

        private float CalcExplosionDamageToObject(VehicleEntity explodedVehicle, VehicleEntity vehicle, Vector3 explosionCenter)
        {
            var dist = (vehicle.position.Value - explosionCenter).magnitude;
            float factorA, factorB;
            explodedVehicle.GetExplosionToVehicleFactor(out factorA, out factorB);

            var damage = Mathf.Max((factorA - dist), 0) / factorA * factorB;

            if (_logger.IsDebugEnabled && damage > 0)
            {
                Debug.LogFormat("Explosion Damage to vehicle {0}, dist {1}, damage {2}",
                    vehicle.entityKey.Value.EntityId,
                    dist,
                    damage);
            }

            return damage;
        }

        private void DoExplosionDamangeToPlayer(VehicleEntity explodedVehicle, Collider collider, Vector3 explosionCenter)
        {
            var player = PlayerEntityUtility.GetPlayerFromChildCollider(collider);
            if (player == null || _processedEntities.Contains(player))
            {
                return;
            }

            _processedEntities.Add(player);

            var damage = CalcExplosionDamageToPlayer(explodedVehicle, player, explosionCenter);

            PlayerEntity sourcePlayer;
            var damageType = GetDamageType(explodedVehicle, out sourcePlayer);

            VehicleDamageUtility.DoPlayerDamage(_contexts, sourcePlayer, player, damage, damageType);
        }

        private EUIDeadType GetDamageType(VehicleEntity explodedVehicle, out PlayerEntity sourcePlayer)
        {
            var gameData = explodedVehicle.GetGameData();
            var damageType = EUIDeadType.VehicleBomb;
            var damgeSourceEntityKey = gameData.LastDamageSource;
            sourcePlayer = null;
            if (gameData.LastDamageType == EUIDeadType.Weapon && !damgeSourceEntityKey.Equals(EntityKey.Default))
            {
                sourcePlayer = _playerContext.GetEntityWithEntityKey(damgeSourceEntityKey);
            }
            else if (gameData.LastDamageType == EUIDeadType.Bombing)
            {
                damageType = EUIDeadType.Bombing;
            }
            return damageType;
        }

        private void DoExplosionDamangeToVehicle(VehicleEntity explodedVehicle, Collider collider, Vector3 explosionCenter)
        {
            var vehicle = VehicleEntityUtility.GetVehicleFromChildCollider(collider);
            if (vehicle == null || vehicle == explodedVehicle || _processedEntities.Contains(vehicle))
            {
                return;
            }
            _processedEntities.Add(vehicle);

            var damage = CalcExplosionDamageToObject(explodedVehicle, vehicle, explosionCenter);

            var gameData = vehicle.GetGameData();
            var indexArray = VehicleIndexHelper.GetAllPartIndexArray();
            foreach(var index in indexArray)
            {
                gameData.DecreaseHp(index, damage);
            }

            //Debug.LogFormat("explosion damange vehicle id {0} damage {1}", vehicle.entityKey.Value.EntityId, damage);
            _logger.InfoFormat("explosion damange vehicle id {0} damage {1}", vehicle.entityKey.Value.EntityId, damage);
        }

        private void SetStatisticsData(VehicleEntity explodedVehicle)
        {
            var gameData = explodedVehicle.GetGameData();
            var damgeSourceEntityKey = gameData.LastDamageSource;

            PlayerEntity sourcePlayer = null;
            if (gameData.LastDamageType == EUIDeadType.Weapon && !damgeSourceEntityKey.Equals(EntityKey.Default))
            {
                sourcePlayer = _playerContext.GetEntityWithEntityKey(damgeSourceEntityKey);
            }
            if (null != sourcePlayer)
            {
                //摧毁载具数量
                sourcePlayer.statisticsData.Statistics.DestroyVehicle++;
            }
        }

        protected abstract void OnBodyBroken(VehicleEntity vehicle);
    }
}
