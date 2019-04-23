using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Core.Utils;
using VNet.Base.Interface;

namespace VNet.Base.Udp
{
    public abstract class UdpService 
    {
        private MemoryStream _receiveStream = new MemoryStream();
        private byte[] _receiveBuffer = new byte[1024];
        protected Socket RealTimeSocket;
        protected Dictionary<IPEndPoint, UdpPeer> Peers = new Dictionary<IPEndPoint, UdpPeer>(); 
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(UdpService));

        public event Action<IVNetPeer, MemoryStream> OnReceiveListener;

        protected void StartReceive(SocketAsyncEventArgs args = null)
        {
            if (null == args)
            {
                args = new SocketAsyncEventArgs();
                args.SetBuffer(_receiveBuffer, 0, _receiveBuffer.Length);
                args.Completed += OnReceive;
                args.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            }
            if (!RealTimeSocket.ReceiveFromAsync(args))
            {
                OnReceive(args);
            }
        }

        public void OnDisconnect(UdpPeer peer)
        {
            if(!Peers.Remove(peer.IpEndPoint))
            {
                Logger.Error("rm peer failed !");
            }
        }

        protected void OnReceive(object sender, SocketAsyncEventArgs args)
        {
            OnReceive(args);
        }

        protected void OnReceive(SocketAsyncEventArgs args)
        {
            try
            {
                if (args.BytesTransferred > 0)
                {
                    var ipEndPoint = args.RemoteEndPoint as IPEndPoint;
                    if (null == ipEndPoint)
                    {
                        Logger.Error("IPEndPoint is null");
                        return;
                    }

                    if (!Peers.ContainsKey(ipEndPoint))
                    {
#if NET_4_6 && UNITY_2017
                        var peer = new UdpPeer(ipEndPoint, args.ConnectSocket);
#else
                        var peer = new UdpPeer(ipEndPoint, RealTimeSocket);
#endif
                        Peers[ipEndPoint] = peer;
                        peer.OnDisconnectListener += OnDisconnect;
                        peer.OnConnected();
                    }

                    _receiveStream.Write(args.Buffer, args.Offset, args.BytesTransferred);
                    if (null != OnReceiveListener)
                    {
                        OnReceiveListener(Peers[ipEndPoint], _receiveStream);
                    }

                    _receiveStream.Position = 0;
                    _receiveStream.SetLength(0);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
            finally
            {
                StartReceive(args);
            }
        }
    }
}
