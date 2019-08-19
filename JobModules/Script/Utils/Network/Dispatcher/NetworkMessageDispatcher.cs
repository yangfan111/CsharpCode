using System.Collections;
using System.Collections.Generic;
using Common;
using Core.ObjectPool;
using Core.Utils;
using Utils.Replay;
using NetworkMessageRecoder = Utils.Replay.NetworkMessageRecoder;

namespace Core.Network
{
    public partial class NetworkMessageDispatcher :INetworkMessageDispatcher
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(NetworkMessageDispatcher));
        private Dictionary<int, CompositeNetworkMessageHandler> _laterMessageType2Handler = new Dictionary<int, CompositeNetworkMessageHandler>();

        private Dictionary<int, CompositeNetworkMessageHandler> _immediateMessageType2Handler = new Dictionary<int, CompositeNetworkMessageHandler>();
        private Queue _queue = Queue.Synchronized(new Queue());
        private IRecordManager _record;


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

       

        public NetworkMessageDispatcher(IRecordManager record=null)
        {
            _record = record;
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
                else if(!_immediateMessageType2Handler.ContainsKey(item.MessageType))
                {
                    _logger.WarnFormat("unknow MessageType;{0}",item.MessageType );
                }

                if (_record != null)
                {
                    _record.AddMessage(EReplayMessageType.IN, NetworkMessageRecoder.RecodMessageItem.Allocate(item.Channel,
                        item.MessageType, item.MessageBody, MyGameTime.stage, MyGameTime.seq, NetworkMessageRecoder.ERecodMessagetype.UdpIn));
                }

                item.ReleaseReference();
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
           
            _queue.Enqueue(item);
           
        }

        class QueueItem:BaseRefCounter
        {
            public class ObjcetFactory :CustomAbstractObjectFactory
            {
                public ObjcetFactory() : base(typeof(QueueItem)){}
                public override object MakeObject()
                {
                    return new QueueItem();
                }
            }
            public static QueueItem Allocate(INetworkChannel channel, int messageType, object messageBody)
            {
                var rc = ObjectAllocatorHolder<QueueItem>.Allocate();
                rc.Channel = channel;
                rc.MessageBody = messageBody;
                rc.MessageType = messageType;
                if (rc.MessageBody != null && rc.MessageBody is IRefCounter)
                {
                    (rc.MessageBody as IRefCounter).AcquireReference();
                }
                return rc;
            }

            public override void AcquireReference()
            {
                if (MessageBody != null && MessageBody is IRefCounter)
                {
                    (MessageBody as IRefCounter).AcquireReference();
                }
                base.AcquireReference();
                
            }

            public override void ReleaseReference()
            {
                if (MessageBody != null && MessageBody is IRefCounter)
                {
                    (MessageBody as IRefCounter).ReleaseReference();
                }
                MessageBody = null;
                base.ReleaseReference();
            }

            private QueueItem()
            {
				
			}

            public INetworkChannel Channel;
            public int MessageType;
            public object MessageBody;

           

            protected override void OnCleanUp()
            {
                ObjectAllocatorHolder<QueueItem>.Free(this);
            }
        }
    }
}