using System;
using System.Collections;
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
        HallServerDisconnect,
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
        GameExit,
        RoomDisposed,
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

    public class RoomEventArg
    {
        private bool _filtered;

        public RoomEvent Event;

        public bool Filtered
        {
            get { return _filtered; }
            set { _filtered = value || _filtered; }
        }

        public void ResetEvent(RoomEvent e)
        {
            Event = e;
            _filtered = false;
        }

    }

    public class RoomEventDispatcher : IRoomEventDispatchter
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(RoomEventDispatcher));

        private Queue[] _eventBuffers = new Queue[2]
        {
            Queue.Synchronized(new Queue()),
            Queue.Synchronized(new Queue())
        };

        private volatile int _activeBufferIndex = 0;

        public event Action<RoomEvent> OnRoomEvent;
        public event Action<RoomEventArg> Intercept; 

        private RoomEventArg _eventArg = new RoomEventArg();

        public void Update()
        {
            var activeBuffer = _eventBuffers[_activeBufferIndex];
            ChangeEventBuffer();
            while (activeBuffer.Count > 0)
            {
                var e = activeBuffer.Dequeue() as RoomEvent;
                try
                {
                    _eventArg.ResetEvent(e);

                    if (Intercept != null)
                        Intercept(_eventArg);

                    if (!_eventArg.Filtered)
                    {
                        if (!e.IsDisposed)
                            OnRoomEvent(e);

                        RoomEvent.FreeEvent(e);
                    }
                    else
                    {
                        if (!e.IsDisposed)
                        {
                            AddEvent(e);
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
        }

        private void ChangeEventBuffer()
        {
            int activeBufferIndex = _activeBufferIndex == 0 ? 1 : 0;

            _activeBufferIndex = activeBufferIndex;
        }

        public void AddEvent(RoomEvent e)
        {
            _eventBuffers[_activeBufferIndex].Enqueue(e);
        }
    }
}
