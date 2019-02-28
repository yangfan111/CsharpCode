using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Vehicle.Common;
using App.Shared.Terrains;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using DWP;
using UnityEngine;
using VehicleCommon;

namespace App.Shared.GameModules.Vehicle.Ship
{
    public class ShipEntityUtility : IVehicleEntityUtility
    {
        private static AdvancedShipController GetController(VehicleEntity vehicle)
        {
            return vehicle.GetController<AdvancedShipController>();
        }

        public void AddDynamicData(VehicleEntity vehicle, Vector3 position, Quaternion rotation)
        {
            if (!vehicle.hasShipDynamicData)
            {
                vehicle.AddShipDynamicData(SharedConfig.ServerAuthorative, position, rotation);
            }
        }

 
        public void AddDynamicAndEffectDataPostInit(VehicleEntity vehicle)
        {
            AddDynamicData(vehicle, Vector3.zero, Quaternion.identity);
            AddRudders(vehicle);
        }

        private void AddRudders(VehicleEntity vehicle)
        {
            bool serverAuthorative = SharedConfig.ServerAuthorative;
            var controller = GetController(vehicle);
            int count = controller.rudders.Count;
            if (count >= 1)
            {
                if (!vehicle.hasShipFirstRudderDynamicData)
                {
                    vehicle.AddShipFirstRudderDynamicData(serverAuthorative);
                }
            }

            if (count >= 2)
            {
                if (!vehicle.hasShipSecondRudderDynamicData)
                {
                    vehicle.AddShipSecondRudderDynamicData(serverAuthorative);
                } 
            }
        }

        public void AddHitBox(VehicleEntity vehicle)
        {
            if (!vehicle.hasShipHitBox)
            {
                vehicle.AddShipHitBox(new Vector3[(int)VehiclePartIndex.MaxRudderCount], new Quaternion[(int) VehiclePartIndex.MaxRudderCount]);
            }
        }

        public void AddVehicleAllGameData(VehicleEntity vehicle)
        {
            if (!vehicle.hasShipGameData)
            {
                vehicle.AddShipGameData();
            }

            if (!vehicle.hasVehicleBrokenFlag)
            {
                vehicle.AddVehicleBrokenFlag();
            }

            ConfigVehicleAllGameData(vehicle);
        }

        public void InitController(VehicleEntity vehicle, bool isServer, int vehicleId)
        {
            var controller = GetController(vehicle);
            VehicleCommonUtility.InitController(controller, isServer, vehicleId);
        }

        private void ConfigVehicleAllGameData(VehicleEntity vehicle)
        {
            var config = GetShipConfig(vehicle);
            var gameData = vehicle.shipGameData;

            if(SharedConfig.IsOffline || SharedConfig.IsServer)
                VehicleCommonUtility.SetSoundGameData(gameData);

            gameData.MaxFuel = config.maxFuelVolume;
            gameData.FuelCost = config.fuelCost / 60.0f;
            gameData.FuleCostOnAcceleration = gameData.FuelCost * (1 + config.accelerateFuelCostRatio);
            gameData.RemainingFuel = config.GetRandomFuelVolume();

            gameData.MaxHp = config.maxHp;
            gameData.Hp = gameData.MaxHp;

            gameData.IsAccelerated = false;

            vehicle.vehicleBrokenFlag.Init();
        }

        public void ConfigHitBoxImposter(VehicleEntity vehicle, GameObject hitboxImposter)
        {
            var config = GetShipConfig(vehicle);

            config.hitBoxRootImposter = VehicleCommonUtility.GetChildByName(hitboxImposter.transform, config.hitBoxRootName);
            if (config.hitBoxRootImposter == null)
            {
                config.hitBoxRootImposter = hitboxImposter.transform;
            }

            var rudderCount = config.rudderHitBoxes.Length;
            config.rudderHitBoxImposters = new Transform[rudderCount];
            for (int i = 0; i < rudderCount; ++i)
            {
                config.rudderHitBoxImposters[i] = VehicleCommonUtility.GetChildByName(hitboxImposter.transform, config.rudderHitBoxNames[i]);
                AssertUtility.Assert(config.rudderHitBoxImposters[i] != null);
            }

        }


        public bool HasDynamicData(VehicleEntity vehicle)
        {
            return vehicle.hasShipDynamicData;
        }

        public VehicleDynamicDataComponent GetDynamicData(VehicleEntity vehicle)
        {
            return vehicle.shipDynamicData;
        }

        public float GetThrottleInput(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller.ThrottleInput;
        }

        public bool IsAccelerated(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller.IsAccelerated;
        }

        public void EnableInput(VehicleEntity vehicle, bool enabled)
        {
            var controller = GetController(vehicle);
            controller.EnableInput = enabled;
        }

        public void SetHandBrakeOn(VehicleEntity vehicle, bool isOn)
        {
            //the ship does not have brake!!!!
        }

        public bool IsCrashed(VehicleEntity vehicle)
        {
            return false;
        }

        public bool IsInWater(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller.IsInWater();
        }

        public bool IsFocusable(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return !controller.IsBroken;
        }

        public bool IsRidable(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return !controller.IsBroken;
        }


        public void ResetPose(VehicleEntity vehicle, bool syncRemote)
        {
            //don't need to reset pose
        }

        public Vector3 GetPrevLinearVelocity(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller.PreviousVelocity;
        }

        public float GetUiPresentSpeed(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            
            return controller.Speed * 3.6f;
        }

        public bool HasGameData(VehicleEntity vehicle)
        {
            return vehicle.hasShipGameData;
        }


        public VehicleBaseGameDataComponent GetGameData(VehicleEntity vehicle)
        {
            return vehicle.shipGameData;
        }

        public bool HasHitBoxBuffer(VehicleEntity vehicle)
        {
            return vehicle.hasShipHitBox;
        }

        public void SetControllerBroken(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            controller.IsBroken = true;
        }

        public float GetHitFactor(VehicleEntity vehicle, Collider collider, out VehiclePartIndex partIndex)
        {
            var config = GetShipConfig(vehicle);
            partIndex = GetPartIndexByName(vehicle, collider.name);
            return config.bodyHitBoxFator;
        }

        public void UpdateHitBoxes(VehicleEntity vehicle, IGameEntity gameEntity)
        {
            var config = GetShipConfig(vehicle);

            var hitBoxComp = gameEntity.GetComponent<ShipHitBoxComponent>();
            config.hitBoxRootImposter.transform.SetPositionAndRotation(hitBoxComp.BodyPosition, hitBoxComp.BodyRotation);
            var wheelCount = config.rudderHitBoxImposters.Length;
            for (int i = 0; i < wheelCount; ++i)
            {
                config.rudderHitBoxImposters[i].SetPositionAndRotation(hitBoxComp.RudderPositionList[i], hitBoxComp.RudderRotationList[i]);
            }
        }

        public void GetCollisionObjectFactor(VehicleEntity vehicle, Collider collider, out float factorA, out float factorB, out float boxFactor)
        {
            GetShipConfig(vehicle).GetCollisionObjectFactor(collider, out factorA, out factorB, out boxFactor);
        }

        public void GetCollisionPlayerFactor(VehicleEntity vehicle, out float factor)
        {
            GetShipConfig(vehicle).GetCollisionPlayerFactor(out factor);
        }

        public void GetExplosionToPlayerFactor(VehicleEntity vehicle, out float factorA, out float factorB)
        {
            GetShipConfig(vehicle).GetExplosionToPlayerFactor(out factorA, out factorB);
        }

        public void GetExplosionToVehicleFactor(VehicleEntity vehicle, out float factorA, out float factorB)
        {
            GetShipConfig(vehicle).GetExplosionToVehicleFactor(out factorA, out factorB);
        }

        public Vector3 GetExplosionCenter(VehicleEntity vehicle)
        {
            return GetController(vehicle).transform.position;
        }

        public static VehiclePartIndex GetPartIndexByName(VehicleEntity vehicle, string goName)
        {
            var config = GetShipConfig(vehicle);
            var indexArray = VehicleIndexHelper.GetRudderIndexArray();
            for (int i = 0; i < config.rudderHitBoxNames.Length; ++i)
            {
                if (config.rudderHitBoxNames[i].Equals(goName))
                {
                    return indexArray[config.rudderHitBoxIndices[i]];
                }
            }

            return VehiclePartIndex.Body;
        }

        public void ClearInput(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            controller.SetInput(0.0f, 0.0f, false);
        }

        public void AddImpulseAtPosition(VehicleEntity vehicle, Vector3 impulse, Vector3 position)
        {
            var controller = GetController(vehicle);
            controller.AddImpulseAtPosition(impulse, position, true);
        }

        public void BuildVehicleGUIInfo(VehicleEntity vehicle, int index, StringBuilder infoBuilder)
        {
            var controller = GetController(vehicle);
            var linearVelocity = controller.Velocity;
            infoBuilder.Length = 0;
            infoBuilder.Append("id:");
            infoBuilder.Append(index);
            infoBuilder.Append("/Hp:");
            infoBuilder.Append(vehicle.shipGameData.Hp);
            infoBuilder.Append("/F:");
            infoBuilder.Append(vehicle.shipGameData.RemainingFuel);
            infoBuilder.Append("/V:");
            infoBuilder.Append(linearVelocity.magnitude.ToString("f1"));
            infoBuilder.Append("/");
            infoBuilder.Append(vehicle.position.Value.ToString("f1"));
            infoBuilder.Append("/M:");
            infoBuilder.Append((linearVelocity.magnitude * controller.cachedRigidbody.mass).ToString("f1"));
        }

        public void SetDebugInput(VehicleEntity vehicle, VechileDebugInput inputType, float value)
        {
            var controller = GetController(vehicle);
            switch (inputType)
            {
                case VechileDebugInput.Throttle:
                    controller.input.throttle = value;
                    break;
                case VechileDebugInput.Steer:
                    controller.input.rudder = value;
                    break;
            }

            if (!controller.input.throttle.Equals(0))
            {
                controller.DisableSleeping = false;
            }
            else
            {
                controller.DisableSleeping = true;
            }
        }

        public static ShipConfig GetShipConfig(VehicleEntity vehicle)
        {
            var go = vehicle.gameObject.UnityObject.AsGameObject;
            return go.GetComponent<ShipConfig>();
        }

        public static bool HasRudder(VehicleEntity vehicle, VehiclePartIndex index)
        {
            switch (index)
            {
                case VehiclePartIndex.FirstRudder:
                    return vehicle.hasShipFirstRudderDynamicData;
                case VehiclePartIndex.SecondRudder:
                    return vehicle.hasShipSecondRudderDynamicData;
                default:
                    return false;
            }

        }

        public static ShipRudderDynamicData GetRudder(VehicleEntity vehicle, VehiclePartIndex index)
        {
            switch (index)
            {
                case VehiclePartIndex.FirstRudder:
                    return vehicle.shipFirstRudderDynamicData;
                case VehiclePartIndex.SecondRudder:
                    return vehicle.shipSecondRudderDynamicData;
                default:
                    return null;
            }
        }

    }
}
