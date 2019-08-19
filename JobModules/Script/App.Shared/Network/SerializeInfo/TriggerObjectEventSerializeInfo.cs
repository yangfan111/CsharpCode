using System.IO;
using App.Protobuf;
using Core.Network;
using Core.SceneTriggerObject;
using RpcNetwork.RpcNetwork;

namespace App.Shared.Network
{
    public class TriggerObjectEventSerializeInfo : ISerializeInfo
    {
        private ProtoBufSerializeInfo<TriggerObjectEventMessage> _serialize;
        public TriggerObjectEventSerializeInfo()
        {
            _serialize = new ProtoBufSerializeInfo<TriggerObjectEventMessage>(TriggerObjectEventMessage.Parser);
        }

        public int Serialize(Stream outStream, object message)
        {
         
            var syncEvent = (TriggerObjectSyncEvent)message;
            var msg = TriggerObjectEventMessage.Allocate();
            TriggerObjectEventMessageConverter.ToProtoBuf(msg, syncEvent);
            var ret =_serialize.Serialize(outStream, msg);
            msg.ReleaseReference();
            return ret;
        }

        public object Deserialize(Stream inStream)
        {
            TriggerObjectEventMessage msg = (TriggerObjectEventMessage)_serialize.Deserialize(inStream);
            var rc = TriggerObjectEventMessageConverter.FromProtoBuf(msg);
            msg.ReleaseReference();
            return rc;
        }

        private SerializationStatistics _statistics = new SerializationStatistics("VehicleEvent");
        public SerializationStatistics Statistics { get { return _statistics; } }

        public void Dispose()
        {
        }
    }
}