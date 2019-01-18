using System.Collections.Generic;
using Core.Components;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.Replicaton;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;

namespace Core.Compensation
{
    public class CompensationWorld : BaseRefCounter, ICompensationWorld
    {
        public static CompensationWorld Allocate()
        {
            return ObjectAllocatorHolder<CompensationWorld>.Allocate();
        }

        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(CompensationWorld))
            {
            }

            public override object MakeObject()
            {
                return new CompensationWorld();
            }
        }

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CompensationWorld));
        private IGameEntity[] _entities;
        private List<bool> _updateAndEnabled = new List<bool>();
        private IHitBoxEntityManager _hitboxHandler;
        private int _serverTime;
        private EntityMap _entityMap;

        private CustomProfileInfo _enableHitBox;
        private CompensationWorld()
        {
        }

        public void Init(int serverTime, EntityMap entityMap, IHitBoxEntityManager hitboxHandler)
        {
            if (_enableHitBox == null)
            {
                _enableHitBox=SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("CompensationWorld_enableHitBox");
            }
         
            _entityMap = entityMap;
            _entityMap.AcquireReference();
            _serverTime = serverTime;
            _entities = _entityMap.ToArray();
            _updateAndEnabled.Clear();
            for (int i = 0; i < _entities.Length; i++)
            {
                _updateAndEnabled.Add(false);
            }

            _hitboxHandler = hitboxHandler;
        }


        public int ServerTime
        {
            get { return _serverTime; }
        }

        public EntityKey Self { get; set; }
        public List<int> ExcludePlayerList { get; set; }

        public bool BoxCast(BoxInfo box, out RaycastHit hitInfo, int hitboxLayerMask)
        {
            for (int i = 0; i < _entities.Length; i++)
            {
                var entity = _entities[i];
                if (!entity.IsCompensation)
                {
                    continue;
                }

                if (!entity.HasComponent<PositionComponent>())
                {
                    continue;
                }
                

                if (entity.EntityKey == Self)
                {
                    _hitboxHandler.EnableHitBox(entity, false);
                    continue;
                }

                if (entity.EntityType == Self.EntityType && ExcludePlayerList.Contains(entity.EntityId))
                {
                    _hitboxHandler.EnableHitBox(entity, false);
                    continue;
                }

                if (!_updateAndEnabled[i])
                {
                    Vector3 position;
                    float radius;
                    if (_hitboxHandler.GetPositionAndRadius(entity, out position, out radius))
                    {
                        var dis = Vector3.Distance(box.Origin, position);
                        var limit = new Vector3(box.HalfExtens.x, box.HalfExtens.y, box.Length).magnitude;
                        if (dis - radius <= limit)
                        {
                            try
                            {
                                _enableHitBox.BeginProfileOnlyEnableProfile();
                                _hitboxHandler.EnableHitBox(entity, true);
                                _hitboxHandler.UpdateHitBox(entity);
                                _hitboxHandler.DrawHitBoxOnBullet(entity);
                                _updateAndEnabled[i] = true;
                            }
                            finally
                            {
                                _enableHitBox.EndProfileOnlyEnableProfile();
                            }
                           
                        }
                    }
                    else
                    {
                        _updateAndEnabled[i] = true;
                        Logger.ErrorFormat("{0} Is Not Exist any more", entity.EntityKey);
                    }
                }
            }

            return _hitboxHandler.BoxCast(box, out hitInfo, hitboxLayerMask);
        }

        public bool Raycast(RaySegment ray, out RaycastHit hitInfo, int hitboxLayerMask)
        {
            for (int i = 0; i < _entities.Length; i++)
            {
                IGameEntity entity = _entities[i];
                if (!entity.IsCompensation)
                    continue;
                if (!entity.HasComponent<PositionComponent>())
                {
                    continue;
                }
                if (entity.EntityKey == Self)
                {
                    _hitboxHandler.EnableHitBox(entity, false);
                    continue;
                }
                if (entity.EntityType == Self.EntityType && ExcludePlayerList.Contains(entity.EntityId))
                {
                    _hitboxHandler.EnableHitBox(entity, false);
                    continue;
                }
                if (!_updateAndEnabled[i]) // 同一个serverTime，hitbox只需要计算一次
                {
                    Vector3 position;
                    float radius;
                    if (_hitboxHandler.GetPositionAndRadius(entity, out position, out radius))
                    {

                        if (IsLineInPointRadius(ray.Ray.origin, ray.Ray.GetPoint(ray.Length), position, radius))
                        {
                            try
                            {
                                _enableHitBox.BeginProfileOnlyEnableProfile();
                                _hitboxHandler.EnableHitBox(entity, true);
                                _hitboxHandler.UpdateHitBox(entity);
                                _hitboxHandler.DrawHitBoxOnBullet(entity);
                                _updateAndEnabled[i] = true;
                            }
                            finally
                            {
                                _enableHitBox.EndProfileOnlyEnableProfile();
                            }
                        }
                    }
                    else
                    {
                        _updateAndEnabled[i] = true;
                        Logger.ErrorFormat("{0} Is Not Exist any more", entity.EntityKey);
                    }
                }
            }

           

            return _hitboxHandler.Raycast(ray.Ray, out hitInfo, ray.Length, hitboxLayerMask);
        }

        public static float DistanceToLine(RaySegment ray, Vector3 point)
        {
            return DistanceToLine(ray.Ray.origin, ray.Ray.GetPoint(ray.Length), point);
        }

        public static bool IsLineInPointRadius(Vector3 lineStart, Vector3 lineEnd, Vector3 point, float radius)
        {
            float radiusSqr = radius * radius;
            float vec1Sqr = (point - lineStart).sqrMagnitude;
            float vec2Sqr = (point - lineEnd).sqrMagnitude;
            float vecLineSqr = (lineEnd - lineStart).sqrMagnitude;
            if (vec1Sqr < radiusSqr || vec2Sqr < radiusSqr)//两个点有一个在圆内
            {
                return true;
            }

            //if (vec1Sqr * vec2Sqr / vecLineSqr > radiusSqr)//用三角形面积推导出 b*c*sinA=a*h
            //{
            //    return false;
            //}

            float r = DistanceToLine(lineStart, lineEnd, point);
            return r < radius;
        }

        public static float DistanceToLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 vec1 = point - lineStart;
            Vector3 vec2 = lineEnd - lineStart;
            Vector3 vecProj = Vector3.Project(vec1, vec2);

            Vector3 crossPos = vecProj + lineStart;
            float disLine = Vector3.SqrMagnitude(vec2);
            float dis1 = Vector3.SqrMagnitude(crossPos - lineStart);
            float dis2 = Vector3.SqrMagnitude(crossPos - lineEnd);
            if (dis1 + dis2 > disLine - 0.001f)
            {
                crossPos = dis1 < dis2 ? lineStart : lineEnd;
            }

            float dis = Vector3.Distance(point, crossPos);
            return dis;
        }

        protected override void OnCleanUp()
        {
            Dispose();
            ObjectAllocatorHolder<CompensationWorld>.Free(this);
        }

        public void Dispose()
        {
            for (int i = 0; i < _entities.Length; i++)
            {
                if (_updateAndEnabled[i])
                {
                    IGameEntity entity = _entities[i];
                    _hitboxHandler.EnableHitBox(entity, false);
                }
            }

            _updateAndEnabled.Clear();
            _entityMap.ReleaseReference();
        }

        public bool TryGetEntityPosition(EntityKey key, out Vector3 pos)
        {
            pos = Vector3.zero;
            for (int i = 0; i < _entities.Length; i++)
            {
                IGameEntity entity = _entities[i];
                if (!entity.IsCompensation)
                    continue;
                if (entity.EntityKey == key)
                {
                    var posComp = entity.Position;
                    if (null != posComp)
                    {
                        pos = posComp.Value;
                        return true;
                    }
                    else
                    {
                        Logger.ErrorFormat("no position component in entity {0}", key);
                        return false;
                    }
                }
            }

            Logger.ErrorFormat("no entity with key {0}", key);
            return false;
        }

        public void Release()
        {
            ReleaseReference();
           
        }
    }
}