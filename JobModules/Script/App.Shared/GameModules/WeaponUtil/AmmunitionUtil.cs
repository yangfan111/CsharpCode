using System;
using Core;
using Core.Attack;
using Core.Components;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    ///     Defines the <see cref="IAmmunitionCalculator" />
    /// </summary>
    public interface IAmmunitionCalculator
    {
        /// <summary>
        ///     视线方向的射出位置
        /// </summary>
        /// <returns></returns>
        Vector3 GetFireViewPosition(PlayerWeaponController controller);

        /// <summary>
        ///     枪口/手方向的射出位置
        /// </summary>
        /// <returns></returns>
        Vector3 GetFireEmitPosition(PlayerWeaponController controller);

        /// <summary>
        ///     射出方向
        /// </summary>
        /// <returns></returns>
        Vector3 GetFireDir(int seed, PlayerWeaponController controller, int userCmdSeq);
    }

    /// <summary>
    ///     Defines the <see cref="AmmunitionDirUtility" />
    /// </summary>
    public class ThrowAmmunitionCalculator : Singleton<ThrowAmmunitionCalculator>,IAmmunitionCalculator
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ThrowAmmunitionCalculator));
        /// <summary>
        /// 预估位置
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public Vector3 GetFireViewPosition(PlayerWeaponController controller)
        {
            go.transform.rotation = Quaternion.Euler(controller.RelatedCameraFinal.EulerAngle);
            go.transform.position = controller.RelatedCameraFinal.Position;
            Vector3 rc;
            if (controller.RelatedCameraSNew.IsAiming())
            {
                rc = go.transform.TransformPoint(GlobalConst.ThrowingEmittorFirstOffset);
            }
            else
            {
                rc = go.transform.TransformPoint(GlobalConst.ThrowingEmittorThirdOffset);
            }

            return rc;
        }
        private static GameObject _go;
        private static GameObject go
        {
            get
            {
                if (_go == null)
                {
                    _go      = new GameObject();
                    _go.name = "GoTransmitUtil";
                }

                return _go;
            }
        }
        /// <summary>
        /// 真实投掷位置
        /// 从手上投掷点做射线检测，检测不到阻挡时以正常方式扔出
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public Vector3 GetFireEmitPosition(PlayerWeaponController controller)
        {
            var handPos = controller.RelatedThrowAction.throwBackupPos;
            var vieDir = GetFireDir(0, controller, 0);
            var tarPos = handPos + vieDir * 20 * 0.2f;
            RaycastHit hitInfo;
            if (Physics.Raycast(handPos, vieDir, out hitInfo, Vector3.Distance(tarPos,handPos),  UnityLayers.PickupObstacleLayerMask))
            {
                return controller.RelatedThrowAction.throwBackupPos;
            }

            return  GetFireViewPosition(controller);;
        }

        public Vector3 GetFireDir(int seed, PlayerWeaponController controller, int userCmdSeq)
        {
            var orientation = controller.RelatedOrientation;
            var yaw         = orientation.Yaw - orientation.AccPunchPitchValue * 2;
            var pitch       = orientation.Pitch - orientation.AccPunchYawValue * 2;

            Quaternion q       = Quaternion.Euler(pitch, yaw, 0);
            Vector3    forward = q.Forward();

            forward.y += 0.2f;
            return forward.normalized;
        }
    }

    /// <summary>
    ///     Defines the <see cref="NormalFireAmmunitionCalculator" />
    /// </summary>
    public class NormalFireAmmunitionCalculator : IAmmunitionCalculator
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(NormalFireAmmunitionCalculator));

        public static Vector3 BulletEmittorOffset = new Vector3(0, 0, 0.1f);

        public virtual Vector3 GetFireDir(int seed, PlayerWeaponController controller, int userCmdSeq)
        {
            var runTimeComponent = controller.HeldWeaponAgent.RunTimeComponent;
            var orientation      = controller.RelatedOrientation;
            var yaw              = orientation.YawRound - orientation.AccPunchYawValue * 2;
            var pitch            = orientation.PitchRound - orientation.AccPunchPitchValue * 2;
            var dir = CalculateShotingDir(seed, yaw, pitch, runTimeComponent.LastSpreadX,
                runTimeComponent.LastSpreadY);
            //            DebugUtil.AppendShootText(userCmdSeq,"Yaw:{0},PunchYaw:{1:F5},pitch:{2:F5},AccPunchPitch:{3:F5},LastSpreadX:{4:F5},LastSpreadY:{5:F5}",yaw,orientation.PunchYaw,orientation.Pitch,orientation.AccPunchPitch,runTimeComponent.LastSpreadX,runTimeComponent.LastSpreadY );
            return dir.normalized;
        }

        public virtual Vector3 GetFireEmitPosition(PlayerWeaponController controller)
        {
            var firePosition = controller.RelatedFirePos;
            if (firePosition == null)
                return Vector3.zero;

            if (firePosition.MuzzleP3Valid)
            {
                return firePosition.MuzzleP3Position.ShiftedVector3();
            }

            return GetFireViewPosition(controller);
        }

        public virtual Vector3 GetFireViewPosition(PlayerWeaponController controller)
        {
            var outputView = controller.RelatedCameraFinal;
            if (outputView == null)
            {
                return Vector3.zero;
            }

            //  var cameraAngle = outputView.EulerAngle;
            //var matrix = Matrix4x4.TRS(BulletEmittorOffset, Quaternion.Euler(cameraAngle), Vector3.one);
            //var offset = matrix.ExtractPosition();
            return outputView.PlayerFocusPosition;
            // var runtimeComp = controller.HeldWeaponAgent.RunTimeComponent;
            // return new Vector3(outputView.PlayerFocusPosition.x+runtimeComp.LastSpreadOffsetX,outputView.PlayerFocusPosition.x+runtimeComp.LastSpreadOffsetX,outputView.PlayerFocusPosition.z); 
            //return offset + outputView.Position;
        }

        protected Vector3 CalculateShotingDir(int seed, float yaw, float pitch, float spreadX, float spreadY)
        {
            Quaternion q       = Quaternion.Euler(pitch, yaw, 0);
            Vector3    forward = q.Forward();
            Vector3    right   = q.Right();
            Vector3    up      = q.Up();

            float x;
            float y;
            x = (float) UniformRandom.RandomFloat(seed + 0, -0.5, 0.5) +
                            (float) UniformRandom.RandomFloat(seed + 1, -0.5, 0.5);
            y = (float) UniformRandom.RandomFloat(seed + 2, -0.5, 0.5) +
                            (float) UniformRandom.RandomFloat(seed + 3, -0.5, 0.5);
            float res1 = spreadX * x;
            float res2 = spreadY * y;
            right = Vector3Ext.Scale(right, res1);
            up    = Vector3Ext.Scale(up, res2);
            var newForward = Vector3Ext.Add(forward, right);
            newForward = Vector3Ext.Add(newForward, up);
            return newForward;
        }
    }

    public class SightFireAmmunitionCalculator : NormalFireAmmunitionCalculator
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SightFireAmmunitionCalculator));


        public override Vector3 GetFireDir(int seed, PlayerWeaponController controller, int userCmdSeq)
        {
            var delta      = GetFireViewPosition(controller) - controller.RelatedCameraFinal.Position;
            var runTimeComponent = controller.HeldWeaponAgent.RunTimeComponent;
   //         DebugUtil.AppendShootText(userCmdSeq, "ViewPos1:{0},ViewPos2:{1} Sx:{2} Sy:{3}",
                // GetFireViewPosition(controller), controller.RelatedCameraFinal.Position, runTimeComponent.LastSpreadX,
                // runTimeComponent.LastSpreadY);
            return CalculateShotingDir(seed, delta.GetYaw(), delta.GetPitch(), runTimeComponent.LastSpreadX,
                runTimeComponent.LastSpreadY);
        }

        public override Vector3 GetFireViewPosition(PlayerWeaponController controller)
        {
            if (controller.RelatedFirePos.SightValid) 
            {
                var firePos = controller.RelatedFirePos.SightPosition.ShiftedVector3();
                var clientUpdateComponent = controller.RelatedClientUpdate;
                return new Vector3(firePos.x+clientUpdateComponent.LastSpreadOffsetX,firePos.y+clientUpdateComponent.LastSpreadOffsetY,firePos.z);
                // DebugUtil.MyLog("{0} {1}",clientUpdateComponent.LastSpreadOffsetX.ToString("f4"),clientUpdateComponent.LastSpreadOffsetY.ToString("f4"));
            }
              //  return controller.RelatedFirePos.SightPosition.ShiftedVector3();
            return base.GetFireViewPosition(controller);
        }
    }
} 