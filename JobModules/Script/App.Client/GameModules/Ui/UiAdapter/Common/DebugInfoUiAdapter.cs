using System;
using App.Client.GameModules.Ui.UiAdapter.Interface.Common;
using Core.GameModule.Step;
using Core.Utils;
using System.Text;
using App.Shared;
using Core.Components;
using Core.OC;
using Sharpen;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.UiAdapter.Common
{
    public class DebugInfoUiAdapter : UIAdapter, IDebugInfoUiAdapter
    {
        private Contexts _contexts;

        public DebugInfoUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }

        public string VersionDebugInfo
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("client :{0} asset;{1}", Core.Utils.Version.Instance.LocalVersion,
                    Core.Utils.Version.Instance.LocalAsset));
                sb.AppendLine(string.Format("server :{0} asset;{1}", Core.Utils.Version.Instance.RemoteVersion,
                    Core.Utils.Version.Instance.RemoteAsset));
                sb.AppendLine(SingletonManager.Get<DurationHelp>().LastAvg);
                sb.AppendLine(string.Format(" Rewind:{0} time:{1}  pos:{2:N2} {3:N2} {4:N2} origin:{5:N2} {6:N2} {7:N2} ",
                    SingletonManager.Get<DurationHelp>().RewindCount,
                    SingletonManager.Get<DurationHelp>().DriveTime,
                    SingletonManager.Get<DurationHelp>().Position.x + WorldOrigin.Origin.x,
                    SingletonManager.Get<DurationHelp>().Position.y + WorldOrigin.Origin.y,
                    SingletonManager.Get<DurationHelp>().Position.z + WorldOrigin.Origin.z,
                    WorldOrigin.Origin.x,
                    WorldOrigin.Origin.y,
                    WorldOrigin.Origin.z
                ));
                sb.AppendLine(string.Format("{0}   Interval:{1} , Delta:{2} rTime:{3} sTime:{4} d:{5}",
                    StepExecuteManager.Instance.FpsString(), SingletonManager.Get<DurationHelp>().LastAvgInterpolateInterval,
                    SingletonManager.Get<DurationHelp>().ServerClientDelta, SingletonManager.Get<DurationHelp>().RenderTime, SingletonManager.Get<DurationHelp>().LastServerTime, SingletonManager.Get<DurationHelp>().LastServerTime - SingletonManager.Get<DurationHelp>().RenderTime));
                sb.AppendLine(string.Format("serverip: {0}, serverId:{1}", SingletonManager.Get<DurationHelp>().ServerInfo, SingletonManager.Get<Core.Utils.ServerInfo>().ServerId));
                sb.AppendLine(string.Format("ocpvs: {0} ocen: {1}", OcclusionRunningState.HasPVSData ? "T" : "F",
                    OcclusionRunningState.OcclusionEnabled ? "T" : "F"));
                return sb.ToString();
            }
        }

        public string PingDebugInfo
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                var sessionObjects = _contexts.session.clientSessionObjects;

                var tt = (int)(DateTime.UtcNow.ToMillisecondsSinceEpoch() -
                               sessionObjects.ServerFpsSatatus.LastTcpPing) / 1000;

                var ut = (int)(DateTime.UtcNow.ToMillisecondsSinceEpoch() -
                               sessionObjects.ServerFpsSatatus.LastUdpPing) / 1000;

                sb.AppendLine(string.Format("client :{0}", sessionObjects.FpsSatatus));
                sb.AppendLine(string.Format("server :{0}", _contexts.session.clientSessionObjects.ServerFpsSatatus));
                sb.AppendLine(string.Format("ping t:{0}, u:{1}, tt:{2}, ut:{3}",
                    sessionObjects.ServerFpsSatatus.TcpPing, sessionObjects.ServerFpsSatatus.UdpPing,
                    tt, ut));
                if (!SharedConfig.IsOffline)
                {
                    sb.AppendLine(string.Format("tcp:{0}",
                        _contexts.session.clientSessionObjects.NetworkChannel.TcpFlowStatus));

                    sb.AppendLine(string.Format("udp:{0}",
                        _contexts.session.clientSessionObjects.NetworkChannel.UdpFlowStatus));
                }
                return sb.ToString();
            }
        }
    }
}
