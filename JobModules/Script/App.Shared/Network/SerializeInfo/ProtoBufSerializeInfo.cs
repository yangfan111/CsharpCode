using System.IO;
using Core.Network;
using Core.ObjectPool;
using Google.Protobuf;

namespace App.Shared.Network
{
    public class ProtoBufSerializeInfo<T>: ISerializeInfo where T : IMessage<T>
    {
        public delegate void ResetMessage(T message);
        private SerializationStatistics _statistics;
        public SerializationStatistics Statistics { get { return _statistics; } }
        private byte[] _outputBuffer = new byte[4096];
        
        public ProtoBufSerializeInfo(MessageParser parser)
        {
			_statistics = new SerializationStatistics(typeof(T).Name);
        }

        public void Serialize(Stream outStream, object message)
        {
            var stream = ObjectAllocatorHolder<CodedOutputStream>.Allocate();
            stream.Initizlize(outStream, _outputBuffer);
            (message as IMessage).WriteTo(stream);
            stream.Flush();
            ObjectAllocatorHolder<CodedOutputStream>.Free(stream);
        }

        public virtual object Deserialize(Stream inStream)
        {
            T message = ObjectAllocatorHolder<T>.Allocate();
            MergeFrom(message, inStream);
            return message;
        }

        public void MergeFrom(IMessage message, Stream input)
        {
         

            
            CodedInputStream codedInputStream = ObjectAllocatorHolder<CodedInputStream>.Allocate();
            MemoryStream stream = (MemoryStream) input;
            codedInputStream.Initialize(null, stream.GetBuffer(), (int)stream.Position, (int)stream.Length);
            message.MergeFrom(codedInputStream);
            
            ObjectAllocatorHolder<CodedInputStream>.Free(codedInputStream);
        }

	    public virtual void Dispose()
	    {
	    }
    }
}