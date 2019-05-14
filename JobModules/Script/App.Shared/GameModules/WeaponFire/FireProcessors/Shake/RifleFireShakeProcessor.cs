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

                orient.FireRoll += runTimeComponent.CameraRotationSpeed * frameInterval;
                if (GlobalConst.EnableWeaponLog)
                    DebugUtil.MyLog(("orient.AccPunchPitch:" + orient.AccPunchPitch));
            }
            else
            {
                UpdateOrientationAttenuation(agent, cmd);
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
            var rotation = orient.FireRoll;
            var rotateYaw = CalculateRotationDegree(heldAgent, dirShakeArgs);
            if (UnityEngine.Random.Range(0, 2) == 0) rotateYaw = -rotateYaw;
            if (rotation + rotateYaw >= 3)
            {
                runTimeComponent.CameraRotationSpeed = (3 - rotation) / runTimeComponent.PunchDecayLeftInterval;
            }
            else if (rotation + rotateYaw <= -3)
            {
                runTimeComponent.CameraRotationSpeed = (-3 - rotation) / runTimeComponent.PunchDecayLeftInterval;
            }
            else
            {
                runTimeComponent.CameraRotationSpeed = rotateYaw / runTimeComponent.PunchDecayLeftInterval;
            }
        }

        private float CalculateRotationDegree(WeaponBaseAgent agent, ShakeInfoStruct dirShakeArgs)
        {
            switch (agent.WeaponConfigAssy.NewWeaponCfg.Type)
            {
                case 1:
                    return dirShakeArgs.UpBase * 1.5f;
                case 2:
                    return dirShakeArgs.UpBase * 2f;
                case 3:
                    return dirShakeArgs.UpBase * 1.3f;
                case 4:
                    return dirShakeArgs.UpBase * 1.5f;
                case 5:
                    return dirShakeArgs.UpBase * 2f;
                case 6:
                    return dirShakeArgs.UpBase;
                default:
                    return 0;
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