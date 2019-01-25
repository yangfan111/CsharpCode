using System.Collections.Generic;
using Core.EntityComponent;
using Core.Utils;
using Utils.Singleton;

namespace Core.Playback
{
    public class PlaybackManager : IPlaybackManager
    {
        private IPlaybackInfoProvider _infoProvider;
        private int _lastLeftPlayBackSnapshot = -1;
        private GameEntityPlayBackDiffComparator _diffComparator = new GameEntityPlayBackDiffComparator();

        private GameEntityPlayBackInterpolateComparator _interpolateComparator =
            new GameEntityPlayBackInterpolateComparator();

        private List<PlayBackInfo> _playBackInfos = new List<PlayBackInfo>();
        private PlaybackDiffHandler _diffDiffHandler = new PlaybackDiffHandler();
        private PlaybackIntroplateHandler _introplateHandler = new PlaybackIntroplateHandler();

        public PlaybackManager(IPlaybackInfoProvider infoProvider)
        {
            _infoProvider = infoProvider;
        }

        public void Playback()
        {
            if (_infoProvider.IsReady())
            {
                var remoteEntityMapLeft = _infoProvider.RemoteLeftEntityMap;
                var remoteEntityMapRight = _infoProvider.RemoteRightEntityMap;

                var localEntityMap = _infoProvider.LocalAllEntityMap;


                if (_lastLeftPlayBackSnapshot != _infoProvider.InterpolationInfo.LeftServerTime)
                {
                    PlayBackInit(localEntityMap, remoteEntityMapLeft);

                    PlayBackInterpolationAll(localEntityMap, remoteEntityMapLeft, remoteEntityMapRight);
                }
                else
                {
                    PlayBackInterpolationEvertFrame();
                }
            }
        }

        private void PlayBackInterpolationEvertFrame()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.PlaybackInit2);
                var interpolationInfo = _infoProvider.InterpolationInfo;
                foreach (var playBackInfo in _playBackInfos)
                {
                    ((IInterpolatableComponent) playBackInfo.LocalComponent).Interpolate(
                        playBackInfo.LeftComponent, playBackInfo.RightComponent, interpolationInfo);
                }
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.PlaybackInit2);
            }
           
        }

        private void PlayBackInterpolationAll(EntityMap localEntityMap, EntityMap remoteEntityMapLeft,
            EntityMap remoteEntityMapRight)
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.PlaybackInit2);
                PlaybackIntroplateHandler interpolateHandler = _introplateHandler.init(localEntityMap,
                    _infoProvider.InterpolationInfo, _playBackInfos);

                EntityMapComparator.Diff(remoteEntityMapLeft, remoteEntityMapRight, interpolateHandler,
                    "playbackInterpolate",
                    _interpolateComparator.Init(interpolateHandler, _infoProvider.InterpolationInfo,
                        localEntityMap));
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.PlaybackInit2);
            }
        }

        private void PlayBackInit(EntityMap localEntityMap, EntityMap remoteEntityMapLeft)
        {
            PlaybackDiffHandler diffDiffHandler = _diffDiffHandler.init();

            var localEntityMapClone = PlayBackEntityMap.Allocate(false);
            localEntityMapClone.AddAll(_infoProvider.LocalEntityMap);
            _lastLeftPlayBackSnapshot = _infoProvider.InterpolationInfo.LeftServerTime;
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.PlaybackInit1);
                EntityMapComparator.Diff(localEntityMapClone, remoteEntityMapLeft, diffDiffHandler,
                    "playbackInit",
                    _diffComparator.Init(diffDiffHandler, _infoProvider.InterpolationInfo, localEntityMap));
                localEntityMapClone.ReleaseReference();
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.PlaybackInit1);
            }
        }
    }
}