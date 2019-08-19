using System;

namespace Utils.Replay
{
    public interface IReplayManager:IDisposable
    {
        NetworkMessageRecoder.RecodMessageItem GetItem(EReplayMessageType @out, int stage, int seq, int networkChannelId);
        ReplayInfo Info { get; }
    }
}