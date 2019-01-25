using App.Shared.Components.Vehicle;
using Core.Prediction.VehiclePrediction.Cmd;
using EVP;
using EVP.Scripts;
using UnityEngine;

namespace App.Shared.GameModules.Vehicle.WheelCarrier
{

    public class CarStateComponentTransfer : VehicleAbstractStateComponentTransfer
    {
        public override void FromStateToComponent(VehicleAbstractState state, VehicleEntity vehicle)
        {
            base.FromStateToComponent(state, vehicle);

            var fromState = (VehicleState) state;
            var comp = (CarDynamicDataComponent) vehicle.GetDynamicData();
            comp.BrakeInput = fromState.BrakeInput;
            comp.HandbrakeInput = state.HandbrakeInput;

            comp.SteerAngle = fromState.SteerAngle;
        }

        protected override void SetWheel(VehicleEntity vehicle, VehiclePartIndex index, WheelAbstractState state)
        {
            base.SetWheel(vehicle, index, state);

            var fromState = (WheelState) state;
            var comp = WheelEntityUtility.GetWheel(vehicle, index);
            comp.SteerAngle = fromState.SteerAngle;
        }

  
    }

    public class CarWheelInfo : WheelAbstractInternInfo
    {
        public float SteerAngle;

        public override float WheelSteerAngle { get { return SteerAngle; } }
    }

    public class CarInternState : VehicleAbstractInternState
    {
        public float BrakeInput;

        public float SteerAngle;
 
        public CarInternState()
        {
            Wheels = new WheelAbstractInternInfo[(int) VehiclePartIndex.MaxWheelCount];
            for (int i = 0; i < (int) VehiclePartIndex.MaxWheelCount; ++i)
            {
                Wheels[i] = new CarWheelInfo();;
            }
        }

        public override void SetFrom(VehicleEntity vehicle)
        {
            base.SetFrom(vehicle);

            var comp = (CarDynamicDataComponent) vehicle.GetDynamicData();
            BrakeInput = comp.BrakeInput;
            SteerAngle = comp.SteerAngle;
        }

        protected override void GetWheelState(CarWheelComponent comp, WheelAbstractInternInfo info)
        {
            base.GetWheelState(comp, info);
            var carWheelInfo = (CarWheelInfo) info;
            carWheelInfo.SteerAngle = comp.SteerAngle;
        }

        public override void SetTo(VehicleAbstractState toState)
        {
            base.SetTo(toState);
            var state = (VehicleState) toState;
            state.BrakeInput = BrakeInput;
            state.HandbrakeInput = HandbrakeInput;

            state.SteerAngle = SteerAngle;

            var wheelStates = state.WheelStates;
            var wheelCount = wheelStates.Length;
            for (int i = 0; i < wheelCount; ++i)
            {
                var destState = (WheelState) wheelStates[i];
                var sourceState = (CarWheelInfo) Wheels[i];

                destState.SteerAngle = sourceState.SteerAngle;
            }
        }
    }
}
