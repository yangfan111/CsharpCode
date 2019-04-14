using App.Shared.Components.Weapon;
using Core;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    public static class FireSpreadFormula
    {
        /// <summary>
        /// 设置同步WeaponRuntimeDataComponent.LastSpreadX.LastSpreadY精准度参数
        /// </summary>
        /// <param name="spreadScaleFactor"> 基于配置和玩家状态读取基础spread值</param>
        /// <param name="spreadScale"> 基于配置的spreadScale值</param>
        /// <param name="runtimeDataComponent">武器运行参数组件</param>
        public static void ApplyRifleFinalSpread(float                      spreadScaleFactor, SpreadScale spreadScale,
                                                 WeaponRuntimeDataComponent runtimeDataComponent)
        {
            runtimeDataComponent.LastSpreadX = spreadScaleFactor * runtimeDataComponent.Accuracy * spreadScale.ScaleX;
            runtimeDataComponent.LastSpreadY = spreadScaleFactor * runtimeDataComponent.Accuracy * spreadScale.ScaleY;
        }

        public static void ApplyPistolFinalSpread(float                      spreadScaleFactor, SpreadScale spreadScale,
                                                  WeaponRuntimeDataComponent runtimeDataComponent)
        {
            runtimeDataComponent.LastSpreadX =
                (1 - runtimeDataComponent.Accuracy) * spreadScaleFactor * spreadScale.ScaleX;
            runtimeDataComponent.LastSpreadY =
                (1 - runtimeDataComponent.Accuracy) * spreadScaleFactor * spreadScale.ScaleY;
        }

        public static void ApplyFixedFinalSpread(float                      baseSpread, SpreadScale spreadScale,
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
                                                 PlayerWeaponController controller)
        {
            Spread spreadAimCfg = controller.RelatedCameraSNew.IsAiming()
                ? rifleSpreadLogicConfig.Aiming
                : rifleSpreadLogicConfig.Default;
            var spreadFactor = GeSpreadFactor(spreadAimCfg, rifleSpreadLogicConfig.FastMoveSpeed, controller);
            return spreadFactor * spreadAimCfg.Base;
        }

        public static float GetSpreadScaleFactor(SniperSpreadLogicConfig snipperSpreadLogicConfig,
                                                 PlayerWeaponController  controller)
        {
            float param   = 0f;
            var   posture = controller.RelatedCharState.GetCurrentPostureState();
            if (!controller.RelatedPlayerMove.IsGround)
            {
                param = snipperSpreadLogicConfig.AirParam;
            }
            else if (controller.RelatedPlayerMove.HorizontalVelocity > GlobalConst.Length2D1)
            {
                param = snipperSpreadLogicConfig.LengthParam1;
            }
            else if (controller.RelatedPlayerMove.HorizontalVelocity > GlobalConst.Length2D2)
            {
                param = snipperSpreadLogicConfig.LengthParam2;
            }
            else if (posture == XmlConfig.PostureInConfig.Crouch || posture == XmlConfig.PostureInConfig.Prone)
            {
                param = snipperSpreadLogicConfig.DuckParam;
            }
            else
            {
                param = snipperSpreadLogicConfig.DefaultParam;
            }

            if (!controller.RelatedCameraSNew.IsAiming())
            {
                param += snipperSpreadLogicConfig.FovAddParam;
            }

            return param;
        }

        public static float GetSpreadScaleFactor(PistolSpreadLogicConfig pistolSpreadLogicConfig,
                                                 PlayerWeaponController  controller)
        {
            return GeSpreadFactor(pistolSpreadLogicConfig, controller);
        }

        private static float GeSpreadFactor(PistolSpreadLogicConfig pistolSpreadLogicConfig,
                                            PlayerWeaponController  controller)
        {
            float param   = pistolSpreadLogicConfig.DefaultParam;
            var   posture = controller.RelatedCharState.GetCurrentPostureState();
            if (!controller.RelatedPlayerMove.IsGround)
            {
                param = pistolSpreadLogicConfig.AirParam;
            }
            else if (controller.RelatedPlayerMove.HorizontalVelocity > 13)
            {
                param = pistolSpreadLogicConfig.LengthGreater13Param;
            }
            else if (posture == XmlConfig.PostureInConfig.Prone || posture == XmlConfig.PostureInConfig.Crouch)
            {
                param = pistolSpreadLogicConfig.DuckParam;
            }

            return param;
        }

        private static float GeSpreadFactor(Spread spreadAimCfg, float fastMoveSpeed, PlayerWeaponController controller)
        {
            //fst layer

            var   posture      = controller.RelatedCharState.GetCurrentPostureState();
            float spreadFactor = 1f;
            if (!controller.RelatedPlayerMove.IsGround)
            {
                spreadFactor = spreadAimCfg.Air;
            }
            else if (controller.RelatedPlayerMove.HorizontalVelocity > fastMoveSpeed)
            {
                spreadFactor = spreadAimCfg.FastMove;
            }
            else if (posture == XmlConfig.PostureInConfig.Prone)
            {
                spreadFactor = spreadAimCfg.Prone;
            }

            return spreadFactor;
        }
    }
}