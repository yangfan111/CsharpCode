using System;
using System.IO;
using Core.Network;
using Core.ObjectPool;
using Core.Utils;
using VNet;

namespace Utils.Replay
{
    public partial class NetworkMessageRecoder
    {
        public enum ERecodMessagetype
        {
            UdpIn,
            UdpOut,
            TcpIn,
            TcpOut
        }

        public class RecodMessageItem : BaseRefCounter
        {
            LoggerAdapter _logger = new LoggerAdapter(typeof(RecodMessageItem));
            public class ObjcetFactory : CustomAbstractObjectFactory
            {
                public ObjcetFactory() : base(typeof(RecodMessageItem))
                {
                }

                public override object MakeObject()
                {
                    return new RecodMessageItem();
                }
            }

            public static RecodMessageItem Allocate(INetworkChannel channel, int messageType, object messageBody, int stage,
                int seq, ERecodMessagetype recodMessagetype)
            {
                var rc = ObjectAllocatorHolder<RecodMessageItem>.Allocate();
                rc.ChannelId = channel.Id;
                rc.MessageBody = messageBody;
                rc.MessageType = messageType;
                rc.ProcessSeq = seq;
                rc.Stage = stage;
                rc.RecodMessagetype = recodMessagetype;
                if (rc.MessageBody != null && rc.MessageBody is IRefCounter)
                {
                    (rc.MessageBody as IRefCounter).AcquireReference();
                }

                return rc;
            }

            public static RecodMessageItem Allocate()
            {
                var rc = ObjectAllocatorHolder<RecodMessageItem>.Allocate();

                return rc;
            }


            public override void AcquireReference()
            {
                if (MessageBody != null && MessageBody is IRefCounter)
                {
                    (MessageBody as IRefCounter).AcquireReference();
                }

                base.AcquireReference();
            }

            public override void ReleaseReference()
            {
                if (MessageBody != null && MessageBody is IRefCounter)
                {
                    (MessageBody as IRefCounter).ReleaseReference();
                }


                base.ReleaseReference();
            }

            private RecodMessageItem()
            {
            }

            public int Stage;
            public int ChannelId;
            public int MessageType;
            public object MessageBody;
            public int ProcessSeq;
            public ERecodMessagetype RecodMessagetype;


            public override string ToString()
            {
                return string.Format("{0}, Stage: {1}, ChannelId: {2}, MessageType: {3}, ProcessSeq: {4}, RecodMessagetype: {5}", "", Stage, ChannelId, MessageType, ProcessSeq, (int)RecodMessagetype);
            }

            protected override void OnCleanUp()
            {
                ObjectAllocatorHolder<RecodMessageItem>.Free(this);
            }

            public void Write()
            {
            }

            public void Write(ISerializeInfo serializeInfo, MemoryStream _stream, BinaryFileAppender _fileAppender)
            {
                _stream.Seek(0, SeekOrigin.Begin);
                serializeInfo.Serialize(_stream, MessageBody);

                long lenght = _stream.Position;

                _fileAppender.Write((int)Stage);
                _fileAppender.Write((int)ProcessSeq);
                _fileAppender.Write((int)ChannelId);
                _fileAppender.Write((int)MessageType);
                _fileAppender.Write((int) lenght);
                _fileAppender.Write(_stream);
                //_logger.InfoFormat("Write:{0}, {1} ,{2} ",this, lenght, _fileAppender.Offset);
                _stream.Position = 0;
            }


            public bool ReadFrom(BinaryFileReader filerReader, IMessageTypeInfo messageTypeInfo)
            {
                try
                {
                    var readerReader = filerReader.Reader;
                    Stage = readerReader.ReadInt32();
                    ProcessSeq = readerReader.ReadInt32();
                    ChannelId = readerReader.ReadInt32();
                    MessageType = readerReader.ReadInt32();
                    
                    int Lenght = readerReader.ReadInt32();
                    //_logger.InfoFormat("ReadFrom:{0}, {1} ,{2}",this, Lenght,filerReader.Offset);
                    VNetPacketMemSteam memoryStream = VNetPacketMemSteam.Allocate();
                    try
                    {

                        var b=readerReader.ReadBytes(Lenght);
                        memoryStream.Stream.Write(b,0,b.Length);
                        memoryStream.Stream.Seek(0, SeekOrigin.Begin);
                        var serializeInfo = messageTypeInfo.GetSerializeInfo(MessageType);
                        if (serializeInfo != null)
                        {
                            try
                            {
                                MessageBody = serializeInfo.Deserialize( memoryStream.Stream);
                                return true;
                            }
                            catch (Exception e)
                            {
                                _logger.ErrorFormat("{0} {1}",memoryStream.Stream.Position, e);
                            }
                           
                        }
                    }
                    finally
                    {
                        memoryStream.ReleaseReference();
                    }
                   
                   
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("{0}", e);
                }

                return false;
            }
        }
    }
}