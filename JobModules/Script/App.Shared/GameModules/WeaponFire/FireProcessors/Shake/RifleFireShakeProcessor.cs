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
                        
        public override void OnFrame(WeaponBaseAgent agent, WeaponSideCmd cmd)
        {
            var weaponController = agent.Owner.WeaponController();
            ShakeGroup shakeGroup = FireShakeProvider.GetShakeGroup(agent.RifleShakeCfg, weaponController);
            var runTimeComponent = agent.RunTimeComponent;
            int frameInterval = cmd.UserCmd.FrameInterval;
            var orient = weaponController.RelatedOrientation;
            //后坐力生效时间
            if (runTimeComponent.PunchDecayLeftInterval > 0)
            {
                float totalInterval = FireShakeProvider.GetDecayInterval(agent);
                float lastInterval = runTimeComponent.PunchDecayLeftInterval;
                runTimeComponent.PunchDecayLeftInterval -= frameInterval;
                float newInterval = runTimeComponent.PunchDecayLeftInterval;

                var lastPunchPitch = FireShakeFormula.EaseOutCubic(0, runTimeComponent.TargetPunchPitchDelta,
                    (totalInterval - lastInterval) / totalInterval);
                var newPunchPitch = FireShakeFormula.EaseOutCubic(0, runTimeComponent.TargetPunchPitchDelta,
                    (totalInterval - newInterval) / totalInterval);

                orient.AccPunchPitch += newPunchPitch - lastPunchPitch;
                orient.AccPunchPitchValue = orient.AccPunchPitch * shakeGroup.VPunchOffsetFactor;


                var deltaTime = cmd.UserCmd.RenderTime - runTimeComponent.LastRenderTimestamp;

                orient.AccPunchYaw += runTimeComponent.PunchYawSpeed * deltaTime;
                orient.AccPunchYawValue = orient.AccPunchYaw * shakeGroup.HPunchOffsetFactor;

                if (GlobalConst.EnableWeaponLog)
                    DebugUtil.MyLog(("orient.AccPunchPitch:" + orient.AccPunchPitch));
            }
            else
            {
                UpdateOrientationAttenuation(agent, cmd);
            }

            if (agent.FireRollCfg != null && runTimeComponent.CameraRotationInterval > 0)
            {
                runTimeComponent.CameraRotationInterval -= frameInterval;
                orient.FireRoll += runTimeComponent.CameraRotationSpeed * frameInterval;
            }
            else
            {
                RecoverFireRoll(agent, cmd);
            }

            base.OnFrame(agent, cmd);
        }


        public override void OnAfterFire(WeaponBaseAgent agent, WeaponSideCmd cmd)
        {
            var weaponController = agent.Owner.WeaponController();
            var config = agent.RifleShakeCfg;
            var shakeGroup = FireShakeProvider.GetShakeGroup(config, weaponController);
            var shakeInfo = FireShakeProvider.GetShakeInfo(config, weaponController, shakeGroup);

            CalcBaseShake(agent, cmd.UserCmd.Seq, shakeInfo);
        }
        /// <summary>
        /// last/NewAccPunchPitch->TargetPunchPitchDelta 
        /// </summary>
        /// <param name="heldAgent"></param>
        /// <param name="seed"></param>
        /// <param name="shakeInfo"></param>
        private void CalcBaseShake(WeaponBaseAgent heldAgent, int seed, ShakeInfo shakeInfo)
        {
            var weaponController = heldAgent.Owner.WeaponController();
            var runTimeComponent = heldAgent.RunTimeComponent;
            var orient = weaponController.RelatedOrientation;
            heldAgent.SyncParts();
            var commonFireConfig = heldAgent.CommonFireCfg;
            ShakeInfoStruct dirShakeArgs = FireShakeProvider.GetFireUpDirShakeArgs(heldAgent, shakeInfo);
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
            runTimeComponent.PunchDecayLeftInterval = (int) FireShakeProvider.GetDecayInterval(heldAgent);
            //PunchYawSpeed 
            runTimeComponent.PunchYawSpeed = FireShakeFormula.CalcPitchSpeed(punchYaw,
                orient.AccPunchYaw, runTimeComponent.PunchDecayLeftInterval);
            //PunchPitchSpeed(Not Speed) 
            if (heldAgent.FireRollCfg != null)
            {
                var rotation = orient.FireRoll;
                var fireRollCfg = heldAgent.WeaponConfigAssy.S_FireRollCfg;
                var rotateYaw = dirShakeArgs.UpBase * fireRollCfg.FireRollFactor;
                runTimeComponent.CameraRotationInterval = heldAgent.FireRollCfg.FireRollTime;
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

        public override float UpdateLen(WeaponBaseAgent agent, float len, float frameTime)
        {
            var r = len;
            r -= (agent.RifleShakeCfg.FixedDecayFactor +
                  r * agent.RifleShakeCfg.LenDecayFactor) * frameTime;
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