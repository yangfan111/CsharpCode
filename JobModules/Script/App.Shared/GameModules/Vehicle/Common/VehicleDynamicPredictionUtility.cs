using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Configuration;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using VehicleCommon;

namespace App.Shared.GameModules.Vehicle.Common
{
    public struct VehicleDynamicState
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 LinearVelocity;
        public Vector3 AngularVelocity;
        public bool Crashed;
        public bool IsSleeping;
        public int SleepingOutSync;
    }

    public static class VehicleDynamicPredictionUtility
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(VehicleDynamicPredictionUtility));

        public static VehicleDynamicState MoveToState(VehicleCommonController controller, VehicleCmd cmd)
        {
            return MoveToStateInternal(controller, GetStateFromCmd(cmd));
        }

        public static VehicleDynamicState MoveToState(VehicleCommonController controller, VehicleInternState state)
        {
            return MoveToStateInternal(controller, GetStateFromInternalState(state));
        }

        private static VehicleDynamicState GetStateFromCmd(VehicleCmd cmd)
        {
            var state = new VehicleDynamicState()
            {
                Position = cmd.Body.Position,
                Rotation = cmd.Body.Rotation,
                LinearVelocity = cmd.Body.LinearVelocity,
                AngularVelocity = cmd.Body.AngularVelocity,
                IsSleeping = cmd.Body.IsSleeping,
                Crashed = false,//state 'crashed' is dertermined by server. 
            };

            return state;
        }

        private static VehicleDynamicState GetStateFromInternalState(VehicleInternState internalState)
        {
            var state = new VehicleDynamicState()
            {
                Position = internalState.Position,
                Rotation = internalState.Rotation,
                LinearVelocity = internalState.LinearVelocity,
                AngularVelocity = internalState.AngularVelocity,
                Crashed = internalState.Crashed,
                IsSleeping = internalState.IsSleeping,
            };

            return state;

        }

        private static VehicleDynamicState GetStateFromController(VehicleCommonController controller)
        {
            var body = controller.cachedRigidbody;
            var state = new VehicleDynamicState()
            {
                Position = body.position,
                Rotation = body.rotation,
                LinearVelocity = body.velocity,
                AngularVelocity = body.angularVelocity,
                IsSleeping =  body.IsSleeping(),
                SleepingOutSync = controller.SleepingOutSync,
            };

            return state;
        }

        private static VehicleDynamicState MoveToStateInternal(VehicleCommonController controller, VehicleDynamicState targetState)
        {
            if (controller.IsKinematic)
            {
                return targetState;
            }

            var currentState = GetStateFromController(controller);
            //ShiftContactObject(vehicle, targetState.Position - currentState.Position);
            return MoveToStateInternal(currentState, targetState);
        }

        private static VehicleDynamicState MoveToStateInternal(VehicleDynamicState currentState, VehicleDynamicState targetState)
        {
            // Find out how much of a correction we are making
            var targetStatePosition = targetState.Position;
            var currentStatePosition = currentState.Position;

            Vector3 deltaPos;
            deltaPos.x = targetStatePosition.x - currentStatePosition.x;
            deltaPos.y = targetStatePosition.y - currentStatePosition.y;
            deltaPos.z = targetStatePosition.z - currentStatePosition.z;

            var deltaMagSq = deltaPos.x * deltaPos.x + deltaPos.y * deltaPos.y + deltaPos.z * deltaPos.z;
            var targetLinVelocity = targetState.LinearVelocity;
            var targetLinVelSqrMag = targetLinVelocity.x * targetLinVelocity.x +
                                     targetLinVelocity.y * targetLinVelocity.y +
                                     targetLinVelocity.z * targetLinVelocity.z;
            var currentLinVelocity = currentState.LinearVelocity;
            var currentLinVelSqrMag = currentLinVelocity.x * currentLinVelocity.x + 
                                      currentLinVelocity.y * currentLinVelocity.y + 
                                      currentLinVelocity.z * currentLinVelocity.z;
            var bodyLinearSpeedSq = targetLinVelSqrMag > currentLinVelSqrMag ? targetLinVelSqrMag : currentLinVelSqrMag;

            // Snap position by default (big correction, or we are moving too slowly)
            var updatePos = targetStatePosition;
            Vector3 fixLinVel;
            fixLinVel.x = fixLinVel.y = fixLinVel.z = 0.0f;

            var errorCorrection = SingletonManager.Get<DynamicPredictionErrorCorrectionConfigManager>();

            // If its a small correction and velocity is above threshold, only make a partial correction, 
            // and calculate a velocity that would fix it over 'fixTime'.
            if (deltaMagSq < errorCorrection.LinearDeltaThresholdSq && (deltaMagSq > errorCorrection.BodyPositionThresholdSq
                || bodyLinearSpeedSq > errorCorrection.BodySpeedThresholdSq))
            {
                updatePos = Vector3.Lerp(currentState.Position, targetState.Position, errorCorrection.LinearInterpAlpha);
                var linearRecipFixTime = errorCorrection.LinearRecipFixTime;
                fixLinVel.x = (targetStatePosition.x - updatePos.x) * linearRecipFixTime;
                fixLinVel.y = (targetStatePosition.y - updatePos.y) * linearRecipFixTime;
                fixLinVel.z = (targetStatePosition.z - updatePos.z) * linearRecipFixTime;
            }
//            else
//            {
//                _logger.DebugFormat("Delta Position is too large {0}", (targetState.Position - currentState.Position).magnitude);
//            }

            /////// ORIENTATION CORRECTION ///////
            // Get quaternion that takes us from old to new
            var newRot = targetState.Rotation;

            var invCurrentRot = Quaternion.Inverse(currentState.Rotation);
            var deltaRot = newRot * invCurrentRot;

            Vector3 deltaAxis;
            float deltaAng = .0f;
            deltaRot.ToAngleAxis(out deltaAng, out deltaAxis);
            //clamp to [-90, 90]
            if (deltaAng >= 180)
            {
                deltaAng = 360 - deltaAng;
                deltaAxis = -deltaAxis;
            }

            if (deltaAng >= 90)
            {
                deltaAng -= 180;
            }

            deltaAng *= Mathf.Deg2Rad;

            var updateRot = targetState.Rotation;
            Vector3 fixAngVel;
            fixAngVel.x = fixAngVel.y = fixAngVel.z = 0;
            var absDeltaAng = Math.Abs(deltaAng);
            if (absDeltaAng < errorCorrection.AngularDeltaThreshold)
            {
                updateRot = Quaternion.Lerp(currentState.Rotation, newRot, errorCorrection.AngularInterpAlpha);
                fixAngVel = VehicleMath.Vector3Normalized(deltaAxis);
                var recipFixTime = deltaAng * (1.0f - errorCorrection.AngularInterpAlpha) * errorCorrection.AngularRecipFixTime;
                fixAngVel.x *= recipFixTime;
                fixAngVel.y *= recipFixTime;
                fixAngVel.z *= recipFixTime;
            }
//            else
//            {
//                _logger.DebugFormat("Delta Angle is too large {0}", deltaAng);
//            }

            /////// STATE UPDATE ///////
            var lerpState = targetState;
            lerpState.Position = updatePos;
            lerpState.Rotation = updateRot;
            var lerpStateLinVel = lerpState.LinearVelocity;
            lerpStateLinVel.x += fixLinVel.x;
            lerpStateLinVel.y += fixLinVel.y;
            lerpStateLinVel.z += fixLinVel.z;
            lerpState.LinearVelocity = lerpStateLinVel;
            var lerpStateAngVel = lerpState.AngularVelocity;
            lerpStateAngVel.x += fixAngVel.x;
            lerpStateAngVel.y += fixAngVel.y;
            lerpStateAngVel.z += fixAngVel.z;
            lerpState.AngularVelocity = lerpStateAngVel;
            lerpState.Crashed = targetState.Crashed;

            var fixLinVelSqrMag = fixLinVel.x * fixLinVel.x + fixLinVel.y * fixLinVel.y + fixLinVel.z * fixLinVel.z;
            var fixAngVelSqrMag = fixAngVel.x * fixAngVel.x + fixAngVel.y * fixAngVel.y + fixAngVel.z * fixAngVel.z;
            const float epsilon = 0.0001f;
            const int maxSleepingOutSyncCount = 100;

            if (targetState.IsSleeping)
            {
                lerpState.SleepingOutSync = currentState.SleepingOutSync + 1;
            }
            else
            {
                lerpState.SleepingOutSync = 0;
            }

            if (targetState.IsSleeping
                && (currentState.SleepingOutSync > maxSleepingOutSyncCount || //put to sleep if target have been sleeping for a long time, prevent vehicle jitter on some cases.
                (fixLinVelSqrMag <= epsilon  && fixAngVelSqrMag <= epsilon)))
            {
                lerpState.Position = targetState.Position;
                lerpState.Rotation = targetState.Rotation;
                lerpState.IsSleeping = true;
                
            }
            else
            {
                lerpState.IsSleeping = false;
            }

//            Debug.LogFormat("id {0}, isSleeping {1}, fixLinearVel {2}, fixAngVel {3}", 
//                vehicle.entityKey.Value.EntityId, 
//                lerpState.IsSleeping,
//                fixLinVel.sqrMagnitude,
//                fixAngVel.sqrMagnitude);

            return lerpState;
        }

        public static void SetControllerState(VehicleCommonController controller, Vector3 position, Quaternion rotation, Vector3 linearVelocity, Vector3 angularVelocity, bool isSleeping, int sleepingOutSync)
        {
            var isKinematic = controller.IsKinematic;
            controller.cachedRigidbody.position = position;
            controller.cachedRigidbody.rotation = rotation;
            if (!isKinematic)
            {
                controller.Velocity = linearVelocity;
                controller.AngularVelocity = angularVelocity;
            }
            
            controller.SleepingOutSync = sleepingOutSync;
            if (isSleeping && !isKinematic)
            {
                controller.PutToSleepImmediate();
            }
        }

        //        private static void ShiftContactObject(VehicleEntity vehicle, Vector3 vecError)
        //        {
        //            if (!SharedConfig.IsServer && vecError.sqrMagnitude < 1.0f)
        //            {
        //                var go = vehicle.gameObject.Value;
        //                var controller = go.GetComponent<VehicleCommonController>();
        //                controller.ShiftContactObjects(vecError);
        //            }
        //        }
    }
}
