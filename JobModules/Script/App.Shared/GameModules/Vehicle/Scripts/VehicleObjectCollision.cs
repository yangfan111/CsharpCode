using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Common;
using Core.EntityComponent;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Prediction.VehiclePrediction.TimeSync;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Vehicle
{
    public class VehicleObjectCollision : VehicleCollisionBase
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(VehicleObjectCollision));

        void OnCollisionEnter(Collision collision)
        {
            //DebugCollision(collision);
            if (collision.impulse.sqrMagnitude < 1E-4f)
            {
                return;
            }

            //Debug.Log("Collision Enter ....");
            if (collision.collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.Default) ||
                collision.collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.Terrain) ||
                collision.collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.Vehicle) ||
                collision.collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.NoCollisionWithBullet))
            {
                DebugCollision(collision);
                var entityRef = GetComponent<EntityReference>();
                if (entityRef != null)
                {
                    var vehicle = (VehicleEntity) entityRef.Reference;
                    if (!vehicle.hasVehicleType || !vehicle.hasGameObject)
                    {
                        _logger.ErrorFormat("VehicleEntity {0} is not exist", vehicle);
                        return;
                    }
                    var deltaVel = GetCollisionVelocityChange(vehicle).magnitude * 3.6f;
                    var collider = collision.contacts[0].thisCollider;
#if UNITY_EDITOR
                    if (DebugEnable)
                    {
                        Debug.LogFormat("Vehicle Collision Collider {0}", collider.name);
                    }
#endif
                    //Debug.LogFormat("deltavel is {0}, {1}, {2} {3}", deltaVel, GetCollisionVelocityChange(vehicle).magnitude, vehicle.GetPrevLinearVelocity().magnitude, vehicle.GetLinearVelocity().magnitude);
                    float factorA, factorB, boxFactor;
                    vehicle.GetCollisionObjectFactor(collider, out factorA, out factorB, out boxFactor);

                    DoCollisionDamageToPassagers(vehicle, CalcObjectCollisionDamageToPlayer(factorA, deltaVel));
                    DoCollisionDamageToVehicle(vehicle, CalcObjectCollisionDamageToVehicle(factorB, boxFactor, deltaVel));
                }
            }
        }


        private float CalcObjectCollisionDamageToPlayer(float factor, float velocity)
        {
            var damage = velocity > 30 ? ((velocity - 30) * 0.5f * factor) * 0.01f : 0.0f;

#if UNITY_EDITOR
            if (DebugEnable && DamageEnabled && damage > 0)
            {
                Debug.LogFormat("Vehicle-Player Collision velocity {0} fractor {1}, damagepercent {2}", velocity, factor, damage);
            }
#endif

            return damage;
        }

        private float CalcObjectCollisionDamageToVehicle(float factor, float boxFactor, float velocity)
        {
            var damage = velocity > 30 ? ((velocity - 30) * factor * boxFactor + 5) * 0.01f : 0.0f;

#if UNITY_EDITOR
            if (DebugEnable && DamageEnabled && damage > 0)
            {
                Debug.LogFormat("Vehicle-Scene Collision velocity {0}, factor {1} boxfacotr {2} damagepercent {3}", velocity, factor, boxFactor, damage);
            }
#endif

            return damage;
        }


        //
#if UNITY_EDITOR
        private List<GameObject> _debugSpheres = new List<GameObject>();
#endif
        void DebugCollision(Collision collision)
        {
#if UNITY_EDITOR
            if (!DebugEnable)
            {
                return;
            }

            var contacts = collision.contacts;
            int diff = contacts.Length - _debugSpheres.Count;
            for (int i = 0; i < diff; ++i)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.parent = transform;
                go.GetComponent<Collider>().enabled = false;
                _debugSpheres.Add(go);
                go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }

            for (int i = 0; i < contacts.Length; ++i)
            {
                Debug.LogFormat("Time {0} this collider {1} other collider {2} vel {3}",
                    Time.time,
                    collision.contacts[i].thisCollider.name,
                    collision.contacts[i].otherCollider.name,
                    GetComponent<Rigidbody>().velocity);

                _debugSpheres[i].transform.position = contacts[i].point;

            }
#endif
        }

        private Vector3 GetCollisionVelocityChange(VehicleEntity vehicle)
        {
            var prevVelocity = vehicle.GetPrevLinearVelocity();
            var velocity = vehicle.GetLinearVelocity();
            // Debug.LogFormat("Collision Change From {0} to {1}", prevVelocity.ToString("f10"), velocity.ToString("f10"));


            //If the velocity after the collision is opposite to the original velocity, the original velocity will be calculated;
            if ((velocity - prevVelocity).sqrMagnitude > prevVelocity.sqrMagnitude)
           {
               return prevVelocity;
           }
           
            return velocity - prevVelocity;
        }

        private void DoCollisionDamageToVehicle(VehicleEntity vehicle, float damage)
        {
            var entityRef = GetComponent<EntityReference>();
            if (entityRef == null)
            {
                return;
            }

            var myVehicle = (VehicleEntity) entityRef.Reference;
            if (!myVehicle.hasVehicleCollisionDamage)
            {
                return;
            }

            if (damage > 0 && DamageEnabled)
            {
                var collisionDamages = myVehicle.vehicleCollisionDamage.CollisionDamages;

                var key = vehicle.entityKey.Value;
                
                if (!collisionDamages.ContainsKey(key))
                {
                    collisionDamages[key] = new VehicleCollisionDamage(); 
                }

                var damages = collisionDamages[key];
                damages.AddVehicleDamage(damage);
                collisionDamages[key] = damages;

                _logger.InfoFormat("Vehicle {0} collision damage {1}", vehicle.entityKey.Value.EntityId, damage);
            }
        }

        private void DoCollisionDamageToPassagers(VehicleEntity vehicle,float damage)
        {
            var myVehicle = (VehicleEntity)GetComponent<EntityReference>().Reference;
            if (!myVehicle.hasVehicleCollisionDamage)
            {
                return;
            }

            if (damage > 0 && DamageEnabled)
            {
                var collisionDamages = myVehicle.vehicleCollisionDamage.CollisionDamages;
                var key = vehicle.entityKey.Value;

                if (!collisionDamages.ContainsKey(key))
                {
                    collisionDamages[key] = new VehicleCollisionDamage();
                }

                var damages = collisionDamages[key];
                damages.AddPassagerDamage(damage);
                collisionDamages[key] = damages;

                _logger.DebugFormat("Vehicle {0} collision damage {1} to all passagers", vehicle.entityKey.Value.EntityId, damage);
            }
        }
       
    }
}
