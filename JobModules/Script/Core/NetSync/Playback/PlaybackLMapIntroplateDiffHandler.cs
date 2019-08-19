using System.Collections.Generic;
using Core.EntityComponent;
using Core.Utils;
using Utils.Singleton;

namespace Core.Playback
{
    public class PlaybackLMapIntroplateDiffHandler : EntityMapDiffHandlerAdapter
    {
        private static LoggerAdapter logger =
                        new LoggerAdapter(LoggerNameHolder<PlaybackLMapIntroplateDiffHandler>.LoggerName);

        private CustomProfileInfo info;
        private IInterpolationInfo interpolationInfo;
        private EntityMap localEntityMap;

        private List<PlayBackInfo> playBackInfos;
        public PlaybackLMapIntroplateDiffHandler()
        {
            info = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("Interpolate");
        }
#pragma warning disable RefCounter001,RefCounter002 // possible reference counter error
        public PlaybackLMapIntroplateDiffHandler init(EntityMap localEntityMap, IInterpolationInfo interpolationInfo,
                                                      List<PlayBackInfo> playBackInfos)
        {
            this.localEntityMap    = localEntityMap;
            this.interpolationInfo = interpolationInfo;
            this.playBackInfos     = playBackInfos;
            this.playBackInfos.Clear();
            return this;
        }


        public override void OnDiffComponent(IGameEntity leftEntity, IGameComponent leftComponent,
                                             IGameEntity rightEntity, IGameComponent rightComponent)
        {
            IGameEntity localEntity;
            localEntityMap.TryGetValue(leftEntity.EntityKey, out localEntity);
            if (localEntity == null) return;
            // entity存在，但是不是playback
            var localComponent = localEntity.GetComponent(leftComponent.GetComponentId());
            if (localComponent != null)
            {
                var local = localComponent as IInterpolatableComponent;
                if (local.IsInterpolateEveryFrame())
                    playBackInfos.Add(new PlayBackInfo(localComponent, leftComponent, rightComponent));
                try
                {
                    info.BeginProfileOnlyEnableProfile();
                    local.Interpolate(leftComponent, rightComponent, interpolationInfo);
                }
                finally
                {
                    info.EndProfileOnlyEnableProfile();
                }
            }
        }


        public override bool IsExcludeComponent(IGameComponent component)
        {
            return !(component is IPlaybackComponent);
        }
    }
}