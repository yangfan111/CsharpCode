using System.Diagnostics.CodeAnalysis;
using System.IO;
using Core.Utils;
using UnityEngine.Profiling;

namespace Core.Network
{
    public class NetworkMessageSerializer : INetworkMessageSerializer
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(NetworkMessageSerializer));
        private IMessageTypeInfo _messageTypeInfo;

        public IMessageTypeInfo MessageTypeInfo
        {
            get { return _messageTypeInfo; }
        }

        public NetworkMessageSerializer(IMessageTypeInfo messageTypeInfo)
        {
            _messageTypeInfo = messageTypeInfo;
        }

        public long Serialize(Stream outStream, int messageType, object message)
        {
            var info = _messageTypeInfo.GetSerializeInfo(messageType);
            if (info != null)
            {
                long start = outStream.Position;
                var wl=info.Serialize(outStream, message);
                info.Statistics.OnSerialize((int) (outStream.Position - start));
                var ol= outStream.Position - start;
                AssertUtility.Assert(ol == wl);
                return ol;
            }
            else
            {
              
                _logger.ErrorFormat("don't know how to serialize messageType {0}, type {1}", messageType, message.GetType());
                return 0;
            }
        }

        [SuppressMessage("ReSharper", "UnusedVariable")]
        public object Deserialize(Stream inStream, int messageType)
        {
            var info = _messageTypeInfo.GetSerializeInfo(messageType);
            if (info != null)
            {
                long start = inStream.Position;
                object rc = info.Deserialize(inStream);

                info.Statistics.OnDeserialize((int)(inStream.Position - start));
                
                return rc;
            }
            _logger.ErrorFormat("don't know how to deserialize messageType {0}", messageType);
            return null;
        }

	    public void Dispose()
	    {
            _logger.InfoFormat("Dispose");
		    _messageTypeInfo.Dispose();

	    }
    }
}