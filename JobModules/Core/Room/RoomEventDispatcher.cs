using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.ObjectPool;
using Core.Utils;

namespace Core.Room
{
    public enum ERoomEventType
    {
        Invalid = 0,
        HallServerConnect,
        LoginServer,
        PlayerLogin,
        CreateRoom,
        CreateRoomResponse,
        JoinRoom,
        JoinRoomResponse,
        MandatoryLogOut,
        UpdateRoomStatus,
        UpdatePlayerStatus,
        GameOver,
        GameOverMessage,
        LeaveRoom,

        SetRoomStatus,
    }

    public abstract class RoomEvent 
    {
        public ERoomEventType EventType;

        private IObjectAllocator _allocator;

        public virtual void Reset()
        {
            _allocator = null;
        }

        public static T AllocEvent<T>() where T :RoomEvent
        {
            var evt = ObjectAllocatorHolder<T>.Allocate();
            evt._allocator = ObjectAllocatorHolder<T>.GetAllocator();
            return evt;
        }

        public static void FreeEvent<T>(T e) where T : RoomEvent
        {
            var allocator = e._allocator;
            if (allocator == null)
            {
                throw new Exception("The room event is not allocated from RoomEvent.Allocate<T>!");
            }

            e.Reset();
            allocator.Free(e);
        }

#pragma warning disable RefCounter001, RefCounter002
        protected static T ChangeReferenceValue<T>(T originVal, T newValue) where T : BaseRefCounter
        {

            if (newValue != null)
            {
                newValue.AcquireReference();
            }

            if (originVal != null)
            {
                originVal.ReleaseReference();
            }

            return newValue;
        }
#pragma warning restore RefCounter001, RefCounter002 

    }

    public class RoomEventDispatcher : IRoomEventDispatchter
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(RoomEventDispatcher));

        private List<RoomEvent> _eventList = new List<RoomEvent>(); 
        private List<RoomEvent> _execList = new List<RoomEvent>();

        public event Action<RoomEvent> OnRoomEvent;

        public void Update()
        {
            if (_eventList.Count > 0)
            {
                _execList.AddRange(_eventList);
                _eventList.Clear();
                foreach (var e in _execList)
                {
                    try
                    {
                        OnRoomEvent(e);
                    }
                    catch (Exception exception)
                    {
                       _logger.ErrorFormat("Room Event {0}", exception);
                    }

                    RoomEvent.FreeEvent(e);
                }

                _execList.Clear();
            }
        }

        public void AddEvent(RoomEvent e)
        {
            _eventList.Add(e);
        }
    }
}
