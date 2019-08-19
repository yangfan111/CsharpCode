using System;

namespace Utils.Replay
{
    public interface IRecordManager : IDisposable
    {
        void AddMessage(EReplayMessageType type, NetworkMessageRecoder.RecodMessageItem item);
        ReplayInfo Info { get; }
        void UpdateInfoToFile();
    }
}