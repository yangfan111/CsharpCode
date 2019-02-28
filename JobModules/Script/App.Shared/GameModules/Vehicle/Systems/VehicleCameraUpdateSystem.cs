using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Player;
using BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core.WeaponLogic;
using Entitas;
using UnityEngine;

namespace App.Shared.GameModules.Vehicle
{
    public class VehicleCameraUpdateSystem : IUserCmdExecuteSystem
    {
        private VehicleContext _vehicleContext;
        private bool _newVehicle = true;
        private float _lastTime;
        public VehicleCameraUpdateSystem(Contexts contexts)
        {
            _vehicleContext = contexts.vehicle;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = (PlayerEntity) owner.OwnerEntity;
            if (!player.IsOnVehicle() || !player.hasCameraConfigNow)
            {
                _newVehicle = true;
                return;
            }

            if (_newVehicle)
            {
                _lastTime = Time.time;
                _newVehicle = false;
                return;
            }

            var currentTime = Time.time;

            var controlledVehicle = player.controlledVehicle;
            var vehicleEntity = _vehicleContext.GetEntityWithEntityKey(controlledVehicle.EntityKey);
            if (vehicleEntity == null)
            {
                _lastTime = currentTime;
                return;
            }
            var target = vehicleEntity.gameObject.UnityObject.AsGameObject.transform;
            var targetOffset = target.TransformDirection(controlledVehicle.CameraLocalTargetOffSet);

            var right = target.right;
            right.y = 0.0f;

            if (right.sqrMagnitude > 1e-4f)
            {
                right = right.normalized;

                var forward = Vector3.Cross(right, Vector3.up);
                const float radius = 0.5f;
                var localPosition = forward * radius;

                var localLastPosition = controlledVehicle.CameraLastPosition;

                if (Vector3.Dot(localLastPosition, localPosition) < 0)
                {
                    localPosition = -localPosition;
                }

                var deltaTime = currentTime - _lastTime;
                var wantedAngle = Mathf.Atan2(localPosition.x, localPosition.z) * Mathf.Rad2Deg;
                var currentAngle = Mathf.LerpAngle(controlledVehicle.CameraLastAngle, wantedAngle, deltaTime * controlledVehicle.CameraRotationDamping);
                
#if UNITY_EDITOR
                if (Mathf.Abs(currentAngle - controlledVehicle.CameraLastAngle) > 3.0f)
                {
                    Debug.LogFormat("Time delta {0} is too large, vehicle camera rotation angle from {1} to {2}, wanted angle {3}",
                        deltaTime, controlledVehicle.CameraLastAngle, currentAngle, wantedAngle);
                }
#endif

                var center = target.position + targetOffset;
                controlledVehicle.CameraCurrentPosition = center;
                controlledVehicle.CameraCurrentRotation = Quaternion.Euler(0.0f, currentAngle, 0.0f);

                controlledVehicle.CameraLastPosition = localPosition;
                controlledVehicle.CameraLastAngle = currentAngle;
            }

            _lastTime = currentTime;
        }

        private float NoramlizeAngle(float angle)
        {
            if (angle < -180)
            {
                angle += 360;
            }else if (angle > 180)
            {
                angle -= 360;
            }

            return angle;
        }

        private Quaternion LookAt(Vector3 position, Vector3 targetPosition, Quaternion rotation)
        {
            const float epsilon = 1e-5f;
            var viewVec = targetPosition - position;
            if (viewVec.magnitude > epsilon)
            {
                var z = viewVec.normalized;
                var x = Vector3.Cross(Vector3.up, z);
                if (x.magnitude > epsilon)
                {
                    x = x.normalized;

                    var y = Vector3.Cross(z, x);

                    if (Mathf.Abs(y.sqrMagnitude - 1.0f) < epsilon)
                    {
                        Matrix4x4 m = new Matrix4x4();
                        m.SetColumn(0, x);
                        m.SetColumn(1, y);
                        m.SetColumn(2, z);

                        return m.ExtractRotation();
                    }
                }
            }
            Debug.LogFormat("Rotation Not Chage {0}",rotation);
            return rotation;
        }

    }
}
