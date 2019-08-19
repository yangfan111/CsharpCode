using Core;
using Core.Utils;
using UnityEngine;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="RifleFireShakeProcessor" />
    /// </summary>
    public class RifleFireShakeProcessor : AbstractFireShakeProcessor
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(RifleFireShakeProcessor));
        /// <summary>
        /// TargetPunchPitchDelta => AccPunchYaw,AccPunchYawValue
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="cmd"></param>
                        
        public override void OnFrame(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            ShakeGroup shakeGroup = FireShakeProvider.GetShakeGroup(attackProxy.WeaponConfigAssy.SRifleShakeCfg, attackProxy.Owner);
            var runTimeComponent = attackProxy.RuntimeComponent;
            //int frameInterval = cmd.UserCmd.FrameInterval;
            var orient = attackProxy.Orientation;
            //后坐力生效时间
            var deltaTime = Mathf.Min(cmd.UserCmd.FrameInterval, cmd.UserCmd.RenderTime - runTimeComponent.LastRenderTimestamp);
            if (runTimeComponent.PunchDecayLeftInterval > 0)
            {
                float totalInterval = FireShakeProvider.GetDecayInterval(attackProxy.WeaponConfigAssy);
                float lastInterval = runTimeComponent.PunchDecayLeftInterval;
                runTimeComponent.PunchDecayLeftInterval -= deltaTime;
                float newInterval = runTimeComponent.PunchDecayLeftInterval;

                var lastPunchPitch = FireShakeFormula.EaseOutCubic(0, runTimeComponent.TargetPunchPitchDelta,
                    (totalInterval - lastInterval) / totalInterval);
                var newPunchPitch = FireShakeFormula.EaseOutCubic(0, runTimeComponent.TargetPunchPitchDelta,
                    (totalInterval - newInterval) / totalInterval);

                orient.AccPunchPitch += newPunchPitch - lastPunchPitch;
                orient.AccPunchPitchValue = orient.AccPunchPitch * shakeGroup.VPunchOffsetFactor;

                orient.AccPunchYaw += runTimeComponent.PunchYawSpeed * deltaTime;
                orient.AccPunchYawValue = orient.AccPunchYaw * shakeGroup.HPunchOffsetFactor;

                if (GlobalConst.EnableWeaponLog)
                    DebugUtil.MyLog(("orient.AccPunchPitch:" + orient.AccPunchPitch));
            }
            else
            {
                UpdateOrientationAttenuation(attackProxy, cmd);
            }

            if (attackProxy.WeaponConfigAssy.S_FireRollCfg!= null && runTimeComponent.CameraRotationInterval > 0)
            {
                runTimeComponent.CameraRotationInterval -= deltaTime;
                orient.FireRoll += runTimeComponent.CameraRotationSpeed * deltaTime;
            }
            else
            {
                RecoverFireRoll(attackProxy, cmd);
            }
            base.OnFrame(attackProxy, cmd);
        }


        public override void OnAfterFire(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            var config = attackProxy.WeaponConfigAssy.SRifleShakeCfg;
            var shakeGroup = FireShakeProvider.GetShakeGroup(config, attackProxy.Owner);
            var shakeInfo = FireShakeProvider.GetShakeInfo(config, attackProxy.Owner, shakeGroup);

            CalcBaseShake(attackProxy, cmd.UserCmd.Seq, shakeInfo);
        }
        /// <summary>
        /// last/NewAccPunchPitch->TargetPunchPitchDelta 
        /// </summary>
        /// <param name="heldAgent"></param>
        /// <param name="seed"></param>
        /// <param name="shakeInfo"></param>
        private void CalcBaseShake(WeaponAttackProxy attackProxy, int seed, ShakeInfo shakeInfo)
        {
            var runTimeComponent = attackProxy.RuntimeComponent;
            var orient = attackProxy.Orientation;
            var commonFireConfig = attackProxy.WeaponConfigAssy.S_CommonFireCfg;
            ShakeInfoStruct dirShakeArgs = FireShakeProvider.GetFireUpDirShakeArgs(attackProxy, shakeInfo);
            /*计算水平，垂直震动增量*/
            float upDirShakeDelta, lateralDirShakeDelta;
            upDirShakeDelta = FireShakeFormula.CalcFireDirShakeDelta(dirShakeArgs.UpBase, dirShakeArgs.UpModifier,
                runTimeComponent.ContinuesShootCount);
            lateralDirShakeDelta = FireShakeFormula.CalcFireDirShakeDelta(dirShakeArgs.LateralBase,
                dirShakeArgs.LateralModifier, runTimeComponent.ContinuesShootCount);
            /*应用水平，垂直震动增量*/
            float punchYaw, punchPitch;
            punchYaw = orient.AccPunchYaw;
            punchPitch = orient.AccPunchPitch;
            //垂直震动增量应用于punchPitch
            punchPitch = FireShakeFormula.CalcPunchPitch(punchPitch, upDirShakeDelta, dirShakeArgs.UpMax,
                commonFireConfig.AttackInterval * 0.01f);
            runTimeComponent.TargetPunchPitchDelta = punchPitch - orient.AccPunchPitch;

            //水平震动增量应用于punchYaw    
            punchYaw = FireShakeFormula.CaclPunchYaw(runTimeComponent.PunchYawLeftSide, punchYaw,
                lateralDirShakeDelta, dirShakeArgs.LateralMax);
            /*应用于WeaponRuntimeComponent*/
            //apply PunchYawLeftSide 
            if (UniformRandom.RandomInt(seed, 0, (int) dirShakeArgs.LateralTurnback) == 0)
                runTimeComponent.PunchYawLeftSide = !runTimeComponent.PunchYawLeftSide;
            //apply PunchDecayCdTime
            runTimeComponent.PunchDecayLeftInterval = (int) FireShakeProvider.GetDecayInterval(attackProxy.WeaponConfigAssy);
            //PunchYawSpeed 
            runTimeComponent.PunchYawSpeed = FireShakeFormula.CalcPitchSpeed(punchYaw,
                orient.AccPunchYaw, runTimeComponent.PunchDecayLeftInterval);
            //PunchPitchSpeed(Not Speed) 
            var fireRollCfg = attackProxy.WeaponConfigAssy.S_FireRollCfg;
            if (fireRollCfg != null)
            {
                var rotation = orient.FireRoll;
                var rotateYaw = dirShakeArgs.UpBase * fireRollCfg.FireRollFactor;
                runTimeComponent.CameraRotationInterval = fireRollCfg.FireRollTime;
                if (Random.Range(0, 2) == 0) rotateYaw = -rotateYaw;
                var maxFireRollAngle = fireRollCfg.MaxFireRollAngle;
                if (rotation + rotateYaw >= maxFireRollAngle)
                {
                    runTimeComponent.CameraRotationSpeed = (maxFireRollAngle - rotation) / runTimeComponent.CameraRotationInterval;
                }
                else if (rotation + rotateYaw <= -maxFireRollAngle)
                {
                    runTimeComponent.CameraRotationSpeed = (-maxFireRollAngle - rotation) / runTimeComponent.CameraRotationInterval;
                }
                else
                {
                    runTimeComponent.CameraRotationSpeed = rotateYaw / runTimeComponent.CameraRotationInterval;
                }
            }
        }

        public override float UpdateLen(WeaponAttackProxy attackProxy, float len, float frameTime)
        {
            var r = len;
            r -= (attackProxy.WeaponConfigAssy.SRifleShakeCfg.FixedDecayFactor +
                  r * attackProxy.WeaponConfigAssy.SRifleShakeCfg.LenDecayFactor) * frameTime;
            r = Mathf.Max(r, 0f);
            return r;
        }

        protected override float GePuntchFallbackFactor(PlayerWeaponController controller)
        {
            return FireShakeProvider.GetShakeGroup(controller.HeldWeaponAgent.RifleShakeCfg, controller)
                .WeaponFallbackFactor;
        }
    }
}