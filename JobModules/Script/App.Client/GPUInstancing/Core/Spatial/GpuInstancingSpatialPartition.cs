using System.Collections;
using System.Collections.Generic;
using App.Client.GPUInstancing.Core.Data;
using App.Client.GPUInstancing.Core.Utils;
using UnityEngine;

namespace App.Client.GPUInstancing.Core.Spatial
{
    /// <summary>
    /// 1. Instantiation one by one
    /// 2. Cull node
    /// 3. Merge node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class GpuInstancingSpatialPartition<T> where T : GpuInstancingNode
    {
        enum NodeStatus
        {
            Invisible,
            ToBeInsstantiated,
            Instantiating,
            Cached
        }

        class NodeIndicator
        {
            public NodeStatus Status;
            public int BufferSlot;

            public bool InLastVisibleSet;
            public bool InCurVisibleSet;

            public bool IsBufferBuilded
            {
                get { return Status == NodeStatus.Cached; }
            }

            public void SetInvisible()
            {
                Status = NodeStatus.Invisible;
                BufferSlot = -1;
            }
        }

        private Vector3 _baseLocation;
        private Vector2 _nodeSize;

        private int _xCount;
        private int _zCount;
        private T[,] _nodes;
        private int[] _maxInstanceCountPerRenderInUnit;

        private NodeIndicator[,] _nodeStatuses;
        private Vector3[,] _aabbMins;
        private Vector3[,] _aabbMaxs;

        private Vector2 _lastVisibleStart = new Vector2(float.MinValue, float.MinValue);
        private Vector2 _lastVisibleEnd = new Vector2(float.MinValue, float.MinValue);
        private readonly Queue<int> _toBeInstantiatedNodes = new Queue<int>();
        private readonly Queue<int> _instantiatingNodes = new Queue<int>();
        private int[] _cachedNodeIndexes;

        private Vector2 _enableGridDistance;
        private Vector2 _minVisiblePos;
        private Vector2 _maxVisiblePos;

        private ComputeBuffer _heightMapBuffer;
        private float[] _heightMapData;

        private ComputeShader _mergeShader;

        public void InitDivision(Vector3 baseLocation, Vector2 nodeSize, int xCount, int zCount, float enableDistance)
        {
            _baseLocation = baseLocation;
            _nodeSize = nodeSize;

            _xCount = xCount;
            _zCount = zCount;

            _nodes = new T[_xCount, _zCount];
            _nodeStatuses = new NodeIndicator[_xCount, _zCount];
            for (int i = 0; i < _xCount; ++i)
            {
                for (int j = 0; j < _zCount; ++j)
                {
                    _nodeStatuses[i, j] = new NodeIndicator
                    {
                        Status = NodeStatus.Invisible,
                        BufferSlot = -1,
                        InLastVisibleSet = false,
                        InCurVisibleSet = false
                    };
                }
            }

            _minVisiblePos = new Vector2(
                baseLocation.x - enableDistance,
                baseLocation.z - enableDistance);
            _maxVisiblePos = new Vector2(
                baseLocation.x + nodeSize.x * _xCount + enableDistance,
                baseLocation.z + nodeSize.y * _zCount + enableDistance);

            _enableGridDistance = new Vector2(
                enableDistance / nodeSize.x,
                enableDistance / nodeSize.y);

            var count = Mathf.CeilToInt(_enableGridDistance.x * 2 + 1) * Mathf.CeilToInt(_enableGridDistance.y * 2 + 1);
                        
            _cachedNodeIndexes = new int[count];
            for (int i = 0; i < count; ++i)
            {
                _cachedNodeIndexes[i] = -1;
            }
        }

        public void InitHeightMap(float[,] heightMapData, float fullHeight)
        {
            var xCount = heightMapData.GetLength(0);
            var zCount = heightMapData.GetLength(1);

            float[,] mins = new float[_xCount, _zCount];
            float[,] maxs = new float[_xCount, _zCount];

            for (int i = 0; i < _xCount; ++i)
            {
                for (int j = 0; j < _zCount; ++j)
                {
                    mins[i, j] = int.MaxValue;
                    maxs[i, j] = int.MinValue;
                }
            }

            _heightMapData = new float[xCount * zCount];
            var index = 0;

            var xPixelsPerUnit = xCount / (float) _xCount;
            var zPixelsPerUnit = zCount / (float) _zCount;

            for (int i = 0; i < xCount; ++i)
            {
                for (int j = 0; j < zCount; ++j)
                {
                    var height = heightMapData[j, i];

                    var x = Mathf.FloorToInt(i / xPixelsPerUnit);
                    var z = Mathf.FloorToInt(j / zPixelsPerUnit);

                    if (x < _xCount && z < _zCount)
                    {
                        mins[x, z] = Mathf.Min(mins[x, z], height);
                        maxs[x, z] = Mathf.Max(maxs[x, z], height);
                    }

                    _heightMapData[index++] = height;
                }
            }

            for (int i = 0; i < _xCount; ++i)
            {
                for (int j = 0; j < _zCount; ++j)
                {
                    mins[i, j] *= fullHeight;
                    maxs[i, j] *= fullHeight;
                }
            }

            BuildAabb(mins, maxs);
        }

        public void AddNode(int x, int z, T node)
        {
            _nodes[x, z] = node;

            var newInstanceCount = node.MaxInstanceCount;
            var count = newInstanceCount.Length;

            if (_maxInstanceCountPerRenderInUnit == null)
                _maxInstanceCountPerRenderInUnit = new int[count];

            for (int i = 0; i < count; ++i)
            {
                _maxInstanceCountPerRenderInUnit[i] =
                    Mathf.Max(_maxInstanceCountPerRenderInUnit[i], newInstanceCount[i]);
            }
        }

        public void SetMergeShader(ComputeShader shader)
        {
            _mergeShader = shader;
        }

        public void FrustumCullingByGrid(CameraFrustum frustum)
        {
            var viewPoint = frustum.ViewPoint - _baseLocation;

            if (viewPoint.x >= _minVisiblePos.x && viewPoint.x <= _maxVisiblePos.x &&
                viewPoint.z >= _minVisiblePos.y && viewPoint.z <= _maxVisiblePos.y)
            {
                if (_heightMapBuffer == null)
                {
                    _heightMapBuffer = new ComputeBuffer(_heightMapData.Length, Constants.StrideSizeInt);
                    _heightMapBuffer.SetData(_heightMapData);
                }

                CullNodesByDistance(viewPoint);

                for (int i = (int) _lastVisibleStart.x; i < _lastVisibleEnd.x; ++i)
                {
                    for (int j = (int) _lastVisibleStart.y; j < _lastVisibleEnd.y; ++j)
                    {
                        if (i >= 0 && i < _xCount && j >= 0 && j < _zCount && _nodeStatuses[i, j].IsBufferBuilded)
                            _nodes[i, j].IsActive = frustum.IsDetailNodeVisible(_aabbMins[i, j], _aabbMaxs[i, j]);
                    }
                }
            }
            else if (viewPoint.x < _minVisiblePos.x - Constants.DetailDisableBufferLength ||
                     viewPoint.x > _maxVisiblePos.x + Constants.DetailDisableBufferLength ||
                     viewPoint.z < _minVisiblePos.y - Constants.DetailDisableBufferLength ||
                     viewPoint.z > _maxVisiblePos.y + Constants.DetailDisableBufferLength)
            {
                if (_heightMapBuffer != null)
                {
                    _heightMapBuffer.Release();
                    _heightMapBuffer = null;
                }
            }
        }

        public void CreateUnitedInstance(InstancingDraw[] instancingData)
        {
            if (instancingData[0].State == InstancingDrawState.NotInitialized)
            {
                var dataLength = instancingData.Length;
                for (int i = 0; i < dataLength; ++i)
                {
                    instancingData[i].SetInstancingCount(_cachedNodeIndexes.Length, _maxInstanceCountPerRenderInUnit[i]);
                }
            }

            if (_instantiatingNodes.Count != 0)
            {
                var index = _instantiatingNodes.Dequeue();
                var zIndex = index / _xCount;
                var xIndex = index - zIndex * _xCount;

                if (_nodeStatuses[xIndex, zIndex].Status == NodeStatus.Instantiating)
                {
                    _nodeStatuses[xIndex, zIndex].Status = NodeStatus.Cached;
                    var count = _cachedNodeIndexes.Length;
                    for (int i = 0; i < count; ++i)
                    {
                        if (_cachedNodeIndexes[i] == -1)
                        {
                            _cachedNodeIndexes[i] = index;
                            _nodeStatuses[xIndex, zIndex].BufferSlot = i;

                            ReplaceBuffer(xIndex, zIndex, instancingData);

                            break;
                        }
                    }
                }
                else
                    _nodes[xIndex, zIndex].ReleaseBuffer();
            }

            if (_toBeInstantiatedNodes.Count != 0)
                _instantiatingNodes.Enqueue(_toBeInstantiatedNodes.Dequeue());

            var drawCount = instancingData.Length;
            for (int i = 0; i < drawCount; ++i)
            {
                instancingData[i].ClearRealBlockCount();

                var maxCachedNodeCount = _cachedNodeIndexes.Length;
                for (int j = 0; j < maxCachedNodeCount; ++j)
                {
                    if (_cachedNodeIndexes[j] >= 0)
                    {
                        var z = _cachedNodeIndexes[j] / _xCount;
                        var x = _cachedNodeIndexes[j] - z * _xCount;

                        if (_nodes[x, z].IsActive)
                        {
                            var realCount = _nodes[x, z].GetInstancingDataCount(i);
                            instancingData[i].SetRealBlockCount(j, realCount);
                        }
                    }
                }
            }
        }

        private void BuildAabb(float[,] mins, float[,] maxs)
        {
            _aabbMins = new Vector3[_xCount, _zCount];
            _aabbMaxs = new Vector3[_xCount, _zCount];

            for (int i = 0; i < _xCount; ++i)
            {
                for (int j = 0; j < _zCount; ++j)
                {
                    _aabbMins[i, j] = new Vector3(_nodeSize.x * i, mins[i, j], _nodeSize.y * j);
                    _aabbMaxs[i, j] = new Vector3(_nodeSize.x * (i + 1), maxs[i, j], _nodeSize.y * (j + 1));
                }
            }
        }

        private void CullNodesByDistance(Vector3 viewPoint)
        {
            Vector2 visibleStart = new Vector2(
                Mathf.FloorToInt(viewPoint.x / _nodeSize.x - _enableGridDistance.x),
                Mathf.FloorToInt(viewPoint.z / _nodeSize.y - _enableGridDistance.y));
            Vector2 visibleEnd = new Vector2(
                Mathf.CeilToInt(viewPoint.x / _nodeSize.x + _enableGridDistance.x),
                Mathf.CeilToInt(viewPoint.z / _nodeSize.y + _enableGridDistance.y));

            if (!Helper.AlmostEqual(visibleStart, _lastVisibleStart) ||
                !Helper.AlmostEqual(visibleEnd, _lastVisibleEnd))
            {
                _toBeInstantiatedNodes.Clear();

                HashSet<int> allRelatedNode = new HashSet<int>();

                for (int i = (int) _lastVisibleStart.x; i < _lastVisibleEnd.x; ++i)
                {
                    for (int j = (int) _lastVisibleStart.y; j < _lastVisibleEnd.y; ++j)
                    {
                        if (i >= 0 && i < _xCount && j >= 0 && j < _zCount)
                        {
                            _nodeStatuses[i, j].InLastVisibleSet = true;
                            allRelatedNode.Add(j * _xCount + i);
                        }
                    }
                }

                for (int i = (int) visibleStart.x; i < visibleEnd.x; ++i)
                {
                    for (int j = (int) visibleStart.y; j < visibleEnd.y; ++j)
                    {
                        if (i >= 0 && i < _xCount && j >= 0 && j < _zCount)
                        {
                            _nodeStatuses[i, j].InCurVisibleSet = true;
                            allRelatedNode.Add(j * _xCount + i);
                        }
                    }
                }

                foreach (var index in allRelatedNode)
                {
                    var z = index / _xCount;
                    var x = index - z * _xCount;

                    if (_nodeStatuses[x, z].InLastVisibleSet &&
                        !_nodeStatuses[x, z].InCurVisibleSet)
                    {
                        if (_nodeStatuses[x, z].Status == NodeStatus.Cached)
                        {
                            _nodes[x, z].ReleaseBuffer();
                            _cachedNodeIndexes[_nodeStatuses[x, z].BufferSlot] = -1;
                        }
                        _nodeStatuses[x, z].SetInvisible();
                    }
                    else if (_nodeStatuses[x, z].InLastVisibleSet &&
                             _nodeStatuses[x, z].InCurVisibleSet)
                    {
                        if (_nodeStatuses[x, z].Status == NodeStatus.ToBeInsstantiated)
                            _toBeInstantiatedNodes.Enqueue(index);
                    }
                    else if (!_nodeStatuses[x, z].InLastVisibleSet &&
                             _nodeStatuses[x, z].InCurVisibleSet)
                    {
                        _nodeStatuses[x, z].Status = NodeStatus.ToBeInsstantiated;
                        _toBeInstantiatedNodes.Enqueue(index);
                    }

                    _nodeStatuses[x, z].InLastVisibleSet = false;
                    _nodeStatuses[x, z].InCurVisibleSet = false;
                }

                _lastVisibleStart = visibleStart;
                _lastVisibleEnd = visibleEnd;
            }

            if (_toBeInstantiatedNodes.Count != 0)
            {
                var index = _toBeInstantiatedNodes.Peek();
                var z = index / _xCount;
                var x = index - z * _xCount;

                _nodeStatuses[x, z].Status = NodeStatus.Instantiating;
                _nodes[x, z].BuildBuffer(_heightMapBuffer);
            }
        }

        private void ReplaceBuffer(int row, int column, InstancingDraw[] instancingData)
        {
            var count = instancingData.Length;

            var node = _nodes[row, column];
            var status = _nodeStatuses[row, column];
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
                    _mergeShader.SetInt(Constants.ShaderVariable.OutputDataOffset, status.BufferSlot * _maxInstanceCountPerRenderInUnit[i]);

                    _mergeShader.Dispatch(kernels[j].Kernel, Mathf.CeilToInt(dataLength / (float)Constants.MergeThreadCount), 1, 1);
                }
            }
        }
    }
}
