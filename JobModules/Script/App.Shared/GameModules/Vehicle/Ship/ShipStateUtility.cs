using System.Collections.Generic;
using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Vehicle.Common;
using App.Shared.GameModules.Vehicle.Ship;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using DWP;
using UnityEngine;
using XmlConfig;

namespace App.Shared.GameModules.Vehicle.Ship
{
    public class ShipStateUtility : IVehicleStateUtility
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ShipStateUtility));

        private static AdvancedShipController GetController(VehicleEntity vehicle)
        {
            return vehicle.GetController<AdvancedShipController>();
        }

        public void FixedUpdate(VehicleEntity vehicle)
        {
            if (vehicle.hasGameObject)
            {
                var controller = GetController(vehicle);

                controller.DoFixedUpdate();
            }
        }

        public bool IsReadyForSync(VehicleEntity vehicle)
        {
            return vehicle.hasShipDynamicData && vehicle.IsActiveSelf();
        }

        public void SyncOnPlayerRideOn(VehicleEntity vehicle)
        {
            
        }

        private ShipInternState _tempState = new ShipInternState();
        public void SyncFromComponent(VehicleEntity vehicle)
        {
            GetStateFromDynamicData(vehicle, _tempState);
            SyncFromComponentToDWP(vehicle, _tempState);
        }

        private void GetStateFromDynamicData(VehicleEntity vehicle, ShipInternState state)
        {
            var data = (ShipDynamicDataComponent)vehicle.GetDynamicData();
            state.Flag = data.Flag;

            state.IsAccelerated = data.IsAccelerated;
            state.SteerInput = data.SteerInput;
            state.ThrottleInput = data.ThrottleInput;

            state.Position = data.Position;
            state.Rotation = data.Rotation;
            state.IsSleeping = data.IsSleeping;
            state.LinearVelocity = data.LinearVelocity;
            state.AngularVelocity = data.AngularVelocity;

            var indexArray = VehicleIndexHelper.GetRudderIndexArray();
            for (int i = 0; i < indexArray.Length; ++i)
            {
                var index = indexArray[i];
                if (ShipEntityUtility.HasRudder(vehicle, index))
                {
                    state.RudderStates[i] = GetRudderState(ShipEntityUtility.GetRudder(vehicle, index));
                }
            }

            data.Clear();
        }

        private RudderInternState GetRudderState(ShipRudderDynamicData comp)
        {
            var rudderState = new RudderInternState();
            rudderState.Angle = comp.Angle;
            return rudderState;
        }

        private void SyncFromComponentToDWP(VehicleEntity vehicle, ShipInternState state)
        {
            if (SharedConfig.DynamicPrediction)
            {
                DynamicSyncFromComponentToDWP(vehicle, state);
            }
            else
            {
                DirectSyncFromComponentToDWP(vehicle, state);
            }
        }

        private void DynamicSyncFromComponentToDWP(VehicleEntity vehicle, ShipInternState internalState)
        {
            var controller = GetController(vehicle);
            var state = VehicleDynamicPredictionUtility.MoveToState(controller, internalState);

            SetRudderAnglesFromState(controller, internalState);
            SetShipInput(controller, internalState);
            VehicleDynamicPredictionUtility.SetControllerState(controller, 
                state.Position, state.Rotation, 
                state.LinearVelocity, state.AngularVelocity, 
                state.IsSleeping, state.SleepingOutSync);
        }

        private void SetShipInput(AdvancedShipController controller, ShipInternState state)
        {
            controller.ThrottleInput = state.ThrottleInput;
            controller.RudderInput = state.SteerInput;
            controller.IsAccelerated = state.IsAccelerated;
        }

        private void SetRudderAnglesFromState(AdvancedShipController controller, ShipInternState state)
        {
            var rudders = controller.rudders;
            for (int i = 0; i < rudders.Count; ++i)
            {
                rudders[i].Angle = state.RudderStates[i].Angle;
            }
        }

        private void DirectSyncFromComponentToDWP(VehicleEntity vehicle, ShipInternState state)
        {
            SyncStateFromComponentToDWP(vehicle, state);
        }

        private void SyncStateFromComponentToDWP(VehicleEntity vehicle, ShipInternState state)
        {
            if (!state.IsSet() || SharedConfig.IsOffline)
            {
                return;
            }

            var currentState = GetCurrentState(vehicle);

            currentState.IsFlagSync = state.IsRemoteSet();
            currentState.IsAccelerated = state.IsAccelerated;
            currentState.RudderInput = state.SteerInput;
            currentState.ThrottleInput = state.ThrottleInput;

            currentState.Position = state.Position;
            currentState.Rotation = state.Rotation;
            currentState.IsSleeping = state.IsSleeping;
            currentState.BodyState.LinearVelocity = state.LinearVelocity;
            currentState.BodyState.AngularVelocity = state.AngularVelocity;
            

            var rudderStates = currentState.RudderStates;
            var rudderCount = rudderStates.Length;
            for (int i = 0; i < rudderCount; ++i)
            {
                var destState = rudderStates[i];
                var sourceState = state.RudderStates[i];

                destState.Angle = sourceState.Angle;
            }

            ApplyState(vehicle);
        }

        private static void ApplyState(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            controller.ApplyShipState();
        }

        public void SyncToComponent(VehicleEntity vehicle)
        {
            var state = GetCurrentState(vehicle);
            var data = vehicle.shipDynamicData;
   
            data.Flag = (int)VehicleFlag.LocalSet;

            data.IsAccelerated = state.IsAccelerated;
            data.SteerInput = state.RudderInput;
            data.ThrottleInput = state.ThrottleInput;

            data.Position = state.Position;
            data.Rotation = state.Rotation;
            data.IsSleeping = state.IsSleeping;
            data.LinearVelocity = state.BodyState.LinearVelocity;
            data.AngularVelocity = state.BodyState.AngularVelocity;

            var indexArray = VehicleIndexHelper.GetRudderIndexArray();
            var rudderCount = GetController(vehicle).rudders.Count;
            for (int i = 0; i < rudderCount; ++i)
            {
                var index = indexArray[i];
                if (ShipEntityUtility.HasRudder(vehicle, index))
                {
                    SetShipRudderState(vehicle, index, state.RudderStates[i]);
                }
            }
        }


        private void SetShipRudderState(VehicleEntity vehicle, VehiclePartIndex index, RudderState state)
        {
            var rudder = ShipEntityUtility.GetRudder(vehicle, index);
            rudder.Angle = state.Angle;
        }

        public void Update(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            controller.DoUpdate();
        }

        private static ShipState GetCurrentState(VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller.GetShipState();
        }

        public void SetVehicleStateToCmd(VehicleEntity vehicle, IVehicleCmd cmd)
        {
            var vehicleCmd = (VehicleCmd)cmd;
            var controller = GetController(vehicle);
           

            vehicleCmd.VehicleType = (int) EVehicleType.Ship;
            vehicleCmd.Body.Position = controller.cachedRigidbody.position;
            vehicleCmd.Body.Rotation = controller.cachedRigidbody.rotation;
            vehicleCmd.Body.LinearVelocity = controller.Velocity;
            vehicleCmd.Body.AngularVelocity = controller.AngularVelocity;
            vehicleCmd.Body.IsSleeping = controller.IsSleeping;

            var rudders = controller.rudders;
            vehicleCmd.SteerableCount = rudders.Count;
            for (int i = 0; i < rudders.Count; ++i)
            {
                vehicleCmd.Steerables[i].Ship.Angle = rudders[i].Angle;
            }

//             var state = controller.GetShipState();
//            vehicleCmd.Body.Position = state.Position;
//            vehicleCmd.Body.Rotation = state.Rotation;
//            vehicleCmd.Body.Body2WorldPosition = state.BodyState.Body2WorldPosition;
//            vehicleCmd.Body.Body2WorldRotation = state.BodyState.Body2WorldRotation;
//            vehicleCmd.Body.LinearVelocity = state.BodyState.LinearVelocity;
//            vehicleCmd.Body.AngularVelocity = state.BodyState.AngularVelocity;
//
//            var rudders = state.RudderStates;
//            vehicleCmd.SteerableCount = rudders.Length;
//            for (int i = 0; i < rudders.Length; ++i)
//            {
//                var rudder = rudders[i];
//                var shipSteerable = vehicleCmd.Steerables[i].Ship;
//                shipSteerable.Angle = rudder.Angle;
//                shipSteerable.Rpm = rudder.Rpm;
//                shipSteerable.SpinVelocity = rudder.SpinVelocity;
//                shipSteerable.Submerged = rudder.Submerged;
//
//                vehicleCmd.Steerables[i].Ship = shipSteerable;
//            }
        }

        public void ApplyVehicleCmdAndState(VehicleEntity vehicle, IVehicleCmd cmd)
        {
            SetStateFromCmd(vehicle, cmd);
            ShipMoveInternal.Move(vehicle, cmd);
        }

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
            SetRudderAnglesFromCmd(controller, vehicleCmd);
            VehicleDynamicPredictionUtility.SetControllerState(controller, 
                state.Position, state.Rotation, 
                state.LinearVelocity, state.AngularVelocity, 
                state.IsSleeping, state.SleepingOutSync);
        }

        private void DirectSetStateFromCmd(VehicleEntity vehicle, IVehicleCmd cmd)
        {
            var vehicleCmd = (VehicleCmd)cmd;
            var body = vehicleCmd.Body;
            var controller = GetController(vehicle);
            SetRudderAnglesFromCmd(controller, vehicleCmd);
            VehicleDynamicPredictionUtility.SetControllerState(controller, 
                body.Position, body.Rotation, 
                body.LinearVelocity, body.AngularVelocity, 
                controller.IsSleeping, 0);

            //            var state = controller.GetShipState();
            //
            //            state.IsFlagSync = true;
            //
            //            state.Position = vehicleCmd.Body.Position;
            //            state.Rotation = vehicleCmd.Body.Rotation;
            //            state.BodyState.Body2WorldPosition = vehicleCmd.Body.Body2WorldPosition;
            //            state.BodyState.Body2WorldRotation = vehicleCmd.Body.Body2WorldRotation;
            //            state.BodyState.LinearVelocity = vehicleCmd.Body.LinearVelocity;
            //            state.BodyState.AngularVelocity = vehicleCmd.Body.AngularVelocity;
            //
            //            var rudders = state.RudderStates;
            //            for (int i = 0; i < vehicleCmd.SteerableCount; ++i)
            //            {
            //                var rudder = rudders[i];
            //                var shipSteerable = vehicleCmd.Steerables[i].Ship;
            //                rudder.Angle = shipSteerable.Angle;
            //                rudder.Rpm = shipSteerable.Rpm;
            //                rudder.SpinVelocity = shipSteerable.SpinVelocity;
            //                rudder.Submerged = shipSteerable.Submerged;
            //            }
            //
            //            controller.ApplyShipState();
        }

        private void SetRudderAnglesFromCmd(AdvancedShipController controller, VehicleCmd cmd)
        {
            var rudders = controller.rudders;
            for (int i = 0; i < rudders.Count; ++i)
            {
                rudders[i].Angle = cmd.Steerables[i].Ship.Angle;
            }
        }

        public void SetVehicleSyncLatest(VehicleEntity vehicle, bool isSyncLatest)
        {
            var dynamicData = vehicle.GetDynamicData();
            dynamicData.IsSyncLatest = isSyncLatest;
            if (vehicle.hasShipFirstRudderDynamicData)
                vehicle.shipFirstRudderDynamicData.IsSyncLatest = isSyncLatest;
            if (vehicle.hasShipSecondRudderDynamicData)
                vehicle.shipSecondRudderDynamicData.IsSyncLatest = isSyncLatest;
        }
    }
}
