using System;
using System.Collections;
using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.GameModules.Player;
using UnityEngine;
using App.Shared.GameModules.Vehicle;
using App.Shared.GameModules.Weapon;
using Core.CameraControl.NewMotor;
using Core.Utils;
using Utils.CharacterState;
using Assets.Utils.Configuration;
using Core.Configuration;
using Core.EntityComponent;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Camera.Utils
{
    public static class CameraUtility
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CameraUtility));
        private static 
            CameraConfigManager _manager = SingletonManager.Get<CameraConfigManager>();
        
        public static float GetPostureTransitionTime(SubCameraMotorType motorType, SubCameraMotorState state)
        {
            var transitionTime = _manager.GetTransitionTime(motorType, state);
            if (transitionTime <= 100) return transitionTime;
            if (transitionTime > 100 && transitionTime <= 200) return 100;
            return transitionTime - 100;
        }

        public static float GetWeaponFov(this PlayerEntity player)
        {
            if(player.weaponLogic.Weapon.IsFovModified())
            {
                return player.weaponLogic.Weapon.GetFov();
            }
            else
            {
                if(player.oxygenEnergyInterface.Oxygen.InShiftState)
                {
                    var weaponId = player.weaponLogicInfo.WeaponId;
                    var weaponCfg = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponId);
                    if(null != weaponCfg)
                    {
                        return weaponCfg.ShiftFov;
                    }
                }
                return player.weaponLogic.Weapon.GetFov();
            }
        }

        public static bool IsCameraCanFire(this PlayerEntity playerEntity)
        {
            return playerEntity.cameraStateNew.CanFire;
        }

        public static FreeMoveEntity GetAirPlane(this PlayerEntity playerEntity, FreeMoveContext freeMoveContext)
        {
           foreach (var freeMoveEntity in freeMoveContext.GetEntities())
           {
               if (freeMoveEntity.hasFreeData && freeMoveEntity.hasPosition &&
                   freeMoveEntity.freeData.Key.Equals("plane"))
               {
                   return freeMoveEntity;
               }
           }
            return null;
        }

        
        public static bool IsCameraGunSight(this PlayerEntity playerEntity)
        {
            return playerEntity.cameraStateNew.ViewNowMode == (short) ECameraViewMode.GunSight;
        }

        public static bool CanWeaponGunSight(this PlayerEntity player)
        {
            return player.hasWeaponComponentAgent && player.GetController<PlayerWeaponController>().CurrSlotWeaponId > 0 &&
                   player.weaponLogic.State.CanCameraFocus();
        }

        public static ECameraArchorType GetCameraArchorType(this PlayerEntity player)
        {
            return player.cameraArchor.ArchorType;
        }
      
    }
}