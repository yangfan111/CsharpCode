using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.GameHandler;
using Core.GameModule.Interface;
using Entitas;

namespace Core.GameModule.System
{
    public abstract class BaseGameStateUpdateSystem<TEntity> : IGameStateUpdateSystem where TEntity : Entity
    {
        protected abstract TEntity[] GetEntities();
        protected abstract GameEventComponent GetGameEventComponent(TEntity entity);
        protected abstract void CollectGameEvents(TEntity vehicle, List<GameEvent> outEvents);

        private List<IGameStateUpdateHandler> _updateHandlerList = new List<IGameStateUpdateHandler>();

        public BaseGameStateUpdateSystem(BaseGameHandlerRegister handlerRegister)
        {
            _updateHandlerList.AddRange(handlerRegister.GetGameStateUpdateHandlerList());
        }

        public void Update()
        {
            var entities = GetEntities();
            foreach (var entity in entities)
            {
                UpdateEntity(entity);
            }
        }

        private void UpdateEntity(TEntity entity)
        {
            foreach(var handler in _updateHandlerList)
            {
                handler.Update(entity);
            }
        }

        public void ProcessGameEvents()
        {
            var entities = GetEntities();
            foreach (var entity in entities)
            {
                var eventList = GetGameEventComponent(entity);
                GameEvent evt;
                while ((evt = eventList.Deque()) != GameEvent.None)
                {
                    ProcessGameEvent(entity, evt);
                }
            }
        }

        private void ProcessGameEvent(Entity entity, GameEvent evt)
        {
            var handlerList = GameEventComponent.GetHandlerList(evt);
            if (handlerList == null)
            {
                return;
            }

            foreach (var handler in handlerList)
            {
                handler.Handle(evt, entity);
            }
        }

        private List<GameEvent> _collectedEvents = new List<GameEvent>();
        public void SendGameEvents()
        {
            var entities = GetEntities();
            foreach (var entity in entities)
            {
                _collectedEvents.Clear();
                CollectGameEvents(entity, _collectedEvents);

                var eventList = GetGameEventComponent(entity);
                foreach (var evt in _collectedEvents)
                {
                    eventList.Enque(evt);
                }
            }
        }
 
    }
}
