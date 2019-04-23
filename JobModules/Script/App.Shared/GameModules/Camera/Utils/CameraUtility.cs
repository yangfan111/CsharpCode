using App.Shared.Components.Player;
using Core.CameraControl.NewMotor;
using Core.Configuration;
using Core.Utils;
using System.Collections.Generic;
using UnityEngine;
using Utils.CharacterState;
using Utils.Configuration;
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

        public static float GetGunSightSpeed(PlayerEntity player, ICameraMotorState state)
        {
            if (state.ViewMode == ECameraViewMode.GunSight)
            {
                var upperRail = player.WeaponController().HeldWeaponAgent.BaseComponent.UpperRail;
                if (upperRail > 0)
                {
                    return SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(upperRail).FovMove;
                }
                return 0.9f;
            }
            return 1f;
        }

        #region PhysicTest
        public const float MinCollisionDistance = 0f;
        public const float DistanceOffset = 0.1f;
        public const float RaycastOffset = 0.05f;
        public static readonly int NonePlayerLayer = ~UnityLayerManager.GetLayerMask(EUnityLayerName.Player);
        private const float RaycastDirFactor = 0.5f;

        public static float SingleRayCast(Vector3 startPostion, Quaternion rotation, Vector3 modelOffset,
            int collisionLayers)
        {
            var endPosition = startPostion + rotation * modelOffset;
            var dist = (endPosition - startPostion).magnitude;
            var dir = (endPosition - startPostion).normalized;
            RaycastHit lViewHit;

            Debug.DrawLine(startPostion, endPosition, Color.red);
            if (Physics.Raycast(startPostion, dir, out lViewHit, dist, collisionLayers))
            {
                var hitDistance = lViewHit.distance;
                var colDis = Mathf.Max(hitDistance, MinCollisionDistance) - DistanceOffset - RaycastOffset;
                if (colDis < dist)
                    return colDis;
            }

            return modelOffset.magnitude;
        }

        public static float ScatterCast(Vector3 startPosition, Quaternion rotation, float distance,
            List<Vector3> points, int collisionLayers, float raycastDirFactor = RaycastDirFactor)                     //parameters?
        {
            float actualDistance = distance;
            Vector3 lNewDirection = -rotation.Forward();
            var rayCastStart = startPosition;
            var castDir = lNewDirection;
            
            for (int i = 0; i < points.Count; i++)
            {
                RaycastHit lViewHit;
                var samplingPoint = points[i];
                
                var xdelta = rotation.Right() * samplingPoint.x;
                var ydelta = rotation.Up() * samplingPoint.y;
                var zdelta = rotation.Forward() * samplingPoint.z;
                
                rayCastStart += xdelta;
                rayCastStart += ydelta;
                rayCastStart += zdelta;
                
                castDir += xdelta * raycastDirFactor;
                castDir += ydelta * raycastDirFactor;
                castDir += zdelta * raycastDirFactor;

                if (Physics.Raycast(rayCastStart, castDir, out lViewHit, distance, collisionLayers))
                {
                    var hitDistance = lViewHit.distance;
                    var colDis = Mathf.Max(hitDistance, MinCollisionDistance) - DistanceOffset;
                    if (colDis < actualDistance)
                    {
                        actualDistance = colDis;
                    }
                }
            }
            return actualDistance;
        }
        
        #endregion

        #region TestByCalcu

        public static bool SphereContains(Vector3 spherePos, float radius, Vector3 point)
        {
            float dist = (spherePos - point).sqrMagnitude;
            return dist <= radius * radius;
        }
        
        public static bool CapsuleContains(Vector3 capsuleP1,Vector3 capsuleP2,float radius,Vector3 point) {
            if (SphereContains(capsuleP1, radius, point) || SphereContains(capsuleP2, radius, point))
            {
                return true;
            }

            Vector3 pDir = point - capsuleP1;
            var direction = capsuleP2 - capsuleP1;
            float dot = Vector3.Dot(direction, pDir);
            float lengthsq = direction.sqrMagnitude;
 
            if (dot < 0f || dot > lengthsq) return false;
 
            float dsq = pDir.x * pDir.x + pDir.y * pDir.y + pDir.z * pDir.z - dot * dot / lengthsq;
 
            if (dsq > radius * radius) {
                return false;
            }
            else {
                return true;
            }
        }

        #endregion
    }
}