using Core.Network;
using Google.Protobuf;
using System;
using System.IO;
using App.Shared;
using UnityEngine;
using Core.Utils;
using System.Collections.Generic;
using Com.Wooduan.Ssjj2.Common.Net.Proto;

namespace RpcNetwork.RpcNetwork
{
    public class RpcNetworkMessageManager : IRpcNetworkMessageManager
    {
        private ProtoBufMessageSerializer _serializer;
        private RpcNetworkMessageGameDispatcher _dispatcher;

        public RpcNetworkMessageManager()
        {
            _serializer = new ProtoBufMessageSerializer();
            _dispatcher = new RpcNetworkMessageGameDispatcher(_serializer);
        }

        public void RegisterLater(string messageName, IRpcNetworkMessageHandler handler)
        {
            _dispatcher.RegisterLater(messageName, handler);
        }

        public void RegisterImmediate(string messageName, IRpcNetworkMessageHandler handler)
        {
            _dispatcher.RegisterImmediate(messageName, handler);
        }

        public void NetworkChannelOnMessageReceived(INetworkChannel channel, int messageType, object messageBody)
        {
            Apc apc = (Apc)messageBody;
            if (apc != null) {
                _dispatcher.SaveDispatch(channel, apc.FunctionName, apc.Parameters);
            }
        }

        public void DriveDispatch()
        {
            _dispatcher.DriveDispatch();
        }

        public INetworkMessageSerializer GetRpcMessageSerializer()
        {
            INetworkMessageSerializer _defaultSerializer = new NetworkMessageSerializer(new AppMessageTypeInfo(new ProtoBufSerializeInfo<Apc>(Apc.Parser)));
             return _defaultSerializer;
        }

        public void Send(INetworkChannel channel, string messageName, params IMessage[] messageBodys)
        {
            var apc = Apc.Allocate();
            apc.FunctionName = messageName;

            foreach (var msg in messageBodys)
            {
                if (null == msg)
                    continue;
                MemoryStream stream = new MemoryStream();
                _serializer.Serialize(stream, msg);
                ByteString byteString = ByteString.CopyFrom(stream.ToArray());
                apc.Parameters.Add(byteString);
            }

            channel.SendReliable(messageName.GetHashCode(), apc);
            apc.ReleaseReference();
        }

        public void Test()
        {
            var apc = Apc.Allocate();
            apc.FunctionName = RpcMessageType.RequestCreateRoom;

            var msg = RequestCreateRoomMessage.Allocate();
            msg.HallRoomId = 1;
            msg.MapId = 33;
            var player = RoomPlayer.Allocate();
            player.Id = 4;
            msg.Players.Add(player);
            {
                MemoryStream stream = new MemoryStream();
                if (msg != null)
                    _serializer.Serialize(stream, msg);
                ByteString byteString = ByteString.CopyFrom(stream.ToArray());
                apc.Parameters.Add(byteString);
            }
            NetworkChannelOnMessageReceived(null, 0, apc);
            msg.ReleaseReference();
        }
    }

    public interface IRpcNetworkMessageManager
    {
        void RegisterLater(string messageName, IRpcNetworkMessageHandler handler);
        void RegisterImmediate(string messageName, IRpcNetworkMessageHandler handler);
        void NetworkChannelOnMessageReceived(INetworkChannel channel, int messageType, object messageBody);
        void DriveDispatch();
        INetworkMessageSerializer GetRpcMessageSerializer();
        void Send(INetworkChannel channel, string messageName, params IMessage[] messageBodys);
        void Test();
    }

    public abstract class AbstractRpcMessageHandler : IRpcNetworkMessageHandler
    {
        protected static LoggerAdapter _logger = new LoggerAdapter(typeof(AbstractRpcMessageHandler));
        private readonly MemoryStream _receiveMemoryStream = new MemoryStream();

        public void DoHandle(INetworkChannel networkChannel, object messagName, object messageBody, IRpcNetworkMessageSerializer serializer)
        {
            //if (messageBody != null && !(messageBody is ByteString))
            //    throw new Exception("error type");

            try
            {
                byte[] byteArray = null;
                //if (messageBody != null)
                //{
                //    byteArray = (messageBody as ByteString).ToByteArray();
                //    _receiveMemoryStream.Write(byteArray, 0, byteArray.Length);
                //}

                var methodInfo = this.GetType().GetMethod("Handle");
                object[] paramValus = new object[methodInfo.GetParameters().Length];
                paramValus[0] = networkChannel;
                paramValus[1] = messagName;
                for (int i = 2; i < methodInfo.GetParameters().Length; i++)
                {
                    byteArray = (messageBody as List<ByteString>)[i - 2].ToByteArray();
                    _receiveMemoryStream.Write(byteArray, 0, byteArray.Length);
                    _receiveMemoryStream.Position = 0;

                    var mi = serializer.GetType().GetMethod("Deserialize");
                    var gmi = mi.MakeGenericMethod(methodInfo.GetParameters()[i].ParameterType);
                    paramValus[i] = gmi.Invoke(serializer, new object[] { _receiveMemoryStream });
                    _receiveMemoryStream.Position = 0;
                    _receiveMemoryStream.SetLength(0);
                }

                methodInfo.Invoke(this, paramValus);
            }
            catch (Exception e)
            {
                _logger.Error("Rpc Error", e);
            }

            //Handle(networkChannel, messagName, obj);
        }

        //public abstract void Handle(INetworkChannel networkChannel, object messagName, object messageData);

        public void DoHandle(INetworkChannel networkChannel, string messagName, int messageCode, object messageBody, IRpcNetworkMessageSerializer serializer)
        {
           
        }

    }
}
