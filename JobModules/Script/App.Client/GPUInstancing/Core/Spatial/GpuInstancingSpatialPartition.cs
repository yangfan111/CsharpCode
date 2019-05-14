using System;
using System.Collections;
using System.Collections.Generic;
using App.Client.GPUInstancing.Core.Data;
using App.Client.GPUInstancing.Core.Utils;
using App.Client.SceneManagement.Vegetation;
using Core.Utils;
using UnityEngine;

namespace App.Client.GPUInstancing.Core.Spatial
{
    /// <summary>
    /// 1. Instantiation one by one
    /// 2. Cull node
    /// 3. Merge node
    /// </summary>
    class GpuInstancingSpatialPartition<T> where T : GpuInstancingNode
    {
        static LoggerAdapter logger = new LoggerAdapter("GpuInstancingSpatialPartition");

        private int _clusterDimensions = 8;
        private GpuInstancingNodeCluster<T>[,] _nodeClusters;
        private float _clusterSize;
        private float _cullingDistance;

        private int[] _maxInstanceCountPerRenderInUnit;
        private bool _newInstanceCountLimit;

        private ComputeShader _mergeShader;

        private int _lastVisibleStartX = int.MinValue;
        private int _lastVisibleEndX = int.MinValue;
        private int _lastVisibleStartZ = int.MinValue;
        private int _lastVisibleEndZ = int.MinValue;

        private NodeIndicator _curInstantiatingNode;
        private NodeIndicator _previousInstantiatingNode;
        private NodeIndicator[] _cachedNodes;
        
        public GpuInstancingSpatialPartition()
        {
            _nodeClusters = new GpuInstancingNodeCluster<T>[_clusterDimensions, _clusterDimensions];
        }

        public void SetGridParam(float clusterSize, float cullingDistance, Vector2 nodeSize)
        {
            if (_cachedNodes == null)
            {
                _clusterSize = clusterSize;
                _cullingDistance = cullingDistance + Constants.DetailDisableBufferLength;

                var cullingGridCountX = Mathf.CeilToInt(cullingDistance / nodeSize.x * 2 + 1);
                var cullingGridCountZ = Mathf.CeilToInt(cullingDistance / nodeSize.y * 2 + 1);
                _cachedNodes = new NodeIndicator[cullingGridCountX * cullingGridCountZ];
            }
        }

        public void AddCluster(GpuInstancingNodeCluster<T> cluster)
        {
            while (true)
            {
                var halfSize = _clusterDimensions / 2;
                var xIndex = Mathf.RoundToInt(cluster.MinPosition.x / _clusterSize) + halfSize;
                var zIndex = Mathf.RoundToInt(cluster.MinPosition.z / _clusterSize) + halfSize;

                if (xIndex >= _clusterDimensions || zIndex >= _clusterDimensions)
                    AdjustClusterSize();
                else
                {
                    _nodeClusters[xIndex, zIndex] = cluster;

                    var clusterMaxInstanceCount = cluster.MaxInstanceCountPerRenderInUnit;
                    var count = clusterMaxInstanceCount.Length;

                    if (_maxInstanceCountPerRenderInUnit == null)
                        _maxInstanceCountPerRenderInUnit = new int[count];

                    for (int i = 0; i < count; ++i)
                    {
                        if (_maxInstanceCountPerRenderInUnit[i] < clusterMaxInstanceCount[i])
                        {
                            _newInstanceCountLimit = true;
                            _maxInstanceCountPerRenderInUnit[i] = clusterMaxInstanceCount[i];
                        }
                    }

                    break;
                }
            }
        }

        public void RemoveCluster(Vector3 minPosition)
        {
            var halfSize = _clusterDimensions / 2;
            var xIndex = Mathf.RoundToInt(minPosition.x / _clusterSize) + halfSize;
            var zIndex = Mathf.RoundToInt(minPosition.z / _clusterSize) + halfSize;

            _nodeClusters[xIndex, zIndex].Clean();
            _nodeClusters[xIndex, zIndex] = null;

            var count = _cachedNodes.Length;
            for (int i = 0; i < count; ++i)
            {
                var node = _cachedNodes[i];
                if (node != null)
                {
                    if (node.IsOutOfRange)
                    {
                        _cachedNodes[i] = null;
                    }
                }
            }
        }

        public void DistanceAndFrustumCulling(CameraFrustum frustum)
        {
            var viewPoint = frustum.ViewPoint;
            var halfSize = _clusterDimensions / 2;

            var minVisiblePosX = viewPoint.x - _cullingDistance;
            var minVisiblePosZ = viewPoint.z - _cullingDistance;
            var maxVisiblePosX = minVisiblePosX + _cullingDistance * 2;
            var maxVisiblePosZ = minVisiblePosZ + _cullingDistance * 2;

            int curVisibleStartX = Mathf.FloorToInt(minVisiblePosX / _clusterSize) + halfSize;
            int curVisibleStartZ = Mathf.FloorToInt(minVisiblePosZ / _clusterSize) + halfSize;
            int curVisibleEndX = Mathf.CeilToInt(maxVisiblePosX / _clusterSize) + halfSize;
            int curVisibleEndZ = Mathf.CeilToInt(maxVisiblePosZ / _clusterSize) + halfSize;

            bool enoughRoom = true;
            
            for (int i = _lastVisibleStartX; i < _lastVisibleEndX; ++i)
            {
                for (int j = _lastVisibleStartZ; j < _lastVisibleEndZ; ++j)
                {
                    if (i >= 0 && i < _clusterDimensions && j >= 0 && j < _clusterDimensions)
                    {
                        var cluster = _nodeClusters[i, j];
                        if (cluster == null)
                            continue;

                        var node = cluster.DistanceAndFrustumCulling(viewPoint, enoughRoom);
                        if (node != null && enoughRoom)
                        {
                            _curInstantiatingNode = node;
                            enoughRoom = false;
                        }
                    }
                }
            }

            if (_lastVisibleStartX != curVisibleStartX || _lastVisibleEndX != curVisibleEndX ||
                _lastVisibleStartZ != curVisibleStartZ || _lastVisibleEndZ != curVisibleEndZ)
            {
                for (int i = curVisibleStartX; i < curVisibleEndX; ++i)
                {
                    for (int j = curVisibleStartZ; j < curVisibleEndZ; ++j)
                    {
                        if (i >= 0 && i < _clusterDimensions && j >= 0 && j < _clusterDimensions)
                        {
                            var cluster = _nodeClusters[i, j];
                            if (cluster == null)
                                continue;

                            var node = cluster.DistanceAndFrustumCulling(viewPoint, enoughRoom);
                            if (node != null && enoughRoom)
                            {
                                _curInstantiatingNode = node;
                                enoughRoom = false;
                            }
                        }
                    }
                }
                
                _lastVisibleStartX = curVisibleStartX;
                _lastVisibleEndX = curVisibleEndX;
                _lastVisibleStartZ = curVisibleStartZ;
                _lastVisibleEndZ = curVisibleEndZ;
            }

            if (!enoughRoom)
            {
                _curInstantiatingNode.SetInstantiating();
                _curInstantiatingNode.Node.BuildBuffer(_curInstantiatingNode.HeightBuffer());
            }

            var count = _cachedNodes.Length;
            for (int i = 0; i < count; ++i)
            {
                var node = _cachedNodes[i];
                if (node != null && node.IsInstantiated)
                    node.IsActive = frustum.IsDetailNodeVisible(node.Mins, node.Maxs);
            }
        }

        public void CreateUnitedInstance(InstancingDraw[] instancingData)
        {
            if (instancingData[0].State == InstancingDrawState.NotInitialized || _newInstanceCountLimit)
            {
                var dataLength = instancingData.Length;
                for (int i = 0; i < dataLength; ++i)
                {
                    instancingData[i].SetInstancingCount(_cachedNodes.Length, _maxInstanceCountPerRenderInUnit[i]);
                }
            }

            if (_previousInstantiatingNode != null)
            {
                if (_previousInstantiatingNode.IsDuringInstantiation)
                {
                    _previousInstantiatingNode.SetInstantiated();
                    var count = _cachedNodes.Length;
                    var index = 0;
                    for (; index < count; ++index)
                    {
                        if (_cachedNodes[index] == null)
                        {
                            _cachedNodes[index] = _previousInstantiatingNode;
                            break;
                        }
                    }

                    if (index == count)
                    {
                        logger.InfoFormat("Full Exception, mins: {0}, maxs: {1}",
                            _previousInstantiatingNode.Mins.ToStringExt(), _previousInstantiatingNode.Maxs.ToStringExt());
                    }

                    if (!_newInstanceCountLimit)
                        ReplaceBuffer(_previousInstantiatingNode.Node, index, instancingData);
                }
                else
                    _previousInstantiatingNode.Node.ReleaseBuffer();

                _previousInstantiatingNode = null;
            }

            if (_newInstanceCountLimit)
            {
                var count = _cachedNodes.Length;
                for (int i = 0; i < count; ++i)
                {
                    if (_cachedNodes[i] != null)
                        ReplaceBuffer(_cachedNodes[i].Node, i, instancingData);
                }

                _newInstanceCountLimit = false;
            }

            if (_curInstantiatingNode != null)
            {
                _previousInstantiatingNode = _curInstantiatingNode;
                _curInstantiatingNode = null;
            }

            var drawCount = instancingData.Length;
            for (int i = 0; i < drawCount; ++i)
            {
                if (instancingData[i].State == InstancingDrawState.Enable)
                {
                    instancingData[i].ClearRealBlockCount();

                    var count = _cachedNodes.Length;
                    for (int j = 0; j < count; ++j)
                    {
                        var node = _cachedNodes[j];
                        if (node != null)
                        {
                            if (node.IsOutOfRange)
                            {
                                _cachedNodes[j] = null;
                                continue;
                            }

                            if (node.IsActive)
                            {
                                var realCount = node.Node.GetInstancingDataCount(i);
                                instancingData[i].SetRealBlockCount(j, realCount);
                            }
                        }
                    }
                }
            }
        }

        private void AdjustClusterSize()
        {
            var newClusterDimensions = _clusterDimensions * 2;
            var offset = _clusterDimensions / 2;
            var newClusters = new GpuInstancingNodeCluster<T>[newClusterDimensions, newClusterDimensions];

            for (int i = 0; i < _clusterDimensions; ++i)
            {
                for (int j = 0; j < _clusterDimensions; ++j)
                {
                    newClusters[i + offset, j + offset] = _nodeClusters[i, j];
                }
            }

            _clusterDimensions = newClusterDimensions;
            _nodeClusters = newClusters;
        }


        public void SetMergeShader(ComputeShader shader)
        {
            _mergeShader = shader;
        }

        private void ReplaceBuffer(GpuInstancingNode node, int slot, InstancingDraw[] instancingData)
        {
            var count = instancingData.Length;
            var kernels = node.GetMergeKernels(_mergeShader);

            for (int i = 0; i < count; ++i)
            {
                var dataLength = node.GetInstancingDataCount(i);

                if (dataLength == 0)
                    continue;

                var inputs = node.GetInstancingData(i);
                for (int j = 0; j < inputs.Length; ++j)
                {
                    _mergeShader.SetBuffer(kernels[j].Kernel, kernels[j].Input, inputs[j]);
                    _mergeShader.SetBuffer(kernels[j].Kernel, kernels[j].Output, instancingData[i].GetMergedTargetBuffer(j));
                    _mergeShader.SetInt(Constants.ShaderVariable.InputDataCount, dataLength);
                    _mergeShader.SetInt(Constants.ShaderVariable.OutputDataOffset, slot * _maxInstanceCountPerRenderInUnit[i]);

                    _mergeShader.Dispatch(kernels[j].Kernel, Mathf.CeilToInt(dataLength / (float)Constants.MergeThreadCount), 1, 1);
                }
            }
        }
    }
}
