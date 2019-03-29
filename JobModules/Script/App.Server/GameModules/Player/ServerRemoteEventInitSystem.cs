using Core.GameModule.Interface;
using Core.Utils;
using Entitas;

namespace App.Server.GameModules.Player
{
    public class ServerRemoteEventInitSystem:IEntityInitSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ServerRemoteEventInitSystem));
        private IGroup<PlayerEntity> _group;
        private Contexts _contexts;
        public ServerRemoteEventInitSystem(Contexts contexts)
        {
            _group = contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.RemoteEvents));
            _contexts = contexts;
        }
        public void OnEntityInit()
        {
            foreach (var playerEntity in _group.GetEntities())
            {
                //Logger.InfoFormat("player:{0} remoteEvents reinit", playerEntity.entityKey.Value);
                playerEntity.remoteEvents.Events.ReInit();
                playerEntity.remoteEvents.Events.ServerTime = _contexts.session.currentTimeObject.CurrentTime;
            }
        }
    }
}