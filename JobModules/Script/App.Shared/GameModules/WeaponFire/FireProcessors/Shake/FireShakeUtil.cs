using System;
using Core.GameInputFilter;
using Core.Utils;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    public static class FireShakeMath
    {
        public static float CalcFireDirShake(float baseArg, float modifierArg,int shootContinueCount)
        {
            if (shootContinueCount == 1)
                shootContinueCount = 0;
            return baseArg + shootContinueCount * modifierArg;
        }
        
    }

    public static class FireShakeProvider
    {
        public static LoggerAdapter logger =new LoggerAdapter(typeof(FireShakeProvider));
        //TODO:改方式
        public static ShakeInfoStruct GetFireUpDirShakeArgs(WeaponBaseAgent heldAgent, ShakeInfo shakeInfo)
        {
            var newInfo = (ShakeInfoStruct) shakeInfo;
            float factor = heldAgent.GetAttachedAttributeByType(WeaponAttributeType.UpBase) ;
            if (factor != 0f)
                newInfo.UpBase *= factor;
        //    logger.Info("Shoot Shake factor:"+factor);
            
            factor = heldAgent.GetAttachedAttributeByType(WeaponAttributeType.UpMax) ;
            if (factor != 0f)
            newInfo.UpMax *= factor;
            factor = heldAgent.GetAttachedAttributeByType(WeaponAttributeType.UpModifier) ;
            if (factor != 0f)
            newInfo.UpModifier *= factor;
            
             factor = heldAgent.GetAttachedAttributeByType(WeaponAttributeType.LateralBase) ;
             if (factor != 0f)
            newInfo.LateralBase *= factor;
            
            factor = heldAgent.GetAttachedAttributeByType(WeaponAttributeType.LateralMax) ;
            if (factor != 0f)
            newInfo.LateralMax *= factor;
            
            factor = heldAgent.GetAttachedAttributeByType(WeaponAttributeType.LateralModifier) ;
            if (factor != 0f)
            newInfo.LateralModifier *= factor;
            
            factor = heldAgent.GetAttachedAttributeByType(WeaponAttributeType.LateralTurnback) ;
            if (factor != 0f)
            newInfo.LateralTurnback *= factor;
            return newInfo;
        }
    }
}
