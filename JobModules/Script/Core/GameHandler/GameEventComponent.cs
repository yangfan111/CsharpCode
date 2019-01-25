using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.GameHandler
{
    public class GameEventComponent : DataBaseComponent
    {
        protected Queue<GameEvent> _eventQueue = new Queue<GameEvent>();
        protected static List<IGameEventHandler>[] _eventHandlers = new List<IGameEventHandler>[(int)GameEvent.MaxGameEventCount];

        public GameEventComponent()
        {

        }

        public void Enque(GameEvent evt)
        {
            _eventQueue.Enqueue(evt);
        }

        public GameEvent Deque()
        {
            if (_eventQueue.Count > 0)
            {
                return _eventQueue.Dequeue();
            }

            return GameEvent.None;
        }

        public static void RegisterHandler(GameEvent evt, IGameEventHandler handler)
        {
            List<IGameEventHandler> handlerList = _eventHandlers[(int) evt];
            if (handlerList == null)
            {
                handlerList  = new List<IGameEventHandler>();
                _eventHandlers[(int) evt] = handlerList;
            }

            handlerList.Add(handler);
        }

        public static List<IGameEventHandler> GetHandlerList(GameEvent evt)
        {
            return _eventHandlers[(int)evt];
        }
    }
}
