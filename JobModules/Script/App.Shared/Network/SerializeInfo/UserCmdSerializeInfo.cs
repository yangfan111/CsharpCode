﻿using System.IO;
using App.Protobuf;
using Core.Network;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using RpcNetwork.RpcNetwork;

namespace App.Shared.Network.SerializeInfo
{
    public class UserCmdSerializeInfo : ISerializeInfo
    {
        private ProtoBufSerializeInfo<UserCmdMessage> _serialize;

        public UserCmdSerializeInfo()
        {
            _serialize = new ProtoBufSerializeInfo<UserCmdMessage>(UserCmdMessage.Parser);
        }

        public int Serialize(Stream outStream, object message)
        {
           
            ReusableList<IUserCmd> list = (ReusableList<IUserCmd>) message;
            UserCmdMessage msg = UserCmdMessage.Allocate();
            UserCmdMessageConverter.ToProtoBuf(msg, list);
            var ret = _serialize.Serialize(outStream, msg);
            msg.ReleaseReference();
            return ret;
        }

        public object Deserialize(Stream inStream)
        {
            UserCmdMessage msg = (UserCmdMessage)_serialize.Deserialize(inStream);
            object rc = UserCmdMessageConverter.FromProtoBuf(msg);
            msg.ReleaseReference();
            return rc;
        }

        public SerializationStatistics Statistics { get { return _serialize.Statistics; } }

        public void Dispose()
        {
        }
    }
}