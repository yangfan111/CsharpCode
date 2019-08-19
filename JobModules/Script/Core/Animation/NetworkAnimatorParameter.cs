using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Core.Compare;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;
using Utils.Utils.Buildin;

namespace Core.Animation
{
    public class NetworkAnimatorParameter : IPatchClass<NetworkAnimatorParameter>,IDisposable
    {
        static LoggerAdapter _logger = new LoggerAdapter(typeof(NetworkAnimatorParameter));

        public BitArrayWrapper BitArray { get; set; }
        public AnimatorControllerParameterType ParamType;
        public bool BoolValue;
        public int IntValue;
        public float FloatValue;
        public int NameHash;


        ~NetworkAnimatorParameter()
        {
            Dispose();
        }

        public NetworkAnimatorParameter(AnimatorControllerParameterType type, bool val, int nameHash)
        {
            ParamType = type;
            BoolValue = val;
            NameHash = nameHash;
        }

        public bool HasFieldValue(int index)
        {
            return BitArray[index];
        }

        public void ClearBitArray()
        {
            BitArray.ReleaseReference();
            BitArray = null;
        }
        
        public NetworkAnimatorParameter(AnimatorControllerParameterType type, float val, int nameHash)
        {
            ParamType = type;
            FloatValue = val;
            NameHash = nameHash;
        }

        public NetworkAnimatorParameter(AnimatorControllerParameterType type, int val, int nameHash)
        {
            ParamType = type;
            IntValue = val;
            NameHash = nameHash;
        }

        public NetworkAnimatorParameter()
        {
        }


        public NetworkAnimatorParameter Clone()
        {
            NetworkAnimatorParameter rc = new NetworkAnimatorParameter();
            rc.ParamType = ParamType;
            rc.IntValue = IntValue;
            rc.FloatValue = FloatValue;
            rc.BoolValue = BoolValue;
            rc.NameHash = NameHash;
            return rc;
        }


        public void RewindTo(NetworkAnimatorParameter right)
        {
            ParamType = right.ParamType;
            IntValue = right.IntValue;
            FloatValue = right.FloatValue;
            BoolValue = right.BoolValue;
            NameHash = right.NameHash;
        }

        public bool IsApproximatelyEqual(NetworkAnimatorParameter right)
        {
            return IsApproximatelyEqual(right, 0.001f);
        }

        public bool IsSimilar(NetworkAnimatorParameter right)
        {
            return IsApproximatelyEqual(right, 0.0001f);
        }

        private bool IsApproximatelyEqual(NetworkAnimatorParameter right, float floatEpsilon)
        {
            if (right == null) return false;
            return (ParamType == right.ParamType)
                   && CompareUtility.IsApproximatelyEqual(IntValue, right.IntValue)
                   && CompareUtility.IsApproximatelyEqual(FloatValue, right.FloatValue, floatEpsilon)
                   && CompareUtility.IsApproximatelyEqual(BoolValue, right.BoolValue)
                   && CompareUtility.IsApproximatelyEqual(NameHash, right.NameHash);
        }

        public bool HasValue { get; set; }
        public NetworkAnimatorParameter CreateInstance()
        {
            return new NetworkAnimatorParameter();
        }

        public string GetName()
        {
            return "NetworkAnimatorParameter";
        }

        public void SetParam(AnimatorControllerParameterType type, bool val, int nameHash)
        {
            ParamType = type;
            BoolValue = val;
            NameHash = nameHash;
        }

        public void SetParam(AnimatorControllerParameterType type, int val, int nameHash)
        {
            ParamType = type;
            IntValue = val;
            NameHash = nameHash;
        }

        public void SetParam(AnimatorControllerParameterType type, float val, int nameHash)
        {
            ParamType = type;
            FloatValue = val;
            NameHash = nameHash;
        }

        public override string ToString()
        {
            return string.Format(
                "NetworkAnimatorParameter ParamType: {0}, BoolValue: {1}, IntValue: {2}, FloatValue: {3}, NameHash: {4}",
                ParamType, BoolValue, IntValue, FloatValue, NameHash);
        }

        public void Dispose()
        {
            if (BitArray != null)
            {
                BitArray.ReleaseReference();
            }
        }
        
    }

    public class CompressedNetworkAnimatorParameter : IPatchClass<CompressedNetworkAnimatorParameter>, IDisposable
    {
        public short Value;
        public BitArrayWrapper BitArray { get; set; }
        
        public void RewindTo(CompressedNetworkAnimatorParameter right)
        {
            Value = right.Value;
        }

        public bool IsSimilar(CompressedNetworkAnimatorParameter right)
        {
            return Value == right.Value;
        }

        public CompressedNetworkAnimatorParameter Clone()
        {
            CompressedNetworkAnimatorParameter ret = new CompressedNetworkAnimatorParameter();
            ret.Value = Value;

            return ret;
        }

        public bool HasValue { get; set; }
        public CompressedNetworkAnimatorParameter CreateInstance()
        {
            return new CompressedNetworkAnimatorParameter();
        }

        public string GetName()
        {
            return "CompressedNetworkAnimatorParameter";
        }

        public void Dispose()
        {
            if (BitArray != null)
            {
                BitArray.ReleaseReference();
            }
        }
    }
}