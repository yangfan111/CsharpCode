using System.Collections.Generic;
using System.Diagnostics;
using Core.EntityComponent;
using Core.Utils;
using Utils.Singleton;

namespace Core.Playback
{
    public class PlaybackIntroplateHandler : EntityMapDummyCompareHandler
    {
        private static LoggerAdapter _logger = new LoggerAdapter(LoggerNameHolder<PlaybackIntroplateHandler>.LoggerName);
        private EntityMap _localEntityMap;
        private IInterpolationInfo _interpolationInfo;
        private CustomProfileInfo _info;

        private List<PlayBackInfo> _playBackInfos;
        public PlaybackIntroplateHandler()
        {
            _info = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("Interpolate");
           
        }
#pragma warning disable RefCounter001,RefCounter002 // possible reference counter error
        public PlaybackIntroplateHandler init(EntityMap localEntityMap,
            IInterpolationInfo interpolationInfo, List<PlayBackInfo> playBackInfos)
        {
            _localEntityMap = localEntityMap;
            _interpolationInfo = interpolationInfo;
            _playBackInfos = playBackInfos;
            _playBackInfos.Clear();
            return this;
           
        }

      
       
        public override void OnDiffComponent(IGameEntity leftEntity, IGameComponent leftComponent, IGameEntity rightEntity, IGameComponent rightComponent)
        {
            IGameEntity localEntity;
            _localEntityMap.TryGetValue(leftEntity.EntityKey, out localEntity);
            if (localEntity != null) // entity存在，但是不是playback
            {
                var localComponent = localEntity.GetComponent(leftComponent.GetComponentId());
                if (localComponent != null)
                {  
                    var local = localComponent as IInterpolatableComponent;
                    if(local.IsInterpolateEveryFrame())
                    _playBackInfos.Add(new PlayBackInfo(localComponent, leftComponent, rightComponent));
                    try
                    {
                        _info.BeginProfileOnlyEnableProfile();
                        local.Interpolate(leftComponent, rightComponent,
                            _interpolationInfo);
                    }
                    finally
                    {
                        _info.EndProfileOnlyEnableProfile();
                    }
                    


                }
                else
                {
                    //_logger.WarnFormat("component is null while playback {0}:{1}", localEntity.EntityKey, leftComponent.GetComponentId());
                }
            }
        }

      

        public override bool IsExcludeComponent(IGameComponent component)
        {
            return !(component is IPlaybackComponent);
        }

       
    }
}