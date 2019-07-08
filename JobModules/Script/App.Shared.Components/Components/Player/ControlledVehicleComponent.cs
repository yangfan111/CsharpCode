using Core.Components;
using Core.EntityComponent;
using Core.Playback;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
// ReSharper disable PossibleNullReferenceException
namespace App.Shared.Components.Player
{
    public enum EVehicleChannel
    {
        None = 0,
        C1 = 1,
        C2 = 2,
        C3 = 3,
        MaxCount
    }

    [Player]
    
    public class ControlledVehicleComponent : IPlaybackComponent, IUserPredictionComponent
    {
        public int GetComponentId() { { return (int)EComponentIds.PlayerControlledEntity; } }

        [DontInitilize]
        [NetworkProperty(16,0,1)]
        public int Role;

        [DontInitilize]
        [NetworkProperty]
        public EntityKey EntityKey;

        [DontInitilize]
        [NetworkProperty]
        public bool IsOnVehicle;

        [DontInitilize]
        [NetworkProperty]
        public byte ForceRideOffVehicleSignal;

        [DontInitilize]
        [NetworkProperty]
        public int ExecuteTime;

        [DontInitilize]
        public Vector3 CameraAnchorOffset;

        [DontInitilize]
        public float CameraDistance;

        [DontInitilize]
        public float CameraRotationDamping;

        [DontInitilize]
        public Vector3 CameraCurrentPosition;
        [DontInitilize]
        public Quaternion CameraCurrentRotation;
        [DontInitilize]
        public Vector3 CameraLastPosition;
        [DontInitilize]
        public float CameraLastAngle;
        [DontInitilize]
        public Vector3 CameraLocalTargetOffSet;

        [DontInitilize]
        public float LastVehicleControllTime = 0.0f;

        [DontInitilize]
        public int CurrentSoundId = 0;

        [DontInitilize]
        public bool IsLocalOnVehicle = false;

        [DontInitilize]
        public int RideOffSignal = 0;

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as ControlledVehicleComponent;
            IsOnVehicle = r.IsOnVehicle;
            Role = r.Role;
            EntityKey = r.EntityKey;
            ForceRideOffVehicleSignal = r.ForceRideOffVehicleSignal;
            ExecuteTime = r.ExecuteTime;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var r = right as ControlledVehicleComponent;
            return Role == r.Role && EntityKey == r.EntityKey;
        }
        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
        public bool IsInterpolateEveryFrame(){ return true; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var leftServerTime = interpolationInfo.LeftServerTime;
            var rightServerTime = interpolationInfo.RightServerTime;
            var serverTime = leftServerTime + (rightServerTime - leftServerTime) * interpolationInfo.Ratio;

            var l = (ControlledVehicleComponent) left;
            var r = (ControlledVehicleComponent) right;

            var comp = r.ExecuteTime > serverTime ? l : r;
            CopyFrom(comp);
        }

        public bool IsRideOffSignalOn()
        {
            return RideOffSignal != ForceRideOffVehicleSignal;
        }

        public void RideOn(int role, EntityKey entityKey, Rigidbody rigidBody, int executeTime)
        {
            Role = role;
            EntityKey = entityKey;
            ExecuteTime = executeTime;
            ResetRideOffSignal();
            SetOnVehicle(true);
            ResetCamearData(rigidBody);
        }

        public void RideOff(int executeTime)
        {
            Role = 0;
            EntityKey = EntityKey.Default;
            ExecuteTime = executeTime;
            SetOnVehicle(false);
            LastVehicleControllTime = Time.time;
            ForceRideOffVehicleSignal++;
        }

        private void SetOnVehicle(bool isOn)
        {
            IsLocalOnVehicle = isOn;
            IsOnVehicle = isOn;
        }

        private void ResetRideOffSignal()
        {
            RideOffSignal = ForceRideOffVehicleSignal;
        }

        public void ResetCamearData(Rigidbody body)
        {
            var target = body.transform;
            CameraLocalTargetOffSet = body.centerOfMass;
            CameraCurrentPosition = target.position + target.TransformDirection(CameraLocalTargetOffSet);

            var currentAngle = target.rotation.eulerAngles.y - 180.0f;
            CameraCurrentRotation = Quaternion.Euler(0.0f, currentAngle, 0.0f);

            var forward = target.forward;
            forward.y = 0;
            forward = forward.normalized;

            CameraLastPosition = -0.5f * forward;
            CameraLastAngle = currentAngle;
        }
    }

    

}
