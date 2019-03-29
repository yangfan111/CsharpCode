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

        private Dictionary<EEventType, List<IEvent>>
            _events = new Dictionary<EEventType, List<IEvent>>(EEventTypeComparer.Instance);

        public PlayerEvents()
        {
            foreach(EEventType etype in Enum.GetValues(typeof(EEventType)))
            {
                _events.Add(etype, new List<IEvent>());
            }
        }

        public Dictionary<EEventType,List<IEvent>> Events
        {
            get { return _events; }
        }

        public void AddEvent(IEvent e)
        {
           
            _events[e.EventType].Add(e);
           
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
                foreach(var vv in v)
                {
                  
                    EventInfos.Instance.Free(vv);
              
                }
                v.Clear();
            }
          
        }

        public void CopyFrom(object rightComponent)
        {
            ReInit();
            var right = rightComponent as PlayerEvents;
            foreach (var rightKeyPair in right._events)
            {

                _events[rightKeyPair.Key].Clear();
                foreach (var node in rightKeyPair.Value)
                {
                    var v = EventInfos.Instance.Allocate(rightKeyPair.Key, node.IsRemote);
                    v.RewindTo(node);
                    _events[rightKeyPair.Key].Add(node);
                }
         
            }

            ServerTime = right.ServerTime;
            HasHandler = right.HasHandler;
        }

       

        public void Write(MyBinaryWriter writer)
        {
            AssertUtility.Assert((int) EEventType.End < 255);
          
            writer.Write((byte) _events.Count);
            foreach (KeyValuePair<EEventType, List<IEvent>> eventListPair in _events)
            {
                writer.Write((byte)eventListPair.Key);
                writer.Write(eventListPair.Value.Count);
                foreach(var node in eventListPair.Value)
                {
                    node.WriteBody(writer);
                }
            }

        }

        public void Read(BinaryReader reader)
        {
            ReInit();
            AssertUtility.Assert((int) EEventType.End < 255);
          
            int count = reader.ReadByte();
           
            for (int i = 0; i < count; i++)
            {
                var type = (EEventType)reader.ReadByte();
                _events[type].Clear();
                
                var length = reader.ReadInt32();
                for(int j=0;j<length;j++)
                {
                    var v = EventInfos.Instance.Allocate(type, true);
                    v.ReadBody(reader);
                    _events[type].Add(v);
                }

            }
        }
    }
}