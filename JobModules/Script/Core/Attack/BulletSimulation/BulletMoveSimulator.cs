using System.Collections.Generic;
using System.Runtime.Remoting;
using Core.Compensation;
using Core.Utils;
using UnityEngine;

namespace Core.Attack
{
    public class BulletMoveSimulator
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(BulletMoveSimulator));
        private int _stepInterval;

        public BulletMoveSimulator(int stepInterval)
        {
            _stepInterval = stepInterval;
        }

        public void MoveBullet(IBulletEntityAgent         bulletEntityAgent, int renderTime,
                               List<DefaultBulletSegment> allBulletSegments, int cmdSeq)
        {
            if(renderTime < bulletEntityAgent.NextFrameTime) return;
            
            var origin = bulletEntityAgent.Position;
            var velocity = bulletEntityAgent.Velocity;
            var gravity = bulletEntityAgent.Gravity;
            var velocityDecay = bulletEntityAgent.VelocityDecay;
            var distance = bulletEntityAgent.Distance;

            float interval = (renderTime - bulletEntityAgent.ServerTime) / 1000.0f;

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
            // string rayInfo = string.Format("direction {0}, length {1},readInterval {2}, move {3} -> {4}, stepinterval {5}",direction,raySegment.Length, interval, oldOrigin, origin, _stepInterval);
                
            DefaultBulletSegment segment =  DefaultBulletSegment.Allocate(renderTime, raySegment, bulletEntityAgent);
         
            allBulletSegments.Add(segment);
                
            if (Mathf.Approximately(v.magnitude, 0))
            {
                bulletEntityAgent.IsValid = false;
                _logger.ErrorFormat("bullet velocity is zero, set to invalid");
                DebugUtil.AppendShootText(cmdSeq,"[Bullet Move] bullet {0}invalid",bulletEntityAgent.OwnerEntityKey);
            }

            bulletEntityAgent.Position = origin;
            bulletEntityAgent.ServerTime = renderTime;
            bulletEntityAgent.Velocity = velocity;
            bulletEntityAgent.Distance = distance;
            bulletEntityAgent.NextFrameTime = renderTime + _stepInterval;
            
         //   DebugUtil.AppendShootText(cmdSeq,"[Bullet Move]rayInfo :{0} //// bulletInfo :{1} ",rayInfo,bulletEntityAgent.ToMoveString());
        }
    }
}