using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using VNet.Base.Interface;
using LoggerAdapter = Core.Utils.LoggerAdapter;

namespace VNet.Base.LiteNet
{
    internal class LiteNetServer : IVNetServer
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(LiteNetServer));
        public event Action<IVNetPeer, MemoryStream> OnReceiveListener;
        private EventBasedNetListener _listener = new EventBasedNetListener();
        private NetManager _server;
        private Dictionary<long, IVNetPeer> _peers = new Dictionary<long, IVNetPeer>();
        private MemoryStream _receiveStream = new MemoryStream(8019);
        public event Action<IVNetPeer> OnAcceptListener;
        public event Action<IVNetPeer> OnDisconnectListener;
        public void Init()
        {
            
        }

        public void Poll()
        {
            if (null != _server)
            {
                _server.PollEvents();
            }
        }

        public void Stop()
        {
            _server.Stop();
        }

        public void Bind(int port)
        {
            _server = new NetManager(_listener);
            //取消超时逻辑，使用TCP的生命周期
            _server.DisconnectTimeout = int.MaxValue;
            
            if (!_server.Start(port))
            {
                Logger.Error("Server Start Fialed !! check if the port is in use");
            }
            _listener.ConnectionRequestEvent += OnRequest;
            _listener.PeerConnectedEvent += OnConnect;
            _listener.NetworkReceiveEvent += OnReceive;
            _listener.PeerDisconnectedEvent += OnDisconnect;
        }

        private void OnDisconnect(NetPeer peer, DisconnectInfo info)
        {
            switch (info.Reason)
            {
                case DisconnectReason.RemoteConnectionClose:
                    break;
                case DisconnectReason.ConnectionFailed:
                case DisconnectReason.SocketReceiveError:
                case DisconnectReason.Timeout:
                case DisconnectReason.DisconnectPeerCalled:
                case DisconnectReason.SocketSendError:
                    Logger.ErrorFormat("Disconnect : {0} with code {1} {2}", info.Reason, (SocketError)info.SocketErrorCode,peer.ConnectId);
                    break;
            }

            IVNetPeer vnetPeer;
            if (_peers.TryGetValue(peer.ConnectId, out vnetPeer))
            {
                vnetPeer.OnDisconnect();
                _peers.Remove(peer.ConnectId);
                if (null != OnDisconnectListener)
                {
                    OnDisconnectListener(vnetPeer);
                }
            }
            else
            {
                Logger.ErrorFormat("peer does not exist with id " + peer.ConnectId);
            }
        }

        private void OnConnect(NetPeer peer)
        {
            var myNetPeer = new LiteNetPeer(peer);
            _peers[peer.ConnectId] = myNetPeer;
            if (null != OnAcceptListener)
            {
                OnAcceptListener(myNetPeer);
            }
        }

        private void OnRequest(ConnectionRequest request)
        {
            request.Accept();
        }

        private void OnReceive(NetPeer peer, NetDataReader reader, DeliveryMethod method)
        {
            IVNetPeer realTimePeer;
            if (_peers.TryGetValue(peer.ConnectId, out realTimePeer))
            {
                if (null != OnReceiveListener)
                {
                    _receiveStream.Write(reader.Data, reader.Position, reader.AvailableBytes);
                    OnReceiveListener(realTimePeer, _receiveStream);
                    _receiveStream.Position = 0;
                    _receiveStream.SetLength(0);
                }
            }
        }
    }

}
