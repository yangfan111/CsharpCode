using System.IO;
using Core.Network.ENet;
using Core.ObjectPool;

namespace VNet
{
    internal class VNetworkMessageItem: NetworkMessageItem
    {
        private MemoryStream _stream;

        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(VNetworkMessageItem)){}
            public override object MakeObject()
            {
                return new VNetworkMessageItem();
            }

            public override int InitPoolSize
            {
                get { return 256; }
            }

            public override int AllocatorNumber
            {
                get { return 64; }
            }
        }
        private VNetworkMessageItem() 
        {
           _stream = new MemoryStream(1024*10);
        }
        public static VNetworkMessageItem Allocate(int messageType, object messageBody, int channel)
        {
            VNetworkMessageItem rc = ObjectAllocatorHolder<VNetworkMessageItem>.Allocate();
            rc.Init(messageType, messageBody, channel);
            return rc;
        }
        public void Init(int messageType, object messageBody, int channel)
        {
            _stream.Seek(0, SeekOrigin.Begin);
            _stream.SetLength(0);
            Channel = channel;
            MessageType = messageType;
            MessageBody = messageBody;
            if (MessageBody != null && MessageBody is IRefCounter)
            {
                (MessageBody as IRefCounter).AcquireReference();
            }
        }

        public int Channel { get; set; }
        protected override void OnCleanUp()
        {
            this.MessageBody = null;
            ObjectAllocatorHolder<VNetworkMessageItem>.Free(this);
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
            base.ReleaseReference();
           
            
        }

        public override MemoryStream MemoryStream
        {
            get
            {
                return _stream;
            }
        }
    }
}
