using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Core.Utils;
using VNet.Base.Interface;

namespace VNet.Base.Tcp
{
    public class TcpServer : TcpService, IVNetServer
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(TcpServer));
        private Socket _listenSocket;
        public event Action<IVNetPeer> OnAcceptListener;
        public event Action<IVNetPeer, MemoryStream> OnReceiveListener;
        public event Action<IVNetPeer> OnDisconnectListener;
        private bool _littleEndian;

        public TcpServer(bool littleEndian)
        {
            _littleEndian = littleEndian;
        }

        public void Init()
        {

        }

        public void Poll()
        {

        }

        public void Bind(int port)
        {
            Bind(port, 50);
        }

        public void Bind(int port, int maxConnection)
        {
            try
            {
                Logger.InfoFormat("TcpServer bind port {0}", port);
                var localEndPoint = new IPEndPoint(IPAddress.Any, port);
                _listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _listenSocket.Bind(localEndPoint);
                _listenSocket.Listen(maxConnection);
                IsRunning = true;
                StartAccept(null);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }

        public void Stop()
        {
            IsRunning = false;
            _listenSocket.Close();
            var cleanlist = new List<TcpConnection>();
            lock (_connections)
            {
                foreach (var tcpConnection in _connections)
                {
                    cleanlist.Add(tcpConnection.Value);
                }
                cleanlist.ForEach(conn => conn.OnDisconnect());
            }
        }

        private void StartAccept(SocketAsyncEventArgs args)
        {
            if (!IsRunning)
            {
                return;
            }

            if (null == args)
            {
                args = new SocketAsyncEventArgs();
                args.Completed += OnAcceptComplete;
            }
            else
            {
                args.AcceptSocket = null;
            }
            var async = _listenSocket.AcceptAsync(args);
            if (!async)
            {
                OnAccept(null);
            }
        }

        private void OnAcceptComplete(object sender, SocketAsyncEventArgs e)
        {
            OnAccept(e);
        }

        private void OnAccept( SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                Logger.ErrorFormat("accept complete with error {0}", e.SocketError);
            }
            else
            {
                var conn = new TcpConnection(_littleEndian);
                lock (_connections)
                {
                    _connections[conn.ConnectId] = conn;
                }
                conn.OnConnect(e.AcceptSocket);
                if (null != OnAcceptListener)
                {
                    OnAcceptListener(conn);
                }
                conn.StartReceive(null);
                conn.OnReceiveListener += OnReceiveListener;
                conn.OnDisconnectListener += OnDisconnected;
            }
            StartAccept(e);
        }

        private void OnDisconnected(IVNetPeer peer)
        {
            lock (_connections)
            {
                if (_connections.ContainsKey(peer.ConnectId))
                {
                    _connections.Remove(peer.ConnectId);
                }
                if (null != OnDisconnectListener)
                {
                    OnDisconnectListener(peer);
                }
            }
        }
    }
}
