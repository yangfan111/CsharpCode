using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Core.Utils;
using LiteNetLib;
using VNet.Base.Interface;

namespace VNet.Base.Tcp
{
    public class TcpClient : TcpService, IVNetClient
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(TcpClient));
        private Socket _socket;
        private readonly TcpConnection _tcpConnection;
        public event Action<IVNetPeer> OnConnectFailedListener;
        public event Action<IVNetPeer> OnConnectListener;
        public event Action<IVNetPeer> OnDisconnectListener;
        public event Action<IVNetPeer, MemoryStream> OnReceiveListener;

        private string _ip;
        private int _port;

        public bool IsConnected
        {
            get { return _tcpConnection.IsConnected; }
        }

        public TcpClient(bool isLittleEndian)
        {
            _tcpConnection = new TcpConnection(isLittleEndian);
        }

        public void Init()
        {
            _tcpConnection.OnReceiveListener -= OnReceiveListener;
            _tcpConnection.OnDisconnectListener -= OnDisconnect;
            OnConnectFailedListener = null;
            OnConnectListener = null;
            OnDisconnectListener = null;
            OnReceiveListener = null;
        }

        public void Poll()
        {

        }

        public void Connect(string ip, int port, long connId)
        {
            _ip = ip;
            _port = port;
//            var point = new IPEndPoint(IPAddress.Parse(ip), port);
            var point = NetUtils.MakeEndPoint(ip, port);
            _socket = new Socket(point.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var arg = new SocketAsyncEventArgs();
            arg.Completed += OnConnectedComplete;
            arg.RemoteEndPoint = point;
            var async = _socket.ConnectAsync(arg);
            if (!async)
            {
                OnConnected(arg);
            }
            
        }

        public void ReConnect()
        {
            CloseSocket(this._socket);
            this._socket = null;
            Connect(_ip, _port, 0);
        }

        private void OnConnectedComplete(object sender, SocketAsyncEventArgs e)
        {
            OnConnected(e);
        }

        private void OnConnected(SocketAsyncEventArgs e)
        {
            _tcpConnection.ErrorCode = e.SocketError;

            if (e.SocketError != SocketError.Success)
            {
                Logger.ErrorFormat("connect failed with error {0}", e.SocketError);
                if (null != OnConnectFailedListener)
                {
                    OnConnectFailedListener(_tcpConnection);
                }
            }
            else
            {
                _tcpConnection.OnConnect(_socket);
                if (null != OnConnectListener)
                {
                    OnConnectListener(_tcpConnection);
                }
                _tcpConnection.OnReceiveListener += OnReceiveListener;
                _tcpConnection.OnDisconnectListener += OnDisconnect;
                _tcpConnection.StartReceive(null);
                _connections[_tcpConnection.ConnectId] = _tcpConnection;
            }
           
        }

        public void OnDisconnect(IVNetPeer peer)
        {
            try
            {
                if (this._socket != null)
                {
                    this._socket.Shutdown(SocketShutdown.Both);
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Shutdown failed with error {0}", ex.Message);
            }
            try
            {
                if (this._socket != null)
                {
                    this._socket.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Close failed with error {0}", ex.Message);
            }
            this._socket = null;
            if(null != OnDisconnectListener)
            {
                OnDisconnectListener(peer);
            }
        }

//        public void Send(string msg)
//        {
//            var bytes = Encoding.UTF8.GetBytes(msg);
//           _tcpConnection.Send(bytes, bytes.Length); 
//        }

        public void CloseSocket(Socket socket)
        {
            if (socket == null || !socket.Connected)
            {
                return;
            }
            try
            {
                if (this._socket != null)
                {
                    this._socket.Shutdown(SocketShutdown.Both);
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
            CloseSocket(this._socket);
            /*throw new NotImplementedException();*/
        }
    }
}
