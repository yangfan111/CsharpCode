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
        private int _remoteConnId;
        public override int RemoteConnId
        {
            get { return _remoteConnId; }
            set
            {
                _remoteConnId = value;
                RealTimeConnect();
            }
        }

        public override int UdpPort
        {
            get
            {
                if (null != _connecter)
                    return _connecter.UdpPort;
                return 0;
            }
            set
            {
                if (null != _connecter)
                    _connecter.UdpPort = value;
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

        public void RealTimeConnect()
        {
            if (null != _connecter)
            {
                _connecter.RealTimeConnect(RemoteConnId);
            }
        }

        public void OnReliableConnected()
        {
            _isConnected = true;
            LocalConnId = RealiableConn.ConnectId;
        }
        Stopwatch _stopwatch = new Stopwatch();
        protected override void DoSend(NetworkMessageItem item)
        {
            var msgItem = item as VNetworkMessageItem;
            if (null == msgItem)
            {
                Logger.ErrorFormat("message item is null or not VNetworkMessageItem {0}",IdInfo());
                return;
            }
            
            switch (msgItem.Channel)
            {
                case RealTimeChannel:
                    if (null == RealTimePeer)
                    {
                        Logger.ErrorFormat("{0} send failed : peer is null", IdInfo());
                        return;
                    }
                    _stopwatch.Reset();
                    long l = item.MemoryStream.Length;
                    item.MemoryStream.Seek(0, SeekOrigin.Begin);
                    item.MemoryStream.Write(BitConverter.GetBytes(RemoteConnId), 0, 4);
                    
                    RealTimePeer.Send(item.MemoryStream.GetBuffer(), (int)item.MemoryStream.Length, 0);

                    _stopwatch.Stop();
                    FlowSend(false,l,_stopwatch.ElapsedMilliseconds);
                    break;
                case ReliableChannel:
                    _stopwatch.Reset();
                    RealiableConn.Send(item.MemoryStream.GetBuffer(), (int)item.MemoryStream.Length-4, 4);
                    _stopwatch.Stop();
                    FlowSend(true,item.MemoryStream.Length,_stopwatch.ElapsedMilliseconds);
                    break;
            }
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

        public override SocketError ErrorCode
        {
            get { return RealiableConn.ErrorCode; }
        }

        public override void SendRealTime(int messageType, object messageBody)
        {
            if (null != RealTimePeer)
            {
                AddToSerializeQueue(new VNetworkMessageItem(messageType, messageBody, RealTimeChannel));
            }
            else
            {
                Logger.WarnFormat("channel:{0} drop realtime packet for realtime channel not connected",IdInfo());
            }
        }

        public override void SendReliable(int messageType, object messageBody)
        {
            AddToSerializeQueue(new VNetworkMessageItem(messageType, messageBody, ReliableChannel));
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
