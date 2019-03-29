using System;
using System.Resources;
using App.Shared;
using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Vehicle;
using App.Shared.GameModules.Vehicle.Common;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using EVP;
using EVP.Scripts;
using UnityEngine;
using XmlConfig;

namespace App.Shared.GameModules.Vehicle.WheelCarrier
{
    public class WheelEntityStateUtility : IVehicleStateUtility
    {

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(WheelEntityStateUtility));

        private VehicleAbstractInternState[] _tempStates = {
            new CarInternState(), 
            new MotorInternState(), 
        };

        private VehicleAbstractStateComponentTransfer[] _stateTransfers =
        {
            new CarStateComponentTransfer(),
            new MotorStateComponentTransfer()
        };

        private static VehicleAbstractController GetController(VehicleEntity vehicle)
        {
            return vehicle.GetController<VehicleAbstractController>();
        }

        public bool IsReadyForSync(VehicleEntity vehicle)
        {
            return vehicle.hasCarRewindData && vehicle.hasCarFirstRewnWheel
                && vehicle.IsActiveSelf();
        }

        public void SyncOnPlayerRideOn(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            if (controller.IsReseting)
            {
                controller.IsReseting = false;
                controller.IsCrashed = false;
            }
        }

        public void SyncFromComponent(VehicleEntity vehicle)
        {
            var controllerType = GetController(vehicle).ControllerType;
            var tempState = _tempStates[(int) controllerType];
            GetStateFromDynamicData(vehicle, tempState);
            SyncFromComponentToController(vehicle, tempState);
        }

        public void FixedUpdate(VehicleEntity vehicle)
        {
            if (vehicle.hasGameObject)
            {
                var controller = GetController(vehicle);
                controller.DoFixedUpdate();
            }
        }

        public void Update(VehicleEntity vehicle)
        {
            if (vehicle.hasGameObject)
            {
                var controller = GetController(vehicle);
                controller.DoUpdate();
            }
        }

        public void SyncToComponent(VehicleEntity vehicle)
        {
            var state = GetCurrentState(vehicle);
            var controllerType = GetController(vehicle).ControllerType;
            _stateTransfers[(int)controllerType].FromStateToComponent(state, vehicle);
        }


        public void SetVehicleStateToCmd(VehicleEntity vehicle, IVehicleCmd cmd)
        {
            var vehicleCmd = (VehicleCmd)cmd;
            var controller = GetController(vehicle);

            vehicleCmd.VehicleType = (int)EVehicleType.Car;

            vehicleCmd.Body.Position = controller.cachedRigidbody.position;
            vehicleCmd.Body.Rotation = controller.cachedRigidbody.rotation;
            vehicleCmd.Body.LinearVelocity = controller.Velocity;
            vehicleCmd.Body.AngularVelocity = controller.AngularVelocity;
 //           vehicleCmd.Body.ContactCount = controller.ContactCount;
            vehicleCmd.Body.Crashed = controller.IsCrashed;
            vehicleCmd.Body.IsSleeping = controller.IsSleeping;
            
            var wheelCount = controller.WheelCount;
            vehicleCmd.SteerableCount = wheelCount;
            for (int i = 0; i < wheelCount; ++i)
            {
                var wheel = controller.GetWheel(i);
//                var wd = controller.GetWheelData(i);
                vehicleCmd.Steerables[i].Car.Position = wheel.wheelTransform.localPosition;
                vehicleCmd.Steerables[i].Car.Rotation = wheel.wheelTransform.localRotation;
                vehicleCmd.Steerables[i].Car.Angle = wheel.wheelCollider.steerAngle;
//                vehicleCmd.Steerables[i].Car.Grounded = wd.Grounded;
//                vehicleCmd.Steerables[i].Car.GroundedOnTerrain = wd.GroundedOnTerrain;
            }


            //            var state = controller.GetCurrentState();        
            //            vehicleCmd.Body.Position = state.BodyState.Position;
            //            vehicleCmd.Body.Rotation = state.BodyState.Rotation;
            //            vehicleCmd.Body.Body2WorldPosition = state.BodyState.Body2WorldPosition;
            //            vehicleCmd.Body.Body2WorldRotation = state.BodyState.Body2WorldRotation;
            //            vehicleCmd.Body.LinearVelocity = state.BodyState.LinearVelocity;
            //            vehicleCmd.Body.AngularVelocity = state.BodyState.AngularVelocity;
            //
            //            var wheels = state.WheelStates;
            //            vehicleCmd.SteerableCount = wheels.Length;
            //            for (int i = 0; i < wheels.Length; ++i)
            //            {
            //                var wheel = wheels[i];
            //                var carSteerable = vehicleCmd.Steerables[i].Car;
            //
            //                carSteerable.ColliderSteerAngle = wheel.ColliderSteerAngle;
            //                carSteerable.SteerAngle = wheel.SteerAngle;
            //                carSteerable.MotorTorque = wheel.MotorTorque;
            //
            //                carSteerable.SuspensionDistance = wheel.SuspensionDistance;
            //                carSteerable.SuspensionSpring = wheel.SuspensionSpring;
            //                carSteerable.SuspensionTargetPosition = wheel.SuspensionTargetPosition;
            //
            //                carSteerable.SprungMass = wheel.SprungMass;
            //                carSteerable.ForceDistance = wheel.ForceDistance;
            //
            //                carSteerable.AngularVelocity = wheel.AngularVelocity;
            //                carSteerable.AngularPosition = wheel.AngularPosition;
            //                carSteerable.RotationAngle = wheel.RotationAngle;
            //
            //                carSteerable.RotationSpeed = wheel.RotationSpeed;
            //
            //                carSteerable.CorrectedRotationSpeed = wheel.CorrectedRotationSpeed;
            //                carSteerable.Jounce = wheel.Jounce;
            //
            //                carSteerable.TireLowSideSpeedTimers = wheel.TireLowSideSpeedTimers;
            //                carSteerable.TireLowforwardSpeedTimers = wheel.TireLowforwardSpeedTimers;
            //                carSteerable.Grounded = wheel.HitInfo.Grounded;
            //
            //                carSteerable.Point = wheel.HitInfo.Point;
            //
            //                carSteerable.Normal = wheel.HitInfo.Normal;
            //                carSteerable.ForwardDir = wheel.HitInfo.ForwardDir;
            //
            //                carSteerable.SidewaysDir = wheel.HitInfo.SidewaysDir;
            //                carSteerable.Force = wheel.HitInfo.Force;
            //
            //                carSteerable.ForwardSlip = wheel.HitInfo.ForwardSlip;
            //                carSteerable.SidewaysSlip = wheel.HitInfo.SidewaysSlip;
            //
            //                vehicleCmd.Steerables[i].Car = carSteerable;
            //            }
        }

        public void ApplyVehicleCmdAndState(VehicleEntity vehicle, IVehicleCmd cmd)
        {
            SetStateFromCmd(vehicle, cmd);
            WheelEntityMoveUtility.MoveByCmd(vehicle, cmd);
        }

//        private void MoveToCmdState(VehicleEntity vehicle, IVehicleCmd cmd)
//        {
//            //check state change
//            var currentState = vehicle.GetCurrentVehicleState();
//
//            // Find out how much of a correction we are making
//            var deltaPos = newState.Position - currentState.Position;
//            var deltaMagSq = deltaPos.sqrMagnitude;
//            var bodyLinearSpeedSq = currentState.LinearVelocity.sqrMagnitude;
//
//            // Snap position by default (big correction, or we are moving too slowly)
//            var updatePos = newState.Position;
//            var fixLinVel = Vector3.zero;
//
//            // If its a small correction and velocity is above threshold, only make a partial correction, 
//            // and calculate a velocity that would fix it over 'fixTime'.
//            if (deltaMagSq < PhysicErrorCorrection.Singleton.LinearDeltaThresholdSq &&
//                bodyLinearSpeedSq > PhysicErrorCorrection.Singleton.BodySpeedThresholdSq)
//            {
//                updatePos = Vector3.Lerp(currentState.Position, newState.Position, PhysicErrorCorrection.Singleton.LinearInterpAlpha);
//                fixLinVel = (newState.Position - updatePos) * PhysicErrorCorrection.Singleton.LinearRecipFixTime;
//            }
//
//
//            /////// ORIENTATION CORRECTION ///////
//            // Get quaternion that takes us from old to new
//            var currentQuat = Quaternion.Euler(currentState.Rotation);
//            var newQuat = Quaternion.Euler(newState.Rotation);
//
//            var invCurrentQuat = Quaternion.Inverse(currentQuat);
//            var deltaQuat = newQuat * invCurrentQuat;
//
//            var deltaAxis = new Vector3();
//            var deltaAng = .0f;
//            deltaQuat.ToAngleAxis(out deltaAng, out deltaAxis);
//            //clamp to [-90, 90]
//            if (deltaAng >= 180)
//            {
//                deltaAng = 360 - deltaAng;
//                deltaAxis = -deltaAxis;
//            }
//
//            if (deltaAng >= 90)
//            {
//                deltaAng -= 180;
//            }
//
//            deltaAng *= Mathf.Deg2Rad;
//
//            var updateRot = newState.Rotation;
//            var fixAngVel = Vector3.zero;
//            var absDeltaAng = Math.Abs(deltaAng);
//            const float epsilon = 0.0001f;
//            if (absDeltaAng < PhysicErrorCorrection.Singleton.AngularDeltaThreshold)
//            {
//                var updateQuat = Quaternion.Lerp(currentQuat, newQuat, PhysicErrorCorrection.Singleton.AngularInterpAlpha);
//                updateRot = updateQuat.eulerAngles;
//
//                if (absDeltaAng > epsilon)
//                {
//                    fixAngVel = deltaAxis.normalized * deltaAng * (1.0f - PhysicErrorCorrection.Singleton.AngularInterpAlpha) * PhysicErrorCorrection.Singleton.AngularRecipFixTime;
//                }
//            }
//            else
//            {
//                _logger.DebugFormat("Delta Angle is too large {0}", deltaAng);
//            }
//
//            if (deltaMagSq <= epsilon &&
//                fixLinVel.sqrMagnitude <= epsilon &&
//                deltaAng <= epsilon &&
//                fixAngVel.sqrMagnitude <= epsilon)
//            {
//                return;
//            }
//
//            /////// STATE UPDATE ///////
//            currentState.IsRemoteSync = true;
//            currentState.IsSyncInput = isClientSide;
//            currentState.SteerInput = newState.SteerInput;
//            currentState.ThrottleInput = newState.ThrottleInput;
//            currentState.BrakeInput = newState.BrakeInput;
//            currentState.HandbrakeInput = newState.HandbrakeInput;
//
//            currentState.Position = updatePos;
//            currentState.Rotation = updateRot;
//            currentState.LinearVelocity += fixLinVel;
//            currentState.AngularVelocity += fixAngVel;
//
//            //Update Wheels
//
//            var wheelStates = currentState.WheelStates;
//            Debug.Assert(wheelStates.Length == 4);
//
//            wheelStates[0].SteerAngle = newState.SteerAngle0;
//            wheelStates[1].SteerAngle = newState.SteerAngle1;
//            wheelStates[2].SteerAngle = newState.SteerAngle2;
//            wheelStates[3].SteerAngle = newState.SteerAngle3;
//        }

        private void SetStateFromCmd(VehicleEntity vehicle, IVehicleCmd cmd)
        {

            if (SharedConfig.DynamicPrediction)
            {
                DynamicPredictState(vehicle, cmd);
            }
            else
            {
                DirectSetStateFromCmd(vehicle, cmd);
            }
           
        }

        private void DynamicPredictState(VehicleEntity vehicle, IVehicleCmd cmd)
        {
            var vehicleCmd = (VehicleCmd)cmd;
            var controller = GetController(vehicle);

            var state = VehicleDynamicPredictionUtility.MoveToState(controller, vehicleCmd);

            SetWheelAnglesFromCmd(controller, vehicleCmd);
            SetControllerState(controller, 
                state.Position, state.Rotation, 
                state.LinearVelocity, state.AngularVelocity, 
                state.Crashed, state.IsSleeping, state.SleepingOutSync);
        }

        private void SetControllerState(VehicleAbstractController controller, Vector3 position, Quaternion rotation,
            Vector3 linearVelocity, Vector3 angularVelocity, 
            bool crashed)
        {
            SetControllerState(controller, position, rotation, 
                linearVelocity, angularVelocity, 
                crashed, controller.IsSleeping, 0);
        }

        private void SetControllerState(VehicleAbstractController controller, Vector3 position, Quaternion rotation,
            Vector3 linearVelocity, Vector3 angularVelocity, bool crashed, bool isSleeping, int sleepingOutSync)
        {
            controller.IsCrashed = crashed;
            VehicleDynamicPredictionUtility.SetControllerState(controller, 
                position, rotation, 
                linearVelocity, angularVelocity, 
                isSleeping, sleepingOutSync);
        }

        private void SetWheelAnglesFromCmd(VehicleAbstractController controller, VehicleCmd cmd)
        {
            var wheelCount = controller.WheelCount;
            for (int i = 0; i < wheelCount; ++i)
            {
                var wheel = controller.GetWheel(i);
                wheel.wheelCollider.steerAngle = cmd.Steerables[i].Car.Angle;
            }
        }

        private void DirectSetStateFromCmd(VehicleEntity vehicle, IVehicleCmd cmd)
        {
            var vehicleCmd = (VehicleCmd)cmd;
            var controller = GetController(vehicle);

            SetControllerState(controller, vehicleCmd.Body.Position, vehicleCmd.Body.Rotation, 
                vehicleCmd.Body.LinearVelocity, vehicleCmd.Body.AngularVelocity,
                vehicleCmd.Body.Crashed);

            SetWheelPosesFromCmd(controller, vehicleCmd);
            //          var state = controller.GetCurrentState();
            //
            //            state.IsSyncFlagSet = true;
            //
            //            state.BodyState.Position = vehicleCmd.Body.Position;
            //            state.BodyState.Rotation = vehicleCmd.Body.Rotation;
            //            state.BodyState.Body2WorldPosition = vehicleCmd.Body.Body2WorldPosition;
            //            state.BodyState.Body2WorldRotation = vehicleCmd.Body.Body2WorldRotation;
            //            state.BodyState.WakeCounter = vehicleCmd.Body.WakeCounter;
            //
            //            var wheels = state.WheelStates;
            //            for (int i = 0; i < vehicleCmd.SteerableCount; ++i)
            //            {
            //                var wheel = wheels[i];
            //                var carSteerable = vehicleCmd.Steerables[i].Car;
            //
            //                wheel.ColliderSteerAngle = carSteerable.ColliderSteerAngle;
            //                wheel.SteerAngle = carSteerable.SteerAngle;
            //                wheel.MotorTorque = carSteerable.MotorTorque;
            //
            //                wheel.SuspensionDistance = carSteerable.SuspensionDistance;
            //                wheel.SuspensionSpring = carSteerable.SuspensionSpring;
            //                wheel.SuspensionTargetPosition = carSteerable.SuspensionTargetPosition;
            //
            //                wheel.SprungMass = carSteerable.SprungMass;
            //                wheel.ForceDistance = carSteerable.ForceDistance;
            //
            //                wheel.AngularVelocity = carSteerable.AngularVelocity;
            //                wheel.AngularPosition = carSteerable.AngularPosition;
            //                wheel.RotationAngle = carSteerable.RotationAngle;
            //
            //                wheel.RotationSpeed = carSteerable.RotationSpeed;
            //
            //                wheel.CorrectedRotationSpeed = carSteerable.CorrectedRotationSpeed;
            //                wheel.Jounce = carSteerable.Jounce;
            //
            //                wheel.TireLowSideSpeedTimers = carSteerable.TireLowSideSpeedTimers;
            //                wheel.TireLowforwardSpeedTimers = carSteerable.TireLowforwardSpeedTimers;
            //                wheel.HitInfo.Grounded = carSteerable.Grounded;
            //
            //                wheel.HitInfo.Point = carSteerable.Point;
            //
            //                wheel.HitInfo.Normal = carSteerable.Normal;
            //                wheel.HitInfo.ForwardDir = carSteerable.ForwardDir;
            //
            //                wheel.HitInfo.SidewaysDir = carSteerable.SidewaysDir;
            //                wheel.HitInfo.Force = carSteerable.Force;
            //
            //                wheel.HitInfo.ForwardSlip = carSteerable.ForwardSlip;
            //                wheel.HitInfo.SidewaysSlip = carSteerable.SidewaysSlip;
            //            }
            //
            //            ApplyState(vehicle);
        }

        private void SetWheelPosesFromCmd(VehicleAbstractController controller, VehicleCmd cmd)
        {
            var wheelCount = controller.WheelCount;
            for (int i = 0; i < wheelCount; ++i)
            {
                var wheel = controller.GetWheel(i);
                var wd = controller.GetWheelData(i);
                wheel.wheelTransform.localPosition = cmd.Steerables[i].Car.Position;
                wheel.wheelTransform.localRotation = cmd.Steerables[i].Car.Rotation;
//                wd.Grounded = cmd.Steerables[i].Car.Grounded;
//                wd.GroundedOnTerrain = cmd.Steerables[i].Car.Grounded;
            }
        }

        private static void GetStateFromDynamicData(VehicleEntity vehicle, VehicleAbstractInternState state)
        {
            var data = (CarRewindDataComponent) vehicle.GetDynamicData();
            state.SetFrom(vehicle);
            data.Clear();
        }

        public void SyncFromComponentToController(VehicleEntity vehicle, VehicleAbstractInternState state)
        {
            if (SharedConfig.DynamicPrediction)
            {
                DynamicSyncFromComponentToController(vehicle, state);
            }
            else
            {
                DirectSyncFromComponentToController(vehicle, state);
            }
        }

        private void DynamicSyncFromComponentToController(VehicleEntity vehicle, VehicleAbstractInternState internalState)
        {
            var controller = GetController(vehicle);
            var state = VehicleDynamicPredictionUtility.MoveToState(controller, internalState);

            SetWheelAngleFromState(controller, internalState);
            SetWheelEntityInput(controller, internalState);
            SetControllerState(controller, 
                state.Position, state.Rotation, 
                state.LinearVelocity, state.AngularVelocity, 
                state.Crashed, state.IsSleeping, state.SleepingOutSync);
        }

        private void SetWheelEntityInput(VehicleAbstractController controller, VehicleAbstractInternState internalState)
        {
            controller.steerInput = internalState.SteerInput;
            controller.throttleInput = internalState.ThrottleInput;
            controller.handbrakeInput = internalState.HandbrakeInput;
            controller.isHornOn = internalState.IsHornOn;
            controller.isAccelerated = internalState.IsAccelerated;

            if (controller.ControllerType == VehicleControllerType.Car)
            {
                var carInternState = (CarInternState) internalState;
                controller.brakeInput = carInternState.BrakeInput;
            }else if (controller.ControllerType == VehicleControllerType.Motor)
            {
                var motorController = (MotorcycleController) controller;
                var motorInsternState = (MotorInternState) internalState;

                motorController.stuntInput = motorInsternState.StuntInput;
                motorController.horizontalMassShift = motorInsternState.HorizontalShift;
            }
        }

        private void SetWheelAngleFromState(VehicleAbstractController controller, VehicleAbstractInternState internalState)
        {
            var wheelCount = controller.WheelCount;
            for (int i = 0; i < wheelCount; ++i)
            {
                var wheel = controller.GetWheel(i);
                wheel.wheelCollider.steerAngle = internalState.Wheels[i].WheelSteerAngle;
            }
        }

        private void DirectSyncFromComponentToController(VehicleEntity vehicle, VehicleAbstractInternState state)
        {
            SyncStateFromComponentToController(vehicle, state);
        }

        private  void SyncStateFromComponentToController(VehicleEntity vehicle, VehicleAbstractInternState state)
        {

            if (!state.IsSet() || SharedConfig.IsOffline)
            {
                return;
            }

            var currentState = GetCurrentState(vehicle);
            state.SetTo(currentState);
            ApplyState(vehicle);
        }

        private static VehicleAbstractState GetCurrentState(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller.GetCurrentState();
        }

        private static void ApplyState(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            controller.ApplyVehicleState();
        }

        public void SetVehicleSyncLatest(VehicleEntity vehicle, bool isSyncLatest)
        {
            var dynamicData = vehicle.GetDynamicData();
            dynamicData.IsSyncLatest = isSyncLatest;
            if (vehicle.hasCarFirstRewnWheel)
                vehicle.carFirstRewnWheel.IsSyncLatest = isSyncLatest;
            if (vehicle.hasCarSecondRewnWheel)
                vehicle.carSecondRewnWheel.IsSyncLatest = isSyncLatest;
            if (vehicle.hasCarThirdRewnWheel)
                vehicle.carThirdRewnWheel.IsSyncLatest = isSyncLatest;
            if (vehicle.hasCarFourthRewnWheel)
                vehicle.carFourthRewnWheel.IsSyncLatest = isSyncLatest;
        }

//        public string[] GetDebugInfo(VehicleEntity vehicle, String stateStr, String filterStr)
//        {
//            var controller = GetController(vehicle);
//            return controller.GetDebugInfo(stateStr, filterStr);
//        }

    }
}