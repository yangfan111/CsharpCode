using System;
using System.IO;
using System.Net.Sockets;
using Core.Utils;
using LiteNetLib;
using LiteNetLib.Utils;
using VNet.Base.Interface;
using VNet.Base.Udp;
using LoggerAdapter = Core.Utils.LoggerAdapter;

namespace VNet.Base.LiteNet
{
    public class LiteNetClient : IVNetClient
    {
        private NetManager _netClient;
        private EventBasedNetListener _listener = new EventBasedNetListener();
        private IVNetPeer _peer;
        public event Action<IVNetPeer> OnConnectFailedListener;
        public event Action<IVNetPeer> OnConnectListener;
        public event Action<IVNetPeer> OnDisconnectListener;
        public event Action<IVNetPeer, MemoryStream> OnReceiveListener;
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(LiteNetClient));
        private MemoryStream _receiveStream = new MemoryStream();

        public void Init()
        {
            OnConnectFailedListener = null;
            OnConnectListener = null;
            OnDisconnectListener = null;
            OnReceiveListener = null;
        }
        
        public bool IsConnected { get; private set; }

        public void Poll()
        {
            if (null != _netClient)
            {
                _netClient.PollEvents();
            }
        }

        public void Connect(string ip, int port, long connId)
        {
            _netClient = new NetManager(_listener);
            //取消超时机制，使用TCP的生命周期
            _netClient.DisconnectTimeout = int.MaxValue;
            
            _netClient.Start();
            var peer = _netClient.Connect(ip, port, "",connId);
            if (null == peer)
            {
                Logger.ErrorFormat("lite net client connect failed , ip {0} port {1}", ip, port);
                if (null != OnConnectFailedListener)
                {
                    OnConnectFailedListener(_peer);
                }
                return;
            }
            _peer = new LiteNetPeer(peer);
            _listener.NetworkReceiveEvent += OnNetworkReceive;
            _listener.PeerDisconnectedEvent += OnDisconnected;
            if(null != OnConnectListener)
            {
                OnConnectListener(_peer);
            }

            IsConnected = true;
        }

//        public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
//        {
//            Logger.Info("[CLIENT] We received error " + socketErrorCode);
//        }

        public void OnNetworkReceive(NetPeer peer, NetDataReader reader, DeliveryMethod deliveryMethod)
        {
            lock (_receiveStream)
            {
                if (null != OnReceiveListener)
                {
                    var stream = _receiveStream;
                    stream.Position = 0;
                    var crc32 = Utils.Utils.Crc32.Compute(reader.Data, reader.Position, reader.AvailableBytes-4);
                    UInt32 crc32Message = (UInt32)(  reader.Data[reader.Position+reader.AvailableBytes - 1] << 24)
                                          | (UInt32)(reader.Data[reader.Position+reader.AvailableBytes - 2] << 16)
                                          | (UInt32)(reader.Data[reader.Position+reader.AvailableBytes - 3] << 8)
                                          | (UInt32)(reader.Data[reader.Position+reader.AvailableBytes - 4] << 0);
                    if ( crc32 == crc32Message)
                    {
                        stream.Write(reader.Data, reader.Position, reader.AvailableBytes-4);

                        OnReceiveListener(_peer, stream);
                        stream.Position = 0;
                        stream.SetLength(0);
                    }
                    else
                    {
                       Logger.ErrorFormat("udp message crc error:{0} |= {1}", crc32,crc32Message);
                    }
                   
                }
                if (reader is NetPacketReader)
                {
                    ((NetPacketReader)reader).RecycleSource();
                }
            }
        }

        public void OnDisconnected(NetPeer peer, DisconnectInfo info)
        {
            IsConnected = false;
            if(null != OnDisconnectListener)
            {
                OnDisconnectListener(_peer);
            }
            Logger.InfoFormat("Lite Net Disconnected with info {0} , error {1} {2}",  info.Reason, info.SocketErrorCode, _peer.ConnectId);
        }

        public void ReConnect()
        {
            /*throw new NotImplementedException();*/
        }

        public void CloseConnect()
        {
            /*throw new NotImplementedException();*/
        }
    }
}
