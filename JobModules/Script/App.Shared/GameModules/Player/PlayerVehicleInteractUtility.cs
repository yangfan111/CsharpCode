using App.Shared.Components.Player;
using App.Shared.Configuration;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.Vehicle;
using App.Shared.GameModules.Weapon;
using App.Shared.Player;
using App.Shared.Util;
using Core.Utils;
using UnityEngine;
using Utils.Appearance;
using Utils.Singleton;
using VehicleCommon;

namespace App.Shared.GameModules.Player
{
    public static class PlayerVehicleInteractUtility
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerVehicleInteractUtility));

        public static bool IsOnVehicle(this PlayerEntity playerEntity)
        {
            return playerEntity.hasControlledVehicle && playerEntity.controlledVehicle.IsOnVehicle;
        }

        public static void SetCharacterStateWithVehicle(this PlayerEntity playerEntity, Contexts contexts, VehicleContext context)
        {
            if(playerEntity.hasCharacterContoller)
            {
                bool enabled = !playerEntity.IsOnVehicle();
                var controller = playerEntity.characterContoller.Value;

                if (controller.enabled != enabled)
                {
                    bool success = SetCharacterPositionOnVehicle(playerEntity, context);

                    if (success)
                    {
                        if (enabled && playerEntity.gamePlay.IsLifeState(EPlayerLifeState.Dead))
                        {
                            controller.enabled = false;
                        }
                        else
                        {
                            controller.enabled = enabled;
                        }

                        if (playerEntity.hasStateInterface)
                        {
                            bool isStateDrive = playerEntity.stateInterface.State.GetActionKeepState() ==
                                                XmlConfig.ActionKeepInConfig.Drive;
                            if (!enabled && !isStateDrive)
                            {
                                var vehicle = context.GetEntityWithEntityKey(playerEntity.controlledVehicle.EntityKey);
                                playerEntity.DriveStart(contexts, playerEntity.controlledVehicle.Role, vehicle.vehicleAssetInfo.PostureId);

                            }
                            else if (enabled && isStateDrive)
                            {
                                playerEntity.stateInterface.State.DriveEnd();
                            }
                        }
 
                        // 上车并且是主驾驶位，设置IK
                        if (playerEntity.IsVehicleDriver())
                        {
                            var vehicle = context.GetEntityWithEntityKey(playerEntity.controlledVehicle.EntityKey);
                            playerEntity.SetSteeringWheelIK(vehicle);
                        }
                        else
                        {
                            playerEntity.EndSteeringWheelIK();
                        }
                    }
                }
            }
        }

        public static Transform GetVehicleSeatTransform(this PlayerEntity player, VehicleContext context)
        {
            if (player.IsOnVehicle())
            {
                var controlledEntity = player.controlledVehicle;
                var vehicle = context.GetEntityWithEntityKey(controlledEntity.EntityKey);
                if (vehicle != null)
                {
                    return vehicle.GetVehicleSeatTransform(player);
                }
            }
            return null;
        }

        private static BoneMount _mount = new BoneMount();
        private static bool SetCharacterPositionOnVehicle(this PlayerEntity playerEntity, VehicleContext context)
        {
            var characterTransform = playerEntity.RootGo().transform;
            var character = playerEntity.RootGo();
            if (null == characterTransform) return false;
            bool on = playerEntity.IsOnVehicle();
            if (on)
            {
                var seat = playerEntity.GetVehicleSeatTransform(context);
                if (seat != null)
                {
                    _mount.MountCharacterToVehicleSeat(character, seat);
                    playerEntity.orientation.Pitch = 0;
                    playerEntity.orientation.Yaw = 0;

                    EnablePassagerCollider(playerEntity, context);
                    return true;
                }

                return false;
            }

            EnablePassagerCollider(playerEntity, context);
            characterTransform.SetParent(null, false);
            return true;
        }

        private static void EnablePassagerCollider(PlayerEntity player, VehicleContext context)
        {

            var vehicle = GetVehicleByHierachy(player, context);
            if (vehicle != null)
            {
                vehicle.EnablePassagerCollider(player);
            }
        }

        private static VehicleEntity GetVehicleByHierachy(PlayerEntity player, VehicleContext context)
        {
            VehicleEntity vehicle = null;
            if (player.IsOnVehicle())
            {
                var controlledEntity = player.controlledVehicle;
                vehicle = context.GetEntityWithEntityKey(controlledEntity.EntityKey);
            }
            else
            {
                var transform = player.RootGo().transform.parent;
                while(transform != null)
                {
                    if (transform.GetComponent<VehicleCommonController>() != null)
                    {
                        var reference = transform.GetComponent<EntityReference>();
                        if (reference != null)
                        {
                            vehicle = (VehicleEntity) reference.Reference;
                        }
                        break;
                    }

                    transform = transform.parent;
                }
            }

            return vehicle;
        }


        public static void SetSteeringWheelIK(this PlayerEntity playerEntity, VehicleEntity vehicleEntity)
        {
            if (vehicleEntity != null && vehicleEntity.hasGameObject)
            {
                var thirdPersonObj = playerEntity.thirdPersonModel.Value;
                var ikControllerP3 = thirdPersonObj.GetComponent<PlayerIK>();

                ikControllerP3.ClearAllIKTarget();
                ikControllerP3.SetIKGoal(AvatarIKGoal.LeftHand);
                ikControllerP3.SetIKGoal(AvatarIKGoal.RightHand);

                var vehicleObj = vehicleEntity.gameObject.UnityObject;
                var leftIKP3 = BoneMount.FindChildBone(vehicleObj, BoneName.SteeringWheelLeftIK, true);
                ikControllerP3.SetSource(AvatarIKGoal.LeftHand, leftIKP3);
                var rightIKP3 = BoneMount.FindChildBone(vehicleObj, BoneName.SteeringWheelRightIK, true);
                ikControllerP3.SetSource(AvatarIKGoal.RightHand, rightIKP3);
                ikControllerP3.SetIKActive(true);

                playerEntity.characterBoneInterface.CharacterBone.IsIKActive = true;
                //playerEntity.appearanceInterface.Appearance.IsIKActive = true;
            }
            
        }

        public static void EndSteeringWheelIK(this PlayerEntity playerEntity)
        {
            var thirdPersonObj = playerEntity.thirdPersonModel.Value;
            var ikControllerP3 = thirdPersonObj.GetComponent<PlayerIK>();

            ikControllerP3.SetIKActive(false);

            playerEntity.characterBoneInterface.CharacterBone.IsIKActive = false;
            //playerEntity.appearanceInterface.Appearance.IsIKActive = false;
        }

        public static bool IsVehicleEnterable(this PlayerEntity playerEntity, VehicleEntity vehicle)
        {
            if (vehicle.vehicleBrokenFlag.IsBodyBroken())
            {
                Logger.Debug("body broken");
                return false;
            }

            if (vehicle.IsCar())
            {
                if(vehicle.IsInWater())
                {
                    Logger.Debug("car in water");
                    return false;
                }
                if (!playerEntity.playerMove.IsGround)
                {
                    Logger.Debug("do not on ground");
                    return false;
                }
              
            }
            else if(vehicle.IsShip())
            {
                if (!SingletonManager.Get<MapConfigManager>().InWater(playerEntity.position.Value) && !playerEntity.playerMove.IsGround)
                {
                    Logger.Debug("do not on ground and do not in water and vehicle is ship");
                    return false;
                }
            }
            return true;
        }

        public static void DriveStart(this PlayerEntity playerEntity, Contexts contexts, int seatId, int postureId)
        {
            //seatId 1~4,  actionSeatId 0~3
            var actionSeatId = seatId - 1 < 0 ? 0 : seatId - 1;
            // 打断投掷
            playerEntity.stateInterface.State.ForceFinishGrenadeThrow();
            playerEntity.stateInterface.State.DriveStart(actionSeatId, postureId);
            //if (playerEntity.IsVehicleDriver())    //主驾驶位置
            {
                playerEntity.WeaponController().ForceUnArmHeldWeapon();
            }
        }
    }
}
