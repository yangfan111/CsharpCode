using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Vehicle.Ship;
using App.Shared.GameModules.Vehicle.WheelCarrier;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using UnityEngine;
using VehicleCommon;

namespace App.Shared.GameModules.Vehicle
{
    public static class VehicleStateUtility
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(VehicleStateUtility));

        private static IVehicleStateUtility[] EntityAPI = {
            new WheelEntityStateUtility(),
            new ShipStateUtility()
        };

        public static bool IsReadyForSync(VehicleEntity vehicle)
        {
            return EntityAPI[vehicle.GetTypeValue()].IsReadyForSync(vehicle);
        }

        public static void SyncOnPlayerRideOn(VehicleEntity vehicle)
        {
            EntityAPI[vehicle.GetTypeValue()].SyncOnPlayerRideOn(vehicle);
        }

        public static void SyncFromComponent(VehicleEntity vehicle)
        {
            var earlyReturn = SyncFromConponentEarlyReturn(vehicle);
            if(!earlyReturn)
                EntityAPI[vehicle.GetTypeValue()].SyncFromComponent(vehicle);
        }

        private static bool SyncFromConponentEarlyReturn(VehicleEntity vehicle)
        {
            if (SharedConfig.IsOffline)
            {
                return true;
            }

            var data = vehicle.GetDynamicData();
            if (!data.IsRemoteSet())
            {
                return true;
            }

            var controller = vehicle.GetController<VehicleCommonController>();
            if (controller.IsKinematic)
            {
                if (!IsTransformSameWithComponent(controller, data))
                {
                    controller.transform.SetPositionAndRotation(data.Position, data.Rotation);
                }

                return true;

            }else if (controller.IsSleeping && data.IsSleeping)
            {
                if (IsTransformSameWithComponent(controller, data))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsTransformSameWithComponent(VehicleCommonController controller, VehicleDynamicDataComponent data)
        {
            const float epsilon = 1e-6f;

            var position = controller.cachedTransform.position;
            var dataPosition = data.Position;
            Vector3 deltaPosition;
            deltaPosition.x = position.x - dataPosition.x;
            deltaPosition.y = position.y - dataPosition.y;
            deltaPosition.z = position.z - dataPosition.z;

            if ((deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y +
                 deltaPosition.z * deltaPosition.z) > epsilon)
            {
                return false;
            }

            var rot = controller.cachedTransform.rotation;
            var dataRot = data.Rotation;
            Quaternion deltaRot;
            deltaRot.x = rot.x - dataRot.x;
            deltaRot.y = rot.y - dataRot.y;
            deltaRot.z = rot.z - dataRot.z;
            deltaRot.w = rot.w - dataRot.w;

            if ((deltaRot.x * deltaRot.x + deltaRot.y * deltaRot.y + deltaRot.z * deltaRot.z +
                 deltaRot.w * deltaRot.w) > epsilon)
            {
                return false;
            }

            return true;

        }
       


        public static void FixedUpdate(VehicleEntity vehicle)
        {
            EntityAPI[vehicle.GetTypeValue()].FixedUpdate(vehicle);
        }

        public static void Update(VehicleEntity vehicle)
        {
            EntityAPI[vehicle.GetTypeValue()].Update(vehicle);
        }

        public static void SyncToComponent(VehicleEntity vehicle, bool forceSync = false)
        {
            var isStateChagned = forceSync || CheckAndResetStateChanged(vehicle);
            if (isStateChagned)
            {
                if (SharedConfig.IsServer || forceSync)
                {
                    EntityAPI[vehicle.GetTypeValue()].SyncToComponent(vehicle);
                }
                else
                {
                    //for client, we only need to sync position and rotation
                    var data = vehicle.GetDynamicData();
                    var controller = vehicle.GetController<VehicleCommonController>();
                    data.Position = controller.cachedRigidbody.position;
                    data.Rotation = controller.cachedRigidbody.rotation;
                }
            }
        }

        private static bool CheckAndResetStateChanged( VehicleEntity vehicle)
        {
            var controller = vehicle.GetController<VehicleCommonController>();
            var isStateChanged = controller.isStateChanged;
            controller.isStateChanged = false;
            return isStateChanged;
        }

        public static void SetVehicleStateToCmd(this VehicleEntity vehicle, IVehicleCmd cmd)
        {
            EntityAPI[vehicle.GetTypeValue()].SetVehicleStateToCmd(vehicle, cmd);
        }

        public static void ApplyVehicleCmdAndState(VehicleEntity vehicle, IVehicleCmd cmd)
        {
            EntityAPI[vehicle.GetTypeValue()].ApplyVehicleCmdAndState(vehicle, cmd);
        }

        public static void SetVehicleSyncLatest(VehicleEntity vehicle, bool isSyncLatest)
        {
            EntityAPI[vehicle.GetTypeValue()].SetVehicleSyncLatest(vehicle, isSyncLatest);
        }
    }

}