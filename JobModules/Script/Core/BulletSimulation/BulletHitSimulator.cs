using System.Collections.Generic;
using Core.Compensation;
using Core.ObjectPool;
using Core.Replicaton;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;

namespace Core.BulletSimulation
{
    public class BulletHitSimulator
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(BulletHitSimulator));
        private int _hitboxLayerMask;
        private IBulletEntityCollector _bulletEntityCollector;
        private ICompensationWorldFactory _compensationWorldFactory;
        private IBulletHitHandler _bulletHitHandler;
        private BulletMoveSimulator _moveSimulator;
        private const float RaycastStepOffset = 0.01f;
        private CustomProfileInfo _createWorld = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("bullet_createWorld");
        private CustomProfileInfo _newBulletHit = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("bullet_newBulletHit");
        private CustomProfileInfo _oldBulletHit = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("bullet_oldBulletHit");
        private CustomProfileInfo _moveBullet = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("bullet_moveBullet");
        public BulletHitSimulator(int hitboxLayerMask,
            IBulletEntityCollector bulletEntityCollector,
            ICompensationWorldFactory compensationWorldFactory,
            IBulletHitHandler bulletHitHandler,
            int moveInterval)
        {
            _hitboxLayerMask = hitboxLayerMask;
            _bulletEntityCollector = bulletEntityCollector;
            _compensationWorldFactory = compensationWorldFactory;
            _bulletHitHandler = bulletHitHandler.SetHitLayerMask(_hitboxLayerMask);
            _moveSimulator = new BulletMoveSimulator(moveInterval);
        }

        private List<DefaultBulletSegment> _allBulletSegments = new List<DefaultBulletSegment>();
        private BulletSegmentComparator _comparator = new BulletSegmentComparator();

        public void Update(int renderTime, int cmdSeq)
        {
            foreach (var segment in _allBulletSegments)
            {
                segment.ReleaseReference();
            }

            _allBulletSegments.Clear();

            try
            {
                _moveBullet.BeginProfileOnlyEnableProfile();
                foreach (IBulletEntity bullet in _bulletEntityCollector.GetAllBulletEntities())
                {
                    if (bullet.IsValid)
                    {
                        _moveSimulator.MoveBullet(bullet, renderTime, _allBulletSegments);
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

        Dictionary<int, ICompensationWorld> _compensationWorlds = new Dictionary<int, ICompensationWorld>();
        List<ICompensationWorld> _compensationWorldList = new List<ICompensationWorld>();
        private void HitSimulator(int cmdSeq)
        {
            try
            {
                foreach (var segment in _allBulletSegments)
                {
                    if (!segment.IsValid)
                        continue;
                    
                    ICompensationWorld world;
                    try
                    {
                        _createWorld.BeginProfileOnlyEnableProfile();
                        world = CreateWorld(segment);
                    }
                    finally
                    {
                        _createWorld.EndProfileOnlyEnableProfile();
                    }
                   
                    if(world==null) continue;
                    
                    // _logger.DebugFormat("move distance in first frame is {0}", segment.BulletEntity.Distance);
                    //第一个帧间隔检查枪口位置
                    if (segment.BulletEntity.IsNew)
                    {
                        try
                        {
                            _newBulletHit.BeginProfileOnlyEnableProfile();
                            NewBulletHit(cmdSeq, segment, world);
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
                            OldBulletHit(cmdSeq, segment, world);
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

        private ICompensationWorld CreateWorld(DefaultBulletSegment segment)
        {
            var serverTime = segment.ServerTime;
            ICompensationWorld world;
            if (!_compensationWorlds.ContainsKey(serverTime))
            {
                world = _compensationWorldFactory.CreateCompensationWorld(serverTime);
                if (world == null)
                {
                    _logger.ErrorFormat("create compensation world at time {0}, FAILED", segment.ServerTime);
                    segment.BulletEntity.IsValid = false;
                }
                else
                {
                    _logger.DebugFormat("create compensation world at time {0}, SUCC", serverTime);
                    _compensationWorlds[serverTime] = world;
                    _compensationWorldList.Add(world);
                }
            }
            else
            {
                world = _compensationWorlds[serverTime];
            }

            if (world == null) return null;


            world.Self = segment.BulletEntity.OwnerEntityKey;
            world.ExcludePlayerList = segment.BulletEntity.ExcludePlayerList;
            return world;
        }

        private void OldBulletHit(int cmdSeq, DefaultBulletSegment segment, ICompensationWorld world)
        {
            RaycastHit camDirHit;
            if (world.Raycast(segment.RaySegment, out camDirHit, _hitboxLayerMask))
            {
                _bulletHitHandler.OnHit(cmdSeq, segment.BulletEntity, camDirHit, world);
            }
        }

        private void NewBulletHit(int cmdSeq, DefaultBulletSegment segment, ICompensationWorld world)
        {
            RaycastHit camDirHit;
            segment.BulletEntity.IsNew = false;
            RaycastHit gunDirHit;
            var camRaySegment = segment.RaySegment;
            bool checkGunDirObstacle = false;
            while (segment.BulletEntity.IsValid
                   && world.Raycast(camRaySegment, out camDirHit, _hitboxLayerMask))
            {
                if (!checkGunDirObstacle)
                {
                    checkGunDirObstacle = true;
                    //如果击中物体，从枪口向击中位置做检测，如果有物体，则使用枪口方向的结果 
                    var startPosition = segment.BulletEntity.GunEmitPosition;
                    var target = camDirHit.point;
                    var dir = target - startPosition;
                    var blockCheckSegment = new RaySegment()
                    {
                        Length = Vector3.Distance(target, startPosition) - RaycastStepOffset,
                        Ray = new Ray(startPosition, dir.normalized),
                    };

                    while (segment.BulletEntity.IsValid
                           && world.Raycast(blockCheckSegment, out gunDirHit,
                               _hitboxLayerMask))
                    {
                        _bulletHitHandler.OnHit(cmdSeq, segment.BulletEntity, gunDirHit, world);
                        blockCheckSegment.Ray.origin =
                            gunDirHit.point + blockCheckSegment.Ray.direction * RaycastStepOffset;
                    }
                }

                if (segment.BulletEntity.IsValid)
                {
                    _bulletHitHandler.OnHit(cmdSeq, segment.BulletEntity, camDirHit, world);
                    camRaySegment.Ray.origin =
                        camDirHit.point + camRaySegment.Ray.direction * RaycastStepOffset;
                }
            }

            if (segment.BulletEntity.IsValid)
            {
                if (!checkGunDirObstacle)
                {
                    //如果没有击中物体，从枪口向第一帧末子弹到达的位置做检测，如果有物体，使用枪口方向的结果
                    var startPosition = segment.BulletEntity.GunEmitPosition;
                    var target = segment.RaySegment.Ray.direction * segment.RaySegment.Length +
                                 segment.RaySegment.Ray.origin;
                    var dir = target - startPosition;
                    var blockCheckSegment = new RaySegment()
                    {
                        Length = Vector3.Distance(target, startPosition) - RaycastStepOffset,
                        Ray = new Ray(startPosition, dir.normalized),
                    };
                    while (segment.BulletEntity.IsValid
                           && world.Raycast(blockCheckSegment, out gunDirHit,
                               _hitboxLayerMask))
                    {
                        checkGunDirObstacle = true;
                        _bulletHitHandler.OnHit(cmdSeq, segment.BulletEntity, gunDirHit, world);
                        blockCheckSegment.Ray.origin =
                            gunDirHit.point + blockCheckSegment.Ray.direction * RaycastStepOffset;
                    }
                }
            }
        }
    }
}