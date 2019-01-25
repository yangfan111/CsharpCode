using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;
using System;
using System.Reflection;
using UnityEditorInternal;

namespace ArtPlugins
{
    public class CpuGpuSampler : EditorWindow
    {
        public static bool gpuData=false;
        private static void GetAvgCpuGpu(out float cpu, out float gpu, int frameCount = 10)
        {
            var firstFrameIndex = ProfilerDriver.firstFrameIndex;
            var lastFrameIndex = ProfilerDriver.lastFrameIndex;
            cpu = -999;
            gpu = -999;
            if (lastFrameIndex - firstFrameIndex < frameCount) return;
            var profilerSortColumn = ProfilerColumn.TotalTime;
            var viewType = ProfilerViewType.Hierarchy;
            firstFrameIndex = lastFrameIndex - frameCount;
            float cpuAll = 0;
            float gpuAll = 0;
            float maxC = 0,minC=9999;
            float maxG = 0,minG=9999;
            for (int frameIndex = firstFrameIndex; frameIndex <= lastFrameIndex; ++frameIndex)
            {

                var property = new ProfilerProperty();
                property.SetRoot(frameIndex, profilerSortColumn, viewType);
                property.onlyShowGPUSamples = false;
                float ccpu = float.Parse(property.frameTime);
                float cgpu = float.Parse(property.frameGpuTime);
                cpuAll += ccpu;
                gpuAll += cgpu;
                if (ccpu > maxC) maxC = ccpu;
                if (cgpu > maxG) maxG = cgpu;
                if (ccpu < minC) minC = ccpu;
                if (cgpu < minG) minG = cgpu;
                property.Cleanup();
              }
            cpuAll -= maxC + minC;
            gpuAll -= maxG + minG;

            cpu = cpuAll / (lastFrameIndex - firstFrameIndex-2);
            gpu = gpuAll / (lastFrameIndex - firstFrameIndex-2);
        }
        abstract class BaseCmd
        {
            public int groupIndex;
            public abstract BaseCmd Init(params object[] data);
            public abstract void Execute();


        };
        class EmptyCmd : BaseCmd
        {
            public static EmptyCmd instance = new EmptyCmd();
            public int _waitFrame = 1;
            public override void Execute()
            {
                waitFrame = _waitFrame;
            }

            public override BaseCmd Init(params object[] data)
            {
                _waitFrame = (int)data[0];
                return this;
            }
        }
        class ChangeCameraCmd : BaseCmd
        {
            float x;
            float z;
            float dir;
            PosInfo posInfo;
            public override void Execute()
            {
                if (posInfo == null)
                {
                    camera.transform.position = terrain.GetPosition() + new Vector3(x, 0, z);
                    camera.transform.position += new Vector3(0, Mathf.Max(113, terrain.SampleHeight(camera.transform.position)) + 2, 0);
                    camera.transform.rotation = Quaternion.Euler(-8, dir, 0);
                }
                else
                {
                    camera.transform.position = posInfo.pos;
                    camera.transform.rotation = posInfo.rot;

                }
            }

            public override BaseCmd Init(params object[] data)
            {
                x = (float)(int)(data[0]);
                z = (float)(int)(data[1]);
                dir = (float)(int)(data[2]);
                return this;
            }
            public BaseCmd Init(PosInfo posInfo)
            {
                this.posInfo = posInfo;
                return this;
            }
        }
        class CpuGpuRecordCmd : BaseCmd
        {
            PosInfo posInfo;
            public override void Execute()
            {
                GetAvgCpuGpu(out posInfo.cpuTime, out posInfo.gpuTime,20);
                posInfo.pos = camera.transform.position;
                posInfo.rot = camera.transform.rotation;
                infoList.Add(posInfo);
            }

            public override BaseCmd Init(params object[] data)
            {
                posInfo = (PosInfo)data[0];
                return this;
            }
        }
        class CpuGpuRecordFinishCmd : BaseCmd
        {
            bool needOrder;
            public override void Execute()
            {
                
                if(needOrder)
                infoList.Sort(sortMain);
                for (int i = 0; i < 10; i++)
                {
                    cmdQueue.Enqueue(new ChangeCameraCmd().Init(infoList[i]));
                    cmdQueue.Enqueue(EmptyCmd.instance);
                    cmdQueue.Enqueue(EmptyCmd.instance);
                    cmdQueue.Enqueue(EmptyCmd.instance);
                    cmdQueue.Enqueue(EmptyCmd.instance);


                    for (int k = 0; k < (int)CostKindEnum.MaxCount; k++)
                    {
                        CostKind costKind = new CostKind();
                        costKind.kindName = kindNames[k];
                        costKind.kindEnum = (CostKindEnum)k;
                        cmdQueue.Enqueue(new CpuGpuCostingCmd(0).Init(costKind, infoList[i]));//关闭自己 等待15frame
                        cmdQueue.Enqueue(new EmptyCmd().Init(25));
                        cmdQueue.Enqueue(new CpuGpuCostingCmd(1).Init(costKind, infoList[i]));// 测量 关闭状态性能 
                        cmdQueue.Enqueue(EmptyCmd.instance);
                        cmdQueue.Enqueue(EmptyCmd.instance);
                        cmdQueue.Enqueue(new CpuGpuCostingCmd(2).Init(costKind, infoList[i]));//  打开
 
                    }




                }
 
            }

            public override BaseCmd Init(params object[] data)
            {
                this.needOrder = (bool)data[0];
                return this;
            }
        }
        class CpuGpuCostingCmd : BaseCmd
        {
            CostKind costKind;
            PosInfo posInfo;
            static float originDis;
            static GameObject hideItem;
            static List<MonoBehaviour> hidects;
            internal CpuGpuCostingCmd(int groupIndex)
            {
                this.groupIndex = groupIndex;
            }

            public override void Execute()
            {
             
                 if (groupIndex==0) closeSelf();
                if (groupIndex == 1) {
                    GetAvgCpuGpu(out costKind.costCpuTime_without_this, out costKind.costGpuTime_without_this,20);
                      
                }
                if (groupIndex == 2)
                {
                    openSelf();
                }

                }

            private void closeSelf()
            {
                switch (costKind.kindEnum)
                {
                    case CostKindEnum.Tree:
                        originDis = terrain.treeDistance;
                        terrain.treeDistance = 0;
                        //   hideItem = GameObject.Find("globalTree").gameObject;
                        //   hideItem.SetActive(false);
                        break;
                    case CostKindEnum.Grass:
                        originDis = terrain.detailObjectDensity;
                        terrain.detailObjectDensity = 0;
                        break;
                    case CostKindEnum.Terrain:
                        terrain.drawHeightmap = false;
                        break;
                    case CostKindEnum.House:
                        hideItem = GameObject.Find(terrain.gameObject.scene.name.Split(' ')[1] + "Bud").gameObject;
                        hideItem.SetActive(false);
                        break;
                    case CostKindEnum.Props:

                        hideItem.SetActive(false);
                        break;
                    case CostKindEnum.LightingBox:

                        hideItem = GameObject.Find("Global Volume").gameObject;

                        hideItem.SetActive(false);
                        hidects = new List<MonoBehaviour>();
                        foreach (MonoBehaviour cpt in camera.GetComponents<MonoBehaviour>())
                        {
                            if (cpt == null || cpt.enabled == false) continue;
                            cpt.enabled = false;
                            hidects.Add(cpt);
                        }

                        break;

                    case CostKindEnum.RoadWater:

                        hideItem = GameObject.Find("Road Network").gameObject;
                        GameObject.Find("AQUAS Waterplane").transform.SetParent(hideItem.transform);
                        hideItem.SetActive(false);


                        break;
                    default:
                        break;
                }

            }

            private void openSelf()
            {
                switch (costKind.kindEnum)
                {
                    case CostKindEnum.Tree:
                        terrain.treeDistance = originDis;
                        //  hideItem.SetActive(true);
                        break;
                    case CostKindEnum.Grass:
                        terrain.detailObjectDensity = originDis;

                        break;
                    case CostKindEnum.Terrain:
                        terrain.drawHeightmap = true;
                        break;
                    case CostKindEnum.House:
                        hideItem.SetActive(true);
                        break;
                    case CostKindEnum.Props:
                        hideItem.SetActive(true);
                        break;
                    case CostKindEnum.LightingBox:
                        hideItem.SetActive(true);
                        foreach (var cpt in hidects)
                        {
                            cpt.enabled = true;
                        }
                        hidects.Clear();
                        break;
                    case CostKindEnum.RoadWater:
                        hideItem.SetActive(true);
                        break;
                    default:
                        break;
                }
                if (posInfo.costItems == null) { posInfo.costItems = new CostKind[(int)CostKindEnum.MaxCount]; }
                posInfo.costItems[(int)costKind.kindEnum] = costKind;


            }

            public override BaseCmd Init(params object[] data)
            {
                costKind = (CostKind)data[0];
                posInfo = (PosInfo)data[1];
                return this;
            }
        }
        class PosInfo
        {
            public Vector3 pos;
            public Quaternion rot;
             private bool orderSord = false;
            public float cpuTime;
            public float gpuTime;
            public CostKind[] costItems;

            internal PosInfo initFrom(string[] pos)
            {
                this.pos = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
                rot = Quaternion.Euler(float.Parse(pos[3]), float.Parse(pos[4]),0);
                return this;
            }
        };
        enum CostKindEnum
        {
            Tree = 0, Grass, Terrain, House, Props, LightingBox, RoadWater, MaxCount
        };
        class CostKind
        {
            public string kindName;
            public CostKindEnum kindEnum;

            public float costCpuTime_without_this;
            public float costGpuTime_without_this;
             
            public float selfCost(float total)
            {
                 
              
                    float cst = total -( gpuData? costGpuTime_without_this:costCpuTime_without_this);
                    if (cst < 0.2)
                    {
                        return 0;
                    }
                    else
                    {
                        return cst;
                    }
               
            }
        };
        static string[] kindNames = { "树木", "花草", "地表", "房子", "物件", "后效", "路海" };

        static Camera camera;
        static int step = 100;
        static Terrain terrain;
        static List<PosInfo> infoList;
        static int waitFrame = 0;
        static bool currentSampling = false;
        static Queue<BaseCmd> cmdQueue;

        static string samplerPosListStr="";

        [MenuItem("场景物件/CpuGpuSampler/singleTerrainMode")]
        static void singleTerrainModeCpu()
        {
            GetWindow<CpuGpuSampler>().Show();
        }
      
        private void OnGUI()
        {
            gpuData = EditorGUILayout.Toggle("gpu数据", gpuData);
            if (GUILayout.Button("相隔100米采样")) {
                startSample(false);
            }
            samplerPosListStr = EditorGUILayout.TextArea(samplerPosListStr);
            if (GUILayout.Button("固定点采样")) {
                startSample(true);
            }

        }
        static void startSample(bool specialPos) { 

            infoList = new List<PosInfo>();
            camera = Camera.main;
            terrain = FindObjectOfType<Terrain>();

            waitFrame = 0;
            var cmdEpy = EmptyCmd.instance;
            cmdQueue = new Queue<BaseCmd>();

            if (specialPos == false)
            {
                for (int i = 0; i < terrain.terrainData.size.z / step; i++)//terrain.terrainData.size.z/step
                {
                    for (int j = 0; j < terrain.terrainData.size.x / step; j++)//terrain.terrainData.size.x / step
                    {

                        cmdQueue.Enqueue(new ChangeCameraCmd().Init(j * step, i * step, 0));


                        cmdQueue.Enqueue(new EmptyCmd().Init(30));
                 

                        PosInfo posInfo = new PosInfo();

                        cmdQueue.Enqueue( new CpuGpuRecordCmd().Init(posInfo));
                   




                    }

                }
            }
            else {
            
               var poslistStrs= samplerPosListStr.Split('\n');
                 
                foreach (var item in poslistStrs)
                {
                    var pos = item.Split(',');

                    //  Debug.Log(pos[0]+":"+pos[1] + ":" + pos[2] + ":" + pos[3] + ":" + pos[4]);
                    cmdQueue.Enqueue(new ChangeCameraCmd().Init(new PosInfo().initFrom(pos)));
                    cmdQueue.Enqueue(new EmptyCmd().Init(15));
                    PosInfo posInfo = new PosInfo();
                    cmdQueue.Enqueue(new CpuGpuRecordCmd().Init(posInfo));


                }
              //  return;
            }
            cmdQueue.Enqueue(new CpuGpuRecordFinishCmd().Init(!specialPos));


            currentSampling = true;
            EditorApplication.update = samplingInTerrain;


        }

        public static void samplingInTerrain()
        {
            if (Application.isPlaying == false) return;
            if (currentSampling == false) return;
            waitFrame--;
            if (waitFrame > 0) return;

            cmdQueue.Dequeue().Execute();
            if (cmdQueue.Count == 0)
            {
                currentSampling = false;
                string log = DateTime.Now.ToShortDateString() + "\t " + terrain.gameObject.scene.name + "\n";
                for (int i = 0; i < 10; i++)
                {
                    log += "采样坐标\t" + infoList[i].pos.x+","+ infoList[i].pos.y + "," + infoList[i].pos.z + "," + (int)infoList[i].rot.eulerAngles.x + "," + (int)+infoList[i].rot.eulerAngles.y + "\n";
                    log += "FPS\t " + (int)(1000 / infoList[i].cpuTime) + "\n";
                    if(gpuData==false)
                    log += "CPU\t " + infoList[i].cpuTime + "\n";
                   
                    else
                        log += "GPU\t " + infoList[i].gpuTime + "\n";
                    //  float other = infoList[i].cpuTime;
                    for (int k = 0; k < (int)CostKindEnum.MaxCount; k++)
                    {
                        if (infoList[i].costItems[k] != null)
                        {
                            float cost = 0;
                            if (gpuData == false)
                             cost=infoList[i].costItems[k].selfCost(infoList[i].cpuTime);
                            else
                                cost = infoList[i].costItems[k].selfCost(infoList[i].gpuTime);
                            // other -=cpu;
                            log += infoList[i].costItems[k].kindName+"\t "+ cost + "\n";
                            
                        }
                        else
                        {
                            log += kindNames[k] + "\t 0" + "\n";

                        }
                    }
                    //  log += "其他\t" + other;
                    // Debug.Log("mainTime:" + infoList[i].mainTime + ",renderTime:" + infoList[i].renderTime + ",pos:" + infoList[i].pos + ",rot:" + infoList[i].rot.eulerAngles.y);


                }
                Debug.Log(log);
                GUIUtility.systemCopyBuffer = log;

            }
        }



        private static int sortMain(PosInfo x, PosInfo y)
        {
            if(gpuData)
                   return (int)((y.gpuTime - x.gpuTime) * 10000);
            else
            return (int)((y.cpuTime - x.cpuTime) * 10000);
        }
 
    }

}