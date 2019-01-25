    	using System.Collections;
using System.Collections.Generic;
using UnityEngine;
	using UnityEngine.Rendering;
namespace ArtPlugins {
    [System.Serializable]
    public struct SubMeshMaterialGroup
    {
        public Mesh mesh;
        public Material[] materials;
    }
    //args[0] = (uint)instanceMesh.GetIndexCount(subMeshIndex);
    //args[1] = (uint)instanceCount;
    //args[2] = (uint)instanceMesh.GetIndexStart(subMeshIndex);
    //args[3] = (uint)instanceMesh.GetBaseVertex(subMeshIndex);
    public class GpuTreeCS : MonoBehaviour
    {
        struct TreeInfo
        {
            public Vector3 position;
            public Vector3  param;//scale,rot,treeIndex;

        };
        private const int submeshCountMax = 4;
        private const int treeKindsMax = 20;
        //private const int treeLodMax = 3;

        public SubMeshMaterialGroup[] lodMeshs;


        public ComputeShader shader;
        public Shader treeShader;
        private ComputeBuffer treeLibBuffer;
        private ComputeBuffer bufferWithArgsMulti;
        private ComputeBuffer renderIDsBuffer;
        private uint[] argsMulti = new uint[5 * 2];
        private Bounds _bounds = new Bounds(Vector3.zero, Vector3.one * 10000);

 
        public TerrainData[] terrainDatas;
        private int treeKindsCount;

        private int treeLibSize;
        [Range(1, 1000)]
        public int lod1Dis = 30;
        [Range(1, 1000)]
        public int lod2Dis = 120;
        [Range(1, 1000)]
        public int lod3Dis = 400;
        [Range(1, 1000)]
        public int lodCull = 600;
        // Use this for initialization
        void Start()
        {
            enabled = false;
            return;

            treeLibBuffer = new ComputeBuffer(100000, 24);
            List<TreeInfo> treeList = new List<TreeInfo>();
            updateDistanceSet();

            treeKindsCount = lodMeshs.Length / 4;

            foreach (var terrainData in terrainDatas)
            {
                if (terrainData == null) continue;
              var tpos=  TerrainItemInstanceTools.getTerrainPos(terrainData);
                foreach (var tree in terrainData.treeInstances)
                {


                    if (tree.prototypeIndex >= treeKindsCount) continue;
                    TreeInfo treeInfo = new TreeInfo();
                    
                    treeInfo.position = new Vector4(tree.position.x * 1000 + tpos.x, tree.position.y * 310 + tpos.y, tree.position.z * 1000 + tpos.z);
                    treeInfo.param.x = tree.heightScale;
                    treeInfo.param.y = tree.rotation * Mathf.Deg2Rad;
                    treeInfo.param.z = tree.prototypeIndex;
                    treeList.Add(treeInfo);


                }
            }

            treeLibSize = treeList.Count;
            print(treeList.Count);
            treeLibBuffer.SetData(treeList.ToArray());
            shader.SetBuffer(0, "treeLibBuffer", treeLibBuffer);

            renderIDsBuffer = new ComputeBuffer(4000 * treeKindsCount * 4, 4);//10 kinds of tree
            shader.SetBuffer(0, "renderIDsBuffer", renderIDsBuffer);
            argsMulti = new uint[5 * treeKindsCount * 4 * submeshCountMax];
            for (int i = 0; i < treeKindsCount * 4; i++)
            {
                 
                for (int j = 0; j < Mathf.Min(submeshCountMax, lodMeshs[i].materials.Length); j++)
                {
                    if (j >= lodMeshs[i].mesh.subMeshCount) continue;
                    argsMulti[i * 5 * submeshCountMax + j * 5] = lodMeshs[i].mesh.GetIndexCount(j);
                    argsMulti[i * 5 * submeshCountMax + j * 5 + 2] = lodMeshs[i].mesh.GetIndexStart(j);

                    var mat = lodMeshs[i].materials[j];
                    if (mat == null) continue;
                    mat.SetBuffer("treeLibBuffer", treeLibBuffer);
                    mat.SetBuffer("renderIDsBuffer", renderIDsBuffer);
                    mat.SetInt("meshIndex", i);
                }

            }



            bufferWithArgsMulti = new ComputeBuffer(argsMulti.Length, sizeof(uint), ComputeBufferType.IndirectArguments);
            bufferWithArgsMulti.SetData(argsMulti);

            shader.SetBuffer(0, "bufferWithArgsMulti", bufferWithArgsMulti);
            shader.SetBuffer(1, "bufferWithArgsMulti", bufferWithArgsMulti);




        }
        void tryDispose(ref ComputeBuffer buffer)
        {
            if (buffer != null) buffer.Dispose();
            buffer = null;
        }
        [ContextMenu("updateDistanceSet")]
        void updateDistanceSet() {

            shader.SetVector("lodDisSet", new Vector4(lod1Dis, lod2Dis, lod3Dis, lodCull));
        }
        [ContextMenu("collectMeshs")]
        void collectMeshs()
        {
            int count = 0;
            var testTerrainData = terrainDatas[0];
            int treeKinds = Mathf.Min(treeKindsMax, testTerrainData.treePrototypes.Length);
            lodMeshs = new SubMeshMaterialGroup[treeKinds * 4];
            for (int i = 0; i < treeKinds; i++)
            {
                var tree = testTerrainData.treePrototypes[i];
                LODGroup lODGroup = tree.prefab.GetComponent<LODGroup>();
                if (lODGroup != null)
                {
                    for (int lod = 0, len = Mathf.Min(lODGroup.GetLODs().Length, 3); lod < len; lod++)
                    {
                        MeshRenderer meshRenderer = lODGroup.GetLODs()[lod].renderers[0] as MeshRenderer;
                        if (meshRenderer == null) break;
                        SubMeshMaterialGroup smg;

                        smg.mesh = meshRenderer.GetComponent<MeshFilter>().sharedMesh;
                        smg.materials = meshRenderer.sharedMaterials;
                        foreach (var m in smg.materials)
                        {
                            m.shader = treeShader;
                        }
                        lodMeshs[i * 4 + lod] = smg;

                    }


                }
                if (count++ >= treeKindsMax) break;

            }

        }
        private void OnDestroy()
        {


            tryDispose(ref bufferWithArgsMulti);
            tryDispose(ref renderIDsBuffer);
            tryDispose(ref treeLibBuffer);
        }

        private int cmrPosID = -1;
        private int cmrDirID = -1;
        public bool culling = true;
        // Update is called once per frame
        void Update()
        {
            if (cmrPosID == -1) {
                cmrPosID = Shader.PropertyToID("rolePos");
                cmrDirID = Shader.PropertyToID("roleDir");
            }
            shader.SetVector(cmrPosID, Camera.main.transform.position);
            shader.SetVector(cmrDirID, Camera.main.transform.forward);

            //for (int i = 0; i < argsMulti.Length; i += 5)
            //{
            //    argsMulti[i + 1] = 0;
            //}

            //bufferWithArgsMulti.SetData(argsMulti);
            // int t = (int)(Mathf.Sqrt(argsMulti.Length / 5) )/4+1;
            if (culling)
            {
                shader.Dispatch(1, argsMulti.Length / 40 + 1, 1, 1);
               // print("treeLibSize:"+treeLibSize);
                shader.Dispatch(0, treeLibSize, 1, 1);

                //  int[] temp = new int[5*20];
                // bufferWithArgsMulti.GetData(temp);//353
                ////print("tree renderid :"+temp[353]);
                // string tempStr = "";

                // foreach (var item in temp)
                //{
                //    tempStr += item + ",";
                // }

                // print(tempStr);
            }




            for (int i = 0; i < lodMeshs.Length; i++)
            {
                var lod = lodMeshs[i];
                if (lod.mesh == null) continue;
                //20 per material   4x20 per meshs 
                for (int j = 0; j < lod.materials.Length; j++)
                {
                    if(lod.materials[j]!=null)
                    Graphics.DrawMeshInstancedIndirect(lod.mesh, j, lod.materials[j], _bounds, bufferWithArgsMulti, i * 20 * submeshCountMax + j * 20, null, ShadowCastingMode.Off, false);

                }
            }
            //enabled = false;
            // print(args[1]);
        }
    }

}