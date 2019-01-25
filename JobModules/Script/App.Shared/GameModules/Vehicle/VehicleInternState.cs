using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Vehicle;
using UnityEngine;

namespace App.Shared.GameModules.Vehicle
{
    public class VehicleInternState
    {
        public int Flag;

        public bool IsAccelerated;
        public float SteerInput;
        public float ThrottleInput;

        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 LinearVelocity;
        public Vector3 AngularVelocity;
        public bool IsSleeping;
        public bool Crashed;

        public bool HasFlag(VehicleFlag flag)
        {
            return VehicleFlagUtility.HasFlag(Flag, flag);
        }

        public void SetFlag(VehicleFlag flag)
        {
            Flag = VehicleFlagUtility.SetFlag(Flag, flag);
        }

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

    }
}
