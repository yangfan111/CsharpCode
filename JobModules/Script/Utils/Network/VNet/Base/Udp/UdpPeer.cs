using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Core.Utils;
using VNet.Base.Interface;

namespace VNet.Base.Udp
{
    public class UdpPeer : IVNetPeer
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(UdpPeer));

        private enum Status
        {
            Connecting,
            Connected,
            Closed,
        }

        public IPEndPoint IpEndPoint { get; private set; }
        private Socket _socket;
        private Status _status;
        private string _ip;

        public event Action<UdpPeer> OnDisconnectListener;
        public int ConnectId { get; set; }
        public SocketError ErrorCode { get; set; }

        public bool IsConnected
        {
            get { return _status == Status.Connected; }
        }

        public UdpPeer(IPEndPoint point, Socket socket)
        {
            IpEndPoint = point;
            _socket = socket;
            _status = Status.Connecting;
        }

        public void OnConnected()
        {
            _status = Status.Connected;
        }

        public void OnDisconnect()
        {
            _status = Status.Closed;
            if (null != OnDisconnectListener)
            {
                OnDisconnectListener(this);
            }
        }

        public void Send(byte[] bytes, int length, int offset = 0)
        {
            if (!IsConnected)
            {
                return;
            }

            //var args = new SocketAsyncEventArgs(); 
            //args.Completed += OnSend;
            //args.RemoteEndPoint = IpEndPoint;
            //args.SetBuffer(bytes, offset, length);
            _socket.SendTo(bytes, offset, length, SocketFlags.None, IpEndPoint);
        }


        private void OnSend(object sender, SocketAsyncEventArgs args)
        {
            OnSend(args);
        }

        private void OnSend(SocketAsyncEventArgs args)
        {
        }

        public string DebugInfo
        {
            get
            {
                return string.Empty;

            }
        }

        public string Ip
        {
            get { return _ip; }
        }
    }
}
