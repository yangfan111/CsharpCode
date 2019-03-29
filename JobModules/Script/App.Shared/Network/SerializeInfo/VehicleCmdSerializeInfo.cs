using System.IO;
using App.Protobuf;
using Core.Network;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;

namespace App.Shared.Network
{
    public class VehicleCmdSerializeInfo : ISerializeInfo
    {
        private ProtoBufSerializeInfo<VehicleCmdMessage> _serialize;
        public VehicleCmdSerializeInfo()
        {
            _serialize = new ProtoBufSerializeInfo<VehicleCmdMessage>(VehicleCmdMessage.Parser);
        }
        public void Serialize(Stream outStream, object message)
        {
            ReusableList<IVehicleCmd> list = (ReusableList<IVehicleCmd>) message;
            VehicleCmdMessage msg = VehicleCmdMessage.Allocate();
            VehicleCmdMessageConverter.ToProtoBuf(msg, list);
            _serialize.Serialize(outStream, msg);
            msg.ReleaseReference();
        }

        public object Deserialize(Stream inStream)
        {
            VehicleCmdMessage msg = (VehicleCmdMessage)_serialize.Deserialize(inStream);
            var rc =  VehicleCmdMessageConverter.FromProtoBuf(msg);
            msg.ReleaseReference();
            return rc;
        }
      
        private SerializationStatistics _statistics = new SerializationStatistics("VehicleCmd");
        public SerializationStatistics Statistics { get { return _statistics; } }

        public void Dispose()
        {
        }
    }
}