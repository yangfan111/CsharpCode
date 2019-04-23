using System;
using System.Collections;
using System.Collections.Generic;
using Core.ObjectPool;
using Core.Utils;

namespace Core.Network
{
    public class NetworkMessageDispatcher 
    {
        class CompositeNetworkMessageHandler : INetworkMessageHandler
        {
            private List<INetworkMessageHandler> _handlers = new List<INetworkMessageHandler>();

            public void Register(INetworkMessageHandler handler)
            {
                _handlers.Add(handler);
            }

            public void Handle(INetworkChannel networkChannel, int messageType, object messageBody)
            {
                foreach (var handler in _handlers)
                {
                    handler.Handle(networkChannel, messageType, messageBody);
                }
            }

            public void UnRegister(INetworkMessageHandler handler)
            {
                _handlers.Remove(handler);
            }
        }
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(NetworkMessageDispatcher));
        private Dictionary<int, CompositeNetworkMessageHandler> _laterMessageType2Handler = new Dictionary<int, CompositeNetworkMessageHandler>();

        private Dictionary<int, CompositeNetworkMessageHandler> _immediateMessageType2Handler = new Dictionary<int, CompositeNetworkMessageHandler>();
        private Queue _queue = Queue.Synchronized(new Queue());


        
        public void RegisterLater(int messageType, INetworkMessageHandler handler)
        {
            
            CompositeNetworkMessageHandler handlers;
            _laterMessageType2Handler.TryGetValue(messageType, out handlers);
            if (handlers == null)
            {
                handlers = new CompositeNetworkMessageHandler();
                _laterMessageType2Handler[messageType] = handlers;
            }
            handlers.Register(handler);
        }

        public void RegisterImmediate(int messageType, INetworkMessageHandler handler)
        {
            CompositeNetworkMessageHandler handlers;
            _immediateMessageType2Handler.TryGetValue(messageType, out handlers);
            if (handlers == null)
            {
                handlers = new CompositeNetworkMessageHandler();
                _immediateMessageType2Handler[messageType] = handlers;
            }
            handlers.Register(handler);
        }



        public void DriveDispatch()
        {
            while (_queue.Count > 0)
            {
                QueueItem item = (QueueItem)_queue.Dequeue();
                CompositeNetworkMessageHandler handler;
                _laterMessageType2Handler.TryGetValue(item.MessageType, out handler);
                if (handler != null)
                {
                    handler.Handle(item.Channel, item.MessageType, item.MessageBody);
                }
                
                QueueItem.Free(item);
            }
        }

        public void SaveDispatch(INetworkChannel channel, int messageType, object messageBody)
        {
           
            CompositeNetworkMessageHandler handlers;
            if (_immediateMessageType2Handler.TryGetValue(messageType, out handlers))
            {
                handlers.Handle(channel, messageType, messageBody);
            }
            var item = QueueItem.Allocate(channel, messageType, messageBody);
            item.RefMessageBody();
            _queue.Enqueue(item);
           
        }

        class QueueItem
        {
            public static QueueItem Allocate(INetworkChannel channel, int messageType, object messageBody)
            {
                var rc = ObjectAllocatorHolder<QueueItem>.Allocate();
                rc.Channel = channel;
                rc.MessageBody = messageBody;
                rc.MessageType = messageType;
                return rc;
            }
            public static void Free(QueueItem item)
            {
	            item.ReleaseMessageBody();
				ObjectAllocatorHolder<QueueItem>.Free(item);
                
            }
            private QueueItem()
            {
				
			}

            public INetworkChannel Channel;
            public int MessageType;
            public object MessageBody;

            public void RefMessageBody()
            {
                if (MessageBody != null && MessageBody is IRefCounter)
                {
                    (MessageBody as IRefCounter).AcquireReference();
                }
            }

            public void ReleaseMessageBody()
            {
                if (MessageBody != null && MessageBody is IRefCounter)
                {
                    (MessageBody as IRefCounter).ReleaseReference();
                }
                MessageBody = null;
            }
        }
    }

    public interface INetworkMessageHandler 
    {
        void Handle(INetworkChannel networkChannel, int messageType, object messageBody);
    }

    public class DelegateNetworkMessageHandler: INetworkMessageHandler
    {
        private Action<INetworkChannel, int, object> _target;

        public DelegateNetworkMessageHandler(Action<INetworkChannel, int, object> target)
        {
            _target = target;
        }

        public void Handle(INetworkChannel networkChannel, int messageType, object messageBody)
        {
            _target.Invoke(networkChannel, messageType, messageBody);
        }
    }
}