using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using Common;
using Core.Network.ENet;
using Core.ObjectPool;
using Core.Utils;
using VNet;
using System.Security.Cryptography;
using Utils.Replay;
using WeaponConfigNs;
using NetworkMessageRecoder = Utils.Replay.NetworkMessageRecoder;

namespace Core.Network
{
    public interface IPacket : IRefCounter
    {
        int Length { get; }
        void CopyTo(byte[] array, int arrayIndex, int count, int sourceIndex);
    }

    public struct RecvMessage
    {
        public AbstractNetowrkChannel Channel;
        public int Type;
        public object msg;
    }

    public class FlowStatue
    {
        public override string ToString()
        {
            return string.Format("sa: {0}{6}, sc: {1}, sm{2}, ra: {3}{7}, rc: {4}, rm{5}",
                SendAvg > 1024 ? SendAvg / 1024 : SendAvg, SendCount, sendMs,
                RecvAvg > 1024 ? RecvAvg / 1024 : RecvAvg, RecvCount, recvMs, SendAvg > 1024 ? "KB" : "B",
                RecvAvg > 1024 ? "KB" : "B");
        }

        private long recv;
        private int recvCount;
        private int sendCount;
        private long send;
        private long sendMs;
        private long recvMs;

        public void Send(long bytes, long ms)
        {
            send += bytes;
            sendCount++;
            sendMs += ms;
        }

        public void Recv(long bytes, long ms)
        {
            recv += bytes;
            recvCount++;
            recvMs += ms;
        }

        public int SendAvg;
        public int SendCount;
        public int RecvAvg;
        public int RecvCount;

        public void Tick(float intval)
        {
            SendAvg = (int) (send / intval);
            SendCount = (int) (sendCount/intval +0.5f);
            RecvAvg = (int) (recv / intval);
            RecvCount =  (int) (recvCount/intval+0.5f);
            sendMs = recvMs = send = recv = recvCount = sendCount = 0;
        }
    }

    public abstract class AbstractNetowrkChannel : INetworkChannel
    {
        public static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AbstractNetowrkChannel));
        public event Action<INetworkChannel> Disconnected;
        public event Action<INetworkChannel> OnDisconnected;
        public event Action<INetworkChannel, int, object> MessageReceived;
        public abstract void SendReliable(int messageType, object messageBody);
        public abstract void SendRealTime(int messageType, object messageBody);
        public abstract bool IsConnected { get; }
        public abstract bool IsUdpConnected { get; }
        public abstract void Disconnect();
        public int LocalConnId { get; set; }
        public virtual int RemoteConnId { get; set; }
        public virtual int UdpPort { get; set; }
        protected bool LittleEndian;
        public abstract SocketError ErrorCode { get; }


        private volatile INetworkMessageSerializer _serializer;
        private volatile IRecordManager _recoder;
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AbstractNetowrkChannel));
        private Queue _serializeQueue = Queue.Synchronized(new Queue());
        private Queue _deserializeQueue = Queue.Synchronized(new Queue());
        private VNetPacketMemSteam _receiveMemoryStream = VNetPacketMemSteam.Allocate();
        private Stopwatch _stopwatch = new Stopwatch();
        private const int MaxLoopCount = 2;

        protected AbstractNetowrkChannel()
        {
            TcpFlowStatus = new FlowStatue();
            UdpFlowStatus = new FlowStatue();
        }


        public void FlowSend(bool Type, long bytes, long ms = 0)
        {
            if (Type)
            {
                TcpFlowStatus.Send(bytes, ms);
            }
            else
            {
                UdpFlowStatus.Send(bytes, ms);
            }
        }

        public void FlowRecv(bool Type, long bytes, long ms = 0)
        {
            if (Type)
            {
                TcpFlowStatus.Recv(bytes, ms);
            }
            else
            {
                UdpFlowStatus.Recv(bytes, ms);
            }
        }

        public FlowStatue TcpFlowStatus { get; private set; }
        public FlowStatue UdpFlowStatus { get; private set; }

        public void Dispose()
        {
            Logger.Info("AbstractNetowrkChannel.Dispose");
            try
            {
                ProcessSerializeQueue();
               
                ProcessDeserializeQueue();
                //_serializer 有可能是公共的，这里不调Dispose
                //			if (_serializer != null)
                //			{
                //				_serializer.Dispose();
                //			}
                _receiveMemoryStream.ReleaseReference();
                _receiveMemoryStream = null;
                if(_serializer!=null)
                    _serializer.Dispose();
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("{0}",e);
            }
           
           
        }

        public abstract int Id { get; }

        public int SerializeQueueCount
        {
            get { return _serializeQueue.Count; }
        }

      
           

        public int RecvQueueCount
        {
            get { return 0; }
        }

        public int DeserializeQueueCount
        {
            get { return _deserializeQueue.Count; }
        }

        public INetworkMessageSerializer Serializer
        {
            get { return _serializer; }
            set { _serializer = value; }
        }

        public IRecordManager Recoder
        {
            get { return _recoder; }
            set { _recoder = value; }
        }


        public void DoReceived(byte[] buffer, int count)
        {
        }

        public void DoReceived(MemoryStream stream)
        {
            var buffer = stream.GetBuffer();
            int messageType = LittleEndian
                ? BitConverter.ToInt32(stream.GetBuffer(), 0)
                : (buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3];
            stream.Seek(4, SeekOrigin.Begin);

            if (_logger.IsDebugEnabled)
            {
                _logger.DebugFormat("received message message type {0}", messageType);
            }

            if (_serializer == null)
            {
                throw new Exception("serializer not set");
            }

            object rc = _serializer.Deserialize(stream, messageType);
            if (MessageReceived != null)
            {

                ProcessMsg(new RecvMessage()
                {
                    Channel = this,
                    msg = rc,
                    Type = messageType
                });
            }
            else
            {
                _logger.WarnFormat("drop message because not event handler, type {0}", messageType);
            }
        }

        // network thread
        public void AddToDeserializeQueue(IPacket enetEventPacket)
        {
            enetEventPacket.AcquireReference();
            _deserializeQueue.Enqueue(enetEventPacket);
        }


        // main Thread
        protected void AddToSerializeQueue(NetworkMessageItem item)
        {
            if (_recoder != null)
            {
                if (this.Id != _recoder.Info.ChannedId)
                {
                    _recoder.Info.ChannedId = this.Id;
                    _recoder.UpdateInfoToFile();
                    
                }
                _recoder.AddMessage(EReplayMessageType.OUT, NetworkMessageRecoder.RecodMessageItem.Allocate(this,
                    item.MessageType, item.MessageBody, MyGameTime.stage, MyGameTime.seq, NetworkMessageRecoder.ERecodMessagetype.UdpOut));
            }
            if (IsConnected && _serializer != null)
            {
                
                item.AcquireReference();
                _serializeQueue.Enqueue(item);
            }
            else
            {
                if (_serializer == null)
                    _logger.InfoFormat("drop message for serializer not set");
                if (!IsConnected)
                {
                    _logger.InfoFormat("drop message for not connected{0}", IdInfo());
                }
            }
        }



        private void ProcessMsg(RecvMessage msg)
        {
            try
            {
                _stopwatch.Start();
                _logger.DebugFormat("ProcessRecvQueue msg {0}", msg.Type);
                MessageReceived(msg.Channel, msg.Type, msg.msg);
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("ProcessRecvQueue error {0} {1}", IdInfo(), e);
            }
            finally
            {
                _stopwatch.Stop();
                var time = _stopwatch.ElapsedTicks / 10000f;
                if (time > 2)
                {
                    _logger.InfoFormat("ProcessRecvQueue {0} too long  {1} {2}", msg.Type, time, msg.Channel);
                }

                _stopwatch.Reset();
                if (msg.msg is IRefCounter)
                {
                    (msg.msg as IRefCounter).ReleaseReference();
                }
            }
        }

        public int ProcessDeserializeQueue(bool isMultiThread = false)
        {
            int count = 0;

            while (_deserializeQueue.Count > 0 && count < MaxLoopCount)
            {
                if(isMultiThread)
                count++;
                IPacket p = (IPacket) _deserializeQueue.Dequeue();
                try
                {
                    if (IsConnected)
                    {
                        if (p is VNetPacketMemSteam)
                        {
                            var memoryStream = ((VNetPacketMemSteam) p).Stream;

                            if (memoryStream != null)
                            {
                                //lock (memoryStream)
                                {
                                    DoReceived(memoryStream);
                                }

                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("error while do send {0} {1}", IdInfo(), e);
                }
                finally
                {
                    if (p != null)
                    {
                        p.ReleaseReference();
                    }
                }
            }

            return count;
        }

        public int ProcessSerializeQueue(bool isMultiThread = false)
        {
            var count = 0;
            if (_serializeQueue.Count > 2000 || !IsConnected)
            {
                while (_serializeQueue.Count > 0 && count < MaxLoopCount)
                {
                    if (isMultiThread)
                        count++;

                    _logger.InfoFormat("too many pending message {0}, count = {1}", ToString(),
                        _serializeQueue.Count);


                    NetworkMessageItem item = (NetworkMessageItem) _serializeQueue.Dequeue();
                    
                    item.ReleaseReference();
                }
            }
            else
            {
                while (_serializeQueue.Count > 0 && count < MaxLoopCount)
                {
                    if (isMultiThread)
                        count++;
                    try
                    {
                        NetworkMessageItem item = (NetworkMessageItem) _serializeQueue.Dequeue();
                       // MemoryStream ms = ObjectAllocatorHolder<MemoryStream>.Allocate();
                        try
                        {
                       
                            item.MemoryStream.Seek(4, SeekOrigin.Begin);
                             var serialize =DoSerialize( item.MemoryStream, item.MessageType, item.MessageBody);
                           
                            if (IsConnected)
                            {
                                AssertUtility.Assert(DoSend(item) == serialize+4);
                            }
                            
                        }
                        finally
                        {
                           
                            item.ReleaseReference();
                        }
                        

                       
                        //_sendQueue.Enqueue(item);
                    }
                    catch (Exception e)
                    {
                        _logger.ErrorFormat("error while do send {0} {1}", IdInfo(), e);
                    }
                }
            }

            return count;
        }

                
        protected abstract int DoSend(NetworkMessageItem item);


        protected int DoSerialize(MemoryStream ms, int messageType, object messageBody)
        {
            
            if (!LittleEndian)
            {
                ms.WriteByte((byte)(messageType>>24));
                ms.WriteByte((byte)(messageType>>16));
                ms.WriteByte((byte)(messageType>>8));
                ms.WriteByte((byte)messageType);
               
               
              
            }
            else
            {
              
                ms.WriteByte((byte)messageType);
                ms.WriteByte((byte)(messageType>>8));
                ms.WriteByte((byte)(messageType>>16));
                ms.WriteByte((byte)(messageType>>24));
            }

            
            int send = (int)_serializer.Serialize(ms, messageType, messageBody);
            return send +4;
        }

        public void PassiveClosed()
        {
            Logger.InfoFormat("AbstractNetowrkChannel.PassiveClosed {0}", IdInfo());
            if (Disconnected != null)
            {
                Disconnected(this);
            }
        }

        private float _tickTime;
        private int _tickIntval = 2;
        

        public void FlowTick(float time)
        {
            if (_tickTime + _tickIntval < time)
            {
                TcpFlowStatus.Tick(time - _tickTime);
                UdpFlowStatus.Tick(time - _tickTime);
                _tickTime = time;
            }
        }

        public abstract string IdInfo();
        public abstract  void RealTimeConnect(int udpPort, int remoteUdpId);

        public abstract string DebugInfo();
    }
}