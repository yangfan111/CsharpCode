using System;
using Core.Utils;
using UnityEngine;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="RifleFireShakeProcessor" />
    /// </summary>
    public class RifleFireShakeProcessor : AbstractFireShakeProcessor<RifleShakeConfig>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(RifleFireShakeProcessor));

        public RifleFireShakeProcessor() 
        {
        }

        public override void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var config = controller.HeldWeaponAgent.RifleShakeCfg;
         
            ShakeGroup shakeGroup = null;
            var hOffsetFactor = 1f;
            var vOffsetFactor = 1f;
            if (controller.RelatedCameraSNew.IsAiming())
            {
                shakeGroup = config.Aiming;
                hOffsetFactor = config.Aiming.HPunchOffsetFactor;
                vOffsetFactor = config.Aiming.VPunchOffsetFactor;
                Logger.DebugFormat("animing offset {0}, {1}", hOffsetFactor, vOffsetFactor);
            }
            else
            {
                shakeGroup = config.Default;
                hOffsetFactor = config.Default.HPunchOffsetFactor;
                vOffsetFactor = config.Default.VPunchOffsetFactor;
                Logger.DebugFormat("default offset {0}, {1}", hOffsetFactor, vOffsetFactor);
            }

            WeaponConfigNs.ShakeInfo shakeInfo;
            var posture = controller.RelatedStateInterface.GetCurrentPostureState();
            if (!controller.RelatedPlayerMove.IsGround)
            {
                shakeInfo = shakeGroup.Air;
            }
            else if (controller.RelatedPlayerMove.HorizontalVelocity > config.FastMoveSpeed)
            {
                shakeInfo = shakeGroup.FastMove;
            }
            else if (posture == XmlConfig.PostureInConfig.Prone)
            {
                shakeInfo = shakeGroup.Prone;
            }
            else if (posture == XmlConfig.PostureInConfig.Crouch)
            {
                shakeInfo = shakeGroup.Duck;
            }
            else
            {
                shakeInfo = shakeGroup.Base;
            }
       
            CalcBaseShake(controller, cmd.CmdSeq,
                shakeInfo,
                hOffsetFactor,
                vOffsetFactor);
        }

        private void CalcBaseShake(PlayerWeaponController controller, int seed, WeaponConfigNs.ShakeInfo shakeInfo,
            float hOffsetFactor, float vOffsetFactor)
        {
            var weaponState        = controller.HeldWeaponAgent.RunTimeComponent;
            var heldAgent          = controller.HeldWeaponAgent;
            heldAgent.SyncParts();
            var commonFireConfig   = heldAgent.CommonFireCfg;
            var dirShakeArgs       = FireShakeProvider.GetFireUpDirShakeArgs(heldAgent, shakeInfo);
            float upDirShake       = FireShakeMath.CalcFireDirShake(dirShakeArgs.UpBase, dirShakeArgs.UpModifier,weaponState.ContinuesShootCount);
            float lateralDirShake = FireShakeMath.CalcFireDirShake(dirShakeArgs.LateralBase, dirShakeArgs.LateralModifier,weaponState.ContinuesShootCount);
          

            var punchYaw = controller.RelatedOrient.NegPunchYaw;
            var punchPitch = controller.RelatedOrient.NegPunchPitch;
            var isMaxUp = false;
            punchPitch += upDirShake;
            if (punchPitch > dirShakeArgs.UpMax + commonFireConfig.AttackInterval * 0.01f)
            {
                punchPitch = dirShakeArgs.UpMax;
                isMaxUp = true;
            }

            if (weaponState.PunchYawLeftSide)
            {
                punchYaw += lateralDirShake;
                if (punchYaw > dirShakeArgs.LateralMax)
                    punchYaw = dirShakeArgs.LateralMax;
            }
            else
            {
                punchYaw -= lateralDirShake;
                if (punchYaw < -1 * dirShakeArgs.LateralMax)
                    punchYaw = -1 * dirShakeArgs.LateralMax;
            }

            if (UniformRandom.RandomInt(seed, 0, (int)dirShakeArgs.LateralTurnback) == 0)
                weaponState.PunchYawLeftSide = !weaponState.PunchYawLeftSide;

            //if (isMaxUp)
            weaponState.PunchDecayCdTime = GetDecayCdTime(controller);
            weaponState.PunchPitchSpeed = (punchPitch - controller.RelatedOrient.NegPunchPitch) / weaponState.PunchDecayCdTime;
            weaponState.PunchYawSpeed = (punchYaw - controller.RelatedOrient.NegPunchYaw) / weaponState.PunchDecayCdTime;
        }

        public override float UpdateLen(PlayerWeaponController controller, float len, float frameTime)
        {
            var r = len;
            r -= (controller.HeldWeaponAgent.RifleShakeCfg.FixedDecayFactor + r * controller.HeldWeaponAgent.RifleShakeCfg.LenDecayFactor) * frameTime;
            r = Mathf.Max(r, 0f);
            return r;
        }

        protected override float GetWeaponPunchYawFactor(PlayerWeaponController controller)
        {
            if (controller.RelatedCameraSNew.IsAiming())
            {
                return controller.HeldWeaponAgent.RifleShakeCfg.Aiming.WeaponFallbackFactor;
            }
            else
            {
                return controller.HeldWeaponAgent.RifleShakeCfg.Default.WeaponFallbackFactor;
            }
        }
    }
}
