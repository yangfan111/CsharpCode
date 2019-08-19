using App.Shared.Components.Weapon;
using Core;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon
{
    public static class FireSpreadFormula
    {
        /// <summary>
        ///     设置同步WeaponRuntimeDataComponent.LastSpreadX.LastSpreadY精准度参数
        /// </summary>
        /// <param name="spreadScaleFactor"> 基于配置和玩家状态读取基础spread值</param>
        /// <param name="locatorDelta"></param>
        /// <param name="spreadScale"> 基于配置的spreadScale值</param>
        /// <param name="runtimeDataComponent">武器运行参数组件</param>
        public static void ApplyRifleFinalSpread(float spreadScaleFactor, SpreadScale spreadScale,
                                                 WeaponRuntimeDataComponent runtimeDataComponent)
        {
            runtimeDataComponent.LastSpreadX = spreadScaleFactor * runtimeDataComponent.Accuracy * spreadScale.ScaleX;
            runtimeDataComponent.LastSpreadY = spreadScaleFactor * runtimeDataComponent.Accuracy * spreadScale.ScaleY;
            // runtimeDataComponent.LastSpreadOffsetX =  locatorDelta.x;
            // runtimeDataComponent.LastSpreadOffsetY = locatorDelta.y  ;
            //   DebugUtil.MyLog(locatorDelta.ToString("f4"));
        }

        public static void ApplyPistolFinalSpread(float spreadScaleFactor, SpreadScale spreadScale,
                                                  WeaponRuntimeDataComponent runtimeDataComponent)
        {
            runtimeDataComponent.LastSpreadX =
                            (1 - runtimeDataComponent.Accuracy) * spreadScaleFactor * spreadScale.ScaleX;
            runtimeDataComponent.LastSpreadY =
                            (1 - runtimeDataComponent.Accuracy) * spreadScaleFactor * spreadScale.ScaleY;
        }

        public static void ApplyFixedFinalSpread(float baseSpread, SpreadScale spreadScale,
                                                 WeaponRuntimeDataComponent runtimeDataComponent)
        {
            runtimeDataComponent.LastSpreadX = baseSpread * spreadScale.ScaleX;
            runtimeDataComponent.LastSpreadY = baseSpread * spreadScale.ScaleY;
        }
    }

    public static class FireSpreadProvider
    {
        ///精准系数*精准值
        public static float GetSpreadScaleFactor(RifleSpreadLogicConfig rifleSpreadLogicConfig,
                                                 WeaponAttackProxy attackProxy)
        {
            Spread spreadAimCfg = attackProxy.IsAiming ? rifleSpreadLogicConfig.Aiming : rifleSpreadLogicConfig.Default;
            var    spreadFactor = GetSpreadFactor(spreadAimCfg, rifleSpreadLogicConfig.FastMoveSpeed, attackProxy);
            return spreadFactor * spreadAimCfg.Base *
                            (1 - attackProxy.GetAttachedAttributeByType(WeaponAttributeType.Spread) / 100);
        }

        public static float GetSpreadScaleFactor(SniperSpreadLogicConfig snipperSpreadLogicConfig,
                                                 WeaponAttackProxy attackProxy)
        {
            float param   = 0f;
            var   posture = attackProxy.CharacterState.GetCurrentPostureState();
            if (!attackProxy.PlayerMove.IsGround)
            {
                param = snipperSpreadLogicConfig.AirParam;
            }
            else if (attackProxy.PlayerMove.HorizontalVelocity > GlobalConst.Length2D1)
            {
                param = snipperSpreadLogicConfig.LengthParam1;
            }
            else if (attackProxy.PlayerMove.HorizontalVelocity > GlobalConst.Length2D2)
            {
                param = snipperSpreadLogicConfig.LengthParam2;
            }
            else if (posture == PostureInConfig.Crouch || posture == PostureInConfig.Prone)
            {
                param = snipperSpreadLogicConfig.DuckParam;
            }
            else
            {
                param = snipperSpreadLogicConfig.DefaultParam;
            }

            if (!attackProxy.IsAiming)
            {
                param += snipperSpreadLogicConfig.FovAddParam;
            }

            return param;
        }

        public static float GetSpreadScaleFactor(PistolSpreadLogicConfig pistolSpreadLogicConfig,
                                                 WeaponAttackProxy attackProxy)
        {
            return GetSpreadFactor(pistolSpreadLogicConfig, attackProxy);
        }

        private static float GetSpreadFactor(PistolSpreadLogicConfig pistolSpreadLogicConfig,
                                             WeaponAttackProxy attackProxy)
        {
            float param   = pistolSpreadLogicConfig.DefaultParam;
            var   posture = attackProxy.CharacterState.GetCurrentPostureState();
            if (!attackProxy.PlayerMove.IsGround)
            {
                param = pistolSpreadLogicConfig.AirParam;
            }
            else if (attackProxy.PlayerMove.HorizontalVelocity > 13)
            {
                param = pistolSpreadLogicConfig.LengthGreater13Param;
            }
            else if (posture == PostureInConfig.Prone || posture == PostureInConfig.Crouch)
            {
                param = pistolSpreadLogicConfig.DuckParam;
            }

            return param;
        }

        private static float GetSpreadFactor(Spread spreadAimCfg, float fastMoveSpeed, WeaponAttackProxy attackProxy)
        {
            //fst layer
            var   posture      = attackProxy.CharacterState.GetCurrentPostureState();
            float spreadFactor = 1f;
            if (!attackProxy.PlayerMove.IsGround)
            {
                spreadFactor = spreadAimCfg.Air;
            }
            else if (attackProxy.PlayerMove.HorizontalVelocity > fastMoveSpeed)
            {
                spreadFactor = spreadAimCfg.FastMove;
            }
            else if (posture == PostureInConfig.Prone)
            {
                spreadFactor = spreadAimCfg.Prone;
            }
            else if (posture == PostureInConfig.Crouch)
            {
                spreadFactor = spreadAimCfg.Duck;
            }

            return spreadFactor;
        }
    }
}