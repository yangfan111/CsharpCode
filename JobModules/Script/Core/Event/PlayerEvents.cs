using System;
using System.Collections.Generic;
using System.IO;
using Core.ObjectPool;
using Core.SnapshotReplication.Serialization.NetworkObject;
using Core.Utils;

namespace Core.Event
{
    public class PlayerEvents : IReusableObject, INetworkObject
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerEvents));
        public int ServerTime = 0;
        public bool HasHandler;

        private List<IEvent>
            _events = new List<IEvent>();


        public PlayerEvents()
        {
        }

        public void GetEvents(EEventType etype, List<IEvent> events)
        {
            foreach (var @event in _events)
            {
                if (@event.EventType == etype)
                {
                    events.Add(@event);
                }
            }
        }

        public void AddEvent(IEvent e)
        {
            _events.Add(e);
        }

        public int Count
        {
            get { return _events.Count; }
        }

        public List<IEvent> Events
        {
            get { return _events; }
        }

        public void ReInit()
        {
            
            ServerTime = -1;
            HasHandler = false;

            foreach (var v in _events)
            {
                EventInfos.Instance.Free(v);
            }

            _events.Clear();
        }

        public void CopyFrom(object rightComponent)
        {
            ReInit();
            var right = rightComponent as PlayerEvents;
            int c = right._events.Count;
            for (var i = 0; i < c; i++)
            {
                var node = right._events[i];
                var v = EventInfos.Instance.Allocate(node.EventType, node.IsRemote);
                v.RewindTo(node);
                _events.Add(v);
            }


            ServerTime = right.ServerTime;
            HasHandler = right.HasHandler;
        }


        public void Write(MyBinaryWriter writer)
        {
            AssertUtility.Assert((int) EEventType.End < 255);
            int c = _events.Count;
            writer.Write((short)c);
            for (var i = 0; i < c; i++)
            {
                var node = _events[i];
                writer.Write((byte) node.EventType);
                node.WriteBody(writer);
            }
        }

        private int EEventsComparer(IEvent x, IEvent y)
        {
            return (int) (y.EventType) - (int) (y.EventType);
        }

        public void Read(BinaryReader reader)
        {
            ReInit();
            AssertUtility.Assert((int) EEventType.End < 255);

            short count = reader.ReadInt16();

            for (int i = 0; i < count; i++)
            {
                var type = (EEventType) reader.ReadByte();

                var v = EventInfos.Instance.Allocate(type, true);
                v.ReadBody(reader);
                _events.Add(v);
            }
        }

        public IEvent GetFirstEvent(EEventType eEventType)
        {
            foreach (var @event in _events)
            {
                if (@event.EventType == eEventType)
                {
                    return @event;
                }
            }

            return null;
        }
    }
}