using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Common;
using Core.Network;
using Core.Utils;

namespace Utils.Replay
{
    public class NetworkMessageReplayThread : AbstractThread
    {
        LoggerAdapter _logger = new LoggerAdapter(typeof(NetworkMessageReplayThread));

        public NetworkMessageReplayThread(string name, INetworkMessageReplay replay) : base(name)
        {
            _replay = replay;
        }

        private readonly INetworkMessageReplay _replay;

        protected override void Run()
        {
            while (Running)
            {
                try
                {
                    if (!_replay.ReadFromFile())
                    {
                        Thread.Sleep(10);
                    }
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("{0}", e);
                }
            }
        }

        public override float Rate { get; }
    }

    public struct ReplayMessageKey
    {
        public int Channel;
        public int Stage;

        public ReplayMessageKey(int channel, int stage)
        {
            Channel = channel;
            Stage = stage;
        }
    }

    public class ReplayMessageKeyComparer : IEqualityComparer<ReplayMessageKey>, IComparer<ReplayMessageKey>
    {
        private static ReplayMessageKeyComparer _instance = new ReplayMessageKeyComparer();

        public static ReplayMessageKeyComparer Instance
        {
            get { return _instance; }
        }

        public bool Equals(ReplayMessageKey x, ReplayMessageKey y)
        {
            return x.Channel == y.Channel && x.Stage == y.Stage;
        }

        public int GetHashCode(ReplayMessageKey obj)
        {
            return obj.Stage;
        }

        public int Compare(ReplayMessageKey x, ReplayMessageKey y)
        {
            if (x.Stage < y.Stage)
                return -1;
            if (x.Stage > y.Stage)
                return 1;

            return 1;
            return 0;
        }
    }

    public class NetworkMessageReplay : INetworkMessageReplay
    {
        LoggerAdapter _logger = new LoggerAdapter(typeof(NetworkMessageReplay));
        private readonly string _fileName;

        private readonly BinaryFileReader _reader;
        private readonly Dictionary<ReplayMessageKey, Queue> _dictionary = new Dictionary<ReplayMessageKey, Queue>();
        private readonly IMessageTypeInfo _messageTypeInfo;
        private int lastGetSeq = 0;
        private int lastReadSeq = 0;
        private int lastGetStage;
        private int lastReadStage;
        private NetworkMessageReplayThread _thread;
        private HashSet<int> _stageSet = new HashSet<int>();
        public NetworkMessageReplay(string fileName, IMessageTypeInfo messageTypeInfo)
        {
            _fileName = fileName;
            _reader = new BinaryFileReader(fileName);
            _messageTypeInfo = messageTypeInfo;
            _thread = new NetworkMessageReplayThread("replay", this);
            _thread.Start();
        }

        public NetworkMessageRecoder.RecodMessageItem GetItem(int stage, int seq, int channel)
        {
            if (!_stageSet.Contains(stage))
            {
                _stageSet.Add(stage);
            }
            // _logger.InfoFormat("GetItem :{0}, {1}, {2}", stage, seq, channel);
            lastGetSeq = seq;
            lastGetStage = stage;
            Queue queue;
            if (_dictionary.TryGetValue(new ReplayMessageKey(channel, stage), out queue))
            {
                if (queue.Count > 0)
                {
                    NetworkMessageRecoder.RecodMessageItem item = (NetworkMessageRecoder.RecodMessageItem) queue.Peek();
                    if (item.ProcessSeq <= seq)
                    {
                        _logger.DebugFormat("GetItem Succ:{0}, {1}, {2} {3} {4}", stage, seq, channel, item,
                            queue.Count);
                        return (NetworkMessageRecoder.RecodMessageItem) queue.Dequeue();
                    }
                }
            }

            return null;
        }

        public bool ReadFromFile()
        {
            if (!_stageSet.Contains(lastGetStage)) return false;
            if (lastGetStage == lastReadStage && lastReadSeq > lastGetSeq + 30) return false;
            if (!_reader.HasRemain())
            {
                return false;
            }


            NetworkMessageRecoder.RecodMessageItem item = NetworkMessageRecoder.RecodMessageItem.Allocate();


            if (item.ReadFrom(_reader, _messageTypeInfo))
            {
                lastReadSeq = item.ProcessSeq;
                lastReadStage = item.Stage;
                Queue queue;
                var key = new ReplayMessageKey(item.ChannelId, item.Stage);
                if (!_dictionary.TryGetValue(key, out queue))
                {
                    queue = Queue.Synchronized(new Queue());
                    _dictionary[key] = queue;
                }

                queue.Enqueue(item);
                if (queue.Count > 50) return false;
                return true;
            }


            return false;
        }

        public void Dispose()
        {
            _messageTypeInfo.Dispose();
            _thread.Dispose();
        }
    }

    public interface INetworkMessageReplay : IDisposable
    {
        NetworkMessageRecoder.RecodMessageItem GetItem(int stage, int seq, int channel);
        bool ReadFromFile();
    }
}