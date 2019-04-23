using Core.Utils;

namespace Core.Network
{
    public class SerializeThread : ChannelWorker
    {
        protected override bool DoProcessChannel(AbstractNetowrkChannel channel)
        {
            int count = channel.ProcessSerializeQueue();
            return count > 0;
        }


        public SerializeThread(ReadWriteList<AbstractNetowrkChannel> holder, int threadIdx, int threadCount) : base(holder, threadIdx, threadCount)
        {
        }
    }
}