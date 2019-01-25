using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
namespace ArtPlugins
{

    public struct Point
    {
        public Vector3 vertex;
        public Vector3 normal;
        public Vector4 tangent;
        public Vector2 uv;
    }
    public class GpuGrassCS : MonoBehaviour
    {
        private const int GrassInstanceMax = 10000000;
        private const int LayerMax =4;
        public Mesh mesh;
        public GameObject role;
        public Material grassMateral;
         public List<TerrainData> terrianDatas;
        public ComputeShader shader;
        public bool isDraw = true;
        public bool isCulling = true;
        ComputeBuffer bufferWithArgs;
        private uint[] args;
        private Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 10000);

        // for init 
        private ComputeBuffer densityAllBuffer;
        private ComputeBuffer heightmapAllBuffer;
  

        private int CSAddTerrainID;

        private int[] allLayerData;

        private int[] flushCountArray;

        //for culling 
        private int CSCullingID;
        private ComputeBuffer rectGrassBuffer;
        private bool initTerrainCompleted = false;
       // public RenderTexture debugTex;
        //private ComputeBuffer renderCountBuffer;
        // Use this for initialization
        void Start()
        {


            enabled = false;
            return;
            role = Camera.main.gameObject;
            int count = 1;
            Stopwatch sd = new Stopwatch();
            sd.Start();
            args = new uint[] { mesh.GetIndexCount(0), (uint)count, 0, 0, 0 };
            bufferWithArgs = new ComputeBuffer(5, sizeof(uint), ComputeBufferType.IndirectArguments);
            bufferWithArgs.SetData(args);
            init();
            sd.Stop();
            print("init time:" + sd.ElapsedMilliseconds);
            sd.Reset();
            sd.Start();
            //	 flushRect(terrianDatas[0], out count);
            sd.Stop();
            print("flushRect time:" + sd.ElapsedMilliseconds);



            sd.Reset();
            sd.Start();
            // culling();

            sd.Stop();
            print("culling time:" + sd.ElapsedMilliseconds);



        }
        bool firstLoad = true;
        void tryLoadTerrainData()
        {
            if (terrianDatas.Count == 0) return;
            int count=0;
          

            //  addToAllData(terrianDatas[0], new Vector3(1024 * (index % 3), 0, 1024 * (index / 3)), out count);
            if(terrianDatas[0]!=null)
            addToAllData(terrianDatas[0], TerrainItemInstanceTools.getTerrainPos(terrianDatas[0]));

            terrianDatas.RemoveAt(0);
            //	args[1] = (uint) count;
            //		bufferWithArgs.SetData(args);
            print("all count:" + count);
            if (terrianDatas.Count == 0)
            {
                 allLayerData = null;
                initTerrainCompleted = true;
                if (GrassInstanceMax <= count) {
                    UnityEngine.Debug.LogError("grass count :"+count+" more than "+ GrassInstanceMax);
                }
                culling();
            }

        }

        private void addToAllData(TerrainData terrainData, Vector3 terrainPosition)
        {

            if (firstLoad)
            {
                firstLoad = false;
                Color[] colors = new Color[terrainData.detailPrototypes.Length*2];
                Vector4[] scales = new Vector4[terrainData.detailPrototypes.Length];
                for (int i = 0; i < terrainData.detailPrototypes.Length; i++)
                {
                    var detail = terrainData.detailPrototypes[i];
                    colors[i*2] = detail.dryColor;
                    colors[i*2+1] = detail.healthyColor;

                    scales[i] =new Vector4( detail.minWidth, detail.maxWidth, detail.minHeight, detail.maxHeight);
                     

                }
                grassMateral.SetColorArray("lerpColors", colors);
                grassMateral.SetVectorArray("scales", scales);
            }
            var sd = new Stopwatch();
            ComputeBuffer heightBuffer = new ComputeBuffer(1024 * 1024, 4);
            shader.SetBuffer(CSAddTerrainID, "heightmap1TerrainBuffer", heightBuffer);

            ComputeBuffer densityBuffer = new ComputeBuffer(1024 * 1024 * LayerMax, 4);
            shader.SetBuffer(CSAddTerrainID, "density1TerrainBuffer", densityBuffer);
            sd.Start();
          int tIndex =  TerrainItemInstanceTools.getTerrainIndex(terrainData);
            shader.SetInt("terrainIndex", tIndex);
           // print(tIndex);
            for (int layer = 0; layer < terrainData.detailPrototypes.Length; layer++)
            {
                if(layer>=LayerMax)break;

                var layerData = terrainData.GetDetailLayer(0, 0, 1024, 1024, layer);

                int offset = 1024 * 1024 * layer;



                for (int i = 0; i < 1024; i++)
                {
                    for (int j = 0; j < 1024; j++)
                    {
                        allLayerData[i * 1024 + j + offset] = layerData[i, j];
                    }

                }
            }
            
            densityBuffer.SetData(allLayerData);

            float[,] hights = terrainData.GetHeights(0, 0, 1024, 1024);
            heightBuffer.SetData(hights);
           
            shader.Dispatch(CSAddTerrainID, 1024 / 8, 1024 / 8, 1);
            heightBuffer.Dispose();
            densityBuffer.Dispose();
            sd.Stop();
            print("load terrain data:" + sd.ElapsedMilliseconds);
        }

        void init()
        {

            CSAddTerrainID = shader.FindKernel("CSAddTerrain");
            CSCullingID = shader.FindKernel("CSCulling");
            allLayerData = new int[1024 * 1024 * LayerMax];


            //debugTex = new RenderTexture(1024, 1024, 0);
            //debugTex.enableRandomWrite = true;
            //debugTex.Create();
            //shader.SetTexture(CSAddTerrainID, "debugT2D", debugTex);
            //shader.SetTexture(CSCullingID, "debugT2D", debugTex);

            densityAllBuffer = new ComputeBuffer(1204 * 1024 * 64 * LayerMax, 4);
            heightmapAllBuffer= new ComputeBuffer(1204 * 1024 * 64, 4);
 
            

            shader.SetBuffer(CSAddTerrainID, "densityAllBuffer", densityAllBuffer);
            shader.SetBuffer(CSAddTerrainID, "heightmapAllBuffer", heightmapAllBuffer);
 
 
            //culling init

            rectGrassBuffer = new ComputeBuffer(200000, 28);
            shader.SetBuffer(CSCullingID, "bufferWithArgs", bufferWithArgs);
            shader.SetBuffer(CSCullingID, "rectGrassBuffer", rectGrassBuffer);

            shader.SetBuffer(CSCullingID, "densityAllBuffer", densityAllBuffer);
            shader.SetBuffer(CSCullingID, "heightmapAllBuffer", heightmapAllBuffer);
            grassMateral.SetBuffer("rectGrassBuffer", rectGrassBuffer);
 

        }


        private int cmrPosID = -1;
        private int cmrDirID = -1;
       void culling()
      {

            if (initTerrainCompleted == false) return;
            if (cmrPosID == -1)
            {
                cmrPosID = Shader.PropertyToID("rolePos");
                cmrDirID = Shader.PropertyToID("roleDir");
            }
            shader.SetVector(cmrPosID, Camera.main.transform.position);
            shader.SetVector(cmrDirID, Camera.main.transform.forward);

            args[1] = 0;
            bufferWithArgs.SetData(args);

            //total =(int) Mathf.Sqrt(total);
            //print("total count:"+total);

            shader.Dispatch(CSCullingID, 512/8 , 512/8, 1);
         
             //int[] temp = new int[5];
             //bufferWithArgs.GetData(temp);
             // print("culling :"+temp[1]);

        }

        void tryDispose(ref ComputeBuffer buffer)
        {
            if (buffer != null) buffer.Dispose();
            buffer = null;
        }

        private void OnDestroy()
        {

 

            tryDispose(ref bufferWithArgs);
            tryDispose(ref this.densityAllBuffer);
            tryDispose(ref this.heightmapAllBuffer);
             tryDispose(ref this.rectGrassBuffer);
             
         



        }

 
        void Update()
        {

            tryLoadTerrainData();
            if (initTerrainCompleted == false) return;
            bounds.center = role.transform.position;

            if (isCulling)
            {
                //isCulling = false;
                 culling();
            }
 
            Graphics.DrawMeshInstancedIndirect(mesh, 0, grassMateral, bounds, bufferWithArgs, 0, null, ShadowCastingMode.Off, false);
      


        }
    }

}