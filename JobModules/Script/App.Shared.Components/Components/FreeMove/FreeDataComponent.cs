﻿using Entitas;
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
using Core.Interpolate;

namespace App.Shared.Components.FreeMove
{
    [FreeMove]
    
    public class FreeDataComponent : IPlaybackComponent, IRule
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
        public float ScaleX;

        [NetworkProperty]
        [DontInitilize]
        public float ScaleY;

        [NetworkProperty]
        [DontInitilize]
        public float ScaleZ;

        public IFreeData FreeData;

        public int GetComponentId()
        {
            return  (int)EComponentIds.FreeMoveKey;
        }
        public bool IsInterpolateEveryFrame(){ return true; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
            ScaleX = InterpolateUtility.Interpolate(((FreeDataComponent)left).ScaleX, ((FreeDataComponent)right).ScaleX, interpolationInfo.RatioWithOutClamp);
            ScaleY = InterpolateUtility.Interpolate(((FreeDataComponent)left).ScaleY, ((FreeDataComponent)right).ScaleY, interpolationInfo.RatioWithOutClamp);
            ScaleZ = InterpolateUtility.Interpolate(((FreeDataComponent)left).ScaleZ, ((FreeDataComponent)right).ScaleZ, interpolationInfo.RatioWithOutClamp);
        }

        public void CopyFrom(object rightComponent)
        {
            FreeDataComponent right = (FreeDataComponent) rightComponent;
            Key = right.Key;
            Cat = right.Cat;
            Value = right.Value;
            IntValue = right.IntValue;
            ScaleX = right.ScaleX;
            ScaleY = right.ScaleY;
            ScaleZ = right.ScaleZ;
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.FreeDataComponent;
        }
    }
}
