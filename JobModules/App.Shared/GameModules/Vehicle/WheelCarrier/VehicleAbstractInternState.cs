using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Vehicle;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using EVP.Scripts;
using UnityEngine;

namespace App.Shared.GameModules.Vehicle.WheelCarrier
{
    public abstract class VehicleAbstractStateComponentTransfer
    {

        public virtual void FromStateToComponent(VehicleAbstractState state, VehicleEntity vehicle)
        {
            var comp = vehicle.GetDynamicData();

            comp.Flag = (int)VehicleFlag.LocalSet;

            comp.IsAccelerated = state.IsAccelerated;
            comp.SteerInput = state.SteerInput;
            comp.ThrottleInput = state.ThrottleInput;
 
            comp.Position = state.BodyState.Position;
            comp.Rotation = state.BodyState.Rotation;
            comp.LinearVelocity = state.BodyState.LinearVelocity;
            comp.AngularVelocity = state.BodyState.AngularVelocity;
            comp.IsSleeping = state.BodyState.IsSleeping;

            var wheelCount = state.WheelStates.Length;
            var indexArray = VehicleIndexHelper.GetWheelIndexArray();
            AssertUtility.Assert(wheelCount <= indexArray.Length);
            for (int i = 0; i < wheelCount; ++i)
            {
                var index = indexArray[i];
                if (WheelEntityUtility.HasWheel(vehicle, index))
                {
                    SetWheel(vehicle, index, state.WheelStates[i]);
                }

            }
        }

        protected virtual void SetWheel(VehicleEntity vehicle, VehiclePartIndex index, WheelAbstractState state)
        {
            var comp = WheelEntityUtility.GetWheel(vehicle, index);

            comp.ColliderSteerAngle = state.ColliderSteerAngle;

        }
    }

    public abstract class WheelAbstractInternInfo
    {
        public float ColliderSteerAngle;
        public virtual float WheelSteerAngle { get { return ColliderSteerAngle; } }
    }

    public abstract class VehicleAbstractInternState : VehicleInternState
    {
        public bool IsHornOn;
        public float HandbrakeInput;

        public WheelAbstractInternInfo[] Wheels;

        public virtual void SetFrom(VehicleEntity vehicle)
        {
            var comp = (CarDynamicDataComponent) vehicle.GetDynamicData();
            Flag = comp.Flag;

            IsHornOn = comp.IsHornOn;
            HandbrakeInput = comp.HandbrakeInput;
            IsAccelerated = comp.IsAccelerated;
            SteerInput = comp.SteerInput;
            ThrottleInput = comp.ThrottleInput;

            Position = comp.Position;
            Rotation = comp.Rotation;
            LinearVelocity = comp.LinearVelocity;
            AngularVelocity = comp.AngularVelocity;
            IsSleeping = comp.IsSleeping;
                  
            var indexArray = VehicleIndexHelper.GetWheelIndexArray();
            for (int i = 0; i < indexArray.Length; ++i)
            {
                var index = indexArray[i];
                if (WheelEntityUtility.HasWheel(vehicle, index))
                {
                    GetWheelState(WheelEntityUtility.GetWheel(vehicle, index), Wheels[i]);
                }
            }
        }

        protected virtual void GetWheelState(CarWheelComponent comp, WheelAbstractInternInfo info)
        {
            info.ColliderSteerAngle = comp.ColliderSteerAngle;
        }

        public virtual void SetTo(VehicleAbstractState state)
        {
            state.IsSyncFlagSet = IsRemoteSet();
            state.IsHornOn = IsHornOn;
            state.HandbrakeInput = HandbrakeInput;
            state.IsAccelerated = IsAccelerated;
            state.SteerInput = SteerInput;
            state.ThrottleInput = ThrottleInput;

            state.BodyState.Position = Position;
            state.BodyState.Rotation = Rotation;
            state.BodyState.LinearVelocity = LinearVelocity;
            state.BodyState.AngularVelocity = AngularVelocity;
            state.BodyState.IsSleeping = IsSleeping;

            var wheelStates = state.WheelStates;
            var wheelCount = wheelStates.Length;
            for (int i = 0; i < wheelCount; ++i)
            {
                var destState = wheelStates[i];
                var sourceState = Wheels[i];

                destState.ColliderSteerAngle = sourceState.ColliderSteerAngle;
            }
        }
    }
}
