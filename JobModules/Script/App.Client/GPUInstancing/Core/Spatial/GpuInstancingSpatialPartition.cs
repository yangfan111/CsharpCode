using System;
using System.Collections;
using System.Collections.Generic;
using App.Client.GPUInstancing.Core.Data;
using App.Client.GPUInstancing.Core.Utils;
using Core.Components;
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
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(GpuInstancingSpatialPartition<T>));

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

        private GpuInstancingNodeIndicator _curInstantiatingNode;
        private GpuInstancingNodeIndicator _previousInstantiatingNode;
        private GpuInstancingNodeIndicator[] _cachedNodes;
        // trees can be seen far far away, so _cachedNodes.Length can be too large to iterate
        private int _cachedNodesActualLength = 0;
        
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
                _cachedNodes = new GpuInstancingNodeIndicator[cullingGridCountX * cullingGridCountZ];
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

            if (_nodeClusters[xIndex, zIndex] != null)
            {
                _nodeClusters[xIndex, zIndex].Clean();
                _nodeClusters[xIndex, zIndex] = null;
            }
            else
                _logger.WarnFormat("remove not existed terrain: {0}", minPosition.ToStringExt());

            bool canShrink = true;
            for (int i = _cachedNodesActualLength; i > 0; --i)
            {
                var index = i - 1;
                var node = _cachedNodes[index];
                if (node != null)
                {
                    if (node.IsOutOfRange)
                    {
                        _cachedNodes[index] = null;

                        if (canShrink)
                            _cachedNodesActualLength = i - 1;

                        continue;
                    }

                    canShrink = false;
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

            for (int i = 0; i < _cachedNodesActualLength; ++i)
            {
                var node = _cachedNodes[i];
                if (node != null && node.IsInstantiated)
                    node.IsActive = frustum.IsNodeVisible(node);
            }
        }

        public void CreateUnitedInstance(InstancingDraw[] instancingData)
        {
            if (instancingData[0].State == InstancingDrawState.NotInitialized || _newInstanceCountLimit)
            {
                var dataLength = instancingData.Length;
                for (int i = 0; i < dataLength; ++i)
                {
                    instancingData[i].SetInstancingFullSizeParam(_cachedNodes.Length, _maxInstanceCountPerRenderInUnit[i]);
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

                            if (index >= _cachedNodesActualLength)
                                _cachedNodesActualLength = index + 1;

                            break;
                        }
                    }

                    if (index == count)
                        _logger.Error("Full Exception");

                    if (!_newInstanceCountLimit)
                        ReplaceBuffer(_previousInstantiatingNode.Node, index, instancingData);
                }
                else
                    _previousInstantiatingNode.Node.ReleaseBuffer();

                _previousInstantiatingNode = null;
            }

            if (_newInstanceCountLimit)
            {
                for (int i = 0; i < _cachedNodesActualLength; ++i)
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

                    bool canShrink = true;
                    for (int j = _cachedNodesActualLength; j > 0; --j)
                    {
                        var index = j - 1;
                        var node = _cachedNodes[index];
                        if (node != null)
                        {
                            if (node.IsOutOfRange)
                            {
                                _cachedNodes[index] = null;

                                if (canShrink)
                                    _cachedNodesActualLength = j - 1;

                                continue;
                            }

                            if (node.IsActive)
                            {
                                var realCount = node.Node.GetInstancingDataCount(i);
                                instancingData[i].SetRealBlockCount(index, realCount);
                            }

                            canShrink = false;
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
            var prototypeCount = instancingData.Length;
            var kernels = node.GetMergeKernels(_mergeShader);

            for (int i = 0; i < prototypeCount; ++i)
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

        public void DebugDraw()
        {
            for (int i = 0; i < _cachedNodesActualLength; ++i)
            {
                if (_cachedNodes[i] != null && _cachedNodes[i].IsActive)
                {
                    var mins = _cachedNodes[i].Vertices[0] - WorldOrigin.Origin;
                    var maxs = _cachedNodes[i].Vertices[7] - WorldOrigin.Origin;

                    Debug.DrawLine(mins, new Vector3(mins.x, mins.y, maxs.z), Color.red);
                    Debug.DrawLine(mins, new Vector3(mins.x, maxs.y, mins.z), Color.red);
                    Debug.DrawLine(mins, new Vector3(maxs.x, mins.y, mins.z), Color.red);

                    Debug.DrawLine(maxs, new Vector3(maxs.x, maxs.y, mins.z), Color.red);
                    Debug.DrawLine(maxs, new Vector3(maxs.x, mins.y, maxs.z), Color.red);
                    Debug.DrawLine(maxs, new Vector3(mins.x, maxs.y, maxs.z), Color.red);

                    Debug.DrawLine(new Vector3(maxs.x, mins.y, mins.z), new Vector3(maxs.x, mins.y, maxs.z), Color.red);
                    Debug.DrawLine(new Vector3(maxs.x, mins.y, mins.z), new Vector3(maxs.x, maxs.y, mins.z), Color.red);

                    Debug.DrawLine(new Vector3(mins.x, maxs.y, mins.z), new Vector3(mins.x, maxs.y, maxs.z), Color.red);
                    Debug.DrawLine(new Vector3(mins.x, maxs.y, mins.z), new Vector3(maxs.x, maxs.y, mins.z), Color.red);

                    Debug.DrawLine(new Vector3(mins.x, mins.y, maxs.z), new Vector3(mins.x, maxs.y, maxs.z), Color.red);
                    Debug.DrawLine(new Vector3(mins.x, mins.y, maxs.z), new Vector3(maxs.x, mins.y, maxs.z), Color.red);
                }
            }
        }
    }
}
