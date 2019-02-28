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

        private Dictionary<EEventType, IEvent>
            _events = new Dictionary<EEventType, IEvent>(EEventTypeComparer.Instance);

        public Dictionary<EEventType, IEvent> Events
        {
            get { return _events; }
        }

        public void AddEvent(IEvent e)
        {
           
            if (_events.ContainsKey(e.EventType))
            {
            }
            else
            {
                _events.Add(e.EventType, e);
            }
        }

        public int Count
        {
            get { return _events.Count; }
        }

        public void ReInit()
        {
            ServerTime = -1;
            HasHandler = false;
            foreach (var v in _events.Values)
            {
                EventInfos.Instance.Free(v);
            }

            _events.Clear();
        }

        public void CopyFrom(object rightComponent)
        {
            ReInit();
            var right = rightComponent as PlayerEvents;
            foreach (var keyValuePair in right._events)
            {
                var v = EventInfos.Instance.Allocate(keyValuePair.Key, keyValuePair.Value.IsRemote);
                v.RewindTo(keyValuePair.Value);
                _events[v.EventType] = v;
            }

            ServerTime = right.ServerTime;
            HasHandler = right.HasHandler;
        }

       

        public void Write(MyBinaryWriter writer)
        {
            AssertUtility.Assert((int) EEventType.End < 255);
           
            writer.Write((byte) _events.Count);
            foreach (var v in _events.Values)
            {
                writer.Write((byte) v.EventType);
                v.WriteBody(writer);
            }
        }

        public void Read(BinaryReader reader)
        {
            AssertUtility.Assert((int) EEventType.End < 255);
          
            int count = reader.ReadByte();
            for (int i = 0; i < count; i++)
            {
                var type = (EEventType) reader.ReadByte();
                var v = EventInfos.Instance.Allocate(type, true);
                v.ReadBody(reader);
                _events.Add(v.EventType, v);
            }
        }
    }
}