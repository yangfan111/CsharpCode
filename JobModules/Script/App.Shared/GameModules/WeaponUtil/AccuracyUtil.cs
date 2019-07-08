using System;
using UnityEngine;
using Utils.Compare;
using Utils.Utils;

namespace App.Shared.GameModules.Weapon
{
    public static class AccuracyFormula
    {
        public static float GetCommonAccuracy(float maxAccuracy, int continueShootCount, float accuracyDivisor ,float accuracyOffset,float factor =1f)
        {
            //Min(精准数值上限 , (连续开枪数^3) / 分数固定值 + 补偿固定值)*固定参数
            float currAccuracy = (continueShootCount*continueShootCount*continueShootCount)/accuracyDivisor +accuracyOffset;
            return Math.Min(maxAccuracy, currAccuracy)*factor;
        }
        [System.Obsolete]
        public static float GetPistolAccuracy(float lastInterval,float factor1,float min,float max, float factor2=0.3f)
        {
            //accuracy = (固定参数1)*(固定参数2 - ( 与上次开枪间隔 )/1000)
            //狙击枪、霰弹枪：开火时精准值不变
            var currAccuracy = factor1 * (factor2 - lastInterval / 1000);
            return CompareUtility.LimitBetween(currAccuracy, min, max);
//            float currAccuracy = (continueShootCount*continueShootCount*continueShootCount)/accuracyDivisor +accuracyOffset;
//            return Math.Min(maxAccuracy, currAccuracy)*factor;
        }
    }

}