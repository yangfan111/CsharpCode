using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using Core.Compare;
using Core.Interpolate;
using Core.Playback;
using Core.Prediction;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Vehicle
{

    [Vehicle]
    [Serializable]
    public class VehicleCmdComponent : CmdListComponent<IVehicleCmd>, IComponent
    {

    }

    public enum VehicleFlag
    {
        None = 0x0,
        RemoteSet = 0x1,
        LocalSet = 0x2,
    }

    public class VehicleFlagUtility
    {
        public static bool HasFlag(int objFlag, VehicleFlag flag)
        {
            return (objFlag & (int)flag) != 0;
        }

        // ReSharper disable once UnusedParameter.Local
        public static int SetFlag(int objFlag, VehicleFlag flag)
        {
            return (int)flag;
        }

        public static int RemoveFlag(int objFlag, VehicleFlag flag)
        {
            objFlag &= ~((int)flag);

            return objFlag;
        }
    }


    public abstract class VehicleDynamicDataComponent : IRewindableComponent, IInterpolatableComponent, IComparableComponent, IVehicleLatestComponent, IVehicleResetableComponent
    {
        public bool ServerAuthoritive;

        [DontInitilize]
        public bool IsSyncLatest { get; set; }

        [DontInitilize]
        public int Flag;

        [NetworkProperty]
        [DontInitilize]
        public bool IsAccelerated;

        [NetworkProperty]
        [DontInitilize]
        public float SteerInput;

        [NetworkProperty]
        [DontInitilize]
        public float ThrottleInput;

        [NetworkProperty]
        public Vector3 Position;

        [NetworkProperty]
        public Quaternion Rotation = Quaternion.identity;

        [NetworkProperty]
        [DontInitilize]
        public Vector3 LinearVelocity;

        [NetworkProperty]
        [DontInitilize]
        public Vector3 AngularVelocity;

        [NetworkProperty]
        [DontInitilize]
        public bool IsSleeping;

        public bool IsSet()
        {
            return Flag != (int)VehicleFlag.None;
        }

        public bool IsRemoteSet()
        {
            return HasFlag(VehicleFlag.RemoteSet);
        }

        public bool IsLocalSet()
        {
            return HasFlag(VehicleFlag.LocalSet);
        }
		
		public void Clear()
        {
            Flag = (int)VehicleFlag.None;
        }

        public bool HasFlag(VehicleFlag flag)
        {
            return VehicleFlagUtility.HasFlag(Flag, flag);
        }

        public void SetFlag(VehicleFlag flag)
        {
            Flag = VehicleFlagUtility.SetFlag(Flag, flag);
        }


        public virtual void CopyFrom(object rightComponent)
        {
            var data = rightComponent as VehicleDynamicDataComponent;

            Flag = (int)VehicleFlag.RemoteSet;

            IsAccelerated = data.IsAccelerated;
            SteerInput = data.SteerInput;
            ThrottleInput = data.ThrottleInput;

            Position = data.Position;
            Rotation = data.Rotation;
            LinearVelocity = data.LinearVelocity;
            AngularVelocity = data.AngularVelocity;

            IsSleeping = data.IsSleeping;
        }

        public abstract void RewindTo(object rightComponent);
        public abstract void SyncLatestFrom(object rightComponent);
        
        public bool IsInterpolateEveryFrame(){ return true; }
        public virtual void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var l = (VehicleDynamicDataComponent)left;
            var r = (VehicleDynamicDataComponent)right;

            if (IsSyncLatest)
            {
                return;
            }

            Flag = (int) VehicleFlag.RemoteSet;
            var rotio = interpolationInfo.Ratio;
            IsAccelerated = l.IsAccelerated && r.IsAccelerated;
            SteerInput = InterpolateUtility.Interpolate(l.SteerInput, r.SteerInput, rotio);
            ThrottleInput = InterpolateUtility.Interpolate(l.ThrottleInput, r.ThrottleInput, rotio);

            Position = InterpolateUtility.Interpolate(l.Position, r.Position, rotio);
            Rotation = InterpolateUtility.Interpolate(l.Rotation, r.Rotation, rotio);

            LinearVelocity = InterpolateUtility.Interpolate(l.LinearVelocity, r.LinearVelocity, rotio);
            AngularVelocity = InterpolateUtility.Interpolate(l.AngularVelocity, r.AngularVelocity, rotio);

            IsSleeping = l.IsSleeping;
        }

        public virtual bool IsApproximatelyEqual(object right)
        {
            var r = right as VehicleDynamicDataComponent;
            if (r.ServerAuthoritive)
            {
                const float delta = 0.000001f;
                return CompareUtility.IsApproximatelyEqual(Position, r.Position, delta) &&
                       CompareUtility.IsApproximatelyEqual(Rotation, r.Rotation, delta);
            }

            return true;
        }


        public virtual void Reset()
        {
            IsSyncLatest = false;
            Flag = 0;
            IsAccelerated = false;
            SteerInput = 0;
            ThrottleInput = 0;
            IsSleeping = false;
        }

        public abstract int GetComponentId();
       
    }
}
