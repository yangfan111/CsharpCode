using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Core.Network;
using Core.Network.ENet;
using Core.Utils;
using VNet.Base.Interface;

namespace VNet
{
    public class VNetworkChannel : AbstractNetowrkChannel 
    {
        protected IVNetPeer RealiableConn;
        public IVNetPeer RealTimePeer { get; set; }
        public const byte ReliableChannel = 0;
        public const byte RealTimeChannel = 1;
        private bool _isConnected;
        private IRealTimeConnector _connecter;

        private new static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(VNetworkChannel));
        private MemoryStream _sendStream = new MemoryStream();
        Stopwatch _stopwatch = new Stopwatch();
        private int _remoteConnId;
        public override int RemoteConnId
        {
            get { return _remoteConnId; }
           
        }

        public override int UdpPort
        {
            get
            {
                if (null != _connecter)
                    return _connecter.UdpPort;
                return 0;
            }
            
        }

        public VNetworkChannel(IVNetPeer connection, bool littleEndian)
        {
            RealiableConn = connection;
            LittleEndian = littleEndian;
        }

        //initative
        public override void Disconnect()
        {
            Logger.InfoFormat("Call Socket Disconnect{0}", Id);
            _isConnected = false;
            if (null != RealiableConn)
            {
                RealiableConn.OnDisconnect();
            }
            if (null != RealTimePeer)
            {
                RealTimePeer.OnDisconnect();
            }
            PassiveClosed();
        }

        //passive
        public void OnDisconnect()
        {
            Logger.InfoFormat("VNetworkChannel.OnDisconnect:{0}", Id);
            _isConnected = false;
            PassiveClosed();
            if (null != RealTimePeer)
            {
                RealTimePeer.OnDisconnect();
            }
        }

        public void SetConnecter(IRealTimeConnector connecter)
        {
            _connecter = connecter;
        }

        public override void RealTimeConnect(int udpPort, int remoteUdpId)
        {
            if (null != _connecter)
            {
                _remoteConnId = remoteUdpId;
                _connecter.RealTimeConnect( udpPort,remoteUdpId);
            }
        }

        public void OnReliableConnected()
        {
            _isConnected = true;
            LocalConnId = RealiableConn.ConnectId;
        }
        
        protected override int DoSend(NetworkMessageItem item)
        {
            var send = 0;
            var msgItem = item as VNetworkMessageItem;
            if (null == msgItem)
            {
                Logger.ErrorFormat("message item is null or not VNetworkMessageItem {0}",IdInfo());
                return send;
            }

          
            switch (msgItem.Channel)
            {
                case RealTimeChannel:
                    if (null == RealTimePeer)
                    {
                        Logger.ErrorFormat("{0} send failed : peer is null", IdInfo());
                        return send;
                    }
                    _stopwatch.Reset();
                    long l = item.MemoryStream.Length;
                    item.MemoryStream.Seek(0, SeekOrigin.Begin);
                    item.MemoryStream.WriteByte((byte)(RemoteConnId>>0));
                    item.MemoryStream.WriteByte((byte)(RemoteConnId>>8));
                    item.MemoryStream.WriteByte((byte)(RemoteConnId>>16));
                    item.MemoryStream.WriteByte((byte)(RemoteConnId>>24));

                    if (item.MemoryStream.Capacity < item.MemoryStream.Length + 4)
                    {
                        item.MemoryStream.Capacity = item.MemoryStream.Capacity + 4;
                    }
                    send = (int) item.MemoryStream.Length;
                    RealTimePeer.Send(item.MemoryStream.GetBuffer(), (int)item.MemoryStream.Length, 0);

                    _stopwatch.Stop();
                    FlowSend(false,l,_stopwatch.ElapsedMilliseconds);
                    break;
                case ReliableChannel:
                    _stopwatch.Reset();
                    send = (int) item.MemoryStream.Length;
                    RealiableConn.Send(item.MemoryStream.GetBuffer(), (int)item.MemoryStream.Length-4, 4);
                    _stopwatch.Stop();
                    FlowSend(true,item.MemoryStream.Length,_stopwatch.ElapsedMilliseconds);
                    break;
            }

            return send;
        }

        public void SendRawDataRealTime(byte[] bytes, int length, int offset)
        {
            RealTimePeer.Send(bytes, length, offset);
        }

        public void SendRawDataReliable(byte[] bytes, int length, int offset)
        {
            RealiableConn.Send(bytes, length, offset);
        }

        public override int Id
        {
            get { return LocalConnId; }
        }

        public override bool IsConnected
        {
            get { return _isConnected; }
        }

        public override bool IsUdpConnected
        {
            get { return _connecter.isRealTimeConnected; }
        }

        public override SocketError ErrorCode
        {
            get { return RealiableConn.ErrorCode; }
        }

        public override void SendRealTime(int messageType, object messageBody)
        {
            if (null != RealTimePeer)
            {
                var item = VNetworkMessageItem.Allocate(messageType, messageBody, RealTimeChannel);
                AddToSerializeQueue(item);
                item.ReleaseReference();
            }
            else
            {
                Logger.WarnFormat("channel:{0} drop realtime packet for realtime channel not connected",IdInfo());
            }
        }

        public override void SendReliable(int messageType, object messageBody)
        {
            var item = VNetworkMessageItem.Allocate(messageType, messageBody, ReliableChannel);
            AddToSerializeQueue(item);
            item.ReleaseReference();
        }

        public override string IdInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("cid:").Append(Id)
                .Append(" tcpId:").Append(RealiableConn != null ? RealiableConn.ConnectId : -1).Append("  ")
                .Append(RealiableConn != null ? RealiableConn.Ip : "")
                .Append(" udpId:").Append(RealTimePeer != null ? RealTimePeer.ConnectId : -1).Append("  ")
                .Append(RealTimePeer != null ? RealTimePeer.Ip : "");
            return sb.ToString();
        }

        public override string DebugInfo()
        {
            return RealTimePeer != null ? RealTimePeer.DebugInfo : "";
        }
    }
}
