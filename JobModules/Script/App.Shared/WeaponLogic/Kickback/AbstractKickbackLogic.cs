using App.Shared.WeaponLogic;
using Core.WeaponLogic.WeaponLogicInterface;
using UnityEngine;

namespace Core.WeaponLogic.Kickback
{
    public abstract class AbstractKickbackLogic<T1> : IKickbackLogic
    {
        protected Contexts _contexts;
        public AbstractKickbackLogic(Contexts contexts)
        {
            _contexts = contexts;
        }

        public abstract void OnAfterFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd);

        public virtual float UpdateLen(PlayerEntity playerEntity, float len, float frameTime)
        {
            var r = len;
            r -= (10f + r * 0.5f) * frameTime;
            r = Mathf.Max(r, 0f);
            return r;
        }

        protected abstract float GetWeaponPunchYawFactor(PlayerEntity playerEntity);

        public void OnFrame(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var weaponState = weaponEntity.weaponData;
            int frameInterval = cmd.FrameInterval;
            if (weaponState.PunchDecayCdTime > 0)
            {
                weaponState.PunchDecayCdTime -= frameInterval;
            }
            else
            {
                var punchYaw = playerEntity.orientation.NegPunchYaw;
                var punchPitch = playerEntity.orientation.NegPunchPitch;
                var frameTime = frameInterval / 1000f;
                var len = (float) Mathf.Sqrt(punchYaw * punchYaw + punchPitch * punchPitch);
                if (len > 0)
                {
                    punchYaw = punchYaw / len;
                    punchPitch = punchPitch / len;
                    len = UpdateLen(playerEntity, len, frameTime);
                    var lastYaw = playerEntity.orientation.NegPunchYaw; 
                    playerEntity.orientation.NegPunchYaw = punchYaw * len;
                    playerEntity.orientation.NegPunchPitch = punchPitch * len;
                    var factor = GetWeaponPunchYawFactor(playerEntity);
                    playerEntity.orientation.WeaponPunchYaw = playerEntity.orientation.NegPunchYaw * factor;
                    playerEntity.orientation.WeaponPunchPitch = playerEntity.orientation.NegPunchPitch * factor;
                }
            }
        }
    }
}