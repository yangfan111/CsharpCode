using Core;
using Core.Prediction.UserPrediction.Cmd;
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


        public abstract void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd);
        //以当前0.5X+10速度衰减
        public virtual float UpdateLen(PlayerWeaponController controller, float len, float frameSec)
        {
            var r = len;
            r -= (10f + r * 0.5f) * frameSec;
            r =  Mathf.Max(r, 0f);
            return r;
        }

        protected abstract float GePuntchFallbackFactor(PlayerWeaponController controller);

        public virtual void OnFrame(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            controller.HeldWeaponAgent.RunTimeComponent.LastRenderTime = cmd.RenderTime;
        }

        protected void UpdateOrientationAttenuation(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var orientation     = controller.RelatedOrientation;
            var punchYaw   = orientation.AccPunchYaw;
            var punchPitch = orientation.AccPunchPitch;

            var frameSec  = cmd.FrameInterval / 1000f;
            //获取向量长度
            var puntchLength        = Mathf.Sqrt(punchYaw * punchYaw + punchPitch * punchPitch);
            if (puntchLength > 0)
            {
                punchYaw   = punchYaw / puntchLength;
                punchPitch = punchPitch / puntchLength;
                   
                puntchLength        = UpdateLen(controller, puntchLength, frameSec);
                //UpdateLen: AccPunchYaw  =>AccPunchYaw   
                orientation.AccPunchYaw   = punchYaw * puntchLength;
                orientation.AccPunchPitch = punchPitch * puntchLength;
        
                var factor = GePuntchFallbackFactor(controller);
                //GePuntchFallbackFactor : AccPunchYaw => AccPunchPitch     
                orientation.AccPunchYawValue   = orientation.AccPunchYaw * factor;
                orientation.AccPunchPitchValue = orientation.AccPunchPitch * factor;
            }
            var rotateYaw = orientation.Roll;
            if (rotateYaw != 0)
            {
                var rotatePos = rotateYaw >= 0;
                rotateYaw -= rotateYaw * cmd.FrameInterval  / FireShakeProvider.GetDecayInterval(controller);
                if ((rotatePos && rotateYaw < 0) || (!rotatePos && rotateYaw > 0)) rotateYaw = 0;
                orientation.Roll = rotateYaw;
            }


        }

//        public static float easeOutExpo(float t, float b, float c, float d)
//        {
//            //t当前时间|b起始位置|c最大距离|d总时间
//            return (t == d) ? b + c : c * (-(float) Mathf.Pow(2, -10 * t / d) + 1) + b;
//        }
    }
}