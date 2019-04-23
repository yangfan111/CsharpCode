using Core.Network;
using Core.Utils;

namespace Utils.Network
{
    public class SerializeAndDeserializeThread: ChannelWorker
    {
        protected override bool DoProcessChannel(AbstractNetowrkChannel channel)
        {
            int serializeCount = channel.ProcessSerializeQueue(true);
            int deserializeCount = channel.ProcessDeserializeQueue(true);
            return serializeCount > 0||serializeCount>0;
        }


        public SerializeAndDeserializeThread(ReadWriteList<AbstractNetowrkChannel> holder, int threadIdx, int threadCount,string name) : base(holder, threadIdx, threadCount,name)
        {
        }
    }
}

