using System.IO;
using App.Protobuf;
using Core.Network;
using Core.Prediction.VehiclePrediction.TimeSync;

namespace App.Shared.Network
{
    public class SimulationTimeMessageSerializeInfo : ISerializeInfo
    {

        private ProtoBufSerializeInfo<SimulationTimeMessageItem> _serialize;
        public SimulationTimeMessageSerializeInfo()
        {
            _serialize = new ProtoBufSerializeInfo<SimulationTimeMessageItem>(SimulationTimeMessageItem.Parser);
        }

        public void Serialize(Stream outStream, object message)
        {
            var msg = SimulationTimeMessageConverter.ToProtoBuf((SimulationTimeMessage)message);
            _serialize.Serialize(outStream, msg);
            msg.ReleaseReference();
        }

        public object Deserialize(Stream inStream)
        {
            SimulationTimeMessageItem msg = (SimulationTimeMessageItem)_serialize.Deserialize(inStream);
            var rc= SimulationTimeMessageConverter.FromProtoBuf(msg);
            msg.ReleaseReference();
            return rc;
        }

        

        private SerializationStatistics _statistics = new SerializationStatistics("SimulationTimeMessage");
        public SerializationStatistics Statistics { get { return _statistics; } }
        public void Dispose()
        {
        }
    }
}