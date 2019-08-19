using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Core.Utils;
using Utils.Network;


namespace Core.Network
{
    public abstract class AbstractNetworkService : IDisposable
    {
        public bool _isMultiThreaded;

        private LoggerAdapter _logger = new LoggerAdapter(typeof(AbstractNetworkService));
        private static List<AbstractNetworkService> _activeNetworks = new List<AbstractNetworkService>();
        private static List<AbstractThread> _activeThreads = new List<AbstractThread>();
        private bool _sendRecvInMain;

        private static int _activeThreadCount;

//        private List<SerializeThread> _serializeThreads = new List<SerializeThread>();
//        private List<DeserializeThread> _deserializeThreads = new List<DeserializeThread>();
        private List<SerializeAndDeserializeThread> _serializeAndDeserializeThreads =
            new List<SerializeAndDeserializeThread>();

        private ReadWriteList<AbstractNetowrkChannel> _channels = new ReadWriteList<AbstractNetowrkChannel>();
        private volatile bool _isThreadRunning;
        private volatile bool _isStarted;
        private AbstractThread _sendRecvThread;
        protected string ServiceName;

        protected AbstractNetworkService(string serviceName)
        {
            ServiceName = serviceName;
        }

        public static List<AbstractNetworkService> ActiveNetworks
        {
            get { return _activeNetworks; }
        }

        public static string PrintDebugInfo()
        {
            StringBuilder sb = new StringBuilder();
          


            sb.Append("<p>Serialize Info</p>");
            foreach (var netowrk in ActiveNetworks)
            {
                sb.Append("<p>").Append(netowrk.ServiceName).Append("</p>");
                sb.Append("<table width='400px' border='1' align='center' cellpadding='2' cellspacing='1'>");
                sb.Append("<thead>");
                sb.Append("<td>network</td>");
                sb.Append("<td>channel</td>");
                sb.Append("<td>send queue</td>");
                sb.Append("<td>revv queue</td>");
                sb.Append("<td>serialize queue</td>");
                sb.Append("<td>deserialize queue</td>");
                sb.Append("<td>statistcs</td>");
                sb.Append("</thead>");


                foreach (var channel in netowrk.Channels)
                {
                    sb.Append("<tr>");

                    sb.Append("<td>");
                    sb.Append(netowrk.GetType().Name);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(channel.IdInfo()).Append("\n").Append(channel.DebugInfo());
                    sb.Append("</td>");


                    sb.Append("<td>");
                    sb.Append(0);
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(channel.RecvQueueCount);
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(channel.SerializeQueueCount);
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(channel.DeserializeQueueCount);
                    sb.Append("</td>");

                    sb.Append("<td>");
                    channel.Serializer.MessageTypeInfo.PrintDebugInfo(sb);
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(channel.TcpFlowStatus.ToString());
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(channel.UdpFlowStatus.ToString());
                    sb.Append("</td>");


                    sb.Append("</tr>");
                }
            }
           
            return sb.ToString();
        }


        public void AddChannel(AbstractNetowrkChannel channel)
        {
            _channels.Add(channel);
        }

        public void RemoveChannel(AbstractNetowrkChannel channel)
        {
            _channels.Remove(channel);
        }

        public void Start( int threadCount = 1, bool isSendRecvInMain = true, bool isMultiThreaded = true)
        {
            if (_isStarted)
            {
                throw new Exception("alread started");
            }

            _isStarted = true;
            
            _activeNetworks.Add(this);
            _sendRecvInMain = isSendRecvInMain;
            _isMultiThreaded = isMultiThreaded;
            if (_isMultiThreaded)
            {
                _isThreadRunning = true;
                for (int i = 0; i < threadCount; i++)
                {

                    SerializeAndDeserializeThread thread = new SerializeAndDeserializeThread(_channels, i, threadCount,ServiceName +"_ SerializeAndDeserialize_" +i);
                    _serializeAndDeserializeThreads.Add(thread);
                    _activeThreads.Add(thread);
                    thread.Start();
                }

                if (!_sendRecvInMain)
                {
                    _sendRecvThread = new PollServiceThread(ProcessSendReceive, 2, ServiceName +"_PollServiceThread");
                    _sendRecvThread.Start();
                    _activeThreads.Add(_sendRecvThread);
                }
            }
        }

        public void Update()
        {
            if (!_isMultiThreaded)
            {
                foreach (var channel in _channels.ForRead())
                {
                    channel.ProcessSerializeQueue();
                    channel.ProcessDeserializeQueue();
                }
            }

            if (!_isMultiThreaded || _sendRecvInMain)
            {
                ProcessSendReceive(false);
            }
        }

        public void FlowTick(float time)
        {
            foreach (var channel in _channels.ForRead())
            {
                channel.FlowTick(time);
            }
        }

       

        public ICollection<AbstractNetowrkChannel> Channels
        {
            get { return _channels.ForRead(); }
        }

        public virtual void Dispose()
        {
            _isStarted = false;

            _logger.InfoFormat("dispose thread {0}", this);
            _activeNetworks.Remove(this);

            if (_isThreadRunning)
            {
                _isThreadRunning = false;
                if (!_sendRecvInMain)
                {
                    _sendRecvThread.Dispose();
                    _activeThreads.Remove(_sendRecvThread);
                }

//                for (var i = 0; i < _serializeThreads.Count; i++)
//                {
//                    _serializeThreads[i].Stop();
//                    _deserializeThreads[i].Stop();
//                }
                foreach (var serializeAndDeserializeThread in _serializeAndDeserializeThreads)
                {
                    serializeAndDeserializeThread.Dispose();
                    _activeThreads.Remove(serializeAndDeserializeThread);
                }
            }

            Close();
        }

//        public SerializeThread GetSerializeThread(int channelId)
//        {
//            return _serializeThreads[channelId % _serializeThreads.Count];
//        }
//
//        public DeserializeThread GetDeSerializeThread(int channelId)
//        {
//            return _deserializeThreads[channelId % _deserializeThreads.Count];
//        }

        

        private void ProcessSendReceive(bool isMultiThread)
        {
           
            Poll();
        }

        protected abstract void Poll();
        public abstract void Close();

        public bool IsStarted
        {
            get { return _isStarted; }
        }

        public static int ActiveThreadCount
        {
            get { return _activeThreadCount; }
        }
    }
}