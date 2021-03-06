﻿using System.IO;
using App.Protobuf;
using Core.Network;
using Core.Prediction.VehiclePrediction.Event;
using RpcNetwork.RpcNetwork;

namespace App.Shared.Network
{
    public class VehicleEventSerializeInfo : ISerializeInfo
    {

        private ProtoBufSerializeInfo<VehicleEventMessage> _serialize;
        public VehicleEventSerializeInfo()
        {
            _serialize = new ProtoBufSerializeInfo<VehicleEventMessage>(VehicleEventMessage.Parser);
        }

        public int Serialize(Stream outStream, object message)
        {
            var syncEvent = (IVehicleSyncEvent) message;
            VehicleEventMessage msg = VehicleEventMessage.Allocate();
            VehicleEventMessageConverter.ToProtoBuf(msg, syncEvent);
            var ret = _serialize.Serialize(outStream, msg);
            msg.ReleaseReference();
            return ret;
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