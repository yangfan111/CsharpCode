using System.IO;
using App.Protobuf;
using Core.Network;
using Core.Prediction.VehiclePrediction.Event;

namespace App.Shared.Network
{
    public class VehicleEventSerializeInfo : ISerializeInfo
    {

        private ProtoBufSerializeInfo<VehicleEventMessage> _serialize;
        public VehicleEventSerializeInfo()
        {
            _serialize = new ProtoBufSerializeInfo<VehicleEventMessage>(VehicleEventMessage.Parser);
        }

        public void Serialize(Stream outStream, object message)
        {
            var syncEvent = (IVehicleSyncEvent) message;
            VehicleEventMessage msg = VehicleEventMessage.Allocate();
            VehicleEventMessageConverter.ToProtoBuf(msg, syncEvent);
            _serialize.Serialize(outStream, msg);
            msg.ReleaseReference();
        }

        public object Deserialize(Stream inStream)
        {
            VehicleEventMessage msg = (VehicleEventMessage)_serialize.Deserialize(inStream);
            var rc = VehicleEventMessageConverter.FromProtoBuf(msg);
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