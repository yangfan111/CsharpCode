using System;
using System.Collections.Generic;
using App.Client.GPUInstancing.Core.Utils;
using Core.Utils;
using UnityEngine;

namespace App.Client.GPUInstancing.Core.Spatial
{
    enum NodeStatus
    {
        OutOfRange,
        ToBeInstantiated,
        Instantiating,
        Cached
    }

    class NodeIndicator
    {
        public GpuInstancingNode Node;
        public bool IsActive;

        private NodeStatus _status;
        private readonly Vector3 _mins;
        private readonly Vector3 _maxs;

        public bool InLastVisibleSet;
        public bool InCurVisibleSet;

        public bool IsOutOfRange { get { return _status == NodeStatus.OutOfRange; } }
        public bool IsNeedInstantiation { get { return _status == NodeStatus.ToBeInstantiated; } }
        public bool IsDuringInstantiation { get { return _status == NodeStatus.Instantiating; } }
        public bool IsInstantiated { get { return _status == NodeStatus.Cached; } }

        public void SetOutOfRange() { _status = NodeStatus.OutOfRange; }
        public void SetInRange() { _status = NodeStatus.ToBeInstantiated; }
        public void SetInstantiating() { _status = NodeStatus.Instantiating; }
        public void SetInstantiated() { _status = NodeStatus.Cached; }
        public Vector3 Mins { get { return _mins; } }
        public Vector3 Maxs { get { return _maxs; } }

        public ComputeBuffer HeightBuffer()
        {
            return _heightMapProvider.GetHeightmap();
        }

        private readonly IHeightMap _heightMapProvider;

        public NodeIndicator(IHeightMap heightMapProvider, Vector3 mins, Vector3 maxs)
        {
            _status = NodeStatus.OutOfRange;
            InLastVisibleSet = InCurVisibleSet = false;
            _heightMapProvider = heightMapProvider;
            _mins = mins;
            _maxs = maxs;
        }
    }

    interface IHeightMap
    {
        ComputeBuffer GetHeightmap();
    }

    // 为简化当下的复杂度，假设场景中所有Terrain的参数是一致的，如各种Resolution，树和草的Prototype。
    // 如果以后出现了参数不一致的情况，需要进行相应的改动
    class GpuInstancingNodeCluster<T> : IHeightMap where T : GpuInstancingNode
    {
        public string TerrainName;

        private Vector3 _minPosition;
        private Vector2 _nodeSize;

        private int _xCount;
        private int _zCount;

        private NodeIndicator[,] _nodeStatuses;
        private int[] _maxInstanceCountPerRenderInUnit;

        private Vector2 _enableGridDistance;
        private Vector2 _minVisiblePos;
        private Vector2 _maxVisiblePos;
        private readonly Queue<NodeIndicator> _toBeInstantiatedNodes = new Queue<NodeIndicator>();

        private ushort[] _heightMapData;
        private TextAsset _compactHeightMapData;
        private int _compactHeightMapDataStart;
        private int _compactHeightMapDataLength;
        private ComputeBuffer _heightMapBuffer;

        private static float[,] _heightMinsF;
        private static float[,] _heightMaxsF;
        private static ushort[,] _heightMinsU;
        private static ushort[,] _heightMaxsU;
        private Vector3[,] _aabbMins;
        private Vector3[,] _aabbMaxs;

        private int _lastVisibleStartX = int.MinValue;
        private int _lastVisibleStartZ = int.MinValue;
        private int _lastVisibleEndX = int.MinValue;
        private int _lastVisibleEndZ = int.MinValue;
        
        public Vector3 MinPosition { get { return _minPosition; } }
        public int[] MaxInstanceCountPerRenderInUnit { get { return _maxInstanceCountPerRenderInUnit; } }

        private const float MaxHeightInUshort = 32766.0f;

        public void InitDivision(Vector3 baseLocation, Vector2 nodeSize, int xCount, int zCount, float enableDistance)
        {
            _minPosition = baseLocation;
            _nodeSize = nodeSize;

            _xCount = xCount;
            _zCount = zCount;
            
            _nodeStatuses = new NodeIndicator[_xCount, _zCount];
            
            _minVisiblePos = new Vector2(
                _minPosition.x - enableDistance,
                _minPosition.z - enableDistance);
            _maxVisiblePos = new Vector2(
                _minPosition.x + _nodeSize.x * _xCount + enableDistance,
                _minPosition.z + _nodeSize.y * _zCount + enableDistance);
            
            _enableGridDistance = new Vector2(
                enableDistance / _nodeSize.x,
                enableDistance / _nodeSize.y);
        }

        public void InitHeightMap(float[,] heightMapData, float fullHeight)
        {
            if (_heightMinsF == null)
            {
                _heightMinsF = new float[_xCount, _zCount];
                _heightMaxsF = new float[_xCount, _zCount];
            }

            for (int i = 0; i < _xCount; ++i)
            {
                for (int j = 0; j < _zCount; ++j)
                {
                    _heightMinsF[i, j] = float.MaxValue;
                    _heightMaxsF[i, j] = float.MinValue;
                }
            }

            var heightMapXCount = heightMapData.GetLength(0);
            var heightMapZCount = heightMapData.GetLength(1);

            _heightMapData = new ushort[heightMapXCount * heightMapZCount];
            var xPixelsPerUnit = heightMapXCount / (float) _xCount;
            var zPixelsPerUnit = heightMapZCount / (float) _zCount;

            while (Mathf.FloorToInt((heightMapXCount - 1) / xPixelsPerUnit) >= _xCount)
                --heightMapXCount;

            while (Mathf.FloorToInt((heightMapZCount - 1) / zPixelsPerUnit) >= _zCount)
                --heightMapZCount;

            int index = 0;
            for (int j = 0; j < heightMapZCount; ++j)
            {
                for (int i = 0; i < heightMapXCount; ++i)
                {
                    // https://docs.unity3d.com/ScriptReference/TerrainData.GetHeights.html
                    var height = heightMapData[j, i];

                    var x = Mathf.FloorToInt(i / xPixelsPerUnit);
                    var z = Mathf.FloorToInt(j / zPixelsPerUnit);

                    _heightMinsF[x, z] = Mathf.Min(_heightMinsF[x, z], height);
                    _heightMaxsF[x, z] = Mathf.Max(_heightMaxsF[x, z], height);

                    _heightMapData[index++] = (ushort) Mathf.RoundToInt(height * MaxHeightInUshort);
                }
            }

            for (int i = 0; i < _xCount; ++i)
            {
                for (int j = 0; j < _zCount; ++j)
                {
                    _heightMinsF[i, j] *= fullHeight;
                    _heightMaxsF[i, j] *= fullHeight;
                }
            }
            
            BuildAabb(_heightMinsF, _heightMaxsF);
        }

        public void InitHeightMap(TextAsset flowData, int start, float fullHeight, int resolution)
        {
            _compactHeightMapData = flowData;
            _compactHeightMapDataStart = start;
            _compactHeightMapDataLength = resolution * resolution * 2;

            if (_heightMinsU == null)
            {
                _heightMinsU = new ushort[_xCount, _zCount];
                _heightMaxsU = new ushort[_xCount, _zCount];
            }

            if (_heightMinsF == null)
            {
                _heightMinsF = new float[_xCount, _zCount];
                _heightMaxsF = new float[_xCount, _zCount];
            }
            
            for (int i = 0; i < _xCount; ++i)
            {
                for (int j = 0; j < _zCount; ++j)
                {
                    _heightMinsU[i, j] = ushort.MaxValue;
                    _heightMaxsU[i, j] = ushort.MinValue;
                }
            }

            var heightMapXCount = resolution;
            var heightMapZCount = resolution;
            var xPixelsPerUnit = heightMapXCount / (float) _xCount;
            var zPixelsPerUnit = heightMapZCount / (float) _zCount;
            
            while (Mathf.FloorToInt((heightMapXCount - 1) / xPixelsPerUnit) >= _xCount)
                --heightMapXCount;
            
            while (Mathf.FloorToInt((heightMapZCount - 1) / zPixelsPerUnit) >= _zCount)
                --heightMapZCount;

            unsafe
            {
                var head = new IntPtr(_compactHeightMapData.GetBytesIntPtr().ToInt64() + start);
                ushort* data = (ushort*) head.ToPointer();

                for (int i = 0; i < heightMapZCount; ++i)
                {
                    for (int j = 0; j < heightMapXCount; ++j)
                    {
                        var height = *data;
                        data += 1;

                        var x = Mathf.FloorToInt(j / xPixelsPerUnit);
                        var z = Mathf.FloorToInt(i / zPixelsPerUnit);

                        _heightMinsU[x, z] = (ushort) Mathf.Min(_heightMinsU[x, z], height);
                        _heightMaxsU[x, z] = (ushort) Mathf.Max(_heightMaxsU[x, z], height);
                    }
                }
            }

            var coefficient = fullHeight / MaxHeightInUshort;
            for (int i = 0; i < _xCount; ++i)
            {
                for (int j = 0; j < _zCount; ++j)
                {
                    _heightMinsF[i, j] = _heightMinsU[i, j] * coefficient;
                    _heightMaxsF[i, j] = _heightMaxsU[i, j] * coefficient;
                }
            }
                        
            BuildAabb(_heightMinsF, _heightMaxsF);
        }

        public void AddNode(int x, int z, T node)
        {
            _nodeStatuses[x, z] = new NodeIndicator(this, _aabbMins[x, z], _aabbMaxs[x, z]) { Node = node };
        }

        public void UpdateMaxCountInLayer(T node)
        {
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

        public void SetMaxCountInLayer(List<int> maxCountInLayer)
        {
            _maxInstanceCountPerRenderInUnit = maxCountInLayer.ToArray();
        }

        public NodeIndicator DistanceAndFrustumCulling(Vector3 viewPoint, bool enoughRoom)
        {
            HandleHeightBuffer(viewPoint);

            viewPoint.Set(viewPoint.x - _minPosition.x, viewPoint.y - _minPosition.y, viewPoint.z - _minPosition.z);

            int visibleStartX = Mathf.Clamp(Mathf.FloorToInt(viewPoint.x / _nodeSize.x - _enableGridDistance.x), 0, _xCount);
            int visibleStartZ = Mathf.Clamp(Mathf.FloorToInt(viewPoint.z / _nodeSize.y - _enableGridDistance.y), 0, _zCount);
            int visibleEndX = Mathf.Clamp(Mathf.CeilToInt(viewPoint.x / _nodeSize.x + _enableGridDistance.x), 0, _xCount);
            int visibleEndZ = Mathf.Clamp(Mathf.CeilToInt(viewPoint.z / _nodeSize.y + _enableGridDistance.y), 0, _zCount);

            if (!Helper.AlmostEqual(visibleStartX, _lastVisibleStartX) ||
                !Helper.AlmostEqual(visibleEndX, _lastVisibleEndX) ||
                !Helper.AlmostEqual(visibleStartZ, _lastVisibleStartZ) ||
                !Helper.AlmostEqual(visibleEndZ, _lastVisibleEndZ))
            {

                HashSet<NodeIndicator> allRelatedNodes = new HashSet<NodeIndicator>();
                _toBeInstantiatedNodes.Clear();

                for (int i = _lastVisibleStartX; i < _lastVisibleEndX; ++i)
                {
                    for (int j = _lastVisibleStartZ; j < _lastVisibleEndZ; ++j)
                    {
                        if (i >= 0 && i < _xCount && j >= 0 && j < _zCount)
                        {
                            _nodeStatuses[i, j].InLastVisibleSet = true;
                            allRelatedNodes.Add(_nodeStatuses[i, j]);
                        }
                    }
                }

                for (int i = visibleStartX; i < visibleEndX; ++i)
                {
                    for (int j = visibleStartZ; j < visibleEndZ; ++j)
                    {
                        if (i >= 0 && i < _xCount && j >= 0 && j < _zCount)
                        {
                            _nodeStatuses[i, j].InCurVisibleSet = true;
                            allRelatedNodes.Add(_nodeStatuses[i, j]);
                        }
                    }
                }

                foreach (var nodeIndicator in allRelatedNodes)
                {
                    if (nodeIndicator.InLastVisibleSet && !nodeIndicator.InCurVisibleSet)
                    {
                        if (nodeIndicator.IsInstantiated)
                            nodeIndicator.Node.ReleaseBuffer();
                        nodeIndicator.SetOutOfRange();
                    }
                    else if (nodeIndicator.InLastVisibleSet && nodeIndicator.InCurVisibleSet)
                    {
                        if (nodeIndicator.IsNeedInstantiation)
                            _toBeInstantiatedNodes.Enqueue(nodeIndicator);
                    }
                    else if (!nodeIndicator.InLastVisibleSet && nodeIndicator.InCurVisibleSet)
                    {
                        nodeIndicator.SetInRange();
                        _toBeInstantiatedNodes.Enqueue(nodeIndicator);
                    }

                    nodeIndicator.InLastVisibleSet = false;
                    nodeIndicator.InCurVisibleSet = false;
                }

                _lastVisibleStartX = visibleStartX;
                _lastVisibleStartZ = visibleStartZ;
                _lastVisibleEndX = visibleEndX;
                _lastVisibleEndZ = visibleEndZ;
            }

            if (enoughRoom && _toBeInstantiatedNodes.Count > 0)
                return _toBeInstantiatedNodes.Dequeue();
            
            return null;
        }

        public void Clean()
        {
            if (_heightMapBuffer != null)
            {
                _heightMapBuffer.Release();
                _heightMapBuffer = null;
            }

            for (int i = 0; i < _xCount; ++i)
            {
                for (int j = 0; j < _zCount; ++j)
                {
                    _nodeStatuses[i, j].SetOutOfRange();
                    _nodeStatuses[i, j].Node.ReleaseBuffer();
                }
            }
        }

        #region IHeightMap

        public ComputeBuffer GetHeightmap()
        {
            return _heightMapBuffer;
        }

        #endregion

        private void BuildAabb(float[,] mins, float[,] maxs)
        {
            _aabbMins = new Vector3[_xCount, _zCount];
            _aabbMaxs = new Vector3[_xCount, _zCount];

            for (int i = 0; i < _xCount; ++i)
            {
                for (int j = 0; j < _zCount; ++j)
                {
                    _aabbMins[i, j] = new Vector3(_nodeSize.x * i + _minPosition.x,
                        mins[i, j] + _minPosition.y, _nodeSize.y * j + _minPosition.z);
                    _aabbMaxs[i, j] = new Vector3(_nodeSize.x * (i + 1) + _minPosition.x,
                        maxs[i, j] + _minPosition.y, _nodeSize.y * (j + 1) + _minPosition.z);
                }
            }
        }
        
        private void HandleHeightBuffer(Vector3 viewPoint)
        {
            if (viewPoint.x >= _minVisiblePos.x && viewPoint.x <= _maxVisiblePos.x &&
                viewPoint.z >= _minVisiblePos.y && viewPoint.z <= _maxVisiblePos.y)
            {
                if (_heightMapBuffer == null)
                {
                    if (_heightMapData != null)
                    {
                        _heightMapBuffer = new ComputeBuffer((_heightMapData.Length + 1) / 2, Constants.StrideSizeUint);
                        _heightMapBuffer.SetData(_heightMapData);
                    }
                    else
                    {
                        _heightMapBuffer = new ComputeBuffer((_compactHeightMapDataLength + 3) / 4, Constants.StrideSizeUint);
                        IntPtr p = new IntPtr(_compactHeightMapData.GetBytesIntPtr().ToInt64() + _compactHeightMapDataStart);
                        _heightMapBuffer.SetDataWithIntPtr(p, _compactHeightMapDataLength);
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
    }
}