using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components;
using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Vehicle;
using Core.EntityComponent;
using Core.Utils;
using Shared.Scripts.Vehicles;
using UnityEngine;
using Utils.Singleton;

namespace App.Shared.DebugHandle
{

    public static class VehicleDebugUtility
    {
        public static bool ShowDebugInfo = false;

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(VehicleDebugUtility));
        public static void ShowExplosionRange(VehicleContext context, PlayerEntity player, bool show,
            float radius)
        {
            var vehicles = context.GetGroup(VehicleMatcher.EntityKey).GetEntities();
            string smallRangeObject = "test_small_explosion_range";

            foreach (var vehicle in vehicles)
            {
                var color = Color.red;
                color.a = 1.0f;
                var range = GetSphereRange(vehicle, smallRangeObject, color);
                if (radius > 0)
                {
                    range.transform.localScale = new Vector3(2 * radius, 2 * radius, 2 * radius);
                }

                range.GetComponent<MeshRenderer>().enabled = show;

                /*  color = Color.blue;
                  color.a = 0.4f;
                  range = GetSphereRange(vehicle, largeRangeObject, largeRadius, color);
                  range.GetComponent<MeshRenderer>().enabled = show;*/
            }
        }

        private static GameObject GetSphereRange(VehicleEntity vehicle, string objName, Color color)
        {
            var go = vehicle.gameObject.UnityObject.AsGameObject;
            for (int i = 0; i < go.transform.childCount; ++i)
            {
                var child = go.transform.GetChild(i);
                if (child.gameObject.name.Equals(objName))
                {
                    return child.gameObject;
                }
            }

            var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            s.GetComponent<SphereCollider>().enabled = false;

            s.transform.parent = go.transform;
            s.transform.localPosition = Vector3.zero;
            s.name = objName;

            var render = s.GetComponent<MeshRenderer>();
            render.sharedMaterial.color = color;
            return s;
        }

        public static void DragCar(VehicleContext context, PlayerEntity player)
        {
            var vehicles = context.GetGroup(VehicleMatcher.GameObject).GetEntities();

            var position = player.position.Value;
            var minSqrDist = float.MaxValue;
            VehicleEntity nearestVehicle = null;
            foreach (var vehicle in vehicles)
            {
                if (vehicle.HasAnyPassager() || (vehicle.hasVehicleDrivable && !vehicle.vehicleDrivable.IsDrivable) ||
                    !vehicle.IsCar())
                {
                    continue;
                }

                var sqrDist = (vehicle.GetDynamicData().Position - position).sqrMagnitude;
                if (sqrDist < minSqrDist)
                {
                    nearestVehicle = vehicle;
                    minSqrDist = sqrDist;
                }
            }

            if (nearestVehicle != null)
            {
                var dragPosition = position + new Vector3(0.0f, 5.0f, 0.0f);
                var dynamicData = nearestVehicle.GetDynamicData();
                dynamicData.Position = dragPosition;
                var go = nearestVehicle.gameObject.UnityObject.AsGameObject;
                go.transform.position = dragPosition;
            }
        }

        public static void ToggleVehicleDebugInfo()
        {
            ShowDebugInfo = !ShowDebugInfo;
        }

        public static void SetVehicleHp(VehicleContext context, int id, int hp)
        {
            var vehicle = GetVehicle(context, id);
            if (vehicle != null)
            {
                vehicle.GetGameData().Hp = hp;
            }
        }

        public static void SetVehicleFuel(VehicleContext context, int id, int fuelVolume)
        {
            var vehicle = GetVehicle(context, id);
            if (vehicle != null)
            {
                vehicle.GetGameData().RemainingFuel = fuelVolume;
            }
        }

        public static void EnableVehicleCollisionDamage(bool enabled)
        {
            VehicleCollisionBase.DamageEnabled = enabled;
        }

        public static void EnableVehicleCollisionDebug(bool enabled)
        {
            VehicleCollisionBase.DebugEnable = enabled;
        }

        private static void SetVehicleInput(VehicleContext context, int id, VechileDebugInput inputType, float val)
        {
            var vehicle = GetVehicle(context, id);
            if(vehicle != null)
                vehicle.SetDebugInput(inputType, val);
        }

        public static void SetVehicleInput(VehicleContext context, string[] args)
        {
            if (args.Length == 5)
            {
                SetVehicleInput(context, int.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]), float.Parse(args[4]));
            }else if (args.Length == 3)
            {
                var type = args[1];
                if (type == "T" || type == "t")
                {
                    SetVehicleInput(context, int.Parse(args[0]), VechileDebugInput.Throttle,
                        float.Parse(args[2]));
                }
                else if (type == "S" || type == "s")
                {
                    SetVehicleInput(context, int.Parse(args[0]), VechileDebugInput.Steer,
                        float.Parse(args[2]));
                }
                else if (type == "H" || type == "h")
                {
                    SetVehicleInput(context, int.Parse(args[0]), VechileDebugInput.HandBrake,
                        float.Parse(args[2]));
                }
                else if (type == "B" || type == "b")
                {
                    SetVehicleInput(context, int.Parse(args[0]), VechileDebugInput.Brake,
                        float.Parse(args[2]));
                }
                else
                {
                    SetVehicleInput(context, int.Parse(args[0]), float.Parse(args[1]),
                        float.Parse(args[2]));
                }
            }
        }

        public static void SetVehicleInput(VehicleContext context, int id, float throttle, float steer)
        {
            var vehicle = GetVehicle(context, id);
            if (vehicle != null)
            {
                vehicle.SetDebugInput(VechileDebugInput.Throttle, throttle);
                vehicle.SetDebugInput(VechileDebugInput.Steer, steer);
            }
        }

        private static void SetVehicleInput(VehicleContext context, int id, float throttle, float steer, float brake, float handbrake)
        {
            var vehicle = GetVehicle(context, id);
            if (vehicle != null)
            {
                vehicle.SetDebugInput(VechileDebugInput.Throttle, throttle);
                vehicle.SetDebugInput(VechileDebugInput.Steer, steer);
                vehicle.SetDebugInput(VechileDebugInput.Brake, brake);
                vehicle.SetDebugInput(VechileDebugInput.HandBrake, handbrake);
            }
        }

        public static void SetVehicleDynamicPrediction(VehicleContext context, bool isPredict)
        {
            if (SharedConfig.DynamicPrediction == isPredict)
            {
                return;
            }
            
            var vehicles = context.GetEntities();
            foreach (var vehicle in vehicles)
            {
                if (vehicle.hasEntityKey)
                {
                    if (SharedConfig.IsServer)
                    {
                        if (vehicle.HasDriver())
                        {
                            vehicle.SetKinematic(!isPredict);
                        }
                    }
                    else
                    {
                        if (!vehicle.HasDriver())
                        {
                            vehicle.SetKinematic(!isPredict);
                        }

                        VehicleStateUtility.SetVehicleSyncLatest(vehicle, isPredict);
                    }
                }
            }

            SharedConfig.DynamicPrediction = isPredict;
        }

        public static void ShowVehicles(VehicleContext context, bool isServer)
        {
            if (SharedConfig.IsServer == isServer)
            {
                var vehicles = context.GetEntities();
                foreach (var vehicle in vehicles)
                {
                    if (vehicle.hasGameObject)
                    {
                        var go = vehicle.gameObject.UnityObject.AsGameObject;

                        if (go.activeSelf)
                        {
                            var linVel = vehicle.GetLinearVelocity();
                            var position = go.transform.position;
                            var eulerAngles = go.transform.rotation.eulerAngles;

                            _logger.InfoFormat("vehicle name {0} position {1} rotation {2} linearVelocity {3}",
                                go.name, position, eulerAngles, linVel);
                        }
                    }
                }
            }
            
        }

        public static void ResetVehicle(VehicleContext context, int id)
        {
            var vehicle = GetVehicle(context, id);
            if (vehicle != null)
            {
                var components = vehicle.GetComponents();
                foreach (var component in components)
                {
                    var resetableComp = component as IVehicleResetableComponent;
                    if (resetableComp != null)
                    {
                        resetableComp.Reset();
                    }
                }

                var go = vehicle.gameObject.UnityObject.AsGameObject;
                var eulerAngles = go.transform.eulerAngles;
                go.transform.eulerAngles = new Vector3(0.0f, eulerAngles.y, 0.0f);
                vehicle.Reset(true);
            }
        }

        public static void EnableVehicleCull(bool isServer, bool enabled)
        {
            if (SharedConfig.IsServer == isServer)
                SharedConfig.DisableVehicleCull = !enabled;
        }

        public static void SetVehicleActiveUpdateRate(bool isServer, int rate)
        {
            if (SharedConfig.IsServer == isServer)
                SharedConfig.VehicleActiveUpdateRate = Math.Max(1, rate);
        }

        public static void CreateVehicleOffline(PlayerEntity player, int vehicleId, int count)
        {
            if (SharedConfig.IsOffline)
            {
                var position = player.position.Value + new Vector3(3, 0, 0);

                var startPosition = position + new Vector3(0, 1000, 0);
                RaycastHit hitInfo;
                if (Physics.Raycast(startPosition, new Vector3(0, -1, 0), out hitInfo, 2000.0f,
                    UnityLayers.AllCollidableLayerMask))
                {
                    position = hitInfo.point + new Vector3(0, 5.0f, 0);
                }

                SingletonManager.Get<VehicleDebugCommonData>().CreateVehicle(position, vehicleId, count);

                
            }
        }

        private static VehicleEntity GetVehicle(VehicleContext context, int id)
        {
            if (SharedConfig.IsOffline)
            {
                id += EntityIdGenerator.LocalBaseId;
            }

            var entityKey = new EntityKey(id, (short) EEntityType.Vehicle);
            return context.GetEntityWithEntityKey(entityKey);
        }
    }
}
