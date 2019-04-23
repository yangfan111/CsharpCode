using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Net.Configuration;
using Core.Network;
using Core.Utils;
using VNet.Base.Interface;
using VNet.Base.LiteNet;
using VNet.Base.Tcp;
using VNet.Base.Udp;

namespace VNet
{
    public abstract class VNetworkService : AbstractNetworkService
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(VNetworkService)); 
        protected Dictionary<long, VNetworkChannel> Connections = new Dictionary<long, VNetworkChannel>();
        protected VNetworkChannel _lastConnection;
        public event Action<INetworkChannel> ChannelConnected;
        public event Action<INetworkChannel> ChannelDisconnected;
        protected bool LittleEndian;

        public event Action<IVNetPeer> OnReliableReceivedListener;

        public event Action<IVNetPeer, MemoryStream> OnRealTimeReceiveListener;
//        protected readonly IRealTimeServer RealTimeServer = new UdpServer();

        protected VNetworkService(string name) : base(name)
        {
        }

        public virtual void OnReliableConnected(IVNetPeer peer)
        {
            var channel = new VNetworkChannel(peer, LittleEndian);

            channel.OnReliableConnected();
            if (Connections.ContainsKey(peer.ConnectId))
            {
                Logger.ErrorFormat("Connid {0} does not exist in connections ", peer.ConnectId);
                return;
            }
            Connections[peer.ConnectId] = channel;
            _lastConnection = channel;
            AddChannel(channel);
            if (null != ChannelConnected)
            {
                ChannelConnected(channel);
            }
            OnChannelConnected(channel);
        }

        protected void OnRealTimeConnected(IVNetPeer peer)
        {
            VNetworkChannel channel;
            if(Connections.TryGetValue(peer.ConnectId, out channel))
            {
                channel.RealTimePeer = peer;
            }
            else
            {
                Logger.ErrorFormat("Illegal connect id : {0}" , peer.ConnectId);
            }
        }

        protected virtual void OnChannelConnected(VNetworkChannel channel)
        {
            
        }

        public void OnReliableReceive(IVNetPeer conn, MemoryStream stream)
        {
            var packet = VNetPacketMemSteam.Allocate();
            packet.CopyFrom(stream.GetBuffer(), 0, (int)stream.Length);
            VNetworkChannel channel;
            if (Connections.TryGetValue(conn.ConnectId, out channel))
            {
                channel.AddToDeserializeQueue(packet);
            }
            channel.FlowRecv(true,packet.Length);
            packet.ReleaseReference();
            if (null != OnReliableReceivedListener)
            {
                OnReliableReceivedListener(conn);
            }
        }

        /// <summary>
        /// 每个非可靠包里附带相关的Tcp的连接ID
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        protected virtual AbstractNetowrkChannel GetChannel(IVNetPeer peer, MemoryStream stream)
        {
            var connid = BitConverter.ToInt32(stream.GetBuffer(), 0);
            VNetworkChannel channel;
            if (Connections.TryGetValue(connid, out channel))
            {
                return channel;
            }
            return null;
        }

        public void OnRealTimeReceive(IVNetPeer peer, MemoryStream stream)
        {
            if (null != OnRealTimeReceiveListener)
            {
                OnRealTimeReceiveListener(peer, stream);
            }
            var channel = GetChannel(peer, stream);
            var length = (int)stream.Length - 4;
            if (null == channel || length <0)
            {
                return;
            }
            var packet = VNetPacketMemSteam.Allocate();
            packet.CopyFrom(stream.GetBuffer(), 4, length);
            channel.AddToDeserializeQueue(packet);
            channel.FlowRecv(false,stream.Length);
            packet.ReleaseReference();
        }

        protected virtual void OnDisconnect(IVNetPeer conn)
        {
            if (Connections.ContainsKey(conn.ConnectId))
            {
                var channel = Connections[conn.ConnectId];
                if (null != ChannelDisconnected)
                {
                    ChannelDisconnected(channel);
                   
                }
                Connections[conn.ConnectId].OnDisconnect();
                Connections.Remove(conn.ConnectId);
                RemoveChannel(channel);
            }
            else
            {
                Logger.ErrorFormat("ConnectId {0} doesn't exist ", conn.ConnectId);
            }
        }

        protected void OnRealTimeDisconnect(IVNetPeer peer)
        {
            // 取消了超时逻辑，依赖tcp连接
        }

        public void StartReceive()
        {

        }

        public override void Close()
        {
            var channels = new List<VNetworkChannel>(Connections.Count);
            foreach (var networkChannel in Connections)
            {
                channels.Add(networkChannel.Value);
            }

            foreach (var channel in channels)
            {
               channel.OnDisconnect();
            }
            channels.Clear();
            Connections.Clear();
        }
    }
}
