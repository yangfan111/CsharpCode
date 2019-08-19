using System.Collections.Generic;
using Core.EntityComponent;
using Core.Utils;

namespace Core.Playback
{
    public struct PlayBackInfo
    {
        public IGameComponent LocalComponent;
        public IGameComponent LeftComponent;
        public IGameComponent RightComponent;

        public PlayBackInfo(IGameComponent localComponent, IGameComponent leftComponent, IGameComponent rightComponent)
        {
            RightComponent = rightComponent;
            LeftComponent = leftComponent;
            LocalComponent = localComponent;
        }
    }
    /// <summary>
    /// left is local, right is base snapshot
    /// 回放：移除左边的组件
    /// </summary>
    public class  PlaybackMapDiffHandler : EntityMapDiffHandlerAdapter
    {
        private static LoggerAdapter _logger = new LoggerAdapter(LoggerNameHolder<PlaybackMapDiffHandler>.LoggerName);


#pragma warning disable RefCounter001,RefCounter002 // possible reference counter error
        public PlaybackMapDiffHandler init()
        {

            return this;
        }

        public override void OnLeftComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent rightComponent)
        {
            _logger.DebugFormat("add component {0}:{1}", leftEntity.EntityKey, rightComponent.GetType());
            leftEntity.AddComponent(rightComponent.GetComponentId());
        }

     
        public override void OnRightComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent leftComponent)
        {
            _logger.DebugFormat("remove component {0}:{1}", leftEntity.EntityKey, leftComponent.GetType());
            leftEntity.RemoveComponent(leftComponent.GetComponentId());
        }
        /// <summary>
        /// 是否是需要比较的Component
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool IsExcludeComponent(IGameComponent component)
        {
            return !(component is IPlaybackComponent);
        }
    }
}