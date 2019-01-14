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
    // left is local, right is base snapshot
    public class  PlaybackDiffHandler : IEntityMapCompareHandler
    {
        private static LoggerAdapter _logger = new LoggerAdapter(LoggerNameHolder<PlaybackDiffHandler>.LoggerName);


#pragma warning disable RefCounter001,RefCounter002 // possible reference counter error
        public PlaybackDiffHandler init()
        {

            return this;
        }

        public void OnLeftEntityMissing(IGameEntity rightEntity)
        {

        }

        public void OnLeftComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent rightComponent)
        {
            _logger.DebugFormat("add component {0}:{1}", leftEntity.EntityKey, rightComponent.GetType());
            leftEntity.AddComponent(rightComponent.GetComponentId());
        }

        public void OnDiffEntityFinish(IGameEntity leftEntity, IGameEntity rightEntity)
        {
            
        }

        public void OnDiffComponent(IGameEntity leftEntity, IGameComponent leftComponent, IGameEntity rightEntity, IGameComponent rightComponent)
        {
           

        }

        public void OnRightEntityMissing(IGameEntity leftEntity)
        {

        }

        public bool IsBreak()
        {
            return false;
        }

        public void OnRightComponentMissing(IGameEntity leftEntity, IGameEntity rightEntity, IGameComponent leftComponent)
        {
            _logger.DebugFormat("remove component {0}:{1}", leftEntity.EntityKey, leftComponent.GetType());
            leftEntity.RemoveComponent(leftComponent.GetComponentId());
        }

        public bool IsExcludeComponent(IGameComponent component)
        {
            return !(component is IPlaybackComponent);
        }

        public void OnDiffEntityStart(IGameEntity leftEntity, IGameEntity rightEntity)
        {
            
        }
    }
}