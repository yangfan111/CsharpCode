using System;
using System.Diagnostics;
using UnityEngine;

namespace Core.SnapshotReplication.Serialization.NetworkProperty
{
    public enum SyncFieldScale
    {
        Position,
        Pitch,
        Yaw,
        Roll,
        stage,
        Quaternion,
        EularAngle,
        PositiveShort,
        PositiveInt,
    }
    
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

        public NetworkPropertyAttribute(SyncFieldScale type)
        {
            switch (type)
            {
                case SyncFieldScale.stage:
                    Max = 256;
                    Min = 0;
                    Deviation = 1;
                    break;
                case SyncFieldScale.Yaw:
                    Max = 180;
                    Min = -180;
                    Deviation = 0.001f;
                    break;
                case SyncFieldScale.Pitch:
                    Max = 90;
                    Min = -90;
                    Deviation = 0.001f;
                    break;
                case SyncFieldScale.Roll:
                    Max = 180;
                    Min = -180;
                    Deviation = 0.01f;
                    break;
                case SyncFieldScale.Position:
                    Max = 4000;
                    Min = -4000;
                    Deviation = 0.01f;
                    break;
                case SyncFieldScale.Quaternion:
                    Max = 1;
                    Min = 0;
                    Deviation = 0.0001f;
                    break;
                case SyncFieldScale.EularAngle:
                    Max = 360;
                    Min = -360;
                    Deviation = 0.01f;
                    break;
                case SyncFieldScale.PositiveShort:
                    Max = short.MaxValue;
                    Min = 0;
                    Deviation = 1;
                    break;
                case SyncFieldScale.PositiveInt:
                    Max = int.MaxValue;
                    Min = 0;
                    Deviation = 1;
                    break;
                default:
                    Min = Max = Deviation = float.MaxValue;
                    break;
            }
        }
    }
}
