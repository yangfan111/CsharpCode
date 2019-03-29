using UnityEngine;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="AbstractKickbackLogic{T1}" />
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public abstract class AbstractKickbackLogic<T1> : IKickbackLogic
    {
        protected Contexts _contexts;

        public AbstractKickbackLogic()
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
            var weaponState = controller.HeldWeaponAgent.RunTimeComponent;
            int frameInterval = cmd.FrameInterval;
            if (weaponState.PunchDecayCdTime > 0)
            {
                weaponState.PunchDecayCdTime -= frameInterval;
                var duration = GetDecayCdTime(controller);
                var deltaTime = cmd.RenderTime - weaponState.LastRenderTime;
                weaponState.LastRenderTime = cmd.RenderTime;
                controller.RelatedOrient.NegPunchPitch += weaponState.PunchPitchSpeed * deltaTime;
                controller.RelatedOrient.WeaponPunchPitch =
                    controller.RelatedOrient.NegPunchPitch * controller.HeldWeaponAgent.RifleKickbackLogicCfg.Default.VPunchOffsetFactor;
                controller.RelatedOrient.NegPunchYaw += weaponState.PunchYawSpeed * deltaTime;
                controller.RelatedOrient.WeaponPunchYaw =
                    controller.RelatedOrient.NegPunchYaw * controller.HeldWeaponAgent.RifleKickbackLogicCfg.Default.HPunchOffsetFactor;
            }
            else
            {
                weaponState.LastRenderTime = cmd.RenderTime;
                var punchYaw = controller.RelatedOrient.NegPunchYaw;
                var punchPitch = controller.RelatedOrient.NegPunchPitch;
                var frameTime = frameInterval / 1000f;
                var len = (float)Mathf.Sqrt(punchYaw * punchYaw + punchPitch * punchPitch);
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
            return (int)(controller.HeldWeaponAgent.CommonFireCfg.AttackInterval * controller.HeldWeaponAgent.RifleKickbackLogicCfg.DecaytimeFactor);
        }
    }
}
