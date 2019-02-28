using System.Collections.Generic;
using System.IO;
using Core.Utils;
using Entitas;

namespace Core.Event
{
    public class EEventTypeComparer : IEqualityComparer<EEventType>
    {
        public bool Equals(EEventType x, EEventType y)
        {
            return x == y;
        }

        public int GetHashCode(EEventType obj)
        {
            return (int)obj;
        }

        private static EEventTypeComparer _instance = new EEventTypeComparer();
        public static EEventTypeComparer Instance
        {
            get
            {
                return _instance;
            }
        }



    }
    public enum EEventType
    {
        Fire,
        PullBolt,
        HitVehicle,
        HitPlayer,
        BeenHit,
        HitEnvironment,
        End,


       
    }
    public interface IEvent
    {
        EEventType EventType { get; }
        bool IsRemote { get; set; }
        void ReadBody(BinaryReader reader);
        void WriteBody(MyBinaryWriter writer);
        void RewindTo(IEvent value);
    }
    public interface IEventInfos
    {
        IEvent Allocate(EEventType eEventType, bool isRemote);
        void Free(IEvent eEventType);
        IEventHandler GetEventHandler(EEventType eEventType);
    }
    
    public interface IEventHandler
    {
        /// <summary>
        /// 通过EventType和IEvent关联
        /// </summary>
        EEventType EventType { get; }
        IEvent MergeEvent(IEvent self, IEvent other);

        void DoEventClient( Entitas.IContexts contexts, IEntity entity, IEvent e);
        void DoEventServer( Entitas.IContexts contexts, IEntity entity, IEvent e);
        bool ClientFilter(IEntity entity, IEvent e);
        bool ServerFilter(IEntity entity, IEvent e);
    }
}