﻿using System;
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

        public const int LadderLayer = 1;
        public const int PlayerSyncLayer = 2;
        public const int PlayerUpperBodyOverlayLayer = 5;
        public const int PlayerUpperBodyAddLayer = 6;
        public const int FirstPersonIKPassLayer = 6;
        public const int ThirdPersonIKPassLayer = 9;

        public int LayerIndex;
        public float Weight;
        public int CurrentStateFullPathHash;
        public float CurrentStateNormalizedTime;
        public int TransitionFullPathHash;
        public byte TransitionIndex;
        public float TransitionStartTime;
        public float NextStateNormalizedTime;
        public float TransitionNormalizedTime;
        public float StateDuration;

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
                right.CurrentStateFullPathHash,
                right.CurrentStateNormalizedTime,
                right.TransitionFullPathHash,
                right.TransitionIndex,
                right.TransitionStartTime,
                right.NextStateNormalizedTime,
                right.TransitionNormalizedTime,
                right.StateDuration);
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
                   && CompareUtility.IsApproximatelyEqual(CurrentStateFullPathHash, right.CurrentStateFullPathHash)
                   && CompareUtility.IsApproximatelyEqual(CurrentStateNormalizedTime, right.CurrentStateNormalizedTime)
                   && CompareUtility.IsApproximatelyEqual(TransitionFullPathHash, right.TransitionFullPathHash)
                   && CompareUtility.IsApproximatelyEqual(TransitionIndex, right.TransitionIndex)
                   && CompareUtility.IsApproximatelyEqual(TransitionStartTime, right.TransitionStartTime)
                   && CompareUtility.IsApproximatelyEqual(NextStateNormalizedTime, right.NextStateNormalizedTime)
                   && CompareUtility.IsApproximatelyEqual(TransitionNormalizedTime, right.TransitionNormalizedTime)
                   && CompareUtility.IsApproximatelyEqual(StateDuration, right.StateDuration);
        }

        #region Serialization

        public NetworkAnimatorLayer Clone()
        {
            var rc = new NetworkAnimatorLayer();
            rc.RewindTo(this);
            return rc;
        }

        public bool HasValue { get; set; }
        public NetworkAnimatorLayer CreateInstance()
        {
            return new NetworkAnimatorLayer();
        }

        public string GetName()
        {
            return "NetworkAnimatorLayer";
        }

        public BitArrayWrapper BitArray { get; set; }

        #endregion

        public NetworkAnimatorLayer(int layerIndex, float weight, int currentStateFullPathHash, float currentStateNormalizedTime,
            int transitionFullPathHash, byte transitionIndex, float transitionStartTime, float nextStateNormalizedTime,
            float transitionNormalizedTime, float stateDuration)
        {
            SetCurrentStateInfo(layerIndex, weight, currentStateFullPathHash, currentStateNormalizedTime, transitionFullPathHash,
                transitionIndex, transitionStartTime, nextStateNormalizedTime, transitionNormalizedTime, stateDuration);
        }

        public void SetCurrentStateInfo(int layerIndex, float weight, int currentStateFullPathHash, float currentStateNormalizedTime,
            int transitionFullPathHash, byte transitionIndex, float transitionStartTime, float nextStateNormalizedTime,
            float transitionNormalizedTime, float stateDuration)
        {
            LayerIndex = layerIndex;
            Weight = weight;
            CurrentStateFullPathHash = currentStateFullPathHash;
            CurrentStateNormalizedTime = currentStateNormalizedTime;
            TransitionFullPathHash = transitionFullPathHash;
            TransitionIndex = transitionIndex;
            TransitionStartTime = transitionStartTime;
            NextStateNormalizedTime = nextStateNormalizedTime;
            TransitionNormalizedTime = transitionNormalizedTime;
            StateDuration = stateDuration;
        }

        public override string ToString()
        {
            return string.Format(
                "NetworkAnimatorLayer LayerIndex: {0}, Weight: {1}, CurrentStateFullPathHash: {2}, CurrentStateNormalizedTime: {3}, " +
                "TransitionFullPathHash: {4}, NextStateNormalizedTime: {5}, TransitionNormalizedTime: {6}, TransitionStartTime: {7}, " +
                "TransitionIndex: {8}",
                LayerIndex, Weight, CurrentStateFullPathHash, CurrentStateNormalizedTime, TransitionFullPathHash,
                NextStateNormalizedTime, TransitionNormalizedTime, TransitionStartTime, TransitionIndex);
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
