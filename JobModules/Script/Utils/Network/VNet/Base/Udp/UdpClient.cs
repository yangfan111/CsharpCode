﻿using Core.Utils;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using VNet.Base.Interface;

namespace VNet.Base.Udp
{
    public class UdpClient : UdpService, IVNetClient
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(UdpClient));
        private IPEndPoint _endPoint;
        public event Action<IVNetPeer> OnConnectFailedListener;
        public event Action<IVNetPeer> OnConnectListener;
        public event Action<IVNetPeer> OnDisconnectListener;

        public bool IsConnected { get; private set; }

        public void Init()
        {
        }

        public void Poll()
        {
            OnConnectFailedListener = null;
            OnConnectListener = null;
            OnDisconnectListener = null;
        }

        public void Connect(string ip, int port, long connId)
        {
            var address = IPAddress.Parse(ip);
            _endPoint = new IPEndPoint(address, port);
            RealTimeSocket = new Socket(_endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            var peer = new UdpPeer(_endPoint, RealTimeSocket);
            peer.OnConnected();
            peer.OnDisconnectListener += OnDisconnected;
            Peers.Add(_endPoint, peer);
            if (null != OnConnectListener)
            {
                OnConnectListener(peer);
            }
            StartReceive();
            IsConnected = true;
        }

        public void OnDisconnected(UdpPeer peer)
        {
            IsConnected = false;
            if(null != OnDisconnectListener)
            {
                OnDisconnectListener(peer);
            }
        }

        public void Send(string msg)
        {
            var bytes = Encoding.UTF8.GetBytes(msg);
            Send(bytes, bytes.Length, 0);
        }

        public bool Send(byte[] bytes, int length, int offset)
        {
            var args = new SocketAsyncEventArgs();
            args.SetBuffer(bytes, offset, length);
            args.Completed += OnSend;
            args.RemoteEndPoint = _endPoint;
            RealTimeSocket.SendToAsync(args);
            return true;
        }

        private void OnSend(object sender, SocketAsyncEventArgs args)
        {

        }

        public void ReConnect()
        {
            /*throw new NotImplementedException();*/
        }

        public void CloseSocket(Socket socket)
        {
            if (socket == null || !socket.Connected)
            {
                return;
            }
            try
            {
                if (this.RealTimeSocket != null)
                {
                    this.RealTimeSocket.Shutdown(SocketShutdown.Both);
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("Close1:" + ex.Message);
            }
            try
            {
                socket.Close();
            }
            catch (Exception ex2)
            {
                Logger.Debug("Close2:" + ex2.Message);
            }
        }

        public void CloseConnect()
        {
            CloseSocket(this.RealTimeSocket);
            /*throw new NotImplementedException();*/
        }
    }
}
