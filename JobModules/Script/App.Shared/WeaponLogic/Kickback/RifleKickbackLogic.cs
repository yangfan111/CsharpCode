using Core.Utils;
using WeaponConfigNs;
using UnityEngine;
using App.Shared.WeaponLogic;
using App.Shared.GameModules.Camera.Utils;

namespace Core.WeaponLogic.Kickback
{
    public class RifleKickbackLogic : AbstractKickbackLogic<RifleKickbackLogicConfig>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(RifleKickbackLogic));
        public RifleKickbackLogic(Contexts contexts):base(contexts)
        {
        }

        public override void OnAfterFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var config = GetKickbackConfig(playerEntity);
            KickbackGroup kickbackGroup = null;
            var hOffsetFactor = 1f;
            var vOffsetFactor = 1f;
            if(playerEntity.IsAiming())
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
            var posture = playerEntity.stateInterface.State.GetCurrentPostureState();
            if (!playerEntity.playerMove.IsGround)
            {
                kickBackParams = kickbackGroup.Air;
            }
            else if (playerEntity.playerMove.HorizontalVelocity > config.FastMoveSpeed)
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


            BaseKickBack(playerEntity, weaponEntity, cmd.CmdSeq,
                kickBackParams,
                hOffsetFactor,
                vOffsetFactor);
        }

        private void BaseKickBack(PlayerEntity playerEntity, WeaponEntity weaponEntity, int seed, WeaponConfigNs.Kickback kickbackParams,
            float hOffsetFactor, float vOffsetFactor)
 
        {
            var commonFireConfig = GetCommonFireConfig(playerEntity);
            var kickbackConfig = GetKickbackConfig(playerEntity);
            var weaponState = weaponEntity.weaponData;
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

            var punchYaw = playerEntity.orientation.NegPunchYaw;
            var punchPitch = playerEntity.orientation.NegPunchPitch;
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

            if (UniformRandom.RandomInt(seed, 0, (int) kickbackParams.LateralTurnback) == 0)
                weaponState.PunchYawLeftSide = !weaponState.PunchYawLeftSide;

            //if (isMaxUp)
                weaponState.PunchDecayCdTime = (int)(commonFireConfig.AttackInterval * kickbackConfig.DecaytimeFactor);

            playerEntity.orientation.NegPunchYaw = punchYaw;
            playerEntity.orientation.NegPunchPitch = punchPitch;
            playerEntity.orientation.WeaponPunchPitch = punchPitch * vOffsetFactor;
            playerEntity.orientation.WeaponPunchYaw = punchYaw * hOffsetFactor;
            Logger.DebugFormat("yaw src {0} new {1} pithc src {2} new {3}", playerEntity.orientation.NegPunchYaw,
                playerEntity.orientation.WeaponPunchYaw,
                playerEntity.orientation.NegPunchPitch,
                playerEntity.orientation.WeaponPunchPitch);
        }

        public override float UpdateLen(PlayerEntity playerEntity, float len, float frameTime)
        {
            var kickbackConfig = GetKickbackConfig(playerEntity);
            var r = len;
            r -= (kickbackConfig.FixedDecayFactor + r * kickbackConfig.LenDecayFactor) * frameTime;
            r = Mathf.Max(r, 0f);
            return r;
        }

        protected override float GetWeaponPunchYawFactor(PlayerEntity playerEntity)
        {
            var kickbackConfig = GetKickbackConfig(playerEntity);
            if(playerEntity.IsAiming())
            {
                return kickbackConfig.Aiming.WeaponFallbackFactor;
            }
            else
            {
                return kickbackConfig.Default.WeaponFallbackFactor;
            }
        }

        private RifleKickbackLogicConfig GetKickbackConfig(PlayerEntity playerEntity)
        {
            var config = playerEntity.GetWeaponConfig(_contexts);
            if(null != config)
            {
                return config.RifleKickbackLogicCfg;
            }
            return null;
        }

        private CommonFireConfig GetCommonFireConfig(PlayerEntity playerEntity)
        {
            var config = playerEntity.GetWeaponConfig(_contexts);
            if(null != config)
            {
                return config.CommonFireCfg;
            }
            return null;
        }
    }
}