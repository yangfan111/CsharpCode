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

    public class GpuInstancingNodeIndicator
    {
        internal GpuInstancingNode Node;
        public bool IsActive;

        private NodeStatus _status;
        private readonly Vector3 _mins;
        private readonly Vector3 _maxs;
        private readonly Vector3[] _vertices;

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
        public Vector3[] Vertices { get { return _vertices; } }
        public int LastOutsidePlaneIndex = 0;

        public ComputeBuffer HeightBuffer()
        {
            return _heightMapProvider.GetHeightmap();
        }

        private readonly IHeightMap _heightMapProvider;

        public GpuInstancingNodeIndicator(IHeightMap heightMapProvider, Vector3 mins, Vector3 maxs)
        {
            _status = NodeStatus.OutOfRange;
            InLastVisibleSet = InCurVisibleSet = false;
            _heightMapProvider = heightMapProvider;

            _vertices = new[]
            {
                new Vector3(mins.x, mins.y, mins.z),
                new Vector3(maxs.x, mins.y, mins.z),
                new Vector3(mins.x, maxs.y, mins.z),
                new Vector3(maxs.x, maxs.y, mins.z),
                new Vector3(mins.x, mins.y, maxs.z),
                new Vector3(maxs.x, mins.y, maxs.z),
                new Vector3(mins.x, maxs.y, maxs.z),
                new Vector3(maxs.x, maxs.y, maxs.z)
            };
        }
    }
}