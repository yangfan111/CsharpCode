using System.Collections.Generic;
using Core;
using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Weapon
{
    
    /// <summary>
    /// 手雷背包缓存
    /// </summary>
    [Player,]
    public class GrenadeCacheDataComponent : IUserPredictionComponent
    {
        [NetworkProperty]                public List<GrenadeCacheData> GrenadeArr;
        [NetworkProperty, DontInitilize] public int                    LastId;

        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerGrenadeCache;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var comp = right as GrenadeCacheDataComponent;
            for (int i = 0; i < GrenadeArr.Count; i++)
            {
                if (!GrenadeArr[i].IsSimilar(comp.GrenadeArr[i]))
                    return false;
            }

            return LastId == comp.LastId;
        }

        public void InternalInitialize(int length)
        {
            if (GrenadeArr == null)
            {
                GrenadeArr = new List<GrenadeCacheData>(length);
                for (int i = 0; i < length; i++)
                {
                    GrenadeArr.Add(new GrenadeCacheData());
                }
            }
        }

        public void CopyFrom(object rightComponent)
        {
            var comp   = rightComponent as GrenadeCacheDataComponent;
            var length = comp.GrenadeArr.Count;
            InternalInitialize(length);
            for (int i = 0; i < length; i++)
            {
                GrenadeArr[i].RewindTo(comp.GrenadeArr[i]);
            }

            LastId = comp.LastId;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }
}