using System.IO;
using Core.Event;
using Core.Utils;
using Entitas;

namespace App.Shared.Player.Events
{
    public abstract class DefaultEvent : IEvent
    {
        public abstract EEventType EventType { get; }


        public bool IsRemote { get; set; }

        public void ReadBody(BinaryReader reader)
        {
        }

        public void WriteBody(MyBinaryWriter writer)
        {
        }

        public void RewindTo(IEvent value)
        {
        }
    }
    public abstract class DefaultEventHandler:IEventHandler
    {
        public abstract EEventType EventType { get; }

        public IEvent MergeEvent(IEvent self, IEvent other)
        {
            return self;
        }

        public abstract void DoEventClient( Entitas.IContexts contexts, IEntity entity, IEvent e);

        public virtual void DoEventServer( Entitas.IContexts contexts, IEntity entity, IEvent e)
        {
            var playerEntity = entity as PlayerEntity;
            if (playerEntity != null)
            {
                var v = EventInfos.Instance.Allocate(e.EventType, true);
                v.RewindTo(e);
                playerEntity.remoteEvents.Events.AddEvent(v);
            }
        }

        public abstract bool ClientFilter(IEntity entity, IEvent e);
        public bool ServerFilter(IEntity entity, IEvent e)
        {
            return true;
        }
    
        
    }
}