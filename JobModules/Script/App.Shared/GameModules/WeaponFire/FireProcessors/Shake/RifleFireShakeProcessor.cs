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
    public class RifleFireShakeProcessor : AbstractFireShakeProcessor
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(RifleFireShakeProcessor));


        public override void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var config     = controller.HeldWeaponAgent.RifleShakeCfg;
            var shakeGroup = FireShakeProvider.GetShakeGroup(config, controller);
            var shakeInfo  = FireShakeProvider.GetShakeInfo(config, controller, shakeGroup);

            CalcBaseShake(controller, cmd.CmdSeq,
                shakeInfo,
                shakeGroup.HPunchOffsetFactor,
                shakeGroup.VPunchOffsetFactor);
        }

        private void CalcBaseShake(PlayerWeaponController controller,    int   seed, ShakeInfo shakeInfo,
                                   float                  hOffsetFactor, float vOffsetFactor)
        {
            var heldAgent        = controller.HeldWeaponAgent;
            var runTimeComponent = heldAgent.RunTimeComponent;
            heldAgent.SyncParts();
            var             commonFireConfig = heldAgent.CommonFireCfg;
            ShakeInfoStruct dirShakeArgs     = FireShakeProvider.GetFireUpDirShakeArgs(heldAgent, shakeInfo);
            /*计算水平，垂直震动增量*/
            float upDirShakeDelta, lateralDirShakeDelta;
            upDirShakeDelta = FireShakeFormula.CalcFireDirShakeDelta(dirShakeArgs.UpBase, dirShakeArgs.UpModifier,
                runTimeComponent.ContinuesShootCount);
            lateralDirShakeDelta = FireShakeFormula.CalcFireDirShakeDelta(dirShakeArgs.LateralBase,
                dirShakeArgs.LateralModifier, runTimeComponent.ContinuesShootCount);
            /*应用水平，垂直震动增量*/
            float punchYaw, punchPitch;
            punchYaw   = controller.RelatedOrient.NegPunchYaw;
            punchPitch = controller.RelatedOrient.NegPunchPitch;
            //垂直震动增量应用于punchPitch
            punchPitch = FireShakeFormula.CalcPunchPitch(punchPitch, upDirShakeDelta, dirShakeArgs.UpMax,
                commonFireConfig.AttackInterval * 0.01f);
            //水平震动增量应用于punchYaw
            punchYaw = FireShakeFormula.CaclPunchYaw(runTimeComponent.PunchYawLeftSide, punchYaw,
                lateralDirShakeDelta, dirShakeArgs.LateralMax);
            /*应用于WeaponRuntimeComponent*/
            //apply PunchYawLeftSide 
            if (UniformRandom.RandomInt(seed, 0, (int) dirShakeArgs.LateralTurnback) == 0)
                runTimeComponent.PunchYawLeftSide = !runTimeComponent.PunchYawLeftSide;
            //apply PunchDecayCdTime
            runTimeComponent.PunchDecayCdTime = FireShakeProvider.GetDecayInterval(controller);
            //PunchPitchSpeed 
            //runTimeComponent.PunchPitchSpeed = FireShakeFormula.CalcPitchSpeed(punchPitch,controller.RelatedOrient.NegPunchPitch, runTimeComponent.PunchDecayCdTime);
            //PunchYawSpeed 
            runTimeComponent.PunchYawSpeed = FireShakeFormula.CalcPitchSpeed(punchYaw,
                controller.RelatedOrient.NegPunchYaw, runTimeComponent.PunchDecayCdTime);
			runTimeComponent.PunchPitchSpeed = punchPitch - controller.RelatedOrient.NegPunchPitch;
        }

        public override float UpdateLen(PlayerWeaponController controller, float len, float frameTime)
        {
            var r = len;
            r -= (controller.HeldWeaponAgent.RifleShakeCfg.FixedDecayFactor +
                  r * controller.HeldWeaponAgent.RifleShakeCfg.LenDecayFactor) * frameTime;
            r = Mathf.Max(r, 0f);
            return r;
        }

        protected override float GetWeaponPunchYawFactor(PlayerWeaponController controller)
        {
            return FireShakeProvider.GetShakeGroup(controller.HeldWeaponAgent.RifleShakeCfg,controller).WeaponFallbackFactor;
        }
    }
}
