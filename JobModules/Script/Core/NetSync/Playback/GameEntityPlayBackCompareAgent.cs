using System;
using App.Shared;
using Core.Playback;
using Core.Utils;
using Utils.Singleton;

namespace Core.EntityComponent
{
    public class GameEntityPlayBackCompareAgent : AbstractGameEntityCompareAgent
    {
        private readonly CustomProfileInfo[] infos = new CustomProfileInfo[(int) EEntityType.End + 1];
        private IInterpolationInfo interpolationInfo;
        private EntityMap localEntityMap;
        public GameEntityPlayBackCompareAgent()
        {
            foreach (EEntityType value in Enum.GetValues(typeof(EEntityType)))
            {
                infos[(int) value] = SingletonManager.Get<DurationHelp>()
                                .GetCustomProfileInfo(string.Format("PlayBackDiff{0}", value.ToString()));
            }
        }
#pragma warning disable RefCounter001,RefCounter002 // possible reference counter error
        public GameEntityPlayBackCompareAgent Init(IEntityMapDiffHandler handler, IInterpolationInfo interpolationInfo,
                                                   EntityMap localEntityMap)
        {
            base.Init(handler);
            this.interpolationInfo = interpolationInfo;
            this.localEntityMap    = localEntityMap;
            return this;
        }
#pragma warning disable RefCounter002
        public override int Diff(IGameEntity leftEntity, IGameEntity rightEntity, bool skipMissHandle)
#pragma warning restore RefCounter002
        {
            bool        needSkip = false;
            IGameEntity localEntity;
            localEntityMap.TryGetValue(leftEntity.EntityKey, out localEntity);
            if (localEntity != null) // entity存在，但是不是playback
            {
                if (localEntity.HasFlagImmutabilityComponent)
                {
                    var local =
                                    localEntity.FlagImmutabilityComponent;
                    if (local.JudgeNeedSkipPlayback(interpolationInfo.LeftServerTime)) needSkip = true;
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

            diffCacheData.LeftEntity  = leftEntity;
            diffCacheData.RightEntity = rightEntity;
            handler.DoDiffEntityStart(leftEntity, rightEntity);
            int count = 0;

            try
            {
                infos[leftEntity.EntityKey.EntityType].BeginProfileOnlyEnableProfile();
                var left    = leftEntity.PlayBackComponentDictionary;
                var right   = rightEntity.PlayBackComponentDictionary;
                foreach (var kv in left)
                {
                    count++;
                    IGameComponent lv = kv.Value;
                    int k  = kv.Key;

                    IGameComponent rv;
                    if (right.TryGetValue(k, out rv))
                    {
                        handler.OnDiffComponent(diffCacheData.LeftEntity ,lv, diffCacheData.RightEntity,rv);
                    }
                    else
                    {
                        handler.OnRightComponentMissing(diffCacheData.LeftEntity ,diffCacheData.RightEntity,lv);
                    }
                }

                foreach (var kv in right)
                {
                    count++;
                    var k = kv.Key;
                    if (!left.ContainsKey(k))
                    {
                        var rv = kv.Value;
                        handler.OnLeftComponentMissing(diffCacheData.LeftEntity ,diffCacheData.RightEntity,rv);
                    }
                }
            }
            finally
            {
                infos[leftEntity.EntityKey.EntityType].EndProfileOnlyEnableProfile();
            }


            handler.DoDiffEntityFinish(leftEntity, rightEntity);
            return count;
        }
    }
}