using System;
using System.Collections.Generic;
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
        public static void Start(int port, IEcsDebugHelper debugHelper)
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
                webserver.AddPageHandler("/debug", new DebugHandler(debugHelper));
                webserver.AddPageHandler("/rigidbody", new RigidbodyInfoHandler());
                webserver.AddPageHandler("/res", new LoadResHandler(true));
                webserver.AddPageHandler("/resall", new LoadResHandler(false));
                webserver.AddPageHandler("/triggerobject", new TriggerObjectDebugHandler());
                webserver.AddPageHandler("/all", new AllPageHandler());
                webserver.AddPageHandler("/freelog-var", new FreeDebugDataHandler(1));
                webserver.AddPageHandler("/freelog-message", new FreeDebugDataHandler(2));
                webserver.AddPageHandler("/freelog-func", new FreeDebugDataHandler(3));
                
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Start Http server failed: {0}", e);
            }
        }

        class FreeDebugDataHandler : IHttpRequestHandler
        {
            private int type;

            public FreeDebugDataHandler(int type)
            {
                this.type = type;
            }
            public string GetResponse()
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

        class AllPageHandler : IHttpRequestHandler
        {
            public string GetResponse()
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
        class ObjectAllocatorPageHandler : IHttpRequestHandler
        {
            public string GetResponse()
            {
                return ObjectAllocators.PrintAllDebugInfo();
            }
        }


        class EntityMapComparePageHandler : IHttpRequestHandler
        {
            public string GetResponse()
            {
                return EntityMapComparator.PrintDebugInfo();
            }
        }


        class ENetNetworkHandler : IHttpRequestHandler
        {
            public string GetResponse()
            {
                return AbstractNetworkService.PrintDebugInfo();
            }
        }

        class BandWidthMonitorHandler : IHttpRequestHandler
        {
            public string GetResponse()
            {
                return "Hello World";
            }
        }

        class SnapSHotHandler : IHttpRequestHandler
        {
            public string GetResponse()
            {
                return ModifyComponentPatch.PrintDebugInfo;
            }
        }

        public class FpsHandler : IHttpRequestHandler
        {
            public string GetResponse()
            {
                return ModifyComponentPatch.PrintDebugInfo;
            }
        }
    }

    public class LoadResHandler : IHttpRequestHandler
    {
        private bool filter;
        public LoadResHandler(bool b)
        {
            filter = b;
        }

        public string GetResponse()
        {
            return SingletonManager.Get<LoadRequestProfileHelp>().GetHtml(filter);
        }
    }

    public class RigidbodyInfoHandler : IHttpRequestHandler
    {
        private int _startKey;

        public string GetResponse()
        {

            RigidbodyDebugSystem.Start(++_startKey);

            int sleepCount = 0;
            while (!RigidbodyDebugSystem.Ready)
            {
                Thread.Sleep(1000);
                sleepCount++;
                if (sleepCount > 60)
                {
                    return "Time Out!";
                }
            }

            var infos = RigidbodyDebugSystem.Infos;
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
            sb.Append("<p>").Append("Summary ").Append("Total: ").Append(infoCount).Append("  ").
                Append("ActiveCount: ").Append(activeCount).Append("  ").
                Append("KinematicCount: ").Append(kinematicCount).Append("  ").
                Append("ActiveKinematicCount: ").Append(activeKinematicCount).Append("  ").
                Append("SleepingCount: ").Append(sleepingCount).Append("  ").
                Append("</p>");
            sb.Append("<table width='1000px' border='1' align='center' cellpadding='2' cellspacing='1'>");
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


    public class DebugHandler : IHttpRequestHandler
    {
        private IEcsDebugHelper _debugHelper;
        public DebugHandler(IEcsDebugHelper debugHelper)
        {
            _debugHelper = debugHelper;
        }

        public string GetResponse()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(
                "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"><script src=\"https://code.jquery.com/jquery-1.10.2.js\"></script><link rel=\"stylesheet\" type=\"text/css\" href=\"https://cdn.datatables.net/1.10.19/css/jquery.dataTables.css\"><script type=\"text/javascript\" charset=\"utf8\" src=\"https://cdn.datatables.net/1.10.19/js/jquery.dataTables.js\"></script><script>$(document).ready( function () {$('#table_id').DataTable({paging: false});});</script></head><body>");
            sb.Append("<p>").Append("exe:").Append(Version.Instance.LocalVersion).Append("</p>");
            sb.Append("<p>").Append("asset:").Append(Version.Instance.LocalVersion).Append("</p>");
            sb.Append("<p>").Append("GC:").Append(System.GC.CollectionCount(0) + System.GC.CollectionCount(1) + System.GC.CollectionCount(2)).Append("</p>");
            sb.Append("<p>").Append("GCTotal:").Append(System.GC.GetTotalMemory(false) / 1024 / 1024).Append("MB</p>");
            sb.Append(SingletonManager.Get<DurationHelp>().GetHtmlTable());
#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
            //_debugHelper.GetSessionStateMachine().GetUpdateSystems().
#endif
            sb.Append("</body></html>");
            return sb.ToString();
        }
    }

    public class TriggerObjectDebugHandler : IHttpRequestHandler
    {
        private StringBuilder _sb;
        public string GetResponse()
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
            _sb.Append("<td>").Append(index1).
                Append("x").Append(index2).Append("</td>");
            _sb.Append("<td>").Append(totalCount).Append("</td>");
            _sb.Append("<td>").Append(totalCount > 0 ? totalCost / totalCount : 0).Append("</td>");
            _sb.Append("</tr>");
        }
    }
}