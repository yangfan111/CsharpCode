using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="AbstractFireShakeProcessor{T1}" />
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public abstract class AbstractFireShakeProcessor : IFireShakeProcessor
    {


        public abstract void OnAfterFire(WeaponAttackProxy attackProxy, WeaponSideCmd cmd);
        //以当前0.5X+10速度衰减
        public virtual float UpdateLen(WeaponAttackProxy attackProxy, float len, float frameSec)
        {
            var r = len;
            r -= (10f + r * 0.5f) * frameSec;
            r =  Mathf.Max(r, 0f);
            return r;
        }

        protected abstract float GePuntchFallbackFactor(PlayerWeaponController weaponController);

        public virtual void OnFrame(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            attackProxy.RuntimeComponent.LastRenderTimestamp = cmd.UserCmd.RenderTime;
        }

        protected void UpdateOrientationAttenuation(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            var orientation     = attackProxy.Orientation;
            var punchYaw   = orientation.AccPunchYaw;
            var punchPitch = orientation.AccPunchPitch;

            var frameSec  = cmd.UserCmd.FrameInterval / 1000f;
            //获取向量长度
            var puntchLength        = Mathf.Sqrt(punchYaw * punchYaw + punchPitch * punchPitch);
            if (puntchLength > 0)
            {
                punchYaw   = punchYaw / puntchLength;
                punchPitch = punchPitch / puntchLength;
                   
                puntchLength        = UpdateLen(attackProxy, puntchLength, frameSec);
                //UpdateLen: AccPunchYaw  =>AccPunchYaw   
                orientation.AccPunchYaw   = punchYaw * puntchLength;
                orientation.AccPunchPitch = punchPitch * puntchLength;
         
                var factor = GePuntchFallbackFactor(attackProxy.Owner);
                //GePuntchFallbackFactor : AccPunchYaw => AccPunchPitch     
                orientation.AccPunchYawValue   = orientation.AccPunchYaw * factor;
                orientation.AccPunchPitchValue = orientation.AccPunchPitch * factor;
            }
        }

        protected void RecoverFireRoll(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            var rotateYaw =  attackProxy.Orientation.FireRoll;
            if (rotateYaw != 0)
            {
                if (attackProxy.WeaponConfigAssy.S_FireRollCfg== null)
                {
                    rotateYaw = 0;
                }
                else
                {
                    var rotatePos = rotateYaw >= 0;
                    rotateYaw -= rotateYaw * cmd.UserCmd.FrameInterval  / attackProxy.WeaponConfigAssy.S_FireRollCfg.FireRollBackTime;
                    if ((rotatePos && rotateYaw < 0) || (!rotatePos && rotateYaw > 0)) rotateYaw = 0;
                }
                attackProxy.Orientation.FireRoll = rotateYaw;
            }
        }

//        public static float easeOutExpo(float t, float b, float c, float d)
//        {
//            //t当前时间|b起始位置|c最大距离|d总时间
//            return (t == d) ? b + c : c * (-(float) Mathf.Pow(2, -10 * t / d) + 1) + b;
//        }

    }
}