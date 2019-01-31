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
        JointRoomList,
        JoinRoomListResponse,
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

        public bool IsDisposed;

        private IObjectAllocator _allocator;

        public virtual void Reset()
        {
            IsDisposed = false;
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
                throw new Exception(String.Format("The room event {0} {1} is not allocated from RoomEvent.Allocate<T>!",  e.EventType,  e.GetType()));
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


        protected static T[] ChangeReferenceValue<T>(T[] originVal, T[] newValue) where T : BaseRefCounter
        {

            if (newValue != null)
            {
                foreach (var newRef in newValue)
                {
                    if(newRef !=  null)
                        newRef.AcquireReference();
                }
                
            }

            if (originVal != null)
            {
                foreach (var orgRef in originVal)
                {
                    if(orgRef != null)
                        orgRef.ReleaseReference();
                }
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
        public event Func<RoomEvent, bool> Filter; 

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
                        if (!Filter(e))
                        {
                            if(!e.IsDisposed)
                                OnRoomEvent(e);

                            RoomEvent.FreeEvent(e);
                        }
                        else
                        {
                            if (!e.IsDisposed)
                            {
                                _eventList.Add(e);
                            }
                            else
                            {
                                RoomEvent.FreeEvent(e);
                            }                  
                        }
                    }
                    catch (Exception exception)
                    {
                       _logger.ErrorFormat("Room Event {0}", exception);
                    }
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
