using System;
using System.IO;
using Core.Compare;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;
using Utils.Utils.Buildin;

namespace Core.Animation
{
    public class NetworkAnimatorLayer : IPatchClass<NetworkAnimatorLayer>, IDisposable
    {
        public const float NotInTransition = -1;

        private BitArrayWrapper _bitArray;
        public const int PlayerSyncLayer = 1;
        public const int PlayerUpperBodyOverlayLayer = 4;
        public const int PlayerUpperBodyAddLayer = 5;
        public const int FirstPersonIKPassLayer = 3;
        public const int ThirdPersonIKPassLayer = 8;

        public int LayerIndex;
        public float Weight;
        public int CurrentStateHash;
        public float NormalizedTime;
        public float StateDuration;
        public float TransitionNormalizedTime;
        public float TransitionDuration;

        public NetworkAnimatorLayer()
        {
            TransitionNormalizedTime = NotInTransition;
        }

        ~NetworkAnimatorLayer()
        {
            Dispose();
        }

        public NetworkAnimatorLayer(int syncLayerIndex)
        {
            LayerIndex = syncLayerIndex;
            TransitionNormalizedTime = NotInTransition;
        }

        public void RewindTo(NetworkAnimatorLayer right)
        {
            SetCurrentStateInfo(right.LayerIndex,
                right.Weight,
                right.CurrentStateHash,
                right.NormalizedTime,
                right.StateDuration,
                right.TransitionNormalizedTime,
                right.TransitionDuration);
        }

        public void CopyFrom(NetworkAnimatorLayer right)
        {
            RewindTo(right);
        }

        public bool IsSimilar(NetworkAnimatorLayer right)
        {
            return IsApproximatelyEqual(right);
        }

        public bool IsApproximatelyEqual(NetworkAnimatorLayer right)
        {
            if (right == null) return false;
            return CompareUtility.IsApproximatelyEqual(LayerIndex, right.LayerIndex)
                   && CompareUtility.IsApproximatelyEqual(Weight, right.Weight)
                   && CompareUtility.IsApproximatelyEqual(CurrentStateHash, right.CurrentStateHash)
                   && CompareUtility.IsApproximatelyEqual(NormalizedTime, right.NormalizedTime)
                   && CompareUtility.IsApproximatelyEqual(StateDuration, right.StateDuration)
                   && CompareUtility.IsApproximatelyEqual(TransitionNormalizedTime, right.TransitionNormalizedTime)
                   && CompareUtility.IsApproximatelyEqual(TransitionDuration, right.TransitionDuration);
        }

        #region Serialization

        public NetworkAnimatorLayer Clone()
        {
            var rc = new NetworkAnimatorLayer();
            rc.RewindTo(this);
            return rc;
        }


        public void Read(BinaryReader reader)
        {
            if (_bitArray != null) _bitArray.ReleaseReference();
            _bitArray = reader.ReadBitArray();
            LayerIndex = _bitArray[0] ? reader.ReadInt32() : LayerIndex;
            Weight = _bitArray[1] ? reader.ReadSingle() : Weight;
            CurrentStateHash = _bitArray[2] ? reader.ReadInt32() : CurrentStateHash;
            NormalizedTime = _bitArray[3] ? reader.ReadSingle() : NormalizedTime;
            StateDuration = _bitArray[4] ? reader.ReadSingle() : StateDuration;
            TransitionNormalizedTime = _bitArray[5] ? reader.ReadSingle() : TransitionNormalizedTime;
            TransitionDuration = _bitArray[6] ? reader.ReadSingle() : TransitionDuration;
        }

        public void Write(NetworkAnimatorLayer right, MyBinaryWriter writer)
        {
            BitArrayWrapper bitArray = BitArrayWrapper.Allocate(7, false);

            if (right == null)
            {
                bitArray.SetAll(true);
            }
            else
            {
                bitArray[0] = !CompareUtility.IsApproximatelyEqual(LayerIndex, right.LayerIndex);
                bitArray[1] = Weight != right.Weight;
                bitArray[2] = !CompareUtility.IsApproximatelyEqual(CurrentStateHash, right.CurrentStateHash);
                bitArray[3] = NormalizedTime != right.NormalizedTime;
                bitArray[4] = StateDuration != right.StateDuration;
                bitArray[5] = TransitionNormalizedTime != right.TransitionNormalizedTime;
                bitArray[6] = TransitionDuration != right.TransitionDuration;
            }

            writer.Write(bitArray);
            if (bitArray[0]) writer.Write(LayerIndex);
            if (bitArray[1]) writer.Write(Weight);
            if (bitArray[2]) writer.Write(CurrentStateHash);
            if (bitArray[3]) writer.Write(NormalizedTime);
            if (bitArray[4]) writer.Write(StateDuration);
            if (bitArray[5]) writer.Write(TransitionNormalizedTime);
            if (bitArray[6]) writer.Write(TransitionDuration);


            bitArray.ReleaseReference();
        }

        public void MergeFromPatch(NetworkAnimatorLayer from)
        {
            LayerIndex = from._bitArray[0] ? from.LayerIndex : LayerIndex;
            Weight = from._bitArray[1] ? from.Weight : Weight;
            CurrentStateHash = from._bitArray[2] ? from.CurrentStateHash : CurrentStateHash;
            NormalizedTime = from._bitArray[3] ? from.NormalizedTime : NormalizedTime;
            StateDuration = from._bitArray[4] ? from.StateDuration : StateDuration;
            TransitionNormalizedTime = from._bitArray[5] ? from.TransitionNormalizedTime : TransitionNormalizedTime;
            TransitionDuration = from._bitArray[6] ? from.TransitionDuration : TransitionDuration;
           
            from._bitArray.ReleaseReference();
            from._bitArray = null;
            
        }

        public bool HasValue { get; set; }

        #endregion

        public NetworkAnimatorLayer(int layerIndex, float weight, int currentStateHash, float normalizedTime,
            float stateDuration, float transitionNormalizedTime, float transitionDuration)
        {
            SetCurrentStateInfo(layerIndex, weight, currentStateHash, normalizedTime, stateDuration,
                transitionNormalizedTime, transitionDuration);
        }

        public void SetCurrentStateInfo(int layerIndex, float weight, int currentStateHash, float normalizedTime,
            float stateDuration, float transitionNormalizedTime, float transitionDuration)
        {
            LayerIndex = layerIndex;
            Weight = weight;
            CurrentStateHash = currentStateHash;
            NormalizedTime = normalizedTime;
            StateDuration = stateDuration;
            TransitionNormalizedTime = transitionNormalizedTime;
            TransitionDuration = transitionDuration;
        }

        public override string ToString()
        {
            return string.Format(
                "NetworkAnimatorLayer LayerIndex: {0}, Weight: {1}, CurrentStateHash: {2}, NormalizedTime: {3}, StateDuration: {4}, TransitionNormalizedTIme: {5}, TransitionDuration: {6}",
                LayerIndex, Weight, CurrentStateHash, NormalizedTime, StateDuration, TransitionNormalizedTime,
                TransitionDuration);
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