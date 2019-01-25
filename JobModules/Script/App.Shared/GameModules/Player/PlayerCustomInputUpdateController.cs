using UnityEngine;

namespace App.Shared.GameModules.Player
{
    public class PlayerCustomInputUpdateController
    {
        public const float AXIS_NEUTRAL = 0.0f;
        public const float AXIS_POSITIVE = 1.0f;
        public const float AXIS_NEGATIVE = -1.0f;
        public const float Sensitivity = 1f;
        public const float Gravity = 3f;
        
        public float UpdateValue(float deltaTime, bool isPositive, bool isNegative, float currentValue)
        {

            float retValue = currentValue;
            if(isPositive)
            {
                if(retValue < AXIS_NEUTRAL)
                {
                    retValue = AXIS_NEUTRAL;
                }

                retValue += Sensitivity * deltaTime;
                if(retValue > AXIS_POSITIVE)
                {
                    retValue = AXIS_POSITIVE;
                }
            }
            else if(isNegative)
            {
                if(retValue > AXIS_NEUTRAL)
                {
                    retValue = AXIS_NEUTRAL;
                }

                retValue -= Sensitivity * deltaTime;
                if(retValue < AXIS_NEGATIVE)
                {
                    retValue = AXIS_NEGATIVE;
                }
            }
            else
            {
                retValue = BackToNeutral(deltaTime, retValue);
            }

            return retValue;
        }

        public float UpdateToTarget(float deltaTime, float target, float currentValue)
        {
            var retValue = currentValue;
            if (target > currentValue)
            {
                retValue = currentValue + Sensitivity * deltaTime;
                if (retValue > target)
                {
                    retValue = target;
                }
            }
            else if (target < currentValue)
            {
                retValue = currentValue - Sensitivity * deltaTime;
                if (retValue < target)
                {
                    retValue = target;
                }
            }

            return retValue;
        }

        private float BackToNeutral(float deltaTime, float currentValue)
        {
            float retValue = currentValue;
            if (retValue < AXIS_NEUTRAL)
            {
                retValue += Gravity * deltaTime;
                if (retValue > AXIS_NEUTRAL)
                {
                    retValue = AXIS_NEUTRAL;
                }
            }
            else if (retValue > AXIS_NEUTRAL)
            {
                retValue -= Gravity * deltaTime;
                if (retValue < AXIS_NEUTRAL)
                {
                    retValue = AXIS_NEUTRAL;
                }
            }

            return retValue;
        }
    }
}