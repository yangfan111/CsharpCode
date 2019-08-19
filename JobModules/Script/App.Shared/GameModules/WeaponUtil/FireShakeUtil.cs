
using Core.Utils;
using System;
using Assets.Utils.Configuration;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon
{
    public static class FireShakeFormula
    {
        /// <summary>
        ///计算实际ireDirShake增量 例如akm=> 0.224 + 0.04 * 5(max) = 0.244
        /// </summary>
        /// <param name="baseArg">基准值</param>
        /// <param name="shootModifierArg">每次开一枪的修改值</param>
        /// <param name="shootContinueCount">当前连续开枪数</param>
        /// <returns></returns>
        public static float CalcFireDirShakeDelta(float baseArg, float shootModifierArg, int shootContinueCount)
        {
            if (shootContinueCount == 1)
                shootContinueCount = 0;
            return baseArg + shootContinueCount * shootModifierArg;
        }
        
        /// <summary>
        ///垂直震动增量应用于punchPitch
        /// 例如akm=>  basePunchPitch第一枪从0开始，每帧+0.224
        /// maxPunchPitch:30
        /// 
        /// </summary>
        /// <param name="basePunchPitch"></param>
        /// <param name="deltaPunchPitch"></param>
        /// <param name="maxPunchPitch"></param>
        /// <param name="otherAddition"></param>
        /// <returns></returns>
        public static float CalcPunchPitch(float basePunchPitch, float delta, float maxPunchPitch,
                                            float otherAddition)
        {
            basePunchPitch += delta;
            if (basePunchPitch > maxPunchPitch + otherAddition)
            {
                basePunchPitch = maxPunchPitch;
            }

            return basePunchPitch;
        }

        public static float CaclPunchYaw(bool  yawLeftSide, float basePunchYaw, float delta,
                                                float maxPunchYaw)
        {
            if (yawLeftSide)
            {
                basePunchYaw += delta;
                return Math.Min(maxPunchYaw, basePunchYaw);
            }

            basePunchYaw -= delta;
            return Math.Max(-1 * maxPunchYaw, basePunchYaw);
        }

        public static float CalcPitchSpeed(float punchPitch,float negPunchPitch,float cdTime)
        {
            return (punchPitch - negPunchPitch) / cdTime;
        }
        public static float EaseOutCubic(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value + 1) + start;
        }
    }


    public static class FireShakeProvider
    {
        public static LoggerAdapter logger = new LoggerAdapter(typeof(FireShakeProvider));
        
        public static float GetDecayInterval(WeaponAllConfigs configs)
        {
            return configs.S_CommonFireCfg.AttackInterval * configs.SRifleShakeCfg.DecaytimeFactor;
        }

        public static ShakeGroup GetShakeGroup(RifleShakeConfig shakeConfig, PlayerWeaponController controller)
        {
            return controller.RelatedCameraSNew.IsAiming() ? shakeConfig.Aiming : shakeConfig.Default;
        }

        public static ShakeInfo GetShakeInfo(RifleShakeConfig shakeConfig, PlayerWeaponController controller,
                                             ShakeGroup       shakeGroup = null)
        {
            shakeGroup = shakeGroup ?? GetShakeGroup(shakeConfig, controller);
            ShakeInfo shakeInfo = shakeGroup.Base;
            var       posture   = controller.RelatedCharState.GetCurrentPostureState();
            if (!controller.RelatedPlayerMove.IsGround)
            {
                shakeInfo = shakeGroup.Air;
            }
            else if (controller.RelatedPlayerMove.HorizontalVelocity > shakeConfig.FastMoveSpeed)
            {
                shakeInfo = shakeGroup.FastMove;
            }
            else if (posture == PostureInConfig.Prone)
            {
                shakeInfo = shakeGroup.Prone;
            }
            else if (posture == PostureInConfig.Crouch)
            {
                shakeInfo = shakeGroup.Duck;
            }
            return shakeInfo;
        }
       
        
        public static ShakeInfoStruct GetFireUpDirShakeArgs(WeaponAttackProxy attackProxy, ShakeInfo shakeInfo)
        {
            var   newInfo = (ShakeInfoStruct) shakeInfo;

            float factor  = attackProxy.GetAttachedAttributeByType(WeaponAttributeType.UpBase);
            if (factor != 0f) newInfo.UpBase *= factor;
        //    logger.Info("Shoot Shake factor:"+factor);

            factor = attackProxy.GetAttachedAttributeByType(WeaponAttributeType.UpMax);
            if (factor != 0f) newInfo.UpMax *= factor;

            factor = attackProxy.GetAttachedAttributeByType(WeaponAttributeType.UpModifier);
            if (factor != 0f) newInfo.UpModifier *= factor;

             factor = attackProxy.GetAttachedAttributeByType(WeaponAttributeType.LateralBase) ;
            if (factor != 0f) newInfo.LateralBase *= factor;

            factor = attackProxy.GetAttachedAttributeByType(WeaponAttributeType.LateralMax);
            if (factor != 0f) newInfo.LateralMax *= factor;

            factor = attackProxy.GetAttachedAttributeByType(WeaponAttributeType.LateralModifier);
            if (factor != 0f) newInfo.LateralModifier *= factor;

            factor = attackProxy.GetAttachedAttributeByType(WeaponAttributeType.LateralTurnback);
            if (factor != 0f) newInfo.LateralTurnback *= factor;

            return newInfo;
        }
    }
}