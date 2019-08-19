using System;
using System.Collections;
using System.IO;
using System.Threading;
using Core.Network;
using Core.Utils;

namespace Utils.Replay
{
    public class NetworkMessageRecoderThread : AbstractThread
    {
        public NetworkMessageRecoderThread(string name, INetworkMessageRecoder recoder) : base(name)
        {
            _recoder = recoder;
        }

        private readonly INetworkMessageRecoder _recoder;

        protected override void Run()
        {
            while (Running)
            {
                _recoder.ProcessMessage();
                Thread.Sleep(10);
            }
        }

        public override float Rate { get; }
    }

    public partial class NetworkMessageRecoder : INetworkMessageRecoder
    {
        LoggerAdapter _logger = new LoggerAdapter(typeof(NetworkMessageRecoder));
        private Queue _queue = Queue.Synchronized(new Queue());
        private IMessageTypeInfo _messageTypeInfo;
        private readonly MemoryStream _stream = new MemoryStream();
        private readonly BinaryFileAppender _fileAppender;
      

        public NetworkMessageRecoder(string filename, IMessageTypeInfo messageTypeInfo)
        {
            _messageTypeInfo = messageTypeInfo;
           
            _fileAppender = new BinaryFileAppender(filename);
            new NetworkMessageRecoderThread("recoderThread", this).Start();
        }

        public void AddMessage(NetworkMessageRecoder.RecodMessageItem item)
        {
            _queue.Enqueue(item);
        }

        public void ProcessMessage()
        {
            while (_queue.Count > 0)
            {
                lock (_stream)
                {


                    //var binaryWriter = MyBinaryWriter.Allocate(_stream);
                    _stream.Position = 0;

                    NetworkMessageRecoder.RecodMessageItem item = (NetworkMessageRecoder.RecodMessageItem) _queue.Dequeue();
                    try
                    {

                        var serializeInfo = _messageTypeInfo.GetSerializeInfo(item.MessageType);
                        if (serializeInfo != null)

                            item.Write(serializeInfo, _stream, _fileAppender);


                    }
                    catch (Exception e)
                    {
                        _logger.ErrorFormat("NetworkMessageRecoder thread{0}", e);
                    }
                    finally
                    {
                        item.ReleaseReference();
                    }
                }
            }
        }

        public void Dispose()
        {
            _messageTypeInfo.Dispose();
            _stream.Dispose();
            
        }
    }

    public interface INetworkMessageRecoder:IDisposable
    {
        void AddMessage(NetworkMessageRecoder.RecodMessageItem item);
        void ProcessMessage();
    }
}