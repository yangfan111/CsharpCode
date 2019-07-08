using System;
using System.Collections.Generic;
using App.Client.GPUInstancing.Core.Data;
using App.Client.GPUInstancing.Core.Spatial;
using App.Client.GPUInstancing.Core.Terrain;
using App.Client.GPUInstancing.Core.Utils;
using Core.Utils;
using UnityEngine;

namespace App.Client.GPUInstancing.Core
{
    public class GpuInstancingTreeOnTerrain : GpuInstancingPipeline
    {
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(GpuInstancingTreeOnTerrain));
        private readonly GpuInstancingSpatialPartition<GpuInstancingTreeNode> _data =
            new GpuInstancingSpatialPartition<GpuInstancingTreeNode>();

        private readonly ComputeShader _instantiationShader;
        private TreeInstancingDraw[] _instancingDraw;
        private TerrainProperty _terrainProperty;
        
        private int _nodeSize = Constants.DefaultDetailNodeSize * 2;

        private bool _gotSthToDraw;
        public GpuInstancingTreeOnTerrain(ComputeShader instantiation, ComputeShader merge,
            ComputeShader visibility) : base(visibility, null)
        {
            _instantiationShader = instantiation;

            _data.SetMergeShader(merge);
        }

        public bool TerrainLoaded(UnityEngine.Terrain setting, float treeDistance, Vector3 basePos, float[,] heightMap)
        {
            return TerrainLoadedInternal(setting, treeDistance, basePos, heightMap, null);
        }

        public bool TerrainLoaded(TextAsset asset, UnityEngine.Terrain setting, float treeDistance, Vector3 basePos)
        {
            return TerrainLoadedInternal(setting, treeDistance, basePos, null, asset);
        }

        private bool TerrainLoadedInternal(UnityEngine.Terrain setting, float treeDistance, Vector3 basePos,
            float[,] heightMap, TextAsset asset)
        {
            var data = setting.terrainData;

            var prototypes = data.treePrototypes;
            var prototypeCount = prototypes.Length;
            
            if (prototypeCount == 0)
            {
                _logger.Warn("no tree prototypes exists");
                return false;
            }

            var heightMapResolution = data.heightmapResolution;
            var division = (heightMapResolution - 1) / _nodeSize;

            if (!Helper.AlmostEqual(heightMapResolution, division * _nodeSize + 1))
            {
                _logger.WarnFormat("unmatch param in tree: heightMapResolution -> {0}, nodeSize -> {1}, division -> {2}",
                    heightMapResolution, _nodeSize, division);
                return false;
            }

            var allTrees = data.treeInstances;
            var treeCount = allTrees.Length;
            var nodeMargin = float.MinValue;

            if (_instancingDraw == null)
                _instancingDraw = new TreeInstancingDraw[prototypeCount];

            for (int i = 0; i < prototypeCount; ++i)
            {
                if (_instancingDraw[i] == null)
                {
                    var renderer = new InstancingRenderer(prototypes[i].prefab);
                    nodeMargin = Mathf.Max(nodeMargin, renderer.SphereRadius);
                    _instancingDraw[i] = new TreeInstancingDraw(renderer, VisibilityShader);
                }
                else
                    nodeMargin = Mathf.Max(nodeMargin, _instancingDraw[i].RendererSphereRadius);
            }

            var nodeSize = new Vector2(data.size.x / division, data.size.z / division);
            _data.SetGridParam(data.size.x, treeDistance, nodeSize);
            
            var cluster = new GpuInstancingNodeCluster<GpuInstancingTreeNode>();
            cluster.InitDivision(basePos, nodeSize, division, division, treeDistance);
            if (heightMap != null)
                cluster.InitHeightMap(heightMap, data.size.y, nodeMargin);
            else
            {
                // ignore version
                int dataIndex = 6;
                unsafe
                {
                    var head = new IntPtr(asset.GetBytesIntPtr().ToInt64() + dataIndex);
                    ushort* flowData = (ushort*) head.ToPointer();
                    var detailCount = *flowData;
                    dataIndex += 2 * (1 + detailCount);
                }

                cluster.InitHeightMap(asset, dataIndex, data.size.y, heightMapResolution, nodeMargin);
            }
            cluster.TerrainName = setting.name;
            
            _terrainProperty = new TerrainProperty();
            _terrainProperty.InitForTree(setting, data, treeDistance, basePos);

            var treeIndex = new List<int>[division, division][];
            var maxIndex = division - 1;
            var maxCountInPrototype = new List<int>(prototypeCount);
            var dummy = new List<int>[prototypeCount];

            for (int i = 0; i < prototypeCount; ++i)
            {
                maxCountInPrototype.Add(0);
                dummy[i] = new List<int>();
            }
            
            for (int i = 0; i < treeCount; ++i)
            {
                var tree = allTrees[i];
                var x = Mathf.Min(Mathf.FloorToInt(tree.position.x * division), maxIndex);
                var z = Mathf.Min(Mathf.FloorToInt(tree.position.z * division), maxIndex);

                if (treeIndex[x, z] == null)
                    treeIndex[x, z] = new List<int>[prototypeCount];
                
                if (treeIndex[x, z][tree.prototypeIndex] == null)
                    treeIndex[x, z][tree.prototypeIndex] = new List<int>();
                
                treeIndex[x, z][tree.prototypeIndex].Add(i);

                maxCountInPrototype[tree.prototypeIndex] = Mathf.Max(maxCountInPrototype[tree.prototypeIndex], treeIndex[x, z][tree.prototypeIndex].Count);
            }

            for (int x = 0; x < division; ++x)
            {
                for (int z = 0; z < division; ++z)
                {
                    var node = new GpuInstancingTreeNode
                    {
                        TerrainName = setting.name,
                        X = x,
                        Z = z
                    };

                    node.SetTreeInstances(treeIndex[x, z] ?? dummy, allTrees);
                    node.SetInstantiationShader(_instantiationShader);
                    node.SetShaderProperty(_terrainProperty);
                    
                    cluster.AddNode(x, z, node);
                }
            }
            
            cluster.SetMaxCountInLayer(maxCountInPrototype);
            
            _data.AddCluster(cluster);
            return true;
        }
        
        public void TerrainUnloaded(Vector3 minPos)
        {
            _data.RemoveCluster(minPos);
        }

        protected override void FrustumCulling()
        {
            if (_instancingDraw != null)
                _data.DistanceAndFrustumCulling(GetFrustum());
        }

        protected override void CreateInstance()
        {
            _gotSthToDraw = false;

            if (_instancingDraw == null)
                return;
            
            _data.CreateUnitedInstance(_instancingDraw);
            foreach (var instance in _instancingDraw)
            {
                if (instance.CanDraw())
                {
                    _gotSthToDraw = true;
                    _terrainProperty.SetTreeCullingProperty(VisibilityShader);
                    VisibilityDetermination(instance);
                }
            }
        }

        protected override void InstancingDraw(Camera camera)
        {
            if (_gotSthToDraw)
            {
                foreach (var instance in _instancingDraw)
                {
                    if (instance.CanDraw())
                    {
                        instance.Draw(camera);
                    }
                }
            }
        }
    }
}

