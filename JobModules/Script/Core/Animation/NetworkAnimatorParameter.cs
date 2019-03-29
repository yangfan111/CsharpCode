using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Core.Compare;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;
using Utils.Utils.Buildin;

namespace Core.Animation
{
    public class NetworkAnimatorParameter : IPatchClass<NetworkAnimatorParameter>,IDisposable
    {
        static LoggerAdapter _logger = new LoggerAdapter(typeof(NetworkAnimatorParameter));

        private BitArrayWrapper _bitArray;
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


        public void  MergeFromPatch(NetworkAnimatorParameter from)
        {
           
            ParamType = from._bitArray[0] ? from.ParamType : ParamType;
            NameHash = from._bitArray[1] ? from.NameHash : NameHash;
            BoolValue = from._bitArray[2] ? from.BoolValue : BoolValue;
            FloatValue = from._bitArray[3] ? from.FloatValue : FloatValue;
            IntValue = from._bitArray[4] ? from.IntValue : IntValue;
            from._bitArray.ReleaseReference();
            from._bitArray = null;
        }

        public bool HasValue { get; set; }

        public void Write(NetworkAnimatorParameter right, MyBinaryWriter writer)
        {
            BitArrayWrapper bitArray = BitArrayWrapper.Allocate(5, false);
            //  return new BitArray(5, true);
            if (right == null)
            {
                bitArray.SetAll(true);
            }
            else
            {
                bitArray[0] = ParamType != right.ParamType;
                bitArray[1] = !CompareUtility.IsApproximatelyEqual(NameHash, right.NameHash);
                bitArray[2] = !CompareUtility.IsApproximatelyEqual(BoolValue, right.BoolValue);
                bitArray[3] = !CompareUtility.IsApproximatelyEqual(FloatValue, right.FloatValue, 0.0001f);
                bitArray[4] = !CompareUtility.IsApproximatelyEqual(IntValue, right.IntValue);
            }

            writer.Write(bitArray);
            if (bitArray[0]) writer.Write((byte) ParamType);
            if (bitArray[1]) writer.Write(NameHash);
            if (bitArray[2]) writer.Write(BoolValue);
            if (bitArray[3]) writer.Write(FloatValue);
            if (bitArray[4]) writer.Write(IntValue);


            bitArray.ReleaseReference();
        }

        public void Read(BinaryReader reader)
        {
            if (_bitArray != null)
            {
                _bitArray.ReleaseReference();
            }
            _bitArray = reader.ReadBitArray();
            ParamType = _bitArray[0] ? (AnimatorControllerParameterType) reader.ReadByte() : ParamType;
            NameHash = _bitArray[1] ? reader.ReadInt32() : NameHash;
            BoolValue = _bitArray[2] ? reader.ReadBoolean() : BoolValue;
            FloatValue = _bitArray[3] ? reader.ReadSingle() : FloatValue;
            IntValue = _bitArray[4] ? reader.ReadInt32() : IntValue;
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
            if (_bitArray != null)
            {
                _bitArray.ReleaseReference();
            }
        }
        
    }
}