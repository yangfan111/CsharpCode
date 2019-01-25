using Core.EntityComponent;
using UnityEngine;

namespace Core.Components
{
    
    public class GlobalFlagComponent : IGameComponent
    {
        public int GetComponentId()
        {
            return (int) ECoreComponentIds.GlobalFlag;
        }
    }

    public enum PositionFilterType
    {
        Filter2D,
        Filter3D
    }

    
    public class PositionFilterComponent : IGameComponent
    {
        public PositionFilterType Type;
        public float Distance;

        public int GetComponentId()
        {
            return (int) ECoreComponentIds.PositionFilter;
        }

        public bool Filter(Vector3 local, Vector3 remote)
        {
            if (PositionFilterType.Filter2D == Type)
            {
                var x = remote.x - local.x;
                var z = remote.z - local.z;
                var sd = x * x + z * z;
                return sd > Distance * Distance;
            }
            else
            {
                var x = remote.x - local.x;
                var y = remote.y - local.y;
                var z = remote.z - local.z;
                var sd = x * x + y * y + z * z;
                return sd > Distance * Distance;
            }
        }
    }
}