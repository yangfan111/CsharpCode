using Core.Utils;
using WeaponConfigNs;
using Core.WeaponLogic.Attachment;
using UnityEngine;

namespace Core.WeaponLogic.Kickback
{
    public class RifleKickbackLogic : AbstractKickbackLogic<RifleKickbackLogicConfig, RifleKickbackModifierArg>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(RifleKickbackLogic));
        private CommonFireConfig _common;
        public RifleKickbackLogic(RifleKickbackLogicConfig config, CommonFireConfig common):base(config)
        {
            _common = common;
        }

        private void ModifyKickbackGroup(KickbackGroup kickbackGroup, KickbackGroup baseKickbackGroup, RifleKickbackModifierArg arg)
        {
            ModifyKickback(kickbackGroup.Air, baseKickbackGroup.Air, arg); 
            ModifyKickback(kickbackGroup.Base, baseKickbackGroup.Base, arg); 
            ModifyKickback(kickbackGroup.Duck, baseKickbackGroup.Duck, arg); 
            ModifyKickback(kickbackGroup.FastMove, baseKickbackGroup.FastMove, arg); 
            ModifyKickback(kickbackGroup.Prone, baseKickbackGroup.Prone, arg); 
        }

        private void ModifyKickback(WeaponConfigNs.Kickback kickback, WeaponConfigNs.Kickback baseKickback, RifleKickbackModifierArg arg)
        {
            kickback.LateralBase = baseKickback.LateralBase * (arg.BasicWidth > 0 ? arg.BasicWidth : 1);
            kickback.LateralMax = baseKickback.LateralMax * (arg.MaxWidth > 0 ? arg.MaxWidth : 1);
            kickback.LateralModifier = baseKickback.LateralModifier * (arg.ContinusWidth > 0 ? arg.ContinusWidth : 1); 
            kickback.UpBase = baseKickback.UpBase * (arg.BasicHeight > 0 ? arg.BasicHeight : 1);
            kickback.UpMax = baseKickback.UpMax * (arg.MaxHeight > 0 ? arg.MaxHeight : 1);
            kickback.UpModifier = baseKickback.UpModifier * (arg.ContinusHeight > 0 ? arg.ContinusHeight : 1);
            kickback.LateralTurnback = baseKickback.LateralTurnback * (arg.Turnback > 0 ? arg.Turnback : 1);
        }

        public override void Apply(RifleKickbackLogicConfig config, RifleKickbackLogicConfig output, RifleKickbackModifierArg arg)
        {
           
            ModifyKickbackGroup(output.Aiming, config.Aiming, arg);
            ModifyKickbackGroup(output.Default, config.Default, arg);
        }
        
        public override void AfterFireBullet(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bullet)
        {

            KickbackGroup kickbackGroup = null;
            var hOffsetFactor = 1f;
            var vOffsetFactor = 1f;
            if (playerWeapon.IsAiming)
            {
                kickbackGroup = _config.Aiming;
                hOffsetFactor = _config.Aiming.HPunchOffsetFactor;
                vOffsetFactor = _config.Aiming.VPunchOffsetFactor;
                Logger.DebugFormat("animing offset {0}, {1}", hOffsetFactor, vOffsetFactor);
            }
            else
            {
                kickbackGroup = _config.Default;
                hOffsetFactor = _config.Default.HPunchOffsetFactor;
                vOffsetFactor = _config.Default.VPunchOffsetFactor;
                Logger.DebugFormat("default offset {0}, {1}", hOffsetFactor, vOffsetFactor);
            }

            WeaponConfigNs.Kickback kickBackParams;
            if (playerWeapon.IsAir)
                kickBackParams = kickbackGroup.Air;
            else if (playerWeapon.HorizontalVeocity > _config.FastMoveSpeed)
                kickBackParams = kickbackGroup.FastMove;
            else if (playerWeapon.IsProne)
                kickBackParams = kickbackGroup.Prone;
            else if (playerWeapon.IsDuckOrProneState)
                kickBackParams = kickbackGroup.Duck;
            else
                kickBackParams = kickbackGroup.Base;



            BaseKickBack(playerWeapon, cmd.CmdSeq + bullet,
                kickBackParams.UpBase,
                kickBackParams.LateralBase,
                kickBackParams.UpModifier,
                kickBackParams.LateralModifier,
                kickBackParams.UpMax,
                kickBackParams.LateralMax,
                kickBackParams.LateralTurnback,
                hOffsetFactor,
                vOffsetFactor);
        }

        private void BaseKickBack(IPlayerWeaponState playerWeapon, int seed, float upBase, float lateralBase,
            float upModifier, float lateralModifier, float upMax, float lateralMax, float directionChang, 
            float hOffsetFactor, float vOffsetFactor)
        {
            float kickUp;
            float kickLateral;
            if (playerWeapon.ContinuesShootCount == 1)
            {
                kickUp = upBase;
                kickLateral = lateralBase;
            }
            else
            {
                kickUp = upBase + playerWeapon.ContinuesShootCount * upModifier;
                kickLateral = lateralBase + playerWeapon.ContinuesShootCount * lateralModifier;
            }

            var punchYaw = playerWeapon.NegPunchYaw;
            var punchPitch = playerWeapon.NegPunchPitch;
            var isMaxUp = false;
            punchPitch += kickUp;
            if (punchPitch > upMax + _common.AttackInterval * 0.01f)
            {
                punchPitch = upMax;
                isMaxUp = true;
            }

            if (playerWeapon.PunchYawLeftSide)
            {
                punchYaw += kickLateral;
                if (punchYaw > lateralMax)
                    punchYaw = lateralMax;
            }
            else
            {
                punchYaw -= kickLateral;
                if (punchYaw < -1 * lateralMax)
                    punchYaw = -1 * lateralMax;
            }

            if (UniformRandom.RandomInt(seed, 0, (int) directionChang) == 0)
                playerWeapon.PunchYawLeftSide = !playerWeapon.PunchYawLeftSide;

            //if (isMaxUp)
                playerWeapon.PunchDecayCdTime = (int)(_common.AttackInterval * _config.DecaytimeFactor);

            playerWeapon.NegPunchYaw = punchYaw;
            playerWeapon.NegPunchPitch = punchPitch;
            playerWeapon.WeaponPunchPitch = punchPitch * vOffsetFactor;
            playerWeapon.WeaponPunchYaw = punchYaw * hOffsetFactor;
            Logger.DebugFormat("yaw src {0} new {1} pithc src {2} new {3}", playerWeapon.NegPunchYaw,
                playerWeapon.WeaponPunchYaw,
                playerWeapon.NegPunchPitch,
                playerWeapon.WeaponPunchPitch);
        }

        public override float UpdateLen(float len, float frameTime)
        {
            var r = len;
            r -= (_config.FixedDecayFactor + r * _config.LenDecayFactor) * frameTime;
            r = Mathf.Max(r, 0f);
            return r;
        }

        protected override float GetWeaponPunchYawFactor(IPlayerWeaponState playerWeapon)
        {
            if(playerWeapon.IsAiming)
            {
                return _config.Aiming.WeaponFallbackFactor;
            }
            else
            {
                return _config.Default.WeaponFallbackFactor;
            }
        }

        public override void SetVisualConfig(ref VisualConfigGroup config)
        {
            config.KickBack = _config;
        }
    }
}