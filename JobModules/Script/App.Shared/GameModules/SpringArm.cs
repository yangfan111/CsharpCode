using System;
using Core.Compare;
using UnityEngine;

namespace App.Shared.GameModules
{
    public class SpringlikeLerp
    {
        public float PreviousValue;
        public int LagMaxTimeStep = 2;
        public float LagRatio = 0.5f;
        public int LastTime;

        public SpringlikeLerp(float baseValue, int stepTime, float speedRatio)
        {
            PreviousValue = baseValue;
            LagMaxTimeStep = stepTime;
            LagRatio = speedRatio;
        }
        
        public float Update(float curValue, int curTime)
        {
            var desireValue = curValue;
            var remainTime = curTime - LastTime;
            if (remainTime < LagMaxTimeStep) return PreviousValue;
            var lerpValue = PreviousValue;
            var step = (desireValue - PreviousValue) / (curTime - LastTime);
            while (remainTime > 0)
            {
                var LerpAmount = Mathf.Min(LagMaxTimeStep, remainTime);
                lerpValue += step * LerpAmount;
                remainTime -= LerpAmount;
                desireValue = Mathf.Lerp(PreviousValue, lerpValue, LagRatio);
                PreviousValue = desireValue;
            }
            LastTime = curTime;
            return PreviousValue;
        }
    }
    
    public class SpringArm
    {
        public Vector3 Offset;
        private float DetectRadius = 0.2f;
        private float LagSpeed = 0.1f;

        private float LagMaxTimeStep = 5;
        private float SmalledstNum = 0f;
        private float MaxLagDistance = 1f;

        public int CollisionLayers;

        private float PreviousDesiredLoc;

        public Vector3 LastLoc;

        public void Set(float radius, float lagSpeed, int timeStep, int layer)
        {
            CollisionLayers = layer;
            DetectRadius = radius;
            LagSpeed = lagSpeed;
            LagMaxTimeStep = timeStep;
            CollisionLayers = layer;
        }
        
        public void Update(Vector3 basePos, Quaternion curQuaternion, float deltaTime, bool doLag, bool detect)
        {
            var detectLoc = basePos + curQuaternion * Offset; 
            if(detect)
                detectLoc = RealLocByCollision(basePos, basePos + curQuaternion * Offset);
            
            var realOffset = detectLoc - basePos;
            var resultLoc = detectLoc;
            if (doLag)
            {
                var lerpOffset = LerpPosition(deltaTime, realOffset.magnitude);
                if ( CompareUtility.IsApproximatelyEqual(realOffset.magnitude, Offset.magnitude))
                {
                    resultLoc = basePos + realOffset.normalized * lerpOffset;
                }
            }

            LastLoc = resultLoc;
        }

        private Vector3 RealLocByCollision(Vector3 baseLoc, Vector3 DesireLoc)
        {
            var start = baseLoc;
            Vector3 curDist = DesireLoc - start;
            Vector3 resultLoc = DesireLoc;
            RaycastHit raycast;
            if (Physics.SphereCast(start, DetectRadius, curDist, out raycast, curDist.magnitude, CollisionLayers))
            {
                resultLoc = start + curDist.normalized * (raycast.distance);
            }
            return resultLoc;
        }

        private float LerpPosition(float deltaTime, float armOrigin)
        {
            var AimValue = armOrigin;
            if (deltaTime > LagMaxTimeStep && LagSpeed > 0)
            {
                var armMoveStep = (AimValue - PreviousDesiredLoc) * (1/deltaTime);
                var LerpTarget = PreviousDesiredLoc;
                var remainTime = deltaTime;
                while (remainTime > SmalledstNum)
                {
                    var LerpAmount = Mathf.Min(LagMaxTimeStep, remainTime);
                    LerpTarget += LerpAmount * armMoveStep;
                    remainTime -= LerpAmount;
                    AimValue = Mathf.Lerp(PreviousDesiredLoc, LerpTarget, LagSpeed);
                    PreviousDesiredLoc = AimValue;
                }
            }
            else
            {
                AimValue = Mathf.Lerp(PreviousDesiredLoc, AimValue, LagSpeed);
            }

            PreviousDesiredLoc = AimValue;
            return AimValue;
        }
    }
}