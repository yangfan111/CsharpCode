using App.Client.GameModules.Player;
using Core.Utils;
using Entitas;

namespace App.Shared.GameModules.Player.Event
{
    public class RemoteEventPlaySystem:AbstractGamePlaySystem<PlayerEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(RemoteEventPlaySystem));
        private bool _isServer;
        private Contexts _contexts;
        public RemoteEventPlaySystem(Contexts contexts, bool isServer) : base(contexts)
        {
            _contexts = contexts;
            _isServer = isServer;
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.RemoteEvents, PlayerMatcher.Position));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return entity.remoteEvents.Events.Count > 0;
        }

        protected override void OnGamePlay(PlayerEntity entity)
        {
            if (!entity.remoteEvents.Events.HasHandler)
            {
                entity.remoteEvents.Events.DoAllEvent(_contexts, entity, false);
                entity.remoteEvents.Events.HasHandler = true;
                _logger.DebugFormat("OnGamePlay :{0} {1}",  entity.remoteEvents.Events.ServerTime, entity.remoteEvents.Events.Count);
            }
        }
    }
    
}