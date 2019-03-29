using Core.Utils;
using UnityEngine;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="RifleKickbackLogic" />
    /// </summary>
    public class RifleKickbackLogic : AbstractKickbackLogic<RifleKickbackLogicConfig>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(RifleKickbackLogic));

        public RifleKickbackLogic() 
        {
        }

        public override void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var config = controller.HeldWeaponAgent.RifleKickbackLogicCfg;
            KickbackGroup kickbackGroup = null;
            var hOffsetFactor = 1f;
            var vOffsetFactor = 1f;
            if (controller.RelatedCameraSNew.IsAiming())
            {
                kickbackGroup = config.Aiming;
                hOffsetFactor = config.Aiming.HPunchOffsetFactor;
                vOffsetFactor = config.Aiming.VPunchOffsetFactor;
                Logger.DebugFormat("animing offset {0}, {1}", hOffsetFactor, vOffsetFactor);
            }
            else
            {
                kickbackGroup = config.Default;
                hOffsetFactor = config.Default.HPunchOffsetFactor;
                vOffsetFactor = config.Default.VPunchOffsetFactor;
                Logger.DebugFormat("default offset {0}, {1}", hOffsetFactor, vOffsetFactor);
            }

            WeaponConfigNs.Kickback kickBackParams;
            var posture = controller.RelatedStateInterface.GetCurrentPostureState();
            if (!controller.RelatedPlayerMove.IsGround)
            {
                kickBackParams = kickbackGroup.Air;
            }
            else if (controller.RelatedPlayerMove.HorizontalVelocity > config.FastMoveSpeed)
            {
                kickBackParams = kickbackGroup.FastMove;
            }
            else if (posture == XmlConfig.PostureInConfig.Prone)
            {
                kickBackParams = kickbackGroup.Prone;
            }
            else if (posture == XmlConfig.PostureInConfig.Crouch)
            {
                kickBackParams = kickbackGroup.Duck;
            }
            else
            {
                kickBackParams = kickbackGroup.Base;
            }

            BaseKickBack(controller, cmd.CmdSeq,
                kickBackParams,
                hOffsetFactor,
                vOffsetFactor);
        }

        private void BaseKickBack(PlayerWeaponController controller, int seed, WeaponConfigNs.Kickback kickbackParams,
            float hOffsetFactor, float vOffsetFactor)
        {
            var commonFireConfig = controller.HeldWeaponAgent.CommonFireCfg;
            var kickbackConfig = controller.HeldWeaponAgent.RifleKickbackLogicCfg;
            var weaponState = controller.HeldWeaponAgent.RunTimeComponent;
            float kickUp;
            float kickLateral;
            if (weaponState.ContinuesShootCount == 1)
            {
                kickUp = kickbackParams.UpBase;
                kickLateral = kickbackParams.LateralBase;
            }
            else
            {
                kickUp = kickbackParams.UpBase + weaponState.ContinuesShootCount * kickbackParams.UpModifier;
                kickLateral = kickbackParams.LateralBase + weaponState.ContinuesShootCount * kickbackParams.LateralModifier;
            }

            var punchYaw = controller.RelatedOrient.NegPunchYaw;
            var punchPitch = controller.RelatedOrient.NegPunchPitch;
            var isMaxUp = false;
            punchPitch += kickUp;
            if (punchPitch > kickbackParams.UpMax + commonFireConfig.AttackInterval * 0.01f)
            {
                punchPitch = kickbackParams.UpMax;
                isMaxUp = true;
            }

            if (weaponState.PunchYawLeftSide)
            {
                punchYaw += kickLateral;
                if (punchYaw > kickbackParams.LateralMax)
                    punchYaw = kickbackParams.LateralMax;
            }
            else
            {
                punchYaw -= kickLateral;
                if (punchYaw < -1 * kickbackParams.LateralMax)
                    punchYaw = -1 * kickbackParams.LateralMax;
            }

            if (UniformRandom.RandomInt(seed, 0, (int)kickbackParams.LateralTurnback) == 0)
                weaponState.PunchYawLeftSide = !weaponState.PunchYawLeftSide;

            //if (isMaxUp)
            weaponState.PunchDecayCdTime = GetDecayCdTime(controller);
            weaponState.PunchPitchSpeed = (punchPitch - controller.RelatedOrient.NegPunchPitch) / weaponState.PunchDecayCdTime;
            weaponState.PunchYawSpeed = (punchYaw - controller.RelatedOrient.NegPunchYaw) / weaponState.PunchDecayCdTime;
        }

        public override float UpdateLen(PlayerWeaponController controller, float len, float frameTime)
        {
            var r = len;
            r -= (controller.HeldWeaponAgent.RifleKickbackLogicCfg.FixedDecayFactor + r * controller.HeldWeaponAgent.RifleKickbackLogicCfg.LenDecayFactor) * frameTime;
            r = Mathf.Max(r, 0f);
            return r;
        }

        protected override float GetWeaponPunchYawFactor(PlayerWeaponController controller)
        {
            if (controller.RelatedCameraSNew.IsAiming())
            {
                return controller.HeldWeaponAgent.RifleKickbackLogicCfg.Aiming.WeaponFallbackFactor;
            }
            else
            {
                return controller.HeldWeaponAgent.RifleKickbackLogicCfg.Default.WeaponFallbackFactor;
            }
        }
    }
}
