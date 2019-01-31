using System;
using System.Collections.Generic;
using System.Net.Configuration;
using System.Reflection;
using System.Text;
using System.Threading;
using App.Shared.DebugSystem;
using App.Shared.SceneTriggerObject;
using BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody;
using Utils.AssetManager;
using Core.EntityComponent;
using Core.Http;
using Core.Network;
using Core.Network.ENet;
using Core.ObjectPool;
using Core.Replicaton;
using Core.SnapshotReplication.Serialization.Patch;
using Core.Utils;
using Version = Core.Utils.Version;
using com.wd.free.debug;
using com.wd.free.action;
using Utils.Singleton;

namespace App.Shared
{
    public class MyHttpServer
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(MyHttpServer));
        private static SimpleHttpServer webserver;

        public static void Stop()
        {
            if (webserver != null)
            {
                try
                {
                    webserver.Stop();
                }
                catch (Exception e)
                {
                    _logger.InfoFormat("Stop MyHttpServer:{0}", e);
                }

            }
        }

        public static void Start(int port)
        {
            try
            {
                _logger.InfoFormat("Start MyHttpServer:{0}", port);
                if (webserver != null)
                {
                    try
                    {
                        webserver.Stop();
                    }
                    catch (Exception e)
                    {
                        _logger.InfoFormat("Stop MyHttpServer:{0}", e);
                    }
                }

                webserver = new SimpleHttpServer("/non-existing-folder", port);
                webserver.AddPageHandler("/ObjectPool", new ObjectAllocatorPageHandler());
                webserver.AddPageHandler("/EntityMap", new EntityMapComparePageHandler());
                webserver.AddPageHandler("/Network", new ENetNetworkHandler());
                webserver.AddPageHandler("/BandWidthMonitor", new BandWidthMonitorHandler());
                webserver.AddPageHandler("/SanpShotData", new SnapSHotHandler());
                webserver.AddPageHandler("/fps", new FpsHandler());
                webserver.AddPageHandler("/debug", new DebugHandler());
                webserver.AddPageHandler("/rigidbody", new RigidbodyInfoHandler());
                webserver.AddPageHandler("/res", new LoadResHandler(true));
                webserver.AddPageHandler("/resall", new LoadResHandler(false));
                webserver.AddPageHandler("/triggerobject", new TriggerObjectDebugHandler());
                webserver.AddPageHandler("/all", new AllPageHandler());
                webserver.AddPageHandler("/freelog-var", new FreeDebugDataHandler(1));
                webserver.AddPageHandler("/freelog-message", new FreeDebugDataHandler(2));
                webserver.AddPageHandler("/freelog-func", new FreeDebugDataHandler(3));
                webserver.AddPageHandler("/threads", new ThreadDebugHandler());
                webserver.AddPageHandler("/server", new ServerInfoHandler());
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Start Http server failed: {0}", e);
            }
        }

        public static void AddServe(string path,IHttpRequestHandler handler)
        {
            webserver.AddPageHandler(path, handler);
        }
    }

    class FreeDebugDataHandler : AbstractHttpRequestHandler
    {
        private int type;

        public FreeDebugDataHandler(int type)
        {
            this.type = type;
        }

        public override string HandlerRequest()
        {
            switch (type)
            {
                case 1:
                    return string.Join("\n\n", FreeLog.vars.ToArray());
                case 2:
                    return string.Join("\n\n", FreeLog.messages.ToArray());
                case 3:
                    return string.Join("\n\n", FreeLog.funcs.ToArray());
                default:
                    break;
            }
            return string.Empty;
        }
    }

    class AllPageHandler : AbstractHttpRequestHandler
    {
        public override string HandlerRequest()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div><p>==================================================================</div>");
            sb.Append(ObjectAllocators.PrintAllDebugInfo());
            sb.Append("<div><p>==================================================================</div>");
            sb.Append(EntityMapComparator.PrintDebugInfo());
            sb.Append("<div><p>==================================================================</div>");
            sb.Append(AbstractNetworkService.PrintDebugInfo());
            return sb.ToString();
        }
    }

    class ObjectAllocatorPageHandler : AbstractHttpRequestHandler
    {
        public override string HandlerRequest()
        {
            return ObjectAllocators.PrintAllDebugInfo();
        }
    }

    class EntityMapComparePageHandler : AbstractHttpRequestHandler
    {
        public override string HandlerRequest()
        {
            return EntityMapComparator.PrintDebugInfo();
        }
    }

    class ENetNetworkHandler : AbstractHttpRequestHandler
    {
        public override string HandlerRequest()
        {
            return AbstractNetworkService.PrintDebugInfo();
        }
    }

    class BandWidthMonitorHandler : AbstractHttpRequestHandler
    {
        public override string HandlerRequest()
        {
            return "Hello World";
        }
    }

    class SnapSHotHandler : AbstractHttpRequestHandler
    {
        public override string HandlerRequest()
        {
            return ModifyComponentPatch.PrintDebugInfo;
        }
    }

    public class FpsHandler : AbstractHttpRequestHandler
    {
        public override string HandlerRequest()
        {
            return ModifyComponentPatch.PrintDebugInfo;
        }
    }

    public class LoadResHandler : AbstractHttpRequestHandler
    {
        private bool filter;

        public LoadResHandler(bool b)
        {
            filter = b;
        }

        public override string HandlerRequest()
        {
            return SingletonManager.Get<LoadRequestProfileHelp>().GetHtml(filter);
        }
    }

    public class RigidbodyInfoHandler : AbstractHttpRequestHandler
    {

        public override string HandlerRequest()
        {

            var infos = RigidbodyDebugInfoSystem.GetDebugInfoOnBlock();
            if (infos == null)
                return "Invalid Debug Info";

            int infoCount = infos.Count;
            int activeCount = 0, activeKinematicCount = 0, kinematicCount = 0, sleepingCount = 0;
            var activeList = new List<RigidbodyInfo>();
            var deactiveList = new List<RigidbodyInfo>();
            for (int i = 0; i < infoCount; ++i)
            {
                var info = infos[i];
                if (info.IsActive)
                {
                    activeList.Add(info);
                    activeCount++;
                    if (info.IsKinematic)
                    {
                        activeKinematicCount++;
                    }

                    if (info.IsSleeping)
                    {
                        sleepingCount++;
                    }
                }
                else
                {
                    deactiveList.Add(info);
                }

                if (info.IsKinematic)
                {
                    kinematicCount++;
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(
                "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"><script src=\"https://code.jquery.com/jquery-1.10.2.js\"></script><link rel=\"stylesheet\" type=\"text/css\" href=\"https://cdn.datatables.net/1.10.19/css/jquery.dataTables.css\"><script type=\"text/javascript\" charset=\"utf8\" src=\"https://cdn.datatables.net/1.10.19/js/jquery.dataTables.js\"></script><script>$(document).ready( function () {$('#table_id').DataTable({paging: false});});</script></head><body>");

            sb.Append("<p>").Append("Summary ").Append("Total: ").Append(infoCount).Append("  ").Append("ActiveCount: ")
                .Append(activeCount).Append("  ").Append("KinematicCount: ").Append(kinematicCount).Append("  ")
                .Append("ActiveKinematicCount: ").Append(activeKinematicCount).Append("  ").Append("SleepingCount: ")
                .Append(sleepingCount).Append("  ").Append("</p>");
            sb.Append(
                "<table id=\"table_id\" class=\"display\" width='400px' border='1' align='center' cellpadding='2' cellspacing='1'>");
            sb.Append("<thead>");
            sb.Append("<td width='10%'>Name</td>");
            sb.Append("<td>EntityKey</td>");
            sb.Append("<td>Active</td>");
            sb.Append("<td>Kinematic</td>");
            sb.Append("<td>Sleeping</td>");
            sb.Append("<td>Position</td>");
            sb.Append("<td>Velocity</td>");
            sb.Append("</thead>");

            AppendInfos(sb, activeList);
            AppendInfos(sb, deactiveList);

            return sb.ToString();
        }

        private void AppendInfos(StringBuilder sb, List<RigidbodyInfo> infos)
        {
            int count = infos.Count;
            for (int i = 0; i < count; ++i)
            {
                var info = infos[i];
                sb.Append("<tr>");
                sb.Append("<td>").Append(info.Name).Append("</td>");
                sb.Append("<td>").Append(info.EntityKey).Append("</td>");
                sb.Append("<td>").Append(info.IsActive).Append("</td>");
                sb.Append("<td>").Append(info.IsKinematic).Append("</td>");
                sb.Append("<td>").Append(info.IsSleeping).Append("</td>");
                sb.Append("<td>").Append(info.Position).Append("</td>");
                sb.Append("<td>").Append(info.Velocity).Append("</td>");
                sb.Append("</tr>");
            }
        }
    }

    public class DebugHandler : AbstractHttpRequestHandler
    {
        public DebugHandler()
        {

        }

        public override string HandlerRequest()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(
                "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"><script src=\"https://code.jquery.com/jquery-1.10.2.js\"></script><link rel=\"stylesheet\" type=\"text/css\" href=\"https://cdn.datatables.net/1.10.19/css/jquery.dataTables.css\"><script type=\"text/javascript\" charset=\"utf8\" src=\"https://cdn.datatables.net/1.10.19/js/jquery.dataTables.js\"></script><script>$(document).ready( function () {$('#table_id').DataTable({paging: false});});</script></head><body>");
            sb.Append("<p>").Append("exe:").Append(Version.Instance.LocalVersion).Append("</p>");
            sb.Append("<p>").Append("asset:").Append(Version.Instance.LocalVersion).Append("</p>");
            sb.Append("<p>").Append("GC:")
                .Append(System.GC.CollectionCount(0) + System.GC.CollectionCount(1) + System.GC.CollectionCount(2))
                .Append("</p>");
            sb.Append("<p>").Append("GCTotal:").Append(System.GC.GetTotalMemory(false) / 1024 / 1024).Append("MB</p>");
            sb.Append(SingletonManager.Get<DurationHelp>().GetHtmlTable());
#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
            //_debugHelper.GetSessionStateMachine().GetUpdateSystems().
#endif
            sb.Append("</body></html>");
            return sb.ToString();
        }
    }

    public class TriggerObjectDebugHandler : AbstractHttpRequestHandler
    {
        private StringBuilder _sb;

        public override string HandlerRequest()
        {
            _sb = new StringBuilder();
            _sb.Append("<table width='400px' border='1' align='center' cellpadding='2' cellspacing='1'>");
            _sb.Append("<thead>");
            _sb.Append("<td>SceneIndex</td>");
            _sb.Append("<td>Count</td>");
            _sb.Append("<td>AverageCost</td>");
            _sb.Append("</thead>");
            TriggerObjectLoadProfiler.Iterate(ProcessLoadTime);
            return _sb.ToString();
        }

        private void ProcessLoadTime(int index1, int index2, int totalCount, float totalCost)
        {
            _sb.Append("<tr>");
            _sb.Append("<td>").Append(index1).Append("x").Append(index2).Append("</td>");
            _sb.Append("<td>").Append(totalCount).Append("</td>");
            _sb.Append("<td>").Append(totalCount > 0 ? totalCost / totalCount : 0).Append("</td>");
            _sb.Append("</tr>");
        }
    }

    public class ThreadDebugHandler : AbstractHttpRequestHandler
    {
        private StringBuilder _sb;

        public override string HandlerRequest()
        {
            _sb = new StringBuilder();
            var threadInfos = AbstractThread.Statistics.AllThreadInfos;
            _sb.Append(
                "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"><script src=\"https://code.jquery.com/jquery-1.10.2.js\"></script><link rel=\"stylesheet\" type=\"text/css\" href=\"https://cdn.datatables.net/1.10.19/css/jquery.dataTables.css\"><script type=\"text/javascript\" charset=\"utf8\" src=\"https://cdn.datatables.net/1.10.19/js/jquery.dataTables.js\"></script><script>$(document).ready( function () {$('#table_id').DataTable({paging: false});});</script></head><body>");

            _sb.Append(
                "<table id=\"table_id\" width='800px' border='1' align='center' cellpadding='2' cellspacing='1'>");
            _sb.Append("<thead>");
            _sb.Append("<td>name</td>");
            _sb.Append("<td>State</td>");
            _sb.Append("<td>rate</td>");

            _sb.Append("</thead>");
            foreach (var thread in threadInfos)
            {
                _sb.Append("<tr>");
                _sb.Append("<td>");
                _sb.Append(thread.Name);
                _sb.Append("</td>");
                _sb.Append("<td>");
                _sb.Append(thread.State);
                _sb.Append("</td>");
                _sb.Append("<td>");
                _sb.Append(thread.Rate);
                _sb.Append("</td>");
                _sb.Append("</tr>");
            }

            return _sb.ToString();
        }
    }

    public class ServerInfoHandler : AbstractHttpRequestHandler
    {
        public override string HandlerRequest()
        {
            var debugInfo = ServerDebugInfoSystem.GetDebugInfoOnBlock();
            var sb = new StringBuilder();
            sb.Append(
                "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"><script src=\"https://code.jquery.com/jquery-1.10.2.js\"></script><link rel=\"stylesheet\" type=\"text/css\" href=\"https://cdn.datatables.net/1.10.19/css/jquery.dataTables.css\"><script type=\"text/javascript\" charset=\"utf8\" src=\"https://cdn.datatables.net/1.10.19/js/jquery.dataTables.js\"></script><script>$(document).ready( function () {$('#table_id').DataTable({paging: false});});</script></head><body>");

            var roomInfo = debugInfo.RoomDebugInfo;
            if (roomInfo != null)
            {
                sb.Append("<p>").Append("Room State:").Append(roomInfo.State).Append("</p>");
                sb.Append("<p>").Append("Hall Room Id:").Append(roomInfo.HallRoomId).Append("</p>");
                sb.Append("<p>").Append("Room Id:").Append(roomInfo.RoomId).Append("</p>");
            }
            else
            {
                sb.Append("<p>").Append("There is no Server Room").Append("</p>");
            }

            var playerInfo = debugInfo.PlayerDebugInfo;
            if (playerInfo != null)
            {
                sb.Append(
               "<table id=\"table_id\" class=\"display\" width='400px' border='1' align='center' cellpadding='2' cellspacing='1'>");
                sb.Append("<thead>");
                sb.Append("<td>HasPlayerEntity</td>");
                sb.Append("<td>HasPlayerInfo</td>");
                sb.Append("<td>IsRobot</td>");
                sb.Append("<td>IsLogin</td>");
                sb.Append("<td>EntityKey</td>");
                sb.Append("<td>EntityId</td>");
                sb.Append("<td>PlayerId</td>");
                sb.Append("<td>TeamId</td>");
                sb.Append("<td>Name</td>");
                sb.Append("<td>Token</td>");
                sb.Append("<td>CreateTime</td>");
                sb.Append("<td>GameStartTime</td>");
                sb.Append("</thead>");


                foreach (var pinfo in playerInfo)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>");
                    sb.Append(pinfo.HasPlayerEntity);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(pinfo.HasPlayerInfo);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(pinfo.IsRobot);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(pinfo.IsLogin);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(pinfo.EntityKey);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(pinfo.EntityId);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(pinfo.PlayerId);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(pinfo.TeamId);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(pinfo.Name);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(pinfo.Token);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(pinfo.CreateTime);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(pinfo.GameStartTime);
                    sb.Append("</td>");
                    sb.Append("</tr>");
                }
            }

            return sb.ToString();

        }
    }
}
