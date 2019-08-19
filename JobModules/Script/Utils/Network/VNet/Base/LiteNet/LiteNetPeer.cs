using System.IO;
using System.Net.Sockets;
using Core.Utils;
using LiteNetLib;
using VNet.Base.Interface;

namespace VNet.Base.LiteNet
{
    internal class LiteNetPeer : IVNetPeer
    {
        private NetPeer _peer;
        public MemoryStream ReceiveData = new MemoryStream();
        private bool _connected;
        private string _ip;
        private static  readonly  LoggerAdapter Logger = new LoggerAdapter(typeof(LiteNetPeer));

        public int ConnectId
        {
            get { return _peer != null ? (int)_peer.ConnectId : -1; }
        }

        public SocketError ErrorCode { get; set; }

        public LiteNetPeer(NetPeer peer)
        {
            if (null != peer)
            {
                _connected = true;
            }
            _peer = peer;
            ReceiveData.Capacity = 1000;
        }

        public void SetPeer(NetPeer peer)
        {
            _peer = peer;
        }

        public void Send(byte[] bytes, int length, int offset)
        {
            if (!_connected )
            {
                Logger.ErrorFormat("LietNet is not connected ! {0}",ConnectId);
                return;
            }

            if (bytes.Length < length + 4)
            {
                Logger.ErrorFormat("LietNet is bytes Length not enough! {0}",bytes.Length);
                return;
            }
            var crc32 = Utils.Utils.Crc32.Compute(bytes, offset, length);
            bytes[length+0]=((byte)(crc32>>0));
            bytes[length+1]=((byte)(crc32>>8));
            bytes[length+2]=((byte)(crc32>>16));
            bytes[length+3]=((byte)(crc32>>24));
            _peer.Send(bytes, offset, length+4, DeliveryMethod.Unreliable); 
        }

        public void OnDisconnect()
        {
            Logger.InfoFormat("LiteNetPeer.OnDisconnect {0}",ConnectId);
            if (!_connected)
            {
                return;
            }

            _connected = false;
            _peer.Disconnect();
        }

        public string DebugInfo
        {
            get { return _peer.Statistics.ToString(); }
        }

        public string Ip
        {
            get { return _peer.EndPoint.Address.ToString(); }
        }
    }
}
