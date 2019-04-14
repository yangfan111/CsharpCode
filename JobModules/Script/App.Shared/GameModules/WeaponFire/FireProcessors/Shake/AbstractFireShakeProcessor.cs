using UnityEngine;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="AbstractFireShakeProcessor{T1}" />
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public abstract class AbstractFireShakeProcessor: IFireShakeProcess
    {
        protected Contexts _contexts;

        public AbstractFireShakeProcessor()
        {
        }

        public abstract void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd);

        public virtual float UpdateLen(PlayerWeaponController controller, float len, float frameTime)
        {
            var r = len;
            r -= (10f + r * 0.5f) * frameTime;
            r = Mathf.Max(r, 0f);
            return r;
        }

        protected abstract float GetWeaponPunchYawFactor(PlayerWeaponController controller);

        public void OnFrame(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var runTimeComponent = controller.HeldWeaponAgent.RunTimeComponent;
            int frameInterval = cmd.FrameInterval;
            //枪械震动回落:RelatedOrient
            if (runTimeComponent.PunchDecayCdTime > 0)
            {
                var duration = GetDecayCdTime(controller);
                var lastDecayTime = runTimeComponent.PunchDecayCdTime;
                runTimeComponent.PunchDecayCdTime -= frameInterval;
                var lastPostion = easeOutExpo(duration - lastDecayTime, 0, runTimeComponent.PunchPitchSpeed, duration);
                var newPostion = easeOutExpo(duration - runTimeComponent.PunchDecayCdTime, 0, runTimeComponent.PunchPitchSpeed, duration);
                controller.RelatedOrient.NegPunchPitch += newPostion - lastPostion;
                /*controller.RelatedOrient.NegPunchPitch += runTimeComponent.PunchPitchSpeed * deltaTime;*/
                controller.RelatedOrient.WeaponPunchPitch = controller.RelatedOrient.NegPunchPitch * controller.HeldWeaponAgent.RifleShakeCfg.Default.VPunchOffsetFactor;
                //var duration = FireShakeProvider.GetDecayInterval(controller);
                var deltaTime = cmd.RenderTime - runTimeComponent.LastRenderTime;
                runTimeComponent.LastRenderTime = cmd.RenderTime;
                //controller.RelatedOrient.NegPunchPitch += runTimeComponent.PunchPitchSpeed * deltaTime;
                
                controller.RelatedOrient.NegPunchYaw += runTimeComponent.PunchYawSpeed * deltaTime;
                controller.RelatedOrient.WeaponPunchYaw = controller.RelatedOrient.NegPunchYaw * controller.HeldWeaponAgent.RifleShakeCfg.Default.HPunchOffsetFactor;
            }
            else
            {
                runTimeComponent.LastRenderTime = cmd.RenderTime;
                var punchYaw = controller.RelatedOrient.NegPunchYaw;
                var punchPitch = controller.RelatedOrient.NegPunchPitch;
                var frameTime = frameInterval / 1000f;
                var len = (float) Mathf.Sqrt(punchYaw * punchYaw + punchPitch * punchPitch);
                if (len > 0)
                {
                    punchYaw = punchYaw / len;
                    punchPitch = punchPitch / len;
                    len = UpdateLen(controller, len, frameTime);
                    var lastYaw = controller.RelatedOrient.NegPunchYaw;
                    controller.RelatedOrient.NegPunchYaw = punchYaw * len;
                    controller.RelatedOrient.NegPunchPitch = punchPitch * len;
                    var factor = GetWeaponPunchYawFactor(controller);
                    controller.RelatedOrient.WeaponPunchYaw = controller.RelatedOrient.NegPunchYaw * factor;
                    controller.RelatedOrient.WeaponPunchPitch = controller.RelatedOrient.NegPunchPitch * factor;
                }
            }
        }

        protected int GetDecayCdTime(PlayerWeaponController controller)
        {
            return (int) (controller.HeldWeaponAgent.CommonFireCfg.AttackInterval * controller.HeldWeaponAgent.RifleShakeCfg.DecaytimeFactor);
        }

        public static float easeOutExpo(float t, float b , float c, float d) {
            //t当前时间|b起始位置|c最大距离|d总时间
            return (t == d) ? b + c : c * (-(float) Mathf.Pow(2, -10 * t/d) + 1) + b;	
        }
    }
}