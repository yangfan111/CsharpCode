using System;
using UnityEngine;

namespace Core.SnapshotReplication.Serialization.NetworkProperty
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NetworkPropertyAttribute : Attribute
    {
        public float Max;
        public float Min;
        public float Deviation;

        public NetworkPropertyAttribute()
        {
            Min = Max = Deviation = float.MaxValue;
        }

        public NetworkPropertyAttribute(int max,int min)
        {
            Max = (float)max;
            Min = (float)min;
            Deviation = 1;
        }
       
        public NetworkPropertyAttribute(float max, float min, float deviation = 1f)
        {
            Max = max;
            Min = min;
            Deviation = deviation;
        }
    }
}
