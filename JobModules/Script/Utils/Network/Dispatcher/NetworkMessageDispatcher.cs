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
        private CompositeNetworkMessageHandler[] laterMessageType2Handlers;
        private CompositeNetworkMessageHandler[] immediateMessageType2Handler;

        private Queue _queue = Queue.Synchronized(new Queue());
        private IRecordManager _record;
        private int msgTypeLength;


        public void RegisterLater(int messageType, INetworkMessageHandler handler)
        {
            var handlers = laterMessageType2Handlers[messageType];
            if (handlers == null)
            {
                handlers = new CompositeNetworkMessageHandler();
                laterMessageType2Handlers[messageType] = handlers;
            }
            handlers.Register(handler);
        }

        public void RegisterImmediate(int messageType, INetworkMessageHandler handler)
        {
            var handlers = immediateMessageType2Handler[messageType];
            if (handlers == null)
            {
                handlers                               = new CompositeNetworkMessageHandler();
                laterMessageType2Handlers[messageType] = handlers;
            }
            handlers.Register(handler);
        }

       

        public NetworkMessageDispatcher(int msgTypeLength, IRecordManager record=null)
        {
            this.msgTypeLength = msgTypeLength;
            laterMessageType2Handlers = new CompositeNetworkMessageHandler[msgTypeLength];
            immediateMessageType2Handler = new CompositeNetworkMessageHandler[msgTypeLength];

            _record = record;
        }

        public void DriveDispatch()
        {
            while (_queue.Count > 0)
            {
                QueueItem item = (QueueItem)_queue.Dequeue();
                
                CompositeNetworkMessageHandler handler;
                handler = laterMessageType2Handlers[item.MessageType];
                if (handler != null)
                {
                    handler.Handle(item.Channel, item.MessageType, item.MessageBody);
                }
                else 
                {
                    handler = immediateMessageType2Handler[item.MessageType];
                    if(handler == null)
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

            var handlers = immediateMessageType2Handler[messageType];
            handlers.Handle(channel, messageType, messageBody);
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