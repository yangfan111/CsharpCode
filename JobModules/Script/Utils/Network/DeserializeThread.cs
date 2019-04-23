using Core.Utils;

namespace Core.Network
{
    public class DeserializeThread : ChannelWorker
    {
        protected override bool DoProcessChannel(AbstractNetowrkChannel channel)
        {
            int count = channel.ProcessDeserializeQueue();
            return count > 0;
        }


        public DeserializeThread(ReadWriteList<AbstractNetowrkChannel> holder, int threadIdx, int threadCount) : base(holder, threadIdx, threadCount)
        {
        }
    }
}