using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Assets.Sources.Utils.EventLogger;
using Common.EventLogger;
using Common.LogAppender;
using Core.MyProfiler;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using Utils.Appearance;
using Utils.Singleton;

namespace Core.Utils
{
    public enum CustomProfilerStep
    {
        GameController,
        Room,
        UserPrediction,
        UserPredictionCreateTasks,
        UserPredictionInternalRun,
        VechiclePrediction,
        VehicleEntityUpdate,
        VehiclePlayerLayerSet,
        VehicleUpdateSimulationTime,
        VehicleSyncFromComponent,
        VehicleExecuteCmds,
        VehicleFixedUpdate,
        VehicleSimulation,
        VehicleUpdate,
        VehicleSyncToComponent,
        VehiclePlayBack,
        SyncLatest,
        PlaybackInit,
        PlaybackInit1,
        PlaybackInit2,
        Playback,
        SendSnapshot,
        SendSnapshotCreate,
        SendSnapshotSend,
        SendSnapshotWait,
        CompensationSnapshot,
        ResourceLoad,
        UnityAssetManager,
        UnityAssetManagerUpdateLoadRequest,
        UnityAssetManagerUpdateLoadedRequest,
        UnityAssetManagerUpdateBundlePool,
        UnityAssetManagerFetchResult,
        LoadRequestManager,
        FreeRule,
        Animator,
        UI,
        GUI,
        Destroy,
        LifeTime,
        StateUpdateTest,
        StateUpdateEventCollect,
        StateUpdateWeaponAnimation,
        StateUpdateResponseToInput,
        StateUpdateResponseToAnimation,
        StateCallBackInvoke,
        StateWriteAnimation,
        AppearancePlaybackUpdateTransform,
        AppearancePlaybackUpdateWeapon,
        SyncStateVar,
        SyncFirePosition,
        ClientMove,
        Render,
        OC,
        CullDynamicObjectsWithUmbra,
        CullObjectsWithoutUmbra,
        End,
    }

    public class CustomProfileInfo
    {
        public static int MainThreadId = 0;
        public static bool EnableCustomSampler = true;
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(CustomProfileInfo));
        public string Name;
        public string ShortName;
        public volatile float Total;
        public volatile int Times;
        public Stopwatch Stopwatch = new Stopwatch();
        public long Memory = 0;
        public long TotalMemory = 0;
        public long StartMemory = 0;
        public int GcCount = 0;
        public int LastCollCount;
        public int GcTotal = 0;
        public bool Paused = false;
        public int ExecCount = 0;
        public string AddInfo = String.Empty;

        public CustomProfileInfo(string name)
        {
            Name = name.Replace(" ", "").Replace(".", "_");
            ShortName = Name.Replace("EntityCreateSystem", "EC")
                .Replace("PhysicsPostUpdateSystem", "PyPo")
                .Replace("ResourceLoadSystem", "Res")
                .Replace("GamePlaySystem", "Play")
                .Replace("UserCmdExecuteManagerSystem", "CMD")
                .Replace("UserCmdUpdateMsgExecuteManage", "CMD")
                .Replace("UiSystem", "UI")
                .Replace("UserPredictionSystem", "CMD")
                .Replace("VehicleSimulation", "VSim")
                .Replace("VechiclePrediction", "VPre")
                .Replace("UserPrediction", "UPre")
                .Replace("SendSnapshot", "Snap")
                .Replace("ResourceLoad", "Res")
                .Replace("PlaybackSystem", "PBS")
                .Replace("Playback", "PB")
                .Replace("PlaybackInit", "PBI")
                .Replace("OnGuiSystem", "OnGUI")
                .Replace("GameController", "GameC")
                .Replace("EntityCleanUp", "ECU")
                .Replace("LoadRequestManager", "LRes")
                .Replace("RenderSystem", "Render")
                .Replace("PlayerAppearance", "PA")
                .Replace("FreeObjectPosition", "FOP")
                .Replace("System", "")
                .Replace("_Common", "_");

            _profile = UnityProfiler.GetSampler(string.Format("voyager_{0}", Name));
        }

        private CustomSampler _profile;

        public void Clean()
        {
            Memory = 0;
            Total = 0;
            Times = 0;
            GcCount = 0;
            ExecCount = 0;
            Stopwatch.Reset();
            //TotalMemory = 0;
        }

        public void CopyTo(CustomProfileInfo to)
        {
            to.Total = Total;
            to.Times = Times;
            to.Memory = Memory;
            to.TotalMemory = TotalMemory;
            to.StartMemory = StartMemory;
            to.GcCount = GcCount;
            to.GcTotal = GcTotal;
            to.ExecCount = ExecCount;
            to.AddInfo = AddInfo;
        }

        public void AddTo(CustomProfileInfo to)
        {
            to.Total += Total;
            to.Times += Times;
            to.Memory = Memory;
            to.TotalMemory += TotalMemory;
            to.StartMemory = StartMemory;
            to.GcCount = GcCount;
            to.ExecCount += ExecCount;
            to.GcTotal += GcTotal;
            to.AddInfo = AddInfo;
        }

        public float Avg
        {
            get { return Times != 0 ? Total / Times : 0f; }
        }

        public float AvgMemory
        {
            get { return Memory; }
        }

        public void AddToProperties(Dictionary<string, object> properties)
        {
            if (Total > 10)
            {
                properties[string.Format("{0}{1}0", ShortName, "Total")] = (int) Total;
                properties[string.Format("{0}{1}0", ShortName, "Mem")] = Memory;
                properties[string.Format("{0}{1}0", ShortName, "Gc")] = GcTotal;
                properties[string.Format("{0}{1}0", ShortName, "Times")] = Times;
                properties[string.Format("{0}{1}0", ShortName, "Avg")] = (int) (Avg * 1000);
            }
        }

        [Conditional("ENABLE_PROFILER")]
        public void BeginProfileOnlyEnableProfile()
        {
            BeginProfile();
        }
        [Conditional("ENABLE_PROFILER")]
        public void PauseProfileOnlyEnableProfile()
        {
            PauseProfile();
        }
        [Conditional("ENABLE_PROFILER")]
        public void EndProfileOnlyEnableProfile()
        {
            EndProfile();
        }
        public void BeginProfile()
        {
            try
            {
                if (!Paused)
                {
                    StartMemory = GC.GetTotalMemory(false);

                    LastCollCount = System.GC.CollectionCount(0) + System.GC.CollectionCount(1) +
                                         System.GC.CollectionCount(2);
                }

                Stopwatch.Start();
                BeginProfileInternal();
            }
            catch (Exception e)
            {
                _logger.ErrorFormat(Name,e);
            }
        }

        public void PauseProfile()
        {

            Stopwatch.Stop();
            EndProfileInternal();
            Paused = true;
        }

        public float EndProfile()
        {

            Stopwatch.Stop();
            var elapsed = Stopwatch.ElapsedTicks / 10000f;

            Stopwatch.Reset();
            EndProfileInternal();
            Paused = false;

            Total += elapsed;
            Times++;
            var m = GC.GetTotalMemory(false) - StartMemory;
            if (m > 0)
            {
                Memory += m;
                TotalMemory += m;
            }

            var gc = System.GC.CollectionCount(0) + System.GC.CollectionCount(1) + System.GC.CollectionCount(2) -
                     LastCollCount;
            GcCount += gc;
            GcTotal += gc;

            return elapsed;
        }
        [Conditional("ENABLE_PROFILER")]
        private void EndProfileInternal()
        {
            try
            {
                if (EnableCustomSampler && _profile != null && Thread.CurrentThread.ManagedThreadId == MainThreadId)  
                {
                    _profile.End();
                }
            }
            catch (Exception e)
            {
                _logger.ErrorFormat(Name,e);
            }
        }
        [Conditional("ENABLE_PROFILER")]
        private void BeginProfileInternal()
        {
            try
            {
                if (EnableCustomSampler && _profile != null && Thread.CurrentThread.ManagedThreadId == MainThreadId)  
                {
                    _profile.Begin();
                }
            }
            catch (Exception e)
            {
                _logger.ErrorFormat(Name,e);
            }
        }
    }

    public class ProfileInfos
    {
        CustomProfileInfo[] _profileInfos = new CustomProfileInfo[(int) CustomProfilerStep.End];
        Dictionary<string, CustomProfileInfo> _customProfile = new Dictionary<string, CustomProfileInfo>();
        public string Profile = string.Empty;

        public ProfileInfos()
        {
            for (int i = 0; i < (int) CustomProfilerStep.End; i++)
            {
                _profileInfos[i] = new CustomProfileInfo(((CustomProfilerStep) i).ToString());
                SingletonManager.Get<MyProfilerManager>().Add(_profileInfos[i].Name);
            }
        }

        public CustomProfileInfo Get(CustomProfilerStep step)
        {
            return _profileInfos[(int) step];
        }

        public CustomProfileInfo Get(String name)
        {
            if (!_customProfile.ContainsKey(name))
            {
                _customProfile[name] = new CustomProfileInfo(name);
                SingletonManager.Get<MyProfilerManager>().Add(_customProfile[name].Name);
            }

            return _customProfile[name];
        }

        public string PrintDuration()
        {
            StringBuilder sb = new StringBuilder();

            int simExecCount = _profileInfos[(int) CustomProfilerStep.VehicleSimulation].ExecCount;
            sb.Append("cpu:").Append((_profileInfos[(int) CustomProfilerStep.GameController].Avg).ToString("G3"))
                .Append(" all:")
                .Append((_profileInfos[(int) CustomProfilerStep.GameController].Total / 20).ToString("G3"))
                .Append(" user:")
                .Append((_profileInfos[(int) CustomProfilerStep.UserPrediction].Total / 20).ToString("G3"))
                .Append((_profileInfos[(int) CustomProfilerStep.UserPrediction].Total / 20).ToString("G3"))
                .Append(" car:")
                .Append((_profileInfos[(int) CustomProfilerStep.VechiclePrediction].Total / 20).ToString("G3"))
                .Append(" pb:").Append((_profileInfos[(int) CustomProfilerStep.Playback].Total / 20).ToString("G3"))
                .Append(" pbI:")
                .Append(((_profileInfos[(int) CustomProfilerStep.PlaybackInit].Total +
                          _profileInfos[(int) CustomProfilerStep.SyncLatest].Total) / 20).ToString("G3"))
                .Append(" res:")
                .Append(((_profileInfos[(int) CustomProfilerStep.LoadRequestManager].Total +
                          _profileInfos[(int) CustomProfilerStep.ResourceLoad].Total +
                          _profileInfos[(int) CustomProfilerStep.UnityAssetManager].Total
                         ) / 20).ToString("G3"))
                .Append(" ot:")
                .Append(((_profileInfos[(int) CustomProfilerStep.Destroy].Total +
                          _profileInfos[(int) CustomProfilerStep.LifeTime].Total) / 20).ToString("G3"))
                .Append(" ui:")
                .Append((_profileInfos[(int) CustomProfilerStep.UI].Total / 20).ToString("G3"))
                .Append(" oc:")
                .Append((_profileInfos[(int)CustomProfilerStep.OC].Total / 20).ToString("G3"))
                .Append(", ")
                .Append((_profileInfos[(int)CustomProfilerStep.OC].Total / 2000 * _profileInfos[(int)CustomProfilerStep.GameController].Avg))
                .Append(" dub:")
                .Append(_profileInfos[(int)CustomProfilerStep.CullDynamicObjectsWithUmbra].Total / _profileInfos[(int)CustomProfilerStep.CullDynamicObjectsWithUmbra].Times)
                .Append(" oub:")
                .Append(_profileInfos[(int)CustomProfilerStep.CullObjectsWithoutUmbra].Total / _profileInfos[(int)CustomProfilerStep.CullObjectsWithoutUmbra].Times)
                .Append("\n")
                .Append(" sim:")
                .Append((_profileInfos[(int) CustomProfilerStep.VehicleSimulation].Total /
                         (simExecCount > 0 ? simExecCount : 1)).ToString("G3"))
                .Append(" , ")
                .Append(simExecCount)
                .Append(" fpd:")
                .Append((_profileInfos[(int) CustomProfilerStep.VehicleFixedUpdate].Total /
                         (simExecCount > 0 ? simExecCount : 1)).ToString("G3"));
#if PHYSICS_PROFILER_STATISTICS
            var stats = Profiler.GetPhysicsStats();
            sb.Append(" ndb: ")
                .Append(stats.numDynamicBodies)
                .Append(" nsb: ")
                .Append(stats.numStaticBodies)
                .Append(" adb: ")
                .Append(stats.numActiveDynamicBodies)
                .Append(" akb: ")
                .Append(stats.numActiveKinematicBodies)
                .Append(" tor: ")
                .Append(stats.numTriggerOverlaps)
                .Append(" cst: ")
                .Append(stats.numConstraints)
                .Append(" nsp: ")
                .Append(stats.numShapePairs);
#endif
#if UNITY_EDITOR
            //walk around bug of RuntimeStats in Editor Mode
            if(!RuntimeStats.enabled)
#else
            if (RuntimeStats.enabled)
#endif
            { 
             
                sb.Append("\n")
#if ENABLE_PROFILER && PROFILER_CPU_GPU_TIME
                    .Append("gpu: ")
                    .Append(Profiler.GPUTime * 0.001f)
                    .Append(" cpu: ")
                    .Append(Profiler.CPUTime * 0.001f)
                    .Append(" ")
#endif
                    .Append("dc: ")
                    .Append(RuntimeStats.drawCalls)
                    .Append(" sdc: ")
                    .Append(RuntimeStats.staticBatchedDrawCalls)
                    .Append(" sdc: ")
                    .Append(RuntimeStats.dynamicBatchedDrawCalls)
                    .Append(" idc: ")
                    .Append(RuntimeStats.instancedBatchedDrawCalls)
                    .Append(" bc: ")
                    .Append(RuntimeStats.batches)
                    .Append(" sbc: ")
                    .Append(RuntimeStats.staticBatches)
                    .Append(" dbc: ")
                    .Append(RuntimeStats.dynamicBatches)
                    .Append(" ibc: ")
                    .Append(RuntimeStats.instancedBatchedDrawCalls)
                    .Append(" vrt: ")
                    .Append(RuntimeStats.vertices)
                    .Append(" tri: ")
                    .Append(RuntimeStats.triangles)
                    .Append(" sct: ")
                    .Append(RuntimeStats.shadowCasters);
            }

#if HAVE_HZB_CULLING
            if (GraphicsSettings.hzbCullingEnable)
            {
                var hzbCullStats = Profiler.GetHZBCullStats();
                sb.Append("\n")
                    .Append(string.Format("Culled Prim:{0}/{1},", hzbCullStats.culledRendererNum, hzbCullStats.totalRendererNum))
                    .Append(string.Format("Culled Lights:{0}/{1}\n", hzbCullStats.culledLightNum, hzbCullStats.totalLightNum));
            }
#endif

            return sb.ToString();
        }

        public string GetHtmlTable()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<p>Duration Info</p>");
            sb.Append(
                "<table id=\"table_id\" class=\"display\" width='400px' border='1' align='center' cellpadding='2' cellspacing='1'>");
            sb.Append("<thead>");
            sb.Append("<td>step</td>");
            sb.Append("<td>AddInfo</td>");
            sb.Append("<td>times</td>");
            sb.Append("<td>total</td>");
            sb.Append("<td>avg</td>");
            sb.Append("<td>percentage</td>");
            sb.Append("<td>Memory</td>");
            sb.Append("<td>TotalMemory</td>");
            sb.Append("<td>GC</td>");
            sb.Append("<td>GCTotal</td>");
            sb.Append("<td>ExecCount</td>");
            sb.Append("</thead>");
            float total = _profileInfos[0].Total > 0 ? _profileInfos[0].Total : 1;
            for (int i = 0; i < (int) CustomProfilerStep.End; i++)
            {
                var info = _profileInfos[i];
                GetInfoHtml(sb, info, total);
            }

            foreach (var kv in _customProfile)
            {
                if (kv.Value.Times == 0) continue;
                var info = kv.Value;
                GetInfoHtml(sb, info, total);
            }


            return sb.ToString();
        }

        private static void GetInfoHtml(StringBuilder sb, CustomProfileInfo info, float total)
        {
            sb.Append("<tr>");
            sb.Append("<td>").Append(info.Name).Append("</td>");
            sb.Append("<td>").Append(info.AddInfo).Append("</td>");
            sb.Append("<td>").Append(info.Times).Append("</td>");
            sb.Append("<td>").Append(info.Total).Append("</td>");
            sb.Append("<td>").Append(info.Avg).Append("</td>");
            sb.Append("<td>").Append(info.Total / total * 100).Append("</td>");
            sb.Append("<td>").Append(info.AvgMemory / 1024).Append("</td>");
            sb.Append("<td>").Append(info.TotalMemory / 1024).Append("</td>");
            sb.Append("<td>").Append(info.GcCount).Append("</td>");
            sb.Append("<td>").Append(info.GcTotal).Append("</td>");
            sb.Append("<td>").Append(info.ExecCount).Append("</td>");
            sb.Append("</tr>");
        }

        public string GetCmdTable()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("uration Info\n");

            float total = _profileInfos[0].Total > 0 ? _profileInfos[0].Total : 1;
            for (int i = 0; i < (int) CustomProfilerStep.End; i++)
            {
                var info = _profileInfos[i];

                sb.Append("  ").Append(info.Name);
                sb.Append("  ").Append(info.Times);
                sb.Append("  ").Append(info.Total);
                sb.Append("  ").Append(info.Avg);
                sb.Append("  ").Append(info.Total / total);
                sb.Append("  ").Append(info.GcTotal);
                sb.Append("  ").Append((int) (info.Memory / 1024));
                sb.Append("\n");
            }

            sb.Append("\n");
            foreach (var kv in _customProfile)
            {
                if (kv.Value.Times == 0) continue;
                var info = kv.Value;
                sb.Append("  ").Append(info.Name);
                sb.Append("  ").Append(info.Times);
                sb.Append("  ").Append(info.Total);
                sb.Append("  ").Append(info.Avg);
                sb.Append("  ").Append(info.Total / total);
                sb.Append("  ").Append(info.GcTotal);
                sb.Append("  ").Append((int) (info.Memory / 1024));
                sb.Append("\n");
            }

            return sb.ToString();
        }

        public void Claen()
        {
            for (int i = 0; i < (int) CustomProfilerStep.End; i++)
            {
                var info = _profileInfos[i];
                info.Clean();
            }

            foreach (var kv in _customProfile)
            {
                kv.Value.Clean();
            }
        }

        public void CopyTo(ProfileInfos last)
        {
            for (int i = 0; i < (int) CustomProfilerStep.End; i++)
            {
                var info = _profileInfos[i];
                info.CopyTo(last.Get((CustomProfilerStep) i));
            }

            foreach (var kv in _customProfile)
            {
                var info = last.Get(kv.Key);
                kv.Value.CopyTo(info);
            }

            Profile = last.Profile;
        }

        public void AddTo(ProfileInfos oneMin)
        {
            for (int i = 0; i < (int) CustomProfilerStep.End; i++)
            {
                var info = _profileInfos[i];
                info.AddTo(oneMin.Get((CustomProfilerStep) i));
            }

            foreach (var kv in _customProfile)
            {
                var info = oneMin.Get(kv.Key);
                kv.Value.AddTo(info);
            }
        }

        public void AddToEventProperties(Dictionary<string, object> properties)
        {
            for (int i = 0; i < (int) CustomProfilerStep.End; i++)
            {
                var info = _profileInfos[i];
                info.AddToProperties(properties);
            }

            foreach (var kv in _customProfile)
            {
                var info = kv.Value;
                info.AddToProperties(properties);
            }
        }
    }

    public class DurationHelp : Singleton<DurationHelp>
    {
        private float fpsMeasuringDelta = 2.0f;

        private float timePassed;

        public static bool Debug = false;

        public string LastAvg
        {
            get { return _last.PrintDuration(); }
        }

        public string LastMax
        {
            get { return _last.Profile; }
        }

        private float oneMinTimePassed;

        public DurationHelp()
        {
            ServerInfo = "";
            CustomProfileInfo.EnableCustomSampler = !UnityProfiler.IsDeepProfiling;
        }

        private static Stack<string> _profilerStack = new Stack<string>();

        public bool Enabled = true;
        private ProfileInfos _current = new ProfileInfos();
        private ProfileInfos _last = new ProfileInfos();
        private ProfileInfos _oneMin = new ProfileInfos();

        public void ProfileStart(CustomProfilerStep step)
        {
            if (Enabled)
            {
                var info = _current.Get(step);
                Start(info);
            }
            
        }

        private static void Start(CustomProfileInfo info)
        {
            if (Debug)
            {
                _logger.InfoFormat("Begin Profiler {0}", info.Name);
                _profilerStack.Push(info.Name);
            }

            info.BeginProfile();
        }

        public void ProfileAddExecuteCount(CustomProfilerStep step, int execCount)
        {
            var info = _current.Get(step);
            info.ExecCount += execCount;
        }

        public void ProfileAddInfo(CustomProfilerStep step, string addInfo)
        {
            var info = _current.Get(step);
            info.AddInfo = addInfo;
        }

        public void ProfilePause(CustomProfilerStep step)
        {
            if (Enabled)
            {
                var info = _current.Get(step);

                if (Debug)
                {
                    _logger.InfoFormat("Pause Profiler {0}", info.Name);
                    var name = _profilerStack.Pop();
                    if (!info.Name.Equals(name, System.StringComparison.Ordinal))
                    {
                        _logger.ErrorFormat("Pause Mismatch Profiler {0}-{1}", name, info.Name);
                    }
                }
            
                info.PauseProfile();
            }
 
        }

        public float ProfileEnd(CustomProfilerStep step)
        {
            if (Enabled)
            {
                var info = _current.Get(step);
                return End(info);
            }
            return 0.0f;
        }

        private static float End(CustomProfileInfo info)
        {
            if (Debug)
            {
                _logger.InfoFormat("End Profiler {0}", info.Name);
                var name = _profilerStack.Pop();
                if (!info.Name.Equals(name, System.StringComparison.Ordinal))
                {
                    _logger.ErrorFormat("End Mismatch Profiler {0}-{1}", name, info.Name);
                }
            }
            
            return info.EndProfile();

        }

        public void ProfileAddInfo(string step, string addInfo)
        {
            var info = _current.Get(step);
            info.AddInfo = addInfo;
        }

        public void ProfileStart(CustomProfileInfo info)
        {
            if(Enabled)
                Start(info);
        }

        public float ProfileEnd(CustomProfileInfo info)
        {
            if (Enabled)
            {
                var t = End(info);
                if (t > 5f)
                {
                    _current.Profile = string.Format("{0}_{1}", info.Name, t);
                }

                return t;
            }
           
            return 0.0f;
        }

        public void ProfileStart(string step)
        {
            if (Enabled)
            {
                var info = _current.Get(step);
                Start(info);
            }
        }

        public void ProfileEnd(string step)
        {
            if (Enabled)
            {
                var info = _current.Get(step);
                var t = End(info);
                if (t > 5f)
                {
                    _current.Profile = string.Format("{0}_{1}", step, t);
                }
            }
        }

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(DurationHelp));

        public void Update()
        {
            try
            {
                CustomProfileInfo.MainThreadId = Thread.CurrentThread.ManagedThreadId;
                float deltaTime = Time.deltaTime;
                timePassed += deltaTime;
                oneMinTimePassed += deltaTime;
                if (timePassed > fpsMeasuringDelta)
                {
                    timePassed = 0;
                    _current.CopyTo(_last);
                    _current.AddTo(_oneMin);

                    _current.Claen();
                }

                if (oneMinTimePassed > 30.0f)
                {
                    oneMinTimePassed = 0;
                    //EventLog(_oneMin);
                    _oneMin.Claen();
                }
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Update {0}", e);
            }
        }

        Dictionary<string, object> ps = new Dictionary<string, object>();
        private int _rewindCount;
        public int DriveTime { get; private set; }

        private void EventLog(ProfileInfos oneMin)
        {
            ps.Clear();
            oneMin.AddToEventProperties(ps);
            EventLogger.Instance.Logger(EventLoggerConstant.Frame, "profile",
                ps);
        }

        public string GetHtmlTable()
        {
            return _last.GetHtmlTable();
        }

        public string GetCmdTable()
        {
            return _last.GetCmdTable();
        }

        public CustomProfileInfo GetCustomProfileInfo(string name)
        {
            return _current.Get(name);
        }

        public CustomProfileInfo GetProfileInfo(CustomProfilerStep step)
        {
            return _current.Get(step);
        }

        public void IncreaseRewindCount()
        {
            _rewindCount++;
        }

        public int RewindCount
        {
            get { return _rewindCount; }
        }

        public int LastAvgInterpolateInterval { get; set; }
        public int ServerClientDelta { get; set; }
        public int RenderTime { get; set; }
        public int LastServerTime { get; set; }
        public string ServerInfo { get; set; }

        public void IncDriveTimeCount()
        {
            DriveTime++;
        }

        public Vector3 Position = Vector3.zero;
    }
}