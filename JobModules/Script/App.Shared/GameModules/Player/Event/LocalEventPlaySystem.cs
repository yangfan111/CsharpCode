using System;
using App.Client.GameModules.Player;
using Core.Event;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;

namespace App.Shared.GameModules.Player.Event
{
    public class LocalEventPlaySystem:AbstractGamePlaySystem<PlayerEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(LocalEventPlaySystem));
        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.LocalEvents, PlayerMatcher.Position));
        }

        private bool _isServer;
        protected override bool Filter(PlayerEntity entity)
        {
            return entity.localEvents.Events.Count > 0;
        }

        protected override void OnGamePlay(PlayerEntity entity)
        {
            entity.localEvents.Events.DoAllEvent(_contexts, entity, _isServer);
            entity.localEvents.Events.ReInit();
        }

        private readonly Contexts _contexts;
        public LocalEventPlaySystem(Contexts contexts, bool isServer) : base(contexts)
        {
            _contexts = contexts;
            _isServer = isServer;
        }

    }
}