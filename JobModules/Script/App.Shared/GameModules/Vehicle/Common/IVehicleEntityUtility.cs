using System;
using System.Text;
using App.Shared.Components.Vehicle;
using Core.EntityComponent;
using Core.Prediction.VehiclePrediction.Cmd;
using UnityEngine;
using VehicleCommon;

namespace App.Shared.GameModules.Vehicle
{
    public class VehicleSeatConfigInfo
    {
        public Transform DriverSeat;
        public Transform CodriverSeat;
        public Transform BackDriverSeat;
        public Transform BackCodriverSeat;
        public Transform BackDriverSeat_1;
        public Transform BackCodriverSeat_1;
    }

    public enum VechileDebugInput
    {
        Throttle,
        Steer,
        Brake,
        HandBrake,
    }


    public  interface IVehicleEntityUtility
    {
        void AddDynamicData(VehicleEntity vehicle, Vector3 position, Quaternion rotation);
        void AddDynamicAndEffectDataPostInit(VehicleEntity vehicle);
        void AddHitBox(VehicleEntity vehicle);
        void AddVehicleAllGameData(VehicleEntity vehicle);
        void InitController(VehicleEntity vehicle, bool isServer, int vehicleId);

        void ClearInput(VehicleEntity vehicle);
        void EnableInput(VehicleEntity vehicle, bool enabled);
        void SetHandBrakeOn(VehicleEntity vehicle, bool isOn);

        void ResetPose(VehicleEntity vehicle, bool syncRemote);
        bool IsCrashed(VehicleEntity vehicle);
        bool IsInWater(VehicleEntity vehicle);

        bool IsFocusable(VehicleEntity vehicle);
        bool IsRidable(VehicleEntity vehicle);

        Vector3 GetPrevLinearVelocity(VehicleEntity vehicle);
        float GetUiPresentSpeed(VehicleEntity vehicle);

        bool HasDynamicData(VehicleEntity vehicle);
        VehicleDynamicDataComponent GetDynamicData(VehicleEntity vehicle);

        float GetThrottleInput(VehicleEntity vehicle);
        bool IsAccelerated(VehicleEntity vehicle);

        bool HasGameData(VehicleEntity vehicle);
        VehicleBaseGameDataComponent GetGameData(VehicleEntity vehicle);

        bool HasHitBoxBuffer(VehicleEntity vehicle);

        void SetControllerBroken(VehicleEntity vehicle);

        void ConfigHitBoxImposter(VehicleEntity vehicle, GameObject hitboxImposter);
        float GetHitFactor(VehicleEntity vehicle, Collider collider, out VehiclePartIndex partIndex);
        void UpdateHitBoxes(VehicleEntity vehicle, IGameEntity gameEntity);

        void GetCollisionObjectFactor(VehicleEntity vehicle, Collider collider, out float factorA, out float factorB, out float boxFactor);

        void GetCollisionPlayerFactor(VehicleEntity vehicle, out float factor);

        void GetExplosionToPlayerFactor(VehicleEntity vehicle, out float factorA, out float factorB);

        void GetExplosionToVehicleFactor(VehicleEntity vehicle, out float factorA, out float factorB);

        Vector3 GetExplosionCenter(VehicleEntity vehicle);

        void AddImpulseAtPosition(VehicleEntity vehicle, Vector3 impulse, Vector3 position);

        void BuildVehicleGUIInfo(VehicleEntity vehicle, int index, StringBuilder infoBuilder);

        void SetDebugInput(VehicleEntity vehicle, VechileDebugInput inputType, float value);
    }
}
