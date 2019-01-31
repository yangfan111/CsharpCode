using System;
using App.Shared.Components.Player;
using App.Shared.GameModules.Common;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Vehicle
{
    public class PlayerVehicleCollision : VehicleCollisionBase
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerVehicleCollision));

        public Contexts AllContext;

        private PlayerEntity _playerEntity;
        private float _lastCollisionTime;
        private int _collidedTriggerCount;

        void OnEnable()
        {
            _playerEntity = (PlayerEntity)GetComponent<EntityReference>().Reference;
            _lastCollisionTime = 0;
            _collidedTriggerCount = 0;
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.VehicleTrigger))
            {
                _collidedTriggerCount++;

                var vehicle = VehicleEntityUtility.GetVehicleFromChildCollider(collider);
                if (vehicle != null)
                {
                    SetCollisionState(vehicle, EVehicleCollisionState.Enter);

                    var collisionTime = Time.time;
                    if (collisionTime - _lastCollisionTime > 0.34f)
                    {
                        var damage = CalcPlayerCollisionDamage(vehicle);
                        if (DamageEnabled && damage > 0.0f)
                        {
                            var sourcePlayer = vehicle.hasOwnerId ? AllContext.player.GetEntityWithEntityKey(vehicle.ownerId.Value) : null;
                            VehicleDamageUtility.DoPlayerDamage(AllContext, sourcePlayer, _playerEntity, damage);
                        }
                        _lastCollisionTime = collisionTime;
                    }
                }
            }
        }

        private float CalcPlayerCollisionDamage(VehicleEntity vehicle)
        {
            float factor;
            vehicle.GetCollisionPlayerFactor(out factor);

            var velocity = vehicle.GetLinearVelocity().magnitude * 3.6f;

            float damage = Int32.MaxValue;

            if (velocity <= 50.0f)
            {
                damage = velocity * factor;
            }

#if UNITY_EDITOR
            if (DebugEnable && DamageEnabled && damage > 0)
            {
                Debug.LogFormat("Player-Vehicle Collision velocity {0} factor {1} damage {2}", velocity, factor, damage);
            }
#endif
            return damage;
        }

        void OnTriggerStay(Collider collider)
        {
            if (collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.VehicleTrigger))
            {
                var vehicle = VehicleEntityUtility.GetVehicleFromChildCollider(collider);
                if (vehicle != null)
                {
                    SetCollisionState(vehicle, EVehicleCollisionState.Stay);
                }
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.VehicleTrigger))
            {
                _collidedTriggerCount--;

                if (_collidedTriggerCount <= 0)
                {
                    var vehicle = VehicleEntityUtility.GetVehicleFromChildCollider(collider);
                    if (vehicle != null)
                    {
                        SetCollisionState(vehicle, EVehicleCollisionState.None);
                    }

                    _collidedTriggerCount = 0;
                }
            }
        }

        void SetCollisionState(VehicleEntity vehicle, EVehicleCollisionState state)
        {
            var gamePlay = _playerEntity.gamePlay;
            gamePlay.VehicleCollisionState = state;
            gamePlay.CollidedVehicleKey = vehicle.entityKey.Value;
        }
    }
}
