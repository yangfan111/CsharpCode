using System.Collections;
using App.Shared.Components.Player;
using App.Shared.GameModules.Weapon;
using App.Shared.Components.Player;
using App.Shared.GameModules.Weapon;
using Core.CameraControl.NewMotor;
using Core.Utils;
using Core.Configuration;
using Utils.CharacterState;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Camera.Utils
{
    public static class CameraUtility
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CameraUtility));
        public static CameraConfigManager _manager = SingletonManager.Get<CameraConfigManager>();
        
        public static float GetPostureTransitionTime(SubCameraMotorType motorType, SubCameraMotorState state)
        {
            return _manager.GetTransitionTime(motorType, state);
        }

        public static bool IsCameraCanFire(this PlayerEntity playerEntity)
        {
            return playerEntity.cameraStateNew.CanFire;
        }
        
        public static void UpdateCameraArchorPostion(this PlayerEntity player)
        {
            if (player.hasAppearanceInterface && player.appearanceInterface.Appearance.IsFirstPerson)
            {
                var camRoot = player.characterBoneInterface.CharacterBone.GetLocation(SpecialLocation
                    .FirstPersonCamera, CharacterView.FirstPerson);
                if (null != camRoot)
                {
                    Logger.ErrorFormat("TestCamera: firstPos:{0}", camRoot.position);
                    player.cameraArchor.ArchorPosition = camRoot.position;
                }
            }
            else
            {
                player.cameraArchor.ArchorPosition = player.position.Value;
            }
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

        public static bool IsAiming(this PlayerEntity playerEntity)
        {
            if(playerEntity.hasCameraStateNew)
            {
                return playerEntity.cameraStateNew.ViewNowMode == (short) ECameraViewMode.GunSight;
            }
            LogError("playerEntity has no cameraStateNew");
            return false;
        }

        public static ECameraArchorType GetCameraArchorType(this PlayerEntity player)
        {
            return player.cameraArchor.ArchorType;
        }

        private static void LogError(string msg)
        {
            Logger.Error(msg);
            System.Console.WriteLine(msg);
        }
    }
}