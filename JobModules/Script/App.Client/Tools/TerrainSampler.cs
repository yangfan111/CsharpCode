using App.Client.Scripts;
using App.Shared;
using Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using App.Shared.Configuration;
using Assets.App.Client.Tools;
using Core.SceneManagement;
using UnityEngine;
using UnityEngine.Profiling;
using Utils.Singleton;
using XmlConfig.BootConfig;
#if UNITY_EDITOR
#endif

namespace App.Client.Tools
{
    public class TerrainSampler : MonoBehaviour
    {
        public BaseTerrainSampler sampler { get; private set; }

        private Contexts contexts;

        private TerrainSampleConfig _sampleConfig = null;
        private TerrainSampleConfig sampleConfig
        {
            get
            {
                if (_sampleConfig == null)
                    _sampleConfig = SingletonManager.Get<ClientFileSystemConfigManager>().TerrainSampleConfig;
                return _sampleConfig;
            }
        }

        private Dictionary<int, List<Vector2>> _smallMaps;
        private Dictionary<int, List<Vector2>> smallMaps
        {
            get
            {
                if (_smallMaps == null)
                {
                    _smallMaps = new Dictionary<int, List<Vector2>>();
                    foreach (SmallMap map in sampleConfig.SmallMaps)
                    {
                        _smallMaps.Add(map.MapId, map.Points);
                    }
                }
                return _smallMaps;
            }
        }

        private Dictionary<int, List<Vector2>> _bigMaps;
        private Dictionary<int, List<Vector2>> bigMaps
        {
            get
            {
                if (_bigMaps == null)
                {
                    _bigMaps = new Dictionary<int, List<Vector2>>();
                    foreach (BigMap map in sampleConfig.BigMaps)
                    {
                        _bigMaps.Add(map.MapId, map.ExcludedScenes);
                    }
                }
                return _bigMaps;
            }
        }

        private MapsDescription mapDes
        {
            get
            {
                return SingletonManager.Get<MapsDescription>();
            }
        }

        private void OnEnable()
        {
#if !UNITY_EDITOR
            RuntimeStats.enabled = true;
#endif
        }

        private void OnDisable()
        {
#if !UNITY_EDITOR
            RuntimeStats.enabled = false;
#endif
        }

        public void StartSample(Contexts contexts)
        {
            int mapId = SingletonManager.Get<ClientFileSystemConfigManager>().BootConfig.MapId;
            sampler = GetTerrainSampler(mapId);
            if (sampler == null) return;

            this.contexts = contexts;
            sampler.Init(contexts, sampleConfig, this);
            sampler.StartSample();
        }

        /// <summary>
        /// 强制停止采样并退出应用
        /// </summary>
        public void ForceStopAndExitSampler(bool autoTranslate)
        {
            if (sampler == null)
            {
                Debug.LogErrorFormat("ForceStopAndExitSampler error, sampler is null");
                return;
            }
            sampler.ForceStopAndExitSampler(autoTranslate);
        }

        /// <summary>
        /// 强制停止采样
        /// </summary>
        /// <param name="autoTranslate"></param>
        public void ForceStopSampler(bool autoTranslate)
        {
            if (sampler == null)
            {
                Debug.LogErrorFormat("ForceStopSampler error, sampler is null");
                return;
            }
            sampler.ForceStopSampler(autoTranslate);
        }

        /// <summary>
        /// 采样指定点列表功能
        /// </summary>
        public bool SamplerSomePoints(List<float> list, bool autoTranslate)
        {
            // 正在采样
            if (sampler != null && sampler.isRunning) return false;

            List<Vector2> points = new List<Vector2>();
            int count = list.Count / 3;
            for (int i = 0; i < count; i++)
            {
                Vector2 vec = new Vector2(list[3 * i], list[3 * i + 2]);
                points.Add(vec);
            }

            // 构建点采样器
            int mapId = SingletonManager.Get<ClientFileSystemConfigManager>().BootConfig.MapId;
            sampler = new PointsTerrainSampler(mapId, points);
            sampler.Init(contexts, sampleConfig, this);
            sampler.StartSample(autoTranslate);

            return true;
        }

        public bool SamplerSomeScenes(List<int> list, bool autoTranslate)
        {
            // 正在采样
            if (sampler != null && sampler.isRunning) return false;

            // 地图场景不支持场景遍历采样
            int mapId = SingletonManager.Get<ClientFileSystemConfigManager>().BootConfig.MapId;
            if (!bigMaps.ContainsKey(mapId)) return false;

            // 提取需要采样的场景
            List<Vector2> scenes = new List<Vector2>();
            int count = list.Count / 2;
            for (int i = 0; i < count; i++)
            {
                Vector2 vec = new Vector2 { x = list[2 * i], y = list[2 * i + 1] };
                if (!IsExcludedScene(mapId, (int)vec.x, (int)vec.y)) scenes.Add(vec);
            }
            if (scenes.Count <= 0) return false;

            // 构建场景采样器
            sampler = new ScenesTerrainSampler(mapId, scenes);
            sampler.Init(contexts, sampleConfig, this);
            sampler.StartSample(autoTranslate);

            return true;
        }

        private BaseTerrainSampler GetTerrainSampler(int mapId)
        {
            if (mapDes.CurrentLevelType == LevelType.Exception)
            {
                Debug.LogErrorFormat("采样地图类型不能确定，无法构造采样器，mapId:{0}", mapId);
                return null;
            }

            if (mapDes.CurrentLevelType == LevelType.BigMap)
            {
                List<Vector2> list = new List<Vector2>();
                int dim = mapDes.BigMapParameters.TerrainDimension;
                for (int column = 0; column < dim; column++)
                {
                    for (int row = 0; row < dim; row++)
                    {
                        if (!IsExcludedScene(mapId, column, row))
                            list.Add(new Vector2 { x = column, y = row });
                    }
                }

                return new ScenesTerrainSampler(mapId, list);
            }

            if (mapDes.CurrentLevelType == LevelType.SmallMap)
            {
                List<Vector2> list = null;
                if (!smallMaps.TryGetValue(mapId, out list) || list.Count <= 0)
                {
                    Debug.LogErrorFormat("小地图未配置采样点，请在terrainsample_config.xml文件中配置，mapId:{0}", mapId);
                    return null;
                }

                return new PointsTerrainSampler(mapId, smallMaps[mapId]);
            }

            Debug.LogErrorFormat("未处理的地图类型type:{0} mapId:{1}", mapDes.CurrentLevelType, mapId);

            return null;
        }

        private bool IsExcludedScene(int mapId, int x, int y)
        {
            bool exclude = false;
            List<Vector2> list = null;
            if (bigMaps.TryGetValue(mapId, out list) && list.Count > 0)
            {
                foreach (Vector2 vec in list)
                {
                    if (x.Equals((int)(vec.x)) && y.Equals((int)(vec.y)))
                    {
                        exclude = true;
                        break;
                    }
                }
            }
            return exclude;
        }
    }

    public abstract class BaseTerrainSampler
    {
        protected int mapId { get; set; }

        protected StringBuilder sampleData { get; set; }

        /// <summary>
        /// 是否正在采样
        /// </summary>
        public bool isRunning { get; private set; }

        /// <summary>
        /// 是否停止采样
        /// </summary>
        protected bool forceStopSample = false;

        /// <summary>
        /// 记录采样完毕后是否强制退出App
        /// </summary>
        protected bool forceExitApp = true;

        /// <summary>
        /// 记录数据采样完毕后是否自动上传网络
        /// </summary>
        protected bool autoTranslate = false;

        protected PlayerContext playerContext;
        protected ILevelManager levelManager;
        protected TerrainSampler mono;

        protected string url;
        protected int waitForSample;
        protected int sampleCount;

        protected bool enableFrameLimit;
        protected int frameLimit;

        protected bool enablePosLimit;
        protected int posLimitX;
        protected int posLimitZ;
        protected int posLimitDir;

        public static int GcThreshold = 3;

        protected WaitForSeconds halfWaitTime = new WaitForSeconds(0.5f);

        protected List<SampleData> sortedDts = new List<SampleData>();

        protected MapsDescription mapDes
        {
            get { return SingletonManager.Get<MapsDescription>(); }
        }

        public BaseTerrainSampler(int mapId)
        {
            this.mapId = mapId;
            sampleData = new StringBuilder();
        }

        public void Init(Contexts contexts, TerrainSampleConfig config, TerrainSampler mono)
        {
            playerContext = contexts.player;
            levelManager = contexts.session.commonSession.LevelManager;
            this.mono = mono;
            url = config.Url;
            waitForSample = config.WaitForSample;
            sampleCount = config.SampleCount;
            enableFrameLimit = config.EnableFrameLimit;
            frameLimit = config.FrameLimit;
            enablePosLimit = config.EnablePosLimit;
            posLimitX = config.PosLimitX;
            posLimitZ = config.PosLimitZ;
            posLimitDir = config.PosLimitDir;
            SetQuality(config.Quality, config.UseHallQuality, config.HallQuality);
        }

        public void StartSample(bool autoTranslate = true)
        {
#if !UNITY_EDITOR
            RuntimeStats.enabled = true;
#endif
            if (isRunning) return;

            isRunning = true;
            SharedConfig.HaveFallDamage = false;
            this.autoTranslate = autoTranslate;

            sampleData.Append("[");
            mono.StartCoroutine(RunSample());
        }

        private SampleData[] _datas = new SampleData[4];
        public IEnumerator RunSample()
        {
            Debug.LogFormat("begin terrain sample, mapId:{0}", mapId);

            // 确保游戏初始化完毕
            while (playerContext.flagSelfEntity == null) yield return null;
            if (forceStopSample) goto FINISHSAMPLE;

            // 相机设置
            SetMainCamera();

            // 采样每一个需要采样的位置点
            IEnumerator<Vector2> points = GetSamplePoints();
            if (points == null) goto FINISHSAMPLE;

            // 采样每一个位置点
            int samplePointNum = 0; // 记录采样点总数
            int lastGcPointNum = 0;
            while (points.MoveNext())
            {
                Vector3 groundPos = new Vector3(points.Current.x, 0f, points.Current.y);

                // 触发周边地块的加载
                MoveTo(groundPos);

                // 等待地块及其相关资源加载完毕
                yield return mono.StartCoroutine(WaitForTerrainReady());
                if (forceStopSample) goto FINISHSAMPLE;

                // 确保玩家可以正常降落
                while (WillPlayerFallToHell(groundPos))
                {
                    yield return null;
                    if (forceStopSample) goto FINISHSAMPLE;
                }

                // 判断运行时是否停止了采样
                while (SharedConfig.StopSampler)
                {
                    yield return null;
                    if (forceStopSample) goto FINISHSAMPLE;
                }

                // 确定人物落地的具体坐标
                groundPos = FindTheGroundPos(groundPos);

                // 移动人物至采样点
                MoveTo(groundPos);

                // 等待场景流式加载完毕
                yield return mono.StartCoroutine(WaitForTerrainReady());
                if (forceStopSample) goto FINISHSAMPLE;

                for(int i = 0; i < _datas.Length; ++i)
                {
                    _dataPool.Return(_datas[i]);
                    _datas[i] = null;
                }

                // 采样点四个角度采样
                int posX = (int) (groundPos.x), posZ = (int) (groundPos.z);
                
                for (int dir = 0; dir < 4; dir++)
                {
                    // 等待相机转向调整
                    yield return mono.StartCoroutine(LookAt(dir * 90));
                    if (forceStopSample) goto FINISHSAMPLE;

                    // 等待帧率稳定
                    yield return mono.StartCoroutine(WaitFpsStable());
                    if (forceStopSample) goto FINISHSAMPLE;

                    // 帧率采样
                    yield return mono.StartCoroutine(SampleFpsData(posX, posZ, _datas, dir));
                    if (forceStopSample) goto FINISHSAMPLE;
                }

                // 记录最坏采样方向
                int worstIndex = RecordTheWorstOne(samplePointNum, _datas);
                samplePointNum++;

                if (samplePointNum - lastGcPointNum >= GcThreshold)
                {
                    lastGcPointNum = samplePointNum;
                    System.GC.Collect();
                }

                // 启用了帧率限制,暂停帧率采样
                if (enableFrameLimit && (int)_datas[worstIndex].FrameRate < frameLimit)
                {
                    SharedConfig.StopSampler = true;
                }

                // 启用了位置限制，暂停帧率采样
                if (enablePosLimit && posX == posLimitX && posZ == posLimitZ)
                {
                    SharedConfig.StopSampler = true;

                    // 将相机调整为指定方向的视角
                    if (posLimitDir >= 0 && posLimitDir <= 3)
                        yield return mono.StartCoroutine(LookAt(posLimitDir * 90));
                    else
                        yield return mono.StartCoroutine(LookAt(worstIndex * 90));
                }

                yield return null;
                if (forceStopSample) goto FINISHSAMPLE;

                while (SharedConfig.StopSampler) // 等待帧率暂停解除
                {
                    yield return null;
                    if (forceStopSample) goto FINISHSAMPLE;
                }
            }

            // 采样完毕后处理
            FINISHSAMPLE:
            OnFinishSample();
        }

        public void OnFinishSample()
        {
            sampleData.Append("]");
            string sampleString = sampleData.ToString();

            // 写入本地json文件
            {
                DateTime now = DateTime.Now;
                string filePath = string.Format("./log/terrainsampler_{0}_{1}_{2}_{3}_{4}.json", now.Year, now.Month,
                    now.Day, now.Hour, now.Minute);
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(filePath, sampleString);
            }

            Debug.LogFormat("mapId:{0}, autoTranslate:{1}, start upload file to web ...", mapId, autoTranslate);

            if (autoTranslate) // 自动上传网络
            {
                using (WebClient client = new WebClient())
                {
                    string dataKey = mapDes.CurrentLevelType == LevelType.BigMap ? "bigMap" : "smallMap";
                    Vector3 minVec = mapDes.SceneParameters.OriginPosition;
                    Vector3 mapSize = mapDes.SceneParameters.Size;

                    NameValueCollection collection = new NameValueCollection()
                    {
                        {"heatMapData", sampleString}
                    };

                    int mapLen = (int) (mapSize.y > mapSize.x ? mapSize.y : mapSize.x);
                    string address = string.Format(
                        "{0}/index.php?mapType={1}&mapId={2}&leftBottomX={3}&leftBottomY={4}&mapSize={5}*{6}&unitSize={7}",
                        url, dataKey, mapId, minVec.x, minVec.z, mapSize.x, mapSize.y, GetUnitSize(mapLen));
                    Debug.LogFormat("address:{0}", address);
                    client.UploadValues(address, "POST", collection);
                }
            }

            Debug.LogFormat("finish upload file to web ...");

            if (forceExitApp) Application.Quit();

            // 采样完毕后
            sampleData.Length = 0;
            isRunning = false;
            forceStopSample = false;
            forceExitApp = true;
        }

        protected virtual IEnumerator<Vector2> GetSamplePoints()
        {
            return null;
        }

        protected void SetQuality(string qualityName, bool useHallQuality, int hallQuality)
        {
            // Unity自带游戏品级设置
            int qualityIndex = -1;
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                if (QualitySettings.names[i].Equals(qualityName))
                {
                    qualityIndex = i;
                    break;
                }
            }
            if (qualityIndex != -1)
            {
                Debug.LogFormat("TerrainSampler SetQuality Level index:{0} name:{1} time:{2}", qualityIndex,
                    QualitySettings.names[qualityIndex], System.DateTime.Now);
                QualitySettings.SetQualityLevel(qualityIndex);
            }

            // 策划游戏品级设置
            if (useHallQuality)
            {
                Utils.SettingManager.SettingManager.GetInstance()
                    .SetQuality((Utils.SettingManager.QualityLevel) hallQuality);
            }
        }

        protected void SetMainCamera()
        {
            Camera.main.fieldOfView = 50.53401f;
            Camera.main.nearClipPlane = 0.03f;
            Camera.main.farClipPlane = 8000;
        }

        /// <summary>
        /// 确保大地形中指定位置所需的地块加载完毕
        /// </summary>
        protected bool IsTerrainReady()
        {
            return levelManager.NotFinishedRequests <= 0;
        }

        /// <summary>
        /// 等待指定位置的地形及其相关资源载入完毕
        /// </summary>
        protected IEnumerator WaitForTerrainReady()
        {
            for (int i = 0; i < 2; i++)
                yield return null;

            while (!IsTerrainReady())
                yield return null;

            int count = waitForSample;
            for (int i = 0; i < count; i++)
                yield return null;
        }

        /// <summary>
        /// 将人物移动到大地图中的指定位置
        /// </summary>
        protected void MoveTo(Vector3 pos)
        {
            var player = playerContext.flagSelfEntity;
            player.position.Value = pos;
            if (App.Shared.SharedConfig.InSamplingMode)
                Camera.main.transform.position = new Vector3(pos.x, pos.y + 1.7f, pos.z);
        }

        /// <summary>
        /// 判断玩家从大地图指定位置是否会落地失败
        /// </summary>
        private bool WillPlayerFallToHell(Vector3 pos)
        {
            Vector3 fromV = new Vector3(pos.x, 1000, pos.z);

            Vector3 toV = new Vector3(pos.x, -1000, pos.z);

            Ray r = new Ray(fromV, new Vector3(toV.x - fromV.x, toV.y - fromV.y, toV.z - fromV.z));

            RaycastHit hitInfo;

            bool hitted = Physics.Raycast(r, out hitInfo, Mathf.Infinity,
                UnityLayers.SceneCollidableLayerMask |
                1 << UnityLayerManager.GetLayerIndex(EUnityLayerName.WaterTrigger));

            if (hitted == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 确定指定x,z位置的落地点（y方向值）
        /// </summary>
        private Vector3 FindTheGroundPos(Vector3 pos)
        {
            Vector3 fromV = new Vector3(pos.x, 1000, pos.z);

            Vector3 toV = new Vector3(pos.x, -1000, pos.z);

            Ray r = new Ray(fromV, new Vector3(toV.x - fromV.x, toV.y - fromV.y, toV.z - fromV.z));

            RaycastHit hitInfo;

            bool hitted = Physics.Raycast(r, out hitInfo, Mathf.Infinity,
                UnityLayers.SceneCollidableLayerMask |
                (1 << UnityLayerManager.GetLayerIndex(EUnityLayerName.WaterTrigger)));
            float yVal = 0;
            if (hitted == false)
            {
                yVal = pos.y;
            }
            else
            {
                yVal = hitInfo.point.y;
            }

            Vector3 groundPos = new Vector3(pos.x, hitInfo.point.y, pos.z);
            return groundPos;
        }

        /// <summary>
        /// 将玩家调整到指定朝向
        /// </summary>
        private IEnumerator LookAt(float yaw)
        {
            var player = playerContext.flagSelfEntity;
            player.orientation.Pitch = 0;
            player.orientation.Yaw = yaw;

            if (App.Shared.SharedConfig.InSamplingMode)
            {
                Camera.main.transform.eulerAngles = new Vector3(0, player.orientation.Yaw, 0);
            }

            // 等待2帧相机调整完毕后渲染
            yield return null;
            yield return null;
        }

        private IEnumerator WaitFpsStable()
        {
            for (int t = 0; t < 10; t++)
            {
                if (Time.deltaTime * 1000 < 16)
                {
                    yield break;
                }

                yield return halfWaitTime;

                if (t == 9) yield break;
            }
        }

        private static SampleDataPool<SampleData> _dataPool = new SampleDataPool<SampleData>();
#if HAVE_FAST_GPU_PROFILER && ENABLE_PROFILER
        private List<GpuTimerQuerySample> _samples = new List<GpuTimerQuerySample>();
#endif
        private IEnumerator SampleFpsData(int posX, int posZ, SampleData[] dts, int dir)
        {
            foreach (var dt in sortedDts)
            {
                _dataPool.Return(dt);
            }
            sortedDts.Clear();

            // 采样指定帧数去除多个最大最小值后取平均值
            for (int c = 0; c < sampleCount; c++)
            {
                float fps = 1 / Time.deltaTime;


                SampleData dt = _dataPool.Get();
#if HAVE_FAST_GPU_PROFILER && ENABLE_PROFILER
                SampleDataSubset subset = dt.SubSet;
                _samples.Clear();
                Profiler.GetFastGpuProfilerSamples(_samples);
                foreach (var sample in _samples)
                {
                    subset.Add(sample.StatName, sample.ElapsedTime);
                }
#endif

                dt.SetData(posX, posZ, fps, RuntimeStats.batches, RuntimeStats.setPassCalls,
                    RuntimeStats.triangles, RuntimeStats.vertices,
                    Camera.main.transform.forward, Camera.main.transform.up, Camera.main.transform.position,
                    Camera.main.fieldOfView, Camera.main.nearClipPlane, Camera.main.farClipPlane,
                    Camera.main.useOcclusionCulling ? 1 : 0, Camera.main.allowHDR ? 1 : 0,
                    Camera.main.allowMSAA ? 1 : 0, RuntimeStats.shadowCasters, RuntimeStats.frameTime,
                    RuntimeStats.renderTime);
                sortedDts.Add(dt);

                yield return null;
            }

            sortedDts.Sort(CompareSampleData);

            float frame = 0f;
            int batches = 0, setPassCalls = 0, triangles = 0, vertices = 0;
            Vector3 forward = Vector3.zero, up = Vector3.zero, position = Vector3.zero;
            float fov = 0f, near = 0f, far = 0f;
            int useOc = 0, useHDR = 0, useMSAA = 0, shadowCaster = 0;
            float frameTime = 0f, renderTime = 0f;
            int dropFrames = 5;

            var dirDt = _dataPool.Get();
#if HAVE_FAST_GPU_PROFILER && ENABLE_PROFILER
            SampleDataSubset avrSubset = dirDt.SubSet;
#endif
            for (int i = dropFrames; i < sampleCount - dropFrames; i++)
            {
                SampleData sd = sortedDts[i];
                frame += sd.FrameRate;
                batches += sd.Batches;
                setPassCalls += sd.SetPassCalls;
                triangles += sd.Triangles;
                vertices += sd.Vertices;
                forward += new Vector3(sd.CamDirX, sd.CamDirY, sd.CamDirZ);
                up += new Vector3(sd.CamUpX, sd.CamUpY, sd.CamUpZ);
                position += new Vector3(sd.CamPosX, sd.CamPosY, sd.CamPosZ);
                fov += sd.CamFOV;
                near += sd.CamNear;
                far += sd.CamFar;
                useOc += sd.camOC;
                useHDR += sd.camHDR;
                useMSAA += sd.camMSAA;
                shadowCaster += sd.shadowCaster;
                frameTime += sd.frameTime;
                renderTime += sd.renderTime;
#if HAVE_FAST_GPU_PROFILER && ENABLE_PROFILER
                avrSubset.Add(sd.SubSet);
#endif
            }
            int count = sampleCount - 2 * dropFrames;
            frame /= count;
            batches /= count;
            setPassCalls /= count;
            triangles /= count;
            vertices /= count;
            forward /= count;
            up /= count;
            position /= count;
            fov /= count;
            near /= count;
            far /= count;
            useOc /= count;
            useHDR /= count;
            useMSAA /= count;
            shadowCaster /= count;
            frameTime /= count;
            renderTime /= count;
#if HAVE_FAST_GPU_PROFILER && ENABLE_PROFILER
            avrSubset.Divide(count);
#endif

            dirDt.SetData(posX, posZ, frame, batches, setPassCalls, triangles, vertices, forward, up,
                position, fov, near, far, useOc, useHDR, useMSAA, shadowCaster, frameTime, renderTime);
            dts[dir] = dirDt;
        }

        private int CompareSampleData(SampleData a, SampleData b)
        {
            if (a.FrameRate < b.FrameRate) return -1;
            else if (a.FrameRate == b.FrameRate) return 0;
            else return 1;
        }

        /// <summary>
        /// 记录采样点的最坏采样数据
        /// </summary>
        private readonly string[] _formatStrings = 
        {
            "{{\"x\":{0},\"y\":{1},\"fps\":{2},\"batches\":{3},\"setPassCalls\":{4},\"tris\":{5},\"verts\":{6},\"camDirX\":{7},\"camDirY\":{8},\"camDirZ\":{9},\"camUpX\":{10},\"camUpY\":{11},\"camUpZ\":{12},\"camPosX\":{13},\"camPosY\":{14},\"camPosZ\":{15},\"camFOV\":{16},\"camNear\":{17},\"camFar\":{18},\"camOC\":{19},\"camHDR\":{20},\"camMSAA\":{21},\"usedHeap\":{22},\"totalHeap\":{23},\"totalMemory\":{24},\"mapID\":{25},\"GC\":{26},\"samplePointNum\":{27}",
            ",{{\"x\":{0},\"y\":{1},\"fps\":{2},\"batches\":{3},\"setPassCalls\":{4},\"tris\":{5},\"verts\":{6},\"camDirX\":{7},\"camDirY\":{8},\"camDirZ\":{9},\"camUpX\":{10},\"camUpY\":{11},\"camUpZ\":{12},\"camPosX\":{13},\"camPosY\":{14},\"camPosZ\":{15},\"camFOV\":{16},\"camNear\":{17},\"camFar\":{18},\"camOC\":{19},\"camHDR\":{20},\"camMSAA\":{21},\"usedHeap\":{22},\"totalHeap\":{23},\"totalMemory\":{24},\"mapID\":{25},\"GC\":{26},\"samplePointNum\":{27}",
        };
        private StringBuilder _profilerData = new StringBuilder();
        private int RecordTheWorstOne(int samplePointNum, SampleData[] dataList)
        {
            int worstIndex = 0;
            double lowestFrame = dataList[0].FrameRate;

            for (int i = 0; i < dataList.Length; i++)
            {
                if (dataList[i].FrameRate < lowestFrame)
                {
                    lowestFrame = dataList[i].FrameRate;
                    worstIndex = i;
                }
            }

            SampleData sampleData = dataList[worstIndex];

            string formatString = samplePointNum > 0 ? _formatStrings[1] : _formatStrings[0];
            long unit = 1024 ^ 2;
            long usedHeap = GC.GetTotalMemory(false) / unit;
            long totalHeap = Profiler.GetMonoHeapSizeLong() / unit;
            long allMemory = Profiler.GetTotalAllocatedMemoryLong() / unit;

            this.sampleData.AppendFormat(formatString, sampleData.X, sampleData.Y, sampleData.FrameRate, sampleData.Batches, sampleData.SetPassCalls, sampleData.Triangles, sampleData.Vertices,
                sampleData.CamDirX, sampleData.CamDirY, sampleData.CamDirZ, sampleData.CamUpX, sampleData.CamUpY, sampleData.CamUpZ, sampleData.CamPosX, sampleData.CamPosY, sampleData.CamPosZ,
                sampleData.CamFOV, sampleData.CamNear, sampleData.CamFar, sampleData.camOC, sampleData.camHDR, sampleData.camMSAA, usedHeap, totalHeap, allMemory, mapId,
                GC.CollectionCount(0), samplePointNum);

#if HAVE_FAST_GPU_PROFILER && ENABLE_PROFILER
            _profilerData.Length = 0;
            _profilerData.AppendFormat(",\"MainThreadTime\":{0}", Profiler.MainThreadFrameTime);
            _profilerData.AppendFormat(",\"RenderThreadTime\":{0}", Profiler.RenderThreadFrameTime);
            _profilerData.AppendFormat(",\"GPUTime\":{0}", Profiler.FastGPUTime);
            foreach (var data in sampleData.SubSet)
            {
                _profilerData.AppendFormat(",\"{0}\":{1}", data.Key, data.Value);
            }
            this.sampleData.Append(_profilerData);
#endif
            this.sampleData.Append("}");

            return worstIndex;
        }

        /// <summary>
        /// 强制停止采样并退出应用
        /// </summary>
        public void ForceStopAndExitSampler(bool autoTranslate)
        {
            this.autoTranslate = autoTranslate;
            forceStopSample = true;
            forceExitApp = true;
        }

        /// <summary>
        /// 强制停止采样
        /// </summary>
        /// <param name="autoTranslate"></param>
        public void ForceStopSampler(bool autoTranslate)
        {
            this.autoTranslate = autoTranslate;
            forceStopSample = true;
            forceExitApp = false;
        }

        public int GetUnitSize(float size)
        {
            int len = 1000;

            if (size > 2000f) len = 1000;
            else if (size > 1000f) len = 500;
            else if (size > 800f) len = 250;
            else if (size > 250f) len = 100;
            else if (size > 100f) len = 50;
            else len = 20;

            return len;
        }
    }

    public class PointsTerrainSampler : BaseTerrainSampler
    {
        private IEnumerable<Vector2> points = null;

        public PointsTerrainSampler(int mapId, IEnumerable<Vector2> points) : base(mapId)
        {
            this.points = points;
        }

        protected override IEnumerator<Vector2> GetSamplePoints()
        {
            var iterator = points.GetEnumerator();
            while (iterator.MoveNext())
            {
                yield return iterator.Current;
            }
        }
    }

    public class ScenesTerrainSampler : BaseTerrainSampler
    {
        private IEnumerable<Vector2> scenes;

        /// <summary>
        /// 采样点的距离间隔
        /// </summary>
        private const int sampleInterval = 100;

        public ScenesTerrainSampler(int mapId, IEnumerable<Vector2> scenes) : base(mapId)
        {
            this.scenes = scenes;
        }

        protected override IEnumerator<Vector2> GetSamplePoints()
        {
            if (mapDes.CurrentLevelType != LevelType.BigMap)
            {
                Debug.LogErrorFormat("非大地图不支持场景采样，请联系tzj修改代码以便支持，mapId:{0} levelType:{1}",
                    mapId, mapDes.CurrentLevelType);
                yield break;
            }

            // 对每一个场景地块进行采样
            float blockSize = mapDes.BigMapParameters.TerrainSize;
            Vector3 minVec = mapDes.BigMapParameters.TerrainMin;
            foreach (Vector2 vec in scenes)
            {
                float posStartX = minVec.x + vec.x * blockSize;
                float posStartZ = minVec.z + vec.y * blockSize;

                for (int x = 1; x < blockSize; x += sampleInterval)
                {
                    for (int z = 1; z < blockSize; z += sampleInterval)
                    {
                        float posX = posStartX + x;
                        float posZ = posStartZ + z;

                        yield return new Vector2(posX, posZ);
                    }
                }
            }
        }
    }
}
