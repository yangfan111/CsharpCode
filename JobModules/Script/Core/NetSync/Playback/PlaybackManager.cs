using System.Collections.Generic;
using Core.EntityComponent;
using Core.Replicaton;
using Core.Utils;
using Utils.Singleton;

namespace Core.Playback
{
    public class PlaybackManager : IPlaybackManager
    {
        private IPlaybackInfoProvider _infoProvider;
        private int _lastLeftPlayBackSnapshot = -1;
        private GameEntityPlayBackCompareAgent _compareAgent = new GameEntityPlayBackCompareAgent();

        private GameEntityPlayBackInterpolateCompareAgent _interpolateCompareAgent =
            new GameEntityPlayBackInterpolateCompareAgent();

        private List<PlayBackInfo> _playBackInfos = new List<PlayBackInfo>();
        private PlaybackMapDiffHandler _diffHandler = new PlaybackMapDiffHandler();
        private PlaybackLMapIntroplateDiffHandler _introplateDiffHandler = new PlaybackLMapIntroplateDiffHandler();

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
                PlaybackLMapIntroplateDiffHandler interpolateDiffHandler = _introplateDiffHandler.init(localEntityMap,
                    _infoProvider.InterpolationInfo, _playBackInfos);

                EntityMapCompareExecutor.Diff(remoteEntityMapLeft, remoteEntityMapRight, interpolateDiffHandler,
                    "playbackInterpolate",
                    _interpolateCompareAgent.Init(interpolateDiffHandler, _infoProvider.InterpolationInfo,
                        localEntityMap));
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.PlaybackInit2);
            }
        }

        private void PlayBackInit(EntityMap localEntityMap, EntityMap remoteEntityMapLeft)
        {
            PlaybackMapDiffHandler diffHandler = _diffHandler.init();

            var localEntityMapClone = PlayBackEntityMap.Allocate(false);
            localEntityMapClone.AddAll(_infoProvider.LocalEntityMap);
            _lastLeftPlayBackSnapshot = _infoProvider.InterpolationInfo.LeftServerTime;
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.PlaybackInit1);
                EntityMapCompareExecutor.Diff(localEntityMapClone, remoteEntityMapLeft, diffHandler,
                    "playbackInit",
                    _compareAgent.Init(diffHandler, _infoProvider.InterpolationInfo, localEntityMap));
                 RefCounterRecycler.Instance.ReleaseReference(localEntityMapClone);
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.PlaybackInit1);
            }
        }
    }
}