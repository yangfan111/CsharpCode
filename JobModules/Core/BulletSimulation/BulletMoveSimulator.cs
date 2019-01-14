using System.Collections.Generic;
using System.Runtime.Remoting;
using Core.Compensation;
using Core.Utils;
using UnityEngine;

namespace Core.BulletSimulation
{
    public class BulletMoveSimulator
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(BulletMoveSimulator));
        private int _stepInterval;

        public BulletMoveSimulator(int stepInterval)
        {
            _stepInterval = stepInterval;
        }

        public void MoveBullet(IBulletEntity bulletEntity, int renderTime,
            List<DefaultBulletSegment> allBulletSegments)
        {
            if(renderTime < bulletEntity.NextFrameTime) return;
            
            var origin = bulletEntity.Origin;
            var velocity = bulletEntity.Velocity;
            var gravity = bulletEntity.Gravity;
            var velocityDecay = bulletEntity.VelocityDecay;
            var distance = bulletEntity.Distance;

            float interval = (renderTime - bulletEntity.ServerTime) / 1000.0f;

            Vector3 oldOrigin = origin;
            // O(1) = O(0) + V(0) * t;
            origin.x = origin.x + velocity.x * interval;
            origin.y = origin.y + velocity.y * interval;
            origin.z = origin.z + velocity.z * interval;

            if (DebugConfig.DrawBulletLine)
            {
                RuntimeDebugDraw.Draw.DrawLine(oldOrigin, origin, Color.blue, 60f);
                Debug.DrawLine(oldOrigin, origin, Color.red, 60f);
            }
            // V(1) = V(0) + a * t
            Vector3 v = velocity;
            v.y = v.y - gravity * interval;
            v = v * Mathf.Pow(velocityDecay, interval);
            velocity = v;

            RaySegment raySegment = new RaySegment();
            raySegment.Ray.origin = oldOrigin;
            var direction = origin - oldOrigin;
            raySegment.Ray.direction = direction;
            raySegment.Length = direction.magnitude;

               
            distance += raySegment.Length;

            _logger.DebugFormat("move bullet velocity {0}, direction {1}, distance {2}, total distance {3}, interval {4}, move {5} -> {6}, stepinterval {7}",
                velocity,
                direction,
                raySegment.Length,
                distance,
                interval,
                oldOrigin,
                origin,
                _stepInterval);
                
            DefaultBulletSegment segment =  DefaultBulletSegment.Allocate(renderTime, raySegment, bulletEntity);
            allBulletSegments.Add(segment);
                
                
            if (Mathf.Approximately(v.magnitude, 0))
            {
                bulletEntity.IsValid = false;
                _logger.ErrorFormat("bullet velocity is zero, set to invalid");
            }

            bulletEntity.Origin = origin;
            bulletEntity.ServerTime = renderTime;
            bulletEntity.Velocity = velocity;
            bulletEntity.Distance = distance;
            bulletEntity.NextFrameTime = renderTime + _stepInterval;
        }
    }
}