using System;
using System.Net.Sockets;
using Common;
using Core.Network;
using Utils.Replay;

namespace App.Shared.Client
{
    public class FakeReplayNetworkChannel : INetworkChannel
    {
        public FakeReplayNetworkChannel(int id)
        {
            Id = id;
            IsConnected = true;
            IsUdpConnected = true;
        }

        public void Dispose()
        {
           
        }

        public void SendReliable(int messageType, object messageBody)
        {
           
        }

        public void SendRealTime(int messageType, object messageBody)
        {
           
        }

        public FlowStatue TcpFlowStatus { get; }
        public FlowStatue UdpFlowStatus { get; }
        public bool IsConnected { get; }
        public bool IsUdpConnected { get; }
        public event Action<INetworkChannel> Disconnected;
        public event Action<INetworkChannel, int, object> MessageReceived;
        public void Disconnect()
        {
            
        }

        public int LocalConnId { get; }
        public int RemoteConnId { get; }
        public INetworkMessageSerializer Serializer { get; set; }
        public IRecordManager Recoder { get; set; }
        public int Id { get; }
        public SocketError ErrorCode { get; }
        public void FlowSend(bool Type, long bytes, long ms = 0)
        {
           
        }

        public void FlowRecv(bool Type, long bytes, long ms = 0)
        {
            
        }

        public string IdInfo()
        {
            return "dummy";
        }

        public void RealTimeConnect(int udpPort, int remoteUdpId)
        {
            
        }
    }
    public class FakeReplayLoginClient:ILoginClient
    {
       // private readonly INetworkMessageDispatcher _dispatcher;
        private readonly IClientNetwork _clientNetwork;
        private readonly IReplayManager _replay;

        public FakeReplayLoginClient(IClientNetwork clientNetwork, IReplayManager replay)
        {
            _clientNetwork = clientNetwork;
            _replay = replay;
            int id = replay.Info.ChannedId;
            _clientNetwork.OnNetworkConnected(new FakeReplayNetworkChannel(id));
        }


        public void Dispose()
        {
            
        }

        public void ReConnect()
        {
            
        }

        public void Update()
        {
            int seq = MyGameTime.seq;
            int stage = MyGameTime.stage;
            while (true)
            {
                var item = _replay.GetItem(EReplayMessageType.IN, stage, seq, _clientNetwork.Channel.Id);
                if (item == null) break;
                _clientNetwork.NetworkChannelOnMessageReceived(_clientNetwork.Channel, item.MessageType, item.MessageBody);
                item.ReleaseReference();
            }
        }

        public void FlowTick(float time)
        {
            
        }

        public void CloseConnect(ProtocolType protocolType)
        {
         
        }
    }
}