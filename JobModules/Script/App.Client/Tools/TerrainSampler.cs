using App.Client.Scripts;
using App.Shared;
using App.Shared.SceneManagement;
using Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using App.Shared.Configuration;
using App.Shared.Player;
using Core.SceneManagement;
using UnityEngine;
using UnityEngine.Profiling;
using Utils.Singleton;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace App.Client.Tools
{
    public class TerrainSampler : MonoBehaviour
    {
        /// <summary>
        /// 日志适配器，最终输出json格式
        /// </summary>
        // private LoggerAdapter logger = new LoggerAdapter("TerrainSamplerLogger");

        /// <summary>
        /// 记录采样得到的文本数据
        /// </summary>
        private StringBuilder logData = new StringBuilder();

        /// <summary>
        /// 记录系统采样开始时间
        /// </summary>
        //private DateTime _systemStartTime;

        /// <summary>
        /// 系统采样消耗时间
        /// </summary>
        private long _systemStartTimeCost = 2 * 1000 * 1000;

        /// <summary>
        /// 采样间隔
        /// </summary>
        private const int SampleInterval = 100;

        /// <summary>
        /// 采样的起始点
        /// </summary>
        private readonly Vector3 SampleStartPoint = new Vector3(0, 200, 0);

        /// <summary>
        /// 采样的最大点
        /// </summary>
        private readonly Vector3 SampleMaxPoint = new Vector3(4000, 200, 4000);

        /// <summary>
        /// 记录地形块尺寸（1公里）
        /// </summary>
        private const float BlockSize = 1000;

        /// <summary>
        /// 记录是否处于采样模式
        /// </summary>
        private Boolean InSamplingMode = false;

        /// <summary>
        /// 记录是否需要强制停止采样
        /// </summary>
        private bool forceStopSample = false;

        /// <summary>
        /// 记录采样完毕后是否强制退出App
        /// </summary>
        private bool forceExitApp = true;

        /// <summary>
        /// 记录数据采样完毕后是否自动上传网络
        /// </summary>
        private bool autoTranslate = false;

        private WaitForSeconds halfWaitTime = new WaitForSeconds(0.5f);

        private List<SampleData> sortedDts = new List<SampleData>();

        private string url
        {
            get
            {
                return SingletonManager.Get<ClientFileSystemConfigManager>().TerrainSampleConfig.Url;
            }
        }
        private string dataKey
        {
            get
            {
                return SingletonManager.Get<ClientFileSystemConfigManager>().TerrainSampleConfig.Key;
            }
        }
        private string quality
        {
            get
            {
                return SingletonManager.Get<ClientFileSystemConfigManager>().TerrainSampleConfig.Quality;
            }
        }
        private bool enableFrameLimit
        {
            get
            {
                return SingletonManager.Get<ClientFileSystemConfigManager>().TerrainSampleConfig.EnableFrameLimit;
            }
        }
        private int frameLimit
        {
            get
            {
                return SingletonManager.Get<ClientFileSystemConfigManager>().TerrainSampleConfig.FrameLimit;
            }
        }
        private bool enablePosLimit
        {
            get
            {
                return SingletonManager.Get<ClientFileSystemConfigManager>().TerrainSampleConfig.EnablePosLimit;
            }
        }
        private int posLimitX
        {
            get
            {
                return SingletonManager.Get<ClientFileSystemConfigManager>().TerrainSampleConfig.PosLimitX;
            }
        }
        private int posLimitZ
        {
            get
            {
                return SingletonManager.Get<ClientFileSystemConfigManager>().TerrainSampleConfig.PosLimitZ;
            }
        }
        private int posLimitDir
        {
            get
            {
                return SingletonManager.Get<ClientFileSystemConfigManager>().TerrainSampleConfig.PosLimitDir;
            }
        }
        private int sampleCount
        {
            get
            {
                return SingletonManager.Get<ClientFileSystemConfigManager>().TerrainSampleConfig.SampleCount;
            }
        }
        private int waitForSample
        {
            get
            {
                return SingletonManager.Get<ClientFileSystemConfigManager>().TerrainSampleConfig.WaitForSample;
            }
        }
        private bool useHallQuality
        {
            get
            {
                return SingletonManager.Get<ClientFileSystemConfigManager>().TerrainSampleConfig.UseHallQuality;
            }
        }
        private int hallQuality
        {
            get
            {
                return SingletonManager.Get<ClientFileSystemConfigManager>().TerrainSampleConfig.HallQuality;
            }
        }

        private PlayerContext _playerContext;
        /// <summary>
        /// 记录玩家上下文
        /// </summary>
        public PlayerContext PlayerContext
        {
            get { return _playerContext; }
            set { _playerContext = value; }
        }

        public ILevelManager LevelManager { get; set; }

        void CheckToStartSample()
        {
            if ((SharedConfig.InSamplingMode || SharedConfig.InLegacySampleingMode) && !InSamplingMode)
            {
                //_systemStartTime = DateTime.UtcNow;
                SharedConfig.HaveFallDamage = false;
                StartCoroutine(Sample());
                InSamplingMode = true;
            }
        }

        private void OnEnable()
        {
#if !UNITY_EDITOR
            RuntimeStats.enabled = true;
#endif

            // Unity自带游戏品级设置
            int qualityIndex = -1;
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                if (QualitySettings.names[i].Equals(quality))
                {
                    qualityIndex = i;
                    break;
                }
            }
            if (qualityIndex != -1)
            {
                Debug.LogFormat("TerrainSampler SetQuality Level index:{0} name:{1} time:{2}", qualityIndex, QualitySettings.names[qualityIndex], System.DateTime.Now);
                QualitySettings.SetQualityLevel(qualityIndex);
            }

            // 策划游戏品级设置
            if (useHallQuality)
            {
                Utils.SettingManager.SettingManager.GetInstance().SetQuality((Utils.SettingManager.QualityLevel)hallQuality);
            }
        }

        void Start()
        {
            CheckToStartSample();
        }

        private void OnDisable()
        {
#if !UNITY_EDITOR
            RuntimeStats.enabled = false;
#endif
        }

        /// <summary>
        /// 记录采样点的最坏采样数据
        /// </summary>
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

            string formatString = "{{\"x\":{0},\"y\":{1},\"fps\":{2},\"batches\":{3},\"setPassCalls\":{4},\"tris\":{5},\"verts\":{6},\"camDirX\":{7},\"camDirY\":{8},\"camDirZ\":{9}," +
                "\"camUpX\":{10},\"camUpY\":{11},\"camUpZ\":{12},\"camPosX\":{13},\"camPosY\":{14},\"camPosZ\":{15},\"camFOV\":{16},\"camNear\":{17},\"camFar\":{18}," +
                "\"camOC\":{19},\"camHDR\":{20},\"camMSAA\":{21},\"shadowCaster\":{22},\"frameTime\":{23},\"renderTime\":{24}}}";

            if (samplePointNum > 0)
            {
                formatString = "," + formatString;
            }

            logData.AppendFormat(formatString, sampleData.X, sampleData.Y, sampleData.FrameRate, sampleData.Batches, sampleData.SetPassCalls, sampleData.Triangles, sampleData.Vertices,
                sampleData.CamDirX, sampleData.CamDirY, sampleData.CamDirZ, sampleData.CamUpX, sampleData.CamUpY, sampleData.CamUpZ, sampleData.CamPosX, sampleData.CamPosY, sampleData.CamPosZ,
                sampleData.CamFOV, sampleData.CamNear, sampleData.CamFar, sampleData.camOC, sampleData.camHDR, sampleData.camMSAA, sampleData.shadowCaster, sampleData.frameTime, sampleData.renderTime);

            return worstIndex;
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

        private IEnumerator SampleFpsData(int posX, int posZ, SampleData[] dts, int dir)
        {
            sortedDts.Clear();

            // 采样指定帧数去除三次最大最小值后取平均值
            for (int c = 0; c < sampleCount; c++)
            {
                float fps = 1 / Time.deltaTime;
                SampleData dt = new SampleData(posX, posZ, fps, RuntimeStats.batches, RuntimeStats.setPassCalls, RuntimeStats.triangles, RuntimeStats.vertices,
                    Camera.main.transform.forward, Camera.main.transform.up, Camera.main.transform.position, Camera.main.fieldOfView, Camera.main.nearClipPlane, Camera.main.farClipPlane,
                    Camera.main.useOcclusionCulling ? 1 : 0, Camera.main.allowHDR ? 1 : 0, Camera.main.allowMSAA ? 1 : 0, RuntimeStats.shadowCasters, RuntimeStats.frameTime, RuntimeStats.renderTime);
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

            dts[dir] = new SampleData(posX, posZ, frame, batches, setPassCalls, triangles, vertices, forward, up, position, fov, near, far, useOc, useHDR, useMSAA, shadowCaster, frameTime, renderTime);
        }

        private int CompareSampleData(SampleData a, SampleData b)
        {
            if (a.FrameRate < b.FrameRate) return -1;
            else if (a.FrameRate == b.FrameRate) return 0;
            else return 1;
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

            bool hitted = Physics.Raycast(r, out hitInfo, Mathf.Infinity, UnityLayers.SceneCollidableLayerMask | 1 << UnityLayerManager.GetLayerIndex(EUnityLayerName.WaterTrigger));

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
        /// 记录不需要采样的空地形块
        /// </summary>
        private List<KeyValuePair<int, int>> EmptyBlockList = new List<KeyValuePair<int, int>>()
        {
            new KeyValuePair<int, int>(0, 0),
            new KeyValuePair<int, int>(0, 1),
            new KeyValuePair<int, int>(0, 2),
            new KeyValuePair<int, int>(0, 3),
            new KeyValuePair<int, int>(0, 4),
            new KeyValuePair<int, int>(0, 5),
            new KeyValuePair<int, int>(0, 6),
            new KeyValuePair<int, int>(0, 7),
            new KeyValuePair<int, int>(1, 0),
            new KeyValuePair<int, int>(1, 1),
            new KeyValuePair<int, int>(1, 2),
            new KeyValuePair<int, int>(1, 6),
            new KeyValuePair<int, int>(1, 7),
            new KeyValuePair<int, int>(2, 6),
            new KeyValuePair<int, int>(2, 7),
            new KeyValuePair<int, int>(3, 7),
            new KeyValuePair<int, int>(4, 0),
            new KeyValuePair<int, int>(5, 0),
            new KeyValuePair<int, int>(6, 0),
            new KeyValuePair<int, int>(6, 1),
            new KeyValuePair<int, int>(7, 0),
            new KeyValuePair<int, int>(7, 1),
            new KeyValuePair<int, int>(7, 2),
            new KeyValuePair<int, int>(7, 3),
            new KeyValuePair<int, int>(7, 4),
            new KeyValuePair<int, int>(7, 7),
        };

        /// <summary>
        /// 判断地形块是否需要采样
        /// </summary>
        private bool IsBlockNeedToSample(int x, int z)
        {
            Predicate<KeyValuePair<int, int>> predicate = delegate (KeyValuePair<int, int> kv)
             {
                 if ((kv.Key == x) && (kv.Value == z))
                 {
                     return true;
                 }
                 else
                 {
                     return false;
                 }
             };
            // KeyValuePair<int, int> res = EmptyBlockList.Find(predicate);
            var resIndex = EmptyBlockList.FindIndex(predicate);
            if (resIndex == -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 执行具体采样
        /// </summary>
        /// <returns></returns>
        private IEnumerator Sample()
        {
            Debug.LogError("begin sampler");

            logData.Append("[");
            yield return new WaitForSeconds(30);
            while (_playerContext.flagSelfEntity == null)                       // 确保游戏加载完毕
            {
                yield return null;
            }
            if (forceStopSample) goto FINISHSAMPLE;

            Camera.main.fieldOfView = 50.53401f;
            Camera.main.nearClipPlane = 0.03f;
            Camera.main.farClipPlane = 8000;

            int samplePointNum = 0;                                             // 记录采样点总数
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    // 判断地形块是否需要采样
                    if (!IsBlockNeedToSample(i, j)) continue;

                    // 计算当前地块中心在大地图中的位置
                    Vector3 pos = CalculateCoordinate(i, j, BlockSize * 0.5f, BlockSize * 0.5f);

                    // 将人物移动到计算位置
                    MoveTo(pos);

                    // 等待地形加载完毕
                    yield return StartCoroutine(WaitForTerrainReady(pos));
                    if (forceStopSample) goto FINISHSAMPLE;

                    // 确保玩家可以正常降落
                    while (WillPlayerFallToHell(pos))
                    {
                        yield return null;
                        if (forceStopSample) goto FINISHSAMPLE;
                    }

                    //在一个地形块里面迭代采样
                    for (int x = 1; x < 1000; x += SampleInterval)
                    {
                        for (int z = 1; z < 1000; z += SampleInterval)
                        {
                            // 判断运行时是否停止了采样
                            while (SharedConfig.StopSampler)
                            {
                                yield return null;
                                if (forceStopSample) goto FINISHSAMPLE;
                            }

                            // 取得采样点在大地图中的位置（仅x,z有意义）
                            Vector3 currentPos = CalculateCoordinate(i, j, x, z);

                            // 确定采样点y方向上的具体坐标
                            Vector3 groundPos = FindTheGroundPos(currentPos);

                            // 移动人物至采样点
                            MoveTo(groundPos);

                            // 等待场景流式加载完毕
                            yield return StartCoroutine(WaitForTerrainReady(groundPos));
                            if (forceStopSample) goto FINISHSAMPLE;

                            // 采样点四个角度采样
                            int posX = (int)(i * BlockSize + x) - 4000, posZ = (int)(j * BlockSize + z) - 4000;
                            SampleData[] datas = new SampleData[4];
                            for (int dir = 0; dir < 4; dir++)
                            {
                                // 等待相机转向调整
                                yield return StartCoroutine(LookAt(dir * 90));
                                if (forceStopSample) goto FINISHSAMPLE;

                                // 等待帧率稳定
                                yield return StartCoroutine(WaitFpsStable());
                                if (forceStopSample) goto FINISHSAMPLE;

                                // 帧率采样
                                yield return StartCoroutine(SampleFpsData(posX, posZ, datas, dir));
                                if (forceStopSample) goto FINISHSAMPLE;
                            }

                            int worstIndex = RecordTheWorstOne(samplePointNum, datas);
                            samplePointNum++;

                            // 启用了帧率限制,暂停帧率采样
                            if (enableFrameLimit && (int)datas[worstIndex].FrameRate < frameLimit)
                            {
                                SharedConfig.StopSampler = true;

                                // 将相机调整为最坏方向的视角
                                yield return StartCoroutine(LookAt(worstIndex * 90));
                            }

                            // 启用了位置限制，暂停帧率采样
                            if (enablePosLimit && posX == posLimitX && posZ == posLimitZ)
                            {
                                SharedConfig.StopSampler = true;

                                // 将相机调整为指定方向的视角
                                if (posLimitDir >= 0 && posLimitDir <= 3)
                                    yield return StartCoroutine(LookAt(posLimitDir * 90));
                                else
                                    yield return StartCoroutine(LookAt(worstIndex * 90));
                            }

                            yield return null;
                            if (forceStopSample) goto FINISHSAMPLE;

                            while (SharedConfig.StopSampler)                        // 等待帧率暂停解除
                            {
                                yield return null;
                                if (forceStopSample) goto FINISHSAMPLE;
                            }

                            // if (i == 1 && j == 3)
                            // {
                            //     goto LaLa;
                            // }
                        }
                    }

                    // LaLa:
                    //     Debug.LogErrorFormat("i:{0} j:{1}", i, j);
                }
            }

            autoTranslate = true;                                               // 全局采样完毕自动上传

        FINISHSAMPLE:
            logData.Append("]");
            string sampleString = logData.ToString();

            // 写入本地json文件
            {
                DateTime now = DateTime.Now;
                string filePath = string.Format("./log/terrainsampler_{0}_{1}_{2}_{3}_{4}.json", now.Year, now.Month, now.Day, now.Hour, now.Minute);
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(filePath, sampleString);
            }

            Debug.Log("start upload file to web ...");

            if (autoTranslate)           // 自动上传网络
            {
                using (WebClient client = new WebClient())
                {
                    NameValueCollection collection = new NameValueCollection()
                    {
                        {dataKey,sampleString}
                    };
                    Debug.LogFormat("url:{0} dataKey:{1}", url, dataKey);
                    client.UploadValues(url, "POST", collection);
                }
            }

            Debug.Log("finish upload file to web ...");

            if (forceExitApp) Application.Quit();

            // 上传完毕后
            logData.Length = 0;
            InSamplingMode = false;
            forceStopSample = false;
            forceExitApp = true;
        }

        private IEnumerator SamplePoints(List<Vector3> points)
        {
            if (points == null || points.Count == 0) yield break;

            Debug.LogError("begin sampler some points");
            logData.Append("[");
            yield return new WaitForSeconds(30);
            while (_playerContext.flagSelfEntity == null)                       // 确保游戏加载完毕
            {
                yield return null;
            }
            if (forceStopSample) goto FINISHSAMPLE;

            Camera.main.fieldOfView = 50.53401f;
            Camera.main.nearClipPlane = 0.03f;
            Camera.main.farClipPlane = 8000;

            int samplePointNum = 0;                                             // 记录采样点总数
            for (int num = 0; num < points.Count; num++)
            {
                Vector2 terrainIndex = GetTerrainIndexByPos(points[num]);
                int i = (int)terrainIndex.x, j = (int)terrainIndex.y;

                // 判断地形块是否需要采样
                if (!IsBlockNeedToSample(i, j)) continue;

                // 计算当前地块中心在大地图中的位置
                Vector3 pos = CalculateCoordinate(i, j, BlockSize * 0.5f, BlockSize * 0.5f);

                // 将人物移动到计算位置
                MoveTo(pos);

                // 等待地形加载完毕
                yield return StartCoroutine(WaitForTerrainReady(pos));
                if (forceStopSample) goto FINISHSAMPLE;

                // 确保玩家可以正常降落
                while (WillPlayerFallToHell(pos))
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

                // 取得采样点在大地图中的位置（仅x,z有意义）
                Vector3 currentPos = points[num];

                // 确定采样点y方向上的具体坐标
                Vector3 groundPos = FindTheGroundPos(currentPos);

                // 移动人物至采样点
                MoveTo(groundPos);

                // 等待场景流式加载完毕
                yield return StartCoroutine(WaitForTerrainReady(groundPos));
                if (forceStopSample) goto FINISHSAMPLE;

                // 采样点四个角度采样
                int posX = (int)(currentPos.x), posZ = (int)(currentPos.z);
                SampleData[] datas = new SampleData[4];
                for (int dir = 0; dir < 4; dir++)
                {
                    // 等待相机转向调整
                    yield return StartCoroutine(LookAt(dir * 90));
                    if (forceStopSample) goto FINISHSAMPLE;

                    // 等待帧率稳定
                    yield return StartCoroutine(WaitFpsStable());
                    if (forceStopSample) goto FINISHSAMPLE;

                    // 帧率采样
                    yield return StartCoroutine(SampleFpsData(posX, posZ, datas, dir));
                    if (forceStopSample) goto FINISHSAMPLE;
                }

                int worstIndex = RecordTheWorstOne(samplePointNum, datas);
                samplePointNum++;

                // 启用了帧率限制,暂停帧率采样
                if (enableFrameLimit && (int)datas[worstIndex].FrameRate < frameLimit)
                {
                    SharedConfig.StopSampler = true;
                }

                // 启用了位置限制，暂停帧率采样
                if (enablePosLimit && posX == posLimitX && posZ == posLimitZ)
                {
                    SharedConfig.StopSampler = true;

                    // 将相机调整为指定方向的视角
                    if (posLimitDir >= 0 && posLimitDir <= 3)
                        yield return StartCoroutine(LookAt(posLimitDir * 90));
                    else
                        yield return StartCoroutine(LookAt(worstIndex * 90));
                }

                yield return null;
                if (forceStopSample) goto FINISHSAMPLE;

                while (SharedConfig.StopSampler)                        // 等待帧率暂停解除
                {
                    yield return null;
                    if (forceStopSample) goto FINISHSAMPLE;
                }
            }

        FINISHSAMPLE:
            logData.Append("]");
            string sampleString = logData.ToString();

            // 写入本地json文件
            {
                DateTime now = DateTime.Now;
                string filePath = string.Format("./log/terrainsampler_{0}_{1}_{2}_{3}_{4}.json", now.Year, now.Month, now.Day, now.Hour, now.Minute);
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(filePath, sampleString);
            }

            Debug.LogError("autoTranslate:" + autoTranslate);

            if (autoTranslate)           // 自动上传网络
            {
                using (WebClient client = new WebClient())
                {
                    NameValueCollection collection = new NameValueCollection()
                    {
                        {dataKey,sampleString}
                    };
                    Debug.LogFormat("url:{0} dataKey:{1}", url, dataKey);
                    client.UploadValues(url, "POST", collection);
                }
            }

            Debug.LogError("finish autoTranslate");

            if (forceExitApp) Application.Quit();

            // 上传完毕后
            logData.Length = 0;
            InSamplingMode = false;
            forceStopSample = false;
            forceExitApp = true;
        }

        private IEnumerator SampleScenes(List<Vector2> scenes)
        {
            if (scenes == null || scenes.Count == 0) yield break;

            Debug.LogError("begin sampler some scenes");
            logData.Append("[");
            yield return new WaitForSeconds(30);
            while (_playerContext.flagSelfEntity == null)                       // 确保游戏加载完毕
            {
                yield return null;
            }
            if (forceStopSample) goto FINISHSAMPLE;

            Camera.main.fieldOfView = 50.53401f;
            Camera.main.nearClipPlane = 0.03f;
            Camera.main.farClipPlane = 8000;

            int samplePointNum = 0;                                             // 记录采样点总数
            for (int k = 0; k < scenes.Count; k++)
            {
                int i = (int)scenes[k].x, j = (int)scenes[k].y;

                // 判断地形块是否需要采样
                if (!IsBlockNeedToSample(i, j)) continue;

                // 计算当前地块中心在大地图中的位置
                Vector3 pos = CalculateCoordinate(i, j, BlockSize * 0.5f, BlockSize * 0.5f);

                // 将人物移动到计算位置
                MoveTo(pos);

                // 等待地形加载完毕
                yield return StartCoroutine(WaitForTerrainReady(pos));
                if (forceStopSample) goto FINISHSAMPLE;

                // 确保玩家可以正常降落
                while (WillPlayerFallToHell(pos))
                {
                    yield return null;
                    if (forceStopSample) goto FINISHSAMPLE;
                }

                //在一个地形块里面迭代采样
                for (int x = 1; x < 1000; x += SampleInterval)
                {
                    for (int z = 1; z < 1000; z += SampleInterval)
                    {
                        // 判断运行时是否停止了采样
                        while (SharedConfig.StopSampler)
                        {
                            yield return null;
                            if (forceStopSample) goto FINISHSAMPLE;
                        }

                        // 取得采样点在大地图中的位置（仅x,z有意义）
                        Vector3 currentPos = CalculateCoordinate(i, j, x, z);

                        // 确定采样点y方向上的具体坐标
                        Vector3 groundPos = FindTheGroundPos(currentPos);

                        // 移动人物至采样点
                        MoveTo(groundPos);

                        // 等待场景流式加载完毕
                        yield return StartCoroutine(WaitForTerrainReady(groundPos));
                        if (forceStopSample) goto FINISHSAMPLE;

                        // 采样点四个角度采样
                        int posX = (int)(i * BlockSize + x) - 4000, posZ = (int)(j * BlockSize + z) - 4000;
                        SampleData[] datas = new SampleData[4];
                        for (int dir = 0; dir < 4; dir++)
                        {
                            // 等待相机转向调整
                            yield return StartCoroutine(LookAt(dir * 90));
                            if (forceStopSample) goto FINISHSAMPLE;

                            // 等待帧率稳定
                            yield return StartCoroutine(WaitFpsStable());
                            if (forceStopSample) goto FINISHSAMPLE;

                            // 帧率采样
                            yield return StartCoroutine(SampleFpsData(posX, posZ, datas, dir));
                            if (forceStopSample) goto FINISHSAMPLE;
                        }

                        int worstIndex = RecordTheWorstOne(samplePointNum, datas);
                        samplePointNum++;

                        // 启用了帧率限制,暂停帧率采样
                        if (enableFrameLimit && (int)datas[worstIndex].FrameRate < frameLimit)
                        {
                            SharedConfig.StopSampler = true;

                            // 将相机调整为最坏方向的视角
                            yield return StartCoroutine(LookAt(worstIndex * 90));
                        }

                        // 启用了位置限制，暂停帧率采样
                        if (enablePosLimit && posX == posLimitX && posZ == posLimitZ)
                        {
                            SharedConfig.StopSampler = true;

                            // 将相机调整为指定方向的视角
                            if (posLimitDir >= 0 && posLimitDir <= 3)
                                yield return StartCoroutine(LookAt(posLimitDir * 90));
                            else
                                yield return StartCoroutine(LookAt(worstIndex * 90));
                        }

                        yield return null;
                        if (forceStopSample) goto FINISHSAMPLE;

                        while (SharedConfig.StopSampler)                        // 等待帧率暂停解除
                        {
                            yield return null;
                            if (forceStopSample) goto FINISHSAMPLE;
                        }
                    }
                }
            }

        FINISHSAMPLE:
            logData.Append("]");
            string sampleString = logData.ToString();

            // 写入本地json文件
            {
                DateTime now = DateTime.Now;
                string filePath = string.Format("./log/terrainsampler_{0}_{1}_{2}_{3}_{4}.json", now.Year, now.Month, now.Day, now.Hour, now.Minute);
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(filePath, sampleString);
            }

            Debug.LogError("autoTranslate:" + autoTranslate);

            if (autoTranslate)           // 自动上传网络
            {
                using (WebClient client = new WebClient())
                {
                    NameValueCollection collection = new NameValueCollection()
                    {
                        {dataKey,sampleString}
                    };
                    Debug.LogFormat("url:{0} dataKey:{1}", url, dataKey);
                    client.UploadValues(url, "POST", collection);
                }
            }

            Debug.LogError("finish autoTranslate");

            if (forceExitApp) Application.Quit();

            // 上传完毕后
            logData.Length = 0;
            InSamplingMode = false;
            forceStopSample = false;
            forceExitApp = true;
        }

        /// <summary>
        /// 计算指定地块指定位置在大地图中的坐标
        /// </summary>
        private Vector3 CalculateCoordinate(int blockX, int blockZ, float xInBlock, float zInBlock)
        {
            float x = blockX * BlockSize + xInBlock +
                      SingletonManager.Get<MapsDescription>().BigMapParameters.TerrainMin.x;
            float z = blockZ * BlockSize + zInBlock +
                      SingletonManager.Get<MapsDescription>().BigMapParameters.TerrainMin.z;
            float y = 0;
            Vector3 pos = new Vector3(x, y, z);
            return pos;
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

            bool hitted = Physics.Raycast(r, out hitInfo, Mathf.Infinity, UnityLayers.SceneCollidableLayerMask | (1 << UnityLayerManager.GetLayerIndex(EUnityLayerName.WaterTrigger)));
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
        /// 将人物移动到大地图中的指定位置
        /// </summary>
        private void MoveTo(Vector3 pos)
        {
            var player = _playerContext.flagSelfEntity;
            player.position.Value = pos;
            if (App.Shared.SharedConfig.InSamplingMode)
                Camera.main.transform.position = new Vector3(pos.x, pos.y + 1.7f, pos.z);
        }

        /// <summary>
        /// 将玩家调整到指定朝向
        /// </summary>
        private IEnumerator LookAt(float yaw)
        {
            var player = _playerContext.flagSelfEntity;
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

        /// <summary>
        /// 确保大地形中指定位置所需的地块加载完毕
        /// </summary>
        private bool IsTerrainReady(Vector3 pos)
        {
            return LevelManager.NotFinishedRequests <= 0;
        }

        /// <summary>
        /// 等待指定位置的地形及其相关资源载入完毕
        /// </summary>
        private IEnumerator WaitForTerrainReady(Vector3 pos)
        {
            for (int i = 0; i < 2; i++)
                yield return null;

            while (!IsTerrainReady(pos))
                yield return null;

            int count = waitForSample;
            for (int i = 0; i < count; i++)
                yield return null;
        }

        /// <summary>
        /// 记录可视网格的数量
        /// </summary>
        private int GetVisibleRenders()
        {
            var mrs = FindObjectsOfType<MeshRenderer>();
            int count = 0, num = mrs.Length;
            for (int i = 0; i < num; i++)
            {
                MeshRenderer mr = mrs[i];
                if (mr != null && mr.isVisible) ++count;
            }
            return count;
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

        /// <summary>
        /// 指定采样点
        /// </summary>
        public bool SamplerSomePoints(List<float> list, bool autoTranslate)
        {
            // 正在采样
            if (InSamplingMode) return false;

            this.autoTranslate = autoTranslate;
            List<Vector3> points = new List<Vector3>();
            int count = list.Count / 3;
            for (int i = 0; i < count; i++)
            {
                Vector3 vec = new Vector3(list[i * 3], list[i * 3 + 1], list[i * 3 + 2]);
                points.Add(vec);
            }

            //_systemStartTime = DateTime.UtcNow;
            SharedConfig.HaveFallDamage = false;
            StartCoroutine(SamplePoints(points));
            InSamplingMode = true;

            return true;
        }

        public bool SamplerSomeScenes(List<int> list, bool autoTranslate)
        {
            // 正在采样
            if (InSamplingMode) return false;

            this.autoTranslate = autoTranslate;
            List<Vector2> scenes = new List<Vector2>();
            int count = list.Count / 2;
            for (int i = 0; i < count; i++)
            {
                Vector2 vec = new Vector2(list[i * 2], list[i * 2 + 1]);
                scenes.Add(vec);
            }

            //_systemStartTime = DateTime.UtcNow;
            SharedConfig.HaveFallDamage = false;
            StartCoroutine(SampleScenes(scenes));
            InSamplingMode = true;

            return true;
        }

        /// <summary>
        /// 根据指定位置取得地形序号
        /// </summary>
        public Vector2 GetTerrainIndexByPos(Vector3 pos)
        {
            if (pos.x > 4000f) pos.x = 4000f;
            else if (pos.x < -4000f) pos.x = -4000f;

            if (pos.z > 4000f) pos.z = 4000f;
            else if (pos.z < -4000f) pos.z = -4000f;

            pos.x -= -4000f;
            pos.z -= -4000f;

            int x = (int)(pos.x / 1000f);
            int z = (int)(pos.z / 1000f);

            return new Vector2(x, z);
        }
    }
}
