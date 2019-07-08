using System.Collections.Generic;
using Core.Compensation;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;

namespace Core.Attack
{
    public class BulletHitSimulator
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(BulletHitSimulator));

        readonly List<DefaultBulletSegment> _allBulletSegments = new List<DefaultBulletSegment>();
        private IBulletHitHandler _bulletHitHandler;
        private BulletSegmentComparator _comparator = new BulletSegmentComparator();
        private ICompensationWorldFactory _compensationWorldCreator;
        List<CompensationWorld> _compensationWorldList = new List<CompensationWorld>();

        Dictionary<int, CompensationWorld> _compensationWorlds = new Dictionary<int, CompensationWorld>();

        private CustomProfileInfo _createWorld =
                        SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("bullet_createWorld");

        private int _hitboxLayerMask;

        private CustomProfileInfo _moveBullet =
                        SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("bullet_moveBullet");

        private BulletMoveSimulator _moveSimulator;

        private CustomProfileInfo _newBulletHit =
                        SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("bullet_newBulletHit");

        private CustomProfileInfo _oldBulletHit =
                        SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("bullet_oldBulletHit");

        public BulletHitSimulator(int hitboxLayerMask, ICompensationWorldFactory compensationWorldCreator,
                                  IBulletHitHandler bulletHitHandler, int moveInterval)
        {
            _hitboxLayerMask          = hitboxLayerMask;
            _compensationWorldCreator = compensationWorldCreator;
            _bulletHitHandler         = bulletHitHandler.SetHitLayerMask(_hitboxLayerMask);
            _moveSimulator            = new BulletMoveSimulator(moveInterval);
        }

        public void Update(int renderTime, int cmdSeq, List<IBulletEntityAgent> bulletEntities)
        {
            foreach (var segment in _allBulletSegments)
            {
                segment.ReleaseReference();
            }

            _allBulletSegments.Clear();

            try
            {
                _moveBullet.BeginProfileOnlyEnableProfile();
                foreach (IBulletEntityAgent bullet in bulletEntities)
                {
                    if (bullet.IsValid)
                    {
                        _moveSimulator.MoveBullet(bullet, renderTime, _allBulletSegments, cmdSeq);
                    }
                }
            }
            finally
            {
                _moveBullet.EndProfileOnlyEnableProfile();
            }


            _allBulletSegments.Sort(_comparator);

            HitSimulator(cmdSeq);
        }
#pragma warning disable RefCounter001, RefCounter002
        private void HitSimulator(int cmdSeq)
        {
            try
            {
               
                foreach (var segment in _allBulletSegments)
                {
                  
                    if (!segment.IsValid)
                        continue;
                    CompensationWorld compensationWorld ;
                    try
                    {
                        _createWorld.BeginProfileOnlyEnableProfile();
                        compensationWorld = CreateWorld(cmdSeq, segment);
                    }
                    finally
                    {
                        _createWorld.EndProfileOnlyEnableProfile();
                    }

                    if (compensationWorld == null) continue;

                    // _logger.DebugFormat("move distance in first frame is {0}", segment.BulletEntity.Distance);
                    //第一个帧间隔检查枪口位置
                    if (segment.BulletEntityAgent.IsNew)
                    {
                        try
                        {
                            _newBulletHit.BeginProfileOnlyEnableProfile();
                            InspectNewBulletHit(cmdSeq, segment, compensationWorld);
                        }
                        finally
                        {
                            _newBulletHit.EndProfileOnlyEnableProfile();
                        }
                    }
                    else
                    {
                        try
                        {
                            _oldBulletHit.BeginProfileOnlyEnableProfile();
                            InspectOldBulletHit(cmdSeq, segment, compensationWorld);
                        }
                        finally
                        {
                            _oldBulletHit.EndProfileOnlyEnableProfile();
                        }
                    }
                }
            }
            finally
            {
                foreach (var compensationWorldsValue in _compensationWorldList)
                {
                    compensationWorldsValue.Release();
                }

                _compensationWorldList.Clear();
                _compensationWorlds.Clear();
            }
        }

        private CompensationWorld CreateWorld(int cmdSeq, DefaultBulletSegment segment)
        {
            //      DebugUtil.AppendShootText(cmdSeq,"Append Segment,renderTime:{0}",segment);
            var               serverTime = segment.ServerTime;
            CompensationWorld world;
            if (!_compensationWorlds.TryGetValue(serverTime, out world))
            {
                world = _compensationWorldCreator.CreateCompensationWorld(serverTime);
                if (world == null)
                {
                    _logger.ErrorFormat("create compensation world at time {0}, FAILED", segment.ServerTime);
                    segment.BulletEntityAgent.IsValid = false;
                }
                else
                {
                    _logger.DebugFormat("create compensation world at time {0}, SUCC", serverTime);
                    _compensationWorlds[serverTime] = world;
                    _compensationWorldList.Add(world);
                }
            }

            if (world == null) return null;

            world.Self              = segment.BulletEntityAgent.OwnerEntityKey;
            world.ExcludePlayerList = segment.BulletEntityAgent.ExcludePlayerList;
            return world;
        }

        private void InspectOldBulletHit(int cmdSeq, DefaultBulletSegment segment, CompensationWorld world)
        {
            RaycastHit camDirHit;
            if (world.Raycast(segment.RaySegment, out camDirHit, _hitboxLayerMask,cmdSeq))
            {
                DebugUtil.AppendShootText(cmdSeq,"[Inspect]old world.Raycas sucess");
                _bulletHitHandler.OnHit(cmdSeq, segment.BulletEntityAgent, camDirHit, world);
            }
        }

        private void InspectNewBulletHit(int cmdSeq, DefaultBulletSegment segment, CompensationWorld world)
        {
            RaycastHit camDirHit;
            segment.BulletEntityAgent.IsNew = false;
            RaycastHit gunDirHit;
            var        camRaySegment       = segment.RaySegment;
            bool       checkGunDirObstacle = false;
            while (segment.BulletEntityAgent.IsValid && world.Raycast(camRaySegment, out camDirHit, _hitboxLayerMask,cmdSeq))
            {
                segment.BulletEntityAgent.SetIsNewInspect(true);
                if (!checkGunDirObstacle)
                {
                    checkGunDirObstacle = true;
                    //如果击中物体，从枪口向击中位置做检测，如果有物体，则使用枪口方向的结果 
                    Vector3 startPosition = segment.BulletEntityAgent.GunEmitPosition;
                    var     target        = camDirHit.point;
                    var     dir           = target - startPosition;
                    var blockCheckSegment = new RaySegment
                    {
                                    Length = Vector3.Distance(target, startPosition) - GlobalConst.RaycastStepOffset,
                                    Ray    = new Ray(startPosition, dir.normalized)
                    };
                
                    while (segment.BulletEntityAgent.IsValid &&
                    world.Raycast(blockCheckSegment, out gunDirHit, _hitboxLayerMask,cmdSeq))
                    {
                        segment.BulletEntityAgent.SetIsBlockInspect(true);
                        _bulletHitHandler.OnHit(cmdSeq, segment.BulletEntityAgent, gunDirHit, world);
                        blockCheckSegment.Ray.origin =
                                        gunDirHit.point + blockCheckSegment.Ray.direction * GlobalConst.RaycastStepOffset;
                    }
                }

                if (segment.BulletEntityAgent.IsValid)
                {
                    _bulletHitHandler.OnHit(cmdSeq, segment.BulletEntityAgent, camDirHit, world);
                    camRaySegment.Ray.origin = camDirHit.point + camRaySegment.Ray.direction * GlobalConst.RaycastStepOffset;
                }
            }

            if (segment.BulletEntityAgent.IsValid)
            {
                if (!checkGunDirObstacle)
                {
                    //如果没有击中物体，从枪口向第一帧末子弹到达的位置做检测，如果有物体，使用枪口方向的结果
                    var startPosition = segment.BulletEntityAgent.GunEmitPosition;
                    var target = segment.RaySegment.Ray.direction * segment.RaySegment.Length +
                                    segment.RaySegment.Ray.origin;
                    var dir = target - startPosition;
                    var blockCheckSegment = new RaySegment
                    {
                                    Length = Vector3.Distance(target, startPosition) - GlobalConst.RaycastStepOffset,
                                    Ray    = new Ray(startPosition, dir.normalized)
                    };
                    while (segment.BulletEntityAgent.IsValid &&
                    world.Raycast(blockCheckSegment, out gunDirHit, _hitboxLayerMask,cmdSeq))
                    {
                        segment.BulletEntityAgent.SetIsBlockInspect(true);
                        checkGunDirObstacle = true;
                        _bulletHitHandler.OnHit(cmdSeq, segment.BulletEntityAgent, gunDirHit, world);
                        blockCheckSegment.Ray.origin =
                                        gunDirHit.point + blockCheckSegment.Ray.direction * GlobalConst.RaycastStepOffset;
                    }
                }
            }
        }
    }
}