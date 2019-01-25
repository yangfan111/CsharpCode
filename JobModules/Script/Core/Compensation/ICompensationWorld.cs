using System.Collections.Generic;
using Core.EntityComponent;
using UnityEngine;

namespace Core.Compensation
{
    public interface ICompensationWorld
    {
        int ServerTime { get; }
        EntityKey Self { get; set; }
        List<int> ExcludePlayerList { get; set; } 
        bool Raycast(RaySegment ray, out RaycastHit hitInfo, int hitboxLayerMask);
        bool BoxCast(BoxInfo box, out RaycastHit hitInfo, int hitboxLayerMask);
        bool TryGetEntityPosition(EntityKey key, out Vector3 pos);
        void Release();
    }
}