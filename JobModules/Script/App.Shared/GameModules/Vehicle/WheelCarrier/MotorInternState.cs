using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Vehicle;
using Core.Prediction.VehiclePrediction.Cmd;
using EVP;
using EVP.Scripts;
using UnityEngine;

namespace App.Shared.GameModules.Vehicle.WheelCarrier
{

    public class MotorStateComponentTransfer : VehicleAbstractStateComponentTransfer
    {
        public override void FromStateToComponent(VehicleAbstractState state, VehicleEntity vehicle)
        {
            base.FromStateToComponent(state, vehicle);

            var comp = (CarDynamicDataComponent)vehicle.GetDynamicData();
            var motorState = (MotorState)state;

            comp.BrakeInput = motorState.StuntInput;
            comp.HandbrakeInput = motorState.HandbrakeInput;
            comp.SteerAngle = motorState.HorizontalShift;
            comp.Crashed = motorState.Crashed;
        }
    }

    public class MotorWheelInfo : WheelAbstractInternInfo
    { }


    public class MotorInternState : VehicleAbstractInternState
    {
        public float StuntInput;
        public float HorizontalShift;

        public MotorInternState()
        {
            Wheels = new WheelAbstractInternInfo[3];
            for (int i = 0; i < 3; ++i)
            {
                Wheels[i] = new MotorWheelInfo(); ;
            }
        }

        public override void SetFrom(VehicleEntity vehicle)
        {
            base.SetFrom(vehicle);

            var comp = (CarDynamicDataComponent)vehicle.GetDynamicData();
            StuntInput = comp.BrakeInput;
            HandbrakeInput = comp.HandbrakeInput;

            HorizontalShift = comp.SteerAngle;

            Crashed = comp.Crashed;

        }


        public override void SetTo(VehicleAbstractState toState)
        {
            base.SetTo(toState);
            var state = (MotorState)toState;

            state.StuntInput = StuntInput;
            state.HandbrakeInput = HandbrakeInput;
            state.HorizontalShift = HorizontalShift;
            state.Crashed = Crashed;
        }
    }
}
