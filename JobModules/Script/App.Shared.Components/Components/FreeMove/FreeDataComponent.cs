using Entitas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Components;
using Core.Free;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Core.Playback;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.FreeMove
{
    [FreeMove]
    
    public class FreeDataComponent : IPlaybackComponent
    {
        [NetworkProperty]
        public string Key;

        [NetworkProperty]
        [DontInitilize]
        public string Cat;

        [NetworkProperty]
        [DontInitilize]
        public string Value;

        [NetworkProperty]
        [DontInitilize]
        public int IntValue;

        [NetworkProperty]
        [DontInitilize]
        public Vector3 Scale;

        public IFreeData FreeData;

        public int GetComponentId()
        {
            return  (int)EComponentIds.FreeMoveKey;
        }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }

        public void CopyFrom(object rightComponent)
        {
            FreeDataComponent right = (FreeDataComponent) rightComponent;
            Key = right.Key;
            Cat = right.Cat;
            Value = right.Value;
            IntValue = right.IntValue;
            Scale = right.Scale;
        }
       
    }
}
