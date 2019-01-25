using System.Collections.Generic;
using Core.Compensation;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Throwing
{
    public class ThrowingMoveSimulator
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ThrowingMoveSimulator));
        private int _stepInterval;
        private ThrowingEntityAdapter _entityAdapter;
        private PlayerContext _playerContext;

        public ThrowingMoveSimulator(int stepInterval, PlayerContext playerContext)
        {
            _stepInterval = stepInterval;
            _playerContext = playerContext;
            _entityAdapter = new ThrowingEntityAdapter(null);
        }

        public List<IThrowingSegment> MoveThrowing(ThrowingEntity throwingEntity, int frameTime)
        {
            _entityAdapter.SetEntity(throwingEntity);

            List<IThrowingSegment> rc = new List<IThrowingSegment>();
            _entityAdapter.RemainFrameTime += frameTime;

            while (_entityAdapter.RemainFrameTime >= _stepInterval)
            {
                int oldServerTime = _entityAdapter.ServerTime;
                int t = oldServerTime / _stepInterval;
                _entityAdapter.ServerTime = _stepInterval * (t + 1);
                float interval = (_entityAdapter.ServerTime - oldServerTime) / 1000.0f;

                Vector3 oldOrigin = _entityAdapter.Origin;

                // O(1) = O(0) + V(0) * t;
                _entityAdapter.Origin = _entityAdapter.Origin + _entityAdapter.Velocity * interval;

                if (DebugConfig.DrawThrowingLine)
                {
                    RuntimeDebugDraw.Draw.DrawLine(oldOrigin, _entityAdapter.Origin, Color.blue, 60f);
                }
                // V(1) = V(0) + a * t
                Vector3 v = _entityAdapter.Velocity;
                v.y = v.y - _entityAdapter.Gravity * interval;
                v = v * Mathf.Pow(_entityAdapter.VelocityDecay, interval);
                _entityAdapter.Velocity = v;

                RaySegment raySegment = new RaySegment();
                raySegment.Ray.origin = oldOrigin;
                var direction = _entityAdapter.Origin - oldOrigin;
                raySegment.Ray.direction = direction;
                raySegment.Length = direction.magnitude;

                DefaultThrowingSegment segment = new DefaultThrowingSegment(oldServerTime, raySegment, throwingEntity, _playerContext);
                rc.Add(segment);

                _entityAdapter.RemainFrameTime -= _stepInterval;

            }

            return rc;
        }
    }
}