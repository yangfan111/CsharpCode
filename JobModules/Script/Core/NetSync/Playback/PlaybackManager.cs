using System.Collections.Generic;
using Core.EntityComponent;
using Core.Replicaton;
using Core.Utils;
using Utils.Singleton;

namespace Core.Playback
{
    /// <summary>
    /// 获取FlagNoneSelfComponent的Entity
    /// 通过当前服务器时间获取时间节点两端的SnapShot，对当前Component进行Plackback操作
    /// </summary>
    public class PlaybackManager
    {
        //Compare
        private GameEntityPlayBackCompareAgent compareAgent = new GameEntityPlayBackCompareAgent();
        private PlaybackMapDiffHandler diffHandler = new PlaybackMapDiffHandler();

        private PlaybackInfoProvider infoProvider;

        private GameEntityPlayBackInterpolateCompareAgent interpolateCompareAgent =
                        new GameEntityPlayBackInterpolateCompareAgent();

        private PlaybackLMapIntroplateDiffHandler introplateDiffHandler = new PlaybackLMapIntroplateDiffHandler();
        private int lastLeftPlaybackSnapshotServerTime = -1;

        private List<PlayBackInfo> playBackInfos = new List<PlayBackInfo>();

        public PlaybackManager(PlaybackInfoProvider infoProvider)
        {
            this.infoProvider = infoProvider;
        }

        public void DoPlaybackInit()
        {
            if (!infoProvider.IsReady())
                return;
            var remoteEntityMapLeft  = infoProvider.RemoteLeftEntityMap;
            var remoteEntityMapRight = infoProvider.RemoteRightEntityMap;

            var localEntityMap = infoProvider.LocalAllEntityMap;


            if (lastLeftPlaybackSnapshotServerTime != infoProvider.InterpolationInfo.LeftServerTime)
            {
                //正常状态下几乎每帧都在做
                PlayBackInit(localEntityMap, remoteEntityMapLeft);
                PlayBackInterpolationAll(localEntityMap, remoteEntityMapLeft, remoteEntityMapRight);
            }
            else
            {
                PlayBackInterpolationEvertFrame();
            }
        }

        private void PlayBackInterpolationEvertFrame()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.PlaybackInit2);
                var interpolationInfo = infoProvider.InterpolationInfo;
                foreach (var playBackInfo in playBackInfos)
                {
                    ((IInterpolatableComponent) playBackInfo.LocalComponent).Interpolate(playBackInfo.LeftComponent,
                        playBackInfo.RightComponent, interpolationInfo);
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
                PlaybackLMapIntroplateDiffHandler interpolateDiffHandler = introplateDiffHandler.init(localEntityMap,
                    infoProvider.InterpolationInfo, playBackInfos);

                EntityMapCompareExecutor.Diff(remoteEntityMapLeft, remoteEntityMapRight, interpolateDiffHandler,
                    "playbackInterpolate",
                    interpolateCompareAgent.Init(interpolateDiffHandler, infoProvider.InterpolationInfo,
                        localEntityMap));
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.PlaybackInit2);
            }
        }

        private void PlayBackInit(EntityMap localEntityMap, EntityMap remoteEntityMapLeft)
        {
            diffHandler.init();

            var localEntityMapClone = PlayBackEntityMap.Allocate(false);
            localEntityMapClone.AddAll(infoProvider.LocalEntityMap);
            lastLeftPlaybackSnapshotServerTime = infoProvider.InterpolationInfo.LeftServerTime;
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.PlaybackInit1);
                EntityMapCompareExecutor.Diff(localEntityMapClone, remoteEntityMapLeft, diffHandler, "playbackInit",
                    compareAgent.Init(diffHandler, infoProvider.InterpolationInfo, localEntityMap));
                RefCounterRecycler.Instance.ReleaseReference(localEntityMapClone);
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.PlaybackInit1);
            }
        }
    }
}