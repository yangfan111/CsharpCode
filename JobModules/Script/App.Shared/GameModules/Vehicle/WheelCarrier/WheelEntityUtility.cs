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
using EVP;
using EVP.Scripts;
using UnityEngine;
using VehicleCommon;

namespace App.Shared.GameModules.Vehicle.WheelCarrier
{
    public class WheelEntityUtility : IVehicleEntityUtility
    {

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(WheelEntityUtility));

        private static VehicleAbstractController GetController(VehicleEntity vehicle)
        {
            return vehicle.GetController<VehicleAbstractController>();
        }

        public static VehicleUiWheelIndex GetUiWheelIndex(VehicleEntity vehicle, VehiclePartIndex index)
        {
            if (HasWheel(vehicle, index))
            {
                var controller = GetController(vehicle);
                var controllerIndex = VehicleIndexHelper.ToVehicleControllerWheelIndex(index);
                return controller.GetWheelUiIndex(controllerIndex);
            }

            return VehicleUiWheelIndex.None;
        }


        public void AddDynamicData(VehicleEntity vehicle, Vector3 position, Quaternion rotation)
        {
            if (!vehicle.hasCarRewindData)
            {
                vehicle.AddCarRewindData(false, SharedConfig.ServerAuthorative, position, rotation);
            }
        }

        public void AddDynamicAndEffectDataPostInit(VehicleEntity vehicle)
        {
            AddDynamicData(vehicle, Vector3.zero, Quaternion.identity);
            AddVehicleWheels(vehicle);
        }

        private void AddVehicleWheels(VehicleEntity vehicle)
        {
            int wheelCount = GetWheelCount(vehicle);
            bool serverAuthorative = SharedConfig.ServerAuthorative;
            if (wheelCount >= 1)
            {
                if (!vehicle.hasCarFirstRewnWheel)
                {
                    vehicle.AddCarFirstRewnWheel(serverAuthorative);
                }
            }

            if (wheelCount >= 2)
            {
                if (!vehicle.hasCarSecondRewnWheel)
                {
                    vehicle.AddCarSecondRewnWheel(serverAuthorative);
                }

            }

            if (wheelCount >= 3)
            {
                if (!vehicle.hasCarThirdRewnWheel)
                {
                    vehicle.AddCarThirdRewnWheel(serverAuthorative);
                }
            }

            if (wheelCount >= 4)
            {
                if (!vehicle.hasCarFourthRewnWheel)
                {
                    vehicle.AddCarFourthRewnWheel(serverAuthorative);
                }
            }
        }

        public void InitController(VehicleEntity vehicle, bool isServer, int vehicleId)
        {
            var controller = GetController(vehicle);
            VehicleCommonUtility.InitController(controller, isServer, vehicleId);
        }

        private int GetWheelCount(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller.WheelCount;
        }

        public void AddHitBox(VehicleEntity vehicle)
        {
            if (!vehicle.hasCarHitBox)
            {
                vehicle.AddCarHitBox(new Vector3[(int)VehiclePartIndex.MaxFlexibleCount], new Quaternion[(int)VehiclePartIndex.MaxFlexibleCount],
                    new Vector3[(int)VehiclePartIndex.MaxWheelCount], new Quaternion[(int)VehiclePartIndex.MaxWheelCount]);
            }
        }

        public void AddVehicleAllGameData(VehicleEntity vehicle)
        {
            if (!vehicle.hasCarGameData)
            {
                vehicle.AddCarGameData();
            }

            if (!vehicle.hasVehicleBrokenFlag)
            {
                vehicle.AddVehicleBrokenFlag(); 
            }
            
            ConfigVehicleAllGameData(vehicle);
        }

        private void ConfigVehicleAllGameData(VehicleEntity vehicle)
        {
            var config = GetCarConfig(vehicle);
            var gameData = vehicle.carGameData;

            if (SharedConfig.IsOffline || SharedConfig.IsServer)
                VehicleCommonUtility.SetSoundGameData(gameData);

            gameData.MaxFuel = config.maxFuelVolume;
            gameData.FuelCost = config.fuelCost / 60.0f;
            gameData.FuleCostOnAcceleration = gameData.FuelCost * (1 + config.accelerateFuelCostRatio);
            gameData.RemainingFuel = config.GetRandomFuelVolume();

            gameData.MaxHp = config.maxHp;
            gameData.Hp = gameData.MaxHp;

            gameData.IsAccelerated = false;

            var controller = GetController(vehicle);
            gameData.WheelCount = controller.WheelCount;
            gameData.MaxWheelHp = config.maxWheelHp;
            gameData.FirstWheelHp = gameData.MaxWheelHp;
            gameData.SecondWheelHp = gameData.MaxWheelHp;
            gameData.ThirdWheelHp = gameData.MaxWheelHp;
            gameData.FourthWheelHp = gameData.MaxWheelHp;

            vehicle.vehicleBrokenFlag.Init();
        }

        public void ClearInput(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            controller.SetInput(0.0f, 0.0f, 0.0f, false, false, false);
        }

        public void EnableInput(VehicleEntity vehicle, bool enabled)
        {
            var controller = GetController(vehicle);
            controller.EnableInput = enabled;
        }

        public void SetHandBrakeOn(VehicleEntity vehicle, bool isOn)
        {
            var controller = GetController(vehicle);
            controller.IsHandBrakeOn = isOn;
        }

        public void ResetPose(VehicleEntity vehicle, bool syncRemote)
        {
            var controller = GetController(vehicle);
            controller.ResetPose(syncRemote);
        }

        public bool IsCrashed(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller.IsCrashed;
        }

        public bool IsInWater(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller.IsInWater();
        }

        public bool IsFocusable(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller.IsFocusable;
        }

        public bool IsRidable(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller.IsRidable;
        }

        public Vector3 GetPrevLinearVelocity(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller.PreviousVelocity;
        }

        public float GetUiPresentSpeed(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller.Velocity.magnitude * 3.6f;
        }

        public bool HasDynamicData(VehicleEntity vehicle)
        {
            return vehicle.hasCarRewindData;
        }

        public VehicleDynamicDataComponent GetDynamicData(VehicleEntity vehicle)
        {
            return vehicle.carRewindData;
        }

        public float GetThrottleInput(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller.throttleInput;
        }

        public bool IsAccelerated(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller.isAccelerated;
        }

        public bool HasGameData(VehicleEntity vehicle)
        {
            return vehicle.hasCarGameData;
        }

        public bool HasHitBoxBuffer(VehicleEntity vehicle)
        {
            return vehicle.hasCarHitBox;
        }

        public VehicleBaseGameDataComponent GetGameData(VehicleEntity vehicle)
        {
            return vehicle.carGameData;
        }

        public void SetControllerBroken(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            controller.IsBroken = true;
        }


        public void ConfigHitBoxImposter(VehicleEntity vehicle, GameObject hitboxImposter)
        {
            var config = GetCarConfig(vehicle);

            config.hitBoxRootImposter = VehicleCommonUtility.GetChildByName(hitboxImposter.transform, config.hitBoxRootName);
            if (config.hitBoxRootImposter == null)
            {
                config.hitBoxRootImposter = hitboxImposter.transform;
            }

            var flexibleHitBoxNames = config.flexibleHitBoxNames;
            if (flexibleHitBoxNames.Length > (int)VehiclePartIndex.MaxFlexibleCount)
            {
                _logger.ErrorFormat("The flexible hitbox in the car exceed maximum number {0}", VehiclePartIndex.MaxFlexibleCount);
            }
            else
            {
                config.flexibleHitBoxImposters = new Transform[flexibleHitBoxNames.Length];
                for (int i = 0; i < flexibleHitBoxNames.Length; ++i)
                {
                    config.flexibleHitBoxImposters[i] =
                        VehicleCommonUtility.GetChildByName(hitboxImposter.transform, flexibleHitBoxNames[i]);
                }
            }

            var wheelCount = config.wheelHitBoxNames.Length;
            config.wheelHitBoxImposters = new WheelHitBox[wheelCount];
            for (int i = 0; i < wheelCount; ++i)
            {
                config.wheelHitBoxImposters[i] = new WheelHitBox();
                config.wheelHitBoxImposters[i].OuterHitBox = VehicleCommonUtility.GetChildByName(hitboxImposter.transform, config.wheelHitBoxNames[i].OuterHitBoxName);
                config.wheelHitBoxImposters[i].InnerHitBox = VehicleCommonUtility.GetChildByName(hitboxImposter.transform, config.wheelHitBoxNames[i].InnerHitBoxName);
                AssertUtility.Assert(config.wheelHitBoxImposters[i].OuterHitBox != null, "node " + i + ", " + config.wheelHitBoxNames[i]);
            }
        }

        public float GetHitFactor(VehicleEntity vehicle, Collider collider, out VehiclePartIndex partIndex)
        {
            var config = GetCarConfig(vehicle);
            partIndex = GetPartIndexByName(vehicle, collider.name);
            return partIndex == VehiclePartIndex.Body
                ? config.bodyHitBoxFator
                : config.wheelHitBoxFactor;
        }

        public void UpdateHitBoxes(VehicleEntity vehicle, IGameEntity gameEntity)
        {
            var config = GetCarConfig(vehicle);

            var hitBoxComp = gameEntity.GetComponent<CarHitBoxComponent>();
            config.hitBoxRootImposter.transform.SetPositionAndRotation(hitBoxComp.BodyPosition, hitBoxComp.BodyRotation);

            var flexibleCount = config.flexibleHitBoxImposters.Length;
            for (int i = 0; i < flexibleCount; ++i)
            {
                var flexibleImposter = config.flexibleHitBoxImposters[i];
                if (flexibleImposter != null)
                {
                    flexibleImposter.SetPositionAndRotation(hitBoxComp.FlexiblePositionList[i], hitBoxComp.FlexibleRotationList[i]);
                }
            }

            var wheelCount = config.wheelHitBoxImposters.Length;
            var controller = GetController(vehicle);
            for (int i = 0; i < wheelCount; ++i)
            {
                var outerGo = config.wheelHitBoxImposters[i].OuterHitBox.gameObject;
                int index = config.wheelHitBoxIndices[i];
                bool isBroken = controller.GetWheelBroken(index);
                if (isBroken && outerGo.activeSelf)
                {
                    outerGo.SetActive(false);
                }else if (!outerGo.activeSelf && !isBroken)
                {
                    outerGo.SetActive(true);
                }

                var hitBoxRoot = config.wheelHitBoxImposters[i].GetHitBoxRoot();
                hitBoxRoot.SetPositionAndRotation(hitBoxComp.WheelPositionList[i], hitBoxComp.WheelRotationList[i]);
            }
        }

        public void GetCollisionObjectFactor(VehicleEntity vehicle, Collider collider, out float factorA, out float factorB, out float boxFactor)
        {
            GetCarConfig(vehicle).GetCollisionObjectFactor(collider, out factorA, out factorB, out boxFactor);
        }

        public void GetCollisionPlayerFactor(VehicleEntity vehicle,  out float factor)
        {
            GetCarConfig(vehicle).GetCollisionPlayerFactor(out factor);
        }


        public void GetExplosionToPlayerFactor(VehicleEntity vehicle, out float factorA, out float factorB)
        {
            GetCarConfig(vehicle).GetExplosionToPlayerFactor(out factorA, out factorB);
        }

        public void GetExplosionToVehicleFactor(VehicleEntity vehicle, out float factorA, out float factorB)
        {
            GetCarConfig(vehicle).GetExplosionToVehicleFactor(out factorA, out factorB);
        }

        public Vector3 GetExplosionCenter(VehicleEntity vehicle)
        {
            var config = GetCarConfig(vehicle);
            return config.effectExplosion == null
                ? GetController(vehicle).transform.position
                : config.effectExplosion.transform.position;
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
            infoBuilder.Append(vehicle.carGameData.Hp);
            infoBuilder.Append("/F:");
            infoBuilder.Append(vehicle.carGameData.RemainingFuel);
            infoBuilder.Append("/V:");
            infoBuilder.Append(linearVelocity.magnitude.ToString("f1"));
            infoBuilder.Append("/P:");
            infoBuilder.Append(vehicle.position.Value.ToString("f1"));
            infoBuilder.Append("/M:");
            infoBuilder.Append((linearVelocity.magnitude * controller.cachedRigidbody.mass).ToString("f1"));

            var indexArray = VehicleIndexHelper.GetWheelIndexArray();
            for (int i = 0; i < indexArray.Length; ++i)
            {
                infoBuilder.Append("/W");
                infoBuilder.Append(i + 1);
                infoBuilder.Append(":");
                infoBuilder.Append(vehicle.carGameData.GetWheelHp(indexArray[i]));
            }
        }

        public void SetDebugInput(VehicleEntity vehicle, VechileDebugInput inputType, float value)
        {
            var controller = GetController(vehicle);
            switch (inputType)
            {
                case VechileDebugInput.Throttle:
                    controller.throttleInput = value;
                    break;
                case VechileDebugInput.Steer:
                    controller.steerInput = value;
                    break;
                case VechileDebugInput.Brake:
                    controller.brakeInput = value;
                    break;
                case VechileDebugInput.HandBrake:
                    controller.handbrakeInput = value;
                    break;
            }

            if (!controller.throttleInput.Equals(0))
            {
                controller.DisableSleeping = false;
            }
            else
            {
                controller.DisableSleeping = true;
            }
         
        }

        public static VehicleConfig GetCarConfig(VehicleEntity vehicle)
        {
            var go = vehicle.gameObject.UnityObject.AsGameObject;
            return go.GetComponent<VehicleConfig>();
        }

        public static VehiclePartIndex GetPartIndexByName(VehicleEntity vehicle, string goName)
        {
            var config = GetCarConfig(vehicle);
            var indexArray = VehicleIndexHelper.GetWheelIndexArray();
            for (int i = 0; i < config.wheelHitBoxNames.Length; ++i)
            {
                var hitBoxNames = config.wheelHitBoxNames[i];
                if (goName.Equals(hitBoxNames.OuterHitBoxName) ||
                    goName.Equals(hitBoxNames.InnerHitBoxName))
                {
                    return indexArray[config.wheelHitBoxIndices[i]];
                }
            }

            return VehiclePartIndex.Body;
        }

        public static bool HasWheel(VehicleEntity vehicle, VehiclePartIndex index)
        {
            switch (index)
            {
                case VehiclePartIndex.FirstWheel:
                    return vehicle.hasCarFirstRewnWheel;
                case VehiclePartIndex.SecondWheel:
                    return vehicle.hasCarSecondRewnWheel;
                case VehiclePartIndex.ThirdWheel:
                    return vehicle.hasCarThirdRewnWheel;
                case VehiclePartIndex.FourthWheel:
                    return vehicle.hasCarFourthRewnWheel;
                default:
                    return false;
            }
        }

        public static CarWheelComponent GetWheel(VehicleEntity vehicle, VehiclePartIndex index)
        {
            switch (index)
            {
                case VehiclePartIndex.FirstWheel:
                {
                    return vehicle.carFirstRewnWheel;
                }

                case VehiclePartIndex.SecondWheel:
                {
                    return vehicle.carSecondRewnWheel;
                }

                case VehiclePartIndex.ThirdWheel:
                {
                    return vehicle.carThirdRewnWheel;
                }
                case VehiclePartIndex.FourthWheel:
                {

                    return vehicle.carFourthRewnWheel;
                }
                default:
                    return null;
            }
        }
    }
}
