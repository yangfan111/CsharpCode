using System;
using System.IO;
using System.Xml.Serialization;
using App.Shared.Network;
using Utils.Replay;
using NetworkMessageRecoder = Utils.Replay.NetworkMessageRecoder;
using Version = Core.Utils.Version;

namespace App.Client.GameModules.Replay
{
    public class RecordManager : IRecordManager
    {
        private ReplayInfo _info;
        INetworkMessageRecoder _in;
        INetworkMessageRecoder _out;
        INetworkMessageRecoder _cmd;
        private string _infoFile;
        private string _path;

        public RecordManager(string path)
        {
            _infoFile = System.IO.Path.Combine(path, "info.xml");
            _path = path;
            _info = new ReplayInfo();
            _info.LocalVersion = Version.Instance.LocalVersion;
            _info.LocalAsset = Version.Instance.LocalAsset;
            _info.RemoteAsset = Version.Instance.RemoteAsset;
            _info.RemoteVersion = Version.Instance.RemoteVersion;
            _info.DateTime = String.Format("{0:yyyy_M_d_HH_mm_ss}_{1}", DateTime.Now,new Random().Next(1,1000));
            _info.InBinName = System.IO.Path.Combine(path, "in.bin");
            _info.OutBinName = System.IO.Path.Combine(path, "out.bin");
            System.IO.Directory.CreateDirectory(path);

            UpdateInfoToFile();
            _in =
                new NetworkMessageRecoder(_info.InBinName,
                    new AppMessageTypeInfo("recoder", 1));
            _out =
                new NetworkMessageRecoder(_info.OutBinName,
                    new AppMessageTypeInfo("recoder", 1));
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


        public void AddMessage(EReplayMessageType type, NetworkMessageRecoder.RecodMessageItem item)
        {
            if (type == EReplayMessageType.IN)
            {
                _in.AddMessage(item);
            }
            else if (type == EReplayMessageType.OUT)
            {
                _out.AddMessage(item);
            }
            else if (type == EReplayMessageType.GM)
            {
                _cmd.AddMessage(item);
            }
        }

        private INetworkMessageRecoder GetRecoder(EReplayMessageType type)
        {
            if (type == EReplayMessageType.IN)
            {
                return _in;
            }
            else if (type == EReplayMessageType.OUT)
            {
                return _out;
            }
            else if (type == EReplayMessageType.GM)
            {
                return _cmd;
            }

            return null;
        }

        public ReplayInfo Info
        {
            get { return _info; }
        }

        public void UpdateInfoToFile()
        {
            ReplayInfo.WriteToFile(_infoFile, _info);
        }
    }
}