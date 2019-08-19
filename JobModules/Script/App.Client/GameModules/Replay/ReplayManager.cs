using App.Shared.Network;
using Utils.Replay;

namespace App.Client.GameModules.Replay
{
    public class ReplayManager : IReplayManager
    {
        INetworkMessageReplay _in;
        INetworkMessageReplay _out;
        INetworkMessageReplay _cmd;
        private ReplayInfo _info;

        public ReplayManager(string path)
        {
            var infoFile = System.IO.Path.Combine(path, "info.xml");
            _info = ReplayInfo.ReadFromFile(infoFile);
            _in =
                new NetworkMessageReplay(_info.InBinName,
                    new AppMessageTypeInfo("replay.in", 1));
            _out =
                new NetworkMessageReplay(_info.OutBinName,
                    new AppMessageTypeInfo("replay.out", 1));
        }

        public void Dispose()
        {
            if (_in != null)
                _in.Dispose();
            if (_out != null)
                _out.Dispose();
            if (_cmd != null)
                _cmd.Dispose();
        }

        public NetworkMessageRecoder.RecodMessageItem GetItem(EReplayMessageType type, int stage, int seq,
            int networkChannelId)
        {
            if (type == EReplayMessageType.IN)
            {
                return _in.GetItem(stage, seq, networkChannelId);
            }

            if (type == EReplayMessageType.OUT)
            {
                return _out.GetItem(stage, seq, networkChannelId);
            }

            if (type == EReplayMessageType.GM)
            {
                return _cmd.GetItem(stage, seq, networkChannelId);
            }

            return null;
        }

        public ReplayInfo Info
        {
            get { return _info; }
        }
    }
}