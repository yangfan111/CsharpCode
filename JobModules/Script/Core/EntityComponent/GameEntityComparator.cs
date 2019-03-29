using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using App.Shared;
using Core.Playback;
using Core.Utils;
using JetBrains.Annotations;
using Utils.Singleton;

namespace Core.EntityComponent
{
    public interface IGameEntityComparator
    {
        int Diff(IGameEntity leftEntity, IGameEntity rightEntity, bool skipMissHandle);
    }

    public abstract class AbstractGameEntityComparator : IGameEntityComparator
    {
        protected GameComponentComparatorHandler _componentComparator = new GameComponentComparatorHandler();
        protected IEntityMapCompareHandler _handler;
        protected Func<IGameComponent, bool> _isIncludeDelegate;

        public virtual void Init(IEntityMapCompareHandler handler)
        {
            _handler = handler;

            _componentComparator.Init(_handler);
            _isIncludeDelegate = IsIncludeDelegate;
        }

        public bool IsIncludeDelegate(IGameComponent item)
        {
            return !_handler.IsExcludeComponent(item);
        }

        public abstract int Diff(IGameEntity leftEntity, IGameEntity rightEntity, bool skipMissHandle);
       
    }

    public class GameEntityComparator : AbstractGameEntityComparator
    {
        public GameEntityComparator(IEntityMapCompareHandler handler) 
        {
            base.Init(handler);
        }

#pragma warning disable RefCounter002
        public override int Diff(IGameEntity leftEntity, IGameEntity rightEntity, bool skipMissHandle)
#pragma warning restore RefCounter002
        {
            _componentComparator.LeftEntity = leftEntity;
            _componentComparator.RightEntity = rightEntity;
            _handler.OnDiffEntityStart(leftEntity, rightEntity);

            int count = SortedEnumeratableComparator<IGameComponent>.Diff(
                leftEntity.SortedComponentList,
                rightEntity.SortedComponentList,
                _componentComparator,
                _isIncludeDelegate);

            _handler.OnDiffEntityFinish(leftEntity, rightEntity);
            return count;
        }
    }

    public class GameEntityPlayBackDiffComparator : AbstractGameEntityComparator
    {
        private  IInterpolationInfo _interpolationInfo;
        private  EntityMap _localEntityMap;
        private readonly CustomProfileInfo [] _infos = new CustomProfileInfo[(int)EEntityType.End + 1];
        public GameEntityPlayBackDiffComparator() 
        {
         
            foreach (EEntityType value in Enum.GetValues(typeof(EEntityType)))
            {
                _infos[(int)value]=SingletonManager.Get<DurationHelp>().GetCustomProfileInfo(string.Format("PlayBackDiff{0}", value.ToString()));
            }
           
        }
#pragma warning disable RefCounter001,RefCounter002 // possible reference counter error
        public GameEntityPlayBackDiffComparator Init(IEntityMapCompareHandler handler,IInterpolationInfo interpolationInfo, EntityMap localEntityMap)
        {
            base.Init(handler);
            _interpolationInfo = interpolationInfo;
            _localEntityMap = localEntityMap;
            return this;
        }
#pragma warning disable RefCounter002
        public override int Diff(IGameEntity leftEntity, IGameEntity rightEntity, bool skipMissHandle)
#pragma warning restore RefCounter002
        {
            bool needSkip = false;
            IGameEntity localEntity;
            _localEntityMap.TryGetValue(leftEntity.EntityKey, out localEntity);
            if (localEntity != null) // entity存在，但是不是playback
            {
                if (localEntity.HasFlagImmutabilityComponent)
                {
                    var local = localEntity.FlagImmutabilityComponent;
                    if (local.JudgeNeedSkipPlayback(_interpolationInfo.LeftServerTime)) needSkip = true;
                }
            }
            else
            {
                needSkip = true;
            }

            if (needSkip)
            {
                return 0;
            }

            _componentComparator.LeftEntity = leftEntity;
            _componentComparator.RightEntity = rightEntity;
            _handler.OnDiffEntityStart(leftEntity, rightEntity);
            int count = 0;

            try
            {
                _infos[leftEntity.EntityKey.EntityType].BeginProfileOnlyEnableProfile();
                var left = leftEntity.PlayBackComponentDictionary;
                var right = rightEntity.PlayBackComponentDictionary;
                var handler = _componentComparator;
                foreach (var kv in left)
                {
                    count++;
                    var lv = kv.Value;
                    var k = kv.Key;
               
                    IGameComponent rv;
                    if (right.TryGetValue(k, out rv))
                    {
                        handler.OnItemSame(lv, rv);
                    }
                    else
                    {
                        handler.OnRightItemMissing(lv);
                    }
                
                }
                foreach (var kv in right)
                {
                    count++;
                    var k = kv.Key;
                    if (!left.ContainsKey(k))
                    {
                        var rv = kv.Value;
                        handler.OnLeftItemMissing(rv);
                    }
                }

            }
            finally
            {
                _infos[leftEntity.EntityKey.EntityType].EndProfileOnlyEnableProfile();
            }


            _handler.OnDiffEntityFinish(leftEntity, rightEntity);
            return count;
        }
    }

    public class GameEntityPlayBackInterpolateComparator : AbstractGameEntityComparator
    {
        private IInterpolationInfo _interpolationInfo;
        private EntityMap _localEntityMap;
        private readonly CustomProfileInfo [] _infos = new CustomProfileInfo[(int)EEntityType.End + 1];
        private bool _isNewLeftEntityMap = false;
        public GameEntityPlayBackInterpolateComparator() 
        {
            foreach (EEntityType value in Enum.GetValues(typeof(EEntityType)))
            {
                _infos[(int)value]=SingletonManager.Get<DurationHelp>().GetCustomProfileInfo(string.Format("PlayBackInterpolate_{0}", value.ToString()));
            }
        }
#pragma warning disable RefCounter001,RefCounter002 // possible reference counter error
        public GameEntityPlayBackInterpolateComparator Init(IEntityMapCompareHandler handler,
            IInterpolationInfo interpolationInfo, EntityMap localEntityMap)
        {
            base.Init(handler);
            _interpolationInfo = interpolationInfo;
            _localEntityMap = localEntityMap;
           
            return this;
        }
#pragma warning disable RefCounter002
        public override int Diff(IGameEntity leftEntity, IGameEntity rightEntity, bool skipMissHandle)
#pragma warning restore RefCounter002
        {
            bool needSkip = false;
            IGameEntity localEntity;
            _localEntityMap.TryGetValue(leftEntity.EntityKey, out localEntity);
            if (localEntity != null) // entity存在，但是不是playback
            {
                if (localEntity.HasFlagImmutabilityComponent)
                {
                    var local = localEntity.FlagImmutabilityComponent;
                    if (local.JudgeNeedSkipPlayback(_interpolationInfo.LeftServerTime, true)) needSkip = true;
                }
            }
            else
            {
                needSkip = true;
            }

            if (needSkip)
            {
                return 0;
            }

            {
                _componentComparator.LeftEntity = leftEntity;
                _componentComparator.RightEntity = rightEntity;
                _handler.OnDiffEntityStart(leftEntity, rightEntity);

             
                int count = 0;

                try
                {
                    _infos[leftEntity.EntityKey.EntityType].BeginProfileOnlyEnableProfile();
                   
                    var left = leftEntity.PlayBackComponentDictionary;
                    var right = rightEntity.PlayBackComponentDictionary;
                    foreach (var kv in left)
                    {
                        count++;
                        var lv = kv.Value;
                        var k = kv.Key;
                    
                        IGameComponent rv;
                        if (right.TryGetValue(k, out rv))
                        {
                            _componentComparator.OnItemSame(lv, rv);
                        }
                    }
               
                }
                finally
                {
                    _infos[leftEntity.EntityKey.EntityType].EndProfileOnlyEnableProfile();
                } 
                _handler.OnDiffEntityFinish(leftEntity, rightEntity);
                return count;
            }
        }
    }

    public class GameEntitySelfLatestComparator : AbstractGameEntityComparator
    {
        private int _serverTime;
        public GameEntitySelfLatestComparator() 
        {
           
        }

        public GameEntitySelfLatestComparator Init(IEntityMapCompareHandler handler, int serverTime)
        {
            base.Init(handler);
            _serverTime = serverTime;
            return this;
        }
#pragma warning disable RefCounter002
        public override int Diff(IGameEntity leftEntity, IGameEntity rightEntity, bool skipMissHandle)
#pragma warning restore RefCounter002
        {
           
            bool needSkip = false;
            if (leftEntity.HasFlagImmutabilityComponent && rightEntity.HasFlagImmutabilityComponent)
            {
                var local = leftEntity.FlagImmutabilityComponent;
                var remote = rightEntity.FlagImmutabilityComponent;
                if (local.LastModifyServerTime != remote.LastModifyServerTime)
                {
                    local.Reset();
                }else  if (local.JudgeNeedSkipSyncLatest(_serverTime))
                {
                    needSkip = true;
                }
            }

            if (needSkip)
            {
                return 0;
            }

            _componentComparator.LeftEntity = leftEntity;
            _componentComparator.RightEntity = rightEntity;
            _handler.OnDiffEntityStart(leftEntity, rightEntity);
            int count = 0;
            var left = leftEntity.SyncLatestComponentDictionary;
            var right = rightEntity.SyncLatestComponentDictionary;
            var handler = _componentComparator;
            foreach (var kv in left)
            {
                count++;
                var lv = kv.Value;
                var k = kv.Key;
               
                IGameComponent rv;
                if (right.TryGetValue(k, out rv))
                {
                    handler.OnItemSame(lv, rv);
                }
                else
                {
                    handler.OnRightItemMissing(lv);
                }
                
            }

            foreach (var kv in right)
            {
                count++;
               
                var k = kv.Key;
                if (!left.ContainsKey(k))
                {
                    var rv = kv.Value;
                    handler.OnLeftItemMissing(rv);
                }
            }

            _handler.OnDiffEntityFinish(leftEntity, rightEntity);
            return count;
        }
    }
}