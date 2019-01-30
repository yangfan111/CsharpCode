using Core.WeaponLogic.Attachment;
using UnityEngine;
using WeaponConfigNs;
using Core.WeaponLogic.Common;

namespace Core.WeaponLogic.Kickback
{
    public abstract class AbstractKickbackLogic<T1, T3> : AbstractAttachableWeaponLogic<T1,  T3>, IKickbackLogic where T1 : ICopyableConfig<T1>, new()
    {
        public AbstractKickbackLogic(T1 config) :base(config)
        {

        }

        public abstract void AfterFireBullet(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bullet);

        public virtual float UpdateLen(float len, float frameTime)
        {
            var r = len;
            r -= (10f + r * 0.5f) * frameTime;
            r = Mathf.Max(r, 0f);
            return r;
        }

        protected abstract float GetWeaponPunchYawFactor(IPlayerWeaponState playerWeapon);

        public void OnFrame(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            int frameInterval = cmd.FrameInterval;
            if (playerWeapon.PunchDecayCdTime > 0)
            {
                playerWeapon.PunchDecayCdTime -= frameInterval;
            }
            else
            {
                var punchYaw = playerWeapon.NegPunchYaw;
                var punchPitch = playerWeapon.NegPunchPitch;
                var frameTime = frameInterval / 1000f;
                var len = (float) Mathf.Sqrt(punchYaw * punchYaw + punchPitch * punchPitch);
                if (len > 0)
                {
                    punchYaw = punchYaw / len;
                    punchPitch = punchPitch / len;
                    len = UpdateLen(len, frameTime);
                    var lastYaw = playerWeapon.NegPunchYaw; 
                    playerWeapon.NegPunchYaw = punchYaw * len;
                    playerWeapon.NegPunchPitch = punchPitch * len;
                    var factor = GetWeaponPunchYawFactor(playerWeapon);
                    playerWeapon.WeaponPunchYaw = playerWeapon.NegPunchYaw * factor;
                    playerWeapon.WeaponPunchPitch = playerWeapon.NegPunchPitch * factor;
                }
            }
        }

        public abstract void SetVisualConfig(ref VisualConfigGroup config);
    }
}